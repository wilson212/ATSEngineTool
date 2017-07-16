using System;
using System.IO;
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
                        case "1.1":
                        case "1.2":
                        case "1.3":
                        case "1.4":
                        case "1.5":
                        case "1.6":
                        case "1.7":
                            throw new Exception($"Impossible migration from: {AppDatabase.DatabaseVersion} to 2.0!");
                        case "2.0":
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
    }
}
