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
using Devcorp.Controls.Design;

namespace MCSkin3D
{
	public class NoiseTool : BrushToolBase
	{
		Random _noise, _noise2;
		int _seed;
		public NoiseTool()
		{
			_noise = new Random();
			_seed = _noise.Next();
			_noise = _noise2 = new Random(_seed);
		}

		public override Color BlendColor(Color l, Color r)
		{
			Color c = r;
			var hsv = ColorSpaceHelper.RGBtoHSB(c);
			hsv.Brightness += (((IsPreview ? _noise : _noise2).NextDouble() - 0.5f) * 2) * GlobalSettings.NoiseSaturation;

			if (hsv.Brightness < 0)
				hsv.Brightness = 0;
			if (hsv.Brightness > 1)
				hsv.Brightness = 1;

			return Color.FromArgb(r.A, ColorSpaceHelper.HSBtoColor(hsv));
		}

		public override void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_noise2 = new Random(_seed);
			base.BeginClick(skin, p, e);
		}

		public override Color GetLeftColor()
		{
			return Color.White;
		}

		public override bool MouseMoveOnSkin(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			return MouseMoveOnSkin(ref pixels, skin, x, y, false);
		}

		public override string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_NOISE");
		}

		public override bool RequestPreview(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			_noise = new Random(_seed);
			return base.RequestPreview(ref pixels, skin, x, y);
		}

		public override bool EndClick(ref ColorGrabber pixels, Skin skin, MouseEventArgs e)
		{
			base.EndClick(ref pixels, skin, e);
			_seed = _noise.Next();
			_noise = _noise2 = new Random(_seed);

			return false;
		}
	}
}