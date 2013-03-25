using System;
using System.Management;
using System.Runtime.InteropServices;

namespace DriverUtilites.InfoCollecting.Utils
{
	/// <summary>
	/// Contains a collection of useful utitlites for dealing with allDevices and drivers.
	/// </summary>
	public class DriverUtils
	{
		#region Static Variables

		public static string SaveDir;
		/// <summary>
		/// Driver classes which are not included in a Freemium Driver Utilites scan results
		/// </summary>
		public static readonly String[] RestrictedClasses = { "COMPUTER", "VOLUME", "DISKDRIVE", "LEGACYDRIVER", "PROCESSOR" };

		#endregion

		#region Delegates

		/// <summary>
		/// A delegate for reporting progress.
		/// </summary>
		/// <param name="percent">The percentage completed of the progress.</param>
		/// <param name="data">Any extra data associated with the progress updates.</param>
		public delegate void ProgressUpdate(int percent, object data);

		#endregion

		#region Methods

		/// <summary>
		/// Scans computer for plug-and-play allDevices.
		/// </summary>
		/// <returns></returns>
		public ManagementObjectCollection ScanDevices()
		{
			var devicesQuery = new ManagementObjectSearcher("root\\CIMV2", @"SELECT * FROM Win32_PnPSignedDriver WHERE DeviceName IS NOT NULL");

			return devicesQuery.Get();
		}

		#endregion

		#region PInvokes

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetupOpenInfFile(
			[MarshalAs(UnmanagedType.LPTStr)] string fileName,
			[MarshalAs(UnmanagedType.LPTStr)] string infClass,
			Int32 infStyle, out uint errorLine);

		[DllImport("DIFxAPI.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern Int32 DriverPackageInstall(
			[MarshalAs(UnmanagedType.LPTStr)] string driverPackageInfPath,
			Int32 flags,
			IntPtr pInstallerInfo,
			out bool pNeedReboot);

		#endregion
	}

	#region Enums

	public enum DriverStatus
	{
		UpToDate,
		OutOfDate,
		NotInstalled
	}

	#endregion
}
