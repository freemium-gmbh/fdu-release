namespace DriverUtilites.InfoCollecting.Models
{
	public class DeviceInfo : DeviceInfoBase
	{
		/// <summary>
		/// Default constructor for the <c>XmlSerializer</c>
		/// </summary>
		public DeviceInfo()
		{ }

		public DeviceInfo(string deviceClass, string deviceClassName, string deviceName, string infName, string version, string id, string hardwareID, string compatID, string date)
		{
			DeviceClass = deviceClass ?? "null";
			DeviceClassName = deviceClassName ?? "null";
			DeviceName = deviceName ?? "null";
			InfName = infName ?? "null";
			Version = version ?? "null";
			Id = id ?? "null";
			HardwareID = hardwareID ?? "null";
			CompatID = compatID ?? "null";
			InstalledDriverDate = date ?? "null";
		}

		public string InstalledDriverDate { get; set; }
	};
}
