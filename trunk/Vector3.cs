using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCSkin3D;

namespace VCSkinner
{
	public struct Vector3
	{
		public static readonly Vector3 Empty = new Vector3(0, 0, 0);
		public float x, y, z;

		public Vector3(float _x, float _y, float _z)
		{
			x = _x; y = _y; z = _z;

			if (float.IsNaN(x))
				x = 0;
			else if (float.IsPositiveInfinity(x))
				x = 0;
			else if (float.IsNegativeInfinity(x))
				x = 0;

			if (float.IsNaN(y))
				y = 0;
			else if (float.IsPositiveInfinity(y))
				y = 0;
			else if (float.IsNegativeInfinity(y))
				y = 0;

			if (float.IsNaN(z))
				z = 0;
			else if (float.IsPositiveInfinity(z))
				z = 0;
			else if (float.IsNegativeInfinity(z))
				z = 0;
		}
		public Vector3(Vector3 Src)
		{
			x = Src.x; y = Src.y; z = Src.z;
			if (float.IsNaN(x))
				x = 0;
			else if (float.IsPositiveInfinity(x))
				x = 1;
			else if (float.IsNegativeInfinity(x))
				x = -1;

			if (float.IsNaN(y))
				y = 0;
			else if (float.IsPositiveInfinity(y))
				y = 1;
			else if (float.IsNegativeInfinity(y))
				y = -1;

			if (float.IsNaN(z))
				z = 0;
			else if (float.IsPositiveInfinity(z))
				z = 1;
			else if (float.IsNegativeInfinity(z))
				z = -1;
		}

		public void Set(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
		public void Set(Vector3 v) { Set(v.x, v.y, v.z); }
		// These are functions that perform math operations using Vector3

		public float DotProduct(Vector3 B) { return x * B.x + y * B.y + z * B.z; }
		public Vector3 CrossProduct(Vector3 B) { return new Vector3(y * B.z - z * B.y, z * B.x - x * B.z, x * B.y - y * B.x); }

		public void Normalize() { float l = Length(); x /= l; y /= l; z /= l; }
		public float Length() { return (float)Math.Sqrt(x * x + y * y + z * z); }

		public float LengthSq()
		{
			return (x * x) + (y * y) + (z * z);
		}
		public static void between(Vector3 v1, Vector3 v2, Vector3 v, float pct, float wid)
		{
			if (pct != -1)
				pct /= 100.0f;
			else
			{
				float dist = (float)(Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z)));
				pct = wid / dist;
			}
			v.x = pct * (v2.x - v1.x) + v1.x;
			v.y = pct * (v2.y - v1.y) + v1.y;
			v.z = pct * (v2.z - v1.z) + v1.z;
		}

		// These operational overload functions hide the math functions from the rest of the code,
		// making the code more readable. Note that the Dot Product is represented by the
		// multiplication operator, and the Cross Product is represented by the division operator.
		public static Vector3 operator +(Vector3 lv, Vector3 rv) { return new Vector3(lv.x + rv.x, lv.y + rv.y, lv.z + rv.z); }
		public static Vector3 operator +(Vector3 lv, float rv) { return new Vector3(lv.x + rv, lv.y + rv, lv.z + rv); }
		public static Vector3 operator -(Vector3 lv, Vector3 rv) { return new Vector3(lv.x - rv.x, lv.y - rv.y, lv.z - rv.z); }
		public static Vector3 operator *(Vector3 lv, Vector3 rv) { return new Vector3(lv.x * rv.x, lv.y * rv.y, lv.z * rv.z); }
		public static Vector3 operator *(Vector3 v, float a) { return new Vector3(v.x * a, v.y * a, v.z * a); }
		public static Vector3 operator *(float a, Vector3 v) { return v * a; }
		public static Vector3 operator /(Vector3 v, float a) { return new Vector3(v.x / a, v.y / a, v.z / a); }
		public static Vector3 operator /(Vector3 lv, Vector3 rv) { return new Vector3(lv.x / rv.x, lv.y / rv.y, lv.z / rv.z); }

		public static bool operator==(Vector3 lv, Vector3 rv)
		{
			return (lv.x == rv.x && lv.y == rv.y && lv.z == rv.z);
		}

		public static bool operator!=(Vector3 lv, Vector3 rv)
		{
			return !(lv == rv);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static Vector3 operator-(Vector3 v)
		{
			return new Vector3(-v.x, -v.y, -v.z);
		}

		// In case some one want to use Vector3 as if it were the old TVector type, we use this
		// operational overload to simulate an array of float variables.
		public float this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return x;
				case 1:
					return y;
				case 2:
					return z;
				}

