using System;

namespace MCSkin3D
{
	public enum VisiblePartFlags
	{
		HeadFlag = 1,
		HelmetFlag = 2,
		ChestFlag = 4,
		LeftArmFlag = 8,
		RightArmFlag = 16,
		LeftLegFlag = 32,
		RightLegFlag = 64,

		Default = HeadFlag | HelmetFlag | ChestFlag | LeftArmFlag | RightArmFlag | LeftLegFlag | RightLegFlag,
	}
}
