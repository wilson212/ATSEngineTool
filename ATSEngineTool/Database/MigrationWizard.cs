using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    /// <summary>
    /// This class is used to migrate changes to the AppData.db database
    /// </summary>
    internal class MigrationWizard
    {
        protected AppDatabase Database { get; set; }

        public MigrationWizard(AppDatabase db)
        {
            Database = db;
        }

        /// <summary>
        /// Migrates the database tables to the latest version
        /// </summary>
        internal void MigrateTables()
        {
            if (AppDatabase.CurrentVersion != AppDatabase.DatabaseVersion)
            {
                // Create backup
                File.Copy(
                    Path.Combine(Program.RootPath, "data", "AppData.db"),
                    Path.Combine(Program.RootPath, "data", "backups", 
                        $"AppData_v{AppDatabase.DatabaseVersion}_{Epoch.Now}.db"
                    )
                );

                // Perform updates until we are caught up!
                while (AppDatabase.CurrentVersion != AppDatabase.DatabaseVersion)
                {
                    switch (AppDatabase.DatabaseVersion.ToString())
                    {
                        case "1.0":
                            MigrateTo_1_1();
                            break;
                        case "1.1":
                            MigrateTo_1_2();
                            break;
                        case "1.2":
                            MigrateTo_1_3();
                            break;
                        case "1.3":
                            MigrateTo_1_4();
                            break;
                        case "1.4":
                            MigrateTo_1_5();
                            break;
                        case "1.5":
                            MigrateTo_1_6();
                            break;
                        case "1.6":
                            MigrateTo_1_7();
                            break;
                        default:
                            throw new Exception($"Unexpected database version: {AppDatabase.DatabaseVersion}");
                    }

                    // Fetch version
                    Database.GetVersion();
                }

                // Always perform a vacuum to optimize the database
                Database.Execute("VACUUM;");
            }
        }

        /// <summary>
        /// Performs an integrity check on the database, and returns the
        /// number of issues found.
        /// </summary>
        /// <returns></returns>
        internal int PerformIntegrityCheck()
        {
            // Log any integrity errors in the database
            var results = Database.Query("PRAGMA integrity_check;").ToList();
            if (results.Count > 0 && results[0]["integrity_check"].ToString() != "ok")
            {
                LogErrors(results, "IntegrityErrors.log");
                return results.Count;
            }

            return 0;
        }

        /// <summary>
        /// Performs a VACUUM on the database
        /// </summary>
        /// <seealso cref="https://sqlite.org/lang_vacuum.html"/>
        internal void VacuumDatabase()
        {
            Database.Execute("VACUUM;");
        }

        /// <summary>
        /// Migrates to 1.7, which added the default sound package field for the Truck table.
        /// </summary>
        private void MigrateTo_1_7()
        {
            // Run the update in a transaction
            using (var trans = Database.BeginTransaction())
            {
                try
                {
                    // === Add new field to the Truck table
                    Database.Execute("ALTER TABLE `Truck` ADD COLUMN `DefaultSoundPackageId` INTEGER NOT NULL DEFAULT 0;");

                    //
                    // ================= Update the 389 =================
                    //
                    // === Grab the default sound package for the 389
                    var package = Database.Query<SoundPackage>(
                            "SELECT * FROM `SoundPackage` WHERE `FolderName`=@P0", "default.389"
                        ).FirstOrDefault();

                    // === Update the 389
                    var truck = Database.Query<Truck>("SELECT * FROM `Truck` WHERE `UnitName`=@P0", "peterbilt.389").FirstOrDefault();
                    if (package != null && truck != null)
                        Database.Execute("UPDATE `Truck` SET `DefaultSoundPackageId`=@P0 WHERE `Id`=@P1;", package.Id, truck.Id);

                    //
                    // ================= Update the w900 =================
                    //
                    // === Grab the default sound package for the 389
                    package = Database.Query<SoundPackage>(
                            "SELECT * FROM `SoundPackage` WHERE `FolderName`=@P0", "default.w900"
                        ).FirstOrDefault();

                    // === Update the w900 default sound package
                    truck = Database.Query<Truck>("SELECT * FROM `Truck` WHERE `UnitName`=@P0", "kenworth.w900").FirstOrDefault();
                    if (package != null && truck != null)
                        Database.Execute("UPDATE `Truck` SET `DefaultSoundPackageId`=@P0 WHERE `Id`=@P1;", package.Id, truck.Id);

                    //
                    // ==== Update database version
                    //
                    string sql = "INSERT INTO `DbVersion`(`Version`, `AppliedOn`) VALUES({0}, {1});";
                    Database.Execute(String.Format(sql, Version.Parse("1.7"), Epoch.Now));

                    // Commit changes
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    // Log any integrity errors in the database
                    PerformIntegrityCheck();
                }
            }
        }

        /// <summary>
        /// Migrates to 1.6, which added the new Peterbilt 389 data.
        /// </summary>
        /// <remarks>This method has been revised to include the 1.7 update as well.</remarks>
        private void MigrateTo_1_6()
        {
            // Run the update in a transaction
            using (var trans = Database.BeginTransaction())
            {
                try
                {
                    // === Add field for 1.7 update.. we need it here
                    Database.Execute("ALTER TABLE `Truck` ADD COLUMN `DefaultSoundPackageId` INTEGER NOT NULL DEFAULT 0;");

                    // === Import sound package
                    var package = new SoundPackage()
                    {
                        Author = "SCS",
                        Version = 1.4m,
                        Name = "Default Engine Sounds (389)",
                        UnitName = "std",
                        FolderName = "default.389",
                        InteriorFileName = "interior.sii",
                        ExteriorFileName = "exterior.sii"
                    };
                    Database.SoundPackages.Add(package);

                    // Set variables
                    string folderPath = Path.Combine(Program.RootPath, "sounds", "engine", "default.389");

                    // === Import sound package data
                    using (Stream stream = Program.GetResource("ATSEngineTool.Resources.Default.389.espack"))
                    using (var reader = new SoundPackageReader(stream))
                    {
                        // Parse sii files
                        var interior = reader.GetSoundFile(SoundType.Interior);
                        var exterior = reader.GetSoundFile(SoundType.Exterior);

                        // Extract data
                        reader.ExtractToDirectory(folderPath, true);

                        // Save sounds in the database
                        reader.ImportSounds(Database, package, interior, SoundType.Interior);
                        reader.ImportSounds(Database, package, exterior, SoundType.Exterior);
                    }

                    // ==== Add truck to the database
                    var truck = new Truck()
                    {
                        Name = "Peterbilt 389",
                        UnitName = "peterbilt.389",
                        DefaultSoundPackageId = package.Id,
                        IsScsTruck = true
                    };
                    Database.Trucks.Add(truck);

                    // === Import existing SCS engines to the 389
                    string[] units = { "isx12", "isx15", "mx", "mx_500" }; 
                    foreach (string e in units)
                    {
                        var eng = Database.Query<Engine>("SELECT * FROM `Engine` WHERE `UnitName`=@P0", e).FirstOrDefault();
                        if (eng != null)
                        {
                            Database.TruckEngines.Add(new TruckEngine() { Truck = truck, Engine = eng });
                        }
                    }

                    // ================= Update the w900 (1.7) =================
                    //
                    // === Grab the default sound package for the 389
                    package = Database.Query<SoundPackage>(
                            "SELECT * FROM `SoundPackage` WHERE `FolderName`=@P0", "default.w900"
                        ).FirstOrDefault();

                    // === Update the w900 default sound package
                    truck = Database.Query<Truck>("SELECT * FROM `Truck` WHERE `UnitName`=@P0", "kenworth.w900").FirstOrDefault();
                    if (package != null && truck != null)
                        Database.Execute("UPDATE `Truck` SET `DefaultSoundPackageId`=@P0 WHERE `Id`=@P1;", package.Id, truck.Id);
                    // ================= End w900 Update =================

                    // === Update database version
                    string sql = "INSERT INTO `DbVersion`(`Version`, `AppliedOn`) VALUES({0}, {1});";
                    Database.Execute(String.Format(sql, Version.Parse("1.6"), Epoch.Now));
                    Database.Execute(String.Format(sql, Version.Parse("1.7"), Epoch.Now));
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    // Log any integrity errors in the database
                    PerformIntegrityCheck();
                }
            }
        }

        /// <summary>
        /// Migrates to 1.5, which added new Foreign key restraints on most tables.
        /// </summary>
        private void MigrateTo_1_5()
        {
            // === Turn off foreign keys
            Database.Execute("PRAGMA foreign_keys = OFF;");

            // === Recreate tables to enforce data integrity
            // Run the update in a transaction
            using (var trans = Database.BeginTransaction())
            {
                try
                {
                    // Recreate tables that don't have the ON UPDATE
                    // integrity check on foreign keys
                    RecreateTable<Engine>();
                    RecreateTable<EngineSeries>();
                    RecreateTable<EngineSound>();
                    RecreateTable<AccessoryConflict>();
                    RecreateTable<SuitableAccessory>();
                    RecreateTable<TorqueRatio>();
                    RecreateTable<Transmission>();
                    RecreateTable<TransmissionSeries>();

                    // === Turn off foreign keys
                    Database.Execute("PRAGMA foreign_keys = ON;");

                    // == foreign key check
                    var results = Database.Query($"PRAGMA foreign_key_check;").ToList();
                    if (results.Count > 0)
                    {
                        // Houston, we have a problem!
                        LogErrors(results, "ForeignKeyErrors.log");
                        throw new Exception("Foreign key check failed!");
                    }

                    // Update database version
                    string sql = "INSERT INTO `DbVersion`(`Version`, `AppliedOn`) VALUES({0}, {1});";
                    Database.Execute(String.Format(sql, Version.Parse("1.5"), Epoch.Now));

                    // Commit changes
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    // Log any integrity errors in the database
                    PerformIntegrityCheck();
                }
            }
        }

        /// <summary>
        /// Migrates to 1.4, which added EngineSounds to the database, and re-creates
        /// the `SoundPackage` table.
        /// </summary>
        private void MigrateTo_1_4()
        {
            // === Turn off foreign keys
            Database.Execute("PRAGMA foreign_keys = OFF;");

            string fileLoc = Path.Combine(Program.RootPath, "data", "Migration_14.db");
            bool attach = File.Exists(fileLoc);

            // Run the update in a transaction
            using (var trans = Database.BeginTransaction())
            {
                // Delete old table rementants
                Database.DropTable<EngineSound>();
                Database.DropTable<SoundPackage>();

                // Create the needed database tables
                Database.CreateTable<SoundPackage>();
                Database.CreateTable<EngineSound>();

                // Fix cummins ISX12 litre size
                var series = Database.Query<EngineSeries>("SELECT * FROM `EngineSeries` WHERE Name='ISX12'").FirstOrDefault();
                if (series != null)
                {
                    series.Displacement = 11.9m;
                    Database.EngineSeries.Update(series);
                }

                // If we are not attaching the migration database
                if (!attach)
                {
                    // Add default sounds
                    var package1 = new SoundPackage()
                    {
                        Author = "SCS",
                        Version = 1.4m,
                        Name = "Default Engine Sounds",
                        UnitName = "std",
                        FolderName = "default",
                        InteriorFileName = "interior.sii",
                        ExteriorFileName = "exterior.sii"
                    };
                    var package2 = new SoundPackage()
                    {
                        Author = "SCS",
                        Version = 1.4m,
                        Name = "Default Engine Sounds (w900)",
                        UnitName = "std",
                        FolderName = "default.w900",
                        InteriorFileName = "interior.sii",
                        ExteriorFileName = "exterior.sii"
                    };
                    Database.SoundPackages.Add(package1);
                    Database.SoundPackages.Add(package2);

                    // Set variables
                    string folderPath = Path.Combine(Program.RootPath, "sounds", "engine", "default");

                    //Insert package 1 stuff
                    using (Stream stream = Program.GetResource("ATSEngineTool.Resources.Default.espack"))
                    using (var reader = new SoundPackageReader(stream))
                    {
                        // Parse sii files
                        var interior = reader.GetSoundFile(SoundType.Interior);
                        var exterior = reader.GetSoundFile(SoundType.Exterior);

                        // Extract data
                        reader.ExtractToDirectory(folderPath, true);

                        // Save sounds in the database
                        reader.ImportSounds(Database, package1, interior, SoundType.Interior);
                        reader.ImportSounds(Database, package1, exterior, SoundType.Exterior);
                    }

                    // Set variables
                    folderPath = Path.Combine(Program.RootPath, "sounds", "engine", "default.w900");

                    //Insert package 1 stuff
                    using (Stream stream = Program.GetResource("ATSEngineTool.Resources.Default.w900.espack"))
                    using (var reader = new SoundPackageReader(stream))
                    {
                        // Parse sii files
                        var interior = reader.GetSoundFile(SoundType.Interior);
                        var exterior = reader.GetSoundFile(SoundType.Exterior);

                        // Extract data
                        reader.ExtractToDirectory(folderPath, true);

                        // Save sounds in the database
                        reader.ImportSounds(Database, package2, interior, SoundType.Interior);
                        reader.ImportSounds(Database, package2, exterior, SoundType.Exterior);
                    }

                    // Reset engine series sounds before turning foreign keys back on
                    foreach (var s in Database.EngineSeries)
                    {
                        s.SoundId = (s.SoundId == 2) ? package2.Id : package1.Id;
                        Database.EngineSeries.Update(s);
                    }
                }

                // Commit
                trans.Commit();
            }

            // If we are attaching the migration database
            if (attach)
            {
                // Begin migrating
                Database.Execute($"ATTACH '{fileLoc}' AS MI;");
                Database.Execute("INSERT INTO `SoundPackage` SELECT * FROM MI.SoundPackage");
                Database.Execute("INSERT INTO `EngineSound` SELECT * FROM MI.EngineSound");
                Database.Execute("DETACH MI;");

                // Make sure no new packages will cause a FKey error
                Database.Execute("UPDATE `EngineSeries` SET `SoundId`=1 WHERE `SoundId` > 7");
            }

            // Update database version
            string sql = "INSERT INTO `DbVersion`(`Version`, `AppliedOn`) VALUES({0}, {1});";
            Database.Execute(String.Format(sql, Version.Parse("1.4"), Epoch.Now));

            // == Enable foreign keys again
            Database.Execute("PRAGMA foreign_keys = ON;");
        }

        /// <summary>
        /// Migrates to 1.3, which added an Engine/Transmission Conflict table
        /// </summary>
        private void MigrateTo_1_3()
        {
            // Run the update in a transaction
            using (var trans = Database.BeginTransaction())
            {
                // Create the `TransmissionConflict` table
                Database.CreateTable<AccessoryConflict>();

                // Create the `SuitableAccessory` table
                Database.CreateTable<SuitableAccessory>();

                // Add the `SuitableFor` column to the `Engine` table
                Database.Execute("ALTER TABLE `Engine` ADD COLUMN `SuitableFor` TEXT DEFAULT \"\";");

                // Update database version
                string sql = "INSERT INTO `DbVersion`(`Version`, `AppliedOn`) VALUES({0}, {1});";
                Database.Execute(String.Format(sql, Version.Parse("1.3"), Epoch.Now));

                // Commit
                trans.Commit();
            }
        }

        /// <summary>
        /// Migrates to 1.2, which added transmissions to the program
        /// </summary>
        private void MigrateTo_1_2()
        {
            // Run the update in a transaction
            using (var trans = Database.BeginTransaction())
            {
                // Create Tables
                Database.CreateTable<TransmissionSeries>();
                Database.CreateTable<Transmission>();
                Database.CreateTable<TransmissionGear>();
                Database.CreateTable<TruckTransmission>();

                // == Add Transmission Data
                AddTransmissionData();

                // Update database version
                string sql = "INSERT INTO `DbVersion`(`Version`, `AppliedOn`) VALUES({0}, {1});";
                Database.Execute(String.Format(sql, Version.Parse("1.2"), Epoch.Now));

                // Commit
                trans.Commit();
            }
        }

        /// <summary>
        /// Migrates to 1.1, which added more field options to Engines
        /// </summary>
        private void MigrateTo_1_1()
        {
            // Run the update in a transaction
            using (var trans = Database.BeginTransaction())
            {
                // Create queries
                string[] queries = new[]
                {
                    "ALTER TABLE `Engine` ADD COLUMN `LowRpmRange_EngineBrake` INTEGER NOT NULL DEFAULT 0;",
                    "ALTER TABLE `Engine` ADD COLUMN `HighRpmRange_EngineBrake` INTEGER NOT NULL DEFAULT 0;",
                    "ALTER TABLE `Engine` ADD COLUMN `AdblueConsumption` REAL NOT NULL DEFAULT 0.0;",
                    "ALTER TABLE `Engine` ADD COLUMN `NoAdbluePowerLimit` REAL NOT NULL DEFAULT 0.0;",
                    "ALTER TABLE `Engine` ADD COLUMN `Conflicts` TEXT DEFAULT \"\";",
                };

                // Run each query
                foreach (string query in queries)
                {
                    Database.Execute(query);
                }

                // Update database version
                string sql = "INSERT INTO `DbVersion`(`Version`, `AppliedOn`) VALUES({0}, {1});";
                Database.Execute(String.Format(sql, Version.Parse("1.1"), Epoch.Now));

                // Commit
                trans.Commit();
            }
        }

        /// <summary>
        /// Logs the results of a foreign_key_check or integrity_check
        /// </summary>
        /// <param name="results"></param>
        /// <param name="fileName"></param>
        private void LogErrors(List<Dictionary<string, object>> results, string fileName)
        {
            // Ensure our directory exists
            string directory = Path.Combine(Program.RootPath, "errors");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create the log file
            string path = Path.Combine(Program.RootPath, "errors", fileName);
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                int i = 1;
                foreach (var item in results)
                {
                    writer.WriteLine("Error #" + i++);
                    foreach (string key in item.Keys)
                    {
                        string value = item[key].ToString();
                        writer.WriteLine($"\t{key} = {value}");
                    }
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// This method is used to perform a mass-migration on a table in the database.
        /// Essentially, this method renames the table, creates a new table using the same
        /// name, and copies all the data from the old table to the new.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void RecreateTable<T>() where T : class
        {
            // Get the in-memory table mapping
            TableMapping table = EntityCache.GetTableMap(typeof(T));

            // Rename table
            var newName = table.TableName + "_old";
            Database.Execute($"ALTER TABLE `{table.TableName}` RENAME TO `{newName}`");

            // Create new table
            Database.CreateTable<T>();

            // Select from old table, and import to the new table
            // NOTE: had to do this the slow way because LowRpmRange_EngineBrake kept
            // throwing a constraing failure (not null).
            var items = Database.Query<T>($"SELECT * FROM `{newName}`");
            var set = new DbSet<T>(Database);
            set.AddRange(items);

            // Drop old table
            Database.Execute($"DROP TABLE `{newName}`");
        }

        /// <summary>
        /// This method adds the default SCS Transmissions to the database
        /// </summary>
        private void AddTransmissionData()
        {
            // == Add Transmission Series
            Database.TransmissionSeries.Add(new TransmissionSeries() { Name = "SCS Eaton Fuller" });
            Database.TransmissionSeries.Add(new TransmissionSeries() { Name = "SCS Allison 4500" });

            #region Add Default Data

            // == Eaton Fuller 10-speed
            var transmission = default(Transmission);
            transmission = new Transmission()
            {
                SeriesId = 1,
                UnitName = "10_speed",
                Name = "Eaton Fuller 10-speed",
                Price = 8750,
                Unlock = 0,
                DifferentialRatio = 2.85m
            };
            Database.Transmissions.Add(transmission);

            // Create Gears
            Database.TransmissionGears.AddRange(
                new TransmissionGear() { Transmission = transmission, GearIndex = 0, Ratio = -18.18m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 1, Ratio = -3.89m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 2, Ratio = 15.42m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 3, Ratio = 11.52m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 4, Ratio = 8.55m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 5, Ratio = 6.28m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 6, Ratio = 4.67m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 7, Ratio = 3.3m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 8, Ratio = 2.46m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 9, Ratio = 1.83m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 10, Ratio = 1.34m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 11, Ratio = 1m }
            );

            // == Eaton Fuller 10-speed Retarder
            transmission = new Transmission()
            {
                SeriesId = 1,
                UnitName = "10_speed_r",
                Name = "Eaton Fuller 10-speed Retarder",
                Price = 10220,
                Unlock = 2,
                DifferentialRatio = 2.85m,
                Retarder = 3,
                FileName = "10_speed_retarder"
            };
            Database.Transmissions.Add(transmission);

            // Create Gears
            Database.TransmissionGears.AddRange(
                new TransmissionGear() { Transmission = transmission, GearIndex = 0, Ratio = -18.18m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 1, Ratio = -3.89m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 2, Ratio = 15.42m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 3, Ratio = 11.52m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 4, Ratio = 8.55m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 5, Ratio = 6.28m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 6, Ratio = 4.67m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 7, Ratio = 3.3m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 8, Ratio = 2.46m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 9, Ratio = 1.83m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 10, Ratio = 1.34m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 11, Ratio = 1m }
            );

            // == Eaton Fuller 13-speed
            transmission = new Transmission()
            {
                SeriesId = 1,
                UnitName = "13_speed",
                Name = "Eaton Fuller 13-speed",
                Price = 9510,
                Unlock = 4,
                DifferentialRatio = 3.55m
            };
            Database.Transmissions.Add(transmission);

            // Create Gears
            Database.TransmissionGears.AddRange(
                new TransmissionGear() { Transmission = transmission, GearIndex = 0, Ratio = -15.06m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 1, Ratio = -12.85m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 2, Ratio = -4.03m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 3, Ratio = 12.29m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 4, Ratio = 8.51m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 5, Ratio = 6.05m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 6, Ratio = 4.38m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 7, Ratio = 3.2m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 8, Ratio = 2.29m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 9, Ratio = 1.95m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 10, Ratio = 1.62m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 11, Ratio = 1.38m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 12, Ratio = 1.17m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 13, Ratio = 1m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 14, Ratio = 0.86m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 15, Ratio = 0.73m }
            );

            // == Eaton Fuller 13-speed Retarder
            transmission = new Transmission()
            {
                SeriesId = 1,
                UnitName = "13_speed_r",
                Name = "Eaton Fuller 13-speed Retarder",
                Price = 12830,
                Unlock = 6,
                DifferentialRatio = 3.55m,
                Retarder = 3,
                FileName = "13_speed_retarder"
            };
            Database.Transmissions.Add(transmission);

            // Create Gears
            Database.TransmissionGears.AddRange(
                new TransmissionGear() { Transmission = transmission, GearIndex = 0, Ratio = -15.06m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 1, Ratio = -12.85m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 2, Ratio = -4.03m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 3, Ratio = 12.29m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 4, Ratio = 8.51m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 5, Ratio = 6.05m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 6, Ratio = 4.38m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 7, Ratio = 3.2m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 8, Ratio = 2.29m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 9, Ratio = 1.95m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 10, Ratio = 1.62m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 11, Ratio = 1.38m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 12, Ratio = 1.17m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 13, Ratio = 1m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 14, Ratio = 0.86m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 15, Ratio = 0.73m }
            );

            // == Eaton Fuller 18-speed
            transmission = new Transmission()
            {
                SeriesId = 1,
                UnitName = "18_speed",
                Name = "Eaton Fuller 18-speed",
                Price = 11120,
                Unlock = 9,
                DifferentialRatio = 3.25m
            };
            Database.Transmissions.Add(transmission);

            // Create Gears
            Database.TransmissionGears.AddRange(
                new TransmissionGear() { Transmission = transmission, GearIndex = 0, Ratio = -15.06m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 1, Ratio = -12.85m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 2, Ratio = -4.03m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 3, Ratio = -3.43m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 4, Ratio = 14.40m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 5, Ratio = 12.29m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 6, Ratio = 8.51m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 7, Ratio = 7.26m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 8, Ratio = 6.05m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 9, Ratio = 5.16m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 10, Ratio = 4.38m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 11, Ratio = 3.74m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 12, Ratio = 3.2m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 13, Ratio = 2.73m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 14, Ratio = 2.28m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 15, Ratio = 1.94m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 16, Ratio = 1.62m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 17, Ratio = 1.38m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 18, Ratio = 1.17m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 19, Ratio = 1.0m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 20, Ratio = 0.86m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 21, Ratio = 0.73m }
            );

            // == Eaton Fuller 18-speed Retarder
            transmission = new Transmission()
            {
                SeriesId = 1,
                UnitName = "18_speed_r",
                Name = "Eaton Fuller 18-speed",
                Price = 14250,
                Unlock = 12,
                DifferentialRatio = 3.25m,
                Retarder = 3,
                FileName = "18_speed_retarder"
            };
            Database.Transmissions.Add(transmission);

            // Create Gears
            Database.TransmissionGears.AddRange(
                new TransmissionGear() { Transmission = transmission, GearIndex = 0, Ratio = -15.06m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 1, Ratio = -12.85m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 2, Ratio = -4.03m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 3, Ratio = -3.43m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 4, Ratio = 14.40m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 5, Ratio = 12.29m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 6, Ratio = 8.51m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 7, Ratio = 7.26m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 8, Ratio = 6.05m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 9, Ratio = 5.16m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 10, Ratio = 4.38m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 11, Ratio = 3.74m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 12, Ratio = 3.2m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 13, Ratio = 2.73m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 14, Ratio = 2.28m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 15, Ratio = 1.94m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 16, Ratio = 1.62m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 17, Ratio = 1.38m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 18, Ratio = 1.17m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 19, Ratio = 1.0m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 20, Ratio = 0.86m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 21, Ratio = 0.73m }
            );

            // == Allison 4500 6 Speed
            transmission = new Transmission()
            {
                SeriesId = 2,
                UnitName = "allison",
                Name = "Allison 4500 6-speed",
                Price = 12430,
                Unlock = 14,
                DifferentialRatio = 3.7m,
                StallTorqueRatio = 2.42m
            };
            Database.Transmissions.Add(transmission);

            // Create Gears
            Database.TransmissionGears.AddRange(
                new TransmissionGear() { Transmission = transmission, GearIndex = 0, Ratio = -5.55m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 1, Ratio = 4.7m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 2, Ratio = 2.21m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 3, Ratio = 1.53m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 4, Ratio = 1.0m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 5, Ratio = 0.76m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 6, Ratio = 0.67m }
            );

            // == Allison 4500 6 Speed Retarder
            transmission = new Transmission()
            {
                SeriesId = 2,
                UnitName = "allison_r",
                Name = "Allison 4500 6-speed Retarder",
                Price = 15577,
                Unlock = 18,
                DifferentialRatio = 3.7m,
                StallTorqueRatio = 2.42m,
                Retarder = 3,
                FileName = "allison_retarder"
            };
            Database.Transmissions.Add(transmission);

            // Create Gears
            Database.TransmissionGears.AddRange(
                new TransmissionGear() { Transmission = transmission, GearIndex = 0, Ratio = -5.55m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 1, Ratio = 4.7m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 2, Ratio = 2.21m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 3, Ratio = 1.53m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 4, Ratio = 1.0m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 5, Ratio = 0.76m },
                new TransmissionGear() { Transmission = transmission, GearIndex = 6, Ratio = 0.67m }
            );

            #endregion
        }
    }
}
