using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
using ATSEngineTool.Properties;
using Microsoft.Win32;

namespace ATSEngineTool
{
    static class Program
    {
        /// <summary>
        /// Root path to this current application instance
        /// </summary>
        public static readonly string RootPath = Application.StartupPath;

        /// <summary>
        /// Our program settings
        /// </summary>
        public static readonly Settings Config = Settings.Default;

        /// <summary>
        /// Program Version
        /// </summary>
        public static Version Version { get; private set; } = new Version(2, 8, 5);

        /// <summary>
        /// English number format info
        /// </summary>
        public static readonly NumberFormatInfo NumberFormat = CultureInfo.CreateSpecificCulture("en-US").NumberFormat;

        /// <summary>
        /// Returns whether this application is running in administrator mode.
        /// </summary>
        public static bool RunAsAdmin
        {
            get
            {
                WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Setup visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set Exception Handler
            Application.ThreadException += ExceptionHandler.OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.OnUnhandledException;

            // Update program settings after a program update
            if (!Settings.Default.Updated)
            {
                Settings.Default.Upgrade();
                Settings.Default.Updated = true;
                Settings.Default.Save();
            }

            // Check for steam installation path (part 1)
            string steamPath = Settings.Default.SteamPath;
            if (String.IsNullOrWhiteSpace(steamPath) || !Directory.Exists(steamPath))
            {
                RegistryKey regKey = Registry.CurrentUser;
                regKey = regKey.OpenSubKey(@"Software\Valve\Steam");
                if (regKey != null)
                {
                    steamPath = regKey.GetValue("SteamPath")?.ToString();
                    Settings.Default.SteamPath = steamPath;
                    Settings.Default.Save();
                }
            }

            // Initialize Database
            InitializeDatabase();

            // Run the main GUI
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Ensures the "data" folder is created, and migrates the database from
        /// 2.0.1 to the new directory
        /// </summary>
        private static void InitializeDatabase()
        {
            // Make sure the data directory exists
            string path = Path.Combine(Program.RootPath, "data", "backups");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Check for app data
            string oldPath = Path.Combine(Program.RootPath, "EngineData.db");
            string newPath = Path.Combine(Program.RootPath, "data", "AppData.db");
            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
            }

            // Create a database from the default data if the database doesnt exist
            string defaultData = Path.Combine(Program.RootPath, "data", "Default.db");
            if (!File.Exists(newPath) && File.Exists(defaultData))
            {
                File.Copy(defaultData, newPath);
            }
        }

        /// <summary>
        /// Gets the string contents of an embedded resource
        /// </summary>
        /// <param name="ResourceName">The name of the embedded resource</param>
        /// <returns></returns>
        public static string GetResourceAsString(string ResourceName)
        {
            string Result = "";
            using (Stream ResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
            using (StreamReader Reader = new StreamReader(ResourceStream))
                Result = Reader.ReadToEnd();

            return Result;
        }

        public static Stream GetResource(string ResourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
        }
    }
}
