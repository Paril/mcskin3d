using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class DodgeBurnTool : BrushToolBase
	{
		public override bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			return MouseMoveOnSkin(pixels, skin, x, y, GlobalSettings.DodgeBurnIncremental);
		}

		public override Color BlendColor(Color l, Color r)
		{
			bool ctrlIng = (Control.ModifierKeys & Keys.Shift) != 0;
			bool switchTools = (!Program.MainForm.DodgeBurnOptions.Inverted && ctrlIng) || (Program.MainForm.DodgeBurnOptions.Inverted && !ctrlIng);
			var mod = l.A / 255.0f;

			if (switchTools)
				return Color.FromArgb(ColorBlending.Burn(r, 1 - ((GlobalSettings.DodgeBurnExposure * mod) / 10.0f)).ToArgb());
			else
				return Color.FromArgb(ColorBlending.Dodge(r, (GlobalSettings.DodgeBurnExposure * mod) / 10.0f).ToArgb());
		}

		public override Color GetLeftColor()
		{
			return Color.FromArgb(255, 0, 0, 0);
		}
	}
}
