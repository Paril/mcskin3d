using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class EraserTool : ITool
	{
		PixelsChangedUndoable _undo;

		public void BeginClick(Skin skin, MouseEventArgs e)
		{
			_undo = new PixelsChangedUndoable();
		}

		public void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		public bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			if (_undo.Points.ContainsKey(new Point(x, y)))
				return false;

			var pixNum = x + (skin.Width * y);
			var c = pixels[pixNum];
			var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);

			_undo.Points.Add(new Point(x, y), Tuple.MakeTuple(oldColor, new ColorAlpha(Color.FromArgb(0), 0)));

			pixels[pixNum] = 0;
			return true;
		}

		public bool RequestPreview(int[] pixels, Skin skin, int x, int y)
		{
			var pixNum = x + (skin.Width * y);
			var c = pixels[pixNum];
			pixels[pixNum] = 0;
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
