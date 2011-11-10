using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCSkin3D.Language
{
	public static class LanguageLoader
	{
		public static List<Language> Languages = new List<Language>();

		public static void LoadLanguages(string path)
		{
			foreach (var file in Directory.GetFiles(path, "*.lang"))
			{
				try
				{
					using (var sr = new StreamReader(file, Encoding.Unicode))
						Languages.Add(Language.Parse(sr));
				}
				catch
				{
				}
			}
		}

		public static Language FindLanguage(string p)
		{
			foreach (var l in Languages)
				if (l.Name == p ||
					l.Culture.TwoLetterISOLanguageName == p)
					return l;

			return null;
		}

		public static Language LoadDefault()
		{
			using (var writer = new FileStream("Languages\\English.lang", FileMode.Create))
				writer.Write(Properties.Resources.English, 0, Properties.Resources.English.Length);

			using (var reader = new StreamReader(new MemoryStream(Properties.Resources.English), Encoding.Unicode))
			{
				var lang = Language.Parse(reader);
				Languages.Add(lang);
				return lang;
			}
		}
	}
}
