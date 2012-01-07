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
using Paril.Components;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using Paril.Extensions;
using Paril.OpenGL;
using Paril.Drawing;
using System.Drawing.Drawing2D;
using Paril.Imaging;
using System.Collections.Generic;

namespace MCSkin3D
{
	[Serializable]
	public class Skin : TreeNode, IDisposable
	{
		public Bitmap Image;
		public Bitmap Head;
		public int GLImage;
		public UndoBuffer Undo;
		public bool Dirty;
		public Size Size;
		public Model Model { get; set; }

		public int Width { get { return Size.Width; } }
		public int Height { get { return Size.Height; } }

		public new string Name
		{
			get { return base.Name; }
			set { base.Name = value; base.Text = base.Name = value; }
		}

        public static Image getHeadFromFile(String str, Size s)
        {
            Image img = new Bitmap(str);

            float scale = img.Size.Width / 64.0f;
            int headSize = (int)(8.0f * scale);

            Image head = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(head))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.DrawImage(img, new Rectangle(0, 0, head.Width, head.Height), new Rectangle(headSize, headSize, headSize, headSize), GraphicsUnit.Pixel);
            }

            img.Dispose();
            img = null;
            return head;
        }

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
		}

		public Skin(FileInfo file) :
			this(file.FullName)
		{
		}

		public void Dispose()
		{
			if (GLImage != 0)
			{
				RenderState.DeleteTexture(GLImage);
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

		public void SetImages(bool updateGL = true)
		{
			if (Head != null)
			{
				Head.Dispose();

				if (updateGL)
					RenderState.DeleteTexture(GLImage);
			}

			using (var file = File.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
				Image = new Bitmap(file);

			var metadata = PNGMetadata.ReadMetadata(File.FullName);

			if (metadata.ContainsKey("Model"))
			{
				Model model;

				if (!ModelLoader.Models.TryGetValue(metadata["Model"], out model))
					Model = ModelLoader.Models["Human"];
				else
					Model = model;
			}
			else
				Model = ModelLoader.Models["Human"];

			Size = Image.Size;

			float scale = Size.Width / 64.0f;
			int headSize = (int)(8.0f * scale);
			int helmetLoc = (int)(40.0f * scale);

			Head = new Bitmap(headSize, headSize);
			using (Graphics g = Graphics.FromImage(Head))
			{
				g.DrawImage(Image, new Rectangle(0, 0, headSize, headSize), new Rectangle(headSize, headSize, headSize, headSize), GraphicsUnit.Pixel);
				g.DrawImage(Image, new Rectangle(0, 0, headSize, headSize), new Rectangle(helmetLoc, headSize, headSize, headSize), GraphicsUnit.Pixel);
			}

			Image.Dispose();
			Image = null;

			if (updateGL)
			{
				GLImage = ImageUtilities.LoadImage(File.FullName);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
			}
		}

		public override string ToString()
		{
			if (Dirty)
				return Name + " *";
			return Name;
		}

		public void CommitChanges(int currentSkin, bool save)
		{
			ColorGrabber grabber = new ColorGrabber(currentSkin, Width, Height);
			grabber.Load();

			if (currentSkin != GLImage)
			{
				grabber.Texture = GLImage;
				grabber.Save();
			}

			if (save)
			{
				Bitmap newBitmap = new Bitmap(Width, Height);

				using (FastPixel fp = new FastPixel(newBitmap, true))
				{
					for (int y = 0; y < Height; ++y)
						for (int x = 0; x < Width; ++x)
						{
							var c = grabber[x, y];
							fp.SetPixel(x, y, System.Drawing.Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue));
						}
				}

				Editor.AddIgnoreFile(File.FullName);
				newBitmap.Save(File.FullName);
				newBitmap.Dispose();

				var md = new Dictionary<string, string>();
				md.Add("Model", Model.Name);
				PNGMetadata.WriteMetadata(File.FullName, md);

				SetImages(true);

				Dirty = false;
			}
		}

		public bool ChangeName(string newName, bool force = false)
		{
			if (!newName.EndsWith(".png"))
				newName += ".png";

			if (force)
			{
				Name = Path.GetFileNameWithoutExtension(newName);
				return true;
			}

			if (System.IO.File.Exists(Directory.FullName + "\\" + newName))
				return false;

			Editor.AddIgnoreFile(Directory.FullName + "\\" + newName);
			File.MoveToParent(newName);
			Name = Path.GetFileNameWithoutExtension(newName);

			return true;
		}

		public void Resize(int width, int height)
		{
			using (var newBitmap = new Bitmap(width, height))
			{
				using (Graphics g = Graphics.FromImage(newBitmap))
				{
					g.SmoothingMode = SmoothingMode.None;
					g.InterpolationMode = InterpolationMode.NearestNeighbor;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;

					using (var temp = Bitmap.FromFile(File.FullName))
						g.DrawImage(temp, 0, 0, newBitmap.Width, newBitmap.Height);
				}

				Editor.AddIgnoreFile(File.FullName);
				newBitmap.Save(File.FullName);
			}

			SetImages();

			Undo.Clear();
			Editor.MainForm.CheckUndo();
		}

		public void Delete()
		{
			Editor.AddIgnoreFile(File.FullName);
			File.Delete();
		}
	}
}
