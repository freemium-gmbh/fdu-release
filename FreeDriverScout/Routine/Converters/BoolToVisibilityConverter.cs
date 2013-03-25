using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FreeDriverScout.Routine
{
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			bool s1 = (bool)value;
			bool s2 = bool.Parse(parameter.ToString());
			return s1 == s2 ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
