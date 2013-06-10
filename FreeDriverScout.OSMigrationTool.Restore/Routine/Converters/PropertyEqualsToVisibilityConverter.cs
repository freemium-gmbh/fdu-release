using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FreeDriverScout.OSMigrationTool.Restore.Models;

namespace FreeDriverScout.OSMigrationTool.Restore.Routine
{
	public sealed class PropertyEqualsToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			InstallStatus s1 = (InstallStatus)value;
			InstallStatus s2 = (InstallStatus)parameter;
			return s1 == s2 ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
