﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Paril.Compatibility;

namespace MCSkin3D
{
	/// <summary>
	/// A simple base class to make brush tools with.
	/// </summary>
	public abstract class BrushToolBase : ITool
	{
		PixelsChangedUndoable _undo;

		Point _oldPixel = new Point(-1, -1);
		public virtual void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_undo = new PixelsChangedUndoable();
		}

		public virtual void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		public bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y, bool incremental)
		{
			if (x == _oldPixel.X && y == _oldPixel.Y)
				return false;
			IsPreview = false;

			var brush = Brushes.SelectedBrush;
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

					var pixNum = xx + (skin.Width * yy);
					var c = pixels[pixNum];
					var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
					var color = ((Control.ModifierKeys & Keys.Shift) != 0) ? Program.MainForm.UnselectedColor : Program.MainForm.SelectedColor;

					var maxAlpha = color.A;
					var alphaToAdd = (float)(byte)(brush[rx, ry] * 255 * (Program.MainForm.SelectedColor.A / 255.0f));

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

					pixels[pixNum] = newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24);
				}
			}

			_oldPixel = new Point(x, y);

			return true;
		}

		public bool IsPreview
		{
			get;
			private set;
		}

		public virtual bool RequestPreview(int[] pixels, Skin skin, int x, int y)
		{
			if (x == -1)
				return false;

			var brush = Brushes.SelectedBrush;
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

					var pixNum = xx + (skin.Width * yy);
					var c = pixels[pixNum];
					var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
					var color = GetLeftColor();
					color = Color.FromArgb((byte)(brush[rx, ry] * 255 * (color.A / 255.0f)), color);

					var newColor = BlendColor(color, oldColor);
					pixels[pixNum] = newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24);
				}
			}

			return true;
		}

		public virtual bool EndClick(int[] pixels, Skin skin, MouseEventArgs e)
		{
			if (_undo.Points.Count != 0)
			{
				skin.Undo.AddBuffer(_undo);
				Program.MainForm.CheckUndo();
				_oldPixel = new Point(-1, -1);
			}
			
			_undo = null;

			return false;
		}

		public abstract bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y);
		public abstract Color BlendColor(Color l, Color r);
		public abstract Color GetLeftColor();

		public abstract string GetStatusLabelText();
	}
}