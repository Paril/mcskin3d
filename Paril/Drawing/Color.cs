using System;

namespace Paril.Drawing
{
	internal static class ColorTables
	{
		public static float[] ByteToFloat;
		public static byte[] LByteToRGBByte;

		static ColorTables()
		{
			ByteToFloat = new float[256];

			for (int i = 0; i < 256; ++i)
				ByteToFloat[i] = (float)i / 255;

			LByteToRGBByte = new byte[240];

			for (int i = 0; i < 240; ++i)
				LByteToRGBByte[i] = (byte)((i / 240.0f) * 256.0f);
		}
	}

	public struct Color
	{
		byte _r, _g, _b, _a;
		ushort _h;
		byte _s, _l;

		public Color(byte r, byte g, byte b, byte a) :
			this()
		{
			_r = r;
			_g = g;
			_b = b;
			_a = a;

			SetHSL();
		}

		public Color(ushort hue, byte saturation, byte lightness, byte alpha) :
			this()
		{
			_h = hue;
			_s = saturation;
			_l = lightness;
			_a = alpha;

			SetRGB();
		}

		public static implicit operator System.Drawing.Color(Color c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static implicit operator Color(System.Drawing.Color c)
		{
			return new Color(c.R, c.G, c.B, c.A);
		}

		public static implicit operator OpenTK.Graphics.Color4(Color c)
		{
			return new OpenTK.Graphics.Color4(c.R, c.G, c.B, c.A);
		}

		public byte R
		{
			get { return _r; }
			set { _r = value; SetHSL(); }
		}

		public byte G
		{
			get { return _g; }
			set { _g = value; SetHSL(); }
		}

		public byte B
		{
			get { return _b; }
			set { _b = value; SetHSL(); }
		}

		public byte A
		{
			get { return _a; }
			set { _a = value; }
		}

		public ushort Hue
		{
			get { return _h; }
			set { _h = value; SetRGB(); }
		}

		public byte Saturation
		{
			get { return _s; }
			set { _s = value; SetRGB(); }
		}

		public byte Luminance
		{
			get { return _l; }
			set { _l = value; SetRGB(); }
		}

		const float oneDivSix = 1.0f / 6.0f;
		const float oneDivThree = 1.0f / 3.0f;
		const float twoDivThree = 2.0f / 3.0f;

		void SetRGB()
		{
			if (_l == 0)
			{
				_r = _g = _b = ColorTables.LByteToRGBByte[_l];
				return;
			}

			float hF = _h / 360.0f;
			float sF = _s / 240.0f;
			float lF = _l / 240.0f;

			float temp2;

			if (lF < 0.5f)
				temp2 = lF * (1.0f + sF);
			else
				temp2 = (lF + sF) - (lF * sF);

			float temp1 = 2.0f * lF - temp2;

			float rF = RGBTest(ref temp1, ref temp2, hF + oneDivThree);
			float gF = RGBTest(ref temp1, ref temp2, hF);
			float bF = RGBTest(ref temp1, ref temp2, hF - oneDivThree);

			_r = (byte)(rF * 256.0f);
			_g = (byte)(gF * 256.0f);
			_b = (byte)(bF * 256.0f);
		}

		float RGBTest(ref float temp1, ref float temp2, float temp3)
		{
			if (temp3 < 0)
				temp3 += 1.0f;
			else if (temp3 > 1)
				temp3 -= 1.0f;

			if ((6.0f * temp3) < 1)
				return Math.Max(0.0f, Math.Min(1.0f, temp1 + (temp2 - temp1) * 6.0f * temp3));
			else if ((2.0f * temp3) < 1)
				return temp2;
			else if ((3.0f * temp3) < 2)
				return Math.Max(0.0f, Math.Min(1.0f, temp1 + (temp2 - temp1) * ((twoDivThree) - temp3) * 6.0f));

			return temp1;
		}

		void SetHSL()
		{
			float rF = ColorTables.ByteToFloat[_r];
			float gF = ColorTables.ByteToFloat[_g];
			float bF = ColorTables.ByteToFloat[_b];

			float max = Math.Max(rF, Math.Max(gF, bF));
			float min = Math.Min(rF, Math.Min(gF, bF));
			float delta = max - min;

			float lF = (max + min) / 2.0f;
			float sF = 0;

			if (delta != 0)
			{
				if (lF < 0.5f)
					sF = (max - min) / (max + min);
				else
					sF = (max - min) / (2.0f - max - min);

				float hF = 0;

				float del_R = (((max - rF) * oneDivSix) + (delta * 0.5f)) / delta;
				float del_G = (((max - gF) * oneDivSix) + (delta * 0.5f)) / delta;
				float del_B = (((max - bF) * oneDivSix) + (delta * 0.5f)) / delta;

				if (rF == max)
					hF = del_B - del_G;
				else if (gF == max)
					hF = (oneDivThree) + del_R - del_B;
				else if (bF == max)
					hF = (twoDivThree) + del_G - del_R;

				if (hF < 0)
					hF += 1;

				if (hF > 1)
					hF -= 1;

				_h = (ushort)(hF * 360);
			}

			_s = (byte)(sF * 240);
			_l = (byte)(lF * 240);

			SetRGB();
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
