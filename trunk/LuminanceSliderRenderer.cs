using System;
using MB.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class LuminanceSliderRenderer : SliderRenderer
	{
		public LuminanceSliderRenderer(ColorSlider slider) :
			base(slider)
		{
		}

		public int Hue { get; set; }
		public int Saturation { get; set; }

		public override void Render(Graphics g)
		{
			var colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);

			float sat = Saturation / 240.0f;
			float lumIncrease = 240.0f / (float)colorRect.Width;

			for (int y = colorRect.Y; y < colorRect.Y + colorRect.Height; ++y)
				for (int x = colorRect.X; x < colorRect.X + colorRect.Width; ++x)
					g.FillRectangle(new SolidBrush(Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(new Devcorp.Controls.Design.HSL(Hue, sat, (x * lumIncrease) / 240.0f)).ToColor()), x, y, 1, 1);

			g.DrawRectangle(Pens.Black, colorRect);

			TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, System.Windows.Forms.VisualStyles.TrackBarThumbState.Normal);
		}
	}
}
