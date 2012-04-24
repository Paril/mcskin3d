using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MCSkin3D.lemon42
{
	//ColorPick is an alternative to the old ColorSquare.
	//It is part based on ColorSquare but replaces it with a nice
	//and sleek HSV triangle picker.

	public partial class ColorPick : UserControl
	{
		private short _currentAlpha = 255; //hue [0-360], sat&val [0-100]
		private short _currentHue, _currentSat, _currentVal; //hue [0-360], sat&val [0-100]
		private bool clickCircle;
		private bool clickNothing;
		private PointF clickPoint;
		private bool clickTriangle;
		private bool drawPoint;
		private short initRot;
		private bool rawCircle;
		private bool rawInner;
		private bool rawTriangle;
		private bool rotatePoint;
		private short thickness;
		private Bitmap wheel;

		public ColorPick()
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer |
			         ControlStyles.UserMouse |
			         ControlStyles.UserPaint |
			         ControlStyles.AllPaintingInWmPaint, true);
		}

		public short CurrentHue
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

		public short CurrentSat
		{
			get { return _currentSat; }
			set
			{
				_currentSat = value;

				if (_currentSat < 0)
					_currentSat = 0;
				if (_currentSat > 100)
					_currentSat = 100;

				OnSatChanged(EventArgs.Empty);
			}
		}

		public short CurrentVal
		{
			get { return _currentVal; }
			set
			{
				_currentVal = value;

				if (_currentVal < 0)
					_currentVal = 0;
				if (_currentVal > 100)
					_currentVal = 100;

				OnValChanged(EventArgs.Empty);
			}
		}

		public short CurrentAlpha
		{
			get { return _currentAlpha; }
			set
			{
				_currentAlpha = value;

				if (_currentVal < 0)
					_currentVal = 0;
				if (_currentVal > 255)
					_currentVal = 255;

				OnAlphaChanged(EventArgs.Empty);
			}
		}

		public ColorManager.HSVColor CurrentHSV
		{
			get { return new ColorManager.HSVColor(_currentHue, (byte) _currentSat, (byte) _currentVal, (byte) _currentAlpha); }
			set
			{
				if (_currentHue != value.H ||
				    _currentSat != value.S ||
				    _currentVal != value.V)
				{
					_currentHue = value.H;
					_currentSat = value.S;
					_currentVal = value.V;
					setPoint();
					OnHSVChanged(EventArgs.Empty);
				}

				if (_currentAlpha != value.A)
					_currentAlpha = value.A;
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			thickness = (14);
			wheel = ColorPickRenderer.ColorWheel(Width, thickness, 4);
			base.OnSizeChanged(e);
		}

		public void setPoint()
		{
			rotatePoint = false;
			clickPoint = LocationForColorHSV(CurrentHSV);
			drawPoint = true;
		}

		public event EventHandler HueChanged;
		public event EventHandler SatChanged;
		public event EventHandler ValChanged;
		public event EventHandler AlphaChanged;
		public event EventHandler HSVChanged;

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

		protected virtual void OnValChanged(EventArgs e)
		{
			Invalidate();

			if (ValChanged != null)
				ValChanged(this, e);
		}

		protected virtual void OnAlphaChanged(EventArgs e)
		{
			if (AlphaChanged != null)
				AlphaChanged(this, e);
		}

		protected virtual void OnHSVChanged(EventArgs e)
		{
			Invalidate();

			if (HSVChanged != null)
				HSVChanged(this, e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Graphics g = e.Graphics;

			using (Bitmap triangle = ColorPickRenderer.ColorTriangle(_currentHue, Width - thickness * 2, 2))
				g.DrawImageUnscaled(triangle, thickness, thickness);

			g.DrawImageUnscaled(wheel, 0, 0);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			PointF p = ColorPickUtil.RotatePoint(clickPoint, new PointF(Width / 2.0f, Width / 2.0f),
			                                     ColorPickUtil.DegreeToRadian(rotatePoint ? _currentHue - initRot : 0));

			if (drawPoint)
				g.DrawEllipse(new Pen(Negative(CurrentHSV), 2), new RectangleF(p.X - 3, p.Y - 3, 6, 6));

			PointF start = ColorPickUtil.RotatePoint(new PointF(Width - thickness, (Height / 2.0f) - 1.5f),
			                                         new PointF(Width / 2.0f, Width / 2.0f),
			                                         ColorPickUtil.DegreeToRadian(_currentHue));
			PointF end = ColorPickUtil.RotatePoint(new PointF(Width, (Height / 2.0f) - 1.5f),
			                                       new PointF(Width / 2.0f, Width / 2.0f),
			                                       ColorPickUtil.DegreeToRadian(_currentHue));

			g.DrawLine(new Pen(Negative(new ColorManager.HSVColor(CurrentHSV.H, 100, 100)), 2), start, end);
		}

		private double AngleFromPoints(Point p2)
		{
			var p = new Point(Width / 2, Width / 2);
			return Math.Atan2(p2.Y - p.Y, p2.X - p.X) * 180 / Math.PI;
		}

		private void CheckMouse(MouseEventArgs e)
		{
			if ((MouseButtons & MouseButtons.Left) != 0)
			{
				if (!clickNothing)
				{
					if (clickCircle)
					{
						_currentHue = (short) WrapDegree((int) AngleFromPoints(e.Location));
						rotatePoint = true;
						OnHSVChanged(EventArgs.Empty);
						setPoint();
					}
				}
				if (!drawPoint) drawPoint = clickTriangle;
				if (clickTriangle)
				{
					PointF pt = ClipPoint(e.Location, _currentHue);
					rotatePoint = false;
					initRot = _currentHue;

					CurrentHSV = ColorHSVForLocation(pt.X, pt.Y, _currentHue);
				}
			}
		}

		public int WrapDegree(int angle)
		{
			return (angle < 0 || angle > 360 ? angle - 360 * (int) Math.Floor((double) angle / 360) : angle);
		}

		private void SetRaw(Point p)
		{
			rawCircle = InCircle(p, new Point(Width / 2, Width / 2), Width / 2);
			rawTriangle = InTriangle(p, Triangle(_currentHue));
			rawInner = InCircle(p, new Point(Width / 2, Width / 2), (Width - thickness * 2) / 2);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			clickCircle = false;
			clickNothing = false;
			clickTriangle = false;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			SetRaw(e.Location);
			clickCircle = rawCircle && !rawInner;
			if (rawInner)
				clickNothing = !rawTriangle;
			else
				clickNothing = !rawCircle && !rawTriangle && !rawInner;
			clickTriangle = rawTriangle && !clickCircle && !clickNothing;
			CheckMouse(e);
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			SetRaw(e.Location);
			CheckMouse(e);
			Invalidate();
		}

		public PointF LocationForColorHSV(ColorManager.HSVColor hsv)
		{
			/*PointF[] polygon = Triangle(0);
			float x = (float)((((float)hsv.S / 100.0f) * (polygon[0].X - polygon[1].X)) + polygon[1].X);
            //if (x > polygon[0].X)
            //    x = polygon[0].X;
			
            PointF p = ColorPickUtil.RotatePoint(polygon[0], new PointF(Width / 2, Height / 2), ColorPickUtil.DegreeToRadian(30));

            float y = (float)(((float)hsv.V / 100.0f) * (Triangle(30)[1].Y + thickness));
            //if (y > Triangle(30)[1].Y)
            //    y = Triangle(30)[1].Y;

			float height = (polygon[1].Y - polygon[2].Y - thickness);
			p.Y = y + (height / 2);

            p = ColorPickUtil.RotatePoint(p, new PointF(Width/2, Height/2), ColorPickUtil.DegreeToRadian(-30));

            return new PointF(x, p.Y);*/
			PointF[] InnerPoints = Triangle(0);
			double h = hsv.H;
			double s = hsv.S / 100.0;
			double v = hsv.V / 100.0;

			PointF vH = InnerPoints[0];
			PointF vS = InnerPoints[1];
			PointF vV = InnerPoints[2];

			// saturation first, then value
			// this design matches with the picture from wiki

			var vStoH = new PointF((vH.X - vS.X) * (float) s, (vH.Y - vS.Y) * (float) s);
			var vS2 = new PointF(vS.X + vStoH.X, vS.Y + vStoH.Y);
			var vVtovS2 = new PointF((vS2.X - vV.X) * (float) v, (vS2.Y - vV.Y) * (float) v);
			var final = new PointF(vV.X + vVtovS2.X, vV.Y + vVtovS2.Y);

			return ColorPickUtil.RotatePoint(final, new PointF(Width / 2.0f, Width / 2.0f),
			                                 ColorPickUtil.DegreeToRadian(_currentHue));
		}

		public ColorManager.HSVColor ColorHSVForLocation(float x, float y, short ang)
		{
			/*PointF p = ColorPickUtil.RotatePoint(new PointF(x, y), new PointF(Width / 2.0f, Width / 2.0f), ColorPickUtil.DegreeToRadian(-angle));
			PointF[] polygon = Triangle(0);
			short hue = angle;
			byte saturation = (byte)(Math.Round((double)(p.X - polygon[1].X) / (polygon[0].X - polygon[1].X) * 100)); // left 0 - right 100

			if (p.X < polygon[1].X || saturation < 0 || saturation == 255)
				saturation = 0;

			if (p.X > polygon[0].X || saturation > 100)
				saturation = 100;

			//Y calcs
			float width = (polygon[0].X - polygon[1].X);
			float height = (polygon[1].Y - polygon[2].Y);
			float maxY = (((polygon[0].X - p.X) / (float)width) * (float)height);
			float min = polygon[0].Y - (maxY - (maxY / 2));
			float max =  polygon[0].Y + maxY - (maxY / 2);

			float valueRaw = ColorPickUtil.RotatePoint(new PointF(x, y), new PointF(Width / 2.0f, Width / 2.0f), ColorPickUtil.DegreeToRadian(-angle + 30)).Y;
			byte value = (byte)(Math.Round((double)(valueRaw - thickness) / (Triangle(30)[1].Y - thickness) * 100));

			if (max == min)
				value = 100;
			if (valueRaw > Triangle(30)[1].Y)
				value = 100;
			if (valueRaw - thickness < 0)
				value = 0;

			return new ColorManager.HSVColor(hue, saturation, value, (byte)_currentAlpha);*/

			double h = CurrentHue;
			PointF[] InnerPoints = Triangle(_currentHue);

			PointF vH = InnerPoints[0];
			PointF vV = InnerPoints[2];
			PointF vS = InnerPoints[1];

			var pt = new PointF(x, y);

			var vVtoPoint = new PointF(pt.X - vV.X, pt.Y - vV.Y);
			var vVtovS = new PointF(vS.X - vV.X, vS.Y - vV.Y);

			// a *dot* b = ||a|| ||b|| cos(o)
			// gonna find the angle between the 2 vectors: vV-> clicked point and vV -> vS
			// then ratio it against PI / 3 (60 degree), I believed the ratio should be the same with the ratio on the vector vS -> vH 

			double dotproduct = vVtoPoint.X * vVtovS.X + vVtoPoint.Y * vVtovS.Y;
			double vVtoPointLength = Math.Sqrt(vVtoPoint.X * vVtoPoint.X + vVtoPoint.Y * vVtoPoint.Y);
			double vVtovSLength = Math.Sqrt(vVtovS.X * vVtovS.X + vVtovS.Y * vVtovS.Y);
			double angle = Math.Acos(dotproduct / (vVtoPointLength * vVtovSLength));
			double s = angle / (Math.PI / 3); // use this ratio for saturation
			s = s <= 1.0
			    	? s >= 0 ? s : 0
			    	: 1.0;

			var vStovH = new PointF(vH.X - vS.X, vH.Y - vS.Y);
			var vStovH2 = new PointF(vStovH.X * (float) s, vStovH.Y * (float) s); // apply scalar to get new vector
			var vVtovH2 = new PointF(vVtovS.X + vStovH2.X, vVtovS.Y + vStovH2.Y);
			double vVtovH2Length = Math.Sqrt(vVtovH2.X * vVtovH2.X + vVtovH2.Y * vVtovH2.Y);
			double v = vVtoPointLength / vVtovH2Length; // ratio for value

			v = v <= 1.0
			    	? v >= 0 ? v : 0
			    	: 1.0;

			return new ColorManager.HSVColor((short) h, (byte) (s * 100), (byte) (v * 100), (byte) _currentAlpha);
		}

		private void ColorPick_Load(object sender, EventArgs e)
		{
			wheel = ColorPickRenderer.ColorWheel(Width, 14, 4);
		}

		//check if point in circle
		public bool InCircle(Point p, Point center, int radius)
		{
			return Math.Pow(p.X - center.X, 2) + Math.Pow(p.Y - center.Y, 2) <= Math.Pow(radius, 2);
		}

		//check if point in triangle
		public int dot(Point p1, Point p2)
		{
			return p1.X * p2.X + p1.Y * p2.Y;
		}

		public Point sub(Point p1, Point p2)
		{
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}

		public Point add(Point p1, Point p2)
		{
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}

		public float dot(PointF p1, PointF p2)
		{
			return p1.X * p2.X + p1.Y * p2.Y;
		}

		public PointF sub(PointF p1, PointF p2)
		{
			return new PointF(p1.X - p2.X, p1.Y - p2.Y);
		}

		public PointF add(PointF p1, PointF p2)
		{
			return new PointF(p1.X + p2.X, p1.Y + p2.Y);
		}

		public bool InTriangle(PointF p, PointF[] t)
		{
			PointF v0 = sub(t[1], t[0]);
			PointF v1 = sub(t[2], t[0]);
			PointF v2 = sub(p, t[0]);

			float dot00 = dot(v0, v0);
			float dot01 = dot(v0, v1);
			float dot02 = dot(v0, v2);
			float dot11 = dot(v1, v1);
			float dot12 = dot(v1, v2);

			double invDenom = (double) 1 / (dot00 * dot11 - dot01 * dot01);
			double u = ((double) dot11 * dot02 - dot01 * dot12) * invDenom;
			double v = ((double) dot00 * dot12 - dot01 * dot02) * invDenom;

			return (u >= 0) && (v >= 0) && (u + v < 1);
		}

		public PointF[] Triangle(int angle)
		{
			PointF first = ColorPickUtil.RotatePoint(new PointF(Width - thickness, (Width - thickness) / 2.0f),
			                                         new PointF(Width / 2.0f, Width / 2.0f),
			                                         ColorPickUtil.DegreeToRadian(angle + 7));

			return new PointF[3]
			       {
			       	first,
			       	ColorPickUtil.RotatePoint(first, new PointF(Width / 2.0f, Width / 2.0f), ColorPickUtil.DegreeToRadian(120)),
			       	ColorPickUtil.RotatePoint(first, new PointF(Width / 2.0f, Width / 2.0f), ColorPickUtil.DegreeToRadian(240))
			       };
		}

		public PointF ClipPoint(PointF p, int angle)
		{
			/*if (InTriangle(p, Triangle(angle)))
				return p;
			else
			{
				PointF _clickPosition = ColorPickUtil.RotatePoint(p, new PointF(Width / 2.0f, Width / 2.0f), ColorPickUtil.DegreeToRadian(-angle));
				PointF[] polygon = Triangle(0);
				float width = (polygon[0].X - polygon[1].X);
				float height = (polygon[1].Y - polygon[2].Y);

				PointF offsetFromPoint = new PointF(polygon[0].X - _clickPosition.X, polygon[0].Y - _clickPosition.Y);

				float maxY = (((polygon[0].X - _clickPosition.X) / (float)width) * (float)height);
				float highestY = (((polygon[0].X - polygon[1].X) / (float)width) * (float)height);

				PointF clippedPosition = _clickPosition;

				if (offsetFromPoint.X > width)
				{
					clippedPosition.X = polygon[1].X;
					maxY = highestY;
				}

				if (offsetFromPoint.Y > maxY - (maxY / 2))
					clippedPosition.Y = (polygon[0].Y - (maxY - (maxY / 2)));
				else if (offsetFromPoint.Y < -(maxY - (maxY / 2)))
					clippedPosition.Y = (polygon[0].Y + (maxY - (maxY / 2)));

				if (clippedPosition.X > polygon[0].X)
					clippedPosition = polygon[0];

				return ColorPickUtil.RotatePoint(clippedPosition, new PointF(Width / 2.0f, Width / 2.0f), ColorPickUtil.DegreeToRadian(angle));
			}*/

			PointF _clickPosition = ColorPickUtil.RotatePoint(p, new PointF(Width / 2.0f, Width / 2.0f),
			                                                  ColorPickUtil.DegreeToRadian(-angle));
			PointF[] polygon = Triangle(0);

			if (_clickPosition.X < polygon[2].X)
				_clickPosition.X = polygon[2].X;
			if (_clickPosition.Y < polygon[2].Y)
				_clickPosition.Y = polygon[2].Y;

			return ColorPickUtil.RotatePoint(_clickPosition, new PointF(Width / 2.0f, Width / 2.0f),
			                                 ColorPickUtil.DegreeToRadian(angle));
		}


		// From GTK:
		public static float Intensity(float r, float g, float b)
		{
			return ((r) * 0.30f + (g) * 0.59f + (b) * 0.11f);
		}

		public static float Intensity(byte r, byte g, byte b)
		{
			return Intensity(r / 255.0f, g / 255.0f, b / 255.0f);
		}

		public static Color Negative(ColorManager.HSVColor c) //haha this isnt even negative LOL
		{
			Color rgb = ColorManager.HSVtoRGB(c);
			if (Intensity(rgb.R, rgb.G, rgb.B) > 0.5f)
				return Color.Black;
			else
				return Color.White;
		}
	}

	public class ColorPickRenderer
	{
		public static Bitmap ColorWheel(int size, int width, int multisampling)
			//4x multisampling recomended for optimum quality and speed.
		{
			if (width < 1 || size < 1)
				return null;

			int m = multisampling;
			size = size * m;
			width = width * m;

			using (var b = new Bitmap(size, size))
			{
				using (Graphics g = Graphics.FromImage(b))
				{
					g.Clear(Color.Transparent);
					var rect = new Rectangle(0, 0, size - m, size - m);
					using (var wheel_path = new GraphicsPath())
					{
						wheel_path.AddEllipse(rect);
						wheel_path.Flatten();

						int num_pts = (wheel_path.PointCount - 1);
						var surround_colors = new Color[wheel_path.PointCount];
						for (int i = 0; i < wheel_path.PointCount; i++)
							surround_colors[i] = new ColorManager.HSVColor((short) ((double) i / num_pts * 360), 100, 100).ToColor();

						using (var brush = new PathGradientBrush(wheel_path))
						{
							brush.CenterColor = SystemColors.Window;
							brush.SurroundColors = surround_colors;
							brush.FocusScales = new PointF(100.0f, 100.0f);
							g.FillEllipse(brush, rect);
						}
						using (var brush = new SolidBrush(Color.White))
							g.FillEllipse(brush, new Rectangle(width, width, size - width * 2, size - width * 2));

						//replace all the white with color.transparent :)
						b.MakeTransparent(Color.White);
					}
				}
				return ColorPickUtil.ResizeImage(b, size / m);
			}
		}

		public static Bitmap ColorTriangle(int angle, int size, int multisampling)
		{
			size = size * multisampling;
			using (var b = new Bitmap(size, size))
			using (Graphics g = Graphics.FromImage(b))
			{
				var gp = new GraphicsPath();

				var origin = new Point(size / 2, size / 2);
				var first = new PointF(size, size / 2.0f);
				gp.AddPolygon(new PointF[3]
				              {
				              	first, ColorPickUtil.RotatePoint(first, origin, ColorPickUtil.DegreeToRadian(120)),
				              	ColorPickUtil.RotatePoint(first, origin, ColorPickUtil.DegreeToRadian(240))
				              });

				using (var pgb = new PathGradientBrush(gp))
				{
					pgb.SurroundColors = new[] {Color.Black, Color.White};
					pgb.CenterPoint = ColorPickUtil.RotatePoint(first, origin, ColorPickUtil.DegreeToRadian(120));
					pgb.CenterColor = ColorManager.HSVtoRGB((short) angle, 100, 100, 255);
					g.FillPath(pgb, gp);
				}

				return ColorPickUtil.ResizeImage(ColorPickUtil.rotateImage(b, (float) angle - 120), size / multisampling);
			}
		}

		//2-3x multisampling recommended ;)
	}
}