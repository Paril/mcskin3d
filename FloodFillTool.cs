using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class FloodFillTool : ITool
	{
		PixelsChangedUndoable _undo;
		private bool[] hitPixels;

		public void BeginClick(Skin skin, MouseEventArgs e)
		{
			_undo = new PixelsChangedUndoable();
		}

		public void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		private void recursiveFill(int x, int y, Color oldColor, Color newColor, int[] pixels, bool[] hitPixels, Skin skin)
		{
			if (x < 0 || y < 0 || x >= skin.Width || y >= skin.Height)
				return;

			int i = x + (y * skin.Width);
			if (pixels[i] != (oldColor.R | (oldColor.G << 8) | (oldColor.B << 16) | (oldColor.A << 24)))
				return;
			if (hitPixels[i])
				return;

			if (!_undo.Points.ContainsKey(new Point(x, y)))
				_undo.Points.Add(new Point(x, y), Tuple.MakeTuple(oldColor, newColor));

			pixels[i] = newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24);
			hitPixels[i] = true;

			recursiveFill(x, y - 1, oldColor, newColor, pixels, hitPixels, skin);
			//recursiveFill(x + 1, y - 1, oldColor, newColor, pixels, hitPixels, skin);
			recursiveFill(x + 1, y, oldColor, newColor, pixels, hitPixels, skin);
			//recursiveFill(x + 1, y + 1, oldColor, newColor, pixels, hitPixels, skin);
			recursiveFill(x, y + 1, oldColor, newColor, pixels, hitPixels, skin);
			//recursiveFill(x - 1, y + 1, oldColor, newColor, pixels, hitPixels, skin);
			recursiveFill(x - 1, y, oldColor, newColor, pixels, hitPixels, skin);
			//recursiveFill(x - 1, y - 1, oldColor, newColor, pixels, hitPixels, skin);
		}

		public bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			hitPixels = new bool[skin.Width * skin.Height];
			var pixNum = x + (skin.Width * y);
			var c = pixels[pixNum];
			var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
			var newColor = Program.MainForm.SelectedColor;

			recursiveFill(x, y, oldColor, newColor, pixels, hitPixels, skin);
			return true;
		}

		public bool RequestPreview(int[] pixels, Skin skin, int x, int y)
		{
			var pixNum = x + (skin.Width * y);
			var c = pixels[pixNum];
			var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
			var newColor = Color.FromArgb(ColorBlending.AlphaBlend(((Control.ModifierKeys & Keys.Shift) != 0) ? Program.MainForm.UnselectedColor : Program.MainForm.SelectedColor, oldColor).ToArgb());
			pixels[pixNum] = newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24);
			return true;
		}

		public void EndClick(Skin skin, MouseEventArgs e)
		{
			skin.Undo.AddBuffer(_undo);
			_undo = null;

			Program.MainForm.CheckUndo();
		}
	}
}
