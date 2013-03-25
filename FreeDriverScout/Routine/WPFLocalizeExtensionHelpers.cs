using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLocalizeExtension.Extensions;

namespace FreeDriverScout.Infrastructure
{
	public class WPFLocalizeExtensionHelpers
	{
		public static string GetUIString(string key)
		{
			string uiString;
			LocTextExtension locExtension = new LocTextExtension(String.Format("FreeDriverScout:Resources:{0}", key));
			locExtension.ResolveLocalizedValue(out uiString);
			return uiString;
		}
	}
}
