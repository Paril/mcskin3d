using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using MiscUtil.IO;
using MiscUtil.Conversion;

namespace MCSkin3D.Swatches
{
	public class ACTSwatch : MCSwatch
	{
		public ACTSwatch(string fileName) :
			base(fileName)
		{
		}

		public override void Load()
		{
			using (EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(FilePath, FileMode.Open, FileAccess.Read)))
			{
				var numColors = reader.BaseStream.Length / 3;

				while (numColors-- != 0)
				{
					byte r = reader.ReadByte();
					byte g = reader.ReadByte();
					byte b = reader.ReadByte();

					Add(new NamedColor("Unnamed", Color.FromArgb(255, r, g, b)));
				}
			}
		}

		public override void Save()
		{
			using (EndianBinaryWriter writer = new EndianBinaryWriter(EndianBitConverter.Little, File.Open(FilePath, FileMode.Open, FileAccess.Write)))
			{
				foreach (var c in this)
				{
					writer.Write((byte)c.Color.R);
					writer.Write((byte)c.Color.G);
					writer.Write((byte)c.Color.B);
				}
			}
		}
	}
}
