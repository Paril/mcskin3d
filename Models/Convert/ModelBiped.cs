#if CONVERT_MODELS
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelBiped : ModelBase
	{
		public ModelRenderer bipedHead;

		/** The Biped's Headwear. Used for the outer layer of player skins. */
		public ModelRenderer bipedHeadwear;
		public ModelRenderer bipedBody;

		/** The Biped's Right Arm */
		public ModelRenderer bipedRightArm;

		/** The Biped's Left Arm */
		public ModelRenderer bipedLeftArm;

		/** The Biped's Right Leg */
		public ModelRenderer bipedRightLeg;

		/** The Biped's Left Leg */
		public ModelRenderer bipedLeftLeg;
		public ModelBiped.ArmPose leftArmPose;
		public ModelBiped.ArmPose rightArmPose;
		public boolean isSneak;

		public ModelBiped() :
			this(0.0F)
		{
		}

		public ModelBiped(float modelSize) :
			this(modelSize, 0.0F, 64, 32)
		{
		}

		public ModelBiped(float modelSize, float p_i1149_2_, int textureWidthIn, int textureHeightIn)
		{
			this.leftArmPose = ModelBiped.ArmPose.EMPTY;
			this.rightArmPose = ModelBiped.ArmPose.EMPTY;
			this.textureWidth = textureWidthIn;
			this.textureHeight = textureHeightIn;
			this.bipedHead = new ModelRenderer(this, 0, 0, ModelPart.Head);
			this.bipedHead.addBox(-4.0F, -8.0F, -4.0F, 8, 8, 8, modelSize, "Head");
			this.bipedHead.setRotationPoint(0.0F, 0.0F + p_i1149_2_, 0.0F);
			this.bipedHeadwear = new ModelRenderer(this, 32, 0, ModelPart.Helmet);
			this.bipedHeadwear.addBox(-4.0F, -8.0F, -4.0F, 8, 8, 8, modelSize + 0.5F, "Headwear");
			this.bipedHeadwear.setRotationPoint(0.0F, 0.0F + p_i1149_2_, 0.0F);
			this.bipedBody = new ModelRenderer(this, 16, 16, ModelPart.Chest);
			this.bipedBody.addBox(-4.0F, 0.0F, -2.0F, 8, 12, 4, modelSize, "Body");
			this.bipedBody.setRotationPoint(0.0F, 0.0F + p_i1149_2_, 0.0F);
			this.bipedRightArm = new ModelRenderer(this, 40, 16, ModelPart.RightArm);
			this.bipedRightArm.addBox(-3.0F, -2.0F, -2.0F, 4, 12, 4, modelSize, "Right Arm");
			this.bipedRightArm.setRotationPoint(-5.0F, 2.0F + p_i1149_2_, 0.0F);
			this.bipedLeftArm = new ModelRenderer(this, 40, 16, ModelPart.LeftArm);
			this.bipedLeftArm.mirror = true;
			this.bipedLeftArm.addBox(-1.0F, -2.0F, -2.0F, 4, 12, 4, modelSize, "Left Arm");
			this.bipedLeftArm.setRotationPoint(5.0F, 2.0F + p_i1149_2_, 0.0F);
			this.bipedRightLeg = new ModelRenderer(this, 0, 16, ModelPart.RightLeg);
			this.bipedRightLeg.addBox(-2.0F, 0.0F, -2.0F, 4, 12, 4, modelSize, "Right Leg");
			this.bipedRightLeg.setRotationPoint(-1.9F, 12.0F + p_i1149_2_, 0.0F);
			this.bipedLeftLeg = new ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
			this.bipedLeftLeg.mirror = true;
			this.bipedLeftLeg.addBox(-2.0F, 0.0F, -2.0F, 4, 12, 4, modelSize, "Left Leg");
			this.bipedLeftLeg.setRotationPoint(1.9F, 12.0F + p_i1149_2_, 0.0F);
		}

#if RENDER
		/**
			* Sets the models various rotation angles then renders the model.
			*/
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			this.setRotationAngles(p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale, entityIn);
			GlStateManager.pushMatrix();

			if (this.isChild)
			{
				float f = 2.0F;
				GlStateManager.scale(1.5F / f, 1.5F / f, 1.5F / f);
				GlStateManager.translate(0.0F, 16.0F * scale, 0.0F);
				this.bipedHead.render(scale);
				GlStateManager.popMatrix();
				GlStateManager.pushMatrix();
				GlStateManager.scale(1.0F / f, 1.0F / f, 1.0F / f);
				GlStateManager.translate(0.0F, 24.0F * scale, 0.0F);
				this.bipedBody.render(scale);
				this.bipedRightArm.render(scale);
				this.bipedLeftArm.render(scale);
				this.bipedRightLeg.render(scale);
				this.bipedLeftLeg.render(scale);
				this.bipedHeadwear.render(scale);
			}
			else
			{
				if (entityIn.isSneaking())
				{
					GlStateManager.translate(0.0F, 0.2F, 0.0F);
				}

				this.bipedHead.render(scale);
				this.bipedBody.render(scale);
				this.bipedRightArm.render(scale);
				this.bipedLeftArm.render(scale);
				this.bipedRightLeg.render(scale);
				this.bipedLeftLeg.render(scale);
				this.bipedHeadwear.render(scale);
			}

			GlStateManager.popMatrix();
		}
