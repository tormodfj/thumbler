using System;

namespace Thumbler.Shell
{
	/// <summary>
	/// Contains logic for querying the environment about the version
	/// of Windows on which the application is running.
	/// </summary>
	static class WindowsVersion
	{
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
					Environment.OSVersion.Version.Major > 6 ||
					Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1;
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
				return Environment.OSVersion.Version.Major >= 6;
			}
		}
	}
}
