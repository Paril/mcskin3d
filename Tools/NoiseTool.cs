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

			if ((Control.ModifierKeys & Keys.Control) != 0)
				_applyRect = PlayerModel.HumanModel.GetTextureFaceBounds(p, skin);
			else
				_applyRect = new Rectangle(0, 0, skin.Width, skin.Height);

			_undo = new PixelsChangedUndoable();
		}

		Point _startPoint = new Point(-1, -1);
		float _percentageApply = 0;
		int seed;
		PixelsChangedUndoable _undo;

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
			Point highlightPoint = new Point(x, y);
			bool doHighlight = ((Control.ModifierKeys & Keys.Control) != 0);

			if (_startPoint.X == -1)
			{
				if (doHighlight)
				{
					var part = PlayerModel.HumanModel.GetTextureFaceBounds(highlightPoint, skin);

					for (int ry = part.Y; ry < part.Y + part.Height; ++ry)
						for (int rx = part.X; rx < part.X + part.Width; ++rx)
						{
							var px = rx + (ry * skin.Width);
							Color c = Color.FromArgb((pixels[px] >> 24) & 0xFF, (pixels[px] >> 0) & 0xFF, (pixels[px] >> 8) & 0xFF, (pixels[px] >> 16) & 0xFF);
							Color blendMe = Color.FromArgb(64, Color.Green);
							var newColor = (Color)ColorBlending.AlphaBlend(blendMe, c);
							pixels[px] = (newColor.R << 0) | (newColor.G << 8) | (newColor.B << 16) | (c.A << 24);
						}

					return true;
				}

				return false;
			}

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

		public bool EndClick(int[] pixels, Skin skin, MouseEventArgs e)
		{
			_startPoint = new Point(-1, -1);
			
			if (_percentageApply != 0)
			{
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
						_undo.Points.Add(new Point(rx, ry), new Tuple<Color,ColorAlpha>(c, new ColorAlpha(Color.FromArgb(c.A, newColor), 0)));
						pixels[px] = (newColor.R << 0) | (newColor.G << 8) | (newColor.B << 16) | (c.A << 24);
					}

				skin.Undo.AddBuffer(_undo);
				Program.MainForm.CheckUndo();
				return true;
			}

			return false;
		}

		public string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_NOISE");
		}
	}
}