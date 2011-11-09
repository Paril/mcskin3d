using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class PencilTool : BrushToolBase
	{
		public override bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			return MouseMoveOnSkin(pixels, skin, x, y, GlobalSettings.PencilIncremental);
		}

		public override Color BlendColor(Color l, Color r)
		{
			return (Color)ColorBlending.AlphaBlend(l, r);
		}

		public override Color GetLeftColor()
		{
			return ((Control.ModifierKeys & Keys.Shift) != 0) ? Program.MainForm.UnselectedColor : Program.MainForm.SelectedColor;
		}
	}
}
