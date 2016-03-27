#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelSkeleton : ModelBiped
	{
		public ModelSkeleton() :
			this(0.0F, false)
		{
		}

		public ModelSkeleton(float p_i46303_1_, boolean p_i46303_2_) :
			base(p_i46303_1_, 0.0F, 64, 32)
		{
			if (!p_i46303_2_)
			{
				this.boxList.Remove(this.bipedRightArm);
				this.boxList.Remove(this.bipedLeftArm);
				this.boxList.Remove(this.bipedRightLeg);
				this.boxList.Remove(this.bipedLeftLeg);

				this.bipedRightArm = new ModelRenderer(this, 40, 16, ModelPart.RightArm);
				this.bipedRightArm.addBox(-1.0F, -2.0F, -1.0F, 2, 12, 2, p_i46303_1_, "Right Arm");
				this.bipedRightArm.setRotationPoint(-5.0F, 2.0F, 0.0F);
				this.bipedLeftArm = new ModelRenderer(this, 40, 16, ModelPart.LeftArm);
				this.bipedLeftArm.mirror = true;
				this.bipedLeftArm.addBox(-1.0F, -2.0F, -1.0F, 2, 12, 2, p_i46303_1_, "Left Arm");
				this.bipedLeftArm.setRotationPoint(5.0F, 2.0F, 0.0F);
				this.bipedRightLeg = new ModelRenderer(this, 0, 16, ModelPart.RightLeg);
				this.bipedRightLeg.addBox(-1.0F, 0.0F, -1.0F, 2, 12, 2, p_i46303_1_, "Right Leg");
				this.bipedRightLeg.setRotationPoint(-2.0F, 12.0F, 0.0F);
				this.bipedLeftLeg = new ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
				this.bipedLeftLeg.mirror = true;
				this.bipedLeftLeg.addBox(-1.0F, 0.0F, -1.0F, 2, 12, 2, p_i46303_1_, "Left Leg");
				this.bipedLeftLeg.setRotationPoint(2.0F, 12.0F, 0.0F);
			}
		}

#if RENDER
		/**
		 * Used for easily adding entity-dependent animations. The second and third float params here are the same second
		 * and third as in the setRotationAngles method.
		 */
		public void setLivingAnimations(EntityLivingBase entitylivingbaseIn, float p_78086_2_, float p_78086_3_, float partialTickTime)
		{
			this.rightArmPose = ModelBiped.ArmPose.EMPTY;
			this.leftArmPose = ModelBiped.ArmPose.EMPTY;
			ItemStack itemstack = entitylivingbaseIn.getHeldItem(EnumHand.MAIN_HAND);

			if (itemstack != null && itemstack.getItem() == Items.bow && ((EntitySkeleton)entitylivingbaseIn).func_184725_db())
			{
				if (entitylivingbaseIn.getPrimaryHand() == EnumHandSide.RIGHT)
				{
					this.rightArmPose = ModelBiped.ArmPose.BOW_AND_ARROW;
				}
				else
				{
					this.leftArmPose = ModelBiped.ArmPose.BOW_AND_ARROW;
				}
			}

			super.setLivingAnimations(entitylivingbaseIn, p_78086_2_, p_78086_3_, partialTickTime);
		}

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			super.setRotationAngles(limbSwing, limbSwingAmount, ageInTicks, netHeadYaw, headPitch, scaleFactor, entityIn);
			ItemStack itemstack = ((EntityLivingBase)entityIn).getHeldItemMainhand();
			EntitySkeleton entityskeleton = (EntitySkeleton)entityIn;

			if (entityskeleton.func_184725_db() && (itemstack == null || itemstack.getItem() != Items.bow))
			{
				float f = MathHelper.sin(this.swingProgress * (float)Math.PI);
				float f1 = MathHelper.sin((1.0F - (1.0F - this.swingProgress) * (1.0F - this.swingProgress)) * (float)Math.PI);
				this.bipedRightArm.rotateAngleZ = 0.0F;
				this.bipedLeftArm.rotateAngleZ = 0.0F;
				this.bipedRightArm.rotateAngleY = -(0.1F - f * 0.6F);
				this.bipedLeftArm.rotateAngleY = 0.1F - f * 0.6F;
				this.bipedRightArm.rotateAngleX = -((float)Math.PI / 2F);
				this.bipedLeftArm.rotateAngleX = -((float)Math.PI / 2F);
				this.bipedRightArm.rotateAngleX -= f * 1.2F - f1 * 0.4F;
				this.bipedLeftArm.rotateAngleX -= f * 1.2F - f1 * 0.4F;
				this.bipedRightArm.rotateAngleZ += MathHelper.cos(ageInTicks * 0.09F) * 0.05F + 0.05F;
				this.bipedLeftArm.rotateAngleZ -= MathHelper.cos(ageInTicks * 0.09F) * 0.05F + 0.05F;
				this.bipedRightArm.rotateAngleX += MathHelper.sin(ageInTicks * 0.067F) * 0.05F;
				this.bipedLeftArm.rotateAngleX -= MathHelper.sin(ageInTicks * 0.067F) * 0.05F;
			}
		}

		public void postRenderArm(float p_187073_1_, EnumHandSide p_187073_2_)
		{
			float f = p_187073_2_ == EnumHandSide.RIGHT ? 1.0F : -1.0F;
			ModelRenderer modelrenderer = this.getArmForSide(p_187073_2_);
			modelrenderer.rotationPointX += f;
			modelrenderer.postRender(p_187073_1_);
			modelrenderer.rotationPointX -= f;
		}
#endif
	}
}
#endif