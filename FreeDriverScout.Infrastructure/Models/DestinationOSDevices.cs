using System.Collections.Generic;
using System.ComponentModel;

namespace FreeDriverScout.Models
{
	public class DestinationOSDevices
	{
		/// <summary>
		/// Default constructor for the <c>XmlSerializer</c>
		/// </summary>
		public DestinationOSDevices()
		{

		}

		public int OS { get; set; }

		public List<DestinationOSDeviceInfo> DownloadedDestinationOSDrivers { get; set; }
	};
}
