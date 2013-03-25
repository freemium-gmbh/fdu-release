using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Animation;
using sysUtils;

namespace FreeDriverScout
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static bool startMinimized;
		public static bool Click1;
		System.Windows.Forms.NotifyIcon notifyIcon;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			//Reducing the frames per second on the WPF animations update
			Timeline.DesiredFrameRateProperty.OverrideMetadata(
				typeof(Timeline),
				new FrameworkPropertyMetadata { DefaultValue = 10 }
				);

			//this line is to make any unhandled exception happening, get directed to the unhandled exception method
			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			//this line sets the file name of the bug reporter exe (which takes the exception data and send it to webservice)
			//Reporting.BugReporterPath = "BugReporter.exe";

			Process thisProc = Process.GetCurrentProcess();
			if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
			{
				// If ther is more than one, than it is already running.
				Environment.Exit(0);
			}

			Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString());

			startMinimized = false;
			Click1 = false;
			for (int i = 0; i != e.Args.Length; ++i)
			{
				if ((e.Args[i] == "StartHidden") || (e.Args[i] == "Click1"))
				{
					startMinimized = true;
				}
			}

			notifyIcon = new System.Windows.Forms.NotifyIcon
							{
								Icon = FreeDriverScout.Properties.Images.driver,
								Text = "Free Driver Scout",
								Visible = true
							};
			notifyIcon.DoubleClick +=
			delegate
			{
				MainWindow.Show();
				MainWindow.ShowInTaskbar = true;
				MainWindow.WindowState = WindowState.Normal;
				MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			};
			notifyIcon.MouseUp += notifyIcon_MouseClick;

			CfgFile.CfgFilePath = "freemium.cfg";
		}

		public void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				var menu = (System.Windows.Controls.ContextMenu)MainWindow.FindResource("RightClickSystemTray");
				menu.IsOpen = true;
			}
		}

		//private void Window_Closed(object sender, CancelEventArgs e)
		//{
		//    notifyIcon.Visible = false;
		//    notifyIcon = null;
		//}

		//private void OnApplicationExit(object sender, EventArgs e)
		//{
		//    notifyIcon.Visible = false;
		//    notifyIcon = null;
		//}

		//private void Application_Deactivated(object sender, EventArgs e)
		//{
		//    notifyIcon.Visible = false;
		//    notifyIcon = null;
		//}

		void Application_Exit(object sender, ExitEventArgs e)
		{
			if (notifyIcon != null)
			{
				notifyIcon.Dispose();
			}
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			Reporting.Report(e.Exception);
			Process.GetCurrentProcess().Kill();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Reporting.Report((Exception)(e.ExceptionObject));
			Process.GetCurrentProcess().Kill();
		}
	}
}
