using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VCSkinner;

namespace MCSkin3D
{
	public class VCMath
	{
		public const float EPSILON = 1e-6f;

		static public bool closeEnough(float f1, float f2)
		{
			// Determines whether the two floating-point values f1 and f2 are
			// close enough together that they can be considered equal.

			return Math.Abs((f1 - f2) / ((f2 == 0.0f) ? 1.0f : f2)) < EPSILON;
		}

		static public float degreesToRadians(float degrees)
		{
			return (float)((degrees * Math.PI) / 180.0f);
		}

		static public float radiansToDegrees(float radians)
		{
			return (float)((radians * 180.0f) / Math.PI);
		}
	};
	//-----------------------------------------------------------------------------
	// This Quaternion class will concatenate quaternions in a left to right order.
	// The reason for this is to maintain the same multiplication semantics as the
	// Matrix3 and Matrix4 classes.

	public class Quaternion
	{
		public static Quaternion IDENTITY = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);
		public float w, x, y, z;

		public static Quaternion slerp(Quaternion a, Quaternion b, float t)
		{
			// Smoothly interpolates from quaternion 'a' to quaternion 'b' using
			// spherical linear interpolation.
			// 
			// Both quaternions must be unit length and represent absolute rotations.
			// In particular quaternion 'b' must not be relative to quaternion 'a'.
			// If 'b' is relative to 'a' make 'b' an absolute rotation by: b = a * b.
			// 
			// The interpolation parameter 't' is in the range [0,1]. When t = 0 the
			// resulting quaternion will be 'a'. When t = 1 the resulting quaternion
			// will be 'b'.
			//
			// The algorithm used is adapted from Allan and Mark Watt's "Advanced
			// Animation and Rendering Techniques" (ACM Press 1992).

			Quaternion result = new Quaternion();
			float omega = 0.0f;
			float cosom = (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a.w * b.w);
			float sinom = 0.0f;
			float scale0 = 0.0f;
			float scale1 = 0.0f;

			if ((1.0f + cosom) > VCMath.EPSILON)
			{
				// 'a' and 'b' quaternions are not opposite each other.

				if ((1.0f - cosom) > VCMath.EPSILON)
				{
					// Standard case - slerp.
					omega = (float)Math.Acos(cosom);
					sinom = (float)Math.Sin(omega);
					scale0 = (float)Math.Sin((1.0f - t) * omega) / sinom;
					scale1 = (float)Math.Sin(t * omega) / sinom;
				}
				else
				{
					// 'a' and 'b' quaternions are very close so lerp instead.
					scale0 = 1.0f - t;
					scale1 = t;
				}

				result.x = scale0 * a.x + scale1 * b.x;
				result.y = scale0 * a.y + scale1 * b.y;
				result.z = scale0 * a.z + scale1 * b.z;
				result.w = scale0 * a.w + scale1 * b.w;
			}
			else
			{
				// 'a' and 'b' quaternions are opposite each other.

				result.x = -b.y;
				result.y = b.x;
				result.z = -b.w;
				result.w = b.z;

				scale0 = (float)Math.Sin((1.0f - t) - (Math.PI / 2));
				scale1 = (float)Math.Sin(t * (Math.PI / 2));

				result.x = scale0 * a.x + scale1 * result.x;
				result.y = scale0 * a.y + scale1 * result.y;
				result.z = scale0 * a.z + scale1 * result.z;
				result.w = scale0 * a.w + scale1 * result.w;
			}

			return result;
		}

		public Quaternion() { }

		public Quaternion(float w_, float x_, float y_, float z_)
		{
			w = w_;
			x = x_;
			y = y_;
			z = z_;
		}

		public Quaternion(float headDegrees, float pitchDegrees, float rollDegrees)
		{
			fromHeadPitchRoll(headDegrees, pitchDegrees, rollDegrees);
		}

		public Quaternion(Vector3 axis, float degrees)
		{
			fromAxisAngle(axis, degrees);
		}

		public Quaternion(Matrix3 m)
		{
			fromMatrix(m);
		}

		public Quaternion(Matrix4 m)
		{
			fromMatrix(m);
		}

		public Quaternion(Quaternion q)
		{
			x = q.x;
			y = q.y;
			z = q.z;
			w = q.w;
		}

		~Quaternion() { }

		public static Quaternion operator*(float lhs, Quaternion rhs)
		{
			return rhs * lhs;
		}

