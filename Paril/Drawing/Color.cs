//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
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
using System.Runtime.InteropServices;
using OpenTK.Graphics;

namespace Paril.Drawing
{
	public enum ColorRepresentation
	{
		RGB,
		HSL,

		Total
	}

	// https://github.com/mjackson/mjijackson.github.com/blob/master/2008/02/rgb-to-hsl-and-rgb-to-hsv-color-model-conversion-algorithms-in-javascript.txt
	public struct ConvertableColor
	{
		unsafe struct Colors
		{
			[StructLayout(LayoutKind.Sequential)]
			public struct RGB
			{
				public float R, G, B;

				static float hue2rgb(ref float p, ref float q, float t)
				{
					if (t < 0.0f) t += 1.0f;
					if (t > 1.0f) t -= 1.0f;
					if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
					if (t < 1.0f / 2.0f) return q;
					if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
					return p;
				}

				public void ConvertFrom(ref HSL hsl)
				{
					if (hsl.S == 0)
					{
						R = G = B = hsl.L; // achromatic
						return;
					}

					var q = hsl.L < 0.5f ? hsl.L * (1 + hsl.S) : hsl.L + hsl.S - hsl.L * hsl.S;
					var p = 2.0f * hsl.L - q;

					R = hue2rgb(ref p, ref q, hsl.H + 1.0f / 3.0f);
					G = hue2rgb(ref p, ref q, hsl.H);
					B = hue2rgb(ref p, ref q, hsl.H - 1.0f / 3.0f);
				}
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct HSL
			{
				public float H, S, L;

				public void ConvertFrom(ref RGB rgb)
				{
					var max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
					var min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));

					L = (max + min) / 2;

					if (max == min)
					{
						H = S = 0;
						return;
					}

					var d = max - min;
					S = L > 0.5 ? d / (2 - max - min) : d / (max + min);

					if (max == rgb.R)
						H = (rgb.G - rgb.B) / d + (rgb.G < rgb.B ? 6 : 0);
					else if (max == rgb.G)
						H = (rgb.B - rgb.R) / d + 2;
					else if (max == rgb.B)
						H = (rgb.R - rgb.G) / d + 4;
					else
						throw new InvalidOperationException();

					H /= 6;
				}
			}
			
			public RGB rgb;
			public HSL hsl;
			public float a;
			
			public fixed bool synced[(int)ColorRepresentation.Total];

			/// <summary>
			/// Check whether this type is synced or not
			/// </summary>
			/// <param name="type">The type to check</param>
			/// <returns>Is synced or not</returns>
			public unsafe bool this[ColorRepresentation type]
			{
				get
				{
					fixed (bool* vals = synced)
						return vals[(int)type];
				}

				set
				{
					fixed (bool* vals = synced)
						vals[(int)type] = value;
				}
			}

			/// <summary>
			/// Sync two types up
			/// </summary>
			/// <param name="fromType">Type to convert from that is already synced</param>
			/// <param name="toType">Type to convert to that will be synced to the from type</param>
			public void ConvertFrom(ColorRepresentation fromType, ColorRepresentation toType)
			{
				// should never happen
				if (!this[fromType])
					throw new InvalidOperationException();

				// if they're already synced up, no worries
				if (this[toType])
					return;

				switch (toType)
				{
					case ColorRepresentation.RGB:
						switch (fromType)
						{
							case ColorRepresentation.HSL:
								rgb.ConvertFrom(ref hsl);
								break;
						}
						break;
					case ColorRepresentation.HSL:
						switch (fromType)
						{
							case ColorRepresentation.RGB:
								hsl.ConvertFrom(ref rgb);
								break;
						}
						break;
				}

				// synced
				this[toType] = true;
			}
		}

		Colors _colors;
		ColorRepresentation _type;

		/// <summary>
		/// Create a color from the specified color representation
		/// </summary>
		/// <param name="representation">The base representation</param>
		/// <param name="values">The values</param>
		public ConvertableColor(ColorRepresentation representation, params float[] values) :
			this()
		{
			if (values.Length < 3 || values.Length > 4)
				throw new IndexOutOfRangeException("values must contain no less than 3 and no more than 4 values");

			if (values.Length >= 4)
				_colors.a = values[3];
			else
				_colors.a = 1;

			_type = representation;

			switch (_type)
			{
				case ColorRepresentation.RGB:
					_colors.rgb = new Colors.RGB() { R = values[0], G = values[1], B = values[2] };
					break;
				case ColorRepresentation.HSL:
					_colors.hsl = new Colors.HSL() { H = values[0], S = values[1], L = values[2] };
					break;
			}

			_colors[_type] = true;
		}

		/// <summary>
		/// Set the base type of the color to the new representation.
		/// </summary>
		/// <param name="representation">The new representation</param>
		public void Convert(ColorRepresentation representation)
		{
			if (_type == representation)
				return;

			_colors.ConvertFrom(_type, representation);
			_type = representation;
		}

		void EnsureSync(ColorRepresentation newType)
		{
			if (_type == newType)
				return;

			_colors.ConvertFrom(_type, newType);
		}

		void EnsureDesync(ColorRepresentation newType)
		{
			for (ColorRepresentation i = 0; i < ColorRepresentation.Total; ++i)
			{
				if (i == newType)
					continue;

				_colors[i] = false;
			}

			_type = newType;
		}

		public float A
		{
			get { return _colors.a; }
			set { _colors.a = value; }
		}

		public float R
		{
			get
			{
				EnsureSync(ColorRepresentation.RGB);
				return _colors.rgb.R;
			}

			set
			{
				EnsureSync(ColorRepresentation.RGB);
				_colors.rgb.R = value;
				EnsureDesync(ColorRepresentation.RGB);
			}
		}

		public float G
		{
			get
			{
				EnsureSync(ColorRepresentation.RGB);
				return _colors.rgb.G;
			}

			set
			{
				EnsureSync(ColorRepresentation.RGB);
				_colors.rgb.G = value;
				EnsureDesync(ColorRepresentation.RGB);
			}
		}

		public float B
		{
			get
			{
				EnsureSync(ColorRepresentation.RGB);
				return _colors.rgb.B;
			}

			set
			{
				EnsureSync(ColorRepresentation.RGB);
				_colors.rgb.B = value;
				EnsureDesync(ColorRepresentation.RGB);
			}
		}

		public float H
		{
			get
			{
				EnsureSync(ColorRepresentation.HSL);
				return _colors.hsl.H;
			}

			set
			{
				EnsureSync(ColorRepresentation.HSL);
				_colors.hsl.H = value;
				EnsureDesync(ColorRepresentation.HSL);
			}
		}

		public float S
		{
			get
			{
				EnsureSync(ColorRepresentation.HSL);
				return _colors.hsl.S;
			}

			set
			{
				EnsureSync(ColorRepresentation.HSL);
				_colors.hsl.S = value;
				EnsureDesync(ColorRepresentation.HSL);
			}
		}

		public float L
		{
			get
			{
				EnsureSync(ColorRepresentation.HSL);
				return _colors.hsl.L;
			}

			set
			{
				EnsureSync(ColorRepresentation.HSL);
				_colors.hsl.L = value;
				EnsureDesync(ColorRepresentation.HSL);
			}
		}

		public System.Drawing.Color ToColor()
		{
			int r = (int)(R * 255.0f);
			int g = (int)(G * 255.0f);
			int b = (int)(B * 255.0f);
			int a = (int)(A * 255.0f);

			return System.Drawing.Color.FromArgb(a, r, g, b);
		}
	}
}