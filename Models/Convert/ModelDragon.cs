#if CONVERT_MODELS
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelDragon : ModelBase
	{
		/** The head Model renderer of the dragon */
		private ModelRenderer head;

		/** The spine Model renderer of the dragon */
		private ModelRenderer[] spine = new ModelRenderer[5 + 12];

		/** The jaw Model renderer of the dragon */
		private ModelRenderer jaw;

		/** The body Model renderer of the dragon */
		private ModelRenderer body;

		/** The rear leg Model renderer of the dragon */
		private ModelRenderer[] rearLegs = new ModelRenderer[2];

		/** The front leg Model renderer of the dragon */
		private ModelRenderer[] frontLegs = new ModelRenderer[2];

		/** The rear leg tip Model renderer of the dragon */
		private ModelRenderer[] rearLegTips = new ModelRenderer[2];

		/** The front leg tip Model renderer of the dragon */
		private ModelRenderer[] frontLegTips = new ModelRenderer[2];

		/** The rear foot Model renderer of the dragon */
		private ModelRenderer[] rearFoots = new ModelRenderer[2];

		/** The front foot Model renderer of the dragon */
		private ModelRenderer[] frontFoots = new ModelRenderer[2];

		/** The wing Model renderer of the dragon */
		private ModelRenderer[] wings = new ModelRenderer[2];

		/** The wing tip Model renderer of the dragon */
		private ModelRenderer[] wingTips = new ModelRenderer[2];
		private float partialTicks;

		public ModelDragon(float p_i46360_1_)
		{
			this.textureWidth = 256;
			this.textureHeight = 256;
			this.setTextureOffset("body.body", 0, 0);

			this.setTextureOffset("leftWing.skin", -56, 88);
			this.setTextureOffset("leftWingtip.skin", -56, 144);
			this.setTextureOffset("leftWing.bone", 112, 88);
			this.setTextureOffset("leftWingtip.bone", 112, 136);

			this.setTextureOffset("rightWing.skin", -56, 88);
			this.setTextureOffset("rightWingtip.skin", -56, 144);
			this.setTextureOffset("rightWing.bone", 112, 88);
			this.setTextureOffset("rightWingtip.bone", 112, 136);

			this.setTextureOffset("rightrearleg.main", 0, 0);
			this.setTextureOffset("rightrearfoot.main", 112, 0);
			this.setTextureOffset("rightrearlegtip.main", 196, 0);
								   
			this.setTextureOffset("rightfrontleg.main", 112, 104);
			this.setTextureOffset("rightfrontfoot.main", 144, 104);
			this.setTextureOffset("rightfrontlegtip.main", 226, 138);

			this.setTextureOffset("leftrearleg.main", 0, 0);
			this.setTextureOffset("leftrearfoot.main", 112, 0);
			this.setTextureOffset("leftrearlegtip.main", 196, 0);

			this.setTextureOffset("leftfrontleg.main", 112, 104);
			this.setTextureOffset("leftfrontfoot.main", 144, 104);
			this.setTextureOffset("leftfrontlegtip.main", 226, 138);

			this.setTextureOffset("head.upperhead", 112, 30);
			this.setTextureOffset("head.upperlip", 176, 44);
			this.setTextureOffset("jaw.jaw", 176, 65);
			this.setTextureOffset("neck.box", 192, 104);
			this.setTextureOffset("body.scale", 220, 53);
			this.setTextureOffset("head.scale", 0, 0);
			this.setTextureOffset("neck.scale", 48, 0);
			this.setTextureOffset("head.nostril", 112, 0);
			float f = -16.0F;
			this.head = new ModelRenderer(this, "head");
			this.head.addBox("upperlip", -6.0F, -1.0F, -8.0F + f, 12, 5, 16);
			this.head.addBox("upperhead", -8.0F, -8.0F, 6.0F + f, 16, 16, 16);
			this.head.mirror = true;
			this.head.addBox("scale", -5.0F, -12.0F, 12.0F + f, 2, 4, 6);
			this.head.addBox("nostril", -5.0F, -3.0F, -6.0F + f, 2, 2, 4);
			this.head.mirror = false;
			this.head.addBox("scale", 3.0F, -12.0F, 12.0F + f, 2, 4, 6);
			this.head.addBox("nostril", 3.0F, -3.0F, -6.0F + f, 2, 2, 4);
			this.jaw = new ModelRenderer(this, "jaw");
			this.jaw.setRotationPoint(0.0F, 4.0F, 8.0F + f);
			this.jaw.addBox("jaw", -6.0F, 0.0F, -16.0F, 12, 4, 16);
			this.head.addChild(this.jaw);
			for (var i = 0; i < this.spine.Length; ++i)
			{
				this.spine[i] = new ModelRenderer(this, "neck");
				this.spine[i].addBox("box", -5.0F, -5.0F, -5.0F, 10, 10, 10);
				this.spine[i].addBox("scale", -1.0F, -9.0F, -3.0F, 2, 4, 6);
			}

			this.body = new ModelRenderer(this, "body");
			this.body.setRotationPoint(0.0F, 4.0F, 8.0F);
			this.body.addBox("body", -12.0F, 0.0F, -16.0F, 24, 24, 64);
			this.body.addBox("scale", -1.0F, -6.0F, -10.0F, 2, 6, 12);
			this.body.addBox("scale", -1.0F, -6.0F, 10.0F, 2, 6, 12);
			this.body.addBox("scale", -1.0F, -6.0F, 30.0F, 2, 6, 12);

			this.wings[0] = new ModelRenderer(this, "rightWing");
			this.wings[0].setRotationPoint(-12.0F, 5.0F, 2.0F);
			this.wings[0].addBox("bone", -56.0F, -4.0F, -4.0F, 56, 8, 8);
			this.wings[0].addBox("skin", -56.0F, 0.0F, 2.0F, 56, 0, 56);
			this.wingTips[0] = new ModelRenderer(this, "rightWingtip");
			this.wingTips[0].setRotationPoint(-56.0F, 0.0F, 0.0F);
			this.wingTips[0].addBox("bone", -56.0F, -2.0F, -2.0F, 56, 4, 4);
			this.wingTips[0].addBox("skin", -56.0F, 0.0F, 2.0F, 56, 0, 56);
			this.wings[0].addChild(this.wingTips[0]);

			this.wings[1] = new ModelRenderer(this, "leftWing");
			this.wings[1].setRotationPoint(-44.0F, 5.0F, 2.0F);
			this.wings[1].addBox("bone", 56.0F, -4.0F, -4.0F, 56, 8, 8, true);
			this.wings[1].addBox("skin", 56.0F, 0.0F, 2.0F, 56, 0, 56, true);
			this.wingTips[1] = new ModelRenderer(this, "leftWingtip");
			this.wingTips[1].setRotationPoint(56.0F, 0.0F, 0.0F);
			this.wingTips[1].addBox("bone", 56.0F, -2.0F, -2.0F, 56, 4, 4, true);
			this.wingTips[1].addBox("skin", 56.0F, 0.0F, 2.0F, 56, 0, 56, true);
			this.wings[1].addChild(this.wingTips[1]);

			this.frontLegs[0] = new ModelRenderer(this, "leftfrontleg");
			this.frontLegs[0].setRotationPoint(-12.0F, 20.0F, 2.0F);
			this.frontLegs[0].addBox("main", -4.0F, -4.0F, -4.0F, 8, 24, 8);
			this.frontLegTips[0] = new ModelRenderer(this, "leftfrontlegtip");
			this.frontLegTips[0].setRotationPoint(0.0F, 4.0F, 18.0F);
			this.frontLegTips[0].addBox("main", -3.0F, -1.0F, -3.0F, 6, 24, 6);
			this.frontLegs[0].addChild(this.frontLegTips[0]);
			this.frontFoots[0] = new ModelRenderer(this, "leftfrontfoot");
			this.frontFoots[0].setRotationPoint(0.0F, 14.0F, 16.0F);
			this.frontFoots[0].addBox("main", -4.0F, 0.0F, -12.0F, 8, 4, 16);
			this.frontLegTips[0].addChild(this.frontFoots[0]);

			this.rearLegs[0] = new ModelRenderer(this, "leftrearleg");
			this.rearLegs[0].setRotationPoint(-16.0F, 16.0F, 42.0F);
			this.rearLegs[0].addBox("main", -8.0F, -4.0F, -8.0F, 16, 32, 16);
			this.rearLegTips[0] = new ModelRenderer(this, "leftrearlegtip");
			this.rearLegTips[0].setRotationPoint(0.0F, 21.0F, 22.0F);
			this.rearLegTips[0].addBox("main", -6.0F, -2.0F, 0.0F, 12, 32, 12);
			this.rearLegs[0].addChild(this.rearLegTips[0]);
			this.rearFoots[0] = new ModelRenderer(this, "leftrearfoot");
			this.rearFoots[0].setRotationPoint(0.0F, 0.0F, 30.0F);
			this.rearFoots[0].addBox("main", -9.0F, 0.0F, -20.0F, 18, 6, 24);
			this.rearLegTips[0].addChild(this.rearFoots[0]);

			this.frontLegs[1] = new ModelRenderer(this, "rightfrontleg");
			this.frontLegs[1].setRotationPoint(12.0F, 20.0F, 2.0F);
			this.frontLegs[1].addBox("main", -4.0F, -4.0F, -4.0F, 8, 24, 8, true);
			this.frontLegTips[1] = new ModelRenderer(this, "rightfrontlegtip");
			this.frontLegTips[1].setRotationPoint(0.0F, 4.0F, 18.0F);
			this.frontLegTips[1].addBox("main", -3.0F, -1.0F, -3.0F, 6, 24, 6, true);
			this.frontLegs[1].addChild(this.frontLegTips[1]);
			this.frontFoots[1] = new ModelRenderer(this, "rightfrontfoot");
			this.frontFoots[1].setRotationPoint(0.0F, 14.0F, 16.0F);
			this.frontFoots[1].addBox("main", -4.0F, 0.0F, -12.0F, 8, 4, 16, true);
			this.frontLegTips[1].addChild(this.frontFoots[1]);

			this.rearLegs[1] = new ModelRenderer(this, "rightrearleg");
			this.rearLegs[1].setRotationPoint(16.0F, 16.0F, 42.0F);
			this.rearLegs[1].addBox("main", -8.0F, -4.0F, -8.0F, 16, 32, 16, true);
			this.rearLegTips[1] = new ModelRenderer(this, "rightrearlegtip");
			this.rearLegTips[1].setRotationPoint(0.0F, 21.0F, 22.0F);
			this.rearLegTips[1].addBox("main", -6.0F, -2.0F, 0.0F, 12, 32, 12, true);
			this.rearLegs[1].addChild(this.rearLegTips[1]);
			this.rearFoots[1] = new ModelRenderer(this, "rightrearfoot");
			this.rearFoots[1].setRotationPoint(0.0F, 0.0F, 30.0F);
			this.rearFoots[1].addBox("main", -9.0F, 0.0F, -20.0F, 18, 6, 24, true);
			this.rearLegTips[1].addChild(this.rearFoots[1]);
		}

		/**
		 * Used for easily adding entity-dependent animations. The second and third float params here are the same second
		 * and third as in the setRotationAngles method.
		 */
		public void setLivingAnimations(EntityLivingBase entitylivingbaseIn, float p_78086_2_, float p_78086_3_, float partialTickTime)
		{
			this.partialTicks = partialTickTime;
		}
		
		/**
		 * Sets the models various rotation angles then renders the model.
		 */
		public void render(Entity entityIn, float p_78088_2_, float p_78088_3_, float p_78088_4_, float p_78088_5_, float p_78088_6_, float scale)
		{
			//GlStateManager.pushMatrix();
			EntityDragon entitydragon = (EntityDragon)entityIn;
			float f = entitydragon.prevAnimTime + (entitydragon.animTime - entitydragon.prevAnimTime) * this.partialTicks;
			this.jaw.rotateAngleX = (float)(Math.sin((double)(f * ((float)Math.PI * 2F))) + 1.0D) * 0.2F;
			float f1 = (float)(Math.sin((double)(f * ((float)Math.PI * 2F) - 1.0F)) + 1.0D);
			f1 = (f1 * f1 + f1 * 2.0F) * 0.05F;
			//GlStateManager.translate(0.0F, f1 - 2.0F, -3.0F);
			//GlStateManager.rotate(f1 * 2.0F, 1.0F, 0.0F, 0.0F);
			float f2 = -30.0F;
			float f4 = 0.0F;
			float f5 = 1.5F;
			double[] adouble = entitydragon.getMovementOffsets(6, this.partialTicks);
			float f6 = this.updateRotations(entitydragon.getMovementOffsets(5, this.partialTicks)[0] - entitydragon.getMovementOffsets(10, this.partialTicks)[0]);
			float f7 = this.updateRotations(entitydragon.getMovementOffsets(5, this.partialTicks)[0] + (double)(f6 / 2.0F));
			f2 = f2 + 2.0F;
			float f8 = f * ((float)Math.PI * 2F);
			f2 = 20.0F;
			float f3 = -12.0F;

			float rotateAngleZ, rotateAngleY, rotateAngleX;
			float rotationPointX, rotationPointY, rotationPointZ;

			for (int i = 0; i < 5; ++i)
			{
				double[] adouble1 = entitydragon.getMovementOffsets(5 - i, this.partialTicks);
				float f9 = (float)Math.cos((double)((float)i * 0.45F + f8)) * 0.15F;
				rotateAngleY = this.updateRotations(adouble1[0] - adouble[0]) * 0.017453292F * f5;
				rotateAngleX = f9 + entitydragon.func_184667_a(i, adouble, adouble1) * 0.017453292F * f5 * 5.0F;
				rotateAngleZ = -this.updateRotations(adouble1[0] - (double)f7) * 0.017453292F * f5;
				rotationPointY = f2;
				rotationPointZ = f3;
				rotationPointX = f4;
				f2 = (float)((double)f2 + Math.sin((double)rotateAngleX) * 10.0D);
				f3 = (float)((double)f3 - Math.cos((double)rotateAngleY) * Math.cos((double)rotateAngleX) * 10.0D);
				f4 = (float)((double)f4 - Math.sin((double)rotateAngleY) * Math.cos((double)rotateAngleX) * 10.0D);
				//this.spine.render(scale);

				this.spine[i].rotateAngleX = rotateAngleX;
				this.spine[i].rotateAngleY = rotateAngleY;
				this.spine[i].rotateAngleZ = rotateAngleZ;
				this.spine[i].rotationPointX = rotationPointX;
				this.spine[i].rotationPointY = rotationPointY;
				this.spine[i].rotationPointZ = rotationPointZ;
			}

			this.head.rotationPointY = f2;
			this.head.rotationPointZ = f3;
			this.head.rotationPointX = f4;
			double[] adouble2 = entitydragon.getMovementOffsets(0, this.partialTicks);
			this.head.rotateAngleY = this.updateRotations(adouble2[0] - adouble[0]) * 0.017453292F;
			this.head.rotateAngleX = this.updateRotations((double)entitydragon.func_184667_a(6, adouble, adouble2)) * 0.017453292F * f5 * 5.0F;
			this.head.rotateAngleZ = -this.updateRotations(adouble2[0] - (double)f7) * 0.017453292F;
			//this.head.render(scale);
			//GlStateManager.pushMatrix();
			//GlStateManager.translate(0.0F, 1.0F, 0.0F);
			//GlStateManager.rotate(-f6 * f5, 0.0F, 0.0F, 1.0F);
			//GlStateManager.translate(0.0F, -1.0F, 0.0F);
			this.body.rotateAngleZ = 0.0F;
			//this.body.render(scale);

			for (int j = 0; j < 2; ++j)
			{
				//GlStateManager.enableCull();
				float f11 = f * ((float)Math.PI * 2F);
				this.wings[j].rotateAngleX = 0;// 0.125F - (float)Math.cos((double)f11) * 0.2F;
				this.wings[j].rotateAngleY = 0;// 0.25F;
				this.wings[j].rotateAngleZ = 0;// (float)(Math.sin((double)f11) + 0.125D) * 0.8F;
				this.wingTips[j].rotateAngleZ = 0;// -((float)(Math.sin((double)(f11 + 2.0F)) + 0.5D)) * 0.75F;
				this.rearLegs[j].rotateAngleX = 1.0F + f1 * 0.1F;
				this.rearLegTips[j].rotateAngleX = 0.5F + f1 * 0.1F;
				this.rearFoots[j].rotateAngleX = 0.75F + f1 * 0.1F;
				this.frontLegs[j].rotateAngleX = 1.3F + f1 * 0.1F;
				this.frontLegTips[j].rotateAngleX = -0.5F - f1 * 0.1F;
				this.frontFoots[j].rotateAngleX = 0.75F + f1 * 0.1F;
				//this.wing.render(scale);
				//this.frontLeg.render(scale);
				//this.rearLeg.render(scale);
				//GlStateManager.scale(-1.0F, 1.0F, 1.0F);

				if (j == 0)
				{
					//GlStateManager.cullFace(GlStateManager.CullFace.FRONT);
				}
			}

			//GlStateManager.popMatrix();
			//GlStateManager.cullFace(GlStateManager.CullFace.BACK);
			//GlStateManager.disableCull();
			float f10 = -((float)Math.sin((double)(f * ((float)Math.PI * 2F)))) * 0.0F;
			f8 = f * ((float)Math.PI * 2F);
			f2 = 10.0F;
			f3 = 60.0F;
			f4 = 0.0F;
			adouble = entitydragon.getMovementOffsets(11, this.partialTicks);

			for (int k = 0; k < 12; ++k)
			{
				adouble2 = entitydragon.getMovementOffsets(12 + k, this.partialTicks);
				f10 = (float)((double)f10 + Math.sin((double)((float)k * 0.45F + f8)) * 0.05000000074505806D);
				rotateAngleY = (this.updateRotations(adouble2[0] - adouble[0]) * f5 + 180.0F) * 0.017453292F;
				rotateAngleX = f10 + (float)(adouble2[1] - adouble[1]) * 0.017453292F * f5 * 5.0F;
				rotateAngleZ = this.updateRotations(adouble2[0] - (double)f7) * 0.017453292F * f5;
				rotationPointY = f2;
				rotationPointZ = f3;
				rotationPointX = f4;
				f2 = (float)((double)f2 + Math.sin((double)rotateAngleX) * 10.0D);
				f3 = (float)((double)f3 - Math.cos((double)rotateAngleY) * Math.cos((double)rotateAngleX) * 10.0D);
				f4 = (float)((double)f4 - Math.sin((double)rotateAngleY) * Math.cos((double)rotateAngleX) * 10.0D);
				//this.spine.render(scale);

				this.spine[k + 5].rotateAngleX = rotateAngleX;
				this.spine[k + 5].rotateAngleY = rotateAngleY;
				this.spine[k + 5].rotateAngleZ = rotateAngleZ;
				this.spine[k + 5].rotationPointX = rotationPointX;
				this.spine[k + 5].rotationPointY = rotationPointY;
				this.spine[k + 5].rotationPointZ = rotationPointZ;
			}

			//GlStateManager.popMatrix();
		}

		/**
		 * Updates the rotations in the parameters for rotations greater than 180 degrees or less than -180 degrees. It adds
		 * or subtracts 360 degrees, so that the appearance is the same, although the numbers are then simplified to range
		 * -180 to 180
		 */
		private float updateRotations(double p_78214_1_)
		{
			while (p_78214_1_ >= 180.0D)
			{
				p_78214_1_ -= 360.0D;
			}

			while (p_78214_1_ < -180.0D)
			{
				p_78214_1_ += 360.0D;
			}

			return (float)p_78214_1_;
		}
	}
}
#endif