using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;
using MiscUtil.Conversion;
using Paril.Cryptography;

namespace Paril.Imaging
{
	public class PNGMetadata
	{
		public static Dictionary<string, string> ReadMetadata(string fileName)
		{
			Dictionary<string, string> metadata = new Dictionary<string, string>();

			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (var br = new EndianBinaryReader(EndianBitConverter.Big, fs))
			{
				br.ReadBytes(8);

				while (true)
				{
					uint len = br.ReadUInt32();
					string type = Encoding.ASCII.GetString(br.ReadBytes(4));

					if (type == "tEXt")
					{
						string keyword = "";
						uint count = 0;
						char c = '\0';

						while (true)
						{
							c = (char)br.ReadByte();

							count++;

							if (c == '\0')
								break;

							keyword += c;
						};

						string text = Encoding.ASCII.GetString(br.ReadBytes((int)(len - count)));
						metadata.Add(keyword, text);
					}
					else if (type == "IEND")
						break;
					else
						br.ReadBytes((int)len);

					br.ReadInt32();
				}
			}

			return metadata;
		}

		public static void WriteMetadata(string fileName, Dictionary<string, string> data)
		{
			long iendPos = 0;

			// FIXME: I hate this.
			byte[] dataBefore, dataAfter;

			// find where the IEND chunk starts
			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (var br = new EndianBinaryReader(EndianBitConverter.Big, fs))
			{
				br.ReadBytes(8);

				while (true)
				{
					iendPos = br.BaseStream.Position;
					uint len = br.ReadUInt32();
					string type = Encoding.ASCII.GetString(br.ReadBytes(4));

					if (type == "IEND")
						break;
					else
						br.ReadBytes((int)len);

					br.ReadInt32();
				}

				br.BaseStream.Position = 0;

				// FIXME: BAH REMOVE
				dataBefore = br.ReadBytes((int)iendPos);
				dataAfter = br.ReadBytes((int)(fs.Length - iendPos));
			}

			using (var fs = new FileStream(fileName, FileMode.Truncate, FileAccess.Write))
			using (var bw = new EndianBinaryWriter(EndianBitConverter.Big, fs))
			{
				bw.Write(dataBefore);

				foreach (var text in data)
				{
					int chunkLen = text.Key.Length + 1 + text.Value.Length;

					bw.Write((uint)chunkLen);
					bw.Write(Encoding.ASCII.GetBytes("tEXt"), 0, 4);

					List<byte> _toCrc = new List<byte>();
					_toCrc.AddRange(Encoding.ASCII.GetBytes(text.Key));
					_toCrc.Add(0);
					_toCrc.AddRange(Encoding.ASCII.GetBytes(text.Value));

					bw.Write(Encoding.ASCII.GetBytes(text.Key));
					bw.Write((byte)0);
					bw.Write(Encoding.ASCII.GetBytes(text.Value));

					Crc32 crc = new Crc32();
					crc.Initialize();
					crc.ComputeHash(_toCrc.ToArray());

					bw.Write(crc.CrcValue);
				}

				bw.Write(dataAfter);
			}
		}
	}
}
