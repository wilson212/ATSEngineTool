using System;
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
        public static Version CurrentVersion { get; protected set; } = new Version(1, 6);

        /// <summary>
        /// Gets the current database tables version
        /// </summary>
        public static Version DatabaseVersion { get; protected set; }

        #region Database Entity Sets

        protected DbSet<DbVersion> DbVersions { get; set; }

        public DbSet<Engine> Engines { get; protected set; }

        public DbSet<EngineSeries> EngineSeries { get; protected set; }

        public DbSet<SoundPackage> SoundPackages { get; protected set; }

        public DbSet<EngineSound> EngineSounds { get; protected set; }

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
        }

        /// <summary>
        /// Creates a new connection to the SQLite Database
        /// </summary>
        public AppDatabase() : base(Builder)
        {
            // Open connection first
            base.Connect();

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
                    RebuildTables();

                    // Try 1 last time to get the Version
                    GetVersion();
                }
            }

            // Create Database Sets
            DbVersions = new DbSet<DbVersion>(this);
            Engines = new DbSet<Engine>(this);
            EngineSeries = new DbSet<EngineSeries>(this);
            SoundPackages = new DbSet<SoundPackage>(this);
            EngineSounds = new DbSet<EngineSound>(this);
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
                CodeFirstSQLite.DropTable<SoundPackage>(this);
                CodeFirstSQLite.DropTable<Truck>(this);
                CodeFirstSQLite.DropTable<DbVersion>(this);

                // Create the needed database tables
                CodeFirstSQLite.CreateTable<DbVersion>(this);
                CodeFirstSQLite.CreateTable<EngineSeries>(this);
                CodeFirstSQLite.CreateTable<SoundPackage>(this);
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
