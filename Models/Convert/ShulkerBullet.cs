#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelShulkerBullet : ModelBase
	{
		public ModelRenderer field_187069_a;

		public ModelShulkerBullet()
		{
			this.textureWidth = 64;
			this.textureHeight = 32;
			this.field_187069_a = new ModelRenderer(this);
			this.field_187069_a.setTextureOffset(0, 0).addBox(-4.0F, -4.0F, -1.0F, 8, 8, 2, 0.0F, "Bullet Piece");
			this.field_187069_a.setTextureOffset(0, 10).addBox(-1.0F, -4.0F, -4.0F, 2, 8, 8, 0.0F, "Bullet Piece");
			this.field_187069_a.setTextureOffset(20, 0).addBox(-4.0F, -1.0F, -4.0F, 8, 2, 8, 0.0F, "Bullet Piece");
			this.field_187069_a.setRotationPoint(0.0F, 0.0F, 0.0F);
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			this.setRotationAngles(p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale, entityIn);
			this.field_187069_a.render(scale);
		}
#endif

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			this.field_187069_a.rotateAngleY = netHeadYaw * 0.017453292F;
			this.field_187069_a.rotateAngleX = headPitch * 0.017453292F;
		}
	}
}
#endif