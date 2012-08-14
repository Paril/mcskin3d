using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSkin3D.Languages
{
	public static class LanguageHandler
	{
		static List<LanguageProvider> _providers = new List<LanguageProvider>();

		public static void RegisterProvider(LanguageProvider provider)
		{
			_providers.Add(provider);

			if (Language != null)
				provider.Language = Language;
		}

		public static void UnregisterProvider(LanguageProvider provider)
		{
			_providers.Remove(provider);
		}

		static Language _language = null;
		public static Language Language
		{
			get { return _language; }

			set
			{
				_language = value;

				foreach (var provider in _providers)
					provider.Language = _language;
			}
		}
	}
}
