using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Paril.Imaging
{
	public static class Extensions
	{
		public static void SaveSafe(this Image img, string fileName, ImageFormat fmt)
		{
			using (var memory = new MemoryStream())
			{
				img.Save(memory, fmt);

				var bytes = memory.ToArray();

				using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
					file.Write(bytes, 0, bytes.Length);
			}
		}

		public static void SaveSafe(this Image img, string fileName)
		{
			SaveSafe(img, fileName, ImageFormat.Png);
		}
	}
}
