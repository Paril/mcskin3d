using System;
using System.Drawing;
using Paril.Components;
using System.IO;
using OpenTK.Graphics.OpenGL;
using DevCIL;

namespace MCSkin3D
{
	public class Skin
	{
		public string Name;
		public Bitmap Image;
		public Bitmap Head;
		public int GLImage;
		public string FileName;
		public UndoBuffer Undo;
		public bool Dirty;
		public Size Size;

		public int Width { get { return Size.Width; } }
		public int Height { get { return Size.Height; } } 

		public Skin(string fileName)
		{
			Undo = new UndoBuffer(this);
			FileName = fileName;
			Name = Path.GetFileNameWithoutExtension(FileName);

			SetImages();
		}

		void SetImages()
		{
			if (Head != null)
			{
				Head.Dispose();
				GL.DeleteTexture(GLImage);
			}

			Image = new Bitmap(FileName);

			Size = Image.Size;

			float scale = Size.Width / 64.0f;
			int headSize = (int)(8.0f * scale);

			Head = new Bitmap(headSize, headSize);
			using (Graphics g = Graphics.FromImage(Head))
				g.DrawImage(Image, new Rectangle(0, 0, headSize, headSize), new Rectangle(headSize, headSize, headSize, headSize), GraphicsUnit.Pixel);

			Image.Dispose();
			Image = null;
			GLImage = ImageUtilities.LoadImage(FileName);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
		}

		public override string ToString()
		{
			if (Dirty)
				return Name + " *";
			return Name;
		}

		public void CommitChanges(int currentSkin, bool save)
		{
			byte[] data = new byte[Width * Height * 4];
			GL.BindTexture(TextureTarget.Texture2D, currentSkin);
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

			GL.BindTexture(TextureTarget.Texture2D, GLImage);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

			if (save)
			{
				uint ilim = IL.ilGenImage();
				IL.ilBindImage(ilim);
				IL.ilLoadDataL(data, (uint)data.Length, (uint)Width, (uint)Height, 1, 4);
				File.Delete(FileName);
				IL.ilSave(IL.ImageType.PNG, FileName);

				SetImages();

				IL.ilDeleteImage(ilim);
				Dirty = false;
			}
		}
	}
}
