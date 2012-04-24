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

using System;
using System.Drawing;
using OpenTK.Graphics;

namespace Paril.Drawing
{
	public struct NColor
	{
		private float _a;
		private float _b;
		private float _g;
		private float _h;
		private float _l;
		private float _r;
		private float _s;

		public NColor(byte r, byte g, byte b, byte a) :
			this()
		{
			_r = r / 255.0f;
			_g = g / 255.0f;
			_b = b / 255.0f;
			_a = a / 255.0f;

			SetHSL();
		}

		public NColor(float r, float g, float b, float a) :
			this()
		{
			_r = r;
			_g = g;
			_b = b;
			_a = a;

			SetHSL();
		}

		public NColor(NColor c) :
			this()
		{
			_r = c.R;
			_g = c.G;
			_b = c.B;
			_a = c.A;
			_h = c.Hue;
			_s = c.Saturation;
			_l = c.Luminance;
		}

		public NColor(float hue, float saturation, float lightness, byte alpha) :
			this()
		{
			_h = hue;
			_s = saturation;
			_l = lightness;
			_a = alpha / 255.0f;

			SetRGB();
		}

		public float R
		{
			get { return _r; }
			set
			{
				_r = value;
				SetHSL();
			}
		}

		public float G
		{
			get { return _g; }
			set
			{
				_g = value;
				SetHSL();
			}
		}

		public float B
		{
			get { return _b; }
			set
			{
				_b = value;
				SetHSL();
			}
		}

		public float A
		{
			get { return _a; }
			set { _a = value; }
		}

		public byte RByte
		{
			get { return (byte) (_r * 255.0f); }
		}

		public byte GByte
		{
			get { return (byte) (_g * 255.0f); }
		}

		public byte BByte
		{
			get { return (byte) (_b * 255.0f); }
		}

		public byte AByte
		{
			get { return (byte) (_a * 255.0f); }
		}

		public float Hue
		{
			get { return _h; }
			set
			{
				_h = value;
				SetRGB();
			}
		}

		public float Saturation
		{
			get { return _s; }
			set
			{
				_s = value;
				SetRGB();
			}
		}

		public float Luminance
		{
			get { return _l; }
			set
			{
				_l = value;
				SetRGB();
			}
		}

		public static implicit operator Color(NColor c)
		{
			return Color.FromArgb((byte) (c.A * 255.0f), (byte) (c.R * 255.0f), (byte) (c.G * 255.0f), (byte) (c.B * 255.0f));
		}

		public static implicit operator NColor(Color c)
		{
			return new NColor(c.R, c.G, c.B, c.A);
		}

		public static implicit operator Color4(NColor c)
		{
			return new Color4(c.R, c.G, c.B, c.A);
		}

		private float ComponentFromHue(float m1, float m2, float h)
		{
			h = (h + 1) % 1;
			if ((h * 6) < 1)
				return m1 + (m2 - m1) * 6 * h;
			else if ((h * 2) < 1)
				return m2;
			else if ((h * 3) < 2)
				return m1 + (m2 - m1) * ((2 / 3) - h) * 6;
			else
				return m1;
		}

		private void SetRGB()
		{
			if (_s == 0)
				_r = _g = _b = _l;
			else
			{
				float min, max, h = _h / 360.0f;

				max = _l < 0.5f ? _l * (1 + _s) : (_l + _s) - (_l * _s);
				min = (_l * 2) - max;

				_r = ComponentFromHue(min, max, h + (1.0f / 3.0f));
				_g = ComponentFromHue(min, max, h);
				_b = ComponentFromHue(min, max, h - (1.0f / 3.0f));
			}
		}

		private void SetHSL()
		{
			float max = Math.Max(Math.Max(_r, _g), _b);
			float min = Math.Min(Math.Min(_r, _g), _b);
			float chroma = max - min;

			_l = (max + min) / 2.0f;
			_s = 0;

			if (chroma != 0)
			{
				if (_r == max)
					_h = ((_g - _b) / chroma);
				else if (_g == max)
					_h = ((_b - _r) / chroma) + 2;
				else
					_h = ((_r - _g) / chroma) + 4;

				_h = 60 * ((_h + 6) % 6);
				_s = _l <= 0.5 ? (chroma / (_l * 2)) : (chroma / (2 - 2 * _l));
			}
		}

		public void SetRGBA(byte r, byte g, byte b, byte a)
		{
			_r = r;
			_g = g;
			_b = b;
			_a = a;

			SetHSL();
		}

		public void SetHSLA(ushort h, byte s, byte l, byte a)
		{
			_h = h;
			_s = s;
			_l = l;
			_a = a;

			SetRGB();
		}
	}
}