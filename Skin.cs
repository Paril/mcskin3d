using System;
using System.Drawing;
using Paril.Components;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using Paril.Extensions;
using Paril.OpenGL;
using Paril.Drawing;

namespace MCSkin3D
{
	public class Skin : TreeNode, IDisposable
	{
		public new string Name;
		public Bitmap Image;
		public Bitmap Head;
		public int GLImage;
		public UndoBuffer Undo;
		public bool Dirty;
		public Size Size;

		public int Width { get { return Size.Width; } }
		public int Height { get { return Size.Height; } }
		
		public DirectoryInfo Directory
		{
			get
			{
				if (Level == 0)
					return new DirectoryInfo("Skins");

				return new DirectoryInfo("Skins\\" + ((this.Parent != null) ? this.Parent.FullPath : ""));
			}
		}

		public FileInfo File
		{
			get
			{
				return new FileInfo(Directory.FullName + '\\' + Name + ".png");
			}
		}

		public Skin(string fileName)
		{
			Undo = new UndoBuffer(this);
			Name = Path.GetFileNameWithoutExtension(fileName);
			Text = Name;
		}

		public Skin(FileInfo file) :
			this(file.FullName)
		{
		}

		public void Dispose()
		{
			if (GLImage != 0)
			{
				GL.DeleteTexture(GLImage);
				GLImage = 0;
			}

			if (Head != null)
			{
				Head.Dispose();
				Head = null;
			}

			if (Image != null)
			{
				Image.Dispose();
				Image = null;
			}
		}

		public void SetImages()
		{
			if (Head != null)
			{
				Head.Dispose();
				GL.DeleteTexture(GLImage);
			}

			Image = new Bitmap(File.FullName);

			Size = Image.Size;

			float scale = Size.Width / 64.0f;
			int headSize = (int)(8.0f * scale);

			Head = new Bitmap(headSize, headSize);
			using (Graphics g = Graphics.FromImage(Head))
				g.DrawImage(Image, new Rectangle(0, 0, headSize, headSize), new Rectangle(headSize, headSize, headSize, headSize), GraphicsUnit.Pixel);

			Image.Dispose();
			Image = null;
			GLImage = ImageUtilities.LoadImage(File.FullName);
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
			int[] data = new int[Width * Height];
			RenderState.BindTexture(currentSkin);
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

			RenderState.BindTexture(GLImage);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

			if (save)
			{
				Bitmap newBitmap = new Bitmap(Width, Height);

				using (FastPixel fp = new FastPixel(newBitmap, true))
				{
					for (int y = 0; y < Height; ++y)
						for (int x = 0; x < Width; ++x)
						{
							var c = data[x + (y * Width)];
							fp.SetPixel(x, y, System.Drawing.Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF));
						}
				}

				newBitmap.Save(File.FullName);
				newBitmap.Dispose();

				/*uint ilim = IL.ilGenImage();
				IL.ilBindImage(ilim);
				IL.ilLoadDataL(data, (uint)data.Length, (uint)Width, (uint)Height, 1, 4);
				File.Delete();
				IL.ilSave(IL.ImageType.PNG, File.FullName);
				IL.ilDeleteImage(ilim);*/

				SetImages();

				Dirty = false;
			}
		}

		public bool ChangeName(string newName)
		{
			if (Directory.GetFiles(newName + ".png", SearchOption.TopDirectoryOnly).Length != 0)
				return false;

			File.CopyToParent(newName + ".png");
			File.Delete();
			Name = newName;

			return true;
		}
	}
}
