using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEngineTool
{
    public static class Epoch
    {
        /// <summary>
        /// The value for the Unix epoch (e.g. January 1, 1970 at midnight, in UTC).
        /// </summary>
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int Now => (int)(DateTime.UtcNow - UnixEpoch).TotalSeconds;

        public static DateTime FromUnix(int secondsSinceepoch)
        {
            return UnixEpoch.AddSeconds(secondsSinceepoch);
        }

        public static int ToUnix(DateTime dateTime)
        {
            return (int)(dateTime - UnixEpoch).TotalSeconds;
        }
    }
}
