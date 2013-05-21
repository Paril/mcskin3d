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

using Paril.OpenGL;
using System.Drawing;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class PencilTool : BrushToolBase
	{
		public override bool MouseMoveOnSkin(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			return MouseMoveOnSkin(ref pixels, skin, x, y, GlobalSettings.PencilIncremental);
		}

		public override Color BlendColor(Color l, Color r)
		{
			return (Color) ColorBlending.AlphaBlend(l, r);
		}

		public override Color GetLeftColor()
		{
			return
				(((Control.ModifierKeys & Keys.Shift) != 0)
				 	? Editor.MainForm.ColorPanel.UnselectedColor
				 	: Editor.MainForm.ColorPanel.SelectedColor).RGB;
		}

		public override string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_PENCIL");
		}
	}
}