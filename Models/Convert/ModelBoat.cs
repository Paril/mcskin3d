#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelBoat : ModelBase, IMultipassModel
	{
		public ModelRenderer[] boatSides = new ModelRenderer[5];
		public ModelRenderer[] field_187057_b = new ModelRenderer[2];
		public ModelRenderer field_187058_c;
		//private int field_187059_d = GLAllocation.generateDisplayLists(1);

		public ModelBoat()
		{
			this.boatSides[0] = (new ModelRenderer(this, 0, 0)).setTextureSize(128, 64);
			this.boatSides[1] = (new ModelRenderer(this, 0, 19)).setTextureSize(128, 64);
			this.boatSides[2] = (new ModelRenderer(this, 0, 27)).setTextureSize(128, 64);
			this.boatSides[3] = (new ModelRenderer(this, 0, 35)).setTextureSize(128, 64);
			this.boatSides[4] = (new ModelRenderer(this, 0, 43)).setTextureSize(128, 64);
			int i = 32;
			int j = 6;
			int k = 20;
			int l = 4;
			int i1 = 28;
			this.boatSides[0].addBox((float)(-i1 / 2), (float)(-k / 2 + 1), -3.0F, i1, k - 4, 3, 0.0F, "Bottom");
			this.boatSides[0].setRotationPoint(0.0F, (float)(l - 1), 1.0F);
			this.boatSides[1].addBox((float)(-i / 2 + 3), (float)(-j - 1), -1.0F, k - 2, j, 2, 0.0F, "Back");
			this.boatSides[1].setRotationPoint((float)(-i / 2 + 1), (float)l, 4.0F);
			this.boatSides[2].addBox((float)(-i / 2 + 8), (float)(-j - 1), -1.0F, k - 4, j, 2, 0.0F, "Front");
			this.boatSides[2].setRotationPoint((float)(i / 2 - 1), (float)l, 0.0F);
			this.boatSides[3].addBox((float)(-i / 2 + 2), (float)(-j - 1), -1.0F, i - 4, j, 2, 0.0F, "Right");
			this.boatSides[3].setRotationPoint(0.0F, (float)l, (float)(-k / 2 + 1));
			this.boatSides[4].addBox((float)(-i / 2 + 2), (float)(-j - 1), -1.0F, i - 4, j, 2, 0.0F, "Left");
			this.boatSides[4].setRotationPoint(0.0F, (float)l, (float)(k / 2 - 1));
			this.boatSides[0].rotateAngleX = ((float)Math.PI / 2F);
			this.boatSides[1].rotateAngleY = ((float)Math.PI * 3F / 2F);
			this.boatSides[2].rotateAngleY = ((float)Math.PI / 2F);
			this.boatSides[3].rotateAngleY = (float)Math.PI;
			this.field_187057_b[0] = this.func_187056_a(true);
			this.field_187057_b[0].setRotationPoint(3.0F, -5.0F, 9.0F);
			this.field_187057_b[1] = this.func_187056_a(false);
			this.field_187057_b[1].setRotationPoint(3.0F, -5.0F, -9.0F);
			this.field_187057_b[1].rotateAngleY = (float)Math.PI;
			this.field_187057_b[0].rotateAngleZ = this.field_187057_b[1].rotateAngleZ = 0.19634955F;
			this.field_187058_c = (new ModelRenderer(this, 0, 0)).setTextureSize(128, 64);
			this.field_187058_c.addBox((float)(-i1 / 2), (float)(-k / 2 + 1), -3.0F, i1, k - 4, 3, 0.0F, "Depth box");
			this.field_187058_c.setRotationPoint(0.0F, (float)(l - 7), 1.0F);
			this.field_187058_c.rotateAngleX = ((float)Math.PI / 2F);
			this.field_187058_c.isHidden = true;
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			GlStateManager.rotate(90.0F, 0.0F, 1.0F, 0.0F);
			EntityBoat entityboat = (EntityBoat)entityIn;
			this.setRotationAngles(p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale, entityIn);

			for (int i = 0; i < 5; ++i)
			{
				this.boatSides[i].render(scale);
			}

			this.func_187055_a(entityboat, 0, scale, p_78088_2_);
			this.func_187055_a(entityboat, 1, scale, p_78088_2_);
		}

		public void func_187054_b(Entity p_187054_1_, float p_187054_2_, float p_187054_3_, float p_187054_4_, float p_187054_5_, float p_187054_6_, float p_187054_7_)
		{
			GlStateManager.rotate(90.0F, 0.0F, 1.0F, 0.0F);
			GlStateManager.colorMask(false, false, false, false);
			this.field_187058_c.render(p_187054_7_);
			GlStateManager.colorMask(true, true, true, true);
		}
#endif

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
		}

		ModelRenderer func_187056_a(boolean p_187056_1_)
		{
			ModelRenderer modelrenderer = (new ModelRenderer(this, 62, p_187056_1_ ? 0 : 20)).setTextureSize(128, 64);
			int i = 20;
			int j = 7;
			int k = 6;
			float f = -5.0F;
			modelrenderer.addBox(-1.0F, 0.0F, f, 2, 2, i - 2, "Oar handle");
			modelrenderer.addBox(p_187056_1_ ? -1.001F : 0.001F, (float)(-k / 2), (float)(i - j) + f, 1, k, j, "Oar head");
			return modelrenderer;
		}

#if RENDER
		void func_187055_a(EntityBoat p_187055_1_, int p_187055_2_, float p_187055_3_, float p_187055_4_)
		{
			float f = 40.0F;
			float f1 = p_187055_1_.func_184448_a(p_187055_2_, p_187055_4_) * f;
			ModelRenderer modelrenderer = this.field_187057_b[p_187055_2_];
			modelrenderer.rotateAngleX = (float)MathHelper.denormalizeClamp(-1.0471975803375244D, -0.2617993950843811D, (double)((MathHelper.sin(-f1) + 1.0F) / 2.0F));
			modelrenderer.rotateAngleY = (float)MathHelper.denormalizeClamp(-(Math.PI / 4D), (Math.PI / 4D), (double)((MathHelper.sin(-f1 + 1.0F) + 1.0F) / 2.0F));

			if (p_187055_2_ == 1)
			{
				modelrenderer.rotateAngleY = (float)Math.PI - modelrenderer.rotateAngleY;
			}

			modelrenderer.render(p_187055_3_);
		}
#endif
	}
}
#endif