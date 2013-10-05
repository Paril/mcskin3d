using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MCSkin3D.Swatches
{
	/// <summary>
	/// Simple swatch format made for MCSkin3D.
	/// Somewhat deprecated, but leaving it here for old support.
	/// </summary>
	public class MCSwatch : SwatchBase
	{
		// The list
		private readonly List<NamedColor> _colors = new List<NamedColor>();
		private string _name;

		public MCSwatch(string fileName)
		{
			FilePath = fileName;
			Name = Path.GetFileNameWithoutExtension(fileName);
		}

		public override int Count
		{
			get { return _colors.Count; }
		}

		public override NamedColor this[int index]
		{
			get { return _colors[index]; }
			set { _colors[index] = value; Dirty = true; }
		}

		public override string Name
		{
			get { return _name; }
			set
			{
				if (!string.IsNullOrEmpty(_name))
				{
					if (value == null)
						File.Delete(FilePath);
					else
					{
						string oldFile = FilePath;
						string newFile = Path.GetDirectoryName(FilePath) + '\\' + value + Path.GetExtension(FilePath);

						File.Move(oldFile, newFile);

						FilePath = newFile;
					}
				}

				_name = value;
			}
		}

		public override string FilePath { get; set; }

		public override void Add(NamedColor color)
		{
			Dirty = true;
			_colors.Add(color);
		}

		public override void Clear()
		{
			Dirty = true;
			_colors.Clear();
		}

		public override void CopyTo(NamedColor[] array, int start)
		{
			_colors.CopyTo(array, start);
		}

		public override IEnumerator<NamedColor> GetEnumerator()
		{
			return _colors.GetEnumerator();
		}

		public override void Insert(int index, NamedColor color)
		{
			Dirty = true;
			_colors.Insert(index, color);
		}

		public override void RemoveAt(int index)
		{
			Dirty = true;
			_colors.RemoveAt(index);
		}

		public override void Load()
		{
			using (var sr = new StreamReader(FilePath))
			{
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();

					if (string.IsNullOrEmpty(line))
						continue;

					string[] split = line.Split();

					if (split.Length < 4)
						continue;

					Color c = Color.FromArgb(byte.Parse(split[3]), byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]));

					if (split.Length == 4)
						_colors.Add(new NamedColor(null, c));
					else if (split.Length >= 5)
					{
						int startText = 0;

						for (; startText < line.Length; ++startText)
						{
							if (char.IsLetter(line[startText]))
								break;
						}

						_colors.Add(new NamedColor(line.Substring(startText), c));
					}
					else
						throw new Exception("Couldn't load swatch: misformed line");
				}
			}

			Dirty = false;
		}

		public override void Save()
		{
			using (var sw = new StreamWriter(FilePath))
			{
				foreach (NamedColor c in _colors)
				{
					sw.Write(c.Color.R.ToString() + " " + c.Color.G.ToString() + " " + c.Color.B.ToString() + " " +
					         c.Color.A.ToString());

					if (c.Name != null)
						sw.Write(" " + c.Name);

					sw.WriteLine();
				}
			}
		}
	}
}