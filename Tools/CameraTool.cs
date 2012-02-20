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
using Paril.OpenGL;

namespace MCSkin3D
{
	public class CameraTool : ITool
	{
		public void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_oldMouse = e.Location;
			_clickedScreen = Screen.FromPoint(Cursor.Position);
		}

		public void SelectedBrushChanged() { }

		Point _oldMouse;
		Screen _clickedScreen;

		public void MouseMove(Skin skin, MouseEventArgs e)
		{
			var delta = new Point(e.X - _oldMouse.X, e.Y - _oldMouse.Y);
			var position = Cursor.Position;

			if (GlobalSettings.InfiniteMouse)
			{
				Rectangle screenBounds = _clickedScreen.Bounds;
				bool wasWrapped = false;
				var oldMouseOnScreen = Editor.MainForm.Renderer.PointToScreen(_oldMouse);

				if (position.X <= screenBounds.X && oldMouseOnScreen.X > screenBounds.X)
				{
					Cursor.Position = new Point(screenBounds.X + screenBounds.Width, position.Y);
					wasWrapped = true;
				}
				else if (position.X >= screenBounds.X + screenBounds.Width - 1 && oldMouseOnScreen.X < screenBounds.X + screenBounds.Width - 1)
				{
					Cursor.Position = new Point(screenBounds.X, position.Y);
					wasWrapped = true;
				}

				if (position.Y <= screenBounds.Y && oldMouseOnScreen.Y > screenBounds.Y)
				{
					Cursor.Position = new Point(position.X, screenBounds.Y + screenBounds.Height);
					wasWrapped = true;
				}
				else if (position.Y >= screenBounds.Y + screenBounds.Height - 1 && oldMouseOnScreen.Y < screenBounds.Y + screenBounds.Height - 1)
				{
					Cursor.Position = new Point(position.X, screenBounds.Y);
					wasWrapped = true;
				}

				if (wasWrapped)
					_oldMouse = Editor.MainForm.Renderer.PointToClient(Cursor.Position);
				else
					_oldMouse = e.Location;
			}
			else
				_oldMouse = e.Location;

			if (e.Button == Editor.MainForm.CameraRotate)
				Editor.MainForm.RotateView(delta, 1);
			else if (e.Button == Editor.MainForm.CameraZoom)
				Editor.MainForm.ScaleView(delta, 1);

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