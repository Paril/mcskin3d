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

using Devcorp.Controls.Design;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Paril.Controls.Color
{
	public class ColorSquare : Control
	{
		#region Component Designer generated code

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
				components.Dispose();
			base.Dispose(disposing);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion

		private int _currentHue, _currentSat;
		private Point _lastPoint;

		public ColorSquare()
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer |
			         ControlStyles.UserMouse |
			         ControlStyles.UserPaint |
			         ControlStyles.AllPaintingInWmPaint, true);
		}

		public int CurrentHue
		{
			get { return _currentHue; }
			set
			{
				_currentHue = value;

				if (_currentHue < 0)
					_currentHue = 0;
				if (_currentHue > 360)
					_currentHue = 360;

				OnHueChanged(EventArgs.Empty);
			}
		}

		public int CurrentSat
		{
			get { return _currentSat; }
			set
			{
				_currentSat = value;

				if (_currentSat < 0)
					_currentSat = 0;
				if (_currentSat > 240)
					_currentSat = 240;

				OnSatChanged(EventArgs.Empty);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			GenerateSquare();
			base.OnSizeChanged(e);
		}

		private void GenerateSquare()
		{
			BackgroundImage = ColorSpaceRenderer.GenerateColorSquare(Width, Height);
		}

		public event EventHandler HueChanged;
		public event EventHandler SatChanged;

		protected virtual void OnHueChanged(EventArgs e)
		{
			Invalidate();

			if (HueChanged != null)
				HueChanged(this, e);
		}

		protected virtual void OnSatChanged(EventArgs e)
		{
			Invalidate();

			if (SatChanged != null)
				SatChanged(this, e);
		}

		private PointF PickPosition()
		{
			return new PointF(((CurrentHue / 360.0f) * Width), Height - ((CurrentSat / 240.0f) * Height));
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			PointF p = PickPosition();
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.DrawEllipse(Pens.Black, new RectangleF(p.X - 2, p.Y - 2, 4, 4));
			ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(0, 0, Width, Height), Border3DStyle.SunkenOuter);
		}

		private void CheckMouse(MouseEventArgs e)
		{
			if ((MouseButtons & MouseButtons.Left) != 0)
			{
				float[] vals = HueSaturationForLocation(e.X, e.Y);
				CurrentHue = (int) vals[0];
				CurrentSat = (int) vals[1];
				_lastPoint = e.Location;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			CheckMouse(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			CheckMouse(e);
		}

		public float[] HueSaturationForLocation(int x, int y)
		{
			return new[]
			       {
			       	(x / (float) Width) * 360.0f,
			       	240.0f - ((y / (float) Height) * 240.0f)
			       };
		}
	}

	public class ColorSpaceRenderer
	{
		public static Bitmap GenerateColorSquare(int width, int height)
		{
			var colorSquare = new Bitmap(width, height);
			// START HUE DRAW
			Graphics g = Graphics.FromImage(colorSquare);
			//Set the hue shades with the correct saturation and luminance
			System.Drawing.Color[] theColors = {
			                                   	ColorSpaceHelper.HSLtoColor(new HSL(0, 1, 0.5f)),
			                                   	ColorSpaceHelper.HSLtoColor(new HSL(60, 1, 0.5f)),
			                                   	ColorSpaceHelper.HSLtoColor(new HSL(120, 1, 0.5f)),
			                                   	ColorSpaceHelper.HSLtoColor(new HSL(180, 1, 0.5f)),
			                                   	ColorSpaceHelper.HSLtoColor(new HSL(240, 1, 0.5f)),
			                                   	ColorSpaceHelper.HSLtoColor(new HSL(300, 1, 0.5f)),
			                                   	ColorSpaceHelper.HSLtoColor(new HSL(360, 1, 0.5f))
			                                   };
			//Calculate positions
			float percent = 1.0f / 6;
			float[] thePositions = {0.0f, percent, percent * 2, percent * 3, percent * 4, percent * 5, 1.0f};
			//Set blend
			var theBlend = new ColorBlend();
			theBlend.Colors = theColors;
			theBlend.Positions = thePositions;
			//Get rectangle
			var colorRect = new Rectangle(0, 0, width, height);
			//Make the linear brush and assign the custom blend to it
			var theBrush = new LinearGradientBrush(colorRect,
			                                       System.Drawing.Color.Red,
			                                       System.Drawing.Color.Red, 0, false);
			theBrush.InterpolationColors = theBlend;
			//Draw rectangle
			g.FillRectangle(theBrush, colorRect);
			//END HUE
			//START SATURATION
			//--- 0% sat 50% lum = 128 r, g and b
			System.Drawing.Color halfSatColor = System.Drawing.Color.FromArgb(128, 128, 128);
			System.Drawing.Color halfSatColorNoAlpha = System.Drawing.Color.FromArgb(0, 128, 128, 128);

			g.FillRectangle(new LinearGradientBrush(colorRect, halfSatColorNoAlpha, halfSatColor, 90, false), colorRect);
			//END SATURATION

			return colorSquare;
		}

		public static void GenerateColorSlider(Graphics g, System.Drawing.Color midColor, Rectangle rect)
		{
			var top = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height / 2.0f);
			var bottom = new RectangleF(rect.X, rect.Y + (rect.Height / 2.0f), rect.Width, rect.Height / 2.0f);

			var top2 = new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height / 2.0f);
			var bottom2 = new RectangleF(rect.X, (float) rect.Y - 1 + (rect.Height / 2.0f), rect.Width, rect.Height / 2.0f);

			g.FillRectangle(new LinearGradientBrush(top2, System.Drawing.Color.White, midColor, LinearGradientMode.Vertical), top);
			g.FillRectangle(new LinearGradientBrush(bottom2, midColor, System.Drawing.Color.Black, LinearGradientMode.Vertical),
			                bottom);
		}
	}
}