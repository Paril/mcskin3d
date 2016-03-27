#if CONVERT_MODELS
using System;

namespace MCSkin3D.Models.Convert
{
	internal class MathHelper
	{
		public static float cos(float v)
		{
			return (float)System.Math.Cos(v);
		}

		internal static float sin(float p)
		{
			return (float)System.Math.Sin(p);
		}

		internal static float sqrt_float(float f1)
		{
			return (float)System.Math.Sqrt(f1);
		}

		/**
		 * the angle is reduced to an angle between -180 and +180 by mod, and a 360 check
		 */
		internal static double wrapAngleTo180_double(double p_76138_0_)
		{
			p_76138_0_ = p_76138_0_ % 360.0D;

			if (p_76138_0_ >= 180.0D)
			{
				p_76138_0_ -= 360.0D;
			}

			if (p_76138_0_ < -180.0D)
			{
				p_76138_0_ += 360.0D;
			}

			return p_76138_0_;
		}
	}
}
#endif