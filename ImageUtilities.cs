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

			/*
			uint imageID = IL.ilGenImage(); 		// Generate the image ID

			IL.ilBindImage(imageID); 			// Bind the image

			bool success = IL.ilLoadImage(fileName); 	// Load the image file

			// If we managed to load the image, then we can start to do things with it...
			if (success)
			{
				// Convert the image into a suitable format to work with
				// NOTE: If your image contains alpha channel you can replace IL_RGB with IL_RGBA
				success = IL.ilConvertImage(IL.ColorFormats.RGBA, IL.Types.UnsignedByte);

				// Quit out if we failed the conversion
				if (!success)
					throw new Exception(IL.ilGetError().ToString());

				// Generate a new texture
				int textureID = GL.GenTexture();

				// Bind the texture to a name
				RenderState.BindTexture(textureID);

				// Set texture interpolation method to use linear interpolation (no MIPMAPS)
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

				// Specify the texture specification
				GL.TexImage2D(TextureTarget.Texture2D, 				// Type of texture
							 0,				// Pyramid level (for mip-mapping) - 0 is the top level
							 (PixelInternalFormat)IL.ilGetInteger(IL.Parameters.ImageBPP),	// Image colour depth
							 IL.ilGetInteger(IL.Parameters.ImageWidth),	// Image width
							 IL.ilGetInteger(IL.Parameters.ImageHeight),	// Image height
							 0,				// Border width in pixels (can either be 1 or 0)
							 (PixelFormat)IL.ilGetInteger(IL.Parameters.ImageFormat),	// Image format (i.e. RGB, RGBA, BGR etc.)
							 PixelType.UnsignedByte,		// Image data type
							 IL.ilGetData());			// The actual image data itself

				IL.ilDeleteImage(imageID); // Because we have already copied image data into texture data we can release memory used by image.

				return textureID; // Return the GLuint to the texture so you can use it!
			}

			// If we failed to open the image file in the first place...
			throw new Exception(IL.ilGetError().ToString());*/

			Bitmap b = new Bitmap(fileName);
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

			b.Dispose();

			if (GL.GetError() != ErrorCode.NoError)
			{
				GL.DeleteTexture(glImage);
				throw new Exception();
			}

			return glImage;
		}
	}
}
