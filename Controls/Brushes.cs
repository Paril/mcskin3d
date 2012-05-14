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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Paril.Controls;
using Paril.Drawing;

namespace MCSkin3D
{
	public class Brush : IComparable<Brush>
	{
		private static readonly AlphanumComparatorFast logical = new AlphanumComparatorFast();
		public float[,] Luminance;

		public Brush(string name, int w, int h)
		{
			Name = name;
			Luminance = new float[w,h];
		}

		public Brush(string file)
		{
			Name = Path.GetFileNameWithoutExtension(file);

			Image = new Bitmap(file);
			Luminance = new float[Image.Width,Image.Height];

			using (var fp = new FastPixel(Image, true))
			{
				for (int y = 0; y < Height; ++y)
				{
					for (int x = 0; x < Width; ++x)
						Luminance[x, y] = fp.GetPixel(x, y).A / 255.0f;
				}
			}
		}

		public string Name { get; set; }

		public int Width
		{
			get { return Luminance.GetLength(0); }
		}

		public int Height
		{
			get { return Luminance.GetLength(1); }
		}

		public Bitmap Image { get; private set; }

		public float this[int x, int y]
		{
			get { return Luminance[x, y]; }
			set { Luminance[x, y] = value; }
		}

		#region IComparable<Brush> Members

		public int CompareTo(Brush b)
		{
			return logical.Compare(Name, b.Name);
		}

		#endregion

		public void BuildImage(bool save = true)
		{
			if (Image != null)
				Image.Dispose();

			Image = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

			using (var fp = new FastPixel(Image, true))
			{
				for (int y = 0; y < Height; ++y)
				{
					for (int x = 0; x < Width; ++x)
						fp.SetPixel(x, y, Color.FromArgb((byte) (Luminance[x, y] * 255), 0, 0, 0));
				}
			}

			if (save)
				Image.Save("Brushes\\" + Editor.GetLanguageString(Name) + " [" + Width + "].png");
		}
	}

	public static class Brushes
	{
		public static int NumBrushes = 10;
		public static List<Brush> BrushList = new List<Brush>();
		public static BrushComboBox BrushBox = new BrushComboBox();

		public static Brush SelectedBrush { get; private set; }

		private static void BrushBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedBrush = (Brush) BrushBox.SelectedItem;

			if (Editor.MainForm.SelectedTool != null)
				Editor.MainForm.SelectedTool.Tool.SelectedBrushChanged();
		}

		public static void LoadBrushes()
		{
			foreach (string file in Directory.GetFiles(GlobalSettings.GetDataURI("Brushes"), "*.png", SearchOption.AllDirectories))
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

			var onebyone = new Brush("Pixel", 1, 1);
			onebyone.Luminance = new float[,] {{1}};
			onebyone.BuildImage(false);

			BrushList.Insert(0, onebyone);

			foreach (Brush b in BrushList)
				BrushBox.Items.Add(b);

			BrushBox.SelectedIndexChanged += BrushBox_SelectedIndexChanged;
			BrushBox.DropDownStyle = ComboBoxStyle.DropDownList;
			BrushBox.SelectedIndex = 0;
			BrushBox.Width = 44;
		}
	}

	public class BrushComboBox : ComboBox
	{
		public BrushComboBox()
		{
			DrawMode = DrawMode.OwnerDrawFixed;
			ItemHeight = 20;
			DropDownWidth = 35;
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem(e);

			e.DrawBackground();

			if (e.Index != -1)
			{
				var brush = (Brush) Items[e.Index];

				if (brush.Width <= e.Bounds.Height)
				{
					e.Graphics.DrawImage(brush.Image, e.Bounds.X + (e.Bounds.Height / 2) - (brush.Width / 2),
					                     e.Bounds.Y + (e.Bounds.Height / 2) - (brush.Height / 2), brush.Width, brush.Height);
				}
				else
					e.Graphics.DrawImage(brush.Image, e.Bounds.X, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height);

				//e.Graphics.DrawRectangle(Pens.Black, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height));

				TextRenderer.DrawText(e.Graphics, brush.Name, Font,
				                      new Rectangle(e.Bounds.X + e.Bounds.Height + 4, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height),
				                      (e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.WindowText,
				                      TextFormatFlags.VerticalCenter);
			}

			e.DrawFocusRectangle();
		}
	}
}