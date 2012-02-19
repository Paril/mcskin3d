using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace MCSkin3D.Swatches
{
	public class GIMPSwatch : MCSwatch
	{
		public GIMPSwatch(string fileName) :
			base(fileName)
		{
		}

		public override void Load()
		{
			using (StreamReader sr = new StreamReader(FilePath))
			{
				string line = sr.ReadLine();
				if (line != "GIMP Palette")
					throw new Exception();

				while (!sr.EndOfStream)
				{
					line = sr.ReadLine();

					if (line.StartsWith("#"))
						continue;
					if (string.IsNullOrEmpty(line))
						continue;

					var split = line.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

					int temp;

					if (int.TryParse(split[0], out temp))
					{
						Color c = Color.FromArgb(byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]));

						if (split.Length >= 4)
						{
							int startText = 0;

							for (; startText < line.Length; ++startText)
								if (char.IsLetter(line[startText]))
									break;

							Add(new NamedColor(line.Substring(startText), c));
						}
						else
							Add(new NamedColor(null, c));
					}
					else
					{
						if (split[0] == "Name:")
							Name = line.Substring(line.IndexOf(' ') + 1);
					}
				}
			}
		}

		public override void Save()
		{
			// TODO
			using (StreamWriter writer = new StreamWriter(FilePath))
			{
				writer.WriteLine("GIMP Palette");
				writer.WriteLine("Name: " + Name);
				writer.WriteLine("#");

				foreach (var c in this)
				{
					writer.Write(c.Color.R.ToString() + " " + c.Color.G.ToString() + " " + c.Color.B.ToString());

					if (c.Name != null)
						writer.Write("\t" + c.Name);

					writer.WriteLine();
				}
			}
		}
	}
}
