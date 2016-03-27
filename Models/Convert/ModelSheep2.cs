#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelSheep2 : ModelQuadruped
	{
		private float headRotationAngleX;

		public ModelSheep2() :
			base(12, 0.0F)
		{
			this.boxList.Remove(this.head);
			this.boxList.Remove(this.body);

			this.head = new ModelRenderer(this, 0, 0);
			this.head.addBox(-3.0F, -4.0F, -6.0F, 6, 6, 8, 0.0F, "Head");
			this.head.setRotationPoint(0.0F, 6.0F, -8.0F);
			this.body = new ModelRenderer(this, 28, 8);
			this.body.addBox(-4.0F, -10.0F, -7.0F, 8, 16, 6, 0.0F, "Body");
			this.body.setRotationPoint(0.0F, 5.0F, 2.0F);
		}

#if RENDER
		/**
		 * Used for easily adding entity-dependent animations. The second and third float params here are the same second
		 * and third as in the setRotationAngles method.
		 */
		public void setLivingAnimations(EntityLivingBase entitylivingbaseIn, float p_78086_2_, float p_78086_3_, float partialTickTime)
		{
			base.setLivingAnimations(entitylivingbaseIn, p_78086_2_, p_78086_3_, partialTickTime);
			this.head.rotationPointY = 6.0F + ((EntitySheep)entitylivingbaseIn).getHeadRotationPointY(partialTickTime) * 9.0F;
			this.headRotationAngleX = ((EntitySheep)entitylivingbaseIn).getHeadRotationAngleX(partialTickTime);
		}
#endif

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public override void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			base.setRotationAngles(limbSwing, limbSwingAmount, ageInTicks, netHeadYaw, headPitch, scaleFactor, entityIn);
			this.head.rotateAngleX = this.headRotationAngleX;
		}
	}
}
#endif