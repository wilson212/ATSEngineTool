using System;
using System.IO;
using System.Reflection;
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
        public static Version Version { get; private set; } = new Version(2, 0, 0);

        /// <summary>
        /// The root path to the __Templates directory
        /// </summary>
        //public static readonly string TemplatesaPath = Path.Combine(Program.RootPath, "__Templates");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Setup visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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

            // Run the main GUI
            Application.Run(new MainForm());
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
    }
}
