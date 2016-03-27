#if CONVERT_MODELS
using System;

namespace MCSkin3D.Models.Convert
{
	internal class EntityHorse : EntityLivingBase
	{
		internal int field_110278_bp;

		internal boolean isBeingRidden()
		{
			return false;
		}

		internal boolean isHorseSaddled()
		{
			return false;
		}

		internal float getMouthOpennessAngle(float partialTickTime)
		{
			return 0;
		}

		internal float getRearingAmount(float partialTickTime)
		{
			return 0;
		}

		internal float getGrassEatingAmount(float partialTickTime)
		{
			return 0;
		}
	}
}
#endif