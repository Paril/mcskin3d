#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelGhast : ModelBase
	{
		ModelRenderer body;
		ModelRenderer[] tentacles = new ModelRenderer[9];

		public ModelGhast()
		{
			int i = -16;
			this.body = new ModelRenderer(this, 0, 0, ModelPart.Chest);
			this.body.addBox(-8.0F, -8.0F, -8.0F, 16, 16, 16, "Body");
			this.body.rotationPointY += (float)(24 + i);
			Random random = new Random(1660L);

			for (int j = 0; j < this.tentacles.Length; ++j)
			{
				this.tentacles[j] = new ModelRenderer(this, 0, 0, ModelPart.LeftLeg);
				float f = (((float)(j % 3) - (float)(j / 3 % 2) * 0.5F + 0.25F) / 2.0F * 2.0F - 1.0F) * 5.0F;
				float f1 = ((float)(j / 3) / 2.0F * 2.0F - 1.0F) * 5.0F;
				int k = random.nextInt(7) + 8;
				this.tentacles[j].addBox(-1.0F, 0.0F, -1.0F, 2, k, 2, "Tentacle");
				this.tentacles[j].rotationPointX = f;
				this.tentacles[j].rotationPointZ = f1;
				this.tentacles[j].rotationPointY = (float)(31 + i);
			}
		}

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			for (int i = 0; i < this.tentacles.Length; ++i)
			{
				this.tentacles[i].rotateAngleX = 0.2F * MathHelper.sin(ageInTicks * 0.3F + (float)i) + 0.4F;
			}
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			this.setRotationAngles(p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale, entityIn);
			GlStateManager.pushMatrix();
			GlStateManager.translate(0.0F, 0.6F, 0.0F);
			this.body.render(scale);

			for (ModelRenderer modelrenderer : this.tentacles)
			{
				modelrenderer.render(scale);
			}

			GlStateManager.popMatrix();
		}
#endif
	}
}
#endif