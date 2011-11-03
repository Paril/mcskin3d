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

		public int Hue { get; set; }
		public int Luminance { get; set; }

		public override void Render(Graphics g)
		{
			var colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);

			float lum = Luminance / 240.0f;
			float satIncrease = 240.0f / (float)colorRect.Width;

            LinearGradientBrush gradBrush = new LinearGradientBrush(new Rectangle(0, 0, colorRect.Width, colorRect.Height), 

			for (int y = colorRect.Y; y < colorRect.Y + colorRect.Height; ++y)
				for (int x = colorRect.X; x < colorRect.X + colorRect.Width; ++x)
					g.FillRectangle(new SolidBrush(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(new Devcorp.Controls.Design.HSL(Hue, (x * satIncrease) / 240.0f, lum)).ToColor()), x, y, 1, 1);

			g.DrawRectangle(Pens.Black, colorRect);

			TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, System.Windows.Forms.VisualStyles.TrackBarThumbState.Normal);
		}
	}
}
