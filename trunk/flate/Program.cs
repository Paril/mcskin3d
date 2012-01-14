using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace flate
{
	class Program
	{
		static void FlateFile(FileInfo file, bool decompress)
		{
			FileInfo compressedFile = new FileInfo(file.FullName.Substring(0, file.FullName.IndexOf('.')) + ".gz." + file.Extension.Substring(1));
			FileInfo uncompressedFile = new FileInfo(file.FullName.Substring(0, file.FullName.IndexOf('.')) + "." + file.Extension.Substring(1));

			if (file.FullName.Contains(".gz.") && decompress)
			{
				Console.Write("deflating " + compressedFile.ToString() + "...");
				using (FileStream input = new FileStream(compressedFile.FullName, FileMode.Open, FileAccess.Read))
				{
					using (FileStream output = new FileStream(uncompressedFile.FullName, FileMode.Create, FileAccess.Write))
					{
						using (GZipStream inputCompressed = new GZipStream(input, CompressionMode.Decompress))
						{
							int readCount = 0;
							byte[] buffer = new byte[4096];

							while ((readCount = inputCompressed.Read(buffer, 0, buffer.Length)) != 0)
								output.Write(buffer, 0, readCount);

							Console.Write(" " + input.Length.ToString() + " => " + output.Length.ToString() + "\r\n");
						}
					}
				}
			}
			else if (!decompress && !file.FullName.Contains(".gz."))
			{
				Console.Write("inflating " + uncompressedFile.ToString() + "...");
				using (FileStream input = new FileStream(uncompressedFile.FullName, FileMode.Open, FileAccess.Read))
				{
					using (FileStream output = new FileStream(compressedFile.FullName, FileMode.Create, FileAccess.Write))
					{
						using (GZipStream outputCompressed = new GZipStream(output, CompressionMode.Compress))
						{
							int readCount = 0;
							byte[] buffer = new byte[4096];

							while ((readCount = input.Read(buffer, 0, buffer.Length)) != 0)
								outputCompressed.Write(buffer, 0, readCount);

							Console.Write(" " + input.Length.ToString() + " => " + output.Length.ToString() + "\r\n");
						}
					}
				}
			}
		}

		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("flate - simple console app for deflating/inflating GZ files.");
				Console.WriteLine("Written by Paril for MCSkin3D. Licensed under GNU GPL v3.");
				Console.WriteLine("------------------------------------------------------------");
				Console.WriteLine("usage: flate.exe input {d or e}");
				return;
			}

			if (args[0].Contains("*"))
			{
				foreach (var file in new DirectoryInfo(Path.GetDirectoryName(args[0])).GetFiles(args[0].Replace(Path.GetDirectoryName(args[0]), "").Substring(1), SearchOption.AllDirectories))
					FlateFile(file, args[1][0] == 'd');
			}
			else
				FlateFile(new FileInfo(args[0]), args[1][0] == 'd');
		}
	}
}
