using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Devcorp.Controls.Design;
using System.Diagnostics;
using System.Security.Permissions;

namespace MultiPainter
{
	public partial class ColorPicker : Form
	{
		public ColorPicker()
		{
			InitializeComponent();
		}

		private void ColorPicker_Load(object sender, EventArgs e)
		{
			panel1.BackColor = Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(MyHSL).ToColor();
			saturationSlider1.Color = new HSL(colorSquare1.CurrentHue, (double)colorSquare1.CurrentSat / 240.0f, 0);
			SetColors();
		}

		bool _skipSet = false;

		void SetColors()
		{
			_skipSet = true;
			numericUpDown5.Value = CurrentColor.R;
			numericUpDown6.Value = CurrentColor.G;
			numericUpDown7.Value = CurrentColor.B;
			_skipSet = false;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			colorSquare1.CurrentHue = (int)numericUpDown1.Value;
			SetColors();
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			colorSquare1.CurrentSat = (int)numericUpDown2.Value;
			SetColors();
		}

		Devcorp.Controls.Design.HSL MyHSL
		{
			get { return new HSL(colorSquare1.CurrentHue, (float)colorSquare1.CurrentSat / 240.0f, (float)saturationSlider1.CurrentLum / 240.0f); }
		}

		public Color CurrentColor
		{
			get
			{
				return Color.FromArgb((int)numericUpDown4.Value, Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(MyHSL).ToColor());
			}

			set
			{
				var hsl = Devcorp.Controls.Design.ColorSpaceHelper.RGBtoHSL(value);

				numericUpDown1.Value = (decimal)hsl.Hue;
				numericUpDown2.Value = (decimal)hsl.Saturation * 240;
				numericUpDown3.Value = (decimal)hsl.Luminance * 240;
			}
		}

		private void colorSquare1_HueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			_skipSet = true;
			numericUpDown1.Value = colorSquare1.CurrentHue;

			saturationSlider1.Color = new HSL(colorSquare1.CurrentHue, (double)colorSquare1.CurrentSat / 240.0f, 0);
			panel1.BackColor = Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(MyHSL).ToColor();
			SetColors();
			_skipSet = false;
		}

		private void colorSquare1_SatChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			_skipSet = true;
			numericUpDown2.Value = colorSquare1.CurrentSat;

			saturationSlider1.Color = new HSL(colorSquare1.CurrentHue, (double)colorSquare1.CurrentSat / 240.0f, 0);
			panel1.BackColor = Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(MyHSL).ToColor();
			SetColors();
			_skipSet = false;
		}

		private void saturationSlider1_LumChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			_skipSet = true;
			numericUpDown3.Value = saturationSlider1.CurrentLum;

			panel1.BackColor = Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(MyHSL).ToColor();
			SetColors();
			_skipSet = false;
		}

		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			saturationSlider1.CurrentLum = (int)numericUpDown3.Value;
			saturationSlider1.Color = new HSL(colorSquare1.CurrentHue, (double)colorSquare1.CurrentSat / 240.0f, 0);
			panel1.BackColor = Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(MyHSL).ToColor();
		}

		private void numericUpDown5_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			var asRGB = Color.FromArgb((int)numericUpDown5.Value, (int)numericUpDown6.Value, (int)numericUpDown7.Value);
			var hsl = Devcorp.Controls.Design.ColorSpaceHelper.RGBtoHSL(asRGB);

			_skipSet = true;

			numericUpDown1.Value = (int)hsl.Hue;
			numericUpDown2.Value = (int)(hsl.Saturation * 240.0f);
			numericUpDown3.Value = (int)(hsl.Luminance * 240.0f);

			colorSquare1.CurrentHue = (int)numericUpDown1.Value;
			colorSquare1.CurrentSat = (int)numericUpDown2.Value;
			saturationSlider1.CurrentLum = (int)numericUpDown3.Value;
			saturationSlider1.Color = new HSL(colorSquare1.CurrentHue, (double)colorSquare1.CurrentSat / 240.0f, 0);
			panel1.BackColor = asRGB;

			_skipSet = false;
		}

		private void numericUpDown6_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			var asRGB = Color.FromArgb((int)numericUpDown5.Value, (int)numericUpDown6.Value, (int)numericUpDown7.Value);
			var hsl = Devcorp.Controls.Design.ColorSpaceHelper.RGBtoHSL(asRGB);

			_skipSet = true;

			numericUpDown1.Value = (int)hsl.Hue;
			numericUpDown2.Value = (int)(hsl.Saturation * 240.0f);
			numericUpDown3.Value = (int)(hsl.Luminance * 240.0f);

			colorSquare1.CurrentHue = (int)numericUpDown1.Value;
			colorSquare1.CurrentSat = (int)numericUpDown2.Value;
			saturationSlider1.CurrentLum = (int)numericUpDown3.Value;
			saturationSlider1.Color = new HSL(colorSquare1.CurrentHue, (double)colorSquare1.CurrentSat / 240.0f, 0);
			panel1.BackColor = asRGB;

			_skipSet = false;
		}

		private void numericUpDown7_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			var asRGB = Color.FromArgb((int)numericUpDown5.Value, (int)numericUpDown6.Value, (int)numericUpDown7.Value);
			var hsl = Devcorp.Controls.Design.ColorSpaceHelper.RGBtoHSL(asRGB);

			_skipSet = true;

			numericUpDown1.Value = (int)hsl.Hue;
			numericUpDown2.Value = (int)(hsl.Saturation * 240.0f);
			numericUpDown3.Value = (int)(hsl.Luminance * 240.0f);

			colorSquare1.CurrentHue = (int)numericUpDown1.Value;
			colorSquare1.CurrentSat = (int)numericUpDown2.Value;
			saturationSlider1.CurrentLum = (int)numericUpDown3.Value;
			saturationSlider1.Color = new HSL(colorSquare1.CurrentHue, (double)colorSquare1.CurrentSat / 240.0f, 0);
			panel1.BackColor = asRGB;

			_skipSet = false;
		}

		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}