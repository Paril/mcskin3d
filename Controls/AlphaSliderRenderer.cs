using System.Drawing;
using System.Drawing.Drawing2D;
using MB.Controls;

namespace MCSkin3D
{
	public class AlphaSliderRenderer : SliderRenderer
	{
		public AlphaSliderRenderer(ColorSlider slider) :
			base(slider)
		{
		}

		public Color EndColor { get; set; }

		public override void Render(Graphics g)
		{
			var colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);
			System.Drawing.Brush brush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.Gray, Color.LightGray);

			g.FillRectangle(brush, colorRect);

			brush = new LinearGradientBrush(colorRect, Color.FromArgb(0, EndColor), EndColor, LinearGradientMode.Horizontal);
			g.FillRectangle(brush, colorRect);
			g.DrawRectangle(Pens.Black, colorRect);

			DrawThumb(g);
		}
	}
}