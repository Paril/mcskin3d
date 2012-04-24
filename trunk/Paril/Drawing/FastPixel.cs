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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Paril.Drawing
{
	public class FastPixel : IDisposable
	{
		private readonly bool _autoRelease;
		private readonly Bitmap _bitmap;
		private readonly int _height;
		private readonly bool _isAlpha;
		private readonly int _width;
		private BitmapData bmpData;
		private IntPtr bmpPtr;
		private bool locked;
		private byte[] rgbValues;

		public FastPixel(Bitmap bitmap)
		{
			if (bitmap == null)
				throw new ArgumentException("FastPixel with no bitmap");
			if ((bitmap.PixelFormat == (bitmap.PixelFormat | PixelFormat.Indexed)))
				throw new ArgumentException("Cannot lock an Indexed image.");
			_bitmap = bitmap;
			_isAlpha = (Bitmap.PixelFormat == (Bitmap.PixelFormat | PixelFormat.Alpha));
			_width = bitmap.Width;
			_height = bitmap.Height;
			_autoRelease = false;
		}

		public FastPixel(Bitmap bitmap, bool autoRelease)
		{
			if (bitmap == null)
				throw new ArgumentException("FastPixel with no bitmap");
			if ((bitmap.PixelFormat == (bitmap.PixelFormat | PixelFormat.Indexed)))
				throw new ArgumentException("Cannot lock an Indexed image.");
			_bitmap = bitmap;
			_isAlpha = (Bitmap.PixelFormat == (Bitmap.PixelFormat | PixelFormat.Alpha));
			_width = bitmap.Width;
			_height = bitmap.Height;
			_autoRelease = autoRelease;

			Lock();
		}

		public int Width
		{
			get { return _width; }
		}

		public int Height
		{
			get { return _height; }
		}

		public bool IsAlphaBitmap
		{
			get { return _isAlpha; }
		}

		public Bitmap Bitmap
		{
			get { return _bitmap; }
		}

		public void Lock()
		{
			if (locked)
				throw new ArgumentException("Bitmap already locked.");

			var rect = new Rectangle(0, 0, Width, Height);
			bmpData = Bitmap.LockBits(rect, ImageLockMode.ReadWrite, Bitmap.PixelFormat);
			bmpPtr = bmpData.Scan0;

			if (IsAlphaBitmap)
			{
				int bytes = (Width * Height) * 4;
				rgbValues = new byte[bytes];

				Marshal.Copy(bmpPtr, rgbValues, 0, rgbValues.Length);
			}
			else
			{
				int bytes = (Width * Height) * 3;
				rgbValues = new byte[bytes];

				Marshal.Copy(bmpPtr, rgbValues, 0, rgbValues.Length);
			}

			locked = true;
		}

		public void Unlock(bool setPixels)
		{
			if (!locked)
				throw new ArgumentException("Bitmap not locked.");
			// Copy the RGB values back to the bitmap
			if (setPixels) Marshal.Copy(rgbValues, 0, bmpPtr, rgbValues.Length);
			// Unlock the bits.
			Bitmap.UnlockBits(bmpData);
			locked = false;
		}

		public void Clear(Color colour)
		{
			if (!locked)
				throw new ArgumentException("Bitmap not locked.");

			if (IsAlphaBitmap)
			{
				for (int index = 0; index <= rgbValues.Length - 1; index += 4)
				{
					rgbValues[index] = colour.B;
					rgbValues[index + 1] = colour.G;
					rgbValues[index + 2] = colour.R;
					rgbValues[index + 3] = colour.A;
				}
			}
			else
			{
				for (int index = 0; index <= rgbValues.Length - 1; index += 3)
				{
					rgbValues[index] = colour.B;
					rgbValues[index + 1] = colour.G;
					rgbValues[index + 2] = colour.R;
				}
			}
		}

		public void SetPixel(Point location, Color colour)
		{
			SetPixel(location.X, location.Y, colour);
		}

		public void SetPixel(int x, int y, Color colour)
		{
			if (!locked)
				throw new ArgumentException("Bitmap not locked.");

			if (IsAlphaBitmap)
			{
				int index = ((y * Width + x) * 4);
				rgbValues[index] = colour.B;
				rgbValues[index + 1] = colour.G;
				rgbValues[index + 2] = colour.R;
				rgbValues[index + 3] = colour.A;
			}
			else
			{
				int index = ((y * Width + x) * 3);
				rgbValues[index] = colour.B;
				rgbValues[index + 1] = colour.G;
				rgbValues[index + 2] = colour.R;
			}
		}

		public Color GetPixel(Point location)
		{
			return GetPixel(location.X, location.Y);
		}

		public Color GetPixel(int x, int y)
		{
			if (!locked)
				throw new ArgumentException("Bitmap not locked.");

			if (IsAlphaBitmap)
			{
				int index = ((y * Width + x) * 4);
				int b = rgbValues[index];
				int g = rgbValues[index + 1];
				int r = rgbValues[index + 2];
				int a = rgbValues[index + 3];
				return Color.FromArgb(a, r, g, b);
			}
			else
			{
				int index = ((y * Width + x) * 3);
				int b = rgbValues[index];
				int g = rgbValues[index + 1];
				int r = rgbValues[index + 2];
				return Color.FromArgb(r, g, b);
			}
		}

		#region "IDisposable Support"

		private bool disposedValue;
		// To detect redundant calls

		// IDisposable

		// TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
		//Protected Overrides Sub Finalize()
		//    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
		//    Dispose(False)
		//    MyBase.Finalize()
		//End Sub

		// This code added by Visual Basic to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					Unlock(_autoRelease);
				}
			}
			// TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
			// TODO: set large fields to null.
			disposedValue = true;
		}

		#endregion
	}
}