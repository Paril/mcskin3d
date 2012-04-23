using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCSkin3D.lemon42;
using System.Globalization;

namespace MCSkin3D.Controls
{
	public partial class ColorPanel : UserControl
	{
		ColorManager
					_primaryColor = ColorManager.FromRGBA(255, 255, 255, 255),
					_secondaryColor = ColorManager.FromRGBA(0, 0, 0, 255);

		bool _skipColors = false;
		bool _secondaryIsFront = false;

		ColorSliderRenderer redRenderer, greenRenderer, blueRenderer;
		AlphaSliderRenderer alphaRenderer;
		HueSliderRenderer hueRenderer;
		SaturationSliderRenderer saturationRenderer;
		ValueSliderRenderer valueRenderer;

		public ColorPanel()
		{
			InitializeComponent();

			redColorSlider.Renderer = redRenderer = new ColorSliderRenderer(redColorSlider);
			greenColorSlider.Renderer = greenRenderer = new ColorSliderRenderer(greenColorSlider);
			blueColorSlider.Renderer = blueRenderer = new ColorSliderRenderer(blueColorSlider);
			alphaColorSlider.Renderer = alphaRenderer = new AlphaSliderRenderer(alphaColorSlider);

			hueColorSlider.Renderer = hueRenderer = new HueSliderRenderer(hueColorSlider);
			saturationColorSlider.Renderer = saturationRenderer = new SaturationSliderRenderer(saturationColorSlider);
			valueColorSlider.Renderer = valueRenderer = new ValueSliderRenderer(valueColorSlider);

			SetColor(ColorManager.FromRGBA(255, 255, 255, 255));
		}

		#region Properties
		Paril.Controls.Color.ColorPreview SelectedColorPreview
		{
			get { return (_secondaryIsFront) ? colorPreview2 : colorPreview1; }
		}

		public TextBox HexTextBox
		{
			get { return textBox1; }
		}

		public ColorManager SelectedColor
		{
			get { return (_secondaryIsFront) ? _secondaryColor : _primaryColor; }
			set { SetColor(value); }
		}

		public ColorManager UnselectedColor
		{
			get { return (!_secondaryIsFront) ? _secondaryColor : _primaryColor; }
			set
			{
				if (_secondaryIsFront)
				{
					if (swatchContainer.InEditMode)
					{
						if (swatchContainer.SwatchDisplayer.HasPrimaryColor)
							swatchContainer.SwatchDisplayer.PrimaryColor = value.RGB;
					}

					SetColor(colorPreview1, ref _primaryColor, value);
				}
				else
				{
					if (swatchContainer.InEditMode)
					{
						if (swatchContainer.SwatchDisplayer.HasSecondaryColor)
							swatchContainer.SwatchDisplayer.SecondaryColor = value.RGB;
					}

					SetColor(colorPreview2, ref _secondaryColor, value);
				}
			}
		}

		public SwatchContainer SwatchContainer { get { return swatchContainer; } }
		#endregion

		#region Public Methods
		public void SwitchColors()
		{
			if (_secondaryIsFront)
				colorPreview1_Click(null, null);
			else
				colorPreview2_Click(null, null);
		}

		public void SetLoading(bool isLoading)
		{
			loadingSwatchLabel.Visible = isLoading;
		}
		#endregion

		#region Events
		void SetColor(Control colorPreview, ref ColorManager currentColor, ColorManager newColor)
		{
			currentColor = newColor;
			colorPreview.ForeColor = currentColor.RGB;

			if (colorPreview != SelectedColorPreview)
				return;

			_skipColors = true;
			colorPick1.CurrentHSV = currentColor.HSV;

			redNumericUpDown.Value = currentColor.RGB.R;
			greenNumericUpDown.Value = currentColor.RGB.G;
			blueNumericUpDown.Value = currentColor.RGB.B;
			alphaNumericUpDown.Value = currentColor.RGB.A;

			hueNumericUpDown.Value = currentColor.HSV.H;
			saturationNumericUpDown.Value = currentColor.HSV.S;
			valueNumericUpDown.Value = currentColor.HSV.V;

			redRenderer.StartColor = Color.FromArgb(255, 0, currentColor.RGB.G, currentColor.RGB.B);
			greenRenderer.StartColor = Color.FromArgb(255, currentColor.RGB.R, 0, currentColor.RGB.B);
			blueRenderer.StartColor = Color.FromArgb(255, currentColor.RGB.R, currentColor.RGB.G, 0);

			redRenderer.EndColor = Color.FromArgb(255, 255, currentColor.RGB.G, currentColor.RGB.B);
			greenRenderer.EndColor = Color.FromArgb(255, currentColor.RGB.R, 255, currentColor.RGB.B);
			blueRenderer.EndColor = Color.FromArgb(255, currentColor.RGB.R, currentColor.RGB.G, 255);

			alphaRenderer.EndColor = Color.FromArgb(255, currentColor.RGB.ToColor());

			hueRenderer.CurrentColor = currentColor;
			saturationRenderer.CurrentColor = currentColor;
			valueRenderer.CurrentColor = currentColor;

			redColorSlider.Value = currentColor.RGB.R;
			greenColorSlider.Value = currentColor.RGB.G;
			blueColorSlider.Value = currentColor.RGB.B;
			alphaColorSlider.Value = currentColor.RGB.A;

			hueColorSlider.Value = currentColor.HSV.H;
			saturationColorSlider.Value = currentColor.HSV.S;
			valueColorSlider.Value = currentColor.HSV.V;

			if (!_editingHex)
				textBox1.Text = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", newColor.RGB.R, newColor.RGB.G, newColor.RGB.B, newColor.RGB.A);

			_skipColors = false;
		}

