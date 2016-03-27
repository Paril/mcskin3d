#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelEnderCrystal : ModelBase
	{
		/** The cube model for the Ender Crystal. */
		private ModelRenderer cube;

		/** The glass model for the Ender Crystal. */
		private ModelRenderer glass;

		/** The base model for the Ender Crystal. */
		private ModelRenderer baseM;

		public ModelEnderCrystal(float p_i1170_1_, boolean p_i1170_2_)
		{
			this.glass = new ModelRenderer(this, "glass");
			this.glass.setTextureOffset(0, 0).addBox(-4.0F, -4.0F, -4.0F, 8, 8, 8, "glass");
			this.glass.setRotationPoint(0, -8f, 0);
			this.glass.rotateAngleX = ((float)Math.PI / 3F);
			this.glass.rotateAngleY = ((float)Math.PI / 3F);
			this.cube = new ModelRenderer(this, "cube");
			this.cube.setTextureOffset(32, 0).addBox(-4.0F, -12.0F, -4.0F, 8, 8, 8, "cube");

			if (p_i1170_2_)
			{
				this.baseM = new ModelRenderer(this, "base");
				this.baseM.setTextureOffset(0, 16).addBox(-6.0F, 0.0F, -6.0F, 12, 4, 12, "cube");
			}
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			GlStateManager.pushMatrix();
			GlStateManager.scale(2.0F, 2.0F, 2.0F);
			GlStateManager.translate(0.0F, -0.5F, 0.0F);

			if (this.base != null)
			{
				this.base.render(scale);
			}

			GlStateManager.rotate(p_78088_3_, 0.0F, 1.0F, 0.0F);
			GlStateManager.translate(0.0F, 0.8F + p_78088_4_, 0.0F);
			GlStateManager.rotate(60.0F, 0.7071F, 0.0F, 0.7071F);
			this.glass.render(scale);
			float f = 0.875F;
			GlStateManager.scale(f, f, f);
			GlStateManager.rotate(60.0F, 0.7071F, 0.0F, 0.7071F);
			GlStateManager.rotate(p_78088_3_, 0.0F, 1.0F, 0.0F);
			this.glass.render(scale);
			GlStateManager.scale(f, f, f);
			GlStateManager.rotate(60.0F, 0.7071F, 0.0F, 0.7071F);
			GlStateManager.rotate(p_78088_3_, 0.0F, 1.0F, 0.0F);
			this.cube.render(scale);
			GlStateManager.popMatrix();
		}
#endif
	}
}
#endif