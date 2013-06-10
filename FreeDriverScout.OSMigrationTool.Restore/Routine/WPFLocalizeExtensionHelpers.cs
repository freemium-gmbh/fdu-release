using System;
using WPFLocalizeExtension.Extensions;

namespace FreeDriverScout.OSMigrationTool.Restore.Infrastructure
{
	public class WPFLocalizeExtensionHelpers
	{
		public static string GetUIString(string key)
		{
			string uiString;
			LocTextExtension locExtension = new LocTextExtension(String.Format("FreeDriverScout.OSMigrationTool.Restore:Resources:{0}", key));
			locExtension.ResolveLocalizedValue(out uiString);
			return uiString;
		}
	}
}
