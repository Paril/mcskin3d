using Devcorp.Controls.Design;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace MCSkin3D.Swatches
{
	public class ACOSwatch : MCSwatch
	{
		public ACOSwatch(string fileName) :
			base(fileName)
		{
		}

		private static string ReadString(EndianBinaryReader reader)
		{
			reader.ReadUInt16();
			ushort len = reader.ReadUInt16();
			char[] chars = new char[len];

			for (ushort i = 0; i < len - 1; ++i)
				chars[i] = (char)reader.ReadUInt16();
	
			reader.ReadUInt16();

			return new string(chars);
		}

		private static void WriteString(EndianBinaryWriter writer, string s)
		{
			//writer.Write((ushort)0);
			if (s == null)
				s = "";

			writer.Write((ushort) s.Length + 1);

			for (ushort i = 0; i < s.Length; ++i)
				writer.Write((ushort) s[i]);
			writer.Write((ushort) 0);
		}

		private static Color ReadRGBColor(EndianBinaryReader reader)
		{
			ushort r = reader.ReadUInt16();
			ushort g = reader.ReadUInt16();
			ushort b = reader.ReadUInt16();
			reader.ReadUInt16();

			return Color.FromArgb(255,
			                      (byte) ((r / (float) ushort.MaxValue) * 255),
			                      (byte) ((g / (float) ushort.MaxValue) * 255),
			                      (byte) ((b / (float) ushort.MaxValue) * 255));
		}

		private static Color ReadHSBColor(EndianBinaryReader reader)
		{
			ushort x = reader.ReadUInt16();
			ushort y = reader.ReadUInt16();
			ushort z = reader.ReadUInt16();
			reader.ReadUInt16();

			float h = x / 182.04f;
			float s = y / 655.35f;
			float b = z / 655.35f;

			return ColorSpaceHelper.HSBtoColor(h, s, b);
		}

		private static Color ReadCMYKColor(EndianBinaryReader reader)
		{
			ushort x = reader.ReadUInt16();
			ushort y = reader.ReadUInt16();
			ushort z = reader.ReadUInt16();
			ushort w = reader.ReadUInt16();

			float c = (100 - (x / 655.35f)) / 100.0f;
			float m = (100 - (y / 655.35f)) / 100.0f;
			float ye = (100 - (z / 655.35f)) / 100.0f;
			float k = (100 - (w / 655.35f)) / 100.0f;

			return ColorSpaceHelper.CMYKtoColor(c, m, ye, k);
		}

		private static Color ReadLABColor(EndianBinaryReader reader)
		{
			ushort x = reader.ReadUInt16();
			short y = reader.ReadInt16();
			short z = reader.ReadInt16();
			ushort w = reader.ReadUInt16();

			float La = x / 100.0f;
			float Aa = y / 100.0f;
			float Bb = z / 100.0f;

			return ColorSpaceHelper.LabtoRGB(La, Aa, Bb).ToColor();
		}

		private static Color ReadGrayscaleColor(EndianBinaryReader reader)
		{
			ushort x = reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();

			return Color.FromArgb(255,
			                      (byte) ((x / 10000.0f) * 255),
			                      (byte) ((x / 10000.0f) * 255),
			                      (byte) ((x / 10000.0f) * 255));
		}

		private void LoadACOData(EndianBinaryReader reader, int version)
		{
			ushort numColors = reader.ReadUInt16();

			while (numColors-- > 0)
			{
				ushort space = reader.ReadUInt16();

				Color color;

				switch (space)
				{
					case 0:
						color = ReadRGBColor(reader);
						break;
					case 1:
						color = ReadHSBColor(reader);
						break;
					case 2:
						color = ReadCMYKColor(reader);
						break;
					case 7:
						color = ReadLABColor(reader);
						break;
					case 8:
						color = ReadGrayscaleColor(reader);
						break;
					default:
						reader.ReadBytes(2 * 4);
						Console.WriteLine("Ignoring space " + space);
						continue;
				}

				string name = "";

				if (version == 2)
					name = ReadString(reader);

				Add(new NamedColor(name, color));
			}
		}

		public override void Load()
		{
			using (
				var reader = new EndianBinaryReader(EndianBitConverter.Big, File.Open(FilePath, FileMode.Open, FileAccess.Read),
				                                    Encoding.Unicode))
			{
				ushort version = reader.ReadUInt16();
				LoadACOData(reader, version);

				if (reader.BaseStream.Position == reader.BaseStream.Length)
					return;

				// Version 2 is here, so clear what we have
				Clear();

				version = reader.ReadUInt16();
				LoadACOData(reader, version);
			}
		}

		private void WriteRGBColor(EndianBinaryWriter writer, NamedColor c)
		{
			writer.Write((ushort) ((c.Color.R / 255.0f) * ushort.MaxValue));
			writer.Write((ushort) ((c.Color.G / 255.0f) * ushort.MaxValue));
			writer.Write((ushort) ((c.Color.B / 255.0f) * ushort.MaxValue));
			writer.Write((ushort) 0);
		}

		private void SaveACOData(EndianBinaryWriter writer)
		{
			writer.Write((ushort) Count);

			foreach (NamedColor c in this)
			{
				writer.Write((ushort) 0);

				WriteRGBColor(writer, c);
				WriteString(writer, c.Name);
			}
		}

		public override void Save()
		{
			using (
				var writer = new EndianBinaryWriter(EndianBitConverter.Big, File.Open(FilePath, FileMode.Create, FileAccess.Write),
				                                    Encoding.Unicode))
			{
				writer.Write((ushort) 2);
				SaveACOData(writer);
			}
		}
	}
}