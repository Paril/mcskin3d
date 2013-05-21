using MiscUtil.Conversion;
using MiscUtil.IO;
using System.Drawing;
using System.IO;

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
			using (
				var reader = new EndianBinaryReader(EndianBitConverter.Little, File.Open(FilePath, FileMode.Open, FileAccess.Read)))
			{
				while ((reader.BaseStream.Length - reader.BaseStream.Position) >= 3)
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
			using (
				var writer = new EndianBinaryWriter(EndianBitConverter.Little,
				                                    File.Open(FilePath, FileMode.Create, FileAccess.Write)))
			{
				foreach (NamedColor c in this)
				{
					writer.Write(c.Color.R);
					writer.Write(c.Color.G);
					writer.Write(c.Color.B);
				}
			}
		}
	}
}