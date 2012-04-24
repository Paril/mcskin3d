using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace MCSkin3D.Swatches
{
	/// <summary>
	/// Simple swatch format made for MCSkin3D.
	/// Somewhat deprecated, but leaving it here for old support.
	/// </summary>
	public class MCSwatch : SwatchBase
	{
		// The list
		List<NamedColor> _colors = new List<NamedColor>();

		public override void Add(NamedColor color)
		{
			_colors.Add(color);
		}

		public override void Clear()
		{
			_colors.Clear();
		}

		public override void CopyTo(NamedColor[] array, int start)
		{
			_colors.CopyTo(array, start);
		}

		public override int Count
		{
			get { return _colors.Count; }
		}

		public override IEnumerator<NamedColor> GetEnumerator()
		{
			return _colors.GetEnumerator();
		}

		public override void Insert(int index, NamedColor color)
		{
			_colors.Insert(index, color);
		}

		public override void RemoveAt(int index)
		{
			_colors.RemoveAt(index);
		}

		public override NamedColor this[int index]
		{
			get { return _colors[index]; }
			set { _colors[index] = value; }
		}

		public MCSwatch(string fileName)
		{
			FilePath = fileName;
			Name = Path.GetFileNameWithoutExtension(fileName);
		}

		string _name;
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
						var oldFile = FilePath;
						var newFile = Path.GetDirectoryName(FilePath) + '\\' + value + Path.GetExtension(FilePath);

						File.Move(oldFile, newFile);

						FilePath = newFile;
					}
				}
					
				_name = value;
			}
		}

		public override string FilePath
		{
			get;
			set;
		}

		public override void Load()
		{
			using (StreamReader sr = new StreamReader(FilePath))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine();

					if (string.IsNullOrEmpty(line))
						continue;

					var split = line.Split();

					if (split.Length < 4)
						continue;

					Color c = Color.FromArgb(byte.Parse(split[3]), byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]));

					if (split.Length == 4)
						_colors.Add(new NamedColor(null, c));
					else if (split.Length >= 5)
					{
						int startText = 0;

						for (; startText < line.Length; ++startText)
							if (char.IsLetter(line[startText]))
								break;

						_colors.Add(new NamedColor(line.Substring(startText), c));
					}
					else
						throw new Exception("Couldn't load swatch: misformed line");
				}
			}
		}

		public override void Save()
		{
			using (StreamWriter sw = new StreamWriter(FilePath))
			{
				foreach (var c in _colors)
				{
					sw.Write(c.Color.R.ToString() + " " + c.Color.G.ToString() + " " + c.Color.B.ToString() + " " + c.Color.A.ToString());

					if (c.Name != null)
						sw.Write(" " + c.Name);

					sw.WriteLine();
				}
			}
		}
	}
}
