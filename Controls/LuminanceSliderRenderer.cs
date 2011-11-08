using System;
using MB.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

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
            //theCode, love theVariableNames :D [Xylem]
            //Set the hue shades with the correct saturation and hue
            Color[] theColors = {Color.Black,
                           Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(Hue, Saturation/240.0f, 0.5f)),
                           Color.White};
            //Calculate positions
            float[] thePositions = { 0.0f, 0.5f, 1.0f };
            //Set blend
            ColorBlend theBlend = new ColorBlend();
            theBlend.Colors = theColors;
            theBlend.Positions = thePositions;
            //Get rectangle
            Rectangle colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);
            //Make the linear brush and assign the custom blend to it
            LinearGradientBrush theBrush = new LinearGradientBrush(colorRect,
                                                              Color.Black,
                                                              Color.White, 0, false);
            theBrush.InterpolationColors = theBlend;
            //Draw rectangle
            g.FillRectangle(theBrush, colorRect);
            //Draw border and trackbar
            g.DrawRectangle(Pens.Black, new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4));
            TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, System.Windows.Forms.VisualStyles.TrackBarThumbState.Normal);
        }
	}
}
