using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using MCSkin3D.lemon42;
using Paril.Controls.Color;

namespace MCSkin3D.Controls
{
	public partial class ColorPanel : UserControl
	{
		#region Events

		private readonly AlphaSliderRenderer _alphaRenderer;
		private readonly ColorSliderRenderer _blueRenderer;
		private readonly ColorSliderRenderer _greenRenderer;
		private readonly HueSliderRenderer _hueRenderer;
		private readonly ColorSliderRenderer _redRenderer;
		private readonly SaturationSliderRenderer _saturationRenderer;
		private readonly ValueSliderRenderer _valueRenderer;
		private bool _editingHex;

		private ColorManager
			_primaryColor = ColorManager.FromRGBA(255, 255, 255, 255),
			_secondaryColor = ColorManager.FromRGBA(0, 0, 0, 255);

		private bool _secondaryIsFront;
		private bool _skipColors;

		public ColorPanel()
		{
			InitializeComponent();

			redColorSlider.Renderer = _redRenderer = new ColorSliderRenderer(redColorSlider);
			greenColorSlider.Renderer = _greenRenderer = new ColorSliderRenderer(greenColorSlider);
			blueColorSlider.Renderer = _blueRenderer = new ColorSliderRenderer(blueColorSlider);
			alphaColorSlider.Renderer = _alphaRenderer = new AlphaSliderRenderer(alphaColorSlider);

			hueColorSlider.Renderer = _hueRenderer = new HueSliderRenderer(hueColorSlider);
			saturationColorSlider.Renderer = _saturationRenderer = new SaturationSliderRenderer(saturationColorSlider);
			valueColorSlider.Renderer = _valueRenderer = new ValueSliderRenderer(valueColorSlider);

			SetColor(ColorManager.FromRGBA(255, 255, 255, 255));
		}

		#region Properties

		private ColorPreview SelectedColorPreview
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

		public SwatchContainer SwatchContainer
		{
			get { return swatchContainer; }
		}

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

		private void SetColor(Control colorPreview, ref ColorManager currentColor, ColorManager newColor)
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

			_redRenderer.StartColor = Color.FromArgb(255, 0, currentColor.RGB.G, currentColor.RGB.B);
			_greenRenderer.StartColor = Color.FromArgb(255, currentColor.RGB.R, 0, currentColor.RGB.B);
			_blueRenderer.StartColor = Color.FromArgb(255, currentColor.RGB.R, currentColor.RGB.G, 0);

			_redRenderer.EndColor = Color.FromArgb(255, 255, currentColor.RGB.G, currentColor.RGB.B);
			_greenRenderer.EndColor = Color.FromArgb(255, currentColor.RGB.R, 255, currentColor.RGB.B);
			_blueRenderer.EndColor = Color.FromArgb(255, currentColor.RGB.R, currentColor.RGB.G, 255);

			_alphaRenderer.EndColor = Color.FromArgb(255, currentColor.RGB.ToColor());

			_hueRenderer.CurrentColor = currentColor;
			_saturationRenderer.CurrentColor = currentColor;
			_valueRenderer.CurrentColor = currentColor;

			redColorSlider.Value = currentColor.RGB.R;
			greenColorSlider.Value = currentColor.RGB.G;
			blueColorSlider.Value = currentColor.RGB.B;
			alphaColorSlider.Value = currentColor.RGB.A;

			hueColorSlider.Value = currentColor.HSV.H;
			saturationColorSlider.Value = currentColor.HSV.S;
			valueColorSlider.Value = currentColor.HSV.V;

			if (!_editingHex)
			{
				textBox1.Text = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", newColor.RGB.R, newColor.RGB.G, newColor.RGB.B,
				                              newColor.RGB.A);
			}

			_skipColors = false;
		}

		private void SetColor(ColorManager c)
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

		private void colorTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (colorTabControl.SelectedIndex == 0 || colorTabControl.SelectedIndex == 1)
			{
				var panel = (Panel) colorTabControl.SelectedTab.Controls[0];

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

		private void hueNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA((short) hueNumericUpDown.Value, SelectedColor.HSV.S, SelectedColor.HSV.V,
			                               SelectedColor.HSV.A));
		}

		private void saturationNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, (byte) saturationNumericUpDown.Value, SelectedColor.HSV.V,
			                               SelectedColor.HSV.A));
		}

		private void valueNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, SelectedColor.HSV.S, (byte) valueNumericUpDown.Value,
			                               SelectedColor.HSV.A));
		}

		private void redNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA((byte) redNumericUpDown.Value, SelectedColor.RGB.G, SelectedColor.RGB.B,
			                               SelectedColor.RGB.A));
		}

		private void greenNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, (byte) greenNumericUpDown.Value, SelectedColor.RGB.B,
			                               SelectedColor.RGB.A));
		}

		private void blueNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, SelectedColor.RGB.G, (byte) blueNumericUpDown.Value,
			                               SelectedColor.RGB.A));
		}

		private void alphaNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, SelectedColor.RGB.G, SelectedColor.RGB.B,
			                               (byte) alphaNumericUpDown.Value));
		}

		private void hueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(e.NewValue, SelectedColor.HSV.S, SelectedColor.HSV.V, SelectedColor.HSV.A));
		}

		private void saturationColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, (byte) e.NewValue, SelectedColor.HSV.V, SelectedColor.HSV.A));
		}

		private void valueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, SelectedColor.HSV.S, (byte) e.NewValue, SelectedColor.HSV.A));
		}

		private void redColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA((byte) e.NewValue, SelectedColor.RGB.G, SelectedColor.RGB.B, SelectedColor.RGB.A));
		}

		private void greenColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, (byte) e.NewValue, SelectedColor.RGB.B, SelectedColor.RGB.A));
		}

		private void blueColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromRGBA(SelectedColor.RGB.R, SelectedColor.RGB.G, (byte) e.NewValue, SelectedColor.RGB.A));
		}

		private void alphaColorSlider_Scroll(object sender, ScrollEventArgs e)
		{
			if (_skipColors)
				return;

			SetColor(ColorManager.FromHSVA(SelectedColor.HSV.H, SelectedColor.HSV.S, SelectedColor.HSV.V, (byte) e.NewValue));
		}

		private void swatchContainer_SwatchChanged(object sender, SwatchChangedEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				SelectedColor = ColorManager.FromRGBA(e.Swatch.R, e.Swatch.G, e.Swatch.B, e.Swatch.A);
			else
				UnselectedColor = ColorManager.FromRGBA(e.Swatch.R, e.Swatch.G, e.Swatch.B, e.Swatch.A);
		}

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

			SetColor(ColorManager.FromHSVA(colorPick1.CurrentHSV.H, colorPick1.CurrentHSV.S, colorPick1.CurrentHSV.V,
			                               SelectedColor.HSV.A));
		}
	}

	#endregion
}