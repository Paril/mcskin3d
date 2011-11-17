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
using MB.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace MCSkin3D
{
	public class SaturationSliderRenderer : SliderRenderer
	{
		public SaturationSliderRenderer(ColorSlider slider) :
			base(slider)
		{
		}

		public double Hue { get; set; }
		public double Luminance { get; set; }

		public override void Render(Graphics g)
		{
			var colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);
			Color c1 = Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(Hue, 0.0f, Luminance / 240.0f));
			Color c2 = Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(Hue, 1, Luminance / 240.0f));
			LinearGradientBrush brush = new LinearGradientBrush(colorRect, c1, c2, LinearGradientMode.Horizontal);
			//Draw color
			g.FillRectangle(brush, colorRect);
			//Draw border
			g.DrawRectangle(Pens.Black, colorRect);
			TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, System.Windows.Forms.VisualStyles.TrackBarThumbState.Normal);
		}
	}
}