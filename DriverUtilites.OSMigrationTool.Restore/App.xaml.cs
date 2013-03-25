using System.Linq;
using System.Windows;
using DriverUtilites.OSMigrationTool.Restore.ViewModels;

namespace DriverUtilites.OSMigrationTool.Restore
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			var window = new MainWindow();
			var viewModel = new MainWindowViewModel();

			if (e.Args.Any())
			{
				viewModel.ZipToUnpack = e.Args[0];
			}

			// Wiring up View and ViewModel
			window.DataContext = viewModel;

			viewModel.UnZipDriverData();
			viewModel.ReadDriverDataFromXML();

			window.Show();
		}
	}
}
