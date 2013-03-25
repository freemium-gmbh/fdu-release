using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Windows.Forms;
using BugReporter.FreemiumWebService;
using Routine;

namespace BugReporter
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			txtException.Text = Program.BugStackTrace;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

		public static bool Is64Bit()
		{
			bool retVal;
			IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
			return retVal;
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			// OS info
			var os = Environment.OSVersion.VersionString;
			if (Is64Bit())
				os += " 64-bit";
			else
				os += " 32-bit";

			// MAC address
			var mac = "";
			var interfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface intrfce in interfaces)
			{
				mac = intrfce.GetPhysicalAddress().ToString();
				if (!string.IsNullOrEmpty(mac))
					break;
			}

			// IP address
			var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
			var ip = ipEntry.AddressList[0].ToString();

			// Host name
			var hostName = Dns.GetHostName();

			string stackTrace = "";
			try
			{
				stackTrace = GZipStreamStringHelper.Zip(Program.BugStackTrace);
			}
			catch { }

			// Hide window
			ShowInTaskbar = false;
			Hide();

			// Submit report
			var binding = new BasicHttpBinding();
			var address = new EndpointAddress("http://driverservice.freemiumlab.com/ReportService.svc");
			//EndpointAddress address = new EndpointAddress("http://localhost:12758/ReportService.svc");

			ReportServiceClient client = null;
			try
			{
				client = new ReportServiceClient(binding, address);
			}
			catch (Exception ex)
			{
				// MessageBox.Show(ex.Message + " " + ex.Source + " " + ex.InnerException);
			}

			try
			{
				string str_ver = "";
				string str_mac = "";
				string str_ip = "";
				string str_os = "";
				string str_host = "";
				string str_stacktrace = "";
				string str_bugtype = "";
				string str_bugmsg = "";
				string str_userinput = "";
				string str_target = "";
				string str_source = "";

				try { if (Program.Version.Length > 0)str_ver = Program.Version; }
				catch { }
				try { if (mac.Length > 0)str_mac = mac; }
				catch { }
				try { if (ip.Length > 0)str_ip = ip; }
				catch { }
				try { if (os.Length > 0)str_os = os; }
				catch { }
				try { if (hostName.Length > 0)str_host = hostName; }
				catch { }
				try { if (stackTrace.Length > 0)str_stacktrace = stackTrace; }
				catch { }
				try { if (Program.BugType.Length > 0)str_bugtype = Program.BugType; }
				catch { }
				try { if (Program.BugMessage.Length > 0)str_bugmsg = Program.BugMessage; }
				catch { }
				try { if (txtContext.Text.Length > 0)str_userinput = txtContext.Text; }
				catch { }
				try { if (Program.BugTargetSite.Length > 0)str_target = Program.BugTargetSite; }
				catch { }
				try { if (Program.BugSource.Length > 0)str_source = Program.BugSource; }
				catch { }

				client.SubmitBugReport(
					str_ver,
					str_mac,
					str_ip,
					str_os,
					str_host,
					str_stacktrace,
					str_bugtype,
					str_bugmsg,
					str_userinput,
					str_target,
					str_source
					);

				client.SubmitNormalReport(
					str_ver,
					str_userinput,
					str_mac,
					str_ip,
					str_os,
					str_host);
			}
			catch { }
			client.Close();

			// Close program
			Application.Exit();
		}

		private void detailsBtn_Click(object sender, EventArgs e)
		{
			if ((string)(btnDetails.Tag) == "Show")
			{
				Height = 333;
				btnDetails.Tag = "Hide";
				btnDetails.Text = "<< &Details";
			}
			else
			{
				Height = 190;
				btnDetails.Tag = "Show";
				btnDetails.Text = "&Details >>";
			}
		}
	}
}
