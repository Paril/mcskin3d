using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MCSkin3D.Macros
{
	public static class MacroHandler
	{
		static Dictionary<string, string> _macros = new Dictionary<string, string>();

		public static Dictionary<string, string> Macros
		{
			get { return _macros; }
		}

		public static void RegisterMacro(string name, string value)
		{
			if (_macros.ContainsKey(name))
				_macros[name] = value;
			else
				_macros.Add(name, value);
		}

		public static string ReplaceMacros(string s)
		{
			var matches = Regex.Matches(s, "\\$\\((.*)\\)");

			foreach (Match m in matches)
			{
				if (_macros.ContainsKey(m.Groups[1].Value))
					s = s.Remove(m.Index, m.Length).Insert(m.Index, _macros[m.Groups[1].Value]);
			}

			return Environment.ExpandEnvironmentVariables(s);
		}
	}
}
