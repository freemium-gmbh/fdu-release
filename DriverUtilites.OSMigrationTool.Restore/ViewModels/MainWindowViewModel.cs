using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
using DriverUtilites.Models;
using DriverUtilites.OSMigrationTool.Restore.Infrastructure;
using DriverUtilites.OSMigrationTool.Restore.Models;
using DriverUtilites.Utils;
using Ionic.Zip;
using MessageBoxUtils;

namespace DriverUtilites.OSMigrationTool.Restore.ViewModels
{
	class MainWindowViewModel : INotifyPropertyChanged
	{
		#region Constructors

		public MainWindowViewModel()
		{
			CurrentDispatcher = Dispatcher.CurrentDispatcher;

			#region Commands initialization

			checkDevicesGroupCommand = new SimpleCommand
			{
				ExecuteDelegate = CheckDevicesGroup
			};

			checkDeviceCommand = new SimpleCommand
			{
				ExecuteDelegate = CheckDevice
			};

			installCommand = new SimpleCommand
			{
				ExecuteDelegate = x => Install()
			};

			cancelInstallCommand = new SimpleCommand
			{
				ExecuteDelegate = x => CancelInstall()
			};

			closeCommand = new SimpleCommand
			{
				ExecuteDelegate = x => Close()
			};

			#endregion
		}

		#endregion

		#region Commands

