//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
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

using MCSkin3D.lemon42;
using OpenTK;
using Paril.Compatibility;
using Paril.OpenGL;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace MCSkin3D
{
	//Added threshold [Xylem] 09/11/2011
	public class FloodFillTool : ITool
	{
		private Rectangle _boundBox;
		private bool _done;
		private byte _threshold;
		private PixelsChangedUndoable _undo;

		public float Threshold //[0-1]
		{
			get { return GlobalSettings.FloodFillThreshold; }
		}

		#region ITool Members

		public void SelectedBrushChanged()
		{
		}

		public void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_undo = new PixelsChangedUndoable(Editor.GetLanguageString("U_PIXELSCHANGED"),
			                                  Editor.MainForm.SelectedTool.MenuItem.Text);
			_boundBox = new Rectangle(0, 0, skin.Width, skin.Height);

			if ((Control.ModifierKeys & Keys.Control) != 0)
				_boundBox = Editor.CurrentModel.GetTextureFaceBounds(new Point(p.X, p.Y), skin);

			_done = false;
		}

		public void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		public bool MouseMoveOnSkin(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			if (_done)
				return false;

			var curve = new BezierCurveQuadric(new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 2));
			_threshold = (byte) ((1 - (curve.CalculatePoint(Threshold)).X) * 255);
			//(byte)((1 - Math.Sin((1 - Threshold) * (Math.PI / 2))) * 255);

			ColorPixel c = pixels[x, y];
			Color oldColor = Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
			ColorManager newColor = ((Control.ModifierKeys & Keys.Shift) != 0)
			                        	? Editor.MainForm.ColorPanel.UnselectedColor
			                        	: Editor.MainForm.ColorPanel.SelectedColor;

			FloodFill(x, y, oldColor, newColor, ref pixels);
			_done = true;
			return true;
		}

		public bool RequestPreview(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			if (x == -1)
				return false;

			var highlightPoint = new Point(x, y);
			bool doHighlight = ((Control.ModifierKeys & Keys.Control) != 0);

			Color newColor;
			if (doHighlight)
			{
				Rectangle part = Editor.CurrentModel.GetTextureFaceBounds(highlightPoint, skin);

				for (int ry = part.Y; ry < part.Y + part.Height; ++ry)
				{
					for (int rx = part.X; rx < part.X + part.Width; ++rx)
					{
						ColorPixel px = pixels[rx, ry];
						Color c = Color.FromArgb(px.Alpha, px.Red, px.Green, px.Blue);
						Color blendMe = Color.FromArgb(64, Color.Green);
						newColor = (Color) ColorBlending.AlphaBlend(blendMe, c);

						pixels[rx, ry] = new ColorPixel((newColor.R << 0) | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24));
					}
				}
			}

			newColor =
				(((Control.ModifierKeys & Keys.Shift) != 0)
				 	? Editor.MainForm.ColorPanel.UnselectedColor
				 	: Editor.MainForm.ColorPanel.SelectedColor).RGB;
			pixels[x, y] = new ColorPixel(newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24));
			return true;
		}

		public bool EndClick(ref ColorGrabber pixels, Skin skin, MouseEventArgs e)
		{
			_done = false;
			if (_undo.Points.Count != 0)
				skin.Undo.AddBuffer(_undo);
			_undo = null;

			Editor.MainForm.CheckUndo();
			return false;
		}

		public string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_FILL");
		}

		#endregion

		private static bool SimilarColor(Color color1, Color color2, byte threshold)
		{
			return
				Math.Abs(color1.R - color2.R) <= threshold &&
				Math.Abs(color1.G - color2.G) <= threshold &&
				Math.Abs(color1.B - color2.B) <= threshold &&
				Math.Abs(color1.A - color2.A) <= threshold
				;
		}

		//Same as similarColor, but avoids some calculations if it can; use this if threshold may be 255 or 0
		private static bool SimilarColor2(Color color1, Color color2, byte threshold)
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
				return SimilarColor(color1, color2, threshold);
		}

		private void FloodFill(int x, int y, Color oldColor, ColorManager newColor, ref ColorGrabber pixels)
		{
			Queue q = new Queue();

			q.Enqueue(new Point(x, y));

			while (q.Count != 0)
			{
				Point pop = (Point)q.Dequeue();

				ColorPixel c = pixels[pop.X, pop.Y];
				Color real = Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);

				if (!SimilarColor2(oldColor, real, _threshold))
					continue;

				if (!_undo.Points.ContainsKey(pop))
				{
					_undo.Points.Add(pop, Tuple.MakeTuple(real, new ColorAlpha(newColor.RGB, 0)));

					pixels[pop.X, pop.Y] =
						new ColorPixel(newColor.RGB.R | (newColor.RGB.G << 8) | (newColor.RGB.B << 16) | (newColor.RGB.A << 24));

					if (_boundBox.Contains(pop.X - 1, pop.Y))
						q.Enqueue(new Point(pop.X - 1, pop.Y));
					if (_boundBox.Contains(pop.X + 1, pop.Y))
						q.Enqueue(new Point(pop.X + 1, pop.Y));
					if (_boundBox.Contains(pop.X, pop.Y - 1))
						q.Enqueue(new Point(pop.X, pop.Y - 1));
					if (_boundBox.Contains(pop.X, pop.Y + 1))
						q.Enqueue(new Point(pop.X, pop.Y + 1));
				}
			}
		}
	}
}