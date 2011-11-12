using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;
using OpenTK;

namespace MCSkin3D
{
    //Added threshold [Xylem] 09/11/2011
	public class FloodFillTool : ITool
	{
        public float Threshold //[0-1]
        {
            get { return GlobalSettings.FloodFillThreshold; }
        }

		PixelsChangedUndoable _undo;
		private bool[] hitPixels;
		Rectangle _boundBox;
		bool _done = false;

		public void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_undo = new PixelsChangedUndoable();
			_boundBox = new Rectangle(0, 0, skin.Width, skin.Height);

			if ((Control.ModifierKeys & Keys.Control) != 0)
				_boundBox = PlayerModel.HumanModel.GetTextureFaceBounds(new Point(p.X, p.Y), skin);

			_done = false;
		}

		public void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		static bool similarColor(Color color1, Color color2, byte threshold)
		{
			return
					Math.Abs((int)color1.R - (int)color2.R) <= threshold &&
					Math.Abs((int)color1.G - (int)color2.G) <= threshold &&
					Math.Abs((int)color1.B - (int)color2.B) <= threshold &&
					Math.Abs((int)color1.A - (int)color2.A) <= threshold
					;
		}
		//Same as similarColor, but avoids some calculations if it can; use this if threshold may be 255 or 0
		static bool similarColor2(Color color1, Color color2, byte threshold)
		{
			if (threshold == 255)
				return true;
			else if (threshold == 0)
			{
				return
						color1.R == color2.R &&
						color1.G == color2.G &&
						color1.B == color2.B &&
						color1.A == color2.A
						;
			}
			else
				return similarColor(color1, color2, threshold);
		}

		byte _threshold;
		private void recursiveFill(int x, int y, Color oldColor, Color newColor, int[] pixels, bool[] hitPixels, Skin skin)
		{
			if (!_boundBox.Contains(x, y))
				return;

			int i = x + (y * skin.Width);
			if (hitPixels[i])
				return;

			var c = pixels[i];
			var real = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);

			if (!similarColor2(oldColor, real, _threshold))
				return;

			if (!_undo.Points.ContainsKey(new Point(x, y)))
				_undo.Points.Add(new Point(x, y), Tuple.MakeTuple(real, new ColorAlpha(newColor, 0)));

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
			if (_done)
				return false;

			var curve = new BezierCurveQuadric(new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 2));
			_threshold = (byte)((1 - (curve.CalculatePoint(Threshold)).X) * 255);//(byte)((1 - Math.Sin((1 - Threshold) * (Math.PI / 2))) * 255);

			hitPixels = new bool[skin.Width * skin.Height];
			var pixNum = x + (skin.Width * y);
			var c = pixels[pixNum];
			var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
			var newColor = ((Control.ModifierKeys & Keys.Shift) != 0) ? Program.MainForm.UnselectedColor : Program.MainForm.SelectedColor;

            recursiveFill(x, y, oldColor, newColor, pixels, hitPixels, skin);
			_done = true;
			return true;
		}

		public bool RequestPreview(int[] pixels, Skin skin, int x, int y)
		{
			var pixNum = x + (skin.Width * y);
			var newColor = ((Control.ModifierKeys & Keys.Shift) != 0) ? Program.MainForm.UnselectedColor : Program.MainForm.SelectedColor;
			pixels[pixNum] = newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24);
			return true;
		}

		public bool EndClick(int[] pixels, Skin skin, MouseEventArgs e)
		{
			_done = false;
			if (_undo.Points.Count != 0)
				skin.Undo.AddBuffer(_undo);
			_undo = null;

			Program.MainForm.CheckUndo();
			return false;
		}

		public string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_FILL");
		}
	}
}