		ICommand checkDevicesGroupCommand;
		public ICommand CheckDevicesGroupCommand
		{
			get { return checkDevicesGroupCommand; }
		}
		void CheckDevicesGroup(object selectedDestinationOSDevicesGroup)
		{
			DestinationOSDevicesGroup destinationOSDevicesGroup = GroupedDevices.Where(d => d.DeviceClass == (string)selectedDestinationOSDevicesGroup).FirstOrDefault();
			if (destinationOSDevicesGroup != null)
			{
				foreach (DestinationOSDeviceInfo item in destinationOSDevicesGroup.Drivers)
				{
					item.SelectedForInstall = destinationOSDevicesGroup.GroupChecked;
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
			DestinationOSDeviceInfo device = allDrivers.DownloadedDestinationOSDrivers.Where(d => d.Id == (string)selectedDevice).FirstOrDefault();
			if (device != null)
			{
				DestinationOSDevicesGroup destinationOSDevicesGroup = GroupedDevices.Where(d => d.DeviceClass == device.DeviceClass).FirstOrDefault();
				if (destinationOSDevicesGroup != null && destinationOSDevicesGroup.Drivers.Where(d => d.SelectedForInstall).Count() == 0)
				{
					destinationOSDevicesGroup.GroupChecked = false;
				}
				else
				{
					if (destinationOSDevicesGroup != null && destinationOSDevicesGroup.Drivers.Where(d => d.SelectedForInstall == false).Count() == 0)
					{
						destinationOSDevicesGroup.GroupChecked = true;
					}
				}
			}
		}

		ICommand installCommand;
		public ICommand InstallCommand
		{
			get { return installCommand; }
		}
		void Install()
		{
			RunInstall();
		}

		ICommand cancelInstallCommand;
		public ICommand CancelInstallCommand
		{
			get { return cancelInstallCommand; }
		}
		void CancelInstall()
		{
			RunCancelInstall();
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

		public string ZipToUnpack = "DriverData.zip";
		Dispatcher CurrentDispatcher;
		string tempDirectory;
		DriverUtils driverUtils = new DriverUtils();
		bool ABORT;

		int DriverIndex;

		InstallStatus status = InstallStatus.NotStarted;
		public InstallStatus Status
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

		string installStatusTitle;
		public string InstallStatusTitle
		{
			get
			{
				return installStatusTitle;
			}
			set
			{
				installStatusTitle = value;
				OnPropertyChanged("InstallStatusTitle");
			}
		}

		string installStatusText;
		public string InstallStatusText
		{
			get
			{
				return installStatusText;
			}
			set
			{
				installStatusText = value;
				OnPropertyChanged("InstallStatusText");
			}
		}

		string installFinishTitle;
		public string InstallFinishTitle
		{
			get
			{
				return installFinishTitle;
			}
			set
			{
				installFinishTitle = value;
				OnPropertyChanged("InstallFinishTitle");
			}
		}

		string panelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("SelectDrivers");
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

		DestinationOSDevices allDrivers = new DestinationOSDevices();
		List<DestinationOSDeviceInfo> driversToInstall;

		ObservableCollection<DestinationOSDevicesGroup> groupedDevices = new ObservableCollection<DestinationOSDevicesGroup>();
		public ObservableCollection<DestinationOSDevicesGroup> GroupedDevices
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

		#endregion

		#region Private methods

		public void UnZipDriverData()
		{
			tempDirectory = String.Format(@"{0}\Driver Utilites\Temp", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			try
			{
				Directory.Delete(tempDirectory);
				Directory.CreateDirectory(tempDirectory);
			}
			catch { }

			if (File.Exists(ZipToUnpack))
			{
				string unpackDirectory = tempDirectory;
				using (ZipFile zipFile = ZipFile.Read(ZipToUnpack))
				{
					// here we extract every entry
					foreach (ZipEntry e in zipFile)
					{
						e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
					}
				}
			}
			else
			{
				WPFMessageBox.Show(WPFLocalizeExtensionHelpers.GetUIString("DriverDataNotFound"), WPFLocalizeExtensionHelpers.GetUIString("FileNotFound"), MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}
		}

		/// <summary>
		/// Reads driver data from the XML file
		/// </summary>
		public void ReadDriverDataFromXML()
		{
			string driverDataXMLFilePath = String.Format(@"{0}\DriverData.xml", tempDirectory);
			if (File.Exists(driverDataXMLFilePath))
			{
				try
				{
					var xs = new XmlSerializer(typeof(DestinationOSDevices));
					using (var rd = new StreamReader(driverDataXMLFilePath))
					{
						allDrivers = xs.Deserialize(rd) as DestinationOSDevices;
					}

					string OSInfo = GetOSInfo();
					if (allDrivers != null && allDrivers.OS != OSInfo)
					{
						WPFMessageBox.Show(
							String.Format(WPFLocalizeExtensionHelpers.GetUIString("OSMismatchText"), allDrivers.OS, OSInfo),
							WPFLocalizeExtensionHelpers.GetUIString("OSMismatch"),
							MessageBoxButton.OK,
							MessageBoxImage.Error
						);
						Close();
					}

					UpdateGroupedDevices();
				}
				catch { }
			}
			else
			{
				WPFMessageBox.Show(WPFLocalizeExtensionHelpers.GetUIString("DriverDataXMLNotFound"), WPFLocalizeExtensionHelpers.GetUIString("FileNotFound"), MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}
		}

		/// <summary>
		/// Analyzes the <c>Environment.OSVersion</c> value and returns a string which represents the current OS info
		/// </summary>
		/// <returns>A string which represents the current OS info</returns>
		string GetOSInfo()
		{
			//Get Operating system information.
			OperatingSystem os = Environment.OSVersion;
			//Get version information about the os.
			Version vs = os.Version;

			//Variable to hold our return value
			string operatingSystem = "";

			if (os.Platform == PlatformID.Win32NT)
			{
				switch (vs.Major)
				{
					case 3:
						operatingSystem = "NT 3.51";
						break;
					case 4:
						operatingSystem = "NT 4.0";
						break;
					case 5:
						operatingSystem = vs.Minor == 0 ? "2000" : "XP";
						break;
					case 6:
						if (vs.Minor == 0)
							operatingSystem = "Vista";
						if (vs.Minor == 1)
							operatingSystem = "7";
						if (vs.Minor == 2)
							operatingSystem = "8";
						break;
				}
			}
			//Make sure we actually got something in our OS check
			//We don't want to just return " Service Pack 2" or " 32-bit"
			//That information is useless without the OS version.
			if (operatingSystem != "")
			{
				//Got something.  Let's prepend "Windows" and get more info.
				operatingSystem = "Windows " + operatingSystem;

				//See if there's a service pack installed.
				//if (os.ServicePack != "")
				//{
				//    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
				//    operatingSystem += " " + os.ServicePack;
				//}

				//Append the OS architecture.  i.e. "Windows XP 32-bit"
				operatingSystem += " " + GetOSArchitecture().ToString() + "-bit";
			}
			//Return the information we've gathered.
			return operatingSystem;
		}

		#region Detecting Windows 64 bit platform

		[DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		public extern static IntPtr LoadLibrary(string libraryName);

		[DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		public extern static IntPtr GetProcAddress(IntPtr hwnd, string procedureName);

		private delegate bool IsWow64ProcessDelegate([In] IntPtr handle, [Out] out bool isWow64Process);

		public static int GetOSArchitecture()
		{
			if (IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor()))
			{
				return 64;
			}
			return 32;
		}

		private static IsWow64ProcessDelegate GetIsWow64ProcessDelegate()
		{
			IntPtr handle = LoadLibrary("kernel32");

			if (handle != IntPtr.Zero)
			{
				IntPtr fnPtr = GetProcAddress(handle, "IsWow64Process");

				if (fnPtr != IntPtr.Zero)
				{
					return (IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer(fnPtr, typeof(IsWow64ProcessDelegate));
				}
			}

			return null;
		}

		private static bool Is32BitProcessOn64BitProcessor()
		{
			IsWow64ProcessDelegate fnDelegate = GetIsWow64ProcessDelegate();

			if (fnDelegate == null)
			{
				return false;
			}

			bool isWow64;
			bool retVal = fnDelegate.Invoke(Process.GetCurrentProcess().Handle, out isWow64);

			if (retVal == false)
			{
				return false;
			}

			return isWow64;
		}

		#endregion

		void UpdateGroupedDevices()
		{
			foreach (DestinationOSDeviceInfo driver in allDrivers.DownloadedDestinationOSDrivers)
			{
				DestinationOSDevicesGroup destinationOSDevicesGroup = GroupedDevices.Where(g => g.DeviceClass == driver.DeviceClass).FirstOrDefault();
				if (destinationOSDevicesGroup == null)
				{
					GroupedDevices.Add(new DestinationOSDevicesGroup(driver.DeviceClass, driver.DeviceClassName, driver.DeviceClassImageSmall, new List<DestinationOSDeviceInfo> { driver }));
				}
				else
				{
					destinationOSDevicesGroup.Drivers.Add(driver);
				}
			}

			OrderedDeviceGroups = CollectionViewSource.GetDefaultView(GroupedDevices);
			OrderedDeviceGroups.SortDescriptions.Clear();
			OrderedDeviceGroups.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
			OrderedDeviceGroups.Refresh();
		}

		void RunInstall()
		{
			driversToInstall = allDrivers.DownloadedDestinationOSDrivers.Where(d => d.SelectedForInstall).ToList();
			if (driversToInstall.Count == 0)
			{
				WPFMessageBox.Show(WPFLocalizeExtensionHelpers.GetUIString("SelectAtLeastOneDriver"), WPFLocalizeExtensionHelpers.GetUIString("SelectDrivers"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}

			Progress = 0;
			InstallStatusTitle = WPFLocalizeExtensionHelpers.GetUIString("InstallingDrivers");
			InstallStatusText = string.Empty;
			PanelScanHeader = WPFLocalizeExtensionHelpers.GetUIString("InstallingDrivers");
			Status = InstallStatus.Started;
			DriverIndex = 0;

			if (ABORT)
			{
				ABORT = false;
				return;
			}

			// Install first driver
			string clearedDeviceName = driversToInstall[DriverIndex].DeviceName.Replace(" ", "").Replace(@"/", "");
			DriverUtils.SaveDir = String.Format(@"{0}\{1}", tempDirectory, clearedDeviceName);
			driverUtils.InstallDriver(driversToInstall[DriverIndex], DriverUtilsInstallProgressChanged, driverUtils_FileInstallCompleted, clearedDeviceName);
		}

		public void DriverUtilsInstallProgressChanged(int progress, object data)
		{
			if (ABORT)
			{
				return;
			}

			Progress = (progress / 100) / driversToInstall.Count * 100;
		}

		public void driverUtils_FileInstallCompleted(int progress, object data)
		{
			if (ABORT)
			{
				return;
			}

			Progress = DriverIndex / driversToInstall.Count * 100;

			if (DriverIndex == driversToInstall.Count - 1)
			{
				if (CurrentDispatcher.Thread != null)
				{
					CurrentDispatcher.BeginInvoke((Action)(() =>
					{
						InstallFinishTitle = String.Format("{0} " + WPFLocalizeExtensionHelpers.GetUIString("DriversInstalled"), driversToInstall.Count());
						Status = InstallStatus.Finished;
						InstallStatusTitle = InstallFinishTitle;
						InstallStatusText = "";
						DriverIndex = 0;
					}));
				}
			}
			else
			{
				// Install next driver from list.
				DriverIndex++;
				string clearedDeviceName = driversToInstall[DriverIndex].DeviceName.Replace(" ", "").Replace(@"/", "");
				DriverUtils.SaveDir = String.Format(@"{0}\{1}", tempDirectory, clearedDeviceName);
				driverUtils.InstallDriver(driversToInstall[DriverIndex], DriverUtilsInstallProgressChanged, driverUtils_FileInstallCompleted, clearedDeviceName);
			}
		}


		void RunCancelInstall()
		{
			ABORT = true;

			InstallFinishTitle = String.Format("{0} " + WPFLocalizeExtensionHelpers.GetUIString("DriversInstalled"), DriverIndex + 1);
			Status = InstallStatus.NotStarted;
			InstallStatusTitle = InstallFinishTitle;
			InstallStatusText = string.Empty;
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