#endif

		//@SuppressWarnings("incomplete-switch")

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public virtual void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			boolean flag = entityIn is EntityLivingBase && ((EntityLivingBase)entityIn).func_184599_cB() > 4;
			this.bipedHead.rotateAngleY = netHeadYaw * 0.017453292F;

			if (flag)
			{
				this.bipedHead.rotateAngleX = -((float)Math.PI / 4F);
			}
			else
			{
				this.bipedHead.rotateAngleX = headPitch * 0.017453292F;
			}

			this.bipedBody.rotateAngleY = 0.0F;
			this.bipedRightArm.rotationPointZ = 0.0F;
			this.bipedRightArm.rotationPointX = -5.0F;
			this.bipedLeftArm.rotationPointZ = 0.0F;
			this.bipedLeftArm.rotationPointX = 5.0F;
			float f = 1.0F;

			if (flag)
			{
				f = (float)(entityIn.motionX * entityIn.motionX + entityIn.motionY * entityIn.motionY + entityIn.motionZ * entityIn.motionZ);
				f = f / 0.2F;
				f = f * f * f;
			}

			if (f < 1.0F)
			{
				f = 1.0F;
			}

			//this.bipedRightArm.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + (float)Math.PI) * 2.0F * limbSwingAmount * 0.5F / f;
			//this.bipedLeftArm.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F) * 2.0F * limbSwingAmount * 0.5F / f;
			this.bipedRightArm.rotateAngleZ = 0.0F;
			this.bipedLeftArm.rotateAngleZ = 0.0F;
			this.bipedRightLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F) * 1.4F * limbSwingAmount / f;
			this.bipedLeftLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + (float)Math.PI) * 1.4F * limbSwingAmount / f;
			this.bipedRightLeg.rotateAngleY = 0.0F;
			this.bipedLeftLeg.rotateAngleY = 0.0F;
			this.bipedRightLeg.rotateAngleZ = 0.0F;
			this.bipedLeftLeg.rotateAngleZ = 0.0F;

			if (this.isRiding)
			{
				this.bipedRightArm.rotateAngleX += -((float)Math.PI / 5F);
				this.bipedLeftArm.rotateAngleX += -((float)Math.PI / 5F);
				this.bipedRightLeg.rotateAngleX = -1.4137167F;
				this.bipedRightLeg.rotateAngleY = ((float)Math.PI / 10F);
				this.bipedRightLeg.rotateAngleZ = 0.07853982F;
				this.bipedLeftLeg.rotateAngleX = -1.4137167F;
				this.bipedLeftLeg.rotateAngleY = -((float)Math.PI / 10F);
				this.bipedLeftLeg.rotateAngleZ = -0.07853982F;
			}

			this.bipedRightArm.rotateAngleY = 0.0F;
			this.bipedRightArm.rotateAngleZ = 0.0F;

			switch (this.leftArmPose)
			{
				case ArmPose.EMPTY:
					this.bipedLeftArm.rotateAngleY = 0.0F;
					break;

				case ArmPose.BLOCK:
					this.bipedLeftArm.rotateAngleX = this.bipedLeftArm.rotateAngleX * 0.5F - 0.9424779F;
					this.bipedLeftArm.rotateAngleY = 0.5235988F;
					break;

				case ArmPose.ITEM:
					this.bipedLeftArm.rotateAngleX = this.bipedLeftArm.rotateAngleX * 0.5F - ((float)Math.PI / 10F);
					this.bipedLeftArm.rotateAngleY = 0.0F;
					break;
			}

			switch (this.rightArmPose)
			{
				case ArmPose.EMPTY:
					this.bipedRightArm.rotateAngleY = 0.0F;
					break;

				case ArmPose.BLOCK:
					this.bipedRightArm.rotateAngleX = this.bipedRightArm.rotateAngleX * 0.5F - 0.9424779F;
					this.bipedRightArm.rotateAngleY = -0.5235988F;
					break;

				case ArmPose.ITEM:
					this.bipedRightArm.rotateAngleX = this.bipedRightArm.rotateAngleX * 0.5F - ((float)Math.PI / 10F);
					this.bipedRightArm.rotateAngleY = 0.0F;
					break;
			}

			if (this.swingProgress > 0.0F)
			{
				EnumHandSide enumhandside = this.getMainHand(entityIn);
				ModelRenderer modelrenderer = this.getArmForSide(enumhandside);
				this.getArmForSide(enumhandside.opposite());
				float f1 = this.swingProgress;
				this.bipedBody.rotateAngleY = MathHelper.sin(MathHelper.sqrt_float(f1) * ((float)Math.PI * 2F)) * 0.2F;

				if (enumhandside == EnumHandSide.LEFT)
				{
					this.bipedBody.rotateAngleY *= -1.0F;
				}

				this.bipedRightArm.rotationPointZ = MathHelper.sin(this.bipedBody.rotateAngleY) * 5.0F;
				this.bipedRightArm.rotationPointX = -MathHelper.cos(this.bipedBody.rotateAngleY) * 5.0F;
				this.bipedLeftArm.rotationPointZ = -MathHelper.sin(this.bipedBody.rotateAngleY) * 5.0F;
				this.bipedLeftArm.rotationPointX = MathHelper.cos(this.bipedBody.rotateAngleY) * 5.0F;
				this.bipedRightArm.rotateAngleY += this.bipedBody.rotateAngleY;
				this.bipedLeftArm.rotateAngleY += this.bipedBody.rotateAngleY;
				this.bipedLeftArm.rotateAngleX += this.bipedBody.rotateAngleY;
				f1 = 1.0F - this.swingProgress;
				f1 = f1 * f1;
				f1 = f1 * f1;
				f1 = 1.0F - f1;
				float f2 = MathHelper.sin(f1 * (float)Math.PI);
				float f3 = MathHelper.sin(this.swingProgress * (float)Math.PI) * -(this.bipedHead.rotateAngleX - 0.7F) * 0.75F;
				modelrenderer.rotateAngleX = (float)((double)modelrenderer.rotateAngleX - ((double)f2 * 1.2D + (double)f3));
				modelrenderer.rotateAngleY += this.bipedBody.rotateAngleY * 2.0F;
				modelrenderer.rotateAngleZ += MathHelper.sin(this.swingProgress * (float)Math.PI) * -0.4F;
			}

			if (this.isSneak)
			{
				this.bipedBody.rotateAngleX = 0.5F;
				this.bipedRightArm.rotateAngleX += 0.4F;
				this.bipedLeftArm.rotateAngleX += 0.4F;
				this.bipedRightLeg.rotationPointZ = 4.0F;
				this.bipedLeftLeg.rotationPointZ = 4.0F;
				this.bipedRightLeg.rotationPointY = 9.0F;
				this.bipedLeftLeg.rotationPointY = 9.0F;
				this.bipedHead.rotationPointY = 1.0F;
			}
			else
			{
				this.bipedBody.rotateAngleX = 0.0F;
				this.bipedRightLeg.rotationPointZ = 0.1F;
				this.bipedLeftLeg.rotationPointZ = 0.1F;
				this.bipedRightLeg.rotationPointY = 12.0F;
				this.bipedLeftLeg.rotationPointY = 12.0F;
				this.bipedHead.rotationPointY = 0.0F;
			}

			//this.bipedRightArm.rotateAngleZ += MathHelper.cos(ageInTicks * 0.09F) * 0.05F + 0.05F;
			//this.bipedLeftArm.rotateAngleZ -= MathHelper.cos(ageInTicks * 0.09F) * 0.05F + 0.05F;
			//this.bipedRightArm.rotateAngleX += MathHelper.sin(ageInTicks * 0.067F) * 0.05F;
			//this.bipedLeftArm.rotateAngleX -= MathHelper.sin(ageInTicks * 0.067F) * 0.05F;

			if (this.rightArmPose == ModelBiped.ArmPose.BOW_AND_ARROW)
			{
				this.bipedRightArm.rotateAngleY = -0.1F + this.bipedHead.rotateAngleY;
				this.bipedLeftArm.rotateAngleY = 0.1F + this.bipedHead.rotateAngleY + 0.4F;
				this.bipedRightArm.rotateAngleX = -((float)Math.PI / 2F) + this.bipedHead.rotateAngleX;
				this.bipedLeftArm.rotateAngleX = -((float)Math.PI / 2F) + this.bipedHead.rotateAngleX;
			}
			else if (this.leftArmPose == ModelBiped.ArmPose.BOW_AND_ARROW)
			{
				this.bipedRightArm.rotateAngleY = -0.1F + this.bipedHead.rotateAngleY - 0.4F;
				this.bipedLeftArm.rotateAngleY = 0.1F + this.bipedHead.rotateAngleY;
				this.bipedRightArm.rotateAngleX = -((float)Math.PI / 2F) + this.bipedHead.rotateAngleX;
				this.bipedLeftArm.rotateAngleX = -((float)Math.PI / 2F) + this.bipedHead.rotateAngleX;
			}

			copyModelAngles(this.bipedHead, this.bipedHeadwear);
		}

		public override void setModelAttributes(ModelBase model)
		{
			base.setModelAttributes(model);

			if (model is ModelBiped)
			{
				ModelBiped modelbiped = (ModelBiped)model;
				this.leftArmPose = modelbiped.leftArmPose;
				this.rightArmPose = modelbiped.rightArmPose;
				this.isSneak = modelbiped.isSneak;
			}
		}

		public virtual void setInvisible(boolean invisible)
		{
			this.bipedHead.showModel = invisible;
			this.bipedHeadwear.showModel = invisible;
			this.bipedBody.showModel = invisible;
			this.bipedRightArm.showModel = invisible;
			this.bipedLeftArm.showModel = invisible;
			this.bipedRightLeg.showModel = invisible;
			this.bipedLeftLeg.showModel = invisible;
		}

#if RENDER
		public void postRenderArm(float p_187073_1_, EnumHandSide p_187073_2_)
		{
			this.getArmForSide(p_187073_2_).postRender(p_187073_1_);
		}
#endif

		protected ModelRenderer getArmForSide(EnumHandSide p_187074_1_)
		{
			return p_187074_1_ == EnumHandSide.LEFT ? this.bipedLeftArm : this.bipedRightArm;
		}

		protected EnumHandSide getMainHand(Entity p_187072_1_)
		{
			return p_187072_1_ is EntityLivingBase ? ((EntityLivingBase)p_187072_1_).getPrimaryHand() : EnumHandSide.RIGHT;
		}

		public enum ArmPose
		{
			EMPTY,
			ITEM,
			BLOCK,
			BOW_AND_ARROW
		}
	}
}
#endif