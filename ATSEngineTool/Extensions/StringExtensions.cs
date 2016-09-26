using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEngineTool
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, bool bCaseInsensitive)
        {
            return source.IndexOf(toCheck, bCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
        }

        /// <summary>
        /// Removes any invalid file path characters from this string
        /// </summary>
        public static string MakeFileNameSafe(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
