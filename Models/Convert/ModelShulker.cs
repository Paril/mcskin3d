#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelShulker : ModelBase
	{
		private ModelRenderer field_187067_b;
		private ModelRenderer field_187068_c;
		public ModelRenderer field_187066_a;

		public ModelShulker()
		{
			this.textureHeight = 64;
			this.textureWidth = 64;
			this.field_187068_c = new ModelRenderer(this);
			this.field_187067_b = new ModelRenderer(this);
			this.field_187066_a = new ModelRenderer(this);
			this.field_187068_c.setTextureOffset(0, 0).addBox(-8.0F, -16.0F, -8.0F, 16, 12, 16, "Top");
			this.field_187068_c.setRotationPoint(0.0F, 24.0F, 0.0F);
			this.field_187067_b.setTextureOffset(0, 28).addBox(-8.0F, -8.0F, -8.0F, 16, 8, 16, "Bottom");
			this.field_187067_b.setRotationPoint(0.0F, 24.0F, 0.0F);
			this.field_187066_a.setTextureOffset(0, 52).addBox(-3.0F, 0.0F, -3.0F, 6, 6, 6, "Head");
			this.field_187066_a.setRotationPoint(0.0F, 12.0F, 0.0F);
		}

		public int func_187065_a()
		{
			return 28;
		}

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			EntityShulker entityshulker = (EntityShulker)entityIn;
			float f = ageInTicks - (float)entityshulker.ticksExisted;
			float f1 = (0.5F + entityshulker.func_184688_a(f)) * (float)Math.PI;
			float f2 = -1.0F + MathHelper.sin(f1);
			float f3 = 0.0F;

			if (f1 > (float)Math.PI)
			{
				f3 = MathHelper.sin(ageInTicks * 0.1F) * 0.7F;
			}

			this.field_187068_c.setRotationPoint(0.0F, 16.0F + MathHelper.sin(f1) * 8.0F + f3, 0.0F);

			if (entityshulker.func_184688_a(f) > 0.3F)
			{
				this.field_187068_c.rotateAngleY = f2 * f2 * f2 * f2 * (float)Math.PI * 0.125F;
			}
			else
			{
				this.field_187068_c.rotateAngleY = 0.0F;
			}

			this.field_187066_a.rotateAngleX = headPitch * 0.017453292F;
			this.field_187066_a.rotateAngleY = netHeadYaw * 0.017453292F;
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			this.field_187067_b.render(scale);
			this.field_187068_c.render(scale);
		}
#endif
	}
}
#endif