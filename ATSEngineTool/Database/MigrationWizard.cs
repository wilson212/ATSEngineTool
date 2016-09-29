using System;
using System.IO;
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
                    default:
                        throw new Exception($"Unexpected database version: {AppDatabase.DatabaseVersion}");
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

        private void MigrateTo_1_2()
        {
            // Create backup
            File.Copy(
                Path.Combine(Program.RootPath, "data", "AppData.db"),
                Path.Combine(Program.RootPath, "data", "backups", $"AppData_v1.1_{Epoch.Now}.db")
            );

            // Run the update in a transaction
            using (var trans = Database.BeginTransaction())
            {
                // Create Tables
                Database.CreateTable<TransmissionSeries>();
                Database.CreateTable<Transmission>();
                Database.CreateTable<TransmissionGear>();
                Database.CreateTable<TruckTransmission>();

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

                // Update database version
                string sql = "INSERT INTO `DbVersion`(`Version`, `AppliedOn`) VALUES({0}, {1});";
                Database.Execute(String.Format(sql, Version.Parse("1.2"), Epoch.Now));

                // Commit
                trans.Commit();
            }
        }
    }
}
