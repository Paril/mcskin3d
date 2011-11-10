using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;
using Devcorp.Controls.Design;

namespace MCSkin3D
{
	public class NoiseTool : ITool
	{
		static Random _random = new Random();
		Rectangle _applyRect;
		public void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_startPoint = e.Location;
			_percentageApply = 0;
			seed = _random.Next();

			_applyRect = PlayerModel.HumanModel.GetTextureFaceBounds(p, skin);
		}

		Point _startPoint = new Point(-1, -1);
		float _percentageApply = 0;
		int seed;
		public void MouseMove(Skin skin, MouseEventArgs e)
		{
			var delta = new Point(e.X - _startPoint.X, e.Y - _startPoint.Y);
			_percentageApply = ((float)Math.Sqrt(Math.Abs((delta.X * delta.X) + (delta.Y * delta.Y)))) / Program.MainForm.Renderer.Height;

			if (_percentageApply > 100)
				_percentageApply = 100;
		}

		public bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			return false;
		}

		public bool RequestPreview(int[] pixels, Skin skin, int x, int y)
		{
			if (_startPoint.X == -1)
				return false;

			var r = new Random(seed);
			for (int ry = _applyRect.Y; ry < _applyRect.Y + _applyRect.Height; ++ry)
				for (int rx = _applyRect.X; rx < _applyRect.X + _applyRect.Width; ++rx)
				{
					var px = rx + (ry * skin.Width);
					Color c = Color.FromArgb((pixels[px] >> 24) & 0xFF, (pixels[px] >> 0) & 0xFF, (pixels[px] >> 8) & 0xFF, (pixels[px] >> 16) & 0xFF);
					var hsv = ColorSpaceHelper.RGBtoHSB(c);
					hsv.Brightness += ((r.NextDouble() - 0.5f) * 2) * _percentageApply;

					if (hsv.Brightness < 0)
						hsv.Brightness = 0;
					if (hsv.Brightness > 1)
						hsv.Brightness = 1;

					var newColor = ColorSpaceHelper.HSBtoColor(hsv);
					pixels[px] = (newColor.R << 0) | (newColor.G << 8) | (newColor.B << 16) | (c.A << 24);
				}

			return true;
		}

		public void EndClick(Skin skin, MouseEventArgs e)
		{
			_startPoint = new Point(-1, -1);
		}

		public string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_NOISE");
		}
	}
}