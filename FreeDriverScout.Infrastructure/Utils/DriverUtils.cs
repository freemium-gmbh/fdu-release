using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using FreeDriverScout.Models;
using System.Management;
using System.Diagnostics;
using System.ComponentModel;

namespace FreeDriverScout.Utils
{
    /// <summary>
    /// Contains a collection of useful utitlites for dealing with allDevices and drivers.
    /// </summary>
    public class DriverUtils
    {
        #region Static Variables

        // TODO: add other driver install error codes (http://msdn.microsoft.com/en-us/library/windows/hardware/ff544813%28v=vs.85%29.aspx)
        // http://msdn.microsoft.com/en-us/library/cc231199.aspx


        public const  Int32 ERROR_SUCCESS = 0x00000000;
        // The catalog file has an Authenticode signature whose certificate chain terminates in a root certificate that is not trusted.
        public const UInt64 CERT_E_EXPIRED = 0x800B0101L;
        
        // The certificate for the driver package is not valid for the requested usage. If the driver package does not have a valid WHQL signature, DriverPackageInstall returns this error if, in response to a driver signing dialog box, the user chose not to install a driver package, or if the caller specified the DRIVER_PACKAGE_SILENT flag.
        public const UInt64 CERT_E_UNTRUSTEDROOT = 0x800B0109L;
        
        // The catalog file for the specified driver package was not found; or possibly, some other error occurred when DriverPackageInstall tried to verify the driver package signature.
        public const UInt64 CERT_E_WRONG_USAGE = 0x800B0110L;
        
        // A caller of DriverPackageInstall must be a member of the Administrators group to install a driver package.
        public const UInt64 CRYPT_E_FILE_ERROR = 80092003;
        
        // The current Microsoft Windows version does not support this operation.
        public const UInt64 ERROR_ACCESS_DENIED = 0x00000005;
        
        //An old or incompatible version of DIFxApp.dll or DIFxAppA.dll might be present in the system. For more information about these .dll files, see How DIFxApp Works.
        public const UInt64 ERROR_BAD_ENVIRONMENT = 0xA;

        //DriverPackageInstall could not preinstall the driver package because the specified INF file is in the system INF file directory.
        public const UInt64 ERROR_CANT_ACCESS_FILE = 0x00000780;

        //The INF file that was specified by DriverPackageInfPath was not found.
        public const UInt64 ERROR_FILE_NOT_FOUND = 0x00000002;
        
        //The INF file path, in characters, that was specified by DriverPackageInfPath is greater than the maximum supported path length. For more information about path length, see Specifying a Driver Package INF File Path.
        public const UInt64 ERROR_FILENAME_EXCED_RANGE = 0x000000CE;

        //The 32-bit version DIFxAPI does not work on Win64 systems. A 64-bit version of DIFxAPI is required.
        //public const Int32 ERROR_IN_WOW64;

        //The installation failed.
        public const UInt64 ERROR_INSTALL_FAILURE = 0x00000643;

        //The catalog file for the specified driver package is not valid or was not found.
        public const UInt64 ERROR_INVALID_CATALOG_DATA = 0xE0000304;

        //The specified INF file path is not valid.
        public const UInt64 ERROR_INVALID_NAME = 0x0000007B;

        //A supplied parameter is not valid.
        public const UInt64 ERROR_INVALID_PARAMETER = 0x00000057;

        //The driver package does not specify a hardware identifier or compatible identifier that is supported by the current platform.
        public const UInt64 ERROR_NO_DEVICE_ID = 0xE0000301;

        //(Applies only to PnP function drivers) The specified driver package was not installed for matching devices because the driver packages already installed for the matching devices are a better match for the devices than the specified driver package. The driver package was preinstalled unless the DRIVER_PACKAGE_ONLY_IF_DEVICE_PRESENT flag was specified.
        public const UInt64 ERROR_NO_MORE_ITEMS = 0x00000103;

        //(Applies only to PnP function drivers) The driver package was not installed on any device because there are no matching devices in the device tree. The driver package was preinstalled unless the DRIVER_PACKAGE_ONLY_IF_DEVICE_PRESENT flag was specified.
        public const UInt64 ERROR_NO_SUCH_DEVINST = 0xE000020B;

        //Available system memory was insufficient to perform the operation.
        public const UInt64 ERROR_OUTOFMEMORY = 0x0000000E;

