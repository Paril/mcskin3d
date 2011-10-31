using System;
using MB.Controls;
using System.Drawing;
using System.Windows.Forms;

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

			TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, System.Windows.Forms.VisualStyles.TrackBarThumbState.Normal);
		}
	}
}
