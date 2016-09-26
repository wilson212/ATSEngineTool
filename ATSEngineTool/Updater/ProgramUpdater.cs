using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ATSEngineTool.Updater;
using Newtonsoft.Json;

namespace ATSEngineTool
{
    /// <summary>
    /// Auto updater for the BF2Statistics control center
    /// </summary>
    public static class ProgramUpdater
    {
        /// <summary>
        /// Path to the Versions file
        /// </summary>
        public static readonly Uri Url = new Uri("https://api.github.com/repos/wilson212/ATSEngineTool/releases?per_page=2");

        /// <summary>
        /// The new updated version
        /// </summary>
        public static Version NewVersion;

        /// <summary>
        /// Indicates whether there is an update avaiable for download
        /// </summary>
        public static bool UpdateAvailable 
        {
            get
            {
                if (NewVersion == null)
                    return false;

                return Program.Version.CompareTo(NewVersion) != 0;
            }
        }

        /// <summary>
        /// Event fired when the update check has completed
        /// </summary>
        public static event EventHandler CheckCompleted;

        /// <summary>
        /// The webclient used to make the requests to github
        /// </summary>
        private static WebClient Web;

        static ProgramUpdater()
        {
            // By pass SSL Cert checks
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// Checks for a new update Async.
        /// </summary>
        public static async void CheckForUpdateAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    // Use WebClient to download the latest version string
                    using (Web = new WebClient())
                    {
                        // Simulate some headers, Github throws a fit otherwise
                        Web.Headers["User-Agent"] = "ATSEngineTool v" + Program.Version;
                        Web.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                        Web.Headers["Accept-Language"] = "en-US,en;q=0.8";
                        Web.Proxy = null; // Disable proxy because this can cause slowdown on some machines

                        // Download file
                        string json = Web.DownloadString(Url);

                        // Use our Json.Net library to convert our API string into an object
                        var Releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(json)
                            .Where(x => x.PreRelease == false && x.Draft == false)
                            .OrderByDescending(x => x.Published).ToList();

                        // Parse version
                        if (Releases?.Count > 0)
                            Version.TryParse(Releases[0].TagName, out NewVersion);
                    }
                });
            }
            catch (Exception e)
            {
                //Program.ErrorLog.Write("WARNING: Error occured while trying to fetch the new release version: " + e.Message);
                NewVersion = Program.Version;
            }

            // Fire Check Completed Event
            CheckCompleted(NewVersion, EventArgs.Empty);
        }
    }
}
