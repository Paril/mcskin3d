#if CONVERT_MODELS
using System;

namespace MCSkin3D.Models.Convert
{
	internal class Math
	{
		internal static readonly double PI = System.Math.PI;

		internal static double pow(double v1, double v2)
		{
			return System.Math.Pow(v1, v2);
		}

		internal static double sin(double rotateAngleX)
		{
			return System.Math.Sin(rotateAngleX);
		}

		internal static double cos(double rotateAngleY)
		{
			return System.Math.Cos(rotateAngleY);
		}

		internal static int abs(int v)
		{
			return System.Math.Abs(v);
		}

		internal static float abs(float v)
		{
			return System.Math.Abs(v);
		}

		internal static float max(float v1, float v2)
		{
			return System.Math.Max(v1, v2);
		}
	}
}
#endif