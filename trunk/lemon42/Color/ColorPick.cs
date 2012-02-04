using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D.lemon42
{
    //ColorPick is an alternative to the old ColorSquare.
    //It is part based on ColorSquare but replaces it with a nice
    //and sleek HSV triangle picker.

    public partial class ColorPick : UserControl
    {
        public ColorPick()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserMouse |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            thickness = (short)(14);
            wheel = ColorPickRenderer.ColorWheel(Width, thickness, 4);
            base.OnSizeChanged(e);
        }

        short _currentHue, _currentSat, _currentVal, _currentAlpha = 255; //hue [0-360], sat&val [0-100]
        short thickness;
        bool clickCircle, clickTriangle, clickNothing;
        bool rawCircle, rawTriangle, rawInner;
        Point clickPoint;
        bool drawPoint, rotatePoint;
        short initRot;
        Bitmap wheel;

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
            get { return new ColorManager.HSVColor(_currentHue, (byte)_currentSat, (byte)_currentVal, (byte)_currentAlpha); }
            set
            {
                _currentHue = value.H;
                _currentSat = value.S;
                _currentVal = value.V;
                _currentAlpha = value.A;

                OnHSVChanged(EventArgs.Empty);
            }
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
            using (Bitmap triangle = ColorPickRenderer.ColorTriangle(_currentHue, Width - thickness * 2, 3))
            {
                g.DrawImageUnscaled(triangle, thickness, thickness);
            }
            g.DrawImageUnscaled(wheel, 0, 0);
            if (drawPoint)
            {
                Point p = ColorPickUtil.RotatePoint(clickPoint, new Point(Width/2, Width/2), ColorPickUtil.DegreeToRadian(rotatePoint ? _currentHue - initRot : 0));
                g.DrawEllipse(new Pen(Negative(ColorHSVForLocation(p.X, p.Y, _currentHue))), new Rectangle(p.X - 2, p.Y - 2, 4, 4));
                
            }
          
        }
        private double AngleFromPoints(Point p2)
        {
            Point p = new Point(Width / 2, Width / 2);
            return Math.Atan2(p2.Y - p.Y, p2.X - p.X)*180/Math.PI;
        }
        void CheckMouse(MouseEventArgs e)
        {
            if ((MouseButtons & System.Windows.Forms.MouseButtons.Left) != 0)
            {
                if (!clickNothing)
                {
                if (clickCircle){
                    _currentHue = (short)WrapDegree((int)AngleFromPoints(e.Location));
                    rotatePoint = true;
                }
                }
                if (!drawPoint) { drawPoint = clickTriangle; }
                if (clickTriangle)
                {
                    clickPoint = ClipPoint(e.Location, _currentHue);
                    rotatePoint = false;
                    initRot = _currentHue;
                }

                ColorManager.HSVColor hsv = ColorHSVForLocation(clickPoint.X, clickPoint.Y, _currentHue);
                CurrentHSV = hsv;
            }
        }
        public int WrapDegree(int angle)
        {
            return (angle < 0 || angle > 360 ? angle - 360 * (int)Math.Floor((double)angle / 360) : angle);
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
            {
                clickNothing = !rawTriangle;
            }
            else
            {
                clickNothing = !rawCircle && !rawTriangle && !rawInner;
            }
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

        public ColorManager.HSVColor ColorHSVForLocation(int x, int y, short angle)
        {
            Point p = ColorPickUtil.RotatePoint(new Point(x, y), new Point(Width / 2, Width / 2), ColorPickUtil.DegreeToRadian(-angle));
            Point[] polygon = Triangle(0);
            short hue = angle;
            byte saturation = (byte)(Math.Round((double)(p.X - polygon[1].X) / (polygon[0].X - polygon[1].X) * 100)); // left 0 - right 100
            if (p.X < polygon[1].X || saturation < 0 || saturation == 255) { saturation = 0; }
            if (p.X > polygon[0].X || saturation > 100) { saturation = 100; }
            //Y calcs
            int width = (int)(polygon[0].X - polygon[1].X);
            int height = (int)(polygon[1].Y - polygon[2].Y);
            int maxY = (int)(((polygon[0].X - p.X) / (float)width) * (float)height);
            int min = polygon[0].Y - (maxY - (maxY / 2));
            int max =  polygon[0].Y  + maxY - (maxY / 2);
            int valueRaw = ColorPickUtil.RotatePoint(new Point(x, y), new Point(Width / 2, Width / 2), ColorPickUtil.DegreeToRadian(-angle +30)).Y;
            byte value = (byte)(Math.Round((double)(valueRaw - thickness) / (Triangle(30)[1].Y - thickness) * 100));
            if (max == min) { value = 100; } if (valueRaw > Triangle(30)[1].Y) { value = 100; } if (valueRaw - thickness < 0) { value = 0; }
            return new ColorManager.HSVColor(hue, saturation, value, (byte)_currentAlpha);
        }

        private void ColorPick_Load(object sender, EventArgs e)
        {
            wheel = ColorPickRenderer.ColorWheel(Width, 14, 4);
        }   
        //check if point in circle
        public bool InCircle(Point p, Point center, int radius)
        {
            return Math.Pow(p.X - center.X, 2) + Math.Pow(p.Y - center.Y, 2) <= Math.Pow(radius,2);
        }
        //check if point in triangle
        public int dot(Point p1, Point p2) {
            return p1.X*p2.X + p1.Y * p2.Y;
        }
        public Point sub(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
        public Point add(Point p1, Point p2) {
             return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }
        public bool InTriangle(Point p, Point[] t) {
            Point v0 = sub(t[1], t[0]);
            Point v1 = sub(t[2], t[0]);
            Point v2 = sub(p, t[0]);

            int dot00 = dot(v0, v0);
            int dot01 = dot(v0, v1);
            int dot02 = dot(v0, v2);
            int dot11 = dot(v1, v1);
            int dot12 = dot(v1, v2);

double invDenom =(double) 1 / (dot00 * dot11 - dot01 * dot01);
double u = ((double)dot11 * dot02 - dot01 * dot12) * (double)invDenom;
double v = ((double)dot00 * dot12 - dot01 * dot02) * (double)invDenom;

return (u >= 0) && (v >= 0) && (u + v < 1);

    }
        public Point[] Triangle(int angle)
        {
            Point first = ColorPickUtil.RotatePoint(new Point(Width - thickness, (Width - thickness) / 2), new Point(Width / 2, Width / 2), ColorPickUtil.DegreeToRadian(angle+7));
            return  new Point[3] { first, ColorPickUtil.RotatePoint(first, new Point(Width / 2, Width / 2), ColorPickUtil.DegreeToRadian(120)), ColorPickUtil.RotatePoint(first, new Point(Width / 2, Width / 2), ColorPickUtil.DegreeToRadian(240)) };
        }
        public Point ClipPoint(Point p, int angle)
        {
            if (InTriangle(p, Triangle(angle)))
            {
                return p;
            }
            else
            {
                Point _clickPosition = ColorPickUtil.RotatePoint(p, new Point(Width/2, Width/2), ColorPickUtil.DegreeToRadian(-angle));
                Point[] polygon = Triangle(0);
                int width = (int)(polygon[0].X - polygon[1].X);
                int height = (int)(polygon[1].Y - polygon[2].Y);

                Point offsetFromPoint = new Point((int)polygon[0].X - _clickPosition.X, (int)polygon[0].Y - _clickPosition.Y);

                int maxY = (int)(((polygon[0].X - _clickPosition.X) / (float)width) * (float)height);
                int highestY = (int)(((polygon[0].X - polygon[1].X) / (float)width) * (float)height);

                Point clippedPosition = _clickPosition;

                if (offsetFromPoint.X > width)
                {
                    clippedPosition.X = (int)polygon[1].X;
                    maxY = highestY;
                }

                if (offsetFromPoint.Y > maxY - (maxY / 2))
                    clippedPosition.Y = (int)(polygon[0].Y - (maxY - (maxY / 2)));
                else if (offsetFromPoint.Y < -(maxY - (maxY / 2)))
                    clippedPosition.Y = (int)(polygon[0].Y + (maxY - (maxY / 2)));

                if (clippedPosition.X > Width - thickness) {
                    clippedPosition.X = Width - thickness;
                    clippedPosition.Y = Width / 2;
                }

                return ColorPickUtil.RotatePoint(clippedPosition, new Point(Width / 2, Width / 2), ColorPickUtil.DegreeToRadian(angle));
            }
            }
        public static Color Negative(ColorManager.HSVColor c)
        {
            if (c.V <= 60)
            {
                return Color.White;
            }
            else
            {
                return Color.Black;
            }
        }
    }
    
    public class ColorPickRenderer
    {
        public static Bitmap ColorWheel(int size, int width, int multisampling) //4x multisampling recomended for optimum quality and speed.
        {
            if (width < 1 || size < 1) { return null; }
            int m = multisampling;
            size = size * m;
            width = width * m;
            using (Bitmap b = new Bitmap(size, size))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    Rectangle rect = new Rectangle(0, 0, size-m, size-m);
                    using (GraphicsPath wheel_path = new GraphicsPath())
                    {
                        wheel_path.AddEllipse(rect);
                        wheel_path.Flatten();
                        int num_pts = (wheel_path.PointCount - 1);
                        Color[] surround_colors = new Color[wheel_path.PointCount];
                        for (int i = 0; i < wheel_path.PointCount; i++)
                        {
                            surround_colors[i] = new ColorManager.HSVColor((short)((double)i / num_pts * 360), (byte)100, (byte)100).ToColor();
                        }
                        using (PathGradientBrush brush = new PathGradientBrush(wheel_path))
                        {
                            brush.CenterColor = SystemColors.Window;
                            brush.SurroundColors = surround_colors;
                            brush.FocusScales = new PointF(100.0f, 100.0f);
                            g.FillEllipse(brush, rect);
                        }
                        using (SolidBrush brush = new SolidBrush(Color.White))
                        {
                            g.FillEllipse(brush, new Rectangle(width, width, size - width * 2, size - width * 2));
                        }
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
            using (Bitmap b = new Bitmap(size, size))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    GraphicsPath gp = new GraphicsPath();
                    
                    Point origin = new Point(size / 2, size / 2);
                    PointF first = new PointF(size, size / 2.0f);
                    gp.AddPolygon(new PointF[3] { first, ColorPickUtil.RotatePoint(first, origin, ColorPickUtil.DegreeToRadian(120)), ColorPickUtil.RotatePoint(first, origin, ColorPickUtil.DegreeToRadian(240)) });

                    using (PathGradientBrush pgb = new PathGradientBrush(gp))
                    {
                        pgb.SurroundColors = new Color[] { Color.Black, Color.White };
                        pgb.CenterPoint = ColorPickUtil.RotatePoint(first, origin, ColorPickUtil.DegreeToRadian(120));
                        pgb.CenterColor = ColorManager.HSVtoRGB((short)angle, (byte)100, (byte)100, (byte)255);
                        g.FillPath(pgb, gp);

                    }
                    return ColorPickUtil.ResizeImage(ColorPickUtil.rotateImage(b, (float)angle - 120), size / multisampling);
                    
                }
            }
        } //3x multisampling recommended ;)
    }
}
