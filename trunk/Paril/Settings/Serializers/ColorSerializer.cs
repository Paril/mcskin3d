//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
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

using System.Drawing;

namespace Paril.Settings.Serializers
{
	// I honestly thought that I wouldn't need to do this, but since structs
	// can't be readonly..
	public class ColorSerializer : ITypeSerializer
	{
		#region ITypeSerializer Members

		public string Serialize(object obj)
		{
			var c = (Color) obj;

			return c.R.ToString() + " " + c.G.ToString() + " " + c.B.ToString() + " " + c.A.ToString();
		}

		public object Deserialize(string str)
		{
			string[] split = str.Split();

			return Color.FromArgb(byte.Parse(split[3]), byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]));
		}

		#endregion
	}
}