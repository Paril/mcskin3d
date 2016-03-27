#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelBook : ModelBase
	{
		/** Right cover renderer (when facing the book) */
		public ModelRenderer coverRight;

		/** Left cover renderer (when facing the book) */
		public ModelRenderer coverLeft;

		/** The right pages renderer (when facing the book) */
		public ModelRenderer pagesRight;

		/** The left pages renderer (when facing the book) */
		public ModelRenderer pagesLeft;

		/** Right cover renderer (when facing the book) */
		public ModelRenderer flippingPageRight;

		/** Right cover renderer (when facing the book) */
		public ModelRenderer flippingPageLeft;

		/** The renderer of spine of the book */
		public ModelRenderer bookSpine;

		public ModelBook()
		{
			coverRight = (new ModelRenderer(this)).setTextureOffset(0, 0).addBox(-6.0F, -5.0F, 0.0F, 6, 10, 0, "Cover Right");
			coverLeft = (new ModelRenderer(this)).setTextureOffset(16, 0).addBox(0.0F, -5.0F, 0.0F, 6, 10, 0, "Cover Left");
			pagesRight = (new ModelRenderer(this)).setTextureOffset(0, 10).addBox(0.0F, -4.0F, -0.99F, 5, 8, 1, "Pages Right");
			pagesLeft = (new ModelRenderer(this)).setTextureOffset(12, 10).addBox(0.0F, -4.0F, -0.01F, 5, 8, 1, "Pages Left");
			flippingPageRight = (new ModelRenderer(this)).setTextureOffset(24, 10).addBox(0.0F, -4.0F, 0.0F, 5, 8, 0, "Flipping Page Right");
			flippingPageLeft = (new ModelRenderer(this)).setTextureOffset(24, 10).addBox(0.0F, -4.0F, 0.0F, 5, 8, 0, "Flipping Page Left");
			bookSpine = (new ModelRenderer(this)).setTextureOffset(12, 0).addBox(-1.0F, -5.0F, 0.0F, 2, 10, 0, "Book Spine");

			this.coverRight.setRotationPoint(0.0F, 0.0F, -1.0F);
			this.coverLeft.setRotationPoint(0.0F, 0.0F, 1.0F);
			this.bookSpine.rotateAngleY = ((float)Math.PI / 2F);
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			this.setRotationAngles(p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale, entityIn);
			this.coverRight.render(scale);
			this.coverLeft.render(scale);
			this.bookSpine.render(scale);
			this.pagesRight.render(scale);
			this.pagesLeft.render(scale);
			this.flippingPageRight.render(scale);
			this.flippingPageLeft.render(scale);
		}
#endif

		/**
		 * Sets the model's various rotation angles. For bipeds, par1 and par2 are used for animating the movement of arms
		 * and legs, where par1 represents the time(so that arms and legs swing back and forth) and par2 represents how
		 * "far" arms and legs can swing at most.
		 */
		public void setRotationAngles(float limbSwing, float limbSwingAmount, float ageInTicks, float netHeadYaw, float headPitch, float scaleFactor, Entity entityIn)
		{
			float f = (MathHelper.sin(limbSwing * 0.02F) * 0.1F + 1.25F) * netHeadYaw;
			this.coverRight.rotateAngleY = (float)Math.PI + f;
			this.coverLeft.rotateAngleY = -f;
			this.pagesRight.rotateAngleY = f;
			this.pagesLeft.rotateAngleY = -f;
			this.flippingPageRight.rotateAngleY = f - f * 2.0F * limbSwingAmount;
			this.flippingPageLeft.rotateAngleY = f - f * 2.0F * ageInTicks;
			this.pagesRight.rotationPointX = MathHelper.sin(f);
			this.pagesLeft.rotationPointX = MathHelper.sin(f);
			this.flippingPageRight.rotationPointX = MathHelper.sin(f);
			this.flippingPageLeft.rotationPointX = MathHelper.sin(f);
		}
	}
}
#endif