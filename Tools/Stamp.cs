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
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;
using Paril.OpenGL;

namespace MCSkin3D
{
	public class StampTool : ITool
	{
		public bool MouseMoveOnSkin(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			return MouseMoveOnSkin(ref pixels, skin, x, y, GlobalSettings.PencilIncremental);
		}

		PixelsChangedUndoable _undo;
		Point _oldPixel = new Point(-1, -1);
		ColorPixel[,] _stampedBrush;
		Brush _brushThatWasStamped;

		public void SelectedBrushChanged()
		{
			_stampedBrush = null;
		}

		public virtual void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_undo = new PixelsChangedUndoable(Editor.GetLanguageString("U_PIXELSCHANGED"), Editor.MainForm.SelectedTool.MenuItem.Text);
		}

		public virtual void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		public Color BlendColor(Color l, Color r)
		{
			return (Color)ColorBlending.AlphaBlend(l, r);
		}

		bool HoldingShift { get { return (Control.ModifierKeys & Keys.Shift) != 0; } }

		public bool MouseMoveOnSkin(ref ColorGrabber pixels, Skin skin, int x, int y, bool incremental)
		{
			var brush = Brushes.SelectedBrush;
			if (_brushThatWasStamped != brush)
				_stampedBrush = null;

			if (x == _oldPixel.X && y == _oldPixel.Y)
				return false;
			if (_stampedBrush == null && !HoldingShift)
				return false;

			IsPreview = false;

			int startX = x - (brush.Width / 2);
			int startY = y - (brush.Height / 2);

			for (int ry = 0; ry < brush.Height; ++ry)
			{
				for (int rx = 0; rx < brush.Width; ++rx)
				{
					int xx = startX + rx;
					int yy = startY + ry;

					if (xx < 0 || xx >= skin.Width ||
						yy < 0 || yy >= skin.Height)
						continue;

					if (brush[rx, ry] == 0.0f)
						continue;

					var c = pixels[xx, yy];

					if (HoldingShift)
					{
						if (_stampedBrush == null)
							_stampedBrush = new ColorPixel[brush.Width, brush.Height];

						_brushThatWasStamped = brush;
						_stampedBrush[rx, ry] = c;
						continue;
					}

					var oldColor = Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
					var color = Color.FromArgb(_stampedBrush[rx, ry].Alpha, _stampedBrush[rx, ry].Red, _stampedBrush[rx, ry].Green, _stampedBrush[rx, ry].Blue);

					var maxAlpha = color.A;
					var alphaToAdd = (float)(byte)(brush[rx, ry] * 255 * (Editor.MainForm.SelectedColor.A / 255.0f));

					if (!incremental && _undo.Points.ContainsKey(new Point(xx, yy)) &&
						_undo.Points[new Point(xx, yy)].Item2.TotalAlpha >= maxAlpha)
						continue;

					if (!incremental && _undo.Points.ContainsKey(new Point(xx, yy)) &&
						_undo.Points[new Point(xx, yy)].Item2.TotalAlpha + alphaToAdd >= maxAlpha)
						alphaToAdd = maxAlpha - _undo.Points[new Point(xx, yy)].Item2.TotalAlpha;

					color = Color.FromArgb((byte)(alphaToAdd), color);

					var newColor = BlendColor(color, oldColor);

					if (oldColor == newColor)
						continue;

					if (_undo.Points.ContainsKey(new Point(xx, yy)))
					{
						var tupl = _undo.Points[new Point(xx, yy)];

						tupl.Item2 = new ColorAlpha(newColor, tupl.Item2.TotalAlpha + alphaToAdd);
						_undo.Points[new Point(xx, yy)] = tupl;
					}
					else
						_undo.Points.Add(new Point(xx, yy), Tuple.MakeTuple(oldColor, new ColorAlpha(newColor, alphaToAdd)));

					pixels[xx, yy] = new ColorPixel(newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24));
				}
			}

			_oldPixel = new Point(x, y);

			return !HoldingShift;
		}

		public bool IsPreview
		{
			get;
			private set;
		}

		public virtual bool RequestPreview(ref ColorGrabber pixels, Skin skin, int x, int y)
		{
			var brush = Brushes.SelectedBrush;
			if (_brushThatWasStamped != brush)
				_stampedBrush = null;
			if (x == -1)
				return false;
			if (_stampedBrush == null && !HoldingShift)
				return false;

			int startX = x - (brush.Width / 2);
			int startY = y - (brush.Height / 2);
			IsPreview = true;

			for (int ry = 0; ry < brush.Height; ++ry)
			{
				for (int rx = 0; rx < brush.Width; ++rx)
				{
					int xx = startX + rx;
					int yy = startY + ry;

					if (xx < 0 || xx >= skin.Width ||
						yy < 0 || yy >= skin.Height)
						continue;

					if (brush[rx, ry] == 0.0f)
						continue;

					var c = pixels[xx, yy];
					Color newColor;

					if (!HoldingShift)
					{
						var oldColor = Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
						var color = Color.FromArgb(_stampedBrush[rx, ry].Alpha, _stampedBrush[rx, ry].Red, _stampedBrush[rx, ry].Green, _stampedBrush[rx, ry].Blue);
						color = Color.FromArgb((byte)(brush[rx, ry] * 255 * (color.A / 255.0f)), color);

						newColor = BlendColor(color, oldColor);
					}
					else
						newColor = Color.FromArgb(c.Alpha, Color.White);

					pixels[xx, yy] = new ColorPixel(newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24));
				}
			}

			return true;
		}

		public virtual bool EndClick(ref ColorGrabber pixels, Skin skin, MouseEventArgs e)
		{
			if (_undo.Points.Count != 0)
			{
				skin.Undo.AddBuffer(_undo);
				Editor.MainForm.CheckUndo();
				_oldPixel = new Point(-1, -1);
			}

			_undo = null;

			return false;
		}

		public string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_STAMP");
		}
	}
}
