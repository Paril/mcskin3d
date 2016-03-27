#if CONVERT_MODELS
using System;

namespace MCSkin3D.Models.Convert
{
	internal class EntityDragon : Entity
	{
		internal float animTime = 0;
		internal float prevAnimTime = 0;

		internal double[] getMovementOffsets(int v, float partialTicks)
		{
			return new double[] { 0, 0, 0, 0, 0, 0 };
		}

		internal float func_184667_a(int i, double[] adouble, double[] adouble1)
		{
			return 0;
		}
	}
}
#endif