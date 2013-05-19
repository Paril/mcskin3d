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
using OpenTK.Graphics;

namespace MCSkin3D
{
	public enum ColorBlendMode
	{
		Zero,
		One,
		SourceColor,
		InverseSourceColor,
		SourceAlpha,
		InverseSourceAlpha,
		DestinationAlpha,
		InverseDestinationAlpha,
		DestinationColor,
		InverseDestinationColor,
		SourceAlphaSaturation
	}

	public static class ColorBlending
	{
		public static Color4 BlendFunc(Color4 source, Color4 destination, ColorBlendMode mode)
		{
			switch (mode)
			{
				case ColorBlendMode.Zero:
					return new Color4(0.0f, 0.0f, 0.0f, 0.0f);
				case ColorBlendMode.One:
					return new Color4(1.0f, 1.0f, 1.0f, 1.0f);
				case ColorBlendMode.SourceColor:
					return new Color4(source.R, source.G, source.B, source.A);
				case ColorBlendMode.InverseSourceColor:
					return new Color4(1 - source.R, 1 - source.G, 1 - source.B, 1 - source.A);
				case ColorBlendMode.SourceAlpha:
					return new Color4(source.A, source.A, source.A, source.A);
				case ColorBlendMode.InverseSourceAlpha:
					return new Color4(1 - source.A, 1 - source.A, 1 - source.A, 1 - source.A);
				case ColorBlendMode.DestinationAlpha:
					return new Color4(destination.A, destination.A, destination.A, destination.A);
				case ColorBlendMode.InverseDestinationAlpha:
					return new Color4(1 - destination.A, 1 - destination.A, 1 - destination.A, 1 - destination.A);
				case ColorBlendMode.DestinationColor:
					return new Color4(destination.R, destination.G, destination.B, destination.A);
				case ColorBlendMode.InverseDestinationColor:
					return new Color4(1 - destination.R, 1 - destination.G, 1 - destination.B, 1 - destination.A);
				case ColorBlendMode.SourceAlphaSaturation:
					return new Color4(Math.Min(source.A, 1 - source.A), Math.Min(source.A, 1 - source.A),
					                  Math.Min(source.A, 1 - source.A), 1);
			}

			throw new Exception();
		}

		private static Color4 MultiplyColors(Color4 l, Color4 r)
		{
			return new Color4(l.R * r.R, l.G * r.G, l.B * r.B, l.A * r.A);
		}

		private static Color4 AddColors(Color4 l, Color4 r)
		{
			return new Color4(l.R + r.R, l.G + r.G, l.B + r.B, l.A + r.A);
		}

		private static Color4 InvertColor(Color4 c)
		{
			return new Color4(1 - c.R, 1 - c.G, 1 - c.B, 1 - c.A);
		}

		public static Color4 Add(Color4 source, ColorBlendMode sourceMode, Color4 destination, ColorBlendMode destMode)
		{
			return AddColors(MultiplyColors(source, BlendFunc(source, destination, sourceMode)),
			                 MultiplyColors(destination, BlendFunc(source, destination, destMode)));
		}

		public static Color4 AlphaBlend(Color4 source, Color4 dest)
		{
			if (dest.A == 0)
				return source;

			float alpha = Math.Min(source.A + dest.A, 1);
			Color4 val = Add(source, ColorBlendMode.SourceAlpha, dest, ColorBlendMode.InverseSourceAlpha);
			val.A = alpha;
			return val;
		}

		public static Color4 MultiplyByte(Color4 l, Color4 r)
		{
			var bL = (Color) l;
			var bR = (Color) r;

			int mR = bL.R * bR.R;
			int mG = bL.G * bR.G;
			int mB = bL.B * bR.B;
			int mA = bL.A * bR.A;

			return Color.FromArgb((mA / 255), (mR / 255), (mG / 255), (mB / 255));
		}

		public static Color4 Burn(Color4 source, float exposure)
		{
			Color4 color = MultiplyByte(source, new Color4(exposure, exposure, exposure, exposure));
			color.A = source.A;
			return color;
		}

		public static Color4 Dodge(Color4 source, float exposure)
		{
			Color4 invertedSource = InvertColor(source);
			var invertedBurn = new Color4(1 - exposure, 1 - exposure, 1 - exposure, 1 - exposure);
			return InvertColor(MultiplyByte(invertedSource, invertedBurn));
		}
	}
}