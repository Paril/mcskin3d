#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelOcelot : ModelBase
	{
		/** The back left leg model for the Ocelot. */
		private ModelRenderer ocelotBackLeftLeg;

		/** The back right leg model for the Ocelot. */
		private ModelRenderer ocelotBackRightLeg;

		/** The front left leg model for the Ocelot. */
		private ModelRenderer ocelotFrontLeftLeg;

		/** The front right leg model for the Ocelot. */
		private ModelRenderer ocelotFrontRightLeg;

		/** The tail model for the Ocelot. */
		private ModelRenderer ocelotTail;

		/** The second part of tail model for the Ocelot. */
		private ModelRenderer ocelotTail2;

		/** The head model for the Ocelot. */
		private ModelRenderer ocelotHead;

		/** The body model for the Ocelot. */
		private ModelRenderer ocelotBody;
		private int field_78163_i = 1;

		public ModelOcelot()
		{
			this.setTextureOffset("head.main", 0, 0);
			this.setTextureOffset("head.nose", 0, 24);
			this.setTextureOffset("head.ear1", 0, 10);
			this.setTextureOffset("head.ear2", 6, 10);
			this.ocelotHead = new ModelRenderer(this, "head");
			this.ocelotHead.addBox("main", -2.5F, -2.0F, -3.0F, 5, 4, 5);
			this.ocelotHead.addBox("nose", -1.5F, 0.0F, -4.0F, 3, 2, 2);
			this.ocelotHead.addBox("ear1", -2.0F, -3.0F, 0.0F, 1, 1, 2);
			this.ocelotHead.addBox("ear2", 1.0F, -3.0F, 0.0F, 1, 1, 2);
			this.ocelotHead.setRotationPoint(0.0F, 15.0F, -9.0F);
			this.ocelotBody = new ModelRenderer(this, 20, 0);
			this.ocelotBody.addBox(-2.0F, 3.0F, -8.0F, 4, 16, 6, 0.0F, "Body");
			this.ocelotBody.setRotationPoint(0.0F, 12.0F, -10.0F);
			this.ocelotTail = new ModelRenderer(this, 0, 15);
			this.ocelotTail.addBox(-0.5F, 0.0F, 0.0F, 1, 8, 1, "Tail");
			this.ocelotTail.rotateAngleX = 0.9F;
			this.ocelotTail.setRotationPoint(0.0F, 15.0F, 8.0F);
			this.ocelotTail2 = new ModelRenderer(this, 4, 15);
			this.ocelotTail2.addBox(-0.5F, 0.0F, 0.0F, 1, 8, 1, "Tail End");
			this.ocelotTail2.setRotationPoint(0.0F, 20.0F, 14.0F);
			this.ocelotBackLeftLeg = new ModelRenderer(this, 8, 13);
			this.ocelotBackLeftLeg.addBox(-1.0F, 0.0F, 1.0F, 2, 6, 2, "Back Left Leg");
			this.ocelotBackLeftLeg.setRotationPoint(1.1F, 18.0F, 5.0F);
			this.ocelotBackRightLeg = new ModelRenderer(this, 8, 13);
			this.ocelotBackRightLeg.addBox(-1.0F, 0.0F, 1.0F, 2, 6, 2, "Back Right Leg");
			this.ocelotBackRightLeg.setRotationPoint(-1.1F, 18.0F, 5.0F);
			this.ocelotFrontLeftLeg = new ModelRenderer(this, 40, 0);
			this.ocelotFrontLeftLeg.addBox(-1.0F, 0.0F, 0.0F, 2, 10, 2, "Front Left Leg");
			this.ocelotFrontLeftLeg.setRotationPoint(1.2F, 13.8F, -5.0F);
			this.ocelotFrontRightLeg = new ModelRenderer(this, 40, 0);
			this.ocelotFrontRightLeg.addBox(-1.0F, 0.0F, 0.0F, 2, 10, 2, "Front Right Leg");
			this.ocelotFrontRightLeg.setRotationPoint(-1.2F, 13.8F, -5.0F);
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			this.setRotationAngles(p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale, entityIn);

			if (this.isChild)
			{
				float f = 2.0F;
				GlStateManager.pushMatrix();
				GlStateManager.scale(1.5F / f, 1.5F / f, 1.5F / f);
				GlStateManager.translate(0.0F, 10.0F * scale, 4.0F * scale);
				this.ocelotHead.render(scale);
				GlStateManager.popMatrix();
				GlStateManager.pushMatrix();
				GlStateManager.scale(1.0F / f, 1.0F / f, 1.0F / f);
				GlStateManager.translate(0.0F, 24.0F * scale, 0.0F);
				this.ocelotBody.render(scale);
				this.ocelotBackLeftLeg.render(scale);
				this.ocelotBackRightLeg.render(scale);
				this.ocelotFrontLeftLeg.render(scale);
				this.ocelotFrontRightLeg.render(scale);
				this.ocelotTail.render(scale);
				this.ocelotTail2.render(scale);
				GlStateManager.popMatrix();
			}
			else
			{
				this.ocelotHead.render(scale);
				this.ocelotBody.render(scale);
				this.ocelotTail.render(scale);
				this.ocelotTail2.render(scale);
				this.ocelotBackLeftLeg.render(scale);
				this.ocelotBackRightLeg.render(scale);
				this.ocelotFrontLeftLeg.render(scale);
				this.ocelotFrontRightLeg.render(scale);
			}
		}
#endif

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			this.ocelotHead.rotateAngleX = headPitch * 0.017453292F;
			this.ocelotHead.rotateAngleY = netHeadYaw * 0.017453292F;

			if (this.field_78163_i != 3)
			{
				this.ocelotBody.rotateAngleX = ((float)Math.PI / 2F);

				if (this.field_78163_i == 2)
				{
					this.ocelotBackLeftLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F) * limbSwingAmount;
					this.ocelotBackRightLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + 0.3F) * limbSwingAmount;
					this.ocelotFrontLeftLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + (float)Math.PI + 0.3F) * limbSwingAmount;
					this.ocelotFrontRightLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + (float)Math.PI) * limbSwingAmount;
					this.ocelotTail2.rotateAngleX = 1.7278761F + ((float)Math.PI / 10F) * MathHelper.cos(limbSwing) * limbSwingAmount;
				}
				else
				{
					this.ocelotBackLeftLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F) * limbSwingAmount;
					this.ocelotBackRightLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + (float)Math.PI) * limbSwingAmount;
					this.ocelotFrontLeftLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + (float)Math.PI) * limbSwingAmount;
					this.ocelotFrontRightLeg.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F) * limbSwingAmount;

					if (this.field_78163_i == 1)
					{
						this.ocelotTail2.rotateAngleX = 1.7278761F + ((float)Math.PI / 4F) * MathHelper.cos(limbSwing) * limbSwingAmount;
					}
					else
					{
						this.ocelotTail2.rotateAngleX = 1.7278761F + 0.47123894F * MathHelper.cos(limbSwing) * limbSwingAmount;
					}
				}
			}
		}

#if RENDER
		/**
		 * Used for easily adding entity-dependent animations. The second and third float params here are the same second
		 * and third as in the setRotationAngles method.
		 */
		public void setLivingAnimations(EntityLivingBase entitylivingbaseIn, float p_78086_2_, float p_78086_3_, float partialTickTime)
		{
			EntityOcelot entityocelot = (EntityOcelot)entitylivingbaseIn;
			this.ocelotBody.rotationPointY = 12.0F;
			this.ocelotBody.rotationPointZ = -10.0F;
			this.ocelotHead.rotationPointY = 15.0F;
			this.ocelotHead.rotationPointZ = -9.0F;
			this.ocelotTail.rotationPointY = 15.0F;
			this.ocelotTail.rotationPointZ = 8.0F;
			this.ocelotTail2.rotationPointY = 20.0F;
			this.ocelotTail2.rotationPointZ = 14.0F;
			this.ocelotFrontLeftLeg.rotationPointY = this.ocelotFrontRightLeg.rotationPointY = 13.8F;
			this.ocelotFrontLeftLeg.rotationPointZ = this.ocelotFrontRightLeg.rotationPointZ = -5.0F;
			this.ocelotBackLeftLeg.rotationPointY = this.ocelotBackRightLeg.rotationPointY = 18.0F;
			this.ocelotBackLeftLeg.rotationPointZ = this.ocelotBackRightLeg.rotationPointZ = 5.0F;
			this.ocelotTail.rotateAngleX = 0.9F;

			if (entityocelot.isSneaking())
			{
				++this.ocelotBody.rotationPointY;
				this.ocelotHead.rotationPointY += 2.0F;
				++this.ocelotTail.rotationPointY;
				this.ocelotTail2.rotationPointY += -4.0F;
				this.ocelotTail2.rotationPointZ += 2.0F;
				this.ocelotTail.rotateAngleX = ((float)Math.PI / 2F);
				this.ocelotTail2.rotateAngleX = ((float)Math.PI / 2F);
				this.field_78163_i = 0;
			}
			else if (entityocelot.isSprinting())
			{
				this.ocelotTail2.rotationPointY = this.ocelotTail.rotationPointY;
				this.ocelotTail2.rotationPointZ += 2.0F;
				this.ocelotTail.rotateAngleX = ((float)Math.PI / 2F);
				this.ocelotTail2.rotateAngleX = ((float)Math.PI / 2F);
				this.field_78163_i = 2;
			}
			else if (entityocelot.isSitting())
			{
				this.ocelotBody.rotateAngleX = ((float)Math.PI / 4F);
				this.ocelotBody.rotationPointY += -4.0F;
				this.ocelotBody.rotationPointZ += 5.0F;
				this.ocelotHead.rotationPointY += -3.3F;
				++this.ocelotHead.rotationPointZ;
				this.ocelotTail.rotationPointY += 8.0F;
				this.ocelotTail.rotationPointZ += -2.0F;
				this.ocelotTail2.rotationPointY += 2.0F;
				this.ocelotTail2.rotationPointZ += -0.8F;
				this.ocelotTail.rotateAngleX = 1.7278761F;
				this.ocelotTail2.rotateAngleX = 2.670354F;
				this.ocelotFrontLeftLeg.rotateAngleX = this.ocelotFrontRightLeg.rotateAngleX = -0.15707964F;
				this.ocelotFrontLeftLeg.rotationPointY = this.ocelotFrontRightLeg.rotationPointY = 15.8F;
				this.ocelotFrontLeftLeg.rotationPointZ = this.ocelotFrontRightLeg.rotationPointZ = -7.0F;
				this.ocelotBackLeftLeg.rotateAngleX = this.ocelotBackRightLeg.rotateAngleX = -((float)Math.PI / 2F);
				this.ocelotBackLeftLeg.rotationPointY = this.ocelotBackRightLeg.rotationPointY = 21.0F;
				this.ocelotBackLeftLeg.rotationPointZ = this.ocelotBackRightLeg.rotationPointZ = 1.0F;
				this.field_78163_i = 3;
			}
			else
			{
				this.field_78163_i = 1;
			}
		}
#endif
	}
}
#endif