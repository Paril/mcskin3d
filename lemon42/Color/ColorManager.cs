//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Drawing;
using System;

namespace MCSkin3D.lemon42
{
	//ColorManager class that will help provide simple and efficient
	//color translations.

	public class ColorManager
	{
		public enum ColorSpace
		{
			RGB = 0,
			HSV = 1
		}

		public struct RGBColor
		{
			public byte R;
			public byte G;
			public byte B;
			public byte A;

			public static implicit operator Color(RGBColor c)
			{
				return c.ToColor();
			}

			public static explicit operator HSVColor(RGBColor c)
			{
				return c.ToHSVColor();
			}

			public static implicit operator RGBColor(Color c)
			{
				return new RGBColor(c.R, c.G, c.B, c.A);
			}

			public RGBColor(byte red, byte green, byte blue, byte alpha)
			{
				R = red;
				G = green;
				B = blue;
				A = alpha;
			}

			public RGBColor(byte red, byte green, byte blue)
			{
				R = red;
				G = green;
				B = blue;
				A = 255;
			}

			//(to)
			public Color ToColor()
			{
				return Color.FromArgb(A, R, G, B);
			}

			public HSVColor ToHSVColor()
			{
				return RGBtoHSV(R, G, B, A);
			}

			public override string ToString()
			{
				return "RGBA(" + R + ", " + G + ", " + B + ", " + A + ")";
			}
		}

		public struct HSVColor
		{
			public short H;
			public byte S;
			public byte V;
			public byte A;

			public static explicit operator Color(HSVColor c)
			{
				return c.ToColor();
			}

			public static explicit operator RGBColor(HSVColor c)
			{
				return c.ToRGBColor();
			}

			public static explicit operator HSVColor(Color c)
			{
				return RGBtoHSV(c);
			}

			public HSVColor(short hue, byte saturation, byte value, byte alpha)
			{
				H = hue;
				S = saturation;
				V = value;
				A = alpha;
			}

			public HSVColor(short hue, byte saturation, byte value)
			{
				H = hue;
				S = saturation;
				V = value;
				A = 255;
			}

			//(to)
			public Color ToColor()
			{
				return HSVtoRGB(H, S, V, A);
			}

			public RGBColor ToRGBColor()
			{
				Color c = ToColor();
				return new RGBColor(c.R, c.G, c.B, c.A);
			}

			public override string ToString()
			{
				return "HSVA(" + H + "º, " + S + "%, " + V + "%, " + A + ")";
			}
		}

		public static HSVColor RGBtoHSV(Color c)
		{
			return RGBtoHSV(c.R, c.G, c.B, c.A);
		}

		public static HSVColor RGBtoHSV(byte r, byte g, byte b, byte a)
		{
			//init variables
			double h = 0;
			double s = 0;
			double v = 0;

			// R,G,B ∈ [0, 1]
			double red = r / 255.0f;
			double green = g / 255.0f;
			double blue = b / 255.0f;
			
			//other variables
			double max = Math.Max(Math.Max(red, green), blue);
			double min = Math.Min(Math.Min(red, green), blue);
			double chroma = max - min;

			//CALCULATION OF HUE
			if (max == min)
				h = 0;
			else if (max == red)
				h = (60 * (green - blue) / chroma + 360) % 360;
			else if (max == green)
				h = 60 * (blue - red) / chroma + 120;
			else if (max == blue)
				h = 60 * (red - green) / chroma + 240;

			//CALCULATION OF SATURATION
			s = max == 0 ? 0 : 1 - (min / max);
			//CALCULATION OF VALUE
			v = max;

			//V,S is a %
			v = Math.Round(v * 100);
			s = Math.Round(s * 100);
			h = Math.Round(h);
			return new HSVColor((short)h, (byte)s, (byte)v, a);
		}

		public static Color HSVtoRGB(HSVColor c)
		{
			return HSVtoRGB(c.H, c.S, c.V, c.A);
		}

		public static Color HSVtoRGB(short h, byte s, byte v, byte a)
		{
			double r = 0;
			double g = 0;
			double b = 0;
			double hue = h;
			if (hue >= 360)
				hue = hue - 360;
			double sat = s / 100.0f;
			double val = v / 100.0f;
			//variables and calculation
			int tempT = (int)Math.Floor((hue / 60) % 6);
			double f = hue / 60.0f - tempT;
			double l = val * (1 - sat);
			double m = val * (1 - f * sat);
			double n = val * (1 - (1 - f) * sat);

			switch (tempT)
			{
			case 0:
				r = val; g = n; b = l; break;
			case 1:
				r = m; g = val; b = l; break;
			case 2:
				r = l; g = val; b = n; break;
			case 3:
				r = l; g = m; b = val; break;
			case 4:
				r = n; g = l; b = val; break;
			case 5:
				r = val; g = l; b = m; break;
			}

			r = Math.Round(r * 255);
			g = Math.Round(g * 255);
			b = Math.Round(b * 255);

			return Color.FromArgb((byte)r, (byte)g, (byte)b);
		}


		ColorSpace _colorspace;
		RGBColor _rgb;
		HSVColor _hsv;
		public event ColorChangedEventHandler ColorChanged;
		public delegate void ColorChangedEventHandler();

		public ColorManager()
		{
			_colorspace = ColorSpace.RGB;
			_rgb = new RGBColor(0, 0, 0, 255);
			_hsv = _rgb.ToHSVColor();
		}

		public ColorManager(RGBColor rgb)
		{
			_colorspace = ColorSpace.RGB;
			_rgb = rgb;
			_hsv = _rgb.ToHSVColor();
		}

		public ColorManager(HSVColor hsv)
		{
			_colorspace = ColorSpace.HSV;
			_hsv = hsv;
			_rgb = _hsv.ToRGBColor();
		}

		public ColorSpace CurrentSpace
		{
			get { return _colorspace; }
			set { _colorspace = value; }
		}

		public RGBColor RGB
		{
			get { return _rgb; }
			set
			{
				_rgb = value;
				_hsv = value.ToHSVColor();
				_colorspace = ColorSpace.RGB;
				if (ColorChanged != null)
					ColorChanged();
			}
		}

		public HSVColor HSV
		{
			get { return _hsv; }
			set
			{
				_hsv = value;
				_rgb = value.ToRGBColor();
				_colorspace = ColorSpace.HSV;
				if (ColorChanged != null)
					ColorChanged();
			}
		}

		public static ColorManager FromRGBA(byte r, byte g, byte b, byte a)
		{
			return new ColorManager(new RGBColor(r, g, b, a));
		}

		public static ColorManager FromHSVA(int h, byte s, byte v, byte a)
		{
			return new ColorManager(new HSVColor((short)h, s, v, a));
		}
	}

}
