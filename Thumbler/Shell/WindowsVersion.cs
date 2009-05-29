using System;

namespace Thumbler.Shell
{
    /// <summary>
    /// Contains logic for querying the environment about the version
    /// of Windows on which the application is running.
    /// </summary>
    static class WindowsVersion
    {
        private static readonly Version version = Environment.OSVersion.Version;

        /// <summary>
        /// Gets a value indicating whether the OS version is Windows 7 or newer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the OS version is Windows 7 or newer; otherwise, 
        /// 	<c>false</c>.
        /// </value>
        public static bool IsSevenOrNewer
        {
            get
            {
                return
                    version.Major > 6 ||
                    version.Major == 6 && version.Minor >= 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the OS version is Windows Vista or newer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the OS version is Windows Vista or newer; otherwise, 
        /// 	<c>false</c>.
        /// </value>
        public static bool IsVistaOrNewer
        {
            get
            {
                return version.Major >= 6;
            }
        }
    }
}
