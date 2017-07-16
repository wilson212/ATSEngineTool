using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    /// <summary>
    /// This object is used to interact with the SQLite database "/data/AppData.db"
    /// which contains all the application data for this program
    /// </summary>
    public class AppDatabase : CrossLite.SQLiteContext
    {
        /// <summary>
        /// Contains the Connection string needed to create and connect
        /// to the application's SQLite database
        /// </summary>
        protected static SQLiteConnectionStringBuilder Builder;

        /// <summary>
        /// Gets the latest database version
        /// </summary>
        public static Version CurrentVersion { get; protected set; } = new Version(2, 0);

        /// <summary>
        /// Gets the current database tables version
        /// </summary>
        public static Version DatabaseVersion { get; protected set; }

        #region Database Entity Sets

        protected DbSet<DbVersion> DbVersions { get; set; }

        public DbSet<Engine> Engines { get; protected set; }

        public DbSet<EngineSeries> EngineSeries { get; protected set; }

        public DbSet<EngineSoundPackage> EngineSoundPackages { get; protected set; }

        public DbSet<EngineSound> EngineSounds { get; protected set; }

        public DbSet<TruckSoundPackage> TruckSoundPackages { get; protected set; }

        public DbSet<TruckSound> TruckSounds { get; protected set; }

        public DbSet<TruckSoundSetting> TruckSoundSettings { get; protected set; }

        public DbSet<TruckEngine> TruckEngines { get; protected set; }

        public DbSet<TorqueRatio> TorqueRatios { get; protected set; }

        public DbSet<TransmissionSeries> TransmissionSeries { get; protected set; }

        public DbSet<Transmission> Transmissions { get; protected set; }

        public DbSet<AccessoryConflict> AccessoryConflicts { get; protected set; }

        public DbSet<SuitableAccessory> SuitableAccessories { get; protected set; }

        public DbSet<TransmissionGear> TransmissionGears { get; protected set; }

        public DbSet<TruckTransmission> TruckTransmissions { get; protected set; }

        public DbSet<Truck> Trucks { get; protected set; }

        #endregion Database Entity Sets

        /// <summary>
        /// Static Constructor
        /// </summary>
        static AppDatabase()
        {
            string database = Path.Combine(Program.RootPath, "data", "AppData.db");
            Builder = new SQLiteConnectionStringBuilder();
            Builder.DataSource = database;
            Builder.ForeignKeys = true;
            Builder.JournalMode = SQLiteJournalModeEnum.Wal;
        }

        /// <summary>
        /// Creates a new connection to the SQLite Database
        /// </summary>
        public AppDatabase() : base(Builder)
        {
            // Open connection first
            base.Connect();
            bool build = false;

            // Grab the current tables version
            if (DatabaseVersion == null)
            {
                try
                {
                    GetVersion();
                }
                catch (SQLiteException e) when (e.Message.Contains("no such table"))
                {
                    // Rebuild database tables
                    BuildTables();
                    build = true;

                    // Try 1 last time to get the Version
                    GetVersion();
                }
            }

            // Create Database Sets
            DbVersions = new DbSet<DbVersion>(this);
            Engines = new DbSet<Engine>(this);
            EngineSeries = new DbSet<EngineSeries>(this);
            EngineSoundPackages = new DbSet<EngineSoundPackage>(this);
            EngineSounds = new DbSet<EngineSound>(this);
            TruckSoundPackages = new DbSet<TruckSoundPackage>(this);
            TruckSoundSettings = new DbSet<TruckSoundSetting>(this);
            TruckSounds = new DbSet<TruckSound>(this);
            TruckEngines = new DbSet<TruckEngine>(this);
            TorqueRatios = new DbSet<TorqueRatio>(this);
            TransmissionSeries = new DbSet<TransmissionSeries>(this);
            Transmissions = new DbSet<Transmission>(this);
            AccessoryConflicts = new DbSet<AccessoryConflict>(this);
            SuitableAccessories = new DbSet<SuitableAccessory>(this);
            TransmissionGears = new DbSet<TransmissionGear>(this);
            TruckTransmissions = new DbSet<TruckTransmission>(this);
            Trucks = new DbSet<Truck>(this);

            // Migrations
            MigrationWizard wizard = new MigrationWizard(this);
            wizard.MigrateTables();

            if (build)
                InsertDefaultData();
        }

        internal void GetVersion()
        {
            // Grab version. Plain SQL query here for performance
            string query = "SELECT * FROM DbVersion ORDER BY UpdateId DESC LIMIT 1";
            DbVersion row = Query<DbVersion>(query).FirstOrDefault();

            // If row is null, then the table exists, but was truncated
            if (row == null)
                throw new Exception("DbVersion table is empty");

            // Set instance database version
            DatabaseVersion = row.Version;
        }

        /// <summary>
        /// Performs a VACUUM on the database
        /// </summary>
        /// <seealso cref="https://sqlite.org/lang_vacuum.html"/>
        public void VacuumDatabase()
        {
            Execute("VACUUM;");
        }

        /// <summary>
        /// Performs an integrity check on the database, and returns the
        /// number of issues found.
        /// </summary>
        /// <returns></returns>
        internal int PerformIntegrityCheck()
        {
            // Log any integrity errors in the database
            var results = Query("PRAGMA integrity_check;").ToList();
            if (results.Count > 0 && results[0]["integrity_check"].ToString() != "ok")
            {
                LogErrors(results, "IntegrityErrors.log");
                return results.Count;
            }

            return 0;
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
        /// Drops all tables from the database, and the creates new
        /// tables.
        /// </summary>
        protected void BuildTables()
        {            
            // Wrap in a transaction
            using (SQLiteTransaction tr = base.BeginTransaction())
            {
                // Delete old table rementants
                CodeFirstSQLite.DropTable<TruckEngine>(this);
                CodeFirstSQLite.DropTable<TruckTransmission>(this);
                CodeFirstSQLite.DropTable<TransmissionGear>(this);
                CodeFirstSQLite.DropTable<AccessoryConflict>(this);
                CodeFirstSQLite.DropTable<SuitableAccessory>(this);
                CodeFirstSQLite.DropTable<Transmission>(this);
                CodeFirstSQLite.DropTable<TransmissionSeries>(this);
                CodeFirstSQLite.DropTable<TorqueRatio>(this);
                CodeFirstSQLite.DropTable<Engine>(this);
                CodeFirstSQLite.DropTable<EngineSeries>(this);
                CodeFirstSQLite.DropTable<EngineSound>(this);
                CodeFirstSQLite.DropTable<EngineSoundPackage>(this);
                CodeFirstSQLite.DropTable<TruckSound>(this);
                CodeFirstSQLite.DropTable<TruckSoundPackage>(this);
                CodeFirstSQLite.DropTable<TruckSoundSetting>(this);
                CodeFirstSQLite.DropTable<Truck>(this);
                CodeFirstSQLite.DropTable<DbVersion>(this);

                // Create the needed database tables
                CodeFirstSQLite.CreateTable<DbVersion>(this);
                CodeFirstSQLite.CreateTable<TruckSoundPackage>(this);
                CodeFirstSQLite.CreateTable<TruckSound>(this);
                CodeFirstSQLite.CreateTable<EngineSeries>(this);
                CodeFirstSQLite.CreateTable<EngineSoundPackage>(this);
                CodeFirstSQLite.CreateTable<EngineSound>(this);
                CodeFirstSQLite.CreateTable<Engine>(this);
                CodeFirstSQLite.CreateTable<Truck>(this);
                CodeFirstSQLite.CreateTable<TorqueRatio>(this);
                CodeFirstSQLite.CreateTable<TruckEngine>(this);
                CodeFirstSQLite.CreateTable<TransmissionSeries>(this);
                CodeFirstSQLite.CreateTable<Transmission>(this);
                CodeFirstSQLite.CreateTable<SuitableAccessory>(this);
                CodeFirstSQLite.CreateTable<AccessoryConflict>(this);
                CodeFirstSQLite.CreateTable<TransmissionGear>(this);
                CodeFirstSQLite.CreateTable<TruckTransmission>(this);
                CodeFirstSQLite.CreateTable<TruckSoundSetting>(this);

                // Create version record
                DbVersion version = new DbVersion();
                version.Version = CurrentVersion;
                version.AppliedOn = DateTime.Now;

                DbVersions = new DbSet<DbVersion>(this);
                DbVersions.Add(version);

                // Commit the transaction
                tr.Commit();
            }
        }

        private void InsertDefaultData()
        {
            // Run the update in a transaction
            using (var trans = BeginTransaction())
            {
                try
                {
                    // ==== Add truck sound packs!
                    var package1 = new TruckSoundPackage()
                    {
                        Author = "SCS",
                        Version = "1.6.2.4",
                        Name = "SCS Default Sounds (579/680)",
                        UnitName = "std",
                        FolderName = "default"
                    };
                    TruckSoundPackages.Add(package1);

                    // === Import default sound package data
                    using (Stream stream = Program.GetResource("ATSEngineTool.Resources.Default.tspack"))
                    using (var reader = new SoundPackageReader(stream, SoundType.Truck))
                    {
                        // Save sounds in the database
                        string folderPath = Path.Combine(Program.RootPath, "sounds", package1.RelativeSystemPath);
                        reader.InstallPackage(this, package1, folderPath, true);
                    }

                    // w900 Sound Package
                    var package2 = new TruckSoundPackage()
                    {
                        Author = "SCS",
                        Version = "1.6.2.4",
                        Name = "SCS Default Sounds (w900)",
                        UnitName = "std",
                        FolderName = "default.w900"
                    };
                    TruckSoundPackages.Add(package2);

                    // === Import w900 sound package data
                    using (Stream stream = Program.GetResource("ATSEngineTool.Resources.Default.w900.tspack"))
                    using (var reader = new SoundPackageReader(stream, SoundType.Truck))
                    {
                        // Save sounds in the database
                        string folderPath = Path.Combine(Program.RootPath, "sounds", package2.RelativeSystemPath);
                        reader.InstallPackage(this, package2, folderPath, true);
                    }

                    // 389 Sound Package
                    var package3 = new TruckSoundPackage()
                    {
                        Author = "SCS",
                        Version = "1.6.2.4",
                        Name = "SCS Default Sounds (389)",
                        UnitName = "std",
                        FolderName = "default.389"
                    };
                    TruckSoundPackages.Add(package3);

                    // === Import 389 sound package data
                    using (Stream stream = Program.GetResource("ATSEngineTool.Resources.Default.389.tspack"))
                    using (var reader = new SoundPackageReader(stream, SoundType.Truck))
                    {
                        // Save sounds in the database
                        string folderPath = Path.Combine(Program.RootPath, "sounds", package3.RelativeSystemPath);
                        reader.InstallPackage(this, package3, folderPath, true);
                    }

                    // ==== Add trucks to the database
                    Trucks.Add(new Truck()
                    {
                        Name = "Kenworth t680",
                        UnitName = "kenworth.t680",
                        SoundPackageId = package1.Id,
                        IsScsTruck = true
                    });
                    Trucks.Add(new Truck()
                    {
                        Name = "Peterbilt 579",
                        UnitName = "peterbilt.579",
                        SoundPackageId = package1.Id,
                        IsScsTruck = true
                    });
                    Trucks.Add(new Truck()
                    {
                        Name = "Kenworth w900",
                        UnitName = "kenworth.w900",
                        SoundPackageId = package2.Id,
                        IsScsTruck = true
                    });
                    Trucks.Add(new Truck()
                    {
                        Name = "Peterbilt 389",
                        UnitName = "peterbilt.389",
                        SoundPackageId = package3.Id,
                        IsScsTruck = true
                    });

                    // === Import Transmissions
                    AddTransmissionData();

                    // === Update database version
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {

                }
            }
        }

        /// <summary>
        /// This method adds the default SCS Transmissions to the database
        /// </summary>
        private void AddTransmissionData()
        {
            // == Add Transmission Series
            TransmissionSeries.Add(new TransmissionSeries() { Name = "SCS Eaton Fuller" });
            TransmissionSeries.Add(new TransmissionSeries() { Name = "SCS Allison 4500" });

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
            Transmissions.Add(transmission);

            // Create Gears
            TransmissionGears.AddRange(
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
            Transmissions.Add(transmission);

            // Create Gears
            TransmissionGears.AddRange(
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
            Transmissions.Add(transmission);

            // Create Gears
            TransmissionGears.AddRange(
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
            Transmissions.Add(transmission);

            // Create Gears
            TransmissionGears.AddRange(
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
            Transmissions.Add(transmission);

            // Create Gears
            TransmissionGears.AddRange(
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
            Transmissions.Add(transmission);

            // Create Gears
            TransmissionGears.AddRange(
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
            Transmissions.Add(transmission);

            // Create Gears
            TransmissionGears.AddRange(
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
            Transmissions.Add(transmission);

            // Create Gears
            TransmissionGears.AddRange(
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
