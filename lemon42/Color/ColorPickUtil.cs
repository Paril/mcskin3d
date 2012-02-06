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
    class ColorPickUtil
    {
        public static int Distance(Point p1, Point p2)
        {
            	int xd = p2.X - p1.X;
                int yd = p2.Y - p1.Y;
	            return (int)Math.Sqrt(xd*xd + yd*yd);
        }

        public static PointF RotatePoint(PointF p, PointF p2, double angle)
        { //angle in radians.
            return new PointF((float)(Math.Cos(angle) * (p.X - p2.X) - Math.Sin(angle) * (p.Y - p2.Y) + p2.X), (float)(Math.Sin(angle) * (p.X - p2.X) + Math.Cos(angle) * (p.Y - p2.Y) + p2.Y));
        }

        public static Point RotatePoint(Point p, Point p2, double angle)
        { //angle in radians.
            return new Point((int)(Math.Cos(angle) * (p.X - p2.X) - Math.Sin(angle) * (p.Y - p2.Y) + p2.X), (int)(Math.Sin(angle) * (p.X - p2.X) + Math.Cos(angle) * (p.Y - p2.Y) + p2.Y));
        }

        public static Bitmap ResizeImage(Image image, int size)
        {
            Bitmap result = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.DrawImage(image, 0, 0, result.Width, result.Height);
            }
            return result;
        }

        public static Bitmap rotateImage(Bitmap b, float angle)
        {
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            using (Graphics g = Graphics.FromImage(returnBitmap))
            {
                g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
                g.DrawImage(b, new Point(0, 0));
            }
            return returnBitmap;
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * 180.0 / Math.PI;
        }
    }
}
