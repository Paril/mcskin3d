#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSkin3D.Models.Convert
{
	public class ModelPig : ModelQuadruped
	{
		public ModelPig() :
			this(0.0F)
		{
		}

		public ModelPig(float p_i1151_1_) :
			base(6, p_i1151_1_)
		{
			this.head.setTextureOffset(16, 16).addBox(-2.0F, 0.0F, -9.0F, 4, 3, 1, p_i1151_1_, "Snout");
			this.childYOffset = 4.0F;
		}
	}
}
#endif