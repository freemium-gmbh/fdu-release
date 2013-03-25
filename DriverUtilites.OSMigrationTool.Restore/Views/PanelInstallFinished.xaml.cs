using System.Windows.Controls;
using System.Diagnostics;

namespace DriverUtilites.OSMigrationTool.Restore.Views
{
	/// <summary>
	/// Interaction logic for PanelInstallFinished.xaml
	/// </summary>
	public partial class PanelInstallFinished : UserControl
	{
		public PanelInstallFinished()
		{
			InitializeComponent();
		}

		private void Click_Facebook(object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo(@"http://www.freemium.com/fsu/facebook"));
		}

		private void Click_Twitter(object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo(@"http://www.freemium.com/fsu/twitter"));
		}

		private void Click_GooglePlus(object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo(@"http://www.freemium.com/fsu/googleplus"));
		}
	}
}
	