		public static bool operator==(Quaternion lhs, Quaternion rhs)
		{
			return VCMath.closeEnough(lhs.w, rhs.w) && VCMath.closeEnough(lhs.x, rhs.x) &&
				 VCMath.closeEnough(lhs.y, rhs.y) && VCMath.closeEnough(lhs.z, rhs.z);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator!=(Quaternion lhs, Quaternion rhs)
		{
			return !(lhs == rhs);
		}

		public static Quaternion operator+(Quaternion lhs, Quaternion rhs)
		{
			Quaternion newQuat = new Quaternion(lhs);
			newQuat.w += rhs.w; newQuat.x += rhs.x; newQuat.y += rhs.y; newQuat.z += rhs.z;
			return newQuat;
		}

		public static Quaternion operator-(Quaternion lhs, Quaternion rhs)
		{
			Quaternion newQuat = new Quaternion(lhs);
			newQuat.w -= rhs.w; newQuat.x -= rhs.x; newQuat.y -= rhs.y; newQuat.z -= rhs.z;
			return newQuat;
		}

		public static Quaternion operator*(Quaternion lhs, Quaternion rhs)
		{
			// Multiply so that rotations are applied in a left to right order.
			return new Quaternion(
				(lhs.w * rhs.w) - (lhs.x * rhs.x) - (lhs.y * rhs.y) - (lhs.z * rhs.z),
				(lhs.w * rhs.x) + (lhs.x * rhs.w) - (lhs.y * rhs.z) + (lhs.z * rhs.y),
				(lhs.w * rhs.y) + (lhs.x * rhs.z) + (lhs.y * rhs.w) - (lhs.z * rhs.x),
				(lhs.w * rhs.z) - (lhs.x * rhs.y) + (lhs.y * rhs.x) + (lhs.z * rhs.w)); ;
		}

		public static Quaternion operator*(Quaternion lhs, float scalar)
		{
			Quaternion newQ = new Quaternion(lhs);
			newQ.w *= scalar;
			newQ.x *= scalar;
			newQ.y *= scalar;
			newQ.z *= scalar;
			return newQ;
		}

		public static Quaternion operator/(Quaternion lhs, float scalar)
		{
			Quaternion newQ = new Quaternion(lhs);
			newQ.w /= scalar;
			newQ.x /= scalar;
			newQ.y /= scalar;
			newQ.z /= scalar;
			return newQ;
		}

		public Quaternion conjugate()
		{
			return new Quaternion(w, -x, -y, -z);
		}

		public void fromAxisAngle(Vector3 axis, float degrees)
		{
			float halfTheta = VCMath.degreesToRadians(degrees) * 0.5f;
			float s = (float)Math.Sin(halfTheta);
			w = (float)Math.Cos(halfTheta);
			x = axis.x * s;
			y = axis.y * s;
			z = axis.z * s;
		}

		public void fromHeadPitchRoll(float headDegrees, float pitchDegrees, float rollDegrees)
		{
			Matrix3 m = new Matrix3();
			m.fromHeadPitchRoll(headDegrees, pitchDegrees, rollDegrees);
			fromMatrix(m);
		}

		public void fromMatrix(Matrix3 m)
		{
			// Creates a quaternion from a rotation matrix. 
			// The algorithm used is from Allan and Mark Watt's "Advanced 
			// Animation and Rendering Techniques" (ACM Press 1992).

			float s = 0.0f;
			float[] q = new float[4];
			q[0] = q[1] = q[2] = q[3] = 0.0f;
			float trace = m[0, 0] + m[1, 1] + m[2, 2];

			if (trace > 0.0f)
			{
				s = (float)Math.Sqrt(trace + 1.0f);
				q[3] = s * 0.5f;
				s = 0.5f / s;
				q[0] = (m[1, 2] - m[2, 1]) * s;
				q[1] = (m[2, 0] - m[0, 2]) * s;
				q[2] = (m[0, 1] - m[1, 0]) * s;
			}
			else
			{
				int[] nxt = new int[3] { 1, 2, 0 };
				int i = 0, j = 0, k = 0;

				if (m[1, 1] > m[0, 0])
					i = 1;

				if (m[2, 2] > m[i, i])
					i = 2;

				j = nxt[i];
				k = nxt[j];
				s = (float)Math.Sqrt((m[i, i] - (m[j, j] + m[k, k])) + 1.0f);

				q[i] = s * 0.5f;
				s = 0.5f / s;
				q[3] = (m[j, k] - m[k, j]) * s;
				q[j] = (m[i, j] + m[j, i]) * s;
				q[k] = (m[i, k] + m[k, i]) * s;
			}

			x = q[0];
			y = q[1];
			z = q[2];
			w = q[3];
		}

		public void fromMatrix(Matrix4 m)
		{
			// Creates a quaternion from a rotation matrix. 
			// The algorithm used is from Allan and Mark Watt's "Advanced 
			// Animation and Rendering Techniques" (ACM Press 1992).

			float s = 0.0f;
			float[] q = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
			float trace = m[0, 0] + m[1, 1] + m[2, 2];

			if (trace > 0.0f)
			{
				s = (float)Math.Sqrt(trace + 1.0f);
				q[3] = s * 0.5f;
				s = 0.5f / s;
				q[0] = (m[1, 2] - m[2, 1]) * s;
				q[1] = (m[2, 0] - m[0, 2]) * s;
				q[2] = (m[0, 1] - m[1, 0]) * s;
			}
			else
			{
				int[] nxt = new int[3] { 1, 2, 0 };
				int i = 0, j = 0, k = 0;

				if (m[1, 1] > m[0, 0])
					i = 1;

				if (m[2, 2] > m[i, i])
					i = 2;

				j = nxt[i];
				k = nxt[j];
				s = (float)Math.Sqrt((m[i, i] - (m[j, j] + m[k, k])) + 1.0f);

				q[i] = s * 0.5f;
				s = 0.5f / s;
				q[3] = (m[j, k] - m[k, j]) * s;
				q[j] = (m[i, j] + m[j, i]) * s;
				q[k] = (m[i, k] + m[k, i]) * s;
			}

			x = q[0];
			y = q[1];
			z = q[2];
			w = q[3];
		}

		public void identity()
		{
			w = 1.0f;
			x = y = z = 0.0f;
		}

		public Quaternion inverse()
		{
			return conjugate() * (1.0f / magnitude());
		}

		public float magnitude()
		{
			return (float)Math.Sqrt(w * w + x * x + y * y + z * z);
		}

		public void normalize()
		{
			float invMag = 1.0f / magnitude();
			w *= invMag;
			x *= invMag;
			y *= invMag;
			z *= invMag;
		}

		public void set(float w_, float x_, float y_, float z_)
		{
			w = w_;
			x = x_;
			y = y_;
			z = z_;
		}

		public void toAxisAngle(Vector3 axis, float degrees)
		{
			// Converts this quaternion to an axis and an angle.

			float sinHalfThetaSq = 1.0f - w * w;

			// Guard against numerical imprecision and identity quaternions.
			if (sinHalfThetaSq <= 0.0f)
			{
				axis.x = 1.0f;
				axis.y = axis.z = 0.0f;
				degrees = 0.0f;
			}
			else
			{
				float invSinHalfTheta = 1.0f / (float)Math.Sqrt(sinHalfThetaSq);

				axis.x = x * invSinHalfTheta;
				axis.y = y * invSinHalfTheta;
				axis.z = z * invSinHalfTheta;
				degrees = VCMath.radiansToDegrees(2.0f * (float)Math.Acos(w));
			}
		}

		public void toHeadPitchRoll(ref float headDegrees, ref float pitchDegrees, ref float rollDegrees)
		{
			Matrix3 m = toMatrix3();
			m.toHeadPitchRoll(ref headDegrees, ref pitchDegrees, ref rollDegrees);
		}

		public Matrix3 toMatrix3()
		{
			// Converts this quaternion to a rotation matrix.
			//
			//  | 1 - 2(y^2 + z^2)	2(xy + wz)			2(xz - wy)		 |
			//  | 2(xy - wz)		1 - 2(x^2 + z^2)	2(yz + wx)		 |
			//  | 2(xz + wy)		2(yz - wx)			1 - 2(x^2 + y^2) |

			float x2 = x + x;
			float y2 = y + y;
			float z2 = z + z;
			float xx = x * x2;
			float xy = x * y2;
			float xz = x * z2;
			float yy = y * y2;
			float yz = y * z2;
			float zz = z * z2;
			float wx = w * x2;
			float wy = w * y2;
			float wz = w * z2;

			Matrix3 m = new Matrix3();

			m[0, 0] = 1.0f - (yy + zz);
			m[0, 1] = xy + wz;
			m[0, 2] = xz - wy;

			m[1, 0] = xy - wz;
			m[1, 1] = 1.0f - (xx + zz);
			m[1, 2] = yz + wx;

			m[2, 0] = xz + wy;
			m[2, 1] = yz - wx;
			m[2, 2] = 1.0f - (xx + yy);

			return m;
		}

		public Matrix4 toMatrix4()
		{
			// Converts this quaternion to a rotation matrix.
			//
			//  | 1 - 2(y^2 + z^2)	2(xy + wz)			2(xz - wy)			0  |
			//  | 2(xy - wz)		1 - 2(x^2 + z^2)	2(yz + wx)			0  |
			//  | 2(xz + wy)		2(yz - wx)			1 - 2(x^2 + y^2)	0  |
			//  | 0					0					0					1  |

			float x2 = x + x;
			float y2 = y + y;
			float z2 = z + z;
			float xx = x * x2;
			float xy = x * y2;
			float xz = x * z2;
			float yy = y * y2;
			float yz = y * z2;
			float zz = z * z2;
			float wx = w * x2;
			float wy = w * y2;
			float wz = w * z2;

			Matrix4 m = new Matrix4();

			m[0, 0] = 1.0f - (yy + zz);
			m[0, 1] = xy + wz;
			m[0, 2] = xz - wy;
			m[0, 3] = 0.0f;

			m[1, 0] = xy - wz;
			m[1, 1] = 1.0f - (xx + zz);
			m[1, 2] = yz + wx;
			m[1, 3] = 0.0f;

			m[2, 0] = xz + wy;
			m[2, 1] = yz - wx;
			m[2, 2] = 1.0f - (xx + yy);
			m[2, 3] = 0.0f;

			m[3, 0] = 0.0f;
			m[3, 1] = 0.0f;
			m[3, 2] = 0.0f;
			m[3, 3] = 1.0f;

			return m;
		}
	};
}
