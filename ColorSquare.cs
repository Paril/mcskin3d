using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Paril.Controls.Color
{
	public partial class ColorSquare : Control
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
			{
				components.Dispose();
			}
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

		public ColorSquare()
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer | 
				ControlStyles.UserMouse | 
				ControlStyles.UserPaint | 
				ControlStyles.AllPaintingInWmPaint, true);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			GenerateSquare();
			base.OnSizeChanged(e);
		}

		void GenerateSquare()
		{
			BackgroundImage = ColorSpaceRenderer.GenerateColorSquare(Width, Height);
		}

		int _currentHue, _currentSat;

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

		PointF PickPosition()
		{
			return new PointF((((float)CurrentHue / 360.0f) * Width), Height - (((float)CurrentSat / 240.0f) * Height));
		}

		Point _lastPoint;
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var p = PickPosition();
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			e.Graphics.DrawEllipse(Pens.Black, new RectangleF(p.X - 2, p.Y - 2, 4, 4));
			ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(0, 0, Width, Height), Border3DStyle.SunkenOuter);
		}

		void CheckMouse(MouseEventArgs e)
		{
			if ((MouseButtons & System.Windows.Forms.MouseButtons.Left) != 0)
			{
				float[] vals = HueSaturationForLocation(e.X, e.Y);
				CurrentHue = (int)vals[0];
				CurrentSat = (int)vals[1];
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
			return new float[]
			{
				((float)x / (float)Width) * 360.0f,
				240.0f - (((float)y / (float)Height) * 240.0f)
			};
		}
	}

	public class ColorSpaceRenderer
	{
		public static Bitmap GenerateColorSquare(int width, int height)
		{
			Bitmap colorSquare = new Bitmap(width, height);
			float satIncrease = 240.0f / (float)height;
			float hueIncrease = 360.0f / (float)width;

			using (Paril.Drawing.FastPixel fp = new Paril.Drawing.FastPixel(colorSquare, true))
			{
				for (int y = 0; y < height; ++y)
					for (int x = 0; x < width; ++x)
						fp.SetPixel(x, y, Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(new Devcorp.Controls.Design.HSL(x * hueIncrease, 1 - ((y * satIncrease) / 240), 0.5f)).ToColor());
			}

			return colorSquare;
		}

		public static void GenerateColorSlider(Graphics g, System.Drawing.Color midColor, Rectangle rect)
		{
			var top = new RectangleF(rect.X, rect.Y, rect.Width, (float)rect.Height / 2.0f);
			var bottom = new RectangleF(rect.X, (float)rect.Y + ((float)rect.Height / 2.0f), rect.Width, (float)rect.Height / 2.0f);
			
			var top2 = new RectangleF(rect.X, rect.Y - 1, rect.Width, (float)rect.Height / 2.0f);
			var bottom2 = new RectangleF(rect.X, (float)rect.Y - 1 + ((float)rect.Height / 2.0f), rect.Width, (float)rect.Height / 2.0f);

			g.FillRectangle(new LinearGradientBrush(top2, System.Drawing.Color.White, midColor, LinearGradientMode.Vertical), top);
			g.FillRectangle(new LinearGradientBrush(bottom2, midColor, System.Drawing.Color.Black, LinearGradientMode.Vertical), bottom);
		}
	}
}
