using System;

namespace Paril.Settings.Serializers
{
	public static class HexSerializer
	{
		public static string GetHex(string s)
		{
			string h = "";

			foreach (var c in s)
				h += string.Format("{0:X2}", (int)c);

			return h;
		}

		public static string GetString(string hex)
		{
			string s = "";

			for (int i = 0; i < hex.Length; i += 2)
				s += (char)int.Parse("" + hex[i] + hex[i + 1], System.Globalization.NumberStyles.HexNumber);

			return s;
		}
	}
}
