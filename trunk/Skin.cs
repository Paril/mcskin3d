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
using OpenTK;

namespace MCSkin3D
{
	[Serializable]
	public class Skin : TreeNode, IDisposable
	{
		public Bitmap Image;
		public Bitmap Head;
		public Texture GLImage;
		public UndoBuffer Undo;
		public bool Dirty;
		public Size Size;
		public Model Model { get; set; }

		public int Width { get { return Size.Width; } }
		public int Height { get { return Size.Height; } }

		Dictionary<int, bool> _transparentParts = new Dictionary<int, bool>();

		public Dictionary<int, bool> TransparentParts { get { return _transparentParts; } }

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
					return new DirectoryInfo(Editor.RootFolderString);

				return new DirectoryInfo(((this.Parent != null) ? Editor.GetFolderForNode(this.Parent) : ""));
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
			if (GLImage != null)
			{
				GLImage.Dispose();
				GLImage = null;
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
			try
			{
				if (Head != null)
				{
					Head.Dispose();

					if (updateGL)
					{
						GLImage.Dispose();
						GLImage = null;
					}
				}

				using (var file = File.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
					Image = new Bitmap(file);

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
					GLImage = new TextureGL(File.FullName);
					GLImage.SetMipmapping(false);
					GLImage.SetRepeat(false);
				}

				if (Model == null)
				{
					var metadata = PNGMetadata.ReadMetadata(File.FullName);

					if (metadata.ContainsKey("Model"))
					{
						Model = ModelLoader.GetModelForPath(metadata["Model"]);

						if (Model == null)
							Model = ModelLoader.GetModelForPath("Mobs/Passive/Human");
					}
					else
						Model = ModelLoader.GetModelForPath("Mobs/Passive/Human");

					SetTransparentParts();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error loading skin \"" + File.FullName + "\"", ex);
			}
		}

		public override string ToString()
		{
			if (Dirty)
				return Name + " *";
			return Name;
		}

		public void CommitChanges(Texture currentSkin, bool save)
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

				newBitmap.Save(File.FullName);
				newBitmap.Dispose();

				var md = new Dictionary<string, string>();
				md.Add("Model", Model.Path);
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

				newBitmap.Save(File.FullName);

				var md = new Dictionary<string, string>();
				md.Add("Model", Model.Name);
				PNGMetadata.WriteMetadata(File.FullName, md);
			}

			SetImages();

			Undo.Clear();
			Editor.MainForm.CheckUndo();
		}

		public void Delete()
		{
			File.Delete();
		}

		public void MoveTo(string newPath)
		{
			while (System.IO.File.Exists(newPath))
				newPath = newPath.Insert(newPath.Length - 4, " - Moved");

			File.MoveTo(newPath);
		}

		public void CheckTransparentPart(ColorGrabber grabber, int index)
		{
			foreach (var f in Model.Meshes[index].Faces)
			{
				Bounds bounds = new Bounds(new Point(9999, 9999), new Point(-9999, -9999));

				foreach (var c in f.TexCoords)
				{
					var coord = new Vector2(c.X * Width, c.Y * Height);
					bounds.AddPoint(new Point((int)coord.X, (int)coord.Y));
				}

				var rect = bounds.ToRectangle();
				bool gotOne = false;

				for (int y = rect.Y; !gotOne && y < rect.Y + rect.Height; ++y)
					for (int x = rect.X; x < rect.X + rect.Width; ++x)
					{
						var pixel = grabber[x, y];

						if (pixel.Alpha != 255)
						{
							gotOne = true;
							break;
						}
					}

				if (gotOne)
				{
					TransparentParts[index] = gotOne;
					return;
				}
			}

			TransparentParts[index] = false;
		}

		public void SetTransparentParts()
		{
			ColorGrabber grabber = new ColorGrabber(GLImage, Width, Height);
			grabber.Load();

			int mesh = 0;

			TransparentParts.Clear();

			foreach (var m in Model.Meshes)
			{
				TransparentParts.Add(mesh, false);

				CheckTransparentPart(grabber, mesh);
				mesh++;
			}
		}
	}
}
