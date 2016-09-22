using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    /// <summary>
    /// This object is used to interact with the SQLite database "EngineData.db"
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
        public static Version CurrentVersion { get; protected set; } = new Version(1, 0);

        /// <summary>
        /// Gets the current database tables version
        /// </summary>
        public static Version DatabaseVersion { get; protected set; }

        #region Database Entity Sets

        protected DbSet<DbVersion> DbVersions { get; set; }

        public DbSet<Engine> Engines { get; protected set; }

        public DbSet<EngineSeries> EngineSeries { get; protected set; }

        public DbSet<SoundPackage> EngineSounds { get; protected set; }

        public DbSet<TruckEngine> TruckEngines { get; protected set; }

        public DbSet<TorqueRatio> TorqueRatios { get; protected set; }

        public DbSet<Truck> Trucks { get; protected set; }

        #endregion Database Entity Sets

        /// <summary>
        /// Static Constructor
        /// </summary>
        static AppDatabase()
        {
            string database = Path.Combine(Program.RootPath, "EngineData.db");
            Builder = new SQLiteConnectionStringBuilder();
            Builder.DataSource = database;
            Builder.ForeignKeys = true;
            EscapeCharacters = new char[2] { '`', '`' };
        }

        /// <summary>
        /// Creates a new connection to the SQLite Database
        /// </summary>
        public AppDatabase() : base(Builder)
        {
            // Open connection first
            base.Connect();
            bool createdTables = false;

            // Grab the current tables version
            if (DatabaseVersion == null)
            {
                GetVersion:
                {
                    try
                    {
                        // Grab version. Plain SQL query here for performance
                        string query = "SELECT * FROM DbVersion ORDER BY UpdateId DESC LIMIT 1";
                        DbVersion row = Query<DbVersion>(query).FirstOrDefault();

                        // If row is null, then the table exists, but was truncated
                        if (row == null)
                        {
                            if (createdTables)
                                throw new Exception("Unable to create database tables");

                            // Rebuild database tables
                            createdTables = true;
                            RebuildTables();

                            // Try the query again
                            goto GetVersion;
                        }

                        // Set instance database version
                        DatabaseVersion = row.Version;
                    }
                    catch (SQLiteException e) when (e.Message.Contains("no such table"))
                    {
                        // If this is the first query on the database, We need to create
                        // new Database tables
                        if (!createdTables)
                        {
                            createdTables = true;
                            RebuildTables();

                            // Try the query again
                            goto GetVersion;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            // Create Database Sets
            DbVersions = new DbSet<DbVersion>(this);
            Engines = new DbSet<Engine>(this);
            EngineSeries = new DbSet<EngineSeries>(this);
            EngineSounds = new DbSet<SoundPackage>(this);
            TruckEngines = new DbSet<TruckEngine>(this);
            TorqueRatios = new DbSet<TorqueRatio>(this);
            Trucks = new DbSet<Truck>(this);

            // Migrations
            MigrationWizard wizard = new MigrationWizard(this);
            if (createdTables)
            {
                // Initialize data
                wizard.InitializeData();
            }
            else if(DatabaseVersion != CurrentVersion)
            {
                wizard.MigrateTables();
            }
        }

        /// <summary>
        /// Drops all tables from the database, and the creates new
        /// tables.
        /// </summary>
        protected void RebuildTables()
        {            
            // Wrap in a transaction
            using (SQLiteTransaction tr = base.BeginTransaction())
            {
                // Delete old table rementants
                CodeFirstSQLite.DropTable<TruckEngine>(this);
                CodeFirstSQLite.DropTable<TorqueRatio>(this);
                CodeFirstSQLite.DropTable<Engine>(this);
                CodeFirstSQLite.DropTable<EngineSeries>(this);
                CodeFirstSQLite.DropTable<SoundPackage>(this);
                CodeFirstSQLite.DropTable<Truck>(this);
                CodeFirstSQLite.DropTable<DbVersion>(this);

                // Create the needed database tables
                CodeFirstSQLite.CreateTable<DbVersion>(this);
                CodeFirstSQLite.CreateTable<EngineSeries>(this);
                CodeFirstSQLite.CreateTable<SoundPackage>(this);
                CodeFirstSQLite.CreateTable<Engine>(this);
                CodeFirstSQLite.CreateTable<Truck>(this);
                CodeFirstSQLite.CreateTable<TorqueRatio>(this);
                CodeFirstSQLite.CreateTable<TruckEngine>(this);

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
    }
}
