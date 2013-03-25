using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLocalizeExtension.Extensions;

namespace DriverUtilites.OSMigrationTool.Backup.Infrastructure
{
	public class WPFLocalizeExtensionHelpers
	{
		public static string GetUIString(string key)
		{
			string uiString;
			LocTextExtension locExtension = new LocTextExtension(String.Format("DriverUtilites.OSMigrationTool.Backup:Resources:{0}", key));
			locExtension.ResolveLocalizedValue(out uiString);
			return uiString;
		}
	}
}
