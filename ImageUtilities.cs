using System;
using OpenTK.Graphics.OpenGL;
using Paril.OpenGL;
using System.Drawing;
using Paril.Drawing;

namespace MCSkin3D
{
	public static class ImageUtilities
	{
		// Function load a image, turn it into a texture, and return the texture ID as a GLuint for use
		public static int LoadImage(string fileName)
		{
			if (!System.IO.File.Exists(fileName))
				throw new System.IO.FileNotFoundException("Not found!", fileName);

			using (Bitmap b = new Bitmap(fileName))
				return LoadImage(b);
		}

		public static int LoadImage(Bitmap b)
		{
			int glImage = GL.GenTexture();

			RenderState.BindTexture(glImage);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

			int[] goodData = new int[b.Width * b.Height];
			int i = 0;

			using (FastPixel fp = new FastPixel(b, true))
			{
				for (int y = 0; y < b.Height; ++y)
					for (int x = 0; x < b.Width; ++x)
					{
						var argb = fp.GetPixel(x, y);
						goodData[i++] = (argb.R << 0) | (argb.G << 8) | (argb.B << 16) | (argb.A << 24);
					}
			}

			GL.TexImage2D(TextureTarget.Texture2D, 				// Type of texture
							0,				// Pyramid level (for mip-mapping) - 0 is the top level
							PixelInternalFormat.Rgba,	// Image colour depth
							b.Width,	// Image width
							b.Height,	// Image height
							0,				// Border width in pixels (can either be 1 or 0)
							PixelFormat.Rgba,	// Image format (i.e. RGB, RGBA, BGR etc.)
							PixelType.UnsignedByte,		// Image data type
							goodData);			// The actual image data itself

			if (GL.GetError() != ErrorCode.NoError)
			{
				GL.DeleteTexture(glImage);
				throw new Exception();
			}

			return glImage;
		}
	}
}
