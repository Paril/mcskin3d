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

using MCSkin3D.lemon42;
using Paril.OpenGL;
using System.Drawing;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class DropperTool : ITool
	{
		#region ITool Members

		public void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
		}

		public void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		public void SelectedBrushChanged()
		{
		}

		public bool MouseMoveOnSkin(ColorGrabber pixels, Skin skin, int x, int y)
		{
			ColorPixel c = pixels[x, y];
			ColorManager oldColor = ColorManager.FromRGBA(c.Red, c.Green, c.Blue, c.Alpha);

			if ((Control.ModifierKeys & Keys.Shift) != 0)
				Editor.MainForm.ColorPanel.UnselectedColor = oldColor;
			else
				Editor.MainForm.ColorPanel.SelectedColor = oldColor;
			return false;
		}

		public bool RequestPreview(ColorGrabber pixels, Skin skin, int x, int y)
		{
			return false;
		}

		public bool EndClick(ColorGrabber pixels, Skin skin, MouseEventArgs e)
		{
			return false;
		}

		public string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_DROPPER");
		}

		#endregion
	}
}