		void SetColor(ColorManager c)
		{
			if (_secondaryIsFront)
			{
				if (swatchContainer.InEditMode)
				{
					if (swatchContainer.SwatchDisplayer.HasSecondaryColor)
						swatchContainer.SwatchDisplayer.SecondaryColor = c.RGB;
				}

				SetColor(colorPreview2, ref _secondaryColor, c);
			}
			else
			{
				if (swatchContainer.InEditMode)
				{
					if (swatchContainer.SwatchDisplayer.HasPrimaryColor)
						swatchContainer.SwatchDisplayer.PrimaryColor = c.RGB;
				}

				SetColor(colorPreview1, ref _primaryColor, c);
			}
		}

		void colorTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (colorTabControl.SelectedIndex == 0 || colorTabControl.SelectedIndex == 1)
			{
				var panel = (Panel)colorTabControl.SelectedTab.Controls[0];

				panel.Controls.Add(colorPick1);
				panel.Controls.Add(colorPreview1);
				panel.Controls.Add(colorPreview2);
				panel.Controls.Add(label5);
				panel.Controls.Add(alphaColorSlider);
				panel.Controls.Add(alphaNumericUpDown);

				if (_secondaryIsFront)
				{
					colorPreview2.BringToFront();
					colorPreview1.SendToBack();
				}
				else
				{
					colorPreview2.SendToBack();
					colorPreview1.BringToFront();
				}
			}
		}

		private void colorPreview1_Click(object sender, EventArgs e)
		{
			_secondaryIsFront = false;
			colorPreview1.BringToFront();

			SetColor(_primaryColor);
		}

		private void colorPreview2_Click(object sender, EventArgs e)
		{
			_secondaryIsFront = true;
			colorPreview2.BringToFront();

			SetColor(_secondaryColor);
		}

		void hueNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA((short)hueNumericUpDown.Value, SelectedColor.HSV.S, SelectedColor.HSV.V, SelectedColor.HSV.A));
		}

		void saturationNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, (byte)saturationNumericUpDown.Value, SelectedColor.HSV.V, SelectedColor.HSV.A));
		}

		void valueNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, SelectedColor.HSV.S, (byte)valueNumericUpDown.Value, SelectedColor.HSV.A));
		}

		void redNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA((byte)redNumericUpDown.Value, SelectedColor.RGB.G, SelectedColor.RGB.B, SelectedColor.RGB.A));
		}

		void greenNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, (byte)greenNumericUpDown.Value, SelectedColor.RGB.B, SelectedColor.RGB.A));
		}

		void blueNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, SelectedColor.RGB.G, (byte)blueNumericUpDown.Value, SelectedColor.RGB.A));
		}

		void alphaNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, SelectedColor.RGB.G, SelectedColor.RGB.B, (byte)alphaNumericUpDown.Value));
		}

		void hueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(e.NewValue, SelectedColor.HSV.S, SelectedColor.HSV.V, SelectedColor.HSV.A));
		}

		void saturationColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, (byte)e.NewValue, SelectedColor.HSV.V, SelectedColor.HSV.A));
		}

		void valueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, SelectedColor.HSV.S, (byte)e.NewValue, SelectedColor.HSV.A));
		}

		void redColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA((byte)e.NewValue, SelectedColor.RGB.G, SelectedColor.RGB.B, SelectedColor.RGB.A));
		}

		void greenColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, (byte)e.NewValue, SelectedColor.RGB.B, SelectedColor.RGB.A));
		}

		void blueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, SelectedColor.RGB.G, (byte)e.NewValue, SelectedColor.RGB.A));
		}

		void alphaColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, SelectedColor.HSV.S, SelectedColor.HSV.V, (byte)e.NewValue));
		}

		void swatchContainer_SwatchChanged(object sender, SwatchChangedEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				SelectedColor = ColorManager.FromRGBA(e.Swatch.R, e.Swatch.G, e.Swatch.B, e.Swatch.A);
			else
				UnselectedColor = ColorManager.FromRGBA(e.Swatch.R, e.Swatch.G, e.Swatch.B, e.Swatch.A);
		}

		bool _editingHex = false;
		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			try
			{
				if (textBox1.Text.Contains('#'))
					textBox1.Text = textBox1.Text.Replace("#", "");

				if (textBox1.Text.Length > 8)
					textBox1.Text = textBox1.Text.Remove(8);

				string realHex = textBox1.Text;

				while (realHex.Length != 8)
					realHex += 'F';

				byte r = byte.Parse(realHex.Substring(0, 2), NumberStyles.HexNumber);
				byte g = byte.Parse(realHex.Substring(2, 2), NumberStyles.HexNumber);
				byte b = byte.Parse(realHex.Substring(4, 2), NumberStyles.HexNumber);
				byte a = byte.Parse(realHex.Substring(6, 2), NumberStyles.HexNumber);

				_editingHex = true;
				SetColor(ColorManager.FromRGBA(r, g, b, a));
				_editingHex = false;
			}
			catch
			{
			}
		}

		private void colorPick1_HSVChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(colorPick1.CurrentHSV.H, colorPick1.CurrentHSV.S, colorPick1.CurrentHSV.V, SelectedColor.HSV.A));
		}
	}
		#endregion
}
