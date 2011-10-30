using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Paril.Drawing
{
    public class FastPixel : IDisposable
    {
        private byte[] rgbValues = null;
        private BitmapData bmpData;
        private IntPtr bmpPtr;
        private bool locked = false;

        private bool _isAlpha = false;
        private Bitmap _bitmap;
        private int _width;
        private int _height;

        private bool _autoRelease = false;

        public int Width
        {
            get { return this._width; }
        }
        public int Height
        {
            get { return this._height; }
        }
        public bool IsAlphaBitmap
        {
            get { return this._isAlpha; }
        }
        public Bitmap Bitmap
        {
            get { return this._bitmap; }
        }

        public FastPixel(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentException("FastPixel with no bitmap");
            if ((bitmap.PixelFormat == (bitmap.PixelFormat | PixelFormat.Indexed)))
                throw new ArgumentException("Cannot lock an Indexed image.");
            this._bitmap = bitmap;
            this._isAlpha = (this.Bitmap.PixelFormat == (this.Bitmap.PixelFormat | PixelFormat.Alpha));
            this._width = bitmap.Width;
            this._height = bitmap.Height;
            _autoRelease = false;
        }

        public FastPixel(Bitmap bitmap, bool autoRelease)
        {
            if (bitmap == null)
                throw new ArgumentException("FastPixel with no bitmap");
            if ((bitmap.PixelFormat == (bitmap.PixelFormat | PixelFormat.Indexed)))
                throw new ArgumentException("Cannot lock an Indexed image.");
            this._bitmap = bitmap;
            this._isAlpha = (this.Bitmap.PixelFormat == (this.Bitmap.PixelFormat | PixelFormat.Alpha));
            this._width = bitmap.Width;
            this._height = bitmap.Height;
            this._autoRelease = autoRelease;

            Lock();
        }

        public void Lock()
        {
            if (this.locked)
                throw new ArgumentException("Bitmap already locked.");

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            this.bmpData = this.Bitmap.LockBits(rect, ImageLockMode.ReadWrite, this.Bitmap.PixelFormat);
            this.bmpPtr = this.bmpData.Scan0;

            if (this.IsAlphaBitmap)
            {
                int bytes = (this.Width * this.Height) * 4;
                rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(this.bmpPtr, rgbValues, 0, this.rgbValues.Length);
            }
            else
            {
                int bytes = (this.Width * this.Height) * 3;
                rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(this.bmpPtr, rgbValues, 0, this.rgbValues.Length);
            }

            this.locked = true;
        }
        public void Unlock(bool setPixels)
        {
            if (!this.locked)
                throw new ArgumentException("Bitmap not locked.");
            // Copy the RGB values back to the bitmap
            if (setPixels) System.Runtime.InteropServices.Marshal.Copy(this.rgbValues, 0, this.bmpPtr, this.rgbValues.Length);
            // Unlock the bits.
            this.Bitmap.UnlockBits(bmpData);
            this.locked = false;
        }

        public void Clear(System.Drawing.Color colour)
        {
            if (!this.locked)
                throw new ArgumentException("Bitmap not locked.");

            if (this.IsAlphaBitmap)
            {
                for (int index = 0; index <= this.rgbValues.Length - 1; index += 4)
                {
                    this.rgbValues[index] = colour.B;
                    this.rgbValues[index + 1] = colour.G;
                    this.rgbValues[index + 2] = colour.R;
                    this.rgbValues[index + 3] = colour.A;
                }
            }
            else
            {
                for (int index = 0; index <= this.rgbValues.Length - 1; index += 3)
                {
                    this.rgbValues[index] = colour.B;
                    this.rgbValues[index + 1] = colour.G;
                    this.rgbValues[index + 2] = colour.R;
                }
            }
        }
		public void SetPixel(Point location, System.Drawing.Color colour)
        {
            this.SetPixel(location.X, location.Y, colour);
        }
		public void SetPixel(int x, int y, System.Drawing.Color colour)
        {
            if (!this.locked)
                throw new ArgumentException("Bitmap not locked.");

            if (this.IsAlphaBitmap)
            {
                int index = ((y * this.Width + x) * 4);
                this.rgbValues[index] = colour.B;
                this.rgbValues[index + 1] = colour.G;
                this.rgbValues[index + 2] = colour.R;
                this.rgbValues[index + 3] = colour.A;
            }
            else
            {
                int index = ((y * this.Width + x) * 3);
                this.rgbValues[index] = colour.B;
                this.rgbValues[index + 1] = colour.G;
                this.rgbValues[index + 2] = colour.R;
            }
        }
		public System.Drawing.Color GetPixel(Point location)
        {
            return this.GetPixel(location.X, location.Y);
        }
		public System.Drawing.Color GetPixel(int x, int y)
        {
            if (!this.locked)
                throw new ArgumentException("Bitmap not locked.");

            if (this.IsAlphaBitmap)
            {
                int index = ((y * this.Width + x) * 4);
                int b = this.rgbValues[index];
                int g = this.rgbValues[index + 1];
                int r = this.rgbValues[index + 2];
                int a = this.rgbValues[index + 3];
				return System.Drawing.Color.FromArgb(a, r, g, b);
            }
            else
            {
                int index = ((y * this.Width + x) * 3);
                int b = this.rgbValues[index];
                int g = this.rgbValues[index + 1];
                int r = this.rgbValues[index + 2];
				return System.Drawing.Color.FromArgb(r, g, b);
            }
        }

        #region "IDisposable Support"
        private bool disposedValue;
        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Unlock(_autoRelease);

                }
            }
            // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            // TODO: set large fields to null.
            this.disposedValue = true;
        }

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
        #endregion

    }
}
