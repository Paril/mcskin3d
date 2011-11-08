using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MCSkin3D.Language
{
	public class Language
	{
		public string Name { get; private set; }
		public string Version { get; private set; }
		public Version SupportedVersion { get; private set; }
		public Dictionary<string, string> StringTable { get; private set; }
		public ToolStripMenuItem Item { get; set; }

		public Language()
		{
			StringTable = new Dictionary<string, string>();
		}

		public static Language Parse(string file)
		{
			if (!File.Exists(file))
				throw new FileNotFoundException();

			Language lang = new Language();
			bool headerFound = false;

			using (StreamReader sr = new StreamReader(file))
			{
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();

					if (line.StartsWith("//") || string.IsNullOrEmpty(line))
						continue;

					if (line == "MCSkin3D Language File")
					{
						headerFound = true;
						continue;
					}

					if (!headerFound)
						throw new Exception("No header");

					if (!line.Contains('='))
						throw new Exception("Parse error");

					var left = line.Substring(0, line.IndexOf('=')).Trim();
					var right = line.Substring(line.IndexOf('=') + 1).Trim(' ', '\t', '\"', '\'');
					lang.StringTable.Add(left, right);

					if (left[0] == '#')
					{
						if (left == "#Name")
							lang.Name = right;
						else if (left == "#Version")
							lang.Version = right;
						else if (left == "#SuppVersion")
							lang.SupportedVersion = new Version(right);
					}
				}
			}

			return lang;
		}
	}
}
