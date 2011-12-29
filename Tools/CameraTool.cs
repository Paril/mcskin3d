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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class CameraTool : ITool
	{
		public void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_oldMouse = e.Location;
		}

		Point _oldMouse;
		public void MouseMove(Skin skin, MouseEventArgs e)
		{
			var delta = new Point(e.X - _oldMouse.X, e.Y - _oldMouse.Y);

			if (e.Button == Program.MainForm.CameraRotate)
				Program.MainForm.RotateView(delta, 1);
			else if (e.Button == Program.MainForm.CameraZoom)
				Program.MainForm.ScaleView(delta, 1);

			_oldMouse = e.Location;
		}

		public bool MouseMoveOnSkin(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			return false;
		}

		public bool RequestPreview(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			return false;
		}

		public bool EndClick(ref ColorGrabber pixels, Skin skin, MouseEventArgs e)
		{
			return false;
		}

		public string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_CAMERA");
		}
	}
}