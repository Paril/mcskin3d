using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class DarkenLightenTool : BrushToolBase
	{
		public override bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			return MouseMoveOnSkin(pixels, skin, x, y, GlobalSettings.DarkenLightenIncremental);
		}

		public override Color BlendColor(Color l, Color r)
		{
			bool ctrlIng = (Control.ModifierKeys & Keys.Shift) != 0;
			bool switchTools = (!Program.MainForm.DarkenLightenOptions.Inverted && ctrlIng) || (Program.MainForm.DarkenLightenOptions.Inverted && !ctrlIng);
			var hsl = Devcorp.Controls.Design.ColorSpaceHelper.RGBtoHSL(r);
			var mod = l.A / 255.0f;

			if (switchTools)
				hsl.Luminance -= (GlobalSettings.DarkenLightenExposure * mod) / 5.0f;
			else
				hsl.Luminance += (GlobalSettings.DarkenLightenExposure * mod) / 5.0f;

			if (hsl.Luminance < 0)
				hsl.Luminance = 0;
			if (hsl.Luminance > 1)
				hsl.Luminance = 1;

			return Color.FromArgb(r.A, Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(hsl));
		}

		public override Color GetLeftColor()
		{
			return Color.FromArgb(255, 0, 0, 0);
		}

		public override string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_DARKENLIGHTEN");
		}
	}
}
