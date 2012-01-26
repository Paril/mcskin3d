﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Paril.Drawing;

namespace Paril.OpenGL
{
	public class TextureGL : Texture
	{
		public int TextureID { get; private set; }

		int _width, _height;

		public int Width
		{
			get
			{
				if (TextureID == 0)
					throw new InvalidOperationException();

				return _width;
			}
		}

		public int Height
		{
			get
			{
				if (TextureID == 0)
					throw new InvalidOperationException();
				
				return _height;
			}
		}

		public TextureGL()
		{
			Generate();
		}

		public TextureGL(Bitmap b)
		{
			Load(b);
		}

		public TextureGL(string s)
		{
			Load(s);
		}

		public void Generate()
		{
			TextureID = GL.GenTexture();
		}

		public override void Load(Bitmap image)
		{
			Generate();
			Bind();

			SetMipmapping(false);
			SetRepeat(false);

			int[] goodData = new int[image.Width * image.Height];
			int i = 0;

			using (FastPixel fp = new FastPixel(image, true))
			{
				for (int y = 0; y < image.Height; ++y)
					for (int x = 0; x < image.Width; ++x)
					{
						var argb = fp.GetPixel(x, y);
						goodData[i++] = (argb.R << 0) | (argb.G << 8) | (argb.B << 16) | (argb.A << 24);
					}
			}

			Upload(goodData, image.Width, image.Height);
		}

		public override void Upload<T>(T[] array, int width, int height)
		{
			Bind();

			GL.TexImage2D(TextureTarget.Texture2D,
							0,
							PixelInternalFormat.Rgba,
							width,
							height,
							0,
							PixelFormat.Rgba,
							PixelType.UnsignedByte,
							array);

			var err = GL.GetError();

			if (err != ErrorCode.NoError)
			{
				Dispose();
				throw new Exception(err.ToString());
			}
			else
			{
				_width = width;
				_height = height;
			}
		}

		public override void Get<T>(T[] array)
		{
			Bind();
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
		}

		public override void SetMipmapping(bool enable)
		{
			Bind();

			if (enable)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			}
			else
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			}
		}

		public override void SetRepeat(bool enable)
		{
			Bind();

			if (enable)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			}
			else
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
			}
		}

		public override void Bind()
		{
			BindTextureInternal(TextureID);
		}

		public override void Dispose()
		{
			if (TextureID == 0)
				return;

			DeleteTextureInternal(TextureID);
		}

		static int _curTex = 0;
		static void BindTextureInternal(int texID)
		{
			if (_curTex == texID)
				return;

			GL.BindTexture(TextureTarget.Texture2D, texID);
			_curTex = texID;
		}

		static void DeleteTextureInternal(int GLImage)
		{
			GL.DeleteTexture(GLImage);

			if (_curTex == GLImage)
				BindTextureInternal(0);
		}

		public static void Unbind()
		{
			BindTextureInternal(0);
		}
	}
}