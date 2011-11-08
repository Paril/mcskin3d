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