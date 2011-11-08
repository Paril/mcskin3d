using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Paril.Drawing.Filters
{
	public abstract class MatrixFilter
	{
		public abstract NMatrix Matrix { get; }

		public bool Apply(Bitmap b)
		{
			// Avoid divide by zero errors
			if (Matrix.Factor == 0)
				return false;

			Bitmap bSrc = (Bitmap)b.Clone();

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			int stride2 = stride * 2;
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcScan0 = bmSrc.Scan0;

			unsafe
			{
				byte * p = (byte*)(void*)Scan0;
				byte * pSrc = (byte*)(void*)SrcScan0;

				int nOffset = stride - b.Width * 3;
				int nWidth = b.Width - 2;
				int nHeight = b.Height - 2;

				int nPixel;

				for (int y=0; y < nHeight; ++y)
				{
					for (int x=0; x < nWidth; ++x)
					{
						nPixel = (int)((((pSrc[2] * Matrix[0, 0]) + (pSrc[5] * Matrix[0, 1]) + (pSrc[8] * Matrix[0, 2]) +
							(pSrc[2 + stride] * Matrix[1, 0]) + (pSrc[5 + stride] * Matrix[1, 1]) + (pSrc[8 + stride] * Matrix[1, 2]) +
							(pSrc[2 + stride2] * Matrix[2, 0]) + (pSrc[5 + stride2] * Matrix[2, 1]) + (pSrc[8 + stride2] * Matrix[2, 2])) / Matrix.Factor) + Matrix.Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[5 + stride] = (byte)nPixel;

						nPixel = (int)((((pSrc[1] * Matrix[0, 0]) + (pSrc[4] * Matrix[0, 1]) + (pSrc[7] * Matrix[0, 2]) +
							(pSrc[1 + stride] * Matrix[1, 0]) + (pSrc[4 + stride] * Matrix[1, 1]) + (pSrc[7 + stride] * Matrix[1, 2]) +
							(pSrc[1 + stride2] * Matrix[2, 0]) + (pSrc[4 + stride2] * Matrix[2, 1]) + (pSrc[7 + stride2] * Matrix[2, 2])) / Matrix.Factor) + Matrix.Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[4 + stride] = (byte)nPixel;

						nPixel = (int)((((pSrc[0] * Matrix[0, 0]) + (pSrc[3] * Matrix[0, 1]) + (pSrc[6] * Matrix[0, 2]) +
							(pSrc[0 + stride] * Matrix[1, 0]) + (pSrc[3 + stride] * Matrix[1, 1]) + (pSrc[6 + stride] * Matrix[1, 2]) +
							(pSrc[0 + stride2] * Matrix[2, 0]) + (pSrc[3 + stride2] * Matrix[2, 1]) + (pSrc[6 + stride2] * Matrix[2, 2])) / Matrix.Factor) + Matrix.Offset);

						if (nPixel < 0) nPixel = 0;
						if (nPixel > 255) nPixel = 255;

						p[3 + stride] = (byte)nPixel;

						p += 3;
						pSrc += 3;
					}
					p += nOffset;
					pSrc += nOffset;
				}
			}

			b.UnlockBits(bmData);
			bSrc.UnlockBits(bmSrc);

			bSrc.Dispose();

			return true;

		}
	}
}
