using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossLite;

namespace ATSEngineTool.Database
{
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
            while (AppDatabase.CurrentVersion != AppDatabase.DatabaseVersion)
            {
                switch (AppDatabase.DatabaseVersion.ToString())
                {
                    case "1.0":
                        MigrateTo_1_1();
                        break;
                    default:
                        throw new Exception("Version out of range");
                }

                // Fetch version
                Database.GetVersion();
            }
        }

        private void MigrateTo_1_1()
        {
            // Create backup
            File.Copy(
                Path.Combine(Program.RootPath, "data", "AppData.db"),
                Path.Combine(Program.RootPath, "data", "backups", $"AppData_v1.0_{Epoch.Now}.db")
            );

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
        /// Populates the database with the default data (SCS default data)
        /// </summary>
        public void InitializeData()
        {
            using (SQLiteTransaction trans = Database.BeginTransaction())
            {
                // === Create sounds === //

                Database.EngineSounds.Add(new SoundPackage()
                {
                    Name = "Default 579/t680",
                    FolderName = "default",
                    InteriorFileName = "interior.sii",
                    ExteriorFileName = "exterior.sii"
                });

                Database.EngineSounds.Add(new SoundPackage()
                {
                    Name = "Default W900",
                    FolderName = "default.w900",
                    InteriorFileName = "interior.sii",
                    ExteriorFileName = "exterior.sii"
                });

                Database.EngineSounds.Add(new SoundPackage()
                {
                    Name = "CAT 3406e",
                    FolderName = "3406E",
                    InteriorFileName = "interior_cat3406.sii",
                    ExteriorFileName = "exterior_cat3406.sii"
                });

                Database.EngineSounds.Add(new SoundPackage()
                {
                    Name = "CAT C15",
                    FolderName = "C15",
                    InteriorFileName = "interior_c15.sii",
                    ExteriorFileName = "exterior_c15.sii"
                });

                Database.EngineSounds.Add(new SoundPackage()
                {
                    Name = "Cummins ISX",
                    FolderName = "ISX",
                    InteriorFileName = "interior_isx.sii",
                    ExteriorFileName = "exterior_isx.sii"
                });

                Database.EngineSounds.Add(new SoundPackage()
                {
                    Name = "Cummins N14",
                    FolderName = "N14",
                    InteriorFileName = "interior_n14.sii",
                    ExteriorFileName = "exterior_n14.sii"
                });

                Database.EngineSounds.Add(new SoundPackage()
                {
                    Name = "Paccar MX-13",
                    FolderName = "MX13",
                    InteriorFileName = "interior_mx13.sii",
                    ExteriorFileName = "exterior_mx13.sii"
                });

                // === Create Trucks === //

                var truck1 = new Truck()
                {
                    Name = "Peterbilt 579",
                    UnitName = "peterbilt.579",
                    IsScsTruck = true
                };
                Database.Trucks.Add(truck1);

                var truck2 = new Truck()
                {
                    Name = "Kenworth t680",
                    UnitName = "kenworth.t680",
                    IsScsTruck = true
                };
                Database.Trucks.Add(truck2);

                var truck3 = new Truck()
                {
                    Name = "Kenworth w900",
                    UnitName = "kenworth.w900",
                    IsScsTruck = true
                };
                Database.Trucks.Add(truck3);

                // === Create Engine Brands === //

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "SCS",
                    Name = "Paccar MX-13",
                    Displacement = 12.9m,
                    EngineIcon = "engine_01",
                    SoundId = 1
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "SCS",
                    Name = "Cummins ISX12",
                    Displacement = 11.9m,
                    EngineIcon = "engine_01",
                    SoundId = 1
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "SCS",
                    Name = "Cummins ISX15",
                    Displacement = 14.9m,
                    EngineIcon = "engine_01",
                    SoundId = 1
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "SCS",
                    Name = "Caterpillar C15",
                    Displacement = 15.2m,
                    EngineIcon = "engine_01",
                    SoundId = 2
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "Caterpillar",
                    Name = "3406E",
                    Displacement = 14.6m,
                    EngineIcon = "cat__3406",
                    SoundId = 3
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "Caterpillar",
                    Name = "C15",
                    Displacement = 15.2m,
                    EngineIcon = "engcat_01",
                    SoundId = 4
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "Cummins",
                    Name = "ISX12",
                    Displacement = 11.9m,
                    EngineIcon = "engisx_02",
                    SoundId = 5
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "Cummins",
                    Name = "ISX15",
                    Displacement = 14.9m,
                    EngineIcon = "engisx_02",
                    SoundId = 5
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "Cummins",
                    Name = "N14",
                    Displacement = 14m,
                    EngineIcon = "engn14_01",
                    SoundId = 6
                });

                Database.EngineSeries.Add(new EngineSeries()
                {
                    Manufacturer = "Paccar",
                    Name = "MX-13",
                    Displacement = 12.9m,
                    EngineIcon = "engine_mx",
                    SoundId = 7
                });

                // Commit here 
                //trans.Commit();

                // === Create Engines! === //
                Engine engine = new Engine()
                {
                    SeriesId = 1,
                    Name = "Paccar MX-13 (SCS)",
                    UnitName = "mx",
                    Price = 47150,
                    Unlock = 6,
                    Horsepower = 450,
                    Torque = 1650,
                    RpmLimit = 2200,
                    IdleRpm = 650,
                    RpmLimitNeutral = 2200,
                    PeakRpm = 1000
                };
                Database.Engines.Add(engine);

                // Add Engine torque curves
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 300, Ratio = 0m});
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 440, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1100, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1400, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1900, Ratio = 0.77m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2400, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2600, Ratio = 0m });

                // Add engine to all 3 trucks
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck1 });
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck2 });
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck3 });

                // MX-13 500
                engine = new Engine()
                {
                    SeriesId = 1,
                    Name = "Paccar MX-13 (SCS)",
                    UnitName = "mx_500",
                    Price = 48870,
                    Unlock = 12,
                    Horsepower = 500,
                    Torque = 1850,
                    RpmLimit = 2200,
                    IdleRpm = 650,
                    RpmLimitNeutral = 2200,
                    PeakRpm = 1000
                };
                Database.Engines.Add(engine);

                // Add Engine torque curves
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 300, Ratio = 0m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 440, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1100, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1400, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1900, Ratio = 0.77m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2400, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2600, Ratio = 0m });

                // Add engine to 579 / t680
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck1 });
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck2 });

                // ISX12
                engine = new Engine()
                {
                    SeriesId = 2,
                    Name = "Cummins ISX 12 (SCS)",
                    UnitName = "isx12",
                    Price = 44650,
                    Unlock = 0,
                    Horsepower = 370,
                    Torque = 1350,
                    RpmLimit = 2200,
                    IdleRpm = 650,
                    RpmLimitNeutral = 1600,
                    PeakRpm = 1100
                };
                Database.Engines.Add(engine);

                // Add Engine torque curves
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 300, Ratio = 0m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 440, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1100, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1400, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1900, Ratio = 0.77m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2400, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2600, Ratio = 0m });

                // Add engine to all 3 trucks
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck1 });
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck2 });
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck3 });

                // ISX15
                engine = new Engine()
                {
                    SeriesId = 3,
                    Name = "Cummins ISX 15 (SCS)",
                    UnitName = "isx15",
                    Price = 49510,
                    Unlock = 18,
                    Horsepower = 550,
                    Torque = 1850,
                    RpmLimit = 2200,
                    IdleRpm = 650,
                    RpmLimitNeutral = 1600,
                    PeakRpm = 1100
                };
                Database.Engines.Add(engine);

                // Add Engine torque curves
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 300, Ratio = 0m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 440, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1100, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1400, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1900, Ratio = 0.77m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2400, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2600, Ratio = 0m });

                // Add engine to all 3 trucks
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck1 });
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck2 });
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck3 });

                // ISX15 600 (w900 only)
                engine = new Engine()
                {
                    SeriesId = 3,
                    Name = "Cummins ISX 15 (SCS)",
                    UnitName = "isx15_600",
                    Price = 50320,
                    Unlock = 16,
                    Horsepower = 600,
                    Torque = 2050,
                    RpmLimit = 2200,
                    IdleRpm = 650,
                    RpmLimitNeutral = 1600,
                    PeakRpm = 1200
                };
                Database.Engines.Add(engine);

                // Add Engine torque curves
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 300, Ratio = 0m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 440, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1100, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1400, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1900, Ratio = 0.77m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2400, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2600, Ratio = 0m });

                // Add engine to the w900
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck3 });

                // C15 600 (w900 only)
                engine = new Engine()
                {
                    SeriesId = 4,
                    Name = "Caterpillar C15 (SCS)",
                    UnitName = "catc15",
                    Price = 51860,
                    Unlock = 18,
                    Horsepower = 625,
                    Torque = 2050,
                    RpmLimit = 2300,
                    IdleRpm = 700,
                    RpmLimitNeutral = 1600,
                    PeakRpm = 1200
                };
                Database.Engines.Add(engine);

                // Add Engine torque curves
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 300, Ratio = 0m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 440, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1100, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1400, Ratio = 1m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 1900, Ratio = 0.77m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2400, Ratio = 0.5m });
                Database.TorqueRatios.Add(new TorqueRatio() { EngineId = engine.Id, RpmLevel = 2600, Ratio = 0m });

                // Add engine to the w900
                Database.TruckEngines.Add(new TruckEngine() { Engine = engine, Truck = truck3 });

                // === Commit Transaction === //
                trans.Commit();
            }
        }
    }
}
