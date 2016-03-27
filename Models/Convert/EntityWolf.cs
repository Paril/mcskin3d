#if CONVERT_MODELS
using System;

namespace MCSkin3D.Models.Convert
{
	internal class EntityWolf : EntityLivingBase
	{
		internal bool isAngry()
		{
			return false;
		}

		internal bool isSitting()
		{
			return false;
		}

		internal int getInterestedAngle(float partialTickTime)
		{
			return 0;
		}

		internal int getShakeAngle(float partialTickTime, float v)
		{
			return 0;
		}
	}
}
#endif