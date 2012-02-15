using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MB.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Drawing2D;

namespace MCSkin3D
{
	public class AlphaSliderRenderer : SliderRenderer
	{
		public AlphaSliderRenderer(ColorSlider slider) :
			base(slider)
		{
		}

		public Color EndColor
		{
			get;
			set;
		}

		public override void Render(Graphics g)
		{
			var colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);
			System.Drawing.Brush brush = new HatchBrush(HatchStyle.LargeCheckerBoard, System.Drawing.Color.Gray, System.Drawing.Color.LightGray);

			g.FillRectangle(brush, colorRect);

			brush = new LinearGradientBrush(colorRect, Color.FromArgb(0, EndColor), EndColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
			g.FillRectangle(brush, colorRect);
			g.DrawRectangle(Pens.Black, colorRect);

			TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, (this.Slider.MouseInThumbRegion) ? (Form.MouseButtons & MouseButtons.Left) != 0 ? TrackBarThumbState.Pressed : TrackBarThumbState.Hot : TrackBarThumbState.Normal);
		}
	}
}
