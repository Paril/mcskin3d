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
	public class ACOSwatch : MCSwatch
	{
		public ACOSwatch(string fileName) :
			base(fileName)
		{
		}

		static string ReadString(EndianBinaryReader reader)
		{
			string str = "";

			reader.ReadUInt16();
			ushort len = reader.ReadUInt16();

			for (ushort i = 0; i < len - 1; ++i)
				str += (char)reader.ReadUInt16();
			reader.ReadUInt16();

			return str;
		}

		static Color ReadRGBColor(EndianBinaryReader reader)
		{
			ushort r = reader.ReadUInt16();
			ushort g = reader.ReadUInt16();
			ushort b = reader.ReadUInt16();
			reader.ReadUInt16();

			return Color.FromArgb(255,
							(byte)(((float)r / (float)ushort.MaxValue) * 255),
							(byte)(((float)g / (float)ushort.MaxValue) * 255),
							(byte)(((float)b / (float)ushort.MaxValue) * 255));
		}

		static Color ReadHSBColor(EndianBinaryReader reader)
		{
			ushort x = reader.ReadUInt16();
			ushort y = reader.ReadUInt16();
			ushort z = reader.ReadUInt16();
			reader.ReadUInt16();

			float h = x / 182.04f;
			float s = y / 655.35f;
			float b = z / 655.35f;

			return Devcorp.Controls.Design.ColorSpaceHelper.HSBtoColor(h, s, b);
		}

		static Color ReadCMYKColor(EndianBinaryReader reader)
		{
			ushort x = reader.ReadUInt16();
			ushort y = reader.ReadUInt16();
			ushort z = reader.ReadUInt16();
			ushort w = reader.ReadUInt16();

			float c = (100 - (x / 655.35f)) / 100.0f;
			float m = (100 - (y / 655.35f)) / 100.0f;
			float ye = (100 - (z / 655.35f)) / 100.0f;
			float k = (100 - (w / 655.35f)) / 100.0f;

			return Devcorp.Controls.Design.ColorSpaceHelper.CMYKtoColor(c, m, ye, k);
		}

		static Color ReadLABColor(EndianBinaryReader reader)
		{
			ushort x = reader.ReadUInt16();
			short y = reader.ReadInt16();
			short z = reader.ReadInt16();
			ushort w = reader.ReadUInt16();

			float La = x / 100.0f;
			float Aa = y / 100.0f;
			float Bb = z / 100.0f;

			return Devcorp.Controls.Design.ColorSpaceHelper.LabtoRGB(La, Aa, Bb).ToColor();
		}

		static Color ReadGrayscaleColor(EndianBinaryReader reader)
		{
			ushort x = reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();

			return Color.FromArgb(255,
				(byte)(((float)x / 10000.0f) * 255),
				(byte)(((float)x / 10000.0f) * 255),
				(byte)(((float)x / 10000.0f) * 255));
		}

		void LoadACOData(EndianBinaryReader reader, int version)
		{
			var numColors = reader.ReadUInt16();

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
			using (EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Big, File.Open(FilePath, FileMode.Open, FileAccess.Read), Encoding.Unicode))
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

		static void WriteString(EndianBinaryWriter writer, string s)
		{
			writer.Write((ushort)0);
			writer.Write((ushort)s.Length);

			for (ushort i = 0; i < s.Length; ++i)
				writer.Write((ushort)s[i]);
			writer.Write((ushort)0);
		}

		private void WriteRGBColor(EndianBinaryWriter writer, NamedColor c)
		{
			writer.Write((ushort)(((float)c.Color.R / 255.0f) * (float)ushort.MaxValue));
			writer.Write((ushort)(((float)c.Color.G / 255.0f) * (float)ushort.MaxValue));
			writer.Write((ushort)(((float)c.Color.B / 255.0f) * (float)ushort.MaxValue));
			writer.Write((ushort)0);
		}

		void SaveACOData(EndianBinaryWriter writer)
		{
			writer.Write((ushort)Count);

			foreach (var c in this)
			{
				writer.Write((ushort)0);

				WriteRGBColor(writer, c);
				WriteString(writer, c.Name);
			}
		}

		public override void Save()
		{
			using (EndianBinaryWriter writer = new EndianBinaryWriter(EndianBitConverter.Big, File.Open(FilePath, FileMode.Create, FileAccess.Write), Encoding.Unicode))
			{
				writer.Write((ushort)2);
				SaveACOData(writer);
			}
		}
	}
}
