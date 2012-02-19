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
using System.Windows.Forms.VisualStyles;
using System.Drawing.Drawing2D;

namespace MCSkin3D
{
	public class ColorSliderRenderer : SliderRenderer
	{
		public ColorSliderRenderer(ColorSlider slider) :
			base(slider)
		{
		}

		public Color StartColor
		{
			get;
			set;
		}

		public Color EndColor
		{
			get;
			set;
		}

		public override void Render(Graphics g)
		{
			//TrackBarRenderer.DrawHorizontalTrack(g, new Rectangle(0, (Slider.Height / 2) - 2, Slider.Width, 4));
			var colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);
			var brush = new System.Drawing.Drawing2D.LinearGradientBrush(colorRect, StartColor, EndColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
			g.FillRectangle(brush, colorRect);
			g.DrawRectangle(Pens.Black, colorRect);

			DrawThumb(g);
		}
	}
}