        //A component of the driver package in the DIFx driver store is locked by a thread or process. This error can occur if a process or thread, other than the thread or process of the caller, is currently accessing the same driver package as the caller.
        public const UInt64 ERROR_SHARING_VIOLATION = 0x00000020;

        //The signing certificate is not valid for the current Windows version or it is expired.
        public const UInt64 ERROR_SIGNATURE_OSATTRIBUTE_MISMATCH = 0xE0000244;

        //The driver package type is not supported.
        public const UInt64 ERROR_UNSUPPORTED_TYPE = 0x0000065E;

        //The driver package is not signed.
        public const UInt64 TRUST_E_NOSIGNATURE = 0x800B0101;


        public enum DIFXAPI_LOG
        {
            DIFXAPI_SUCCESS = 0, // Successes
            DIFXAPI_INFO = 1,    // Basic logging information that should always be shown
            DIFXAPI_WARNING = 2, // Warnings
            DIFXAPI_ERROR = 3    // Errors
        }

        public delegate void DIFLOGCALLBACK(DIFXAPI_LOG EventType, Int32 ErrorCode, [MarshalAs(UnmanagedType.LPTStr)] string EventDescription, IntPtr CallbackContext);

        [DllImport("DIFxAPI.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SetDifxLogCallback(DIFLOGCALLBACK LogCallback, IntPtr CallbackContext);
        






        public static string SaveDir;
        /// <summary>
        /// Driver classes which are not included in a Freemium Driver Utilites scan results
        /// </summary>
        public static readonly String[] RestrictedClasses = { "COMPUTER", "VOLUME", "DISKDRIVE", "LEGACYDRIVER", "PROCESSOR" };

        #endregion

        #region Instance Variables

        private bool needsReboot;

        #endregion

        #region Events
        /// <summary>
        /// Occurs when the currently downloaded driver file finishes download.
        /// </summary>
        public event EventHandler<System.ComponentModel.AsyncCompletedEventArgs> FileDownloadCompleted;

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


        /// <summary>
        /// Make sure to call this before any driver installations or restorations
        /// to get the correct decision of whether to reboot.
        /// </summary>
        public void WatchForReboot()
        {
            needsReboot = false;
        }

        /// <summary>
        /// Checks if Reboot is required (to be called after installations or restorations
        /// of drivers).
        /// </summary>
        /// <returns><code>true</code> if system reboot is required, <code>false</code> otherwise</returns>
        public bool IsRebootRequired()
        {
            return needsReboot;
        }

        /// <summary>
        /// Backs up a driver.
        /// </summary>
        /// <param name="deviceName">The device name whose driver is to be backed up</param>
        /// <param name="infFileName">The name of the driver inf file</param>
        /// <param name="backupDir">The output backup directory</param>               
        public bool BackupDriver(string deviceName, string infFileName, string backupDir)
        {
            bool result = false;
            try
            {
                backupDir = backupDir.EndsWith("\\") ? backupDir : backupDir + "\\";
                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                deviceName = deviceName.Trim().Replace('/', ' ').Replace('\\', ' ');
                var deviceBackupDir = backupDir + deviceName + "\\";
                if (!Directory.Exists(deviceBackupDir))
                    Directory.CreateDirectory(deviceBackupDir);

                // Empty target device backup dir
                var oldFiles = new DirectoryInfo(deviceBackupDir).GetFiles();
                foreach (var oldFile in oldFiles)
                {
                    oldFile.Delete();
                }

                var oldDirs = new DirectoryInfo(deviceBackupDir).GetDirectories();
                foreach (var oldDir in oldDirs)
                {
                    oldDir.Delete(true);
                }

                var windir = Environment.GetEnvironmentVariable("windir") + "\\";

                // Check if driver exists in driver store
                string[] possibleDriverDirsInStore = null;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    var driverStoreRepo = windir + "System32\\DriverStore\\FileRepository";
                    possibleDriverDirsInStore = Directory.GetDirectories(driverStoreRepo, infFileName + "*");
                }

                if (possibleDriverDirsInStore != null &&
                    possibleDriverDirsInStore.Length == 1)
                {
                    CopyFolder(possibleDriverDirsInStore[0], deviceBackupDir);
                }
                else
                {
                    // Backup inf file
                    var infFilePath = windir + "inf\\" + infFileName;
                    File.Copy(infFilePath, deviceBackupDir + infFileName);

                    // Backup PNF file
                    var pnfFileName = infFileName.Replace(".inf", ".PNF");
                    var pnfFilePath = windir + "inf\\" + pnfFileName;
                    File.Copy(pnfFilePath, deviceBackupDir + pnfFileName);

                    // Backup CAT file
                    string originalCATName = IniFileUtils.GetValue(infFilePath, "Version", "CatalogFile");
                    if (originalCATName != string.Empty)
                    {
                        var catName = infFileName.Replace(".inf", ".cat");
                        var catroot = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\catroot";
                        var catrootDirs = new DirectoryInfo(catroot).GetDirectories();

                        foreach (var dir in catrootDirs)
                        {
                            var catPath = dir.FullName + "\\" + catName;
                            if (File.Exists(catPath))
                            {
                                File.Copy(catPath, deviceBackupDir + originalCATName);
                                break;
                            }
                        }
                    }
                    // Backup driver files from by parsing the inf file
                    if (Is64BitWindows())
                        BackupDriverFilesFromInf(".amd64", infFilePath, deviceBackupDir);
                    else
                        BackupDriverFilesFromInf(".x86", infFilePath, deviceBackupDir);
                }
                result = true;
            }
            catch (Exception ex)
            {
            }
            return result;
        }


        public static void DIFLogCallbackFunc(DIFXAPI_LOG EventType, Int32 ErrorCode, string EventDescription, IntPtr CallbackContext)
        {
            switch (EventType)
            {
                case DIFXAPI_LOG.DIFXAPI_SUCCESS:
                    Console.WriteLine("SUCCESS: {0}. Error code: {1}", EventDescription, ErrorCode);
                    break;
                case DIFXAPI_LOG.DIFXAPI_INFO:
                    Console.WriteLine("INFO: {0}. Error code: {1}", EventDescription, ErrorCode);
                    break;
                case DIFXAPI_LOG.DIFXAPI_WARNING:
                    Console.WriteLine("WARNING: {0}. Error code: {1}", EventDescription, ErrorCode);
                    break;
                case DIFXAPI_LOG.DIFXAPI_ERROR:
                    Console.WriteLine("ERROR: {0}. Error code: {1}", EventDescription, ErrorCode);
                    break;
            }
        }

        /// <summary>
        /// Restores a backed up driver to the system dir.
        /// </summary>
        /// <param name="deviceName">The name of device whose driver was backed up</param>
        /// <param name="backupDir">The backup directory</param>
        /// <returns><code>True</code> if restoring was successfull, <code>False</code> otherwise</returns>
        public bool RestoreDriver(string deviceName, string backupDir)
        {
            bool result = false;
            // Format paths
            deviceName = deviceName.Trim().Replace('/', ' ').Replace('\\', ' ');
            backupDir = !backupDir.EndsWith("\\") ? backupDir + "\\" : backupDir;

            // Find inf file
            var deviceBackupDirPath = backupDir + deviceName;

            try
            {
                var deviceBackupDir = new DirectoryInfo(deviceBackupDirPath);
                var infFile = deviceBackupDir.GetFiles("*.inf")[0];

                SetDifxLogCallback(new DIFLOGCALLBACK(DIFLogCallbackFunc), IntPtr.Zero);

                bool driverNeedsReboot;
                int err = DriverPackageInstall(infFile.FullName, DRIVER_PACKAGE_FORCE, IntPtr.Zero, out driverNeedsReboot);
//                if (err != 0)
  //                  throw new Win32Exception(err); // << the right test was here
                needsReboot = needsReboot || driverNeedsReboot;

                result = ((Int32)err == ERROR_SUCCESS);
            }
            catch
            {                
            }
            return result;
        }

        private void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        private void BackupDriverFilesFromInf(string platform, string infFilePath, string deviceBackupDir)
        {
            // Get driver files
            var sourceDisksFiles = "SourceDisksFiles" + platform;
            var driverFiles = IniFileUtils.GetKeys(infFilePath, sourceDisksFiles);
            if (driverFiles.Count == 0)
            {
                sourceDisksFiles = "SourceDisksFiles";
                driverFiles = IniFileUtils.GetKeys(infFilePath, sourceDisksFiles);
            }

            // Determine source disks names section
            var sourceDisksNames = "SourceDisksNames" + platform;
            var sourceDisk = IniFileUtils.GetKeys(infFilePath, sourceDisksNames);
            if (sourceDisk.Count == 0)
            {
                sourceDisksNames = "SourceDisksNames";
            }

            // Get search dirs
            var destinationDirs = IniFileUtils.GetKeys(infFilePath, "DestinationDirs");
            List<string> searchDirs = new List<string>();

            foreach (var dir in destinationDirs)
            {
                var dirVal = IniFileUtils.GetValue(infFilePath, "DestinationDirs", dir).Split(',');
                var dirid = int.Parse(dirVal[0]);

                var searchDir = IniFileUtils.ResolveDirId(dirid);
                if (dirVal.Length > 1)
                    searchDir += "\\" + dirVal[1].Trim();

                searchDirs.Add(searchDir);
            }

            foreach (var driverFile in driverFiles)
            {
                var sourceDiskId = IniFileUtils.GetValue(infFilePath, sourceDisksFiles, driverFile);
                var sourcePath = IniFileUtils.GetValue(infFilePath, sourceDisksNames, sourceDiskId).Split(',');

                var backupSubdir = deviceBackupDir;
                if (sourcePath.Length == 4)
                    backupSubdir = Path.Combine(deviceBackupDir, sourcePath[3]);

                if (!Directory.Exists(backupSubdir))
                    Directory.CreateDirectory(backupSubdir);

                foreach (var possibleDir in searchDirs)
                {
                    if (File.Exists(possibleDir + "\\" + driverFile))
                    {
                        File.Copy(possibleDir + "\\" + driverFile, backupSubdir + "\\" + driverFile);
                        break;
                    }
                }
            }
        }

        private static bool Is64BitWindows()
        {
            return IntPtr.Size == 8;
        }

        #endregion

        #region PInvokes

        const uint SP_COPY_DELETESOURCE = 0x0000001; // delete source file on successful copy
        const uint SP_COPY_REPLACEONLY = 0x0000002; // copy only if target file already present
        const uint SP_COPY_NEWER_OR_SAME = 0x0000004; // copy only if source newer than or same as target
        const uint SP_COPY_NEWER_ONLY = 0x0010000; // copy only if source file newer than target
        const uint SP_COPY_NOOVERWRITE = 0x0000008; // copy only if target doesn't exist
        const uint SP_COPY_NODECOMP = 0x0000010; // don't decompress source file while copying
        const uint SP_COPY_LANGUAGEAWARE = 0x0000020; // don't overwrite file of different language
        const uint SP_COPY_SOURCE_ABSOLUTE = 0x0000040; // SourceFile is a full source path
        const uint SP_COPY_SOURCEPATH_ABSOLUTE = 0x0000080; // SourcePathRoot is the full path
        const uint SP_COPY_FORCE_IN_USE = 0x0000200; // Force target- in-use behavior
        const uint SP_COPY_FORCE_NOOVERWRITE = 0x0001000; // like NOOVERWRITE but no callback nofitication
        const uint SP_COPY_FORCE_NEWER = 0x0002000; // like NEWER but no callback nofitication

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetupInstallFile(
            IntPtr InfHandle,
            IntPtr InfContext,
            string SourceFile,
            string SourcePathRoot,
            string DestinationName,
            uint CopyStyle,
            IntPtr CopyMsgHandler,
            IntPtr Context);

        const Int32 INF_STYLE_OLDNT = 0x00000001;
        const Int32 INF_STYLE_WIN4 = 0x00000002;

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupOpenInfFile(
            [MarshalAs(UnmanagedType.LPTStr)] string FileName,
            [MarshalAs(UnmanagedType.LPTStr)] string InfClass,
            Int32 InfStyle, out uint ErrorLine);

        private const Int32 DRIVER_PACKAGE_FORCE = 0x00000004;
        private const Int32 DRIVER_PACKAGE_ONLY_IF_DEVICE_PRESENT = 0x00000008;
        private const Int32 DRIVER_PACKAGE_LEGACY_MODE = 0x00000010;
        private const Int32 DRIVER_PACKAGE_SILENT = 0x00000002;
        private const Int32 DRIVER_PACKAGE_REPAIR = 0x00000001;

        [DllImport("DIFxAPI.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 DriverPackageInstall(
            [MarshalAs(UnmanagedType.LPTStr)] string DriverPackageInfPath,
            Int32 Flags,
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
