using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class DarkenLightenTool : ITool
	{
		PixelsChangedUndoable _undo;

		public bool Incremental
		{
			get { return GlobalSettings.DarkenLightenIncremental; }
		}

		public float Exposure
		{
			get { return GlobalSettings.DarkenLightenExposure; }
		}

		public void BeginClick(Skin skin, MouseEventArgs e)
		{
			_undo = new PixelsChangedUndoable();
		}

		public void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		Color GetColor(Color old)
		{
			bool ctrlIng = (Control.ModifierKeys & Keys.Shift) != 0;
			bool switchTools = (!Program.MainForm.DarkenLightenOptions.Inverted && ctrlIng) || (Program.MainForm.DarkenLightenOptions.Inverted && !ctrlIng);
			var hsl = Devcorp.Controls.Design.ColorSpaceHelper.RGBtoHSL(old);

			if (switchTools)
				hsl.Luminance -= Exposure / 5.0f;
			else
				hsl.Luminance += Exposure / 5.0f;

			if (hsl.Luminance < 0)
				hsl.Luminance = 0;
			if (hsl.Luminance > 1)
				hsl.Luminance = 1;

			return Devcorp.Controls.Design.ColorSpaceHelper.HSLtoColor(hsl);
		}

		public bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			if (!Incremental && _undo.Points.ContainsKey(new Point(x, y)))
				return false;

			var pixNum = x + (skin.Width * y);
			var c = pixels[pixNum];
			var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
			var newColor = GetColor(oldColor);

			if (oldColor == newColor)
				return false;

			if (_undo.Points.ContainsKey(new Point(x, y)))
			{
				var tupl = _undo.Points[new Point(x, y)];
				tupl.Item2 = newColor;
				_undo.Points[new Point(x, y)] = tupl;
			}
			else
				_undo.Points.Add(new Point(x, y), Tuple.MakeTuple(oldColor, newColor));

			pixels[pixNum] = newColor.R | (newColor.G << 8) | (newColor.B << 16) | (newColor.A << 24);
			return true;
		}

		public bool RequestPreview(int[] pixels, Skin skin, int x, int y)
		{
			var pixNum = x + (skin.Width * y);
			var c = pixels[pixNum];
			var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);
			var newColor = GetColor(oldColor);
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
