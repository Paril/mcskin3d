using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;

namespace Paril.Settings.Serializers
{
	// I honestly thought that I wouldn't need to do this, but since structs
	// can't be readonly..
	public class ColorSerializer : ITypeSerializer
	{
		public string Serialize(object obj)
		{
			Color c = (Color)obj;

			return c.R.ToString() + " " + c.G.ToString() + " " + c.B.ToString() + " " + c.A.ToString();
		}
		
		public object Deserialize(string str)
		{
			var split = str.Split();

			return Color.FromArgb(byte.Parse(split[3]), byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]));
		}
	}
}
