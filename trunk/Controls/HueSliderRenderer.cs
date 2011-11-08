using System;
using MB.Controls;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            //theCode, love theVariableNames :D [Xylem]
            //Set the hue shades with the correct saturation and luminance
            Color[] theColors = {Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(0, Saturation / 240.0f, Luminance / 240.0f)),
                           Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(60, Saturation / 240.0f, Luminance / 240.0f)),
                           Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(120, Saturation / 240.0f, Luminance / 240.0f)),
                           Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(180, Saturation / 240.0f, Luminance / 240.0f)),
                           Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(240, Saturation / 240.0f, Luminance / 240.0f)),
                           Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(300, Saturation / 240.0f, Luminance / 240.0f)),
                           Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(new Devcorp.Controls.Design.HSL(360, Saturation / 240.0f, Luminance / 240.0f))};
            //Calculate positions
            float percent = 1.0f / 6;
            float[] thePositions = { 0.0f, percent, percent*2, percent*3, percent*4, percent*5, 1.0f };
            //Set blend
            ColorBlend theBlend = new ColorBlend();
            theBlend.Colors = theColors;
            theBlend.Positions = thePositions;
            //Get rectangle
            Rectangle colorRect = new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4);
            //Make the linear brush and assign the custom blend to it
            LinearGradientBrush theBrush = new LinearGradientBrush(colorRect,
                                                              Color.Red,
                                                              Color.Red, 0, false);
            theBrush.InterpolationColors = theBlend;
            //Draw rectangle
            g.FillRectangle(theBrush, colorRect);
            //Draw border and trackbar
            g.DrawRectangle(Pens.Black, new Rectangle(0, (Slider.Height / 2) - 3, Slider.Width - 6, 4));
            TrackBarRenderer.DrawHorizontalThumb(g, Slider.ThumbRect, System.Windows.Forms.VisualStyles.TrackBarThumbState.Normal);
        }
	}
}
