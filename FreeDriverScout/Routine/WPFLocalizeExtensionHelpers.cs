using System;
using WPFLocalizeExtension.Extensions;

namespace FreeDriverScout.Infrastructure
{
	public class WPFLocalizeExtensionHelpers
	{
		public static string GetUIString(string key)
		{
			string uiString;
			LocExtension locExtension = new LocExtension(String.Format("FreeDriverScout:Resources:{0}", key));
			locExtension.ResolveLocalizedValue(out uiString);
			return uiString;
		}
	}
}
