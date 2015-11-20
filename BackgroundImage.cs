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

using System.Windows.Forms;
using Paril.OpenGL;

namespace MCSkin3D
{
	internal class BackgroundImage
	{
		public Texture GLImage;
		public ToolStripMenuItem Item;
		public string Name, Path;

		public BackgroundImage(string path, string name, Texture image)
		{
			Path = path;
			Name = name;
			GLImage = image;
		}
	}
}