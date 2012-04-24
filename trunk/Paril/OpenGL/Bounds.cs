using System.Drawing;

namespace Paril.OpenGL
{
	public struct Bounds
	{
		private Point _maxs;
		private Point _mins;

		public Bounds(Point mins, Point maxs) :
			this()
		{
			_mins = mins;
			_maxs = maxs;
		}

		public Point Mins
		{
			get { return _mins; }
			set { _mins = value; }
		}

		public Point Maxs
		{
			get { return _maxs; }
			set { _maxs = value; }
		}

		public void AddPoint(Point p)
		{
			if (p.X < _mins.X)
				_mins.X = p.X;
			if (p.Y < _mins.Y)
				_mins.Y = p.Y;

			if (p.X > _maxs.X)
				_maxs.X = p.X;
			if (p.Y > _maxs.Y)
				_maxs.Y = p.Y;
		}

		public static Bounds operator +(Bounds left, Rectangle right)
		{
			left.AddPoint(new Point(right.Left, right.Top));
			left.AddPoint(new Point(right.Right, right.Bottom));

			return left;
		}

		public Rectangle ToRectangle()
		{
			var r = new Rectangle();

			r.X = _mins.X;
			r.Y = _mins.Y;
			r.Width = _maxs.X - _mins.X;
			r.Height = _maxs.Y - _mins.Y;

			return r;
		}
	}
}
