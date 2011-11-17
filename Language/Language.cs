//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace MCSkin3D.Language
{
	public class Language
	{
		public string Name { get; private set; }
		public string Version { get; private set; }
		public Version SupportedVersion { get; private set; }
		public Dictionary<string, string> StringTable { get; private set; }
		public ToolStripMenuItem Item { get; set; }
		public CultureInfo Culture { get; set; }

		public Language()
		{
			StringTable = new Dictionary<string, string>();
		}

		public static Language Parse(StreamReader sr)
		{
			Language lang = new Language();
			bool headerFound = false;

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
				var right = line.Substring(line.IndexOf('=') + 1).Trim(' ', '\t', '\"', '\'').Replace("\\r", "\r").Replace("\\n", "\n");
				lang.StringTable.Add(left, right);

				if (left[0] == '#')
				{
					if (left == "#Name")
						lang.Name = right;
					else if (left == "#Version")
						lang.Version = right;
					else if (left == "#SuppVersion")
						lang.SupportedVersion = new Version(right);
					else if (left == "#Culture")
						lang.Culture = CultureInfo.GetCultureInfo(right);
				}
			}

			return lang;
		}
	}
}
