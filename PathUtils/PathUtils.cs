using System;
using System.Collections.Generic;
using System.Text;

namespace PathUtils {
    public static class PathUtils {

        public static string PathSplitter
        {
            get {
                return IsLinux ? "/" : "\\";
            }
        } 

        public static bool IsLinux
        {
            get {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        /// <summary>
        /// Converts path to correspond with the current system's path style.
        /// </summary>
        /// <param name="path">Accepts a Path like C:/Hello World/Bye World (yes, use forward slashes)</param>
        /// <returns>Keeps it as is on Linux machines, converts it to C:\Hello World\Bye World on Windows.</returns>
        public static string ConvertToSystemPath(string path) {
            return path.Replace("/", PathSplitter);
        }

    }
}
