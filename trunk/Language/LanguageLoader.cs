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
					Languages.Add(Language.Parse(file));
				}
				catch
				{
				}
			}
		}

		public static Language FindLanguage(string p)
		{
			foreach (var l in Languages)
				if (l.Name == p)
					return l;

			return null;
		}
	}
}
