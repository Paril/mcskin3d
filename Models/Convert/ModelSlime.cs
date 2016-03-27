#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelSlime : ModelBase
	{
		/** The slime's bodies, both the inside box and the outside box */
		ModelRenderer slimeBodies;

		/** The slime's right eye */
		ModelRenderer slimeRightEye;

		/** The slime's left eye */
		ModelRenderer slimeLeftEye;

		/** The slime's mouth */
		ModelRenderer slimeMouth;

		ModelRenderer slimeOutside;

		public ModelSlime(int p_i1157_1_)
		{
			this.slimeBodies = new ModelRenderer(this, 0, p_i1157_1_, ModelPart.Head);
			this.slimeBodies.addBox(-3.0F, 17.0F, -3.0F, 6, 6, 6, "Body");
			this.slimeRightEye = new ModelRenderer(this, 32, 0);
			this.slimeRightEye.addBox(-3.25F, 18.0F, -3.5F, 2, 2, 2, "Right Eye");
			this.slimeLeftEye = new ModelRenderer(this, 32, 4);
			this.slimeLeftEye.addBox(1.25F, 18.0F, -3.5F, 2, 2, 2, "Left Eye");
			this.slimeMouth = new ModelRenderer(this, 32, 8);
			this.slimeMouth.addBox(0.0F, 21.0F, -3.5F, 1, 1, 1, "Mouth");

			this.slimeOutside = new ModelRenderer(this, 0, 0, ModelPart.Helmet);
			this.slimeOutside.addBox(-4.0F, 16.0F, -4.0F, 8, 8, 8, "Outside");
		}

#if RENDER
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			this.setRotationAngles(p_78088_2_, p_78088_3_, p_78088_4_, p_78088_5_, p_78088_6_, scale, entityIn);
			GlStateManager.translate(0.0F, 0.001F, 0.0F);
			this.slimeBodies.render(scale);

			if (this.slimeRightEye != null)
			{
				this.slimeRightEye.render(scale);
				this.slimeLeftEye.render(scale);
				this.slimeMouth.render(scale);
			}
		}
#endif
	}
}
#endif