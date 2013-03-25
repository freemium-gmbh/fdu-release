using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
using DriverUtilites.Models;
using DriverUtilites.OSMigrationTool.Backup.Infrastructure;
using DriverUtilites.OSMigrationTool.Backup.Models;
using DriverUtilites.Utils;
using Ionic.Zip;
using MessageBoxUtils;
using Microsoft.Win32;

namespace DriverUtilites.OSMigrationTool.Backup.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public MainWindowViewModel()
        {
            CurrentDispatcher = Dispatcher.CurrentDispatcher;

            #region Commands initialization

            scanCommand = new SimpleCommand
            {
                ExecuteDelegate = x => Scan()
            };

            cancelScanCommand = new SimpleCommand
            {
                ExecuteDelegate = x => CancelScan()
            };

            checkDevicesGroupCommand = new SimpleCommand
            {
                ExecuteDelegate = CheckDevicesGroup
            };

            checkDeviceCommand = new SimpleCommand
            {
                ExecuteDelegate = CheckDevice
            };

            downloadDriversCommand = new SimpleCommand
            {
                ExecuteDelegate = x => DownloadDrivers()
            };

            cancelDownloadDriversCommand = new SimpleCommand
            {
                ExecuteDelegate = x => CancelDownloadDrivers()
            };

            composeCommand = new SimpleCommand
            {
                ExecuteDelegate = x => Compose()
            };

            cancelComposeCommand = new SimpleCommand
            {
                ExecuteDelegate = x => CancelCompose()
            };

            closeCommand = new SimpleCommand
            {
                ExecuteDelegate = x => Close()
            };

            #endregion

            InitializeBackgroundWorker();

            DownloadsDirectory = String.Format(@"{0}\Driver Utilites\Downloads", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            try
            {
                if (!Directory.Exists(DownloadsDirectory))
                {
                    Directory.CreateDirectory(DownloadsDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Commands

        ICommand scanCommand;
        public ICommand ScanCommand
        {
            get { return scanCommand; }
        }
        void Scan()
        {
            if (DestinationOS == null || (DestinationOS != null && string.IsNullOrEmpty(DestinationOS.Trim())))
            {
                WPFMessageBox.Show(WPFLocalizeExtensionHelpers.GetUIString("ChooseDestinationOS"), WPFLocalizeExtensionHelpers.GetUIString("ChooseDestOS"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Run scanning in a background thread
            ThreadPool.QueueUserWorkItem(x => RunScan());
        }

        ICommand cancelScanCommand;
        public ICommand CancelScanCommand
        {
            get { return cancelScanCommand; }
        }
        void CancelScan()
        {
            RunCancelScan();
        }

        ICommand checkDevicesGroupCommand;
        public ICommand CheckDevicesGroupCommand
        {
            get { return checkDevicesGroupCommand; }
        }
        void CheckDevicesGroup(object selectedMigrationDevicesGroup)
        {
            MigrationDevicesGroup migrationDevicesGroup = GroupedDevices.Where(d => d.DeviceClass == (string)selectedMigrationDevicesGroup).FirstOrDefault();
            if (migrationDevicesGroup != null)
            {
                foreach (MigrationDeviceInfo item in migrationDevicesGroup.Devices.Where(d => d.IsDestOSDriverAvailable))
                {
                    item.SelectedForDestOSDriverDownload = migrationDevicesGroup.GroupChecked;
                }
            }
        }

        ICommand checkDeviceCommand;
        public ICommand CheckDeviceCommand
        {
            get { return checkDeviceCommand; }
        }
        void CheckDevice(object selectedDevice)
        {
            MigrationDeviceInfo device = AllDevices.Where(d => d.Id == (string)selectedDevice).FirstOrDefault();
            if (device != null)
            {
                MigrationDevicesGroup migrationDevicesGroup = GroupedDevices.Where(d => d.DeviceClass == device.DeviceClass).FirstOrDefault();
                if (migrationDevicesGroup != null && migrationDevicesGroup.Devices.Where(d => d.SelectedForDestOSDriverDownload && d.IsDestOSDriverAvailable).Count() == 0)
                {
                    migrationDevicesGroup.GroupChecked = false;
                }
                else
                {
                    if (migrationDevicesGroup != null && migrationDevicesGroup.Devices.Where(d => d.SelectedForDestOSDriverDownload == false && d.IsDestOSDriverAvailable).Count() == 0)
                    {
                        migrationDevicesGroup.GroupChecked = true;
                    }
                }
            }
        }

        ICommand downloadDriversCommand;
        public ICommand DownloadDriversCommand
        {
            get { return downloadDriversCommand; }
        }
        void DownloadDrivers()
        {
            RunDownloadDrivers();
        }

        ICommand cancelDownloadDriversCommand;
        public ICommand CancelDownloadDriversCommand
        {
            get { return cancelDownloadDriversCommand; }
        }
        void CancelDownloadDrivers()
        {
            RunCancelDownloadDrivers();
        }

        ICommand composeCommand;
        public ICommand ComposeCommand
        {-
            get { return composeCommand; }
        }
        void Compose()
        {
            RunCompose();
        }

        ICommand cancelComposeCommand;
        public ICommand CancelComposeCommand
        {
            get { return cancelComposeCommand; }
        }
        void CancelCompose()
        {
            RunCancelCompose();
        }

        ICommand closeCommand;
        public ICommand CloseCommand
        {
            get { return closeCommand; }
        }
        void Close()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Properties

        readonly RegistryKey deviceClasses = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\", true);

        BackgroundWorker DownloadingBackgroundWorker = new BackgroundWorker();

        Dispatcher CurrentDispatcher;
        public delegate void MethodInvoker();
        DriverUtils driverUtils = new DriverUtils();
        bool ABORT;
        string DownloadsDirectory;

        string destinationDirectory;
        public string DestinationDirectory
        {
            get
            {
                return destinationDirectory;
            }
            set
            {
                destinationDirectory = value;
                OnPropertyChanged("DestinationDirectory");
            }
        }

        string destinationOS;
        public string DestinationOS
        {
            get
            {
                return destinationOS;
            }
            set
            {
                destinationOS = value;
                OnPropertyChanged("DestinationOS");
            }
        }

        int driverIndex;
        bool driverDownloading;

        /// <summary>
        /// Drivers for a destination OS collection presented as a <c>Dictionary</c> because this approach
        /// used in the <c>GetDriversForDestinationOS(Dictionary<string, string[]> driverScanResults)</c> method
        /// from the Freemium Utils driver info service, which is referenced via WCF
        /// </summary>
        Dictionary<string, string[]> driversForDestinationOs;
        DestinationOSDevices DestinationOSDevices = new DestinationOSDevices();

        ScanStatus status = ScanStatus.NotStarted;
        public ScanStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        string scanStatusTitle;
        public string ScanStatusTitle
        {
            get
            {
                return scanStatusTitle;
            }
            set
            {
                scanStatusTitle = value;
                OnPropertyChanged("ScanStatusTitle");
            }
        }

        string scanStatusText;
        public string ScanStatusText
        {
            get
            {
                return scanStatusText;
            }
            set
            {
                scanStatusText = value;
                OnPropertyChanged("ScanStatusText");
            }
        }

        string scanFinishTitle;
        public string ScanFinishTitle
        {
            get
            {
                return scanFinishTitle;
            }
            set
            {
                scanFinishTitle = value;
                OnPropertyChanged("ScanFinishTitle");
            }
        }

        string panelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("ScanDrivers");
        public string PanelScanHeader
        {
            get
            {
                return panelScanHeader;
            }
            set
            {
                panelScanHeader = value;
                OnPropertyChanged("PanelScanHeader");
            }
        }

        int progress;
        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
            }
        }

        ObservableCollection<MigrationDeviceInfo> allDevices = new ObservableCollection<MigrationDeviceInfo>();
        public ObservableCollection<MigrationDeviceInfo> AllDevices
        {
            get
            {
                return allDevices;
            }
            set
            {
                allDevices = value;
                OnPropertyChanged("AllDevices");
            }
        }

        ObservableCollection<MigrationDevicesGroup> groupedDevices = new ObservableCollection<MigrationDevicesGroup>();
        public ObservableCollection<MigrationDevicesGroup> GroupedDevices
        {
            get
            {
                return groupedDevices;
            }
            set
            {
                groupedDevices = value;
                OnPropertyChanged("GroupedDevices");
            }
        }

        ICollectionView orderedDeviceGroups;
        public ICollectionView OrderedDeviceGroups
        {
            get
            {
                return orderedDeviceGroups;
            }
            set
            {
                orderedDeviceGroups = value;
                OnPropertyChanged("OrderedDeviceGroups");
            }
        }

        List<MigrationDeviceInfo> devicesForDestinationOS = new List<MigrationDeviceInfo>();
        public List<MigrationDeviceInfo> DevicesForDestinationOS
        {
            get
            {
                return devicesForDestinationOS;
            }
            set
            {
                devicesForDestinationOS = value;
                OnPropertyChanged("DevicesForDestinationOS");
            }
        }

        #endregion

        #region Private methods

        private void InitializeBackgroundWorker()
        {
            DownloadingBackgroundWorker.WorkerReportsProgress = false;
            DownloadingBackgroundWorker.WorkerSupportsCancellation = true;
            DownloadingBackgroundWorker.DoWork += DownloadingBackgroundWorker_DoWork;
        }

        void RunScan()
        {
            try
            {
                ABORT = false;

                if (Status == ScanStatus.NotStarted)
                {
                    // Update device collections in the UI thread
                    if (CurrentDispatcher.Thread != null)
                    {
                        CurrentDispatcher.BeginInvoke((Action)(() =>
                        {
                            AllDevices = new ObservableCollection<MigrationDeviceInfo>();

                            PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("DriverScan");
                            ScanStatusTitle = WPFLocalizeExtensionHelpers.GetUIString("NowScanning");
                        }));
                    }

                    Progress = 0;
                    Status = ScanStatus.ScanStarted;
                    ScanStatusText = string.Empty;

                    var devices = driverUtils.ScanDevices();
                    int devicesProcessed = 0;
                    int allDevicesProcessed = 0;

                    // Driver scan results collection presented as a <c>Dictionary</c> because it is
                    // used as a parameter in the <c>GetDriversForDestinationOS(Dictionary<string, string[]> driverScanResults)</c> method from the
                    // Freemium Utils driver info service, which is referenced via WCF
                    var driverScanResults = new Dictionary<string, string[]>();

                    foreach (ManagementObject device in devices)
                    {
                        if (ABORT)
                            break;

                        Progress = (int)(85.0 * allDevicesProcessed++ / devices.Count);

                        if (DriverUtils.RestrictedClasses.Contains(device.GetPropertyValue("DeviceClass")))
                            continue;

                        //Uses device friendly name instead of device name when it's possible
                        string deviceName = string.Empty;
                        if (device.GetPropertyValue("FriendlyName") != null)
                        {
                            deviceName = (string)device.GetPropertyValue("FriendlyName");
                        }
                        else
                        {
                            deviceName = (string)(device.GetPropertyValue("DeviceName"));
                        }

                        //Use device friendly name instead of class name when it's possible
                        string deviceClassName = string.Empty;

                        if (device.GetPropertyValue("ClassGuid") != null)
                        {
                            var openSubKey = deviceClasses.OpenSubKey((string)device.GetPropertyValue("ClassGuid"));
                            if (openSubKey != null)
                            {
                                object classGuid = openSubKey.GetValue("Class");
                                if (classGuid != null)
                                {
                                    deviceClassName = classGuid.ToString();
                                }
                            }
                        }
                        else
                        {
                            deviceClassName = (string)(device.GetPropertyValue("DeviceClass"));
                        }

                        var item = new MigrationDeviceInfo(
                            (string)(device.GetPropertyValue("DeviceClass")),
                            deviceClassName,
                            deviceName,
                            (string)(device.GetPropertyValue("InfName")),
                            (string)(device.GetPropertyValue("DriverVersion")),
                            (string)(device.GetPropertyValue("DeviceId")),
                            (string)(device.GetPropertyValue("HardwareID")),
                            (string)(device.GetPropertyValue("CompatID"))
                        );

                        ScanStatusText = item.DeviceName;

                        if (CurrentDispatcher.Thread != null)
                        {
                            CurrentDispatcher.Invoke((MethodInvoker)(() => { if (item != null) AllDevices.Add(item); })
                            , null);
                        }
                        driverScanResults.Add(
                            AllDevices[devicesProcessed].Id,
                            new[] {
								AllDevices[devicesProcessed].HardwareID,
								AllDevices[devicesProcessed].CompatID
							      }
                        );
                        devicesProcessed++;
                    }

                    if (!ABORT)
                    {
                        if (CurrentDispatcher.Thread != null)
                        {
                            CurrentDispatcher.BeginInvoke((Action)(() =>
                            {
                                ScanStatusTitle = WPFLocalizeExtensionHelpers.GetUIString("ContactingServer");
                                ScanStatusText = string.Empty;
                            }));
                        }

                        // Here we have a filled DriverScanResults collection
                        driversForDestinationOs = GetDriversForDestinationOS(DestinationOS, driverScanResults);
                        foreach (KeyValuePair<string, string[]> driver in driversForDestinationOs)
                        {
                            foreach (MigrationDeviceInfo migrationDeviceInfo in AllDevices)
                            {
                                if (migrationDeviceInfo.Id == driver.Key)
                                {
                                    migrationDeviceInfo.IsDestOSDriverAvailable = true;
                                    migrationDeviceInfo.SelectedForDestOSDriverDownload = true;
                                    migrationDeviceInfo.DownloadLink = driver.Value[0];
                                    break;
                                }
                            }
                        }

                        if (CurrentDispatcher.Thread != null)
                        {
                            CurrentDispatcher.Invoke((MethodInvoker)delegate
                            {
                                UpdateGroupedDevices();

                                if (driversForDestinationOs.Count == 0)
                                {
                                    PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("NoDriversToDownload");
                                    ScanStatusTitle = WPFLocalizeExtensionHelpers.GetUIString("NoDriversToDownload");
                                    ScanStatusText = string.Empty;
                                    ScanFinishTitle = WPFLocalizeExtensionHelpers.GetUIString("NoDriversToDownload");
                                    Status = ScanStatus.ScanFinishedNoDrivers;
                                }
                                else
                                {
                                    if (ABORT)
                                    {
                                        return;
                                    }

                                    PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("ChooseDriversToDownload");
                                    Status = ScanStatus.ScanFinishedDriversFound;
                                    ScanFinishTitle = String.Format("{0} " + WPFLocalizeExtensionHelpers.GetUIString("DriversReadyToDownload"), AllDevices.Where(d => d.IsDestOSDriverAvailable).Count());
                                }
                            }
                            , null);
                        }
                    }
                }
                else
                {
                    ABORT = true;
                    PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("ScanDrivers");
                    Status = ScanStatus.NotStarted;
                }
            }
            catch { }
        }

        Dictionary<string, string[]> GetDriversForDestinationOS(string os, Dictionary<string, string[]> driverScanResults)
        {
            //BasicHttpBinding binding = new BasicHttpBinding();
            //EndpointAddress address = new EndpointAddress("http://driverservice.freemiumlab.com/ReportService.svc");
            //ReportServiceClient client = null;
            //try
            //{
            //    client = new ReportServiceClient(binding, address);
            //}
            //catch (Exception ex)
            //{
            //}
            //return client.GetDriverUpdates(driverScanResults);

            // TODO:
            // return a Dictionary<string, string[]> with id, downloadLink fields from server

            var result = new Dictionary<string, string[]>();

            int i = 0;
            foreach (MigrationDeviceInfo migrationDeviceInfo in AllDevices)
            {
                if (migrationDeviceInfo.DeviceName.IndexOf("Fax") != -1)
                {
                    result.Add(driverScanResults.ElementAt(i).Key, new[] { "http://dl.dropbox.com/u/38404707/Fax.zip" });
                }
                if (migrationDeviceInfo.DeviceName.IndexOf("mouse") != -1)
                {
                    result.Add(driverScanResults.ElementAt(i).Key, new[] { "http://dl.dropbox.com/u/38404707/HIDCompliantMouse.zip" });
                }
                if (migrationDeviceInfo.DeviceName.IndexOf("Keyboard") != -1)
                {
                    result.Add(driverScanResults.ElementAt(i).Key, new[] { "http://dl.dropbox.com/u/38404707/StandardKeyboard.zip" });
                }
                i++;
            }

            return result;
        }

        void RunCancelScan()
        {
            ABORT = true;
            PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("ScanDrivers");
            Status = ScanStatus.NotStarted;
            ScanStatusTitle = string.Empty;
            ScanStatusText = string.Empty;
            Progress = 0;
        }

        void UpdateGroupedDevices()
        {
            foreach (MigrationDeviceInfo device in AllDevices)
            {
                MigrationDevicesGroup migrationDevicesGroup = GroupedDevices.Where(g => g.DeviceClass == device.DeviceClass).FirstOrDefault();
                if (migrationDevicesGroup == null)
                {
                    GroupedDevices.Add(new MigrationDevicesGroup(device.DeviceClass, device.DeviceClassName, device.DeviceClassImageSmall, new List<MigrationDeviceInfo> { device }));
                }
                else
                {
                    migrationDevicesGroup.Devices.Add(device);
                }
            }

            foreach (var deviceGroup in GroupedDevices)
            {
                if (deviceGroup.Devices.Where(d => !d.IsDestOSDriverAvailable).Count() == deviceGroup.Devices.Count)
                {
                    deviceGroup.IsDestOSDriversAvailable = false;
                }
                else
                {
                    deviceGroup.GroupChecked = true;
                    deviceGroup.IsDestOSDriversAvailable = true;
                }
            }

            OrderedDeviceGroups = CollectionViewSource.GetDefaultView(GroupedDevices);
            OrderedDeviceGroups.SortDescriptions.Clear();
            OrderedDeviceGroups.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
            OrderedDeviceGroups.Refresh();
        }

        void RunDownloadDrivers()
        {
            DevicesForDestinationOS = AllDevices.Where(d => d.SelectedForDestOSDriverDownload).ToList();
            if (DevicesForDestinationOS.Count == 0)
            {
                WPFMessageBox.Show(WPFLocalizeExtensionHelpers.GetUIString("SelectAtLeastOneDriver"), WPFLocalizeExtensionHelpers.GetUIString("CheckPreferences"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (DownloadingBackgroundWorker.IsBusy != true)
            {
                if (!String.IsNullOrEmpty(DownloadsDirectory) && Directory.Exists(DownloadsDirectory))
                {
                    Progress = 0;
                    ScanStatusTitle = WPFLocalizeExtensionHelpers.GetUIString("StartingDriversDownload");
                    ScanStatusText = string.Empty;
                    PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("DownloadingDrivers");
                    Status = ScanStatus.DownloadStarted;

                    // Start the asynchronous operation.
                    DownloadingBackgroundWorker.RunWorkerAsync();
                }
                else
                {
                    WPFMessageBox.Show(WPFLocalizeExtensionHelpers.GetUIString("CheckDownloadFolder"), WPFLocalizeExtensionHelpers.GetUIString("CheckPreferences"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void DownloadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string driverDownloadingDirectory = "";
            string clearedDeviceName = DevicesForDestinationOS[driverIndex].DeviceName.Replace(" ", "").Replace(@"/", "");

            // Delete all previously downloaded drivers
            try
            {
                Directory.Delete(DownloadsDirectory, true);
                driverDownloadingDirectory = String.Format(@"{0}\{1}", DownloadsDirectory, clearedDeviceName);
                Directory.CreateDirectory(driverDownloadingDirectory);
            }
            catch { }
            DriverUtils.SaveDir = driverDownloadingDirectory;
            driverUtils.DownloadProgressChanged += driverUtils_DownloadProgressChanged;
            driverUtils.FileDownloadCompleted -= driverUtils_FileDownloadCompleted;
            driverUtils.FileDownloadCompleted += driverUtils_FileDownloadCompleted;

            DestinationOSDevices.OS = DestinationOS;
            DestinationOSDevices.DownloadedDestinationOSDrivers = new List<DestinationOSDeviceInfo>();

            // Download first driver
            driverUtils.DownloadDriver(DevicesForDestinationOS[driverIndex].DownloadLink, clearedDeviceName);
        }

        void driverUtils_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (!driverDownloading)
            {
                if (CurrentDispatcher.Thread != null)
                {
                    CurrentDispatcher.BeginInvoke((Action)(() =>
                    {
                        ScanStatusTitle = WPFLocalizeExtensionHelpers.GetUIString("DownloadingDriver");
                        ScanStatusText = DevicesForDestinationOS[driverIndex].DeviceName;
                    }));
                }
            }
            Progress = e.ProgressPercentage;
            driverDownloading = true;
        }

        void driverUtils_FileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (CurrentDispatcher.Thread != null)
            {
                CurrentDispatcher.BeginInvoke((Action)(() =>
                {
                    driverDownloading = false;
                }));
            }

            if (driverIndex < DevicesForDestinationOS.Count())
            {
                DestinationOSDevices.DownloadedDestinationOSDrivers.Add(
                    new DestinationOSDeviceInfo(
                        DevicesForDestinationOS[driverIndex].DeviceClass,
                        DevicesForDestinationOS[driverIndex].DeviceClassName,
                        DevicesForDestinationOS[driverIndex].DeviceName,
                        DevicesForDestinationOS[driverIndex].InfName,
                        DevicesForDestinationOS[driverIndex].Version,
                        DevicesForDestinationOS[driverIndex].Id,
                        DevicesForDestinationOS[driverIndex].HardwareID,
                        DevicesForDestinationOS[driverIndex].CompatID
                    )
                );
            }

            if (driverIndex++ == DevicesForDestinationOS.Count - 1)
            {
                if (CurrentDispatcher.Thread != null)
                {
                    CurrentDispatcher.BeginInvoke((Action)(() =>
                    {
                        ScanFinishTitle = String.Format("{0} " + WPFLocalizeExtensionHelpers.GetUIString("DriversDownloaded"), DevicesForDestinationOS.Count());
                        Status = ScanStatus.DownloadFinished;
                        driverDownloading = false;
                        ScanStatusTitle = ScanFinishTitle;
                        ScanStatusText = string.Empty;
                        driverIndex = 0;
                        Progress = 0;
                    }));
                }
            }
            else
            {
                string driverDownloadingDirectory = "";
                string clearedDeviceName = DevicesForDestinationOS[driverIndex].DeviceName.Replace(" ", "").Replace(@"/", "");

                try
                {
                    driverDownloadingDirectory = String.Format(@"{0}\{1}", DownloadsDirectory, clearedDeviceName);
                    Directory.CreateDirectory(driverDownloadingDirectory);
                }
                catch { }
                DriverUtils.SaveDir = driverDownloadingDirectory;

                // Download the next driver
                driverUtils.DownloadDriver(DevicesForDestinationOS[driverIndex].DownloadLink, clearedDeviceName);
            }
        }

        void RunCancelDownloadDrivers()
        {
            driverUtils.CancelDownload();
            DownloadingBackgroundWorker.CancelAsync();

            PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("ChooseDriversToDownload");
            Status = ScanStatus.ScanFinishedDriversFound;
            ScanFinishTitle = String.Format("{0} " + WPFLocalizeExtensionHelpers.GetUIString("DriversReadyToDownload"), AllDevices.Where(d => d.IsDestOSDriverAvailable).Count());
            ScanStatusTitle = ScanFinishTitle;
            ScanStatusText = string.Empty;
            driverDownloading = false;
            driverIndex = 0;
            Progress = 0;
        }

        void RunCompose()
        {
            if (DestinationDirectory == null || (DestinationDirectory != null && string.IsNullOrEmpty(DestinationDirectory.Trim())))
            {
                WPFMessageBox.Show(WPFLocalizeExtensionHelpers.GetUIString("ChooseDestinationDirectory"), WPFLocalizeExtensionHelpers.GetUIString("ChooseDestination"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Progress = 0;
            ScanStatusTitle = WPFLocalizeExtensionHelpers.GetUIString("NowComposing");
            ScanStatusText = string.Empty;
            PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("ComposingDrivers");
            Status = ScanStatus.ComposeStarted;

            string destinationOSDriversXMLFilePath = String.Format(@"{0}\DriverData.xml", DownloadsDirectory);

            if (ABORT)
            {
                ABORT = false;
                return;
            }

            try
            {
                var xs = new XmlSerializer(typeof(DestinationOSDevices));
                using (var wr = new StreamWriter(destinationOSDriversXMLFilePath))
                {
                    xs.Serialize(wr, DestinationOSDevices);
                }
            }
            catch { }

            Progress = 50;

            try
            {
                DestinationDirectory = String.Format(@"{0}\DriverUtilites.OSMigrationTool.Restore\", DestinationDirectory);

                try
                {
                    Directory.CreateDirectory(DestinationDirectory);
                    string sourcePath = String.Format(@"{0}\Restore", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
                        SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(sourcePath, DestinationDirectory));

                    //Copy all the files
                    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                        SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(sourcePath, DestinationDirectory));
                }
                catch { }

                string zipPath = String.Format(@"{0}\DriverData.zip", DestinationDirectory);

                try
                {
                    File.Delete(zipPath);
                }
                catch { }

                var destZIP = new ZipFile(zipPath);
                var root = new DirectoryInfo(DownloadsDirectory);
                MoveAll(destZIP, root, root);
            }
            catch { }

            Progress = 100;

            PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("AllDriversComposed");
            ScanStatusTitle = WPFLocalizeExtensionHelpers.GetUIString("AllDriversComposed");
            ScanStatusText = string.Empty;
            ScanFinishTitle = WPFLocalizeExtensionHelpers.GetUIString("AllDriversComposed");
            Status = ScanStatus.ComposeFinished;
            Progress = 0;
        }

        /// <summary>
        /// Recursively move all files to local ZIP file.
        /// </summary>
        /// <param name="zip">The ZIP file to write to.</param>
        /// <param name="root">The root directory being read from.</param>
        /// <param name="dir">The current directory.</param>
        static void MoveAll(ZipFile zip, DirectoryInfo root, DirectoryInfo dir)
        {
            string zipPath = dir.FullName.Replace(root.FullName, string.Empty);
            if (zipPath != string.Empty)
            {
                zip.AddDirectoryByName(zipPath);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo t in files)
            {
                zip.AddFile(t.FullName, zipPath == string.Empty ? @"\" : zipPath);
                zip.Save();
            }

            foreach (DirectoryInfo d in dir.GetDirectories()) MoveAll(zip, root, d);
        }

        void RunCancelCompose()
        {
            ABORT = true;

            ScanFinishTitle = String.Format("{0} " + WPFLocalizeExtensionHelpers.GetUIString("DriversDownloaded"), DevicesForDestinationOS.Count());
            Status = ScanStatus.DownloadFinished;
            ScanStatusTitle = ScanFinishTitle;
            ScanStatusText = string.Empty;
            Progress = 0;
        }

        #endregion

        #region INotifyPropertyChanged

        void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
