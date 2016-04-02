#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelCape : ModelBase
	{
		private ModelRenderer bipedCape;

		public ModelCape()
		{
			this.bipedCape = new ModelRenderer(this, 0, 0);
			this.bipedCape.setTextureSize(64, 32);
			this.bipedCape.addBox(-5.0F, 0.0F, -1.0F, 10, 16, 1, 0, "Cape");
			this.bipedCape.rotateAngleY = (float)(Math.PI );
		}
	}

	public class ModelPlayer : ModelBiped
	{
		public ModelRenderer bipedLeftArmwear;
		public ModelRenderer bipedRightArmwear;
		public ModelRenderer bipedLeftLegwear;
		public ModelRenderer bipedRightLegwear;
		public ModelRenderer bipedBodyWear;
		//private ModelRenderer bipedCape;
		private boolean smallArms;
		private bool _19;

		public ModelPlayer(float p_i46304_1_, boolean p_i46304_2_, bool _19 = true) :
			base(p_i46304_1_, 0.0F, 64, _19 ? 64 : 32)
		{
			this._19 = _19;
			this.smallArms = p_i46304_2_;
			//this.bipedCape = new ModelRenderer(this, 0, 0);
			//this.bipedCape.setTextureSize(64, 32);
			//this.bipedCape.addBox(-5.0F, 0.0F, -1.0F, 10, 16, 1, p_i46304_1_);
			
			if (p_i46304_2_)
			{
				boxList.Remove(this.bipedRightArm);
				boxList.Remove(this.bipedLeftArm);

				this.bipedRightArm = new ModelRenderer(this, 40, 16, ModelPart.RightArm);
				this.bipedRightArm.addBox(-2.0F, -2.0F, -2.0F, 3, 12, 4, p_i46304_1_, "Right Arm");
				this.bipedRightArm.setRotationPoint(-5.0F, 2.5F, 0.0F);

				if (_19)
				{
					this.bipedLeftArm = new ModelRenderer(this, 32, 48, ModelPart.LeftArm);
					this.bipedLeftArm.addBox(-1.0F, -2.0F, -2.0F, 3, 12, 4, p_i46304_1_, "Left Arm");
					this.bipedLeftArm.setRotationPoint(5.0F, 2.5F, 0.0F);
				}
				else
				{
					this.bipedLeftArm = new ModelRenderer(this, 40, 16, ModelPart.LeftArm);
					this.bipedLeftArm.mirror = true;
					this.bipedLeftArm.addBox(-1.0F, -2.0F, -2.0F, 3, 12, 4, p_i46304_1_, "Left Arm");
					this.bipedLeftArm.setRotationPoint(5.0F, 2.5F, 0.0F);
				}

				if (_19)
				{
					this.bipedLeftArmwear = new ModelRenderer(this, 48, 48, ModelPart.LeftArmArmor);
					this.bipedLeftArmwear.addBox(-1.0F, -2.0F, -2.0F, 3, 12, 4, p_i46304_1_ + 0.25F, "Left Armwear");
					this.bipedLeftArmwear.setRotationPoint(5.0F, 2.5F, 0.0F);
					this.bipedRightArmwear = new ModelRenderer(this, 40, 32, ModelPart.RightArmArmor);
					this.bipedRightArmwear.addBox(-2.0F, -2.0F, -2.0F, 3, 12, 4, p_i46304_1_ + 0.25F, "Right Armwear");
					this.bipedRightArmwear.setRotationPoint(-5.0F, 2.5F, 10.0F);
				}
			}
			else
			{
				if (_19)
				{
					boxList.Remove(this.bipedLeftArm);
					this.bipedLeftArm = new ModelRenderer(this, 32, 48, ModelPart.LeftArm);
					this.bipedLeftArm.addBox(-1.0F, -2.0F, -2.0F, 4, 12, 4, p_i46304_1_, "Left Arm");
					this.bipedLeftArm.setRotationPoint(5.0F, 2.0F, 0.0F);

					this.bipedLeftArmwear = new ModelRenderer(this, 48, 48, ModelPart.LeftArmArmor);
					this.bipedLeftArmwear.addBox(-1.0F, -2.0F, -2.0F, 4, 12, 4, p_i46304_1_ + 0.25F, "Left Armwear");
					this.bipedLeftArmwear.setRotationPoint(5.0F, 2.0F, 0.0F);
					this.bipedRightArmwear = new ModelRenderer(this, 40, 32, ModelPart.RightArmArmor);
					this.bipedRightArmwear.addBox(-3.0F, -2.0F, -2.0F, 4, 12, 4, p_i46304_1_ + 0.25F, "Right Armwear");
					this.bipedRightArmwear.setRotationPoint(-5.0F, 2.0F, 10.0F);
				}
			}

			if (_19)
			{
				boxList.Remove(this.bipedLeftLeg);
				this.bipedLeftLeg = new ModelRenderer(this, 16, 48, ModelPart.LeftLeg);
				this.bipedLeftLeg.addBox(-2.0F, 0.0F, -2.0F, 4, 12, 4, p_i46304_1_, "Left Leg");
				this.bipedLeftLeg.setRotationPoint(1.9F, 12.0F, 0.0F);
				this.bipedLeftLegwear = new ModelRenderer(this, 0, 48, ModelPart.LeftLegArmor);
				this.bipedLeftLegwear.addBox(-2.0F, 0.0F, -2.0F, 4, 12, 4, p_i46304_1_ + 0.25F, "Left Legwear");
				this.bipedLeftLegwear.setRotationPoint(1.9F, 12.0F, 0.0F);
				this.bipedRightLegwear = new ModelRenderer(this, 0, 32, ModelPart.RightLegArmor);
				this.bipedRightLegwear.addBox(-2.0F, 0.0F, -2.0F, 4, 12, 4, p_i46304_1_ + 0.25F, "Right Legwear");
				this.bipedRightLegwear.setRotationPoint(-1.9F, 12.0F, 0.0F);
				this.bipedBodyWear = new ModelRenderer(this, 16, 32, ModelPart.ChestArmor);
				this.bipedBodyWear.addBox(-4.0F, 0.0F, -2.0F, 8, 12, 4, p_i46304_1_ + 0.25F, "Bodywear");
				this.bipedBodyWear.setRotationPoint(0.0F, 0.0F, 0.0F);
			}

			this.bipedHead.isSolid = true;
			this.bipedBody.isSolid = true;
			this.bipedLeftLeg.isSolid = true;
			this.bipedRightLeg.isSolid = true;
			this.bipedLeftArm.isSolid = true;
			this.bipedRightArm.isSolid = true;

			if (_19)
			{
				this.bipedLeftLegwear.isArmor = true;
				this.bipedRightLegwear.isArmor = true;
				this.bipedBodyWear.isArmor = true;
				this.bipedHeadwear.isArmor = true;
				this.bipedLeftArmwear.isArmor = true;
				this.bipedRightArmwear.isArmor = true;
			}
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			super.render(entityIn, p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale);
			GlStateManager.pushMatrix();

			if (this.isChild)
			{
				float f = 2.0F;
				GlStateManager.scale(1.0F / f, 1.0F / f, 1.0F / f);
				GlStateManager.translate(0.0F, 24.0F * scale, 0.0F);
				this.bipedLeftLegwear.render(scale);
				this.bipedRightLegwear.render(scale);
				this.bipedLeftArmwear.render(scale);
				this.bipedRightArmwear.render(scale);
				this.bipedBodyWear.render(scale);
			}
			else
			{
				if (entityIn.isSneaking())
				{
					GlStateManager.translate(0.0F, 0.2F, 0.0F);
				}

				this.bipedLeftLegwear.render(scale);
				this.bipedRightLegwear.render(scale);
				this.bipedLeftArmwear.render(scale);
				this.bipedRightArmwear.render(scale);
				this.bipedBodyWear.render(scale);
			}

			GlStateManager.popMatrix();
		}

		public void renderDeadmau5Head(float p_178727_1_)
		{
			copyModelAngles(this.bipedHead, this.bipedDeadmau5Head);
			this.bipedDeadmau5Head.rotationPointX = 0.0F;
			this.bipedDeadmau5Head.rotationPointY = 0.0F;
			this.bipedDeadmau5Head.render(p_178727_1_);
		}

		public void renderCape(float p_178728_1_)
		{
			this.bipedCape.render(p_178728_1_);
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

			if (_19)
			{
				copyModelAngles(this.bipedLeftLeg, this.bipedLeftLegwear);
				copyModelAngles(this.bipedRightLeg, this.bipedRightLegwear);
				copyModelAngles(this.bipedLeftArm, this.bipedLeftArmwear);
				copyModelAngles(this.bipedRightArm, this.bipedRightArmwear);
				copyModelAngles(this.bipedBody, this.bipedBodyWear);
			}

			/*if (entityIn.isSneaking())
			{
				this.bipedCape.rotationPointY = 2.0F;
			}
			else
			{
				this.bipedCape.rotationPointY = 0.0F;
			}*/
		}

		public override void setInvisible(boolean invisible)
		{
			base.setInvisible(invisible);
			if (_19)
			{
				this.bipedLeftArmwear.showModel = invisible;
				this.bipedRightArmwear.showModel = invisible;
				this.bipedLeftLegwear.showModel = invisible;
				this.bipedRightLegwear.showModel = invisible;
				this.bipedBodyWear.showModel = invisible;
			}

			//this.bipedCape.showModel = invisible;
			//this.bipedDeadmau5Head.showModel = invisible;
		}

#if NO
		public void postRenderArm(float p_187073_1_, EnumHandSide p_187073_2_)
		{
			ModelRenderer modelrenderer = this.getArmForSide(p_187073_2_);

			if (this.smallArms)
			{
				float f = 0.5F * (float)(p_187073_2_ == EnumHandSide.RIGHT ? 1 : -1);
				modelrenderer.rotationPointX += f;
				modelrenderer.postRender(p_187073_1_);
				modelrenderer.rotationPointX -= f;
			}
			else
			{
				modelrenderer.postRender(p_187073_1_);
			}
		}
#endif
	}
}
#endif