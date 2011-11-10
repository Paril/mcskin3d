using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class EraserTool : BrushToolBase
	{
		public override Color BlendColor(Color l, Color r)
		{
			return Color.FromArgb(0, 0, 0, 0);
		}

		public override bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			return MouseMoveOnSkin(pixels, skin, x, y, false);
		}

		public override Color GetLeftColor()
		{
			return Color.FromArgb(0, 0, 0, 0);
		}

		public override string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_ERASER");
		}
	}
}