				throw new InvalidOperationException("Vector3[] i > 3");
			}

			set
			{
				switch (i)
				{
				case 0:
					x = value;
					return;
				case 1:
					y = value;
					return;
				case 2:
					z = value;
					return;
				}

				throw new InvalidOperationException("Vector3[] i > 3");
			}
		}

		static void RotateWithVector(Vector3 x, Vector3 u, float ang, Vector3 Res)
		{
			Vector3 h, v, uxx;
			uxx = (u.CrossProduct(x)) * (float)Math.Sin(ang);
			h = u * (x * u);
			v = (x - h) * (float)Math.Cos(ang);
			Res = (h + v) + uxx;
		}

		public Vector3 ToAngles()
		{
			float yaw, pitch;

			if (y == 0 && x == 0)
			{
				yaw = 0;
				if (z > 0)
					pitch = 90;
				else
					pitch = 270;
			}
			else
			{
				if (x != 0)
					yaw = (float)(Math.Atan2(y, x) * (180 / Math.PI));
				else if (y > 0)
					yaw = 90;
				else
					yaw = 270;

				if (yaw < 0)
					yaw += 360;

				pitch = (float)(Math.Atan2(z, Math.Sqrt(x * x + y * y)) * (180 / Math.PI));
				if (pitch < 0)
					pitch += 360;
			}

			return new Vector3(-pitch, yaw, 0);
		}

		public override string ToString()
		{
			return x.ToString() + " " + y.ToString() + " " + z.ToString();
		}

		public void ToVectors(out Vector3 forward, out Vector3 right, out Vector3 up)
		{
			float sp = (float)Math.Sin(VCMath.degreesToRadians(x)), cp = (float)Math.Cos(VCMath.degreesToRadians(x));
			float sy = (float)Math.Sin(VCMath.degreesToRadians(y)), cy = (float)Math.Cos(VCMath.degreesToRadians(y));
			float sr = (float)Math.Sin(VCMath.degreesToRadians(z)), cr = (float)Math.Cos(VCMath.degreesToRadians(z));

			forward = new Vector3(cp* cy, cp*sy, -sp);
			right = new Vector3(-1 * sr * sp * cy + -1 * cr * -sy,
							-1 * sr * sp * sy + -1 * cr * cy,
							-1 * sr * cp);
			up = new Vector3(cr * sp * cy + -sr * -sy,
							cr * sp * sy + -sr * cy,
							cr * cp);
		}

		public Vector3 MultiplyAngles(float scale, Vector3 b)
		{
			return new Vector3(x + b.x * scale,
							y + b.y * scale,
							z + b.z * scale);
		}

		public void Read(System.IO.BinaryReader reader)
		{
			x = reader.ReadSingle();
			y = reader.ReadSingle();
			z = reader.ReadSingle();
		}

		public void ReadCompressed(System.IO.BinaryReader reader)
		{
			x = ((float)reader.ReadInt16()) * (1.0f / 64);
			y = ((float)reader.ReadInt16()) * (1.0f / 64);
			z = ((float)reader.ReadInt16()) * (1.0f / 64);
		}

		public void Write(System.IO.BinaryWriter writer)
		{
			writer.Write(x);
			writer.Write(y);
			writer.Write(z);
		}

		public void WriteCompressed(System.IO.BinaryWriter writer)
		{
			writer.Write((short)(x / (1.0f / 64)));
			writer.Write((short)(y / (1.0f / 64)));
			writer.Write((short)(z / (1.0f / 64)));
		}

		public float DistSquared(Vector3 v2)
		{
			return (x-v2.x) * (x-v2.x) + (y-v2.y) * (y-v2.y) + (z-v2.z) * (z-v2.z);
		}

		public float Dist(Vector3 v2)
		{
			return (float)Math.Sqrt(DistSquared(v2));
		}

		public void MakeNormalVectors(ref Vector3 right, ref Vector3 up)
		{
			// This rotate and negate guarantees a vector not colinear with the original
			right.Set(z, -x, y);

			float d = right.DotProduct(this);
			right = right.MultiplyAngles(-d, this);
			right.Normalize();
			up = right.CrossProduct(this);
		}

		public Vector3 Rotate(Vector3 Dir, float Degrees)
		{
			Vector3 dest = Vector3.Empty;

			float c = (float)Math.Cos(VCMath.degreesToRadians(Degrees)), s = (float)Math.Sin(VCMath.degreesToRadians(Degrees));

			Vector3 vr = Vector3.Empty, vu = Vector3.Empty;
			Dir.MakeNormalVectors(ref vr, ref vu);

			float t0, t1;
			t0 = vr.x * c + vu.x * -s;
			t1 = vr.x * s + vu.x *  c;
			dest.x = (t0 * vr.x + t1 * vu.x + Dir.x * Dir.x) * x
						+ (t0 * vr.y + t1 * vu.y + Dir.x * Dir.y) * y
						+ (t0 * vr.z + t1 * vu.z + Dir.x * Dir.z) * z;

			t0 = vr.y * c + vu.y * -s;
			t1 = vr.y * s + vu.y *  c;
			dest.y = (t0 * vr.x + t1 * vu.x + Dir.y * Dir.x) * x
						+ (t0 * vr.y + t1 * vu.y + Dir.y * Dir.y) * y
						+ (t0 * vr.z + t1 * vu.z + Dir.y * Dir.z) * z;

			t0 = vr.z * c + vu.z * -s;
			t1 = vr.z * s + vu.z *  c;
			dest.z = (t0 * vr.x + t1 * vu.x + Dir.z * Dir.x) * x
						+ (t0 * vr.y + t1 * vu.y + Dir.z * Dir.y) * y
						+ (t0 * vr.z + t1 * vu.z + Dir.z * Dir.z) * z;

			return dest;
		}
	}
}
