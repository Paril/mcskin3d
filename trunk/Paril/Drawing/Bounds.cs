using OpenTK;

namespace Paril.Drawing
{
	public struct Bounds3
	{
		public static readonly Bounds3 EmptyBounds = new Bounds3(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
		                                                         new Vector3(float.MinValue, float.MinValue, float.MinValue));

		public Vector3 Maxs;
		public Vector3 Mins;

		public Bounds3(Vector3 mins, Vector3 maxs)
		{
			Mins = mins;
			Maxs = maxs;
		}

		public Vector3 Center
		{
			get { return Mins + ((Maxs - Mins) / 2); }
		}

		public static Bounds3 operator +(Bounds3 left, Vector3 right)
		{
			if (right.X < left.Mins.X)
				left.Mins.X = right.X;
			if (right.Y < left.Mins.Y)
				left.Mins.Y = right.Y;
			if (right.Z < left.Mins.Z)
				left.Mins.Z = right.Z;

			if (right.X > left.Maxs.X)
				left.Maxs.X = right.X;
			if (right.Y > left.Maxs.Y)
				left.Maxs.Y = right.Y;
			if (right.Z > left.Maxs.Z)
				left.Maxs.Z = right.Z;

			return left;
		}

		public static Bounds3 operator +(Bounds3 left, Bounds3 right)
		{
			return (left + right.Mins) + right.Maxs;
		}
	}
}