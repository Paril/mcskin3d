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
			Head = new Bitmap(8, 8);

			using (Graphics g = Graphics.FromImage(Head))
				g.DrawImage(Image, new Rectangle(0, 0, 8, 8), new Rectangle(8, 8, 8, 8), GraphicsUnit.Pixel);

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
			byte[] data = new byte[64 * 32 * 4];
			GL.BindTexture(TextureTarget.Texture2D, currentSkin);
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

			GL.BindTexture(TextureTarget.Texture2D, GLImage);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

			if (save)
			{
				uint ilim = IL.ilGenImage();
				IL.ilBindImage(ilim);
				IL.ilLoadDataL(data, (uint)data.Length, 64, 32, 1, 4);
				File.Delete(FileName);
				IL.ilSave(IL.ImageType.PNG, FileName);

				SetImages();

				IL.ilDeleteImage(ilim);
				Dirty = false;
			}
		}
	}
}
