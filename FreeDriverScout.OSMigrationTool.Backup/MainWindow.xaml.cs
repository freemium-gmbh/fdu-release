﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using FreeDriverScout.OSMigrationTool.Backup.ViewModels;

namespace FreeDriverScout.OSMigrationTool.Backup
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			string path = String.Format(@"Themes/{0}/Theme.xaml", "Blue");
			using (var fs = new FileStream(path, FileMode.Open))
			{
				var dic = (ResourceDictionary)XamlReader.Load(fs);
				Application.Current.MainWindow.Resources.MergedDictionaries.Clear();
				Application.Current.MainWindow.Resources.MergedDictionaries.Add(dic);
			}

			// Wiring up View and ViewModel
			var model = new MainWindowViewModel();
			DataContext = model;
		}

		#region Window operations

		public void DragWindow(object sender, MouseButtonEventArgs args)
		{
			DragMove();
		}

		void CloseApp(object sender, RoutedEventArgs e)
		{
			//if (CfgFile.Get("MinimizeToTray") == "1")
			//{
			//    this.Hide();
			//}
			//else
			{
				Application.Current.Shutdown();
			}
		}

		//void AppExit(object sender, RoutedEventArgs e)
		//{
		//    Application.Current.Shutdown();
		//}

		void MinimizeApp(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
			//if (CfgFile.Get("MinimizeToTray") == "1")
				//this.Hide();
		}

		#endregion
	}
}
