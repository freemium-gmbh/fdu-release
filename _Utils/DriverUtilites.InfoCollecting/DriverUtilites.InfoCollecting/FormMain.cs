using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Management;
using System.Windows.Forms;
using System.Xml.Serialization;
using DriverUtilites.InfoCollecting.Models;
using DriverUtilites.InfoCollecting.Utils;
using Microsoft.Win32;

namespace DriverUtilites.InfoCollecting
{
	public partial class frmMain : Form
	{
		int progress;
		ScanStatus status;
		readonly DriverUtils driverUtils = new DriverUtils();
		readonly List<DeviceInfo> allDevices = new List<DeviceInfo>();
		readonly RegistryKey deviceClasses = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\", true);
		const string DevicesXMLFilePath = "devices.xml";
		readonly BackgroundWorker worker = new BackgroundWorker();

		public frmMain()
		{
			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			InitializeBackgoundWorker();
			worker.RunWorkerAsync();
		}

		// Set up the BackgroundWorker object by 
		// attaching event handlers. 
		private void InitializeBackgoundWorker()
		{
			worker.WorkerSupportsCancellation = true;
			worker.WorkerReportsProgress = true;
			worker.DoWork += worker_DoWork;
			worker.RunWorkerCompleted += worker_RunWorkerCompleted;
			worker.ProgressChanged += worker_ProgressChanged;
		}

		// This event handler is where the actual,
		// potentially time-consuming work is done.
		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			CollectInfo();
		}

		// This event handler deals with the results of the
		// background operation.
		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// First, handle the case where an exception was thrown.
			if (e.Error != null)
			{
				MessageBox.Show(e.Error.Message);
			}
			else if (e.Cancelled)
			{
				// Next, handle the case where the user canceled 
				// the operation.
				// Note that due to a race condition in 
				// the DoWork event handler, the Cancelled
				// flag may not have been set, even though
				// CancelAsync was called.
				lblResult.Text = "Canceled";
			}
			else
			{
				// Finally, handle the case where the operation 
				// succeeded.
				lblResult.Text = "Done!";
			}

			// Disable the Cancel button.
			//cancelAsyncButton.Enabled = false;
		}

		// This event handler updates the progress bar.
		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			prbMain.Value = e.ProgressPercentage;
		}

		void SaveAllDevicesToXML()
		{
			try
			{
				if (!File.Exists(DevicesXMLFilePath))
				{
					File.Create(DevicesXMLFilePath);
				}

				var xs = new XmlSerializer(typeof(List<DeviceInfo>));
				using (var wr = new StreamWriter(DevicesXMLFilePath))
				{
					xs.Serialize(wr, allDevices);
				}
			}
			catch { }
		}

		/// <summary>
		/// Collects the data about devices at the machine.
		/// </summary>
		void CollectInfo()
		{
			try
			{
				if (status == ScanStatus.NotStarted)
				{
					progress = 0;
					status = ScanStatus.ScanStarted;

					var devices = driverUtils.ScanDevices();
					int allDevicesProcessed = 1;

					foreach (ManagementObject device in devices)
					{
						progress = Convert.ToInt32((decimal)allDevicesProcessed++ / devices.Count * 100);
						worker.ReportProgress(progress);

						var flag = false;
						if (device.GetPropertyValue("DeviceClass") == null)
							continue;
						var deviceClass = device.GetPropertyValue("DeviceClass").ToString();
						foreach (var restricted in DriverUtils.RestrictedClasses)
						{
							if (restricted != deviceClass) continue;
							flag = true;
							break;
						}

						if (flag)
						{
							continue;
						}

						//Uses device friendly name instead of device name when it's possible
						string deviceName;
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

						string date = null;
						var installedDriverDateStr = (string)(device.GetPropertyValue("DriverDate"));
						if (!string.IsNullOrEmpty(installedDriverDateStr))
						{
							int year = int.Parse(installedDriverDateStr.Substring(0, 4));
							int month = int.Parse(installedDriverDateStr.Substring(4, 2));
							int day = int.Parse(installedDriverDateStr.Substring(6, 2));
							DateTime installedDriverDate = new DateTime(year, month, day);
							date = installedDriverDate.ToShortDateString();
						}

						allDevices.Add(
							new DeviceInfo(
								(string)(device.GetPropertyValue("DeviceClass")),
								deviceClassName,
								deviceName,
								(string)(device.GetPropertyValue("InfName")),
								(string)(device.GetPropertyValue("DriverVersion")),
								(string)(device.GetPropertyValue("DeviceId")),
								(string)(device.GetPropertyValue("HardwareID")),
								(string)(device.GetPropertyValue("CompatID")),
								date
							)
						);

						SaveAllDevicesToXML();
					}
				}
			}
			catch { }
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			worker.CancelAsync();
		}
	}
}
