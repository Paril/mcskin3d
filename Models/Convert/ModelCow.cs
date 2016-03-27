#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelCow : ModelQuadruped
	{
		public ModelCow() :
			base(12, 0.0F)
		{
			this.boxList.Remove(this.head);
			this.boxList.Remove(this.body);

			this.head = new ModelRenderer(this, 0, 0);
			this.head.addBox(-4.0F, -4.0F, -6.0F, 8, 8, 6, 0.0F, "Head");
			this.head.setRotationPoint(0.0F, 4.0F, -8.0F);
			this.head.setTextureOffset(22, 0).addBox(-5.0F, -5.0F, -4.0F, 1, 3, 1, 0.0F, "Right Ear");
			this.head.setTextureOffset(22, 0).addBox(4.0F, -5.0F, -4.0F, 1, 3, 1, 0.0F, "Left Ear");
			this.body = new ModelRenderer(this, 18, 4);
			this.body.addBox(-6.0F, -10.0F, -7.0F, 12, 18, 10, 0.0F, "Body");
			this.body.setRotationPoint(0.0F, 5.0F, 2.0F);
			this.body.setTextureOffset(52, 0).addBox(-2.0F, 2.0F, -8.0F, 4, 6, 1, "Udders");
			--this.leg1.rotationPointX;
			++this.leg2.rotationPointX;
			this.leg1.rotationPointZ += 0.0F;
			this.leg2.rotationPointZ += 0.0F;
			--this.leg3.rotationPointX;
			++this.leg4.rotationPointX;
			--this.leg3.rotationPointZ;
			--this.leg4.rotationPointZ;
			this.childZOffset += 2.0F;
		}
	}
}
#endif