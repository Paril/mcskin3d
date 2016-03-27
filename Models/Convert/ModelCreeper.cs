#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelCreeper : ModelBase
	{
		public ModelRenderer head;
		public ModelRenderer creeperArmor;
		public ModelRenderer body;
		public ModelRenderer leg1;
		public ModelRenderer leg2;
		public ModelRenderer leg3;
		public ModelRenderer leg4;

		public ModelCreeper() :
			this(0.0F)
		{
		}

		public ModelCreeper(float p_i46366_1_)
		{
			int i = 6;
			this.head = new ModelRenderer(this, 0, 0, ModelPart.Head);
			this.head.addBox(-4.0F, -8.0F, -4.0F, 8, 8, 8, p_i46366_1_, "Head");
			this.head.setRotationPoint(0.0F, (float)i, 0.0F);
			//this.creeperArmor = new ModelRenderer(this, 32, 0, ModelPart.ChestArmor);
			//this.creeperArmor.addBox(-4.0F, -8.0F, -4.0F, 8, 8, 8, p_i46366_1_ + 0.5F);
			//this.creeperArmor.setRotationPoint(0.0F, (float)i, 0.0F);
			this.body = new ModelRenderer(this, 16, 16, ModelPart.Chest);
			this.body.addBox(-4.0F, 0.0F, -2.0F, 8, 12, 4, p_i46366_1_, "Body");
			this.body.setRotationPoint(0.0F, (float)i, 0.0F);
			this.leg1 = new ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
			this.leg1.addBox(-2.0F, 0.0F, -2.0F, 4, 6, 4, p_i46366_1_, "Leg 1");
			this.leg1.setRotationPoint(-2.0F, (float)(12 + i), 4.0F);
			this.leg2 = new ModelRenderer(this, 0, 16, ModelPart.RightLeg);
			this.leg2.addBox(-2.0F, 0.0F, -2.0F, 4, 6, 4, p_i46366_1_, "Leg 2");
			this.leg2.setRotationPoint(2.0F, (float)(12 + i), 4.0F);
			this.leg3 = new ModelRenderer(this, 0, 16, ModelPart.LeftArm);
			this.leg3.addBox(-2.0F, 0.0F, -2.0F, 4, 6, 4, p_i46366_1_, "Leg 3");
			this.leg3.setRotationPoint(-2.0F, (float)(12 + i), -4.0F);
			this.leg4 = new ModelRenderer(this, 0, 16, ModelPart.RightArm);
			this.leg4.addBox(-2.0F, 0.0F, -2.0F, 4, 6, 4, p_i46366_1_, "Leg 4");
			this.leg4.setRotationPoint(2.0F, (float)(12 + i), -4.0F);
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			this.setRotationAngles(p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale, entityIn);
			this.head.render(scale);
			this.body.render(scale);
			this.leg1.render(scale);
			this.leg2.render(scale);
			this.leg3.render(scale);
			this.leg4.render(scale);
		}
#endif

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			this.head.rotateAngleY = netHeadYaw * 0.017453292F;
			this.head.rotateAngleX = headPitch * 0.017453292F;
			this.leg1.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F) * 1.4F * limbSwingAmount;
			this.leg2.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + (float)Math.PI) * 1.4F * limbSwingAmount;
			this.leg3.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F + (float)Math.PI) * 1.4F * limbSwingAmount;
			this.leg4.rotateAngleX = MathHelper.cos(limbSwing * 0.6662F) * 1.4F * limbSwingAmount;
		}
	}
}
#endif