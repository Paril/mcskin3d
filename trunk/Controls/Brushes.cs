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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
using System.Drawing.Drawing2D;
using Paril.Drawing.Filters;
using System.Drawing.Imaging;
using Paril.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

namespace MCSkin3D
{
	public class Brush : IComparable<Brush>
	{
		public string Name { get; set; }
		public float[,] Luminance;

		public int Width { get { return Luminance.GetLength(0); } }
		public int Height { get { return Luminance.GetLength(1); } }
		public Bitmap Image { get; private set; }

		public float this[int x, int y]
		{
			get { return Luminance[x, y]; }
			set { Luminance[x, y] = value; }
		}

		public Brush(string name, int w, int h)
		{
			Name = name;
			Luminance = new float[w, h];
		}

		public Brush(string file)
		{
			Name = Path.GetFileNameWithoutExtension(file);

			Image = new Bitmap(file);
			Luminance = new float[Image.Width, Image.Height];

			using (FastPixel fp = new FastPixel(Image, true))
			{
				for (int y = 0; y < Height; ++y)
					for (int x = 0; x < Width; ++x)
						Luminance[x, y] = (float)fp.GetPixel(x, y).A / 255.0f;
			}
		}

		public void BuildImage()
		{
			if (Image != null)
				Image.Dispose();

			Image = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

			using (FastPixel fp = new FastPixel(Image, true))
			{
				for (int y = 0; y < Height; ++y)
					for (int x = 0; x < Width; ++x)
						fp.SetPixel(x, y, System.Drawing.Color.FromArgb((byte)(Luminance[x, y] * 255), 0, 0, 0));
			}

			Image.Save("Brushes\\" + Editor.GetLanguageString(Name) + " [" + Width + "].png");
		}

		static Paril.Controls.AlphanumComparatorFast logical = new Paril.Controls.AlphanumComparatorFast();
		public int CompareTo(Brush b)
		{
			return logical.Compare(Name, b.Name);
		}
	}

	public static class Brushes
	{
		public static int NumBrushes = 10;
		public static List<Brush> BrushList = new List<Brush>();
		public static BrushComboBox BrushBox = new BrushComboBox();

		public static Brush SelectedBrush
		{
			get { return (Brush)BrushBox.SelectedItem; }
		}

		static Brush GenerateSquare(int size)
		{
			Brush brush = new Brush("C_SQUARE", size, size);
			Editor.MainForm.languageProvider1.SetPropertyNames(brush, "Name");

			for (int y = 0; y < brush.Height; ++y)
				for (int x = 0; x < brush.Width; ++x)
					brush.Luminance[x, y] = 1;

			return brush;
		}

		static Brush GenerateCircle(int size)
		{
			Brush brush = new Brush("C_CIRCLE", size, size);
			Editor.MainForm.languageProvider1.SetPropertyNames(brush, "Name");
			int radius = (int)Math.Floor(size / 2.0);

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (Math.Pow((i - radius), 2) + Math.Pow((j - radius), 2) <= Math.Pow(radius, 2))
						brush.Luminance[i, j] = 1;
				}
			}

			return brush;
		}

		static Brush GenerateFeatheredSquare(int size)
		{
			Brush brush = new Brush("Smooth Square [" + size + "]", size + 4, size + 4);

			for (int y = 2; y < brush.Height - 2; ++y)
				for (int x = 2; x < brush.Width - 2; ++x)
				{
					brush.Luminance[x, y] = 1;
				}

			GaussianBlurFilter filter = new GaussianBlurFilter(4);
			var op = new FloatMatrixOperand();
			op.Value = brush.Luminance;
			filter.Apply(op);

			float scale = brush.Luminance[(brush.Width - 1) / 2, (brush.Height - 1) / 2];
			
			for (int y = 0; y < brush.Height; ++y)
				for (int x = 0; x < brush.Width; ++x)
				{
					if (brush.Luminance[x, y] != 0)
						brush.Luminance[x, y] /= scale;
				}

			return brush;
		}

		static Brush GenerateSmoothCircle(int size)
		{
			Brush brush = new Brush("Smooth Circle [" + size + "]", size + 4, size + 4);
			int radius = (int)Math.Floor((size) / 2.0);

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (Math.Pow((i - radius), 2) + Math.Pow((j - radius), 2) <= Math.Pow(radius, 2))
						brush.Luminance[i + 2, j + 2] = 1;
				}
			}

			GaussianBlurFilter filter = new GaussianBlurFilter(4);
			var op = new FloatMatrixOperand();
			op.Value = brush.Luminance;
			filter.Apply(op);

			float scale = brush.Luminance[(brush.Width - 1) / 2, (brush.Height - 1) / 2];

			for (int y = 0; y < brush.Height; ++y)
				for (int x = 0; x < brush.Width; ++x)
				{
					if (brush.Luminance[x, y] != 0)
						brush.Luminance[x, y] /= scale;
				}

			return brush;
		}

		public static void LoadBrushes()
		{
			foreach (var file in Directory.GetFiles("Brushes", "*.png", SearchOption.AllDirectories))
			{
				try
				{
					BrushList.Add(new Brush(file));
				}
				catch
				{
				}
			}

			BrushList.Sort();

			foreach (var b in BrushList)
				BrushBox.Items.Add(b);

			BrushBox.DropDownStyle = ComboBoxStyle.DropDownList;
			BrushBox.SelectedIndex = 0;
			BrushBox.Width = 44;
		}
	}

	public class BrushComboBox : ComboBox
	{
		public BrushComboBox()
		{
			DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			ItemHeight = 20;
			DropDownWidth = 35;
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem(e);

			e.DrawBackground();

			if (e.Index != -1)
			{
				var brush = (Brush)Items[e.Index];

				if (brush.Width <= e.Bounds.Height)
					e.Graphics.DrawImage(brush.Image, e.Bounds.X + (e.Bounds.Height / 2) - (brush.Width / 2), e.Bounds.Y + (e.Bounds.Height / 2) - (brush.Height / 2), brush.Width, brush.Height);
				else
					e.Graphics.DrawImage(brush.Image, e.Bounds.X, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height);

				//e.Graphics.DrawRectangle(Pens.Black, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height));

				TextRenderer.DrawText(e.Graphics, brush.Name, Font, new Rectangle(e.Bounds.X + e.Bounds.Height + 4, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), (e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.WindowText, TextFormatFlags.VerticalCenter);
			}

			e.DrawFocusRectangle();
		}
	}
}
