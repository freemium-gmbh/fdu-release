using System.Collections.Generic;
using System.ComponentModel;

namespace DriverUtilites.InfoCollecting.Models
{
	public class DeviceInfoBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Default constructor for the <c>XmlSerializer</c>
		/// </summary>
		public DeviceInfoBase()
		{
		}

		public DeviceInfoBase(string deviceClass, string deviceClassName, string deviceName, string infName, string version, string id, string hardwareID, string compatID)
		{
			DeviceClass = deviceClass != null ? deviceClass : "null";
			DeviceClassName = deviceClassName != null ? deviceClassName : "null";
			DeviceName = deviceName != null ? deviceName : "null";
			InfName = infName != null ? infName : "null";
			Version = version != null ? version : "null";
			Id = id != null ? id : "null";
			HardwareID = hardwareID != null ? hardwareID : "null";
			CompatID = compatID != null ? compatID : "null";
		}

		string deviceClass;
		public string DeviceClass
		{
			get
			{
				return deviceClass;
			}
			set
			{
				try
				{
					deviceClass = value;
				}
				catch { }
			}
		}
		public string DeviceClassName { get; set; }
		public string DeviceClassImage { get; set; }
		public string DeviceClassImageSmall { get; set; }
		public string DeviceName { get; set; }
		public string Version { get; set; }
		public string InfName { get; set; }
		public string Id { get; set; }
		public string HardwareID { get; set; }
		public string CompatID { get; set; }
		public string DownloadLink { get; set; }

		#region INotifyPropertyChanged

		protected void OnPropertyChanged(string property)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	};
}
