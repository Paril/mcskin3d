using System;
using MB.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class HueSliderRenderer : SliderRenderer
	{
		public HueSliderRenderer(ColorSlider slider) :
			base(slider)
		{
		}

		public int Saturation { get; set; }
		public int Luminance { get; set; }

		public override void Render(Graphics g)
		{
			var colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);

			float sat = Saturation / 240.0f;
			float lum = Luminance / 240.0f;
			float hueIncrease = 360.0f / (float)colorRect.Width;

			for (int y = colorRect.Y; y < colorRect.Y + colorRect.Height; ++y)
				for (int x = colorRect.X; x < colorRect.X + colorRect.Width; ++x)
					g.FillRectangle(new SolidBrush(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(new Devcorp.Controls.Design.HSL(x * hueIncrease, sat, lum)).ToColor()), x, y, 1, 1);

			g.DrawRectangle(Pens.Black, colorRect);

			TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, System.Windows.Forms.VisualStyles.TrackBarThumbState.Normal);
		}
	}
}
