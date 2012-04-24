//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Globalization;

namespace Paril.Settings.Serializers
{
	public static class HexSerializer
	{
		public static string GetHex(string s)
		{
			string h = "";

			foreach (char c in s)
				h += string.Format("{0:X2}", (int) c);

			return h;
		}

		public static string GetString(string hex)
		{
			string s = "";

			for (int i = 0; i < hex.Length; i += 2)
				s += (char) int.Parse("" + hex[i] + hex[i + 1], NumberStyles.HexNumber);

			return s;
		}
	}
}