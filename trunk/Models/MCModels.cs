using System;

namespace MCSkin3D.Models
{
	public class ModelQuadruped : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer body;
		protected float field_40331_g;
		protected float field_40332_n;
		public ModelLoader.ModelRenderer head;
		public ModelLoader.ModelRenderer leg1;
		public ModelLoader.ModelRenderer leg2;
		public ModelLoader.ModelRenderer leg3;
		public ModelLoader.ModelRenderer leg4;

		public ModelQuadruped(int i, float f)
		{
			field_40331_g = 8F;
			field_40332_n = 4F;
			head = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			head.addBox("Head", -4F, -4F, -8F, 8, 8, 8, f);
			head.setRotationPoint(0.0F, 18 - i, -6F);
			body = new ModelLoader.ModelRenderer(this, 28, 8, ModelPart.Chest);
			body.addBox("Body", -5F, -10F, -7F, 10, 16, 8, f);
			body.setRotationPoint(0.0F, 17 - i, 2.0F);
			body.rotateAngleX = 1.570796F;
			leg1 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
			leg1.addBox("Left Leg", -2F, 0.0F, -2F, 4, i, 4, f);
			leg1.setRotationPoint(-3F, 24 - i, 7F);
			leg2 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightLeg);
			leg2.mirror = true;
			leg2.addBox("Right Leg", -2F, 0.0F, -2F, 4, i, 4, f);
			leg2.setRotationPoint(3F, 24 - i, 7F);
			leg3 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftArm);
			leg3.addBox("Left Arm", -2F, 0.0F, -2F, 4, i, 4, f);
			leg3.setRotationPoint(-3F, 24 - i, -5F);
			leg4 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightArm);
			leg4.addBox("Right Arm", -2F, 0.0F, -2F, 4, i, 4, f);
			leg4.setRotationPoint(3F, 24 - i, -5F);
			leg4.mirror = true;
		}
	}

	public class ModelPig : ModelQuadruped
	{
		public ModelPig() :
			this(0)
		{
		}

		public ModelPig(float f) :
			base(6, f)
		{
			head.setTextureOffset(16, 16).addBox("Snout", -2F, 0.0F, -9F, 4, 3, 1, f);
			field_40331_g = 4F;
		}
	}


	public class ModelBiped : ModelLoader.ModelBase
	{
		public bool aimedBow;
		public ModelLoader.ModelRenderer bipedBody;
		public ModelLoader.ModelRenderer bipedCloak;
		public ModelLoader.ModelRenderer bipedEars;
		public ModelLoader.ModelRenderer bipedHead;
		public ModelLoader.ModelRenderer bipedHeadwear;
		public ModelLoader.ModelRenderer bipedLeftArm;
		public ModelLoader.ModelRenderer bipedLeftLeg;
		public ModelLoader.ModelRenderer bipedRightArm;
		public ModelLoader.ModelRenderer bipedRightLeg;
		public int heldItemLeft;
		public int heldItemRight;
		public bool isSneak;

		public ModelBiped() :
			this(0.0F)
		{
		}

		public ModelBiped(float f) :
			this(f, 0.0F)
		{
		}

		public ModelBiped(float f, float f1)
		{
			heldItemLeft = 0;
			heldItemRight = 0;
			isSneak = false;
			aimedBow = false;
			/*bipedCloak = new ModelRenderer(this, 0, 0);
			bipedCloak.addBox(-5F, 0.0F, -1F, 10, 16, 1, f);
			bipedEars = new ModelRenderer(this, 24, 0);
			bipedEars.addBox(-3F, -6F, -1F, 6, 6, 1, f);*/
			bipedHead = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			bipedHead.addBox("Head", -4F, -8F, -4F, 8, 8, 8, f);
			bipedHead.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
			bipedHeadwear = new ModelLoader.ModelRenderer(this, 32, 0, ModelPart.Helmet);
			bipedHeadwear.addBox("Headwear", -4F, -8F, -4F, 8, 8, 8, f + 0.5F);
			bipedHeadwear.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
			bipedBody = new ModelLoader.ModelRenderer(this, 16, 16, ModelPart.Chest);
			bipedBody.addBox("Body", -4F, 0.0F, -2F, 8, 12, 4, f);
			bipedBody.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
			bipedRightArm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.RightArm);
			bipedRightArm.addBox("Right Arm", -3F, -2F, -2F, 4, 12, 4, f);
			bipedRightArm.setRotationPoint(-5F, 2.0F + f1, 0.0F);
			bipedLeftArm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.LeftArm);
			bipedLeftArm.mirror = true;
			bipedLeftArm.addBox("Left Arm", -1F, -2F, -2F, 4, 12, 4, f);
			bipedLeftArm.setRotationPoint(5F, 2.0F + f1, 0.0F);
			bipedRightLeg = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightLeg);
			bipedRightLeg.addBox("Right Leg", -2F, 0.0F, -2F, 4, 12, 4, f);
			bipedRightLeg.setRotationPoint(-2F, 12F + f1, 0.0F);
			bipedLeftLeg = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
			bipedLeftLeg.mirror = true;
			bipedLeftLeg.addBox("Left Leg", -2F, 0.0F, -2F, 4, 12, 4, f);
			bipedLeftLeg.setRotationPoint(2.0F, 12F + f1, 0.0F);
		}
	}

	public class ModelArmor : ModelBiped
	{
		public ModelArmor() :
			base(0.5f)
		{
			boxList.Remove(bipedHeadwear);
		}
	}

	public class ModelCloak : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer bipedCloak;

		public ModelCloak()
		{
			bipedCloak = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.LeftLeg);
			bipedCloak.addBox("Cloak", -5F, 0.0F, -1F, 10, 16, 1, 0);
		}
	}

	public class ModelVillager : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer arms;
		public ModelLoader.ModelRenderer body, bodyOverwear;
		public int field_40334_f;
		public int field_40335_g;
		public ModelLoader.ModelRenderer field_40336_d;
		public ModelLoader.ModelRenderer field_40337_e;
		public bool field_40341_n;
		public bool field_40342_o;
		public ModelLoader.ModelRenderer head;

		public ModelVillager() :
			this(0)
		{
		}

		public ModelVillager(float f) :
			this(f, 0)
		{
		}

		public ModelVillager(float f, float f1)
		{
			field_40334_f = 0;
			field_40335_g = 0;
			field_40341_n = false;
			field_40342_o = false;
			byte byte0 = 64;
			byte byte1 = 64;
			head = (new ModelLoader.ModelRenderer(this, ModelPart.Head)).setTextureSize(byte0, byte1);
			head.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
			head.setTextureOffset(0, 0).addBox("Head", -4F, -10F, -4F, 8, 10, 8, f);
			head.setTextureOffset(24, 0).addBox("Nose", -1F, -3F, -6F, 2, 4, 2, f);
			arms = (new ModelLoader.ModelRenderer(this, ModelPart.LeftArm)).setTextureSize(byte0, byte1);
			arms.rotationPointY = 3F;
			arms.rotationPointZ = -1F;
			arms.rotateAngleX = -0.75F;
			arms.setTextureOffset(44, 22).addBox("Arms", -8F, -2F, -2F, 4, 8, 4, f);
			arms.setTextureOffset(44, 22).addBox("Arms", 4F, -2F, -2F, 4, 8, 4, f);
			arms.setTextureOffset(40, 38).addBox("Arms", -4F, 2.0F, -2F, 8, 4, 4, f);
			field_40336_d = (new ModelLoader.ModelRenderer(this, 0, 22, ModelPart.LeftLeg)).setTextureSize(byte0, byte1);
			field_40336_d.setRotationPoint(-2F, 12F + f1, 0.0F);
			field_40336_d.addBox("Left Leg", -2F, 0.0F, -2F, 4, 12, 4, f);
			field_40337_e = (new ModelLoader.ModelRenderer(this, 0, 22, ModelPart.RightLeg)).setTextureSize(byte0, byte1);
			field_40337_e.mirror = true;
			field_40337_e.setRotationPoint(2.0F, 12F + f1, 0.0F);
			field_40337_e.addBox("Left Leg", -2F, 0.0F, -2F, 4, 12, 4, f);
			body = (new ModelLoader.ModelRenderer(this, ModelPart.Chest)).setTextureSize(byte0, byte1);
			body.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
			body.setTextureOffset(16, 20).addBox("Body", -4F, 0.0F, -3F, 8, 12, 6, f);
			bodyOverwear = (new ModelLoader.ModelRenderer(this, ModelPart.Chest)).setTextureSize(byte0, byte1);
			bodyOverwear.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
			bodyOverwear.setTextureOffset(0, 38).addBox("Overwear", -4F, 0.0F, -3F, 8, 18, 6, f + 0.5F);
		}
	}

	public class ModelCreeper : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer body;
		public ModelLoader.ModelRenderer head;
		public ModelLoader.ModelRenderer leg1;
		public ModelLoader.ModelRenderer leg2;
		public ModelLoader.ModelRenderer leg3;
		public ModelLoader.ModelRenderer leg4;
		public ModelLoader.ModelRenderer unusedCreeperHeadwear;

		public ModelCreeper() :
			this(0)
		{
		}

		public ModelCreeper(float f)
		{
			int i = 4;
			head = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			head.addBox("Head", -4F, -8F, -4F, 8, 8, 8, f);
			head.setRotationPoint(0.0F, i, 0.0F);
			//unusedCreeperHeadwear = new ModelRenderer(this, 32, 0);
			//unusedCreeperHeadwear.addBox(-4F, -8F, -4F, 8, 8, 8, f + 0.5F);
			//unusedCreeperHeadwear.setRotationPoint(0.0F, i, 0.0F);
			body = new ModelLoader.ModelRenderer(this, 16, 16, ModelPart.Chest);
			body.addBox("Body", -4F, 0.0F, -2F, 8, 12, 4, f);
			body.setRotationPoint(0.0F, i, 0.0F);
			leg1 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
			leg1.addBox("Left Leg", -2F, 0.0F, -2F, 4, 6, 4, f);
			leg1.setRotationPoint(-2F, 12 + i, 4F);
			leg2 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightLeg);
			leg2.addBox("Right Leg", -2F, 0.0F, -2F, 4, 6, 4, f);
			leg2.setRotationPoint(2.0F, 12 + i, 4F);
			leg2.mirror = true;
			leg3 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftArm);
			leg3.addBox("Left Arm", -2F, 0.0F, -2F, 4, 6, 4, f);
			leg3.setRotationPoint(-2F, 12 + i, -4F);
			leg4 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightArm);
			leg4.addBox("Right Arm", -2F, 0.0F, -2F, 4, 6, 4, f);
			leg4.setRotationPoint(2.0F, 12 + i, -4F);
			leg4.mirror = true;
		}
	}

	public class ModelCow : ModelQuadruped
	{
		public ModelCow() :
			base(12, 0)
		{
			boxList.Remove(head);
			head = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			head.addBox("Head", -4F, -4F, -6F, 8, 8, 6, 0.0F);
			head.setRotationPoint(0.0F, 4F, -8F);
			head.setTextureOffset(22, 0).addBox("Ear", -5F, -5F, -4F, 1, 3, 1, 0.0F);
			head.setTextureOffset(22, 0).addBox("Ear", 4F, -5F, -4F, 1, 3, 1, 0.0F);
			boxList.Remove(body);
			body = new ModelLoader.ModelRenderer(this, 18, 4, ModelPart.Chest);
			body.addBox("Body", -6F, -10F, -7F, 12, 18, 10, 0.0F);
			body.setRotationPoint(0.0F, 5F, 2.0F);
			body.setTextureOffset(52, 0).addBox("Udder", -2F, 2.0F, -8F, 4, 6, 1);
			body.rotateAngleX = 1.570796F;
			leg1.rotationPointX--;
			leg2.rotationPointX++;
			leg1.rotationPointZ += 0.0F;
			leg2.rotationPointZ += 0.0F;
			leg3.rotationPointX--;
			leg4.rotationPointX++;
			leg3.rotationPointZ--;
			leg4.rotationPointZ--;
			field_40332_n += 2.0F;
		}
	}

	public class ModelChicken : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer bill;
		public ModelLoader.ModelRenderer body;
		public ModelLoader.ModelRenderer chin;
		public ModelLoader.ModelRenderer head;
		public ModelLoader.ModelRenderer leftLeg;
		public ModelLoader.ModelRenderer leftWing;
		public ModelLoader.ModelRenderer rightLeg;
		public ModelLoader.ModelRenderer rightWing;

		public ModelChicken()
		{
			byte byte0 = 16;
			head = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			head.addBox("Head", -2F, -6F, -2F, 4, 6, 3, 0.0F);
			head.setRotationPoint(0.0F, -1 + byte0, -4F);
			bill = new ModelLoader.ModelRenderer(this, 14, 0, ModelPart.Head);
			bill.addBox("Bill", -2F, -4F, -4F, 4, 2, 2, 0.0F);
			bill.setRotationPoint(0.0F, -1 + byte0, -4F);
			chin = new ModelLoader.ModelRenderer(this, 14, 4, ModelPart.Head);
			chin.addBox("Chin", -1F, -2F, -3F, 2, 2, 2, 0.0F);
			chin.setRotationPoint(0.0F, -1 + byte0, -4F);
			body = new ModelLoader.ModelRenderer(this, 0, 9, ModelPart.Chest);
			body.addBox("Body", -3F, -4F, -3F, 6, 8, 6, 0.0F);
			body.setRotationPoint(0.0F, 0 + byte0, 0.0F);
			rightLeg = new ModelLoader.ModelRenderer(this, 26, 0, ModelPart.RightLeg);
			rightLeg.addBox("Right Leg", -1F, 0.0F, -3F, 3, 5, 3);
			rightLeg.setRotationPoint(-2F, 3 + byte0, 1.0F);
			leftLeg = new ModelLoader.ModelRenderer(this, 26, 0, ModelPart.LeftLeg);
			leftLeg.addBox("Left Leg", -1F, 0.0F, -3F, 3, 5, 3);
			leftLeg.setRotationPoint(1.0F, 3 + byte0, 1.0F);
			leftLeg.mirror = true;
			rightWing = new ModelLoader.ModelRenderer(this, 24, 13, ModelPart.RightArm);
			rightWing.addBox("Right Wing", 0.0F, 0.0F, -3F, 1, 4, 6);
			rightWing.setRotationPoint(-4F, -3 + byte0, 0.0F);
			leftWing = new ModelLoader.ModelRenderer(this, 24, 13, ModelPart.LeftArm);
			leftWing.addBox("Left Leg", -1F, 0.0F, -3F, 1, 4, 6);
			leftWing.setRotationPoint(4F, -3 + byte0, 0.0F);
		}
	}

	public class ModelSlime : ModelLoader.ModelBase
	{
		private readonly ModelLoader.ModelRenderer slimeBodies;
		private readonly ModelLoader.ModelRenderer slimeBodies2;
		private readonly ModelLoader.ModelRenderer slimeLeftEye;
		private readonly ModelLoader.ModelRenderer slimeMouth;
		private readonly ModelLoader.ModelRenderer slimeRightEye;

		public ModelSlime(int i)
		{
			slimeBodies = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			slimeBodies.addBox("Slime", -4F, 16F, -4F, 8, 8, 8);
			if (i > 0)
			{
				slimeBodies2 = new ModelLoader.ModelRenderer(this, 0, i, ModelPart.Helmet);
				slimeBodies2.addBox("Slime", -3F, 17F, -3F, 6, 6, 6);
				slimeRightEye = new ModelLoader.ModelRenderer(this, 32, 0, ModelPart.RightLeg);
				slimeRightEye.addBox("Eye", -3.25F, 18F, -3.5F, 2, 2, 2);
				slimeLeftEye = new ModelLoader.ModelRenderer(this, 32, 4, ModelPart.LeftLeg);
				slimeLeftEye.addBox("Eye", 1.25F, 18F, -3.5F, 2, 2, 2);
				slimeMouth = new ModelLoader.ModelRenderer(this, 32, 8, ModelPart.Chest);
				slimeMouth.addBox("Mouth", 0.0F, 21F, -3.5F, 1, 1, 1);
			}
		}
	}

	public class ModelSquid : ModelLoader.ModelBase
	{
		private readonly ModelLoader.ModelRenderer squidBody;
		private readonly ModelLoader.ModelRenderer[] squidTentacles;

		public ModelSquid()
		{
			squidTentacles = new ModelLoader.ModelRenderer[8];
			int byte0 = -16;
			squidBody = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Chest);
			squidBody.addBox("Body", -6F, -8F, -6F, 12, 16, 12);
			squidBody.rotationPointY += 24 + byte0;
			for (int i = 0; i < squidTentacles.Length; i++)
			{
				squidTentacles[i] = new ModelLoader.ModelRenderer(this, 48, 0, ModelPart.LeftArm);
				double d = (i * 3.1415926535897931D * 2D) / squidTentacles.Length;
				float f = (float) Math.Cos(d) * 5F;
				float f1 = (float) Math.Sin(d) * 5F;
				squidTentacles[i].addBox("Tentacle", -1F, 0.0F, -1F, 2, 18, 2);
				squidTentacles[i].rotationPointX = f;
				squidTentacles[i].rotationPointZ = f1;
				squidTentacles[i].rotationPointY = 31 + byte0;
				d = (i * 3.1415926535897931D * -2D) / squidTentacles.Length + 1.5707963267948966D;
				squidTentacles[i].rotateAngleY = (float) d;
			}
		}
	}

	public class ModelMagmaCube : ModelLoader.ModelBase
	{
		private readonly ModelLoader.ModelRenderer field_40344_b;
		private readonly ModelLoader.ModelRenderer[] field_40345_a;

		public ModelMagmaCube()
		{
			field_40345_a = new ModelLoader.ModelRenderer[8];
			for (int i = 0; i < field_40345_a.Length; i++)
			{
				byte byte0 = 0;
				int j = i;
				if (i == 2)
				{
					byte0 = 24;
					j = 10;
				}
				else if (i == 3)
				{
					byte0 = 24;
					j = 19;
				}
				field_40345_a[i] = new ModelLoader.ModelRenderer(this, byte0, j, ModelPart.Head);
				field_40345_a[i].addBox("Head", -4F, 16 + i, -4F, 8, 1, 8);
			}

			field_40344_b = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.Chest);
			field_40344_b.addBox("Core", -2F, 18F, -2F, 4, 4, 4);
		}
	}

	public class ModelBlaze : ModelLoader.ModelBase
	{
		private readonly ModelLoader.ModelRenderer field_40322_b;
		private readonly ModelLoader.ModelRenderer[] field_40323_a;

		public ModelBlaze()
		{
			field_40323_a = new ModelLoader.ModelRenderer[12];
			for (int i = 0; i < field_40323_a.Length; i++)
			{
				field_40323_a[i] = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftArm);
				field_40323_a[i].addBox("Rod", 0.0F, 0.0F, 0.0F, 2, 8, 2);
			}

			field_40322_b = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			field_40322_b.addBox("Head", -4F, -4F, -4F, 8, 8, 8);

			setRotationAngles(0, 0, 0, 0, 0, 0);
		}

		public void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
		{
			float f6 = f2 * 3.141593F * -0.1F;
			for (int i = 0; i < 4; i++)
			{
				field_40323_a[i].rotationPointY = -2F + (float) Math.Cos(((i * 2) + f2) * 0.25F);
				field_40323_a[i].rotationPointX = (float) Math.Cos(f6) * 9F;
				field_40323_a[i].rotationPointZ = (float) Math.Sin(f6) * 9F;
				f6 += 1.570796F;
			}

			f6 = 0.7853982F + f2 * 3.141593F * 0.03F;
			for (int j = 4; j < 8; j++)
			{
				field_40323_a[j].rotationPointY = 2.0F + (float) Math.Cos(((j * 2) + f2) * 0.25F);
				field_40323_a[j].rotationPointX = (float) Math.Cos(f6) * 7F;
				field_40323_a[j].rotationPointZ = (float) Math.Sin(f6) * 7F;
				f6 += 1.570796F;
			}

			f6 = 0.4712389F + f2 * 3.141593F * -0.05F;
			for (int k = 8; k < 12; k++)
			{
				field_40323_a[k].rotationPointY = 11F + (float) Math.Cos((k * 1.5F + f2) * 0.5F);
				field_40323_a[k].rotationPointX = (float) Math.Cos(f6) * 5F;
				field_40323_a[k].rotationPointZ = (float) Math.Sin(f6) * 5F;
				f6 += 1.570796F;
			}

			field_40322_b.rotateAngleY = f3 / 57.29578F;
			field_40322_b.rotateAngleX = f4 / 57.29578F;
		}
	}

	public class ModelSilverfish : ModelLoader.ModelBase
	{
		private static readonly int[,] silverfishBoxLength = {
		                                                     	{
		                                                     		3, 2, 2
		                                                     	}, {
		                                                     	   	4, 3, 2
		                                                     	   }, {
		                                                     	      	6, 4, 3
		                                                     	      }, {
		                                                     	         	3, 3, 3
		                                                     	         }, {
		                                                     	            	2, 2, 3
		                                                     	            }, {
		                                                     	               	2, 1, 2
		                                                     	               }, {
		                                                     	                  	1, 1, 2
		                                                     	                  }
		                                                     };

		private static readonly int[,] silverfishTexturePositions = {
		                                                            	{
		                                                            		0, 0
		                                                            	}, {
		                                                            	   	0, 4
		                                                            	   }, {
		                                                            	      	0, 9
		                                                            	      }, {
		                                                            	         	0, 16
		                                                            	         }, {
		                                                            	            	0, 22
		                                                            	            }, {
		                                                            	               	11, 0
		                                                            	               }, {
		                                                            	                  	13, 4
		                                                            	                  }
		                                                            };

		private readonly float[] field_35399_c;
		private readonly ModelLoader.ModelRenderer[] silverfishBodyParts;
		private readonly ModelLoader.ModelRenderer[] silverfishWings;

		public ModelSilverfish()
		{
			field_35399_c = new float[7];
			silverfishBodyParts = new ModelLoader.ModelRenderer[7];
			float f = -3.5F;
			for (int i = 0; i < silverfishBodyParts.Length; i++)
			{
				silverfishBodyParts[i] = new ModelLoader.ModelRenderer(this, silverfishTexturePositions[i, 0],
				                                                       silverfishTexturePositions[i, 1], ModelPart.Chest);
				silverfishBodyParts[i].addBox("Body", silverfishBoxLength[i, 0] * -0.5F, 0.0F, silverfishBoxLength[i, 2] * -0.5F,
				                              silverfishBoxLength[i, 0], silverfishBoxLength[i, 1], silverfishBoxLength[i, 2]);
				silverfishBodyParts[i].setRotationPoint(0.0F, 24 - silverfishBoxLength[i, 1], f);
				field_35399_c[i] = f;
				if (i < silverfishBodyParts.Length - 1)
					f += (silverfishBoxLength[i, 2] + silverfishBoxLength[i + 1, 2]) * 0.5F;
			}

			silverfishWings = new ModelLoader.ModelRenderer[3];
			silverfishWings[0] = new ModelLoader.ModelRenderer(this, 20, 0, ModelPart.Head);
			silverfishWings[0].addBox("Wings", -5F, 0.0F, silverfishBoxLength[2, 2] * -0.5F, 10, 8, silverfishBoxLength[2, 2]);
			silverfishWings[0].setRotationPoint(0.0F, 16F, field_35399_c[2]);
			silverfishWings[1] = new ModelLoader.ModelRenderer(this, 20, 11, ModelPart.Head);
			silverfishWings[1].addBox("Wings", -3F, 0.0F, silverfishBoxLength[4, 2] * -0.5F, 6, 4, silverfishBoxLength[4, 2]);
			silverfishWings[1].setRotationPoint(0.0F, 20F, field_35399_c[4]);
			silverfishWings[2] = new ModelLoader.ModelRenderer(this, 20, 18, ModelPart.Head);
			silverfishWings[2].addBox("Wings", -3F, 0.0F, silverfishBoxLength[4, 2] * -0.5F, 6, 5, silverfishBoxLength[1, 2]);
			silverfishWings[2].setRotationPoint(0.0F, 19F, field_35399_c[1]);
		}
	}

	public class ModelEnderman : ModelBiped
	{
		public bool isAttacking;
		public bool isCarrying;

		public ModelEnderman() :
			base(0.0F, -14F)
		{
			isCarrying = false;
			isAttacking = false;
			float f = -14F;
			float f1 = 0.0F;
			boxList.Remove(bipedHeadwear);
			bipedHeadwear = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.Helmet);
			bipedHeadwear.addBox("Jaw", -4F, -8F, -4F, 8, 8, 8, f1 - 0.5F);
			bipedHeadwear.setRotationPoint(0.0F, 0.0F + f, 0.0F);
			boxList.Remove(bipedBody);
			bipedBody = new ModelLoader.ModelRenderer(this, 32, 16, ModelPart.Chest);
			bipedBody.addBox("Body", -4F, 0.0F, -2F, 8, 12, 4, f1);
			bipedBody.setRotationPoint(0.0F, 0.0F + f, 0.0F);
			boxList.Remove(bipedRightArm);
			bipedRightArm = new ModelLoader.ModelRenderer(this, 56, 0, ModelPart.RightArm);
			bipedRightArm.addBox("Right Arm", -1F, -2F, -1F, 2, 30, 2, f1);
			bipedRightArm.setRotationPoint(-5F, 2.0F + f, 0.0F);
			boxList.Remove(bipedLeftArm);
			bipedLeftArm = new ModelLoader.ModelRenderer(this, 56, 0, ModelPart.LeftArm);
			bipedLeftArm.mirror = true;
			bipedLeftArm.addBox("Left Arm", -1F, -2F, -1F, 2, 30, 2, f1);
			bipedLeftArm.setRotationPoint(5F, 2.0F + f, 0.0F);
			boxList.Remove(bipedRightLeg);
			bipedRightLeg = new ModelLoader.ModelRenderer(this, 56, 0, ModelPart.RightLeg);
			bipedRightLeg.addBox("Right Leg", -1F, 0.0F, -1F, 2, 30, 2, f1);
			bipedRightLeg.setRotationPoint(-2F, 12F + f, 0.0F);
			boxList.Remove(bipedLeftLeg);
			bipedLeftLeg = new ModelLoader.ModelRenderer(this, 56, 0, ModelPart.LeftLeg);
			bipedLeftLeg.mirror = true;
			bipedLeftLeg.addBox("Left Leg", -1F, 0.0F, -1F, 2, 30, 2, f1);
			bipedLeftLeg.setRotationPoint(2.0F, 12F + f, 0.0F);
		}
	}

	public class ModelWolf : ModelLoader.ModelBase
	{
		private readonly ModelLoader.ModelRenderer wolfMane;
		private readonly ModelLoader.ModelRenderer wolfTail;
		public ModelLoader.ModelRenderer wolfBody;
		public ModelLoader.ModelRenderer wolfHeadMain;
		public ModelLoader.ModelRenderer wolfLeg1;
		public ModelLoader.ModelRenderer wolfLeg2;
		public ModelLoader.ModelRenderer wolfLeg3;
		public ModelLoader.ModelRenderer wolfLeg4;

		public ModelWolf()
		{
			float f = 0.0F;
			float f1 = 13.5F;
			wolfHeadMain = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			wolfHeadMain.addBox("Head", -3F, -3F, -2F, 6, 6, 4, f);
			wolfHeadMain.setRotationPoint(-1F, f1, -7F);
			wolfBody = new ModelLoader.ModelRenderer(this, 18, 14, ModelPart.Chest);
			wolfBody.addBox("Body", -4F, -2F, -3F, 6, 9, 6, f);
			wolfBody.setRotationPoint(0.0F, 14F, 2.0F);
			wolfMane = new ModelLoader.ModelRenderer(this, 21, 0, ModelPart.Helmet);
			wolfMane.addBox("Mane", -4F, -3F, -3F, 8, 6, 7, f);
			wolfMane.setRotationPoint(-1F, 14F, 2.0F);
			wolfLeg1 = new ModelLoader.ModelRenderer(this, 0, 18, ModelPart.LeftLeg);
			wolfLeg1.addBox("Left Leg", -1F, 0.0F, -1F, 2, 8, 2, f);
			wolfLeg1.setRotationPoint(-2.5F, 16F, 7F);
			wolfLeg2 = new ModelLoader.ModelRenderer(this, 0, 18, ModelPart.RightLeg);
			wolfLeg2.addBox("Right Leg", -1F, 0.0F, -1F, 2, 8, 2, f);
			wolfLeg2.setRotationPoint(0.5F, 16F, 7F);
			wolfLeg3 = new ModelLoader.ModelRenderer(this, 0, 18, ModelPart.LeftArm);
			wolfLeg3.addBox("Left Arm", -1F, 0.0F, -1F, 2, 8, 2, f);
			wolfLeg3.setRotationPoint(-2.5F, 16F, -4F);
			wolfLeg4 = new ModelLoader.ModelRenderer(this, 0, 18, ModelPart.RightArm);
			wolfLeg4.addBox("Right Arm", -1F, 0.0F, -1F, 2, 8, 2, f);
			wolfLeg4.setRotationPoint(0.5F, 16F, -4F);
			wolfTail = new ModelLoader.ModelRenderer(this, 9, 18, ModelPart.Chest);
			wolfTail.addBox("Chest", -1F, 0.0F, -1F, 2, 8, 2, f);
			wolfTail.setRotationPoint(-1F, 12F, 8F);
			wolfHeadMain.setTextureOffset(16, 14).addBox("Ear", -3F, -5F, 0.0F, 2, 2, 1, f);
			wolfHeadMain.setTextureOffset(16, 14).addBox("Ear", 1.0F, -5F, 0.0F, 2, 2, 1, f);
			wolfHeadMain.setTextureOffset(0, 10).addBox("Nose", -1.5F, 0.0F, -5F, 3, 3, 4, f);

			setLivingAnimations(0, 0, 0);
		}

		public void setLivingAnimations(float f, float f1, float f2)
		{
			wolfBody.setRotationPoint(0.0F, 14F, 2.0F);
			wolfBody.rotateAngleX = 1.570796F;
			wolfMane.setRotationPoint(-1F, 14F, -3F);
			wolfMane.rotateAngleX = wolfBody.rotateAngleX;
			wolfTail.setRotationPoint(-1F, 12F, 8F);
			wolfTail.rotateAngleX = wolfBody.rotateAngleX;
			wolfLeg1.setRotationPoint(-2.5F, 16F, 7F);
			wolfLeg2.setRotationPoint(0.5F, 16F, 7F);
			wolfLeg3.setRotationPoint(-2.5F, 16F, -4F);
			wolfLeg4.setRotationPoint(0.5F, 16F, -4F);
			wolfLeg1.rotateAngleX = (float) Math.Cos(f * 0.6662F) * 1.4F * f1;
			wolfLeg2.rotateAngleX = (float) Math.Cos(f * 0.6662F + 3.141593F) * 1.4F * f1;
			wolfLeg3.rotateAngleX = (float) Math.Cos(f * 0.6662F + 3.141593F) * 1.4F * f1;
			wolfLeg4.rotateAngleX = (float) Math.Cos(f * 0.6662F) * 1.4F * f1;
		}
	}

	public class ModelGhast : ModelLoader.ModelBase
	{
		private readonly ModelLoader.ModelRenderer body;
		private readonly ModelLoader.ModelRenderer[] tentacles;

		public ModelGhast()
		{
			tentacles = new ModelLoader.ModelRenderer[9];
			int byte0 = -16;
			body = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Chest);
			body.addBox("Body", -8F, -8F, -8F, 16, 16, 16);
			body.rotationPointY += 24 + byte0;
			var random = new Random(1660);
			for (int i = 0; i < tentacles.Length; i++)
			{
				tentacles[i] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.LeftLeg);
				float f = (((((i % 3) - ((i / 3) % 2) * 0.5F) + 0.25F) / 2.0F) * 2.0F - 1.0F) * 5F;
				float f1 = (((i / 3) / 2.0F) * 2.0F - 1.0F) * 5F;
				int j = random.Next(7) + 8;
				tentacles[i].addBox("Tentacle", -1F, 0.0F, -1F, 2, j, 2);
				tentacles[i].rotationPointX = f;
				tentacles[i].rotationPointZ = f1;
				tentacles[i].rotationPointY = 31 + byte0;
			}

			setRotationAngles(0, 0, 123456, 0, 0, 0);
		}

		public void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
		{
			for (int i = 0; i < tentacles.Length; i++)
				tentacles[i].rotateAngleX = 0.2F * (float) Math.Sin(f2 * 0.3F + i) + 0.4F;
		}
	}

	public class ModelSpider : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer spiderBody;
		public ModelLoader.ModelRenderer spiderHead;
		public ModelLoader.ModelRenderer spiderLeg1;
		public ModelLoader.ModelRenderer spiderLeg2;
		public ModelLoader.ModelRenderer spiderLeg3;
		public ModelLoader.ModelRenderer spiderLeg4;
		public ModelLoader.ModelRenderer spiderLeg5;
		public ModelLoader.ModelRenderer spiderLeg6;
		public ModelLoader.ModelRenderer spiderLeg7;
		public ModelLoader.ModelRenderer spiderLeg8;
		public ModelLoader.ModelRenderer spiderNeck;

		public ModelSpider()
		{
			float f = 0.0F;
			int i = 15;
			spiderHead = new ModelLoader.ModelRenderer(this, 32, 4, ModelPart.Head);
			spiderHead.addBox("Head", -4F, -4F, -8F, 8, 8, 8, f);
			spiderHead.setRotationPoint(0.0F, 0 + i, -3F);
			spiderNeck = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Helmet);
			spiderNeck.addBox("Neck", -3F, -3F, -3F, 6, 6, 6, f);
			spiderNeck.setRotationPoint(0.0F, i, 0.0F);
			spiderBody = new ModelLoader.ModelRenderer(this, 0, 12, ModelPart.Chest);
			spiderBody.addBox("Body", -5F, -4F, -6F, 10, 8, 12, f);
			spiderBody.setRotationPoint(0.0F, 0 + i, 9F);
			spiderLeg1 = new ModelLoader.ModelRenderer(this, 18, 0, ModelPart.LeftArm);
			spiderLeg1.addBox("Leg", -15F, -1F, -1F, 16, 2, 2, f);
			spiderLeg1.setRotationPoint(-4F, 0 + i, 2.0F);
			spiderLeg2 = new ModelLoader.ModelRenderer(this, 18, 0, ModelPart.LeftArm);
			spiderLeg2.addBox("Leg", -1F, -1F, -1F, 16, 2, 2, f);
			spiderLeg2.setRotationPoint(4F, 0 + i, 2.0F);
			spiderLeg3 = new ModelLoader.ModelRenderer(this, 18, 0, ModelPart.LeftArm);
			spiderLeg3.addBox("Leg", -15F, -1F, -1F, 16, 2, 2, f);
			spiderLeg3.setRotationPoint(-4F, 0 + i, 1.0F);
			spiderLeg4 = new ModelLoader.ModelRenderer(this, 18, 0, ModelPart.LeftArm);
			spiderLeg4.addBox("Leg", -1F, -1F, -1F, 16, 2, 2, f);
			spiderLeg4.setRotationPoint(4F, 0 + i, 1.0F);
			spiderLeg5 = new ModelLoader.ModelRenderer(this, 18, 0, ModelPart.LeftArm);
			spiderLeg5.addBox("Leg", -15F, -1F, -1F, 16, 2, 2, f);
			spiderLeg5.setRotationPoint(-4F, 0 + i, 0.0F);
			spiderLeg6 = new ModelLoader.ModelRenderer(this, 18, 0, ModelPart.LeftArm);
			spiderLeg6.addBox("Leg", -1F, -1F, -1F, 16, 2, 2, f);
			spiderLeg6.setRotationPoint(4F, 0 + i, 0.0F);
			spiderLeg7 = new ModelLoader.ModelRenderer(this, 18, 0, ModelPart.LeftArm);
			spiderLeg7.addBox("Leg", -15F, -1F, -1F, 16, 2, 2, f);
			spiderLeg7.setRotationPoint(-4F, 0 + i, -1F);
			spiderLeg8 = new ModelLoader.ModelRenderer(this, 18, 0, ModelPart.LeftArm);
			spiderLeg8.addBox("Leg", -1F, -1F, -1F, 16, 2, 2, f);
			spiderLeg8.setRotationPoint(4F, 0 + i, -1F);

			setRotationAngles(0, 0, 0, 0, 0, 0);
		}

		public void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
		{
			spiderHead.rotateAngleY = f3 / 57.29578F;
			spiderHead.rotateAngleX = f4 / 57.29578F;
			float f6 = 0.7853982F;
			spiderLeg1.rotateAngleZ = -f6;
			spiderLeg2.rotateAngleZ = f6;
			spiderLeg3.rotateAngleZ = -f6 * 0.74F;
			spiderLeg4.rotateAngleZ = f6 * 0.74F;
			spiderLeg5.rotateAngleZ = -f6 * 0.74F;
			spiderLeg6.rotateAngleZ = f6 * 0.74F;
			spiderLeg7.rotateAngleZ = -f6;
			spiderLeg8.rotateAngleZ = f6;
			float f7 = -0F;
			float f8 = 0.3926991F;
			spiderLeg1.rotateAngleY = f8 * 2.0F + f7;
			spiderLeg2.rotateAngleY = -f8 * 2.0F - f7;
			spiderLeg3.rotateAngleY = f8 * 1.0F + f7;
			spiderLeg4.rotateAngleY = -f8 * 1.0F - f7;
			spiderLeg5.rotateAngleY = -f8 * 1.0F + f7;
			spiderLeg6.rotateAngleY = f8 * 1.0F - f7;
			spiderLeg7.rotateAngleY = -f8 * 2.0F + f7;
			spiderLeg8.rotateAngleY = f8 * 2.0F - f7;
			float f9 = (float) -(Math.Cos(f * 0.6662F * 2.0F + 0.0F) * 0.4F) * f1;
			float f10 = (float) (-Math.Cos(f * 0.6662F * 2.0F + 3.141593F) * 0.4F) * f1;
			float f11 = (float) (-Math.Cos(f * 0.6662F * 2.0F + 1.570796F) * 0.4F) * f1;
			float f12 = (float) (-Math.Cos(f * 0.6662F * 2.0F + 4.712389F) * 0.4F) * f1;
			float f13 = (float) Math.Abs(Math.Sin(f * 0.6662F + 0.0F) * 0.4F) * f1;
			float f14 = (float) Math.Abs(Math.Sin(f * 0.6662F + 3.141593F) * 0.4F) * f1;
			float f15 = (float) Math.Abs(Math.Sin(f * 0.6662F + 1.570796F) * 0.4F) * f1;
			float f16 = (float) Math.Abs(Math.Sin(f * 0.6662F + 4.712389F) * 0.4F) * f1;
			spiderLeg1.rotateAngleY += f9;
			spiderLeg2.rotateAngleY += -f9;
			spiderLeg3.rotateAngleY += f10;
			spiderLeg4.rotateAngleY += -f10;
			spiderLeg5.rotateAngleY += f11;
			spiderLeg6.rotateAngleY += -f11;
			spiderLeg7.rotateAngleY += f12;
			spiderLeg8.rotateAngleY += -f12;
			spiderLeg1.rotateAngleZ += f13;
			spiderLeg2.rotateAngleZ += -f13;
			spiderLeg3.rotateAngleZ += f14;
			spiderLeg4.rotateAngleZ += -f14;
			spiderLeg5.rotateAngleZ += f15;
			spiderLeg6.rotateAngleZ += -f15;
			spiderLeg7.rotateAngleZ += f16;
			spiderLeg8.rotateAngleZ += -f16;
		}
	}

	public class ModelSheep1 : ModelQuadruped
	{
		public ModelSheep1() :
			base(12, 0.0F)
		{
			boxList.Remove(head);
			head = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			head.addBox("Head", -3F, -4F, -4F, 6, 6, 6, 0.6F);
			head.setRotationPoint(0.0F, 6F, -8F);
			boxList.Remove(body);
			body = new ModelLoader.ModelRenderer(this, 28, 8, ModelPart.Chest);
			body.addBox("Body", -4F, -10F, -7F, 8, 16, 6, 1.75F);
			body.setRotationPoint(0.0F, 5F, 2.0F);
			body.rotateAngleX = 1.570796F;
			float f = 0.5F;
			boxList.Remove(leg1);
			leg1 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
			leg1.addBox("Left Leg", -2F, 0.0F, -2F, 4, 6, 4, f);
			leg1.setRotationPoint(-3F, 12F, 7F);
			boxList.Remove(leg2);
			leg2 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightLeg);
			leg2.addBox("Right Leg", -2F, 0.0F, -2F, 4, 6, 4, f);
			leg2.setRotationPoint(3F, 12F, 7F);
			boxList.Remove(leg3);
			leg3 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftArm);
			leg3.addBox("Left Arm", -2F, 0.0F, -2F, 4, 6, 4, f);
			leg3.setRotationPoint(-3F, 12F, -5F);
			boxList.Remove(leg4);
			leg4 = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightArm);
			leg4.addBox("Right Arm", -2F, 0.0F, -2F, 4, 6, 4, f);
			leg4.setRotationPoint(3F, 12F, -5F);
		}
	}

	public class ModelSheep2 : ModelQuadruped
	{
		public ModelSheep2() :
			base(12, 0.0F)
		{
			boxList.Remove(head);
			head = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			head.addBox("Head", -3F, -4F, -6F, 6, 6, 8, 0.0F);
			head.setRotationPoint(0.0F, 6F, -8F);
			boxList.Remove(body);
			body = new ModelLoader.ModelRenderer(this, 28, 8, ModelPart.Chest);
			body.addBox("Body", -4F, -10F, -7F, 8, 16, 6, 0.0F);
			body.setRotationPoint(0.0F, 5F, 2.0F);
			body.rotateAngleX = 1.570796F;
		}
	}

	public class ModelChest : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer chestBelow;
		public ModelLoader.ModelRenderer chestKnob;
		public ModelLoader.ModelRenderer chestLid;

		public ModelChest()
		{
			chestLid = (new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head)).setTextureSize(64, 64);
			chestLid.addBox("Lid", 0.0F, -5F, -14F, 14, 5, 14, 0.0F);
			chestLid.rotationPointX = 1.0F;
			chestLid.rotationPointY = 7F;
			chestLid.rotationPointZ = 15F;
			chestKnob = (new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Helmet)).setTextureSize(64, 64);
			chestKnob.addBox("Knob", -1F, -2F, -15F, 2, 4, 1, 0.0F);
			chestKnob.rotationPointX = 8F;
			chestKnob.rotationPointY = 7F;
			chestKnob.rotationPointZ = 15F;
			chestBelow = (new ModelLoader.ModelRenderer(this, 0, 19, ModelPart.Chest)).setTextureSize(64, 64);
			chestBelow.addBox("Base", 0.0F, 0.0F, 0.0F, 14, 10, 14, 0.0F);
			chestBelow.rotationPointX = 1.0F;
			chestBelow.rotationPointY = 6F;
			chestBelow.rotationPointZ = 1.0F;
		}
	}

	public class ModelLargeChest : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer chestBelow;
		public ModelLoader.ModelRenderer chestKnob;
		public ModelLoader.ModelRenderer chestLid;

		public ModelLargeChest()
		{
			chestLid = (new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head)).setTextureSize(128, 64);
			chestLid.addBox("Lid", 0.0F, -5F, -14F, 30, 5, 14, 0.0F);
			chestLid.rotationPointX = 1.0F;
			chestLid.rotationPointY = 7F;
			chestLid.rotationPointZ = 15F;
			chestKnob = (new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Helmet)).setTextureSize(128, 64);
			chestKnob.addBox("Knob", -1F, -2F, -15F, 2, 4, 1, 0.0F);
			chestKnob.rotationPointX = 16F;
			chestKnob.rotationPointY = 7F;
			chestKnob.rotationPointZ = 15F;
			chestBelow = (new ModelLoader.ModelRenderer(this, 0, 19, ModelPart.Chest)).setTextureSize(128, 64);
			chestBelow.addBox("Base", 0.0F, 0.0F, 0.0F, 30, 10, 14, 0.0F);
			chestBelow.rotationPointX = 1.0F;
			chestBelow.rotationPointY = 6F;
			chestBelow.rotationPointZ = 1.0F;
		}
	}

	public class ModelBoat : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer[] boatSides;

		public ModelBoat()
		{
			boatSides = new ModelLoader.ModelRenderer[5];
			boatSides[0] = new ModelLoader.ModelRenderer(this, 0, 8, ModelPart.Head);
			boatSides[1] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Helmet);
			boatSides[2] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Chest);
			boatSides[3] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.LeftArm);
			boatSides[4] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.RightArm);
			byte byte0 = 24;
			byte byte1 = 6;
			byte byte2 = 20;
			byte byte3 = 4;
			boatSides[0].addBox("Base", -byte0 / 2, -byte2 / 2 + 2, -3F, byte0, byte2 - 4, 4, 0.0F);
			boatSides[0].setRotationPoint(0.0F, 0 + byte3, 0.0F);
			boatSides[1].addBox("Side", -byte0 / 2 + 2, -byte1 - 1, -1F, byte0 - 4, byte1, 2, 0.0F);
			boatSides[1].setRotationPoint(-byte0 / 2 + 1, 0 + byte3, 0.0F);
			boatSides[2].addBox("Side", -byte0 / 2 + 2, -byte1 - 1, -1F, byte0 - 4, byte1, 2, 0.0F);
			boatSides[2].setRotationPoint(byte0 / 2 - 1, 0 + byte3, 0.0F);
			boatSides[3].addBox("Side", -byte0 / 2 + 2, -byte1 - 1, -1F, byte0 - 4, byte1, 2, 0.0F);
			boatSides[3].setRotationPoint(0.0F, 0 + byte3, -byte2 / 2 + 1);
			boatSides[4].addBox("Side", -byte0 / 2 + 2, -byte1 - 1, -1F, byte0 - 4, byte1, 2, 0.0F);
			boatSides[4].setRotationPoint(0.0F, 0 + byte3, byte2 / 2 - 1);
			boatSides[0].rotateAngleX = 1.570796F;
			boatSides[1].rotateAngleY = 4.712389F;
			boatSides[2].rotateAngleY = 1.570796F;
			boatSides[3].rotateAngleY = 3.141593F;
		}
	}

	public class SignModel : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer signBoard;
		public ModelLoader.ModelRenderer signStick;

		public SignModel()
		{
			signBoard = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Helmet);
			signBoard.addBox("Board", -12F, -14F, -1F, 24, 12, 2, 0.0F);
			signStick = new ModelLoader.ModelRenderer(this, 0, 14, ModelPart.Chest);
			signStick.addBox("Stick", -1F, -2F, -1F, 2, 14, 2, 0.0F);
		}
	}

	public class ModelBook : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer field_40324_f;
		public ModelLoader.ModelRenderer field_40325_g;
		public ModelLoader.ModelRenderer field_40326_d;
		public ModelLoader.ModelRenderer field_40327_e;
		public ModelLoader.ModelRenderer field_40328_b;
		public ModelLoader.ModelRenderer field_40329_c;
		public ModelLoader.ModelRenderer field_40330_a;

		public ModelBook()
		{
			field_40330_a = (new ModelLoader.ModelRenderer(this, ModelPart.Head)).setTextureOffset(0, 0).addBox("Left Cover", -6F,
			                                                                                                    -5F, 0.0F, 6, 10,
			                                                                                                    0);
			field_40328_b = (new ModelLoader.ModelRenderer(this, ModelPart.Chest)).setTextureOffset(16, 0).addBox("Right Cover",
			                                                                                                      0.0F, -5F, 0.0F,
			                                                                                                      6, 10, 0);
			field_40325_g = (new ModelLoader.ModelRenderer(this, ModelPart.Helmet)).setTextureOffset(12, 0).addBox("Spine", -1F,
			                                                                                                       -5F, 0.0F, 2,
			                                                                                                       10, 0);
			field_40329_c = (new ModelLoader.ModelRenderer(this, ModelPart.LeftArm)).setTextureOffset(0, 10).addBox("Page", 0.0F,
			                                                                                                        -4F, -0.99F,
			                                                                                                        5, 8, 1);
			field_40326_d = (new ModelLoader.ModelRenderer(this, ModelPart.LeftLeg)).setTextureOffset(12, 10).addBox("Page", 0.0F,
			                                                                                                         -4F, -0.01F,
			                                                                                                         5, 8, 1);
			field_40327_e = (new ModelLoader.ModelRenderer(this, ModelPart.RightArm)).setTextureOffset(24, 10).addBox("Page",
			                                                                                                          0.0F, -4F,
			                                                                                                          0.0F, 5, 8,
			                                                                                                          0);
			field_40324_f = (new ModelLoader.ModelRenderer(this, ModelPart.RightLeg)).setTextureOffset(24, 10).addBox("Page",
			                                                                                                          0.0F, -4F,
			                                                                                                          0.0F, 5, 8,
			                                                                                                          0);
			field_40330_a.setRotationPoint(0.0F, 0.0F, -1F);
			field_40328_b.setRotationPoint(0.0F, 0.0F, 1.0F);
			field_40325_g.rotateAngleY = 1.570796F;
		}

		public void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
		{
			float f6 = (float) (Math.Sin(f * 0.02F) * 0.1F + 1.25F) * f3;
			field_40330_a.rotateAngleY = 3.141593F + f6;
			field_40328_b.rotateAngleY = -f6;
			field_40329_c.rotateAngleY = f6;
			field_40326_d.rotateAngleY = -f6;
			field_40327_e.rotateAngleY = f6 - f6 * 2.0F * f1;
			field_40324_f.rotateAngleY = f6 - f6 * 2.0F * f2;
			field_40329_c.rotationPointX = (float) Math.Sin(f6);
			field_40326_d.rotationPointX = (float) Math.Sin(f6);
			field_40327_e.rotationPointX = (float) Math.Sin(f6);
			field_40324_f.rotationPointX = (float) Math.Sin(f6);
		}
	}

	public class ModelMinecart : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer[] sideModels;

		public ModelMinecart()
		{
			sideModels = new ModelLoader.ModelRenderer[7];
			sideModels[0] = new ModelLoader.ModelRenderer(this, 0, 10, ModelPart.Head);
			sideModels[1] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.LeftArm);
			sideModels[2] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.LeftLeg);
			sideModels[3] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.RightArm);
			sideModels[4] = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.RightLeg);
			sideModels[5] = new ModelLoader.ModelRenderer(this, 44, 10, ModelPart.Chest);
			byte byte0 = 20;
			byte byte1 = 8;
			byte byte2 = 16;
			byte byte3 = 4;
			sideModels[0].addBox("Base", -byte0 / 2, -byte2 / 2, -1F, byte0, byte2, 2, 0.0F);
			sideModels[0].setRotationPoint(0.0F, 0 + byte3, 0.0F);
			sideModels[5].addBox("Leather", -byte0 / 2 + 1, -byte2 / 2 + 1, -1F, byte0 - 2, byte2 - 2, 1, 0.0F);
			sideModels[5].setRotationPoint(0.0F, 0 + byte3, 0.0F);
			sideModels[1].addBox("Side", -byte0 / 2 + 2, -byte1 - 1, -1F, byte0 - 4, byte1, 2, 0.0F);
			sideModels[1].setRotationPoint(-byte0 / 2 + 1, 0 + byte3, 0.0F);
			sideModels[2].addBox("Side", -byte0 / 2 + 2, -byte1 - 1, -1F, byte0 - 4, byte1, 2, 0.0F);
			sideModels[2].setRotationPoint(byte0 / 2 - 1, 0 + byte3, 0.0F);
			sideModels[3].addBox("Side", -byte0 / 2 + 2, -byte1 - 1, -1F, byte0 - 4, byte1, 2, 0.0F);
			sideModels[3].setRotationPoint(0.0F, 0 + byte3, -byte2 / 2 + 1);
			sideModels[4].addBox("Side", -byte0 / 2 + 2, -byte1 - 1, -1F, byte0 - 4, byte1, 2, 0.0F);
			sideModels[4].setRotationPoint(0.0F, 0 + byte3, byte2 / 2 - 1);
			sideModels[0].rotateAngleX = 1.570796F;
			sideModels[1].rotateAngleY = 4.712389F;
			sideModels[2].rotateAngleY = 1.570796F;
			sideModels[3].rotateAngleY = 3.141593F;
			sideModels[5].rotateAngleX = -1.570796F;
		}
	}

	public class ModelEnderCrystal : ModelLoader.ModelBase
	{
		private readonly ModelLoader.ModelRenderer field_41057_g;
		private readonly ModelLoader.ModelRenderer field_41058_h;
		private readonly ModelLoader.ModelRenderer field_41059_i;

		public ModelEnderCrystal()
		{
			field_41058_h = new ModelLoader.ModelRenderer(this, "glass", ModelPart.Head);
			field_41058_h.setTextureOffset(0, 0).addBox("Glass", -4F, -4F, -4F, 8, 8, 8);
			field_41057_g = new ModelLoader.ModelRenderer(this, "cube", ModelPart.Helmet);
			field_41057_g.setTextureOffset(32, 0).addBox("Cube", -4F, -4F, -4F, 8, 8, 8);
			field_41059_i = new ModelLoader.ModelRenderer(this, "base", ModelPart.Chest);
			field_41059_i.setTextureOffset(0, 16).addBox("Base", -6F, 0.0F, -6F, 12, 4, 12);
		}
	}

	public class ModelSnowMan : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer field_40302_d;
		public ModelLoader.ModelRenderer field_40303_e;
		public ModelLoader.ModelRenderer field_40304_b;
		public ModelLoader.ModelRenderer field_40305_c;
		public ModelLoader.ModelRenderer field_40306_a;

		public ModelSnowMan()
		{
			float f = 4F;
			float f1 = 0.0F;
			field_40305_c = (new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head)).setTextureSize(64, 64);
			field_40305_c.addBox("Head", -4F, -8F, -4F, 8, 8, 8, f1 - 0.5F);
			field_40305_c.setRotationPoint(0.0F, 0.0F + f, 0.0F);
			field_40302_d = (new ModelLoader.ModelRenderer(this, 32, 0, ModelPart.Helmet)).setTextureSize(64, 64);
			field_40302_d.addBox("Top", -1F, 0.0F, -1F, 12, 2, 2, f1 - 0.5F);
			field_40302_d.setRotationPoint(0.0F, (0.0F + f + 9F) - 7F, 0.0F);
			field_40303_e = (new ModelLoader.ModelRenderer(this, 32, 0, ModelPart.Chest)).setTextureSize(64, 64);
			field_40303_e.addBox("Bottom", -1F, 0.0F, -1F, 12, 2, 2, f1 - 0.5F);
			field_40303_e.setRotationPoint(0.0F, (0.0F + f + 9F) - 7F, 0.0F);
			field_40306_a = (new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftArm)).setTextureSize(64, 64);
			field_40306_a.addBox("Left Arm", -5F, -10F, -5F, 10, 10, 10, f1 - 0.5F);
			field_40306_a.setRotationPoint(0.0F, 0.0F + f + 9F, 0.0F);
			field_40304_b = (new ModelLoader.ModelRenderer(this, 0, 36, ModelPart.RightArm)).setTextureSize(64, 64);
			field_40304_b.addBox("Right Arm", -6F, -12F, -6F, 12, 12, 12, f1 - 0.5F);
			field_40304_b.setRotationPoint(0.0F, 0.0F + f + 20F, 0.0F);

			setRotationAngles(0, 0, 0, (float) Math.PI / 2, 0, 0);
		}

		public void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
		{
			field_40305_c.rotateAngleY = f3 / 57.29578F;
			field_40305_c.rotateAngleX = f4 / 57.29578F;
			field_40306_a.rotateAngleY = (f3 / 57.29578F) * 0.25F;
			var f6 = (float) Math.Sin(field_40306_a.rotateAngleY);
			var f7 = (float) Math.Cos(field_40306_a.rotateAngleY);
			field_40302_d.rotateAngleZ = 1.0F;
			field_40303_e.rotateAngleZ = -1F;
			field_40302_d.rotateAngleY = 0.0F + field_40306_a.rotateAngleY;
			field_40303_e.rotateAngleY = 3.141593F + field_40306_a.rotateAngleY;
			field_40302_d.rotationPointX = f7 * 5F;
			field_40302_d.rotationPointZ = -f6 * 5F;
			field_40303_e.rotationPointX = -f7 * 5F;
			field_40303_e.rotationPointZ = f6 * 5F;
		}
	}

	public class ModelZombie : ModelBiped
	{
		public ModelZombie()
		{
			//boxList.Remove(bipedHeadwear);
			setRotationAngles(0, 0, 0, 0, 0, 0);
		}

		public void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
		{
			var f6 = (float) Math.Sin(onGround * 3.141593F);
			var f7 = (float) Math.Sin((1.0F - (1.0F - onGround) * (1.0F - onGround)) * 3.141593F);
			bipedRightArm.rotateAngleZ = 0.0F;
			bipedLeftArm.rotateAngleZ = 0.0F;
			bipedRightArm.rotateAngleY = -(0.1F - f6 * 0.6F);
			bipedLeftArm.rotateAngleY = 0.1F - f6 * 0.6F;
			bipedRightArm.rotateAngleX = -1.570796F;
			bipedLeftArm.rotateAngleX = -1.570796F;
			bipedRightArm.rotateAngleX -= f6 * 1.2F - f7 * 0.4F;
			bipedLeftArm.rotateAngleX -= f6 * 1.2F - f7 * 0.4F;
			bipedRightArm.rotateAngleZ += (float) Math.Cos(f2 * 0.09F) * 0.05F + 0.05F;
			bipedLeftArm.rotateAngleZ -= (float) Math.Cos(f2 * 0.09F) * 0.05F + 0.05F;
			bipedRightArm.rotateAngleX += (float) Math.Sin(f2 * 0.067F) * 0.05F;
			bipedLeftArm.rotateAngleX -= (float) Math.Sin(f2 * 0.067F) * 0.05F;
		}
	}

	public class ModelSkeleton : ModelZombie
	{
		public ModelSkeleton()
		{
			float f = 0.0F;
			boxList.Remove(bipedRightArm);
			bipedRightArm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.RightArm);
			bipedRightArm.addBox("Right Arm", -1F, -2F, -1F, 2, 12, 2, f);
			bipedRightArm.setRotationPoint(-5F, 2.0F, 0.0F);
			boxList.Remove(bipedLeftArm);
			bipedLeftArm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.LeftArm);
			bipedLeftArm.mirror = true;
			bipedLeftArm.addBox("Right Arm", -1F, -2F, -1F, 2, 12, 2, f);
			bipedLeftArm.setRotationPoint(5F, 2.0F, 0.0F);
			boxList.Remove(bipedRightLeg);
			bipedRightLeg = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightLeg);
			bipedRightLeg.addBox("Right Leg", -1F, 0.0F, -1F, 2, 12, 2, f);
			bipedRightLeg.setRotationPoint(-2F, 12F, 0.0F);
			boxList.Remove(bipedLeftLeg);
			bipedLeftLeg = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
			bipedLeftLeg.mirror = true;
			bipedLeftLeg.addBox("Left Leg", -1F, 0.0F, -1F, 2, 12, 2, f);
			bipedLeftLeg.setRotationPoint(2.0F, 12F, 0.0F);

			setRotationAngles(0, 0, 0, 0, 0, 0);
		}

		public new void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
		{
			aimedBow = true;
			base.setRotationAngles(f, f1, f2, f3, f4, f5);
		}
	}

	public class pm_Pony : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer Body;
		public ModelLoader.PlaneRenderer[] Bodypiece;
		public ModelLoader.ModelRenderer LeftArm;
		public ModelLoader.ModelRenderer LeftLeg;
		public ModelLoader.ModelRenderer[] LeftWing;
		public ModelLoader.ModelRenderer[] LeftWingExt;
		public ModelLoader.ModelRenderer RightLeg;
		public ModelLoader.ModelRenderer[] RightWing;
		public ModelLoader.ModelRenderer[] RightWingExt;
		public ModelLoader.ModelRenderer SteveArm;
		public ModelLoader.PlaneRenderer[] Tail;
		private float TailRotateAngleY;
		private float WingRotateAngleX;
		private float WingRotateAngleY;
		private float WingRotateAngleZ;
		public ModelLoader.ModelRenderer head;
		public ModelLoader.ModelRenderer[] headpiece;
		public ModelLoader.ModelRenderer helmet;

		public bool isFlying;
		public bool isPegasus;
		public bool isUnicorn;
		private bool rainboom;
		public ModelLoader.ModelRenderer rightarm;
		public float strech;
		public ModelLoader.ModelRenderer unicornarm;

		public pm_Pony init(bool unicorn, bool pegasis)
		{
			isUnicorn = unicorn;
			isPegasus = pegasis;
			init(0.0F);
			return this;
		}

		public void init(float yoffset)
		{
			init(yoffset, 0.0F);
		}

		public void init(float yoffset, float init_strech)
		{
			strech = init_strech;

			float headR1 = 0.0F;
			float headR2 = 0.0F;
			float headR3 = 0.0F;

			head = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			head.addBox("Head", -4.0F, -4.0F, -6.0F, 8, 8, 8, strech);
			head.setRotationPoint(headR1, headR2 + yoffset, headR3);

			headpiece = new ModelLoader.ModelRenderer[3];

			headpiece[0] = new ModelLoader.ModelRenderer(this, 12, 16, ModelPart.Helmet);
			headpiece[0].addBox("Ear", -4.0F, -6.0F, -1.0F, 2, 2, 2, strech);
			headpiece[0].setRotationPoint(headR1, headR2 + yoffset, headR3);

			headpiece[1] = new ModelLoader.ModelRenderer(this, 12, 16, ModelPart.Helmet);
			headpiece[1].addBox("Ear", 2.0F, -6.0F, -1.0F, 2, 2, 2, strech);
			headpiece[1].setRotationPoint(headR1, headR2 + yoffset, headR3);

			headpiece[2] = new ModelLoader.ModelRenderer(this, 56, 0, ModelPart.Helmet);
			headpiece[2].addBox("Horn", -0.5F, -10.0F, -4.0F, 1, 4, 1, strech);
			headpiece[2].setRotationPoint(headR1, headR2 + yoffset, headR3);

			helmet = new ModelLoader.ModelRenderer(this, 32, 0, ModelPart.Helmet);
			helmet.addBox("Hair", -4.0F, -4.0F, -6.0F, 8, 8, 8, strech + 0.5F);
			helmet.setRotationPoint(headR1, headR2 + yoffset, headR3);

			float BodyR1 = 0.0F;
			float BodyR2 = 0.0F;
			float BodyR3 = 0.0F;

			Body = new ModelLoader.ModelRenderer(this, 16, 16, ModelPart.Chest);
			Body.addBox("Body.Body", -4.0F, 4.0F, -2.0F, 8, 8, 4, strech);
			Body.setRotationPoint(BodyR1, BodyR2 + yoffset, BodyR3);

			Bodypiece = new ModelLoader.PlaneRenderer[13];

			Bodypiece[0] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[0].addSidePlane(-4.0F, 4.0F, 2.0F, 0, 8, 8, strech);
			Bodypiece[0].setRotationPoint(BodyR1, BodyR2 + yoffset, BodyR3);

			Bodypiece[1] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[1].addSidePlane(4.0F, 4.0F, 2.0F, 0, 8, 8, strech);
			Bodypiece[1].setRotationPoint(BodyR1, BodyR2 + yoffset, BodyR3);

			Bodypiece[2] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[2].addTopPlane(-4.0F, 4.0F, 2.0F, 8, 0, 8, strech);
			Bodypiece[2].setRotationPoint(headR1, headR2 + yoffset, headR3);

			Bodypiece[3] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[3].addTopPlane(-4.0F, 12.0F, 2.0F, 8, 0, 8, strech);
			Bodypiece[3].setRotationPoint(headR1, headR2 + yoffset, headR3);

			Bodypiece[4] = new ModelLoader.PlaneRenderer(this, "Body.Cutie Mark", 0, 20, ModelPart.Chest);
			Bodypiece[4].addSidePlane(-4.0F, 4.0F, 10.0F, 0, 8, 4, strech);
			Bodypiece[4].setRotationPoint(BodyR1, BodyR2 + yoffset, BodyR3);

			Bodypiece[5] = new ModelLoader.PlaneRenderer(this, "Body.Body", 0, 20, ModelPart.Chest);
			Bodypiece[5].addSidePlane(4.0F, 4.0F, 10.0F, 0, 8, 4, strech);
			Bodypiece[5].setRotationPoint(BodyR1, BodyR2 + yoffset, BodyR3);

			Bodypiece[6] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[6].addTopPlane(-4.0F, 4.0F, 10.0F, 8, 0, 4, strech);
			Bodypiece[6].setRotationPoint(headR1, headR2 + yoffset, headR3);

			Bodypiece[7] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[7].addTopPlane(-4.0F, 12.0F, 10.0F, 8, 0, 4, strech);
			Bodypiece[7].setRotationPoint(headR1, headR2 + yoffset, headR3);

			Bodypiece[8] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[8].addBackPlane(-4.0F, 4.0F, 14.0F, 8, 8, 0, strech);
			Bodypiece[8].setRotationPoint(headR1, headR2 + yoffset, headR3);

			Bodypiece[9] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[9].addTopPlane(-1.0F, 10.0F, 8.0F, 2, 0, 6, strech);
			Bodypiece[9].setRotationPoint(headR1, headR2 + yoffset, headR3);

			Bodypiece[10] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[10].addTopPlane(-1.0F, 12.0F, 8.0F, 2, 0, 6, strech);
			Bodypiece[10].setRotationPoint(headR1, headR2 + yoffset, headR3);

			Bodypiece[11] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[11].mirror = true;
			Bodypiece[11].addSidePlane(-1.0F, 10.0F, 8.0F, 0, 2, 6, strech);
			Bodypiece[11].setRotationPoint(headR1, headR2 + yoffset, headR3);

			Bodypiece[12] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[12].addSidePlane(1.0F, 10.0F, 8.0F, 0, 2, 6, strech);
			Bodypiece[12].setRotationPoint(headR1, headR2 + yoffset, headR3);

			rightarm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.RightArm);
			rightarm.addBox("Right Arm", -2.0F, 4.0F, -2.0F, 4, 12, 4, strech);
			rightarm.setRotationPoint(-3.0F, 8.0F + yoffset, 0.0F);

			LeftArm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.LeftArm);
			LeftArm.mirror = true;
			LeftArm.addBox("Left Arm", -2.0F, 4.0F, -2.0F, 4, 12, 4, strech);
			LeftArm.setRotationPoint(3.0F, 8.0F + yoffset, 0.0F);

			RightLeg = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.RightLeg);
			RightLeg.addBox("Right Leg", -2.0F, 4.0F, -2.0F, 4, 12, 4, strech);
			RightLeg.setRotationPoint(-3.0F, 0.0F + yoffset, 0.0F);

			LeftLeg = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.LeftLeg);
			LeftLeg.mirror = true;
			LeftLeg.addBox("Left Leg", -2.0F, 4.0F, -2.0F, 4, 12, 4, strech);
			LeftLeg.setRotationPoint(3.0F, 0.0F + yoffset, 0.0F);

			SteveArm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.None);
			SteveArm.addBox("Steve Arm?", -3.0F, -2.0F, -2.0F, 4, 12, 4, strech);
			SteveArm.setRotationPoint(-5.0F, 2.0F + yoffset, 0.0F);
			boxList.Remove(SteveArm);

			unicornarm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.None);
			unicornarm.addBox("Unicorn Arm?", -3.0F, -2.0F, -2.0F, 4, 12, 4, strech);
			unicornarm.setRotationPoint(-5.0F, 2.0F + yoffset, 0.0F);
			boxList.Remove(unicornarm);

			float txf = 0.0F;
			float tyf = 8.0F;
			float tzf = -14.0F;
			float TailR1 = 0.0F - txf;
			float TailR2 = 10.0F - tyf;
			float TailR3 = 0.0F;
			Tail = new ModelLoader.PlaneRenderer[10];

			Tail[0] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 32, 0, ModelPart.None);
			Tail[0].addTopPlane(-2.0F + txf, -7.0F + tyf, 16.0F + tzf, 4, 0, 4, strech);
			Tail[0].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[1] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 32, 0, ModelPart.None);
			Tail[1].addTopPlane(-2.0F + txf, 9.0F + tyf, 16.0F + tzf, 4, 0, 4, strech);
			Tail[1].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[2] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 32, 0, ModelPart.None);
			Tail[2].addBackPlane(-2.0F + txf, -7.0F + tyf, 16.0F + tzf, 4, 8, 0, strech);
			Tail[2].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[3] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 32, 0, ModelPart.None);
			Tail[3].addBackPlane(-2.0F + txf, -7.0F + tyf, 20.0F + tzf, 4, 8, 0, strech);
			Tail[3].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[4] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 32, 0, ModelPart.None);
			Tail[4].addBackPlane(-2.0F + txf, 1.0F + tyf, 16.0F + tzf, 4, 8, 0, strech);
			Tail[4].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[5] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 32, 0, ModelPart.None);
			Tail[5].addBackPlane(-2.0F + txf, 1.0F + tyf, 20.0F + tzf, 4, 8, 0, strech);
			Tail[5].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[6] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 36, 0, ModelPart.None);
			Tail[6].mirror = true;
			Tail[6].addSidePlane(2.0F + txf, -7.0F + tyf, 16.0F + tzf, 0, 8, 4, strech);
			Tail[6].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[7] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 36, 0, ModelPart.None);
			Tail[7].addSidePlane(-2.0F + txf, -7.0F + tyf, 16.0F + tzf, 0, 8, 4, strech);
			Tail[7].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[8] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 36, 0, ModelPart.None);
			Tail[8].mirror = true;
			Tail[8].addSidePlane(2.0F + txf, 1.0F + tyf, 16.0F + tzf, 0, 8, 4, strech);
			Tail[8].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			Tail[9] = new ModelLoader.PlaneRenderer(this, "Tail.Tail", 36, 0, ModelPart.None);
			Tail[9].addSidePlane(-2.0F + txf, 1.0F + tyf, 16.0F + tzf, 0, 8, 4, strech);
			Tail[9].setRotationPoint(TailR1, TailR2 + yoffset, TailR3);

			TailRotateAngleY = Tail[0].rotateAngleY;
			TailRotateAngleY = Tail[0].rotateAngleY;

			float WingR1 = 0.0F;

			float WingR2 = 0.0F;
			float WingR3 = 0.0F;

			LeftWing = new ModelLoader.ModelRenderer[3];

			LeftWing[0] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			LeftWing[0].mirror = true;
			LeftWing[0].addBox("Wings.Folded.Left Wing.Left Wing", 4.0F, 5.0F, 2.0F, 2, 6, 2, strech);
			LeftWing[0].setRotationPoint(WingR1, WingR2 + yoffset, WingR3);

			LeftWing[1] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			LeftWing[1].mirror = true;
			LeftWing[1].addBox("Wings.Folded.Left Wing.Left Wing", 4.0F, 5.0F, 4.0F, 2, 8, 2, strech);
			LeftWing[1].setRotationPoint(WingR1, WingR2 + yoffset, WingR3);

			LeftWing[2] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			LeftWing[2].mirror = true;
			LeftWing[2].addBox("Wings.Folded.Left Wing.Left Wing", 4.0F, 5.0F, 6.0F, 2, 6, 2, strech);
			LeftWing[2].setRotationPoint(WingR1, WingR2 + yoffset, WingR3);

			RightWing = new ModelLoader.ModelRenderer[3];

			RightWing[0] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			RightWing[0].addBox("Wings.Folded.Right Wing.Right Wing", -6.0F, 5.0F, 2.0F, 2, 6, 2, strech);
			RightWing[0].setRotationPoint(WingR1, WingR2 + yoffset, WingR3);

			RightWing[1] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			RightWing[1].addBox("Wings.Folded.Right Wing.Right Wing", -6.0F, 5.0F, 4.0F, 2, 8, 2, strech);
			RightWing[1].setRotationPoint(WingR1, WingR2 + yoffset, WingR3);

			RightWing[2] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			RightWing[2].addBox("Wings.Folded.Right Wing.Right Wing", -6.0F, 5.0F, 6.0F, 2, 6, 2, strech);
			RightWing[2].setRotationPoint(WingR1, WingR2 + yoffset, WingR3);

			float LeftWingExtR1 = headR1 + 4.5F;
			float LeftWingExtR2 = headR2 + 5.0F;
			float LeftWingExtR3 = headR3 + 6.0F;

			LeftWingExt = new ModelLoader.ModelRenderer[7];

			LeftWingExt[0] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[0].mirror = true;

			LeftWingExt[0].addBox("Wings.Extended.Left Wing Extended.Left Wing Extended", 0.0F, 0.0F, 0.0F, 1, 8, 2,
			                      strech + 0.1F);
			LeftWingExt[0].setRotationPoint(LeftWingExtR1, LeftWingExtR2 + yoffset, LeftWingExtR3);

			LeftWingExt[1] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[1].mirror = true;

			LeftWingExt[1].addBox("Wings.Extended.Left Wing Extended.Left Wing Extended", 0.0F, 8.0F, 0.0F, 1, 6, 2,
			                      strech + 0.1F);
			LeftWingExt[1].setRotationPoint(LeftWingExtR1, LeftWingExtR2 + yoffset, LeftWingExtR3);

			LeftWingExt[2] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[2].mirror = true;
			LeftWingExt[2].addBox("Wings.Extended.Left Wing Extended.Left Wing Extended", 0.0F, -1.2F, -0.2F, 1, 8, 2,
			                      strech - 0.2F);
			LeftWingExt[2].setRotationPoint(LeftWingExtR1, LeftWingExtR2 + yoffset, LeftWingExtR3);

			LeftWingExt[3] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[3].mirror = true;
			LeftWingExt[3].addBox("Wings.Extended.Left Wing Extended.Left Wing Extended", 0.0F, 1.8F, 1.3F, 1, 8, 2,
			                      strech - 0.1F);
			LeftWingExt[3].setRotationPoint(LeftWingExtR1, LeftWingExtR2 + yoffset, LeftWingExtR3);

			LeftWingExt[4] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[4].mirror = true;
			LeftWingExt[4].addBox("Wings.Extended.Left Wing Extended.Left Wing Extended", 0.0F, 5.0F, 2.0F, 1, 8, 2, strech);
			LeftWingExt[4].setRotationPoint(LeftWingExtR1, LeftWingExtR2 + yoffset, LeftWingExtR3);

			LeftWingExt[5] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[5].mirror = true;

			LeftWingExt[5].addBox("Wings.Extended.Left Wing Extended.Left Wing Extended", 0.0F, 0.0F, -0.2F, 1, 6, 2,
			                      strech + 0.3F);
			LeftWingExt[5].setRotationPoint(LeftWingExtR1, LeftWingExtR2 + yoffset, LeftWingExtR3);

			LeftWingExt[6] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[6].mirror = true;

			LeftWingExt[6].addBox("Wings.Extended.Left Wing Extended.Left Wing Extended", 0.0F, 0.0F, 0.2F, 1, 3, 2,
			                      strech + 0.2F);
			LeftWingExt[6].setRotationPoint(LeftWingExtR1, LeftWingExtR2 + yoffset, LeftWingExtR3);

			float RightWingExtR1 = headR1 - 5.5F;
			float RightWingExtR2 = headR2 + 5.0F;
			float RightWingExtR3 = headR3 + 6.0F;

			RightWingExt = new ModelLoader.ModelRenderer[7];

			RightWingExt[0] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[0].mirror = true;

			RightWingExt[0].addBox("Wings.Extended.Right Wing Extended.Right Wing Extended", 0.0F, 0.0F, 0.0F, 1, 8, 2,
			                       strech + 0.1F);
			RightWingExt[0].setRotationPoint(RightWingExtR1, RightWingExtR2 + yoffset, RightWingExtR3);

			RightWingExt[1] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[1].mirror = true;

			RightWingExt[1].addBox("Wings.Extended.Right Wing Extended.Right Wing Extended", 0.0F, 8.0F, 0.0F, 1, 6, 2,
			                       strech + 0.1F);
			RightWingExt[1].setRotationPoint(RightWingExtR1, RightWingExtR2 + yoffset, RightWingExtR3);

			RightWingExt[2] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[2].mirror = true;
			RightWingExt[2].addBox("Wings.Extended.Right Wing Extended.Right Wing Extended", 0.0F, -1.2F, -0.2F, 1, 8, 2,
			                       strech - 0.2F);
			RightWingExt[2].setRotationPoint(RightWingExtR1, RightWingExtR2 + yoffset, RightWingExtR3);

			RightWingExt[3] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[3].mirror = true;
			RightWingExt[3].addBox("Wings.Extended.Right Wing Extended.Right Wing Extended", 0.0F, 1.8F, 1.3F, 1, 8, 2,
			                       strech - 0.1F);
			RightWingExt[3].setRotationPoint(RightWingExtR1, RightWingExtR2 + yoffset, RightWingExtR3);

			RightWingExt[4] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[4].mirror = true;
			RightWingExt[4].addBox("Wings.Extended.Right Wing Extended.Right Wing Extended", 0.0F, 5.0F, 2.0F, 1, 8, 2, strech);
			RightWingExt[4].setRotationPoint(RightWingExtR1, RightWingExtR2 + yoffset, RightWingExtR3);

			RightWingExt[5] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[5].mirror = true;

			RightWingExt[5].addBox("Wings.Extended.Right Wing Extended.Right Wing Extended", 0.0F, 0.0F, -0.2F, 1, 6, 2,
			                       strech + 0.3F);
			RightWingExt[5].setRotationPoint(RightWingExtR1, RightWingExtR2 + yoffset, RightWingExtR3);

			RightWingExt[6] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[6].mirror = true;

			RightWingExt[6].addBox("Wings.Extended.Right Wing Extended.Right Wing Extended", 0.0F, 0.0F, 0.2F, 1, 3, 2,
			                       strech + 0.2F);
			RightWingExt[6].setRotationPoint(RightWingExtR1, RightWingExtR2 + yoffset, RightWingExtR3);

			WingRotateAngleX = LeftWingExt[0].rotateAngleX;
			WingRotateAngleY = LeftWingExt[0].rotateAngleY;
			WingRotateAngleZ = LeftWingExt[0].rotateAngleZ;

			animate(0, 0, 0, 0, 0);
		}

		public void animate(float Move, float Moveswing, float Loop, float Right, float Down)
		{
			int heldItemRight = 0;
			bool isSleeping = false, issneak = false, aimedBow = false;

			//Naming Error by MCP! Renamed for you...
			float SwingProgress = onGround;
			//
			rainboom = false;

			float headRotateAngleY;
			float headRotateAngleX;
			if (isSleeping)
			{
				// the full 90 degrees looks a bit off
				//headRotateAngleY = 1.571F;
				headRotateAngleY = 1.4F;
				headRotateAngleX = .1F;
			}
			else
			{
				headRotateAngleY = Right / 57.29578F;
				headRotateAngleX = Down / 57.29578F;
			}
			head.rotateAngleY = headRotateAngleY;
			head.rotateAngleX = headRotateAngleX;
			headpiece[0].rotateAngleY = headRotateAngleY;
			headpiece[0].rotateAngleX = headRotateAngleX;
			headpiece[1].rotateAngleY = headRotateAngleY;
			headpiece[1].rotateAngleX = headRotateAngleX;
			headpiece[2].rotateAngleY = headRotateAngleY;
			headpiece[2].rotateAngleX = headRotateAngleX;
			helmet.rotateAngleY = headRotateAngleY;
			helmet.rotateAngleX = headRotateAngleX;
			// finally, whatever else we did, tilt the horn forward a bit
			headpiece[2].rotateAngleX = headRotateAngleX + 0.5F;

			float rightarmRotateAngleX;
			float LeftArmRotateAngleX;
			float RightLegRotateAngleX;
			float LeftLegRotateAngleX;

			if (!isFlying || !isPegasus)
			{
				// swing the legs a bit
				rightarmRotateAngleX = (float) Math.Cos(Move * 0.6662F + (float) Math.PI) * 0.6F * Moveswing;
				LeftArmRotateAngleX = (float) Math.Cos(Move * 0.6662F) * 0.6F * Moveswing;
				RightLegRotateAngleX = (float) Math.Cos(Move * 0.6662F) * 0.3F * Moveswing;
				LeftLegRotateAngleX = (float) Math.Cos(Move * 0.6662F + (float) Math.PI) * 0.3F * Moveswing;
				rightarm.rotateAngleY = 0F;
				SteveArm.rotateAngleY = 0F;
				unicornarm.rotateAngleY = 0F;
				LeftArm.rotateAngleY = 0F;
				RightLeg.rotateAngleY = 0F;
				LeftLeg.rotateAngleY = 0F;
			}
			else
			{
				if (Moveswing < 0.9999F)
				{
					// just sweep the legs back slightly
					rainboom = false;
					rightarmRotateAngleX = (float) Math.Sin(0F - (Moveswing * 0.5F));
					LeftArmRotateAngleX = (float) Math.Sin(0F - (Moveswing * 0.5F));
					RightLegRotateAngleX = (float) Math.Sin(Moveswing * 0.5F);
					LeftLegRotateAngleX = (float) Math.Sin(Moveswing * 0.5F);
				}
				else
				{
					//sonic rainboom pose
					rainboom = true;
					rightarmRotateAngleX = 4.712F;
					LeftArmRotateAngleX = 4.712F;
					RightLegRotateAngleX = 1.571F;
					LeftLegRotateAngleX = 1.571F;
				}
				rightarm.rotateAngleY = .2F;
				SteveArm.rotateAngleY = .2F;
				LeftArm.rotateAngleY = -.2F;
				RightLeg.rotateAngleY = -.2F;
				LeftLeg.rotateAngleY = .2F;
			}

			if (isSleeping)
			{
				rightarmRotateAngleX = 4.712F;
				LeftArmRotateAngleX = 4.712F;
				RightLegRotateAngleX = 1.571F;
				LeftLegRotateAngleX = 1.571F;
			}

			rightarm.rotateAngleX = rightarmRotateAngleX;
			SteveArm.rotateAngleX = rightarmRotateAngleX;
			// unicorn arm stays still, because magic
			//unicornarm.rotateAngleX = rightarmRotateAngleX;
			unicornarm.rotateAngleX = 0F;
			LeftArm.rotateAngleX = LeftArmRotateAngleX;
			RightLeg.rotateAngleX = RightLegRotateAngleX;
			LeftLeg.rotateAngleX = LeftLegRotateAngleX;
			rightarm.rotateAngleZ = 0F;
			SteveArm.rotateAngleZ = 0F;
			unicornarm.rotateAngleZ = 0F;
			LeftArm.rotateAngleZ = 0F;


			for (int i = 0; i < Tail.Length; i++)
			{
				if (rainboom)
					Tail[i].rotateAngleZ = 0F;
				else
					Tail[i].rotateAngleZ = (float) Math.Cos(Move * 0.8F) * 0.2F * Moveswing;
			}

			if ((heldItemRight != 0) && !rainboom)
			{
				if (isUnicorn)
				{
					// unicorn arm stays still, because magic
					//unicornarm.rotateAngleX = rightarm.rotateAngleX * 0.5F - (float)Math.PI * 0.1F;
				}
				else
				{
					rightarm.rotateAngleX = rightarm.rotateAngleX * 0.5F - (float) Math.PI * 0.1F;
					SteveArm.rotateAngleX = SteveArm.rotateAngleX * 0.5F - (float) Math.PI * 0.1F;
				}
			}

			float BodyRotateAngleY = 0F;

			if ((SwingProgress > -9990F) && !isUnicorn)
			{
				// swing the body and tail if we are smashing stuff
				BodyRotateAngleY = (float) Math.Sin((float) Math.Sqrt(SwingProgress) * (float) Math.PI * 2F) * 0.2F;
			}

			Body.rotateAngleY = (float) (BodyRotateAngleY * 0.2);
			for (int i = 0; i < Bodypiece.Length; i++)
				Bodypiece[i].rotateAngleY = (float) (BodyRotateAngleY * 0.2);
			for (int i = 0; i < LeftWing.Length; i++)
				LeftWing[i].rotateAngleY = (float) (BodyRotateAngleY * 0.2);
			for (int i = 0; i < RightWing.Length; i++)
				RightWing[i].rotateAngleY = (float) (BodyRotateAngleY * 0.2);

			for (int i = 0; i < Tail.Length; i++)
				Tail[i].rotateAngleY = BodyRotateAngleY;

			float ArmRotationPointZ = (float) Math.Sin(Body.rotateAngleY) * 5F;
			float ArmRotationPointX = (float) Math.Cos(Body.rotateAngleY) * 5F;
			float LegSplay = 4F;
			if (issneak && !isFlying)
				LegSplay = 0F;
			if (isSleeping)
				LegSplay = 2.6F;
			if (rainboom)
			{
				rightarm.rotationPointZ = ArmRotationPointZ + 2F;
				SteveArm.rotationPointZ = ArmRotationPointZ + 2F;
				LeftArm.rotationPointZ = 0 - ArmRotationPointZ + 2F;
			}
			else
			{
				rightarm.rotationPointZ = ArmRotationPointZ + 1F;
				SteveArm.rotationPointZ = ArmRotationPointZ + 1F;
				LeftArm.rotationPointZ = 0 - ArmRotationPointZ + 1F;
			}
			rightarm.rotationPointX = 0 - ArmRotationPointX - 1F + LegSplay;
			SteveArm.rotationPointX = 0 - ArmRotationPointX;
			LeftArm.rotationPointX = ArmRotationPointX + 1F - LegSplay;
			RightLeg.rotationPointX = 0 - ArmRotationPointX - 1F + LegSplay;
			LeftLeg.rotationPointX = ArmRotationPointX + 1F - LegSplay;


			rightarm.rotateAngleY += Body.rotateAngleY;
			LeftArm.rotateAngleY += Body.rotateAngleY;
			LeftArm.rotateAngleX += Body.rotateAngleY;


			// set rotation point Y as well, to reset leg position after riding
			rightarm.rotationPointY = 8F;
			LeftArm.rotationPointY = 8F;
			RightLeg.rotationPointY = 4F;
			LeftLeg.rotationPointY = 4F;

			if (SwingProgress > -9990F)
			{
				float f = SwingProgress;
				f = 1F - SwingProgress;
				f *= f * f; //f^3
				f = 1F - f;
				var f1 = (float) Math.Sin(f * (float) Math.PI);
				var SwingProgressPi = (float) Math.Sin(SwingProgress * (float) Math.PI);
				float f2 = SwingProgressPi * -(head.rotateAngleX - 0.7F) * 0.75F;

				if (isUnicorn)
				{
					// swingprogress is used when we hit stuff with a pick or whatever      
					unicornarm.rotateAngleX -= (float) (f1 * 1.2D + f2);
					unicornarm.rotateAngleY += Body.rotateAngleY * 2F;
					unicornarm.rotateAngleZ = SwingProgressPi * -0.4F;
				}
				else
				{
					rightarm.rotateAngleX -= (float) (f1 * 1.2D + f2);
					rightarm.rotateAngleY += Body.rotateAngleY * 2F;
					rightarm.rotateAngleZ = SwingProgressPi * -0.4F;

					SteveArm.rotateAngleX -= (float) (f1 * 1.2D + f2);
					SteveArm.rotateAngleY += Body.rotateAngleY * 2F;
					SteveArm.rotateAngleZ = SwingProgressPi * -0.4F;
				}
			}

			if (issneak && !isFlying)
			{
				// face down, plot up, that's the way we like to buck

				// body stuff
				float BodyRotateAngleX = 0.4F;
				float BodyRotationPointY = 7F;
				float BodyRotationPointZ = -4F;
				Body.rotateAngleX = BodyRotateAngleX;
				Body.rotationPointY = BodyRotationPointY;
				Body.rotationPointZ = BodyRotationPointZ;
				for (int i = 0; i < Bodypiece.Length; i++)
				{
					Bodypiece[i].rotateAngleX = BodyRotateAngleX;
					Bodypiece[i].rotationPointY = BodyRotationPointY;
					Bodypiece[i].rotationPointZ = BodyRotationPointZ;
				}


				// extended wings
				float lwrpy = 3.5F;
				float lwrpz = 6F;

				for (int i = 0; i < LeftWingExt.Length; i++)
				{
					LeftWingExt[i].rotateAngleX = (float) (BodyRotateAngleX + (float) Math.PI * 0.75);
					LeftWingExt[i].rotationPointY = BodyRotationPointY + lwrpy;
					LeftWingExt[i].rotationPointZ = BodyRotationPointZ + lwrpz;
					// point the wings up
					LeftWingExt[i].rotateAngleX = 2.5F;
					LeftWingExt[i].rotateAngleZ = -6F;
				}

				float rwrpy = 4.5F;
				float rwrpz = 6F;

				for (int i = 0; i < LeftWingExt.Length; i++)
				{
					RightWingExt[i].rotateAngleX = (float) (BodyRotateAngleX + (float) Math.PI * 0.75);
					RightWingExt[i].rotationPointY = BodyRotationPointY + rwrpy;
					RightWingExt[i].rotationPointZ = BodyRotationPointZ + rwrpz;
					// point the wings up
					RightWingExt[i].rotateAngleX = 2.5F;
					RightWingExt[i].rotateAngleZ = 6F;
				}

				// legs stuff
				RightLeg.rotateAngleX -= 0F;
				LeftLeg.rotateAngleX -= 0F;
				rightarm.rotateAngleX -= 0.4F;
				SteveArm.rotateAngleX += 0.4F;
				unicornarm.rotateAngleX += 0.4F;
				LeftArm.rotateAngleX -= 0.4F;
				RightLeg.rotationPointZ = 10F;
				LeftLeg.rotationPointZ = 10F;
				RightLeg.rotationPointY = 7F;
				LeftLeg.rotationPointY = 7F;

				// head stuff
				float headRotationPointY;
				float headRotationPointZ;
				float headRotationPointX;
				if (isSleeping)
				{
					headRotationPointY = 2F;
					headRotationPointZ = -1F;
					headRotationPointX = 1F;
				}
				else
				{
					headRotationPointY = 6F;
					headRotationPointZ = -2F;
					headRotationPointX = 0F;
				}
				head.rotationPointY = headRotationPointY;
				head.rotationPointZ = headRotationPointZ;
				head.rotationPointX = headRotationPointX;
				helmet.rotationPointY = headRotationPointY;
				helmet.rotationPointZ = headRotationPointZ;
				helmet.rotationPointX = headRotationPointX;
				headpiece[0].rotationPointY = headRotationPointY;
				headpiece[0].rotationPointZ = headRotationPointZ;
				headpiece[0].rotationPointX = headRotationPointX;
				headpiece[1].rotationPointY = headRotationPointY;
				headpiece[1].rotationPointZ = headRotationPointZ;
				headpiece[1].rotationPointX = headRotationPointX;
				headpiece[2].rotationPointY = headRotationPointY;
				headpiece[2].rotationPointZ = headRotationPointZ;
				headpiece[2].rotationPointX = headRotationPointX;

				// tail stuff
				float txf = 0F; // tail horizontal fudge
				float tyf = 8F; // tail vertical fudge
				float tzf = -14F; // tail depth fudge
				float TailRotationPointX = 0.0F - txf;
				float TailRotationPointY = 9F - tyf;
				float TailRotationPointZ = -4F - tzf;
				float TailRotateAngleX = 0.0F;
				for (int i = 0; i < Tail.Length; i++)
				{
					Tail[i].rotationPointX = TailRotationPointX;
					Tail[i].rotationPointY = TailRotationPointY;
					Tail[i].rotationPointZ = TailRotationPointZ;
					Tail[i].rotateAngleX = TailRotateAngleX;
				}
			}
			else
			{
				// body stuff
				float BodyRotateAngleX = 0F;
				float BodyRotationPointY = 0F;
				float BodyRotationPointZ = 0F;
				Body.rotateAngleX = BodyRotateAngleX;
				Body.rotationPointY = BodyRotationPointY;
				Body.rotationPointZ = BodyRotationPointZ;
				for (int i = 0; i < Bodypiece.Length; i++)
				{
					Bodypiece[i].rotateAngleX = BodyRotateAngleX;
					Bodypiece[i].rotationPointY = BodyRotationPointY;
					Bodypiece[i].rotationPointZ = BodyRotationPointZ;
				}

				if (isPegasus)
				{
					if (!isFlying)
					{
						// wings folded
						for (int i = 0; i < LeftWing.Length; i++)
						{
							LeftWing[i].rotateAngleX = (float) (BodyRotateAngleX + (float) Math.PI * 0.5);
							LeftWing[i].rotationPointY = BodyRotationPointY + 13F;
							LeftWing[i].rotationPointZ = BodyRotationPointZ - 3F;
						}
						for (int i = 0; i < RightWing.Length; i++)
						{
							RightWing[i].rotateAngleX = (float) (BodyRotateAngleX + (float) Math.PI * 0.5);
							RightWing[i].rotationPointY = BodyRotationPointY + 13F;
							RightWing[i].rotationPointZ = BodyRotationPointZ - 3F;
						}
					}
					else
					{
						// wings extended
						float lwrpy = 5.5F;
						float lwrpz = 3F;

						for (int i = 0; i < LeftWingExt.Length; i++)
						{
							LeftWingExt[i].rotateAngleX = (float) (BodyRotateAngleX + (float) Math.PI * 0.5);
							LeftWingExt[i].rotationPointY = BodyRotationPointY + lwrpy;
							LeftWingExt[i].rotationPointZ = BodyRotationPointZ + lwrpz;
						}

						float rwrpy = 6.5F;
						float rwrpz = 3F;

						for (int i = 0; i < RightWingExt.Length; i++)
						{
							RightWingExt[i].rotateAngleX = (float) (BodyRotateAngleX + (float) Math.PI * 0.5);
							RightWingExt[i].rotationPointY = BodyRotationPointY + rwrpy;
							RightWingExt[i].rotationPointZ = BodyRotationPointZ + rwrpz;
						}
					}
				}

				// legs stuff
				RightLeg.rotationPointZ = 10F;
				LeftLeg.rotationPointZ = 10F;
				RightLeg.rotationPointY = 8F;
				LeftLeg.rotationPointY = 8F;

				// swing the front arms a bit when still
				// scratch that, only swing steve arm and pony tail
				float ArmRotateAngleZ = (float) Math.Cos(Loop * 0.09F) * 0.05F + 0.05F;
				float ArmRotateAngleX = (float) Math.Sin(Loop * 0.067F) * 0.05F;
				SteveArm.rotateAngleZ += ArmRotateAngleZ;
				unicornarm.rotateAngleZ += ArmRotateAngleZ;
				SteveArm.rotateAngleX += ArmRotateAngleX;
				unicornarm.rotateAngleX += ArmRotateAngleX;

				if (isPegasus && isFlying)
				{
					// swing (flap) the large wings a bit
					WingRotateAngleY = (float) Math.Sin(Loop * 0.067F * 8) * 1F;
					WingRotateAngleZ = (float) Math.Sin(Loop * 0.067F * 8) * 1F;
					for (int i = 0; i < LeftWingExt.Length; i++)
					{
						LeftWingExt[i].rotateAngleX = 2.5F;
						LeftWingExt[i].rotateAngleZ = -WingRotateAngleZ - 4.712F - .4F;
					}
					for (int i = 0; i < RightWingExt.Length; i++)
					{
						RightWingExt[i].rotateAngleX = 2.5F;
						RightWingExt[i].rotateAngleZ = WingRotateAngleZ + 4.712F + .4F;
					}
				}

				// head stuff
				float headRotationPointY;
				float headRotationPointZ;
				float headRotationPointX;
				if (isSleeping)
				{
					headRotationPointY = 2F;
					headRotationPointZ = 1F;
					headRotationPointX = 1F;
				}
				else
				{
					headRotationPointY = 0F;
					headRotationPointZ = 0F;
					headRotationPointX = 0F;
				}
				head.rotationPointY = headRotationPointY;
				head.rotationPointZ = headRotationPointZ;
				head.rotationPointX = headRotationPointX;
				helmet.rotationPointY = headRotationPointY;
				helmet.rotationPointZ = headRotationPointZ;
				helmet.rotationPointX = headRotationPointX;
				headpiece[0].rotationPointY = headRotationPointY;
				headpiece[0].rotationPointZ = headRotationPointZ;
				headpiece[0].rotationPointX = headRotationPointX;
				headpiece[1].rotationPointY = headRotationPointY;
				headpiece[1].rotationPointZ = headRotationPointZ;
				headpiece[1].rotationPointX = headRotationPointX;
				headpiece[2].rotationPointY = headRotationPointY;
				headpiece[2].rotationPointZ = headRotationPointZ;
				headpiece[2].rotationPointX = headRotationPointX;

				float txf = 0F; // tail horizontal fudge
				float tyf = 8F; // tail vertical fudge
				float tzf = -14F; // tail depth fudge
				float TailRotationPointX = 0F - txf;
				float TailRotationPointY = 9F - tyf;
				float TailRotationPointZ = 0F - tzf;
				float TailRotateAngleX = 0.5F * Moveswing;
				for (int i = 0; i < Tail.Length; i++)
				{
					Tail[i].rotationPointX = TailRotationPointX;
					Tail[i].rotationPointY = TailRotationPointY;
					Tail[i].rotationPointZ = TailRotationPointZ;
					if (rainboom)
						Tail[i].rotateAngleX = 1.571F + 0.1F * (float) Math.Sin(Move);
					else
						Tail[i].rotateAngleX = TailRotateAngleX;
				}

				// swing the tail a bit
				for (int i = 0; i < Tail.Length; i++)
				{
					if (rainboom)
					{
						// does nothing
					}
					else
						Tail[i].rotateAngleX += ArmRotateAngleX;
				}
			}

			//// finally, whatever else we did, tilt the wing feathers all over the place
			//// shortest feather of large wing, to be rotated 60 degrees
			LeftWingExt[2].rotateAngleX = LeftWingExt[2].rotateAngleX - .85F;
			//// lower middle feather of large wing, also to be rotated 60 degrees
			LeftWingExt[3].rotateAngleX = LeftWingExt[3].rotateAngleX - .75F;
			//// upper middle feather of large wing, to be rotated 40 degrees
			LeftWingExt[4].rotateAngleX = LeftWingExt[4].rotateAngleX - 0.5F;
			//// other feathers of smaller wing, to be rotated 60 degrees
			LeftWingExt[6].rotateAngleX = LeftWingExt[6].rotateAngleX - .85F;

			//// shortest feather of large wing, to be rotated 60 degrees
			RightWingExt[2].rotateAngleX = RightWingExt[2].rotateAngleX - .85F;
			//// lower middle feather of large wing, also to be rotated 60 degrees
			RightWingExt[3].rotateAngleX = RightWingExt[3].rotateAngleX - .75F;
			//// upper middle feather of large wing, to be rotated 40 degrees
			RightWingExt[4].rotateAngleX = RightWingExt[4].rotateAngleX - 0.5F;
			//// other feathers of smaller wing, to be rotated 60 degrees
			RightWingExt[6].rotateAngleX = RightWingExt[6].rotateAngleX - .85F;


			// finally, whatever else we did, tilt the tail joining piece up a bit
			Bodypiece[9].rotateAngleX = Bodypiece[9].rotateAngleX + 0.5F;
			Bodypiece[10].rotateAngleX = Bodypiece[10].rotateAngleX + 0.5F;
			Bodypiece[11].rotateAngleX = Bodypiece[11].rotateAngleX + 0.5F;
			Bodypiece[12].rotateAngleX = Bodypiece[12].rotateAngleX + 0.5F;

			// move the tail into position if we are rainbooming
			if (rainboom)
			{
				for (int i = 0; i < Tail.Length; i++)
				{
					Tail[i].rotationPointY = Tail[i].rotationPointY + 6;
					Tail[i].rotationPointZ = Tail[i].rotationPointZ + 1;
				}
			}

			if (isRiding)
			{
				// We need to move every single piece up and forward (except Steve's arm)
				// so the pony stands in the boat.
				float ShiftY = -10F;
				float ShiftZ = -10F;
				//    rightarm.rotateAngleX += -0.6283185F;
				//      SteveArm.rotateAngleX += -0.6283185F;
				//    LeftArm.rotateAngleX += -0.6283185F;
				//    RightLeg.rotateAngleX = -1.256637F;
				//    LeftLeg.rotateAngleX = -1.256637F;
				//    RightLeg.rotateAngleY = (float)Math.PI * 0.1F;
				//    LeftLeg.rotateAngleY = -(float)Math.PI * 0.1F;
				head.rotationPointY = head.rotationPointY + ShiftY;
				head.rotationPointZ = head.rotationPointZ + ShiftZ;
				headpiece[0].rotationPointY = headpiece[0].rotationPointY + ShiftY;
				headpiece[0].rotationPointZ = headpiece[0].rotationPointZ + ShiftZ;
				headpiece[1].rotationPointY = headpiece[1].rotationPointY + ShiftY;
				headpiece[1].rotationPointZ = headpiece[1].rotationPointZ + ShiftZ;
				//if (isUnicorn) {
				headpiece[2].rotationPointY = headpiece[2].rotationPointY + ShiftY;
				headpiece[2].rotationPointZ = headpiece[2].rotationPointZ + ShiftZ;
				//}
				//for(int i = 0; i < headpiece.Length; i++)
				//{
				//      headpiece[i].rotationPointY = headpiece[i].rotationPointY + ShiftY;
				//      headpiece[i].rotationPointZ = headpiece[i].rotationPointZ + ShiftZ;
				//}
				helmet.rotationPointY = helmet.rotationPointY + ShiftY;
				helmet.rotationPointZ = helmet.rotationPointZ + ShiftZ;
				Body.rotationPointY = Body.rotationPointY + ShiftY;
				Body.rotationPointZ = Body.rotationPointZ + ShiftZ;
				for (int i = 0; i < Bodypiece.Length; i++)
				{
					Bodypiece[i].rotationPointY = Bodypiece[i].rotationPointY + ShiftY;
					Bodypiece[i].rotationPointZ = Bodypiece[i].rotationPointZ + ShiftZ;
				}
				LeftArm.rotationPointY = LeftArm.rotationPointY + ShiftY;
				LeftArm.rotationPointZ = LeftArm.rotationPointZ + ShiftZ;
				rightarm.rotationPointY = rightarm.rotationPointY + ShiftY;
				rightarm.rotationPointZ = rightarm.rotationPointZ + ShiftZ;
				LeftLeg.rotationPointY = LeftLeg.rotationPointY + ShiftY;
				LeftLeg.rotationPointZ = LeftLeg.rotationPointZ + ShiftZ;
				RightLeg.rotationPointY = RightLeg.rotationPointY + ShiftY;
				RightLeg.rotationPointZ = RightLeg.rotationPointZ + ShiftZ;
				for (int i = 0; i < Tail.Length; i++)
				{
					Tail[i].rotationPointY = Tail[i].rotationPointY + ShiftY;
					Tail[i].rotationPointZ = Tail[i].rotationPointZ + ShiftZ;
				}

				// wings folded
				for (int i = 0; i < LeftWing.Length; i++)
				{
					LeftWing[i].rotationPointY = LeftWing[i].rotationPointY + ShiftY;
					LeftWing[i].rotationPointZ = LeftWing[i].rotationPointZ + ShiftZ;
				}
				for (int i = 0; i < RightWing.Length; i++)
				{
					RightWing[i].rotationPointY = RightWing[i].rotationPointY + ShiftY;
					RightWing[i].rotationPointZ = RightWing[i].rotationPointZ + ShiftZ;
				}

				//wings extended
				for (int i = 0; i < LeftWingExt.Length; i++)
				{
					LeftWingExt[i].rotationPointY = LeftWingExt[i].rotationPointY + ShiftY;
					LeftWingExt[i].rotationPointZ = LeftWingExt[i].rotationPointZ + ShiftZ;
				}

				for (int i = 0; i < RightWingExt.Length; i++)
				{
					RightWingExt[i].rotationPointY = RightWingExt[i].rotationPointY + ShiftY;
					RightWingExt[i].rotationPointZ = RightWingExt[i].rotationPointZ + ShiftZ;
				}
			}

			if (isSleeping)
			{
				// move the head down into the body, and lock it rotated fully 90 degrees.
				// move the legs out to the sides of the body (a bit)
				// and rotate the front legs fully forward and the back legs fully back (a bit like a rainboom pose).
				rightarm.rotationPointZ = rightarm.rotationPointZ + 6F;
				LeftArm.rotationPointZ = LeftArm.rotationPointZ + 6F;
				RightLeg.rotationPointZ = RightLeg.rotationPointZ - 8F;
				LeftLeg.rotationPointZ = LeftLeg.rotationPointZ - 8F;
				rightarm.rotationPointY = rightarm.rotationPointY + 2F;
				LeftArm.rotationPointY = LeftArm.rotationPointY + 2F;
				RightLeg.rotationPointY = RightLeg.rotationPointY + 2F;
				LeftLeg.rotationPointY = LeftLeg.rotationPointY + 2F;
			}

			if (aimedBow)
			{
				if (isUnicorn)
				{
					float f7 = 0.0F;
					float f9 = 0.0F;
					unicornarm.rotateAngleZ = 0.0F;
					unicornarm.rotateAngleY = -(0.1F - f7 * 0.6F) + head.rotateAngleY;
					unicornarm.rotateAngleX = 4.712F + head.rotateAngleX;
					unicornarm.rotateAngleX -= f7 * 1.2F - f9 * 0.4F;
					float f2 = 0; // I am lazy
					unicornarm.rotateAngleZ += (float) Math.Cos(f2 * 0.09F) * 0.05F + 0.05F;
					unicornarm.rotateAngleX += (float) Math.Sin(f2 * 0.067F) * 0.05F;
				}
				else
				{
					float f7 = 0.0F;
					float f9 = 0.0F;
					rightarm.rotateAngleZ = 0.0F;
					rightarm.rotateAngleY = -(0.1F - f7 * 0.6F) + head.rotateAngleY;
					rightarm.rotateAngleX = 4.712F + head.rotateAngleX;
					rightarm.rotateAngleX -= f7 * 1.2F - f9 * 0.4F;
					float f2 = 0; // I am lazy
					rightarm.rotateAngleZ += (float) Math.Cos(f2 * 0.09F) * 0.05F + 0.05F;
					rightarm.rotateAngleX += (float) Math.Sin(f2 * 0.067F) * 0.05F;
					// move the arm back a bit so it looks less weird
					rightarm.rotationPointZ = rightarm.rotationPointZ + 1F;
				}
			}
		}
	}


	// Decompiled by Jad v1.5.8g. Copyright 2001 Pavel Kouznetsov.
	// Jad home page: http://www.kpdus.com/jad.html
	// Decompiler options: packimports(3) braces deadcode fieldsfirst

	// Referenced classes of package net.minecraft.src:
	//                  ModelPlayer, ModelRenderer, PlaneRenderer, AniParams,
	//                  MathHelper, EntityPlayer, InventoryPlayer, ItemStack,
	//                  RenderManager, RenderEngine, Tessellator

	public class pm_newPonyAdv : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer Body;
		public ModelLoader.PlaneRenderer[] Bodypiece;
		public ModelLoader.PlaneRenderer[] BodypieceNeck;
		public ModelLoader.ModelRenderer LeftArm;
		public ModelLoader.ModelRenderer LeftLeg;
		public ModelLoader.ModelRenderer[] LeftWing;
		public ModelLoader.ModelRenderer[] LeftWingExt;
		public ModelLoader.PlaneRenderer[] MuzzleFemale;
		public ModelLoader.PlaneRenderer[] MuzzleMale;
		private float NeckRotX = 0.166F;
		public ModelLoader.ModelRenderer RightLeg;
		public ModelLoader.ModelRenderer[] RightWing;
		public ModelLoader.ModelRenderer[] RightWingExt;
		public ModelLoader.ModelRenderer SteveArm;
		public ModelLoader.PlaneRenderer[] Tail;
		private float TailRotateAngleY;
		private float WingRotateAngleX;
		private float WingRotateAngleY;
		private float WingRotateAngleZ;
		public ModelLoader.ModelRenderer head;
		public ModelLoader.ModelRenderer[] headpiece;
		public ModelLoader.ModelRenderer helmet;
		private bool rainboom;
		public ModelLoader.ModelRenderer rightarm;
		private float strech;
		public int tailstop;
		public ModelLoader.ModelRenderer unicornarm;

		public void init()
		{
			init(0.0F);
		}

		public void init(float f)
		{
			init(f, 0.0F);
		}

		public pm_newPonyAdv init(float f, float f1)
		{
			strech = f1;
			float f2 = 0.0F;
			float f3 = 0.0F;
			float f4 = 0.0F;
			head = new ModelLoader.ModelRenderer(this, 0, 0, ModelPart.Head);
			head.addBox("Head", -4F, -5F, -6F, 8, 8, 8, strech);
			head.setRotationPoint(f2, f3 + f, f4);
			headpiece = new ModelLoader.ModelRenderer[3];
			headpiece[0] = new ModelLoader.ModelRenderer(this, 12, 16, ModelPart.Helmet);
			headpiece[0].addBox("Ear", -4F, -7F, -1F, 2, 2, 2, strech);
			headpiece[0].setRotationPoint(f2, f3 + f, f4);
			headpiece[1] = new ModelLoader.ModelRenderer(this, 12, 16, ModelPart.Helmet);
			headpiece[1].mirror = true;
			headpiece[1].addBox("Ear", 2.0F, -7F, -1F, 2, 2, 2, strech);
			headpiece[1].setRotationPoint(f2, f3 + f, f4);
			headpiece[2] = new ModelLoader.ModelRenderer(this, 0, 3, ModelPart.Helmet);
			headpiece[2].addBox("Horn", -0.5F, -11F, -3.5F, 1, 4, 1, strech);
			headpiece[2].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale = new ModelLoader.PlaneRenderer[10];
			MuzzleFemale[0] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 10, 14, ModelPart.Head);
			MuzzleFemale[0].addBackPlane(-2F, 1.0F, -7F, 4, 2, 0, strech);
			MuzzleFemale[0].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[1] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 11, 13, ModelPart.Head);
			MuzzleFemale[1].addBackPlane(-1F, 0F, -7F, 2, 1, 0, strech);
			MuzzleFemale[1].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[2] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 9, 14, ModelPart.Head);
			MuzzleFemale[2].addTopPlane(-2F, 1.0F, -7F, 1, 0, 1, strech);
			MuzzleFemale[2].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[3] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 14, 14, ModelPart.Head);
			MuzzleFemale[3].addTopPlane(1.0F, 1.0F, -7F, 1, 0, 1, strech);
			MuzzleFemale[3].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[4] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 11, 12, ModelPart.Head);
			MuzzleFemale[4].addTopPlane(-1F, 0F, -7F, 2, 0, 1, strech);
			MuzzleFemale[4].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[5] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 18, 7, ModelPart.Head);
			MuzzleFemale[5].addTopPlane(-2F, 3F, -7F, 4, 0, 1, strech);
			MuzzleFemale[5].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[6] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 9, 14, ModelPart.Head);
			MuzzleFemale[6].addSidePlane(-2F, 1.0F, -7F, 0, 2, 1, strech);
			MuzzleFemale[6].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[7] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 14, 14, ModelPart.Head);
			MuzzleFemale[7].addSidePlane(2.0F, 1.0F, -7F, 0, 2, 1, strech);
			MuzzleFemale[7].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[8] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 11, 12, ModelPart.Head);
			MuzzleFemale[8].addSidePlane(-1F, 0F, -7F, 0, 1, 1, strech);
			MuzzleFemale[8].setRotationPoint(f2, f3 + f, f4);
			MuzzleFemale[9] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Female.Female Muzzle", 12, 12, ModelPart.Head);
			MuzzleFemale[9].addSidePlane(1.0F, 0F, -7F, 0, 1, 1, strech);
			MuzzleFemale[9].setRotationPoint(f2, f3 + f, f4);
			MuzzleMale = new ModelLoader.PlaneRenderer[5];
			MuzzleMale[0] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Male.Male Muzzle", 10, 13, ModelPart.Head);
			MuzzleMale[0].addBackPlane(-2.0F, 0F, -7.0F, 4, 3, 0, strech);
			MuzzleMale[0].setRotationPoint(f2, f3 + f, f4);
			MuzzleMale[1] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Male.Male Muzzle", 10, 13, ModelPart.Head);
			MuzzleMale[1].addTopPlane(-2.0F, 0F, -7.0F, 4, 0, 1, strech);
			MuzzleMale[1].setRotationPoint(f2, f3 + f, f4);
			MuzzleMale[2] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Male.Male Muzzle", 18, 7, ModelPart.Head);
			MuzzleMale[2].addTopPlane(-2.0F, 3.0F, -7.0F, 4, 0, 1, strech);
			MuzzleMale[2].setRotationPoint(f2, f3 + f, f4);
			MuzzleMale[3] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Male.Male Muzzle", 10, 13, ModelPart.Head);
			MuzzleMale[3].addSidePlane(-2.0F, 0F, -7.0F, 0, 3, 1, strech);
			MuzzleMale[3].setRotationPoint(f2, f3 + f, f4);
			MuzzleMale[4] = new ModelLoader.PlaneRenderer(this, "Muzzles.@Male.Male Muzzle", 13, 13, ModelPart.Head);
			MuzzleMale[4].addSidePlane(2.0F, 0F, -7.0F, 0, 3, 1, strech);
			MuzzleMale[4].setRotationPoint(f2, f3 + f, f4);
			helmet = new ModelLoader.ModelRenderer(this, 32, 0, ModelPart.Helmet);
			helmet.addBox("Hair", -4F, -5F, -6F, 8, 8, 8, strech + 0.5F);
			helmet.setRotationPoint(f2, f3 + f, f4);
			float f5 = 0.0F;
			float f6 = 0.0F;
			float f7 = 0.0F;
			Body = new ModelLoader.ModelRenderer(this, 16, 16, ModelPart.Chest);
			Body.addBox("Body.Body", -4F, 4F, -2F, 8, 8, 4, strech);
			Body.setRotationPoint(f5, f6 + f, f7);
			Bodypiece = new ModelLoader.PlaneRenderer[14];
			Bodypiece[0] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[0].addSidePlane(-4F, 4F, 2.0F, 0, 8, 8, strech);
			Bodypiece[0].setRotationPoint(f5, f6 + f, f7);
			Bodypiece[1] = new ModelLoader.PlaneRenderer(this, "Body.Body", 24, 0, ModelPart.Chest);
			Bodypiece[1].addSidePlane(4F, 4F, 2.0F, 0, 8, 8, strech);
			Bodypiece[1].setRotationPoint(f5, f6 + f, f7);
			Bodypiece[2] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 20, ModelPart.Chest);
			Bodypiece[2].addTopPlane(-4F, 4F, 2.0F, 8, 0, 12, strech);
			Bodypiece[2].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[3] = new ModelLoader.PlaneRenderer(this, "Body.Body", 56, 0, ModelPart.Chest);
			Bodypiece[3].addTopPlane(-4F, 12F, 2.0F, 8, 0, 8, strech);
			Bodypiece[3].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[4] = new ModelLoader.PlaneRenderer(this, "Body.Cutie Mark", 4, 0, ModelPart.Chest);
			Bodypiece[4].addSidePlane(-4F, 4F, 10F, 0, 8, 4, strech);
			Bodypiece[4].setRotationPoint(f5, f6 + f, f7);
			Bodypiece[5] = new ModelLoader.PlaneRenderer(this, "Body.Body", 4, 0, ModelPart.Chest);
			Bodypiece[5].addSidePlane(4F, 4F, 10F, 0, 8, 4, strech);
			Bodypiece[5].setRotationPoint(f5, f6 + f, f7);
			Bodypiece[6] = new ModelLoader.PlaneRenderer(this, "Body.Body", 36, 16, ModelPart.Chest);
			Bodypiece[6].addBackPlane(-4F, 4F, 14F, 8, 4, 0, strech);
			Bodypiece[6].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[7] = new ModelLoader.PlaneRenderer(this, "Body.Body", 36, 16, ModelPart.Chest);
			Bodypiece[7].addTopPlane(-4F, 12F, 10F, 8, 0, 4, strech);
			Bodypiece[7].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[8] = new ModelLoader.PlaneRenderer(this, "Body.Body", 36, 16, ModelPart.Chest);
			Bodypiece[8].addBackPlane(-4F, 8F, 14F, 8, 4, 0, strech);
			Bodypiece[8].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[9] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[9].addTopPlane(-1F, 10F, 8F, 2, 0, 6, strech);
			Bodypiece[9].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[10] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[10].addTopPlane(-1F, 12F, 8F, 2, 0, 6, strech);
			Bodypiece[10].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[11] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[11].mirror = true;
			Bodypiece[11].addSidePlane(-1F, 10F, 8F, 0, 2, 6, strech);
			Bodypiece[11].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[12] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[12].addSidePlane(1.0F, 10F, 8F, 0, 2, 6, strech);
			Bodypiece[12].setRotationPoint(f2, f3 + f, f4);
			Bodypiece[13] = new ModelLoader.PlaneRenderer(this, "Body.Body", 32, 0, ModelPart.Chest);
			Bodypiece[13].addBackPlane(-1F, 10F, 14F, 2, 2, 0, strech);
			Bodypiece[13].setRotationPoint(f2, f3 + f, f4);

			BodypieceNeck = new ModelLoader.PlaneRenderer[4];
			BodypieceNeck[0] = new ModelLoader.PlaneRenderer(this, "Neck.Neck", 0, 16, ModelPart.Head);
			BodypieceNeck[0].addBackPlane(-2F, 1.2F, -2.8F, 4, 4, 0, strech);
			BodypieceNeck[0].setRotationPoint(f2, f3 + f, f4);
			BodypieceNeck[1] = new ModelLoader.PlaneRenderer(this, "Neck.Neck", 0, 16, ModelPart.Head);
			BodypieceNeck[1].addBackPlane(-2F, 1.2F, 1.2F, 4, 4, 0, strech);
			BodypieceNeck[1].setRotationPoint(f2, f3 + f, f4);
			BodypieceNeck[2] = new ModelLoader.PlaneRenderer(this, "Neck.Neck", 0, 16, ModelPart.Head);
			BodypieceNeck[2].addSidePlane(-2F, 1.2F, -2.8F, 0, 4, 4, strech);
			BodypieceNeck[2].setRotationPoint(f2, f3 + f, f4);
			BodypieceNeck[3] = new ModelLoader.PlaneRenderer(this, "Neck.Neck", 0, 16, ModelPart.Head);
			BodypieceNeck[3].addSidePlane(2F, 1.2F, -2.8F, 0, 4, 4, strech);
			BodypieceNeck[3].setRotationPoint(f2, f3 + f, f4);

			BodypieceNeck[0].rotateAngleX = NeckRotX;
			BodypieceNeck[1].rotateAngleX = NeckRotX;
			BodypieceNeck[2].rotateAngleX = NeckRotX;
			BodypieceNeck[3].rotateAngleX = NeckRotX;

			rightarm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.RightArm);
			rightarm.addBox("Leg", -2F, 4F, -2F, 4, 12, 4, strech);
			rightarm.setRotationPoint(-3F, 8F + f, 0.0F);
			LeftArm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.LeftArm);
			LeftArm.mirror = true;
			LeftArm.addBox("Leg", -2F, 4F, -2F, 4, 12, 4, strech);
			LeftArm.setRotationPoint(3F, 8F + f, 0.0F);
			RightLeg = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.RightLeg);
			RightLeg.addBox("Leg", -2F, 4F, -2F, 4, 12, 4, strech);
			RightLeg.setRotationPoint(-3F, 0.0F + f, 0.0F);
			LeftLeg = new ModelLoader.ModelRenderer(this, 0, 16, ModelPart.LeftLeg);
			LeftLeg.mirror = true;
			LeftLeg.addBox("Leg", -2F, 4F, -2F, 4, 12, 4, strech);
			LeftLeg.setRotationPoint(3F, 0.0F + f, 0.0F);
			SteveArm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.None);
			SteveArm.addBox("", -3F, -2F, -2F, 4, 12, 4, strech);
			SteveArm.setRotationPoint(-5F, 2.0F + f, 0.0F);
			boxList.Remove(SteveArm);
			unicornarm = new ModelLoader.ModelRenderer(this, 40, 16, ModelPart.None);
			unicornarm.addBox("", -3F, -2F, -2F, 4, 12, 4, strech);
			unicornarm.setRotationPoint(-5F, 2.0F + f, 0.0F);
			boxList.Remove(unicornarm);
			float f8 = 0.0F;
			float f9 = 8F;
			float f10 = -14F;
			float f11 = 0.0F - f8;
			float f12 = 8.8F - f9;
			float f13 = 0.0F;

			Tail = new ModelLoader.PlaneRenderer[21];
			Tail[0] = new ModelLoader.PlaneRenderer(this, "Tail.1/4.Tail", 32, 0, ModelPart.None);
			Tail[0].addTopPlane(-2F + f8, -7F + f9, 16F + f10, 4, 0, 4, strech);
			Tail[0].setRotationPoint(f11, f12 + f, f13);
			Tail[1] = new ModelLoader.PlaneRenderer(this, "Tail.1/4.Tail", 36, 0, ModelPart.None);
			Tail[1].addSidePlane(-2F + f8, -7F + f9, 16F + f10, 0, 4, 4, strech);
			Tail[1].setRotationPoint(f11, f12 + f, f13);
			Tail[2] = new ModelLoader.PlaneRenderer(this, "Tail.1/4.Tail", 32, 0, ModelPart.None);
			Tail[2].addBackPlane(-2F + f8, -7F + f9, 16F + f10, 4, 4, 0, strech);
			Tail[2].setRotationPoint(f11, f12 + f, f13);
			Tail[3] = new ModelLoader.PlaneRenderer(this, "Tail.1/4.Tail", 36, 0, ModelPart.None);
			Tail[3].addSidePlane(2F + f8, -7F + f9, 16F + f10, 0, 4, 4, strech);
			Tail[3].setRotationPoint(f11, f12 + f, f13);
			Tail[4] = new ModelLoader.PlaneRenderer(this, "Tail.1/4.Tail", 32, 0, ModelPart.None);
			Tail[4].addBackPlane(-2F + f8, -7F + f9, 20F + f10, 4, 4, 0, strech);
			Tail[4].setRotationPoint(f11, f12 + f, f13);
			Tail[5] = new ModelLoader.PlaneRenderer(this, "Tail.1/4.Tail", 32, 0, ModelPart.None);
			Tail[5].addTopPlane(-2F + f8, -3F + f9, 16F + f10, 4, 0, 4, strech);
			Tail[5].setRotationPoint(f11, f12 + f, f13);

			Tail[6] = new ModelLoader.PlaneRenderer(this, "Tail.2/4.Tail", 36, 4, ModelPart.None);
			Tail[6].addSidePlane(-2F + f8, -3F + f9, 16F + f10, 0, 4, 4, strech);
			Tail[6].setRotationPoint(f11, f12 + f, f13);
			Tail[7] = new ModelLoader.PlaneRenderer(this, "Tail.2/4.Tail", 32, 4, ModelPart.None);
			Tail[7].addBackPlane(-2F + f8, -3F + f9, 16F + f10, 4, 4, 0, strech);
			Tail[7].setRotationPoint(f11, f12 + f, f13);
			Tail[8] = new ModelLoader.PlaneRenderer(this, "Tail.2/4.Tail", 36, 4, ModelPart.None);
			Tail[8].addSidePlane(2F + f8, -3F + f9, 16F + f10, 0, 4, 4, strech);
			Tail[8].setRotationPoint(f11, f12 + f, f13);
			Tail[9] = new ModelLoader.PlaneRenderer(this, "Tail.2/4.Tail", 32, 4, ModelPart.None);
			Tail[9].addBackPlane(-2F + f8, -3F + f9, 20F + f10, 4, 4, 0, strech);
			Tail[9].setRotationPoint(f11, f12 + f, f13);
			Tail[10] = new ModelLoader.PlaneRenderer(this, "Tail.2/4.Tail", 32, 0, ModelPart.None);
			Tail[10].addTopPlane(-2F + f8, 1F + f9, 16F + f10, 4, 0, 4, strech);
			Tail[10].setRotationPoint(f11, f12 + f, f13);

			Tail[11] = new ModelLoader.PlaneRenderer(this, "Tail.3/4.Tail", 36, 0, ModelPart.None);
			Tail[11].addSidePlane(-2F + f8, 1F + f9, 16F + f10, 0, 4, 4, strech);
			Tail[11].setRotationPoint(f11, f12 + f, f13);
			Tail[12] = new ModelLoader.PlaneRenderer(this, "Tail.3/4.Tail", 32, 0, ModelPart.None);
			Tail[12].addBackPlane(-2F + f8, 1F + f9, 16F + f10, 4, 4, 0, strech);
			Tail[12].setRotationPoint(f11, f12 + f, f13);
			Tail[13] = new ModelLoader.PlaneRenderer(this, "Tail.3/4.Tail", 36, 0, ModelPart.None);
			Tail[13].addSidePlane(2F + f8, 1F + f9, 16F + f10, 0, 4, 4, strech);
			Tail[13].setRotationPoint(f11, f12 + f, f13);
			Tail[14] = new ModelLoader.PlaneRenderer(this, "Tail.3/4.Tail", 32, 0, ModelPart.None);
			Tail[14].addBackPlane(-2F + f8, 1F + f9, 20F + f10, 4, 4, 0, strech);
			Tail[14].setRotationPoint(f11, f12 + f, f13);
			Tail[15] = new ModelLoader.PlaneRenderer(this, "Tail.3/4.Tail", 32, 0, ModelPart.None);
			Tail[15].addTopPlane(-2F + f8, 5F + f9, 16F + f10, 4, 0, 4, strech);
			Tail[15].setRotationPoint(f11, f12 + f, f13);

			Tail[16] = new ModelLoader.PlaneRenderer(this, "Tail.4/4.Tail", 36, 4, ModelPart.None);
			Tail[16].addSidePlane(-2F + f8, 5F + f9, 16F + f10, 0, 4, 4, strech);
			Tail[16].setRotationPoint(f11, f12 + f, f13);
			Tail[17] = new ModelLoader.PlaneRenderer(this, "Tail.4/4.Tail", 32, 4, ModelPart.None);
			Tail[17].addBackPlane(-2F + f8, 5F + f9, 16F + f10, 4, 4, 0, strech);
			Tail[17].setRotationPoint(f11, f12 + f, f13);
			Tail[18] = new ModelLoader.PlaneRenderer(this, "Tail.4/4.Tail", 36, 4, ModelPart.None);
			Tail[18].addSidePlane(2F + f8, 5F + f9, 16F + f10, 0, 4, 4, strech);
			Tail[18].setRotationPoint(f11, f12 + f, f13);
			Tail[19] = new ModelLoader.PlaneRenderer(this, "Tail.4/4.Tail", 32, 4, ModelPart.None);
			Tail[19].addBackPlane(-2F + f8, 5F + f9, 20F + f10, 4, 4, 0, strech);
			Tail[19].setRotationPoint(f11, f12 + f, f13);
			Tail[20] = new ModelLoader.PlaneRenderer(this, "Tail.4/4.Tail", 32, 0, ModelPart.None);
			Tail[20].addTopPlane(-2F + f8, 9F + f9, 16F + f10, 4, 0, 4, strech);
			Tail[20].setRotationPoint(f11, f12 + f, f13);

			TailRotateAngleY = Tail[0].rotateAngleY;

			float f14 = 0.0F;
			float f15 = 0.0F;
			float f16 = 0.0F;
			LeftWing = new ModelLoader.ModelRenderer[3];
			LeftWing[0] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			LeftWing[0].mirror = true;
			LeftWing[0].addBox("Wings.@Folded.Left Wing.Left Wing", 4F, 5F, 2.0F, 2, 6, 2, strech);
			LeftWing[0].setRotationPoint(f14, f15 + f, f16);
			LeftWing[1] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			LeftWing[1].mirror = true;
			LeftWing[1].addBox("Wings.@Folded.Left Wing.Left Wing", 4F, 5F, 4F, 2, 8, 2, strech);
			LeftWing[1].setRotationPoint(f14, f15 + f, f16);
			LeftWing[2] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			LeftWing[2].mirror = true;
			LeftWing[2].addBox("Wings.@Folded.Left Wing.Left Wing", 4F, 5F, 6F, 2, 6, 2, strech);
			LeftWing[2].setRotationPoint(f14, f15 + f, f16);
			RightWing = new ModelLoader.ModelRenderer[3];
			RightWing[0] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			RightWing[0].addBox("Wings.@Folded.Right Wing.Right Wing", -6F, 5F, 2.0F, 2, 6, 2, strech);
			RightWing[0].setRotationPoint(f14, f15 + f, f16);
			RightWing[1] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			RightWing[1].addBox("Wings.@Folded.Right Wing.Right Wing", -6F, 5F, 4F, 2, 8, 2, strech);
			RightWing[1].setRotationPoint(f14, f15 + f, f16);
			RightWing[2] = new ModelLoader.ModelRenderer(this, 56, 16, ModelPart.None);
			RightWing[2].addBox("Wings.@Folded.Right Wing.Right Wing", -6F, 5F, 6F, 2, 6, 2, strech);
			RightWing[2].setRotationPoint(f14, f15 + f, f16);
			float f17 = f2 + 4.5F;
			float f18 = f3 + 5F;
			float f19 = f4 + 6F;
			LeftWingExt = new ModelLoader.ModelRenderer[7];
			LeftWingExt[0] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[0].mirror = true;
			LeftWingExt[0].addBox("Wings.@Extended.Left Wing Extended.Left Wing Extended", 0.0F, 0.0F, 0.0F, 1, 8, 2,
			                      strech + 0.1F);
			LeftWingExt[0].setRotationPoint(f17, f18 + f, f19);
			LeftWingExt[1] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[1].mirror = true;
			LeftWingExt[1].addBox("Wings.@Extended.Left Wing Extended.Left Wing Extended", 0.0F, 8F, 0.0F, 1, 6, 2, strech + 0.1F);
			LeftWingExt[1].setRotationPoint(f17, f18 + f, f19);
			LeftWingExt[2] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[2].mirror = true;
			LeftWingExt[2].addBox("Wings.@Extended.Left Wing Extended.Left Wing Extended", 0.0F, -1.2F, -0.2F, 1, 8, 2,
			                      strech - 0.2F);
			LeftWingExt[2].setRotationPoint(f17, f18 + f, f19);
			LeftWingExt[3] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[3].mirror = true;
			LeftWingExt[3].addBox("Wings.@Extended.Left Wing Extended.Left Wing Extended", 0.0F, 1.8F, 1.3F, 1, 8, 2,
			                      strech - 0.1F);
			LeftWingExt[3].setRotationPoint(f17, f18 + f, f19);
			LeftWingExt[4] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[4].mirror = true;
			LeftWingExt[4].addBox("Wings.@Extended.Left Wing Extended.Left Wing Extended", 0.0F, 5F, 2.0F, 1, 8, 2, strech);
			LeftWingExt[4].setRotationPoint(f17, f18 + f, f19);
			LeftWingExt[5] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[5].mirror = true;
			LeftWingExt[5].addBox("Wings.@Extended.Left Wing Extended.Left Wing Extended", 0.0F, 0.0F, -0.2F, 1, 6, 2,
			                      strech + 0.3F);
			LeftWingExt[5].setRotationPoint(f17, f18 + f, f19);
			LeftWingExt[6] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			LeftWingExt[6].mirror = true;
			LeftWingExt[6].addBox("Wings.@Extended.Left Wing Extended.Left Wing Extended", 0.0F, 0.0F, 0.2F, 1, 3, 2,
			                      strech + 0.2F);
			LeftWingExt[6].setRotationPoint(f17, f18 + f, f19);
			float f20 = f2 - 4.5F;
			float f21 = f3 + 5F;
			float f22 = f4 + 6F;
			RightWingExt = new ModelLoader.ModelRenderer[7];
			RightWingExt[0] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[0].mirror = true;
			RightWingExt[0].addBox("Wings.@Extended.Right Wing Extended.Right Wing Extended", 0.0F - 1, 0.0F, 0.0F, 1, 8, 2,
			                       strech + 0.1F);
			RightWingExt[0].setRotationPoint(f20, f21 + f, f22);
			RightWingExt[1] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[1].mirror = true;
			RightWingExt[1].addBox("Wings.@Extended.Right Wing Extended.Right Wing Extended", 0.0F - 1, 8F, 0.0F, 1, 6, 2,
			                       strech + 0.1F);
			RightWingExt[1].setRotationPoint(f20, f21 + f, f22);
			RightWingExt[2] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[2].mirror = true;
			RightWingExt[2].addBox("Wings.@Extended.Right Wing Extended.Right Wing Extended", 0.0F - 1, -1.2F, -0.2F, 1, 8, 2,
			                       strech - 0.2F);
			RightWingExt[2].setRotationPoint(f20, f21 + f, f22);
			RightWingExt[3] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[3].mirror = true;
			RightWingExt[3].addBox("Wings.@Extended.Right Wing Extended.Right Wing Extended", 0.0F - 1, 1.8F, 1.3F, 1, 8, 2,
			                       strech - 0.1F);
			RightWingExt[3].setRotationPoint(f20, f21 + f, f22);
			RightWingExt[4] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[4].mirror = true;
			RightWingExt[4].addBox("Wings.@Extended.Right Wing Extended.Right Wing Extended", 0.0F - 1, 5F, 2.0F, 1, 8, 2, strech);
			RightWingExt[4].setRotationPoint(f20, f21 + f, f22);
			RightWingExt[5] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[5].mirror = true;
			RightWingExt[5].addBox("Wings.@Extended.Right Wing Extended.Right Wing Extended", 0.0F - 1, 0.0F, -0.2F, 1, 6, 2,
			                       strech + 0.3F);
			RightWingExt[5].setRotationPoint(f20, f21 + f, f22);
			RightWingExt[6] = new ModelLoader.ModelRenderer(this, 56, 19, ModelPart.None);
			RightWingExt[6].mirror = true;
			RightWingExt[6].addBox("Wings.@Extended.Right Wing Extended.Right Wing Extended", 0.0F - 1, 0.0F, 0.2F, 1, 3, 2,
			                       strech + 0.2F);
			RightWingExt[6].setRotationPoint(f20, f21 + f, f22);
			WingRotateAngleX = LeftWingExt[0].rotateAngleX;
			WingRotateAngleY = LeftWingExt[0].rotateAngleY;
			WingRotateAngleZ = LeftWingExt[0].rotateAngleZ;

			animate();

			return this;
		}

		public void animate()
		{
			bool isSleeping = false;
			bool isMale = true;
			bool isFlying = false;
			bool isPegasus = true;
			bool isUnicorn = true;
			bool issneak = false;
			int wantTail = 0;
			int heldItemRight = 0;
			bool aimedBow = false;

			float f = 0;
			float f1 = 0;
			float f2 = 0;
			float f3 = 0;
			float f4 = 0;
			float f5 = onGround;
			rainboom = false;
			float f6;
			float f7;
			if (isSleeping)
			{
				f6 = 1.4F;
				f7 = 0.1F;
			}
			else
			{
				f6 = f3 / 57.29578F;
				f7 = f4 / 57.29578F;
			}

			if (f7 > .5F)
				f7 = .5F;

			if (f7 < -.5F)
				f7 = -.5F;

			head.rotateAngleY = f6;
			head.rotateAngleX = f7;

			if (isMale)
			{
				for (int i = 0; i < MuzzleMale.Length; i++)
				{
					MuzzleMale[i].rotateAngleY = f6;
					MuzzleMale[i].rotateAngleX = f7;
				}
			}
			else
			{
				for (int i = 0; i < MuzzleFemale.Length; i++)
				{
					MuzzleFemale[i].rotateAngleY = f6;
					MuzzleFemale[i].rotateAngleX = f7;
				}
			}

			headpiece[0].rotateAngleY = f6;
			headpiece[0].rotateAngleX = f7;
			headpiece[1].rotateAngleY = f6;
			headpiece[1].rotateAngleX = f7;
			headpiece[2].rotateAngleY = f6;
			headpiece[2].rotateAngleX = f7;
			helmet.rotateAngleY = f6;
			helmet.rotateAngleX = f7;
			headpiece[2].rotateAngleX = f7 + 0.5F;
			float f8;
			float f9;
			float f10;
			float f11;
			if (!isFlying || !isPegasus)
			{
				f8 = (float) Math.Cos(f * 0.6662F + 3.141593F) * 0.6F * f1;
				f9 = (float) Math.Cos(f * 0.6662F) * 0.6F * f1;
				f10 = (float) Math.Cos(f * 0.6662F) * 0.3F * f1;
				f11 = (float) Math.Cos(f * 0.6662F + 3.141593F) * 0.3F * f1;
				rightarm.rotateAngleY = 0.0F;
				SteveArm.rotateAngleY = 0.0F;
				unicornarm.rotateAngleY = 0.0F;
				LeftArm.rotateAngleY = 0.0F;
				RightLeg.rotateAngleY = 0.0F;
				LeftLeg.rotateAngleY = 0.0F;
			}
			else
			{
				if (f1 < 0.9999F)
				{
					rainboom = false;
					f8 = (float) Math.Sin(0.0F - f1 * 0.5F);
					f9 = (float) Math.Sin(0.0F - f1 * 0.5F);
					f10 = (float) Math.Sin(f1 * 0.5F);
					f11 = (float) Math.Sin(f1 * 0.5F);
				}
				else
				{
					rainboom = true;
					f8 = 4.712F;
					f9 = 4.712F;
					f10 = 1.571F;
					f11 = 1.571F;
				}
				rightarm.rotateAngleY = 0.2F;
				SteveArm.rotateAngleY = 0.2F;
				LeftArm.rotateAngleY = -0.2F;
				RightLeg.rotateAngleY = -0.2F;
				LeftLeg.rotateAngleY = 0.2F;
			}
			if (isSleeping)
			{
				f8 = 4.712F;
				f9 = 4.712F;
				f10 = 1.571F;
				f11 = 1.571F;
			}
			rightarm.rotateAngleX = f8;
			SteveArm.rotateAngleX = f8;
			unicornarm.rotateAngleX = 0.0F;
			LeftArm.rotateAngleX = f9;
			RightLeg.rotateAngleX = f10;
			LeftLeg.rotateAngleX = f11;
			rightarm.rotateAngleZ = 0.0F;
			SteveArm.rotateAngleZ = 0.0F;
			unicornarm.rotateAngleZ = 0.0F;
			LeftArm.rotateAngleZ = 0.0F;

			tailstop = 0;
			tailstop = Tail.Length - (wantTail * 5);
			if (tailstop <= 1) tailstop = 0;
			for (int j = 0; j < tailstop; j++)
			{
				if (rainboom)
					Tail[j].rotateAngleZ = 0.0F;
				else
					Tail[j].rotateAngleZ = (float) Math.Cos(f * 0.8F) * 0.2F * f1;
			}

			if (heldItemRight != 0 && !rainboom && !isUnicorn)
			{
				rightarm.rotateAngleX = rightarm.rotateAngleX * 0.5F - 0.3141593F;
				SteveArm.rotateAngleX = SteveArm.rotateAngleX * 0.5F - 0.3141593F;
			}
			float f12 = 0.0F;
			if (f5 > -9990F && !isUnicorn)
				f12 = (float) Math.Sin(Math.Sqrt(f5) * 3.141593F * 2.0F) * 0.2F;
			Body.rotateAngleY = (float) (f12 * 0.20000000000000001D);
			for (int k = 0; k < Bodypiece.Length; k++)
				Bodypiece[k].rotateAngleY = (float) (f12 * 0.20000000000000001D);
			for (int kk = 0; kk < BodypieceNeck.Length; kk++)
				BodypieceNeck[kk].rotateAngleY = (float) (f12 * 0.20000000000000001D);

			for (int l = 0; l < LeftWing.Length; l++)
				LeftWing[l].rotateAngleY = (float) (f12 * 0.20000000000000001D);

			for (int i1 = 0; i1 < RightWing.Length; i1++)
				RightWing[i1].rotateAngleY = (float) (f12 * 0.20000000000000001D);

			tailstop = 0;
			tailstop = Tail.Length - (wantTail * 5);
			if (tailstop <= 1) tailstop = 0;
			for (int j1 = 0; j1 < tailstop; j1++)
				Tail[j1].rotateAngleY = f12;

			float f13 = (float) Math.Sin(Body.rotateAngleY) * 5F;
			float f14 = (float) Math.Cos(Body.rotateAngleY) * 5F;
			float f15 = 4F;
			if (issneak && !isFlying)
				f15 = 0.0F;
			if (isSleeping)
				f15 = 2.6F;
			if (rainboom)
			{
				rightarm.rotationPointZ = f13 + 2.0F;
				SteveArm.rotationPointZ = f13 + 2.0F;
				LeftArm.rotationPointZ = (0.0F - f13) + 2.0F;
			}
			else
			{
				rightarm.rotationPointZ = f13 + 1.0F;
				SteveArm.rotationPointZ = f13 + 1.0F;
				LeftArm.rotationPointZ = (0.0F - f13) + 1.0F;
			}
			rightarm.rotationPointX = (0.0F - f14 - 1.0F) + f15;
			SteveArm.rotationPointX = 0.0F - f14;
			LeftArm.rotationPointX = (f14 + 1.0F) - f15;
			RightLeg.rotationPointX = (0.0F - f14 - 1.0F) + f15;
			LeftLeg.rotationPointX = (f14 + 1.0F) - f15;
			rightarm.rotateAngleY += Body.rotateAngleY;
			LeftArm.rotateAngleY += Body.rotateAngleY;
			LeftArm.rotateAngleX += Body.rotateAngleY;
			rightarm.rotationPointY = 8F;
			LeftArm.rotationPointY = 8F;
			RightLeg.rotationPointY = 4F;
			LeftLeg.rotationPointY = 4F;
			if (f5 > -9990F)
			{
				float f16 = f5;
				f16 = 1.0F - f5;
				f16 *= f16 * f16;
				f16 = 1.0F - f16;
				var f22 = (float) Math.Sin(f16 * 3.141593F);
				var f28 = (float) Math.Sin(f5 * 3.141593F);
				float f33 = f28 * -(head.rotateAngleX - 0.7F) * 0.75F;
				if (isUnicorn)
				{
					unicornarm.rotateAngleX -= (float) (f22 * 1.2D + f33);
					unicornarm.rotateAngleY += Body.rotateAngleY * 2.0F;
					unicornarm.rotateAngleZ = f28 * -0.4F;
				}
				else
				{
					rightarm.rotateAngleX -= (float) ((double) f22 * 1.2D + (double) f33);
					rightarm.rotateAngleY += Body.rotateAngleY * 2.0F;
					rightarm.rotateAngleZ = f28 * -0.4F;
					SteveArm.rotateAngleX -= (float) ((double) f22 * 1.2D + (double) f33);
					SteveArm.rotateAngleY += Body.rotateAngleY * 2.0F;
					SteveArm.rotateAngleZ = f28 * -0.4F;
				}
			}
			if (issneak && !isFlying)
			{
				float f17 = 0.4F;
				float f23 = 7F;
				float f29 = -4F;
				Body.rotateAngleX = f17;
				Body.rotationPointY = f23;
				Body.rotationPointZ = f29;
				for (int k3 = 0; k3 < Bodypiece.Length; k3++)
				{
					Bodypiece[k3].rotateAngleX = f17;
					Bodypiece[k3].rotationPointY = f23;
					Bodypiece[k3].rotationPointZ = f29;
				}

				for (int k3 = 0; k3 < BodypieceNeck.Length; k3++)
				{
					BodypieceNeck[k3].rotateAngleX = NeckRotX + f17;
					BodypieceNeck[k3].rotationPointY = f23;
					BodypieceNeck[k3].rotationPointZ = f29;
				}

				float f34 = 3.5F;
				float f37 = 6F;
				for (int k4 = 0; k4 < LeftWingExt.Length; k4++)
				{
					LeftWingExt[k4].rotateAngleX = (float) ((double) f17 + 2.3561947345733643D);
					LeftWingExt[k4].rotationPointY = f23 + f34;
					LeftWingExt[k4].rotationPointZ = f29 + f37;
					LeftWingExt[k4].rotateAngleX = 2.5F;
					LeftWingExt[k4].rotateAngleZ = -6F;
				}

				float f40 = 4.5F;
				float f43 = 6F;
				for (int k5 = 0; k5 < LeftWingExt.Length; k5++)
				{
					RightWingExt[k5].rotateAngleX = (float) ((double) f17 + 2.3561947345733643D);
					RightWingExt[k5].rotationPointY = f23 + f40;
					RightWingExt[k5].rotationPointZ = f29 + f43;
					RightWingExt[k5].rotateAngleX = 2.5F;
					RightWingExt[k5].rotateAngleZ = 6F;
				}

				RightLeg.rotateAngleX -= 0.0F;
				LeftLeg.rotateAngleX -= 0.0F;
				rightarm.rotateAngleX -= 0.4F;
				SteveArm.rotateAngleX += 0.4F;
				unicornarm.rotateAngleX += 0.4F;
				LeftArm.rotateAngleX -= 0.4F;
				RightLeg.rotationPointZ = 10F;
				LeftLeg.rotationPointZ = 10F;
				RightLeg.rotationPointY = 7F;
				LeftLeg.rotationPointY = 7F;
				float f46;
				float f48;
				float f50;
				if (isSleeping)
				{
					f46 = 2.0F;
					f48 = -1F;
					f50 = 1.0F;
				}
				else
				{
					f46 = 6F;
					f48 = -2F;
					f50 = 0.0F;
				}
				head.rotationPointY = f46;
				head.rotationPointZ = f48;
				head.rotationPointX = f50;
				helmet.rotationPointY = f46;
				helmet.rotationPointZ = f48;
				helmet.rotationPointX = f50;
				headpiece[0].rotationPointY = f46;
				headpiece[0].rotationPointZ = f48;
				headpiece[0].rotationPointX = f50;
				headpiece[1].rotationPointY = f46;
				headpiece[1].rotationPointZ = f48;
				headpiece[1].rotationPointX = f50;
				headpiece[2].rotationPointY = f46;
				headpiece[2].rotationPointZ = f48;
				headpiece[2].rotationPointX = f50;

				if (isMale)
				{
					for (int j6 = 0; j6 < MuzzleMale.Length; j6++)
					{
						MuzzleMale[j6].rotationPointY = f46;
						MuzzleMale[j6].rotationPointX = f50;
						MuzzleMale[j6].rotationPointZ = f48;
					}
				}
				else
				{
					for (int j6 = 0; j6 < MuzzleFemale.Length; j6++)
					{
						MuzzleFemale[j6].rotationPointY = f46;
						MuzzleFemale[j6].rotationPointX = f50;
						MuzzleFemale[j6].rotationPointZ = f48;
					}
				}

				float f52 = 0.0F;
				float f54 = 8F;
				float f56 = -14F;
				float f58 = 0.0F - f52;
				float f60 = 8.8F - f54;
				float f62 = -4F - f56;
				float f63 = 0.0F;
				tailstop = 0;
				tailstop = Tail.Length - (wantTail * 5);
				if (tailstop <= 1) tailstop = 0;
				for (int i7 = 0; i7 < tailstop; i7++)
				{
					Tail[i7].rotationPointX = f58;
					Tail[i7].rotationPointY = f60;
					Tail[i7].rotationPointZ = f62;
					Tail[i7].rotateAngleX = f63;
				}
			}
			else
			{
				float f18 = 0.0F;
				float f24 = 0.0F;
				float f30 = 0.0F;
				Body.rotateAngleX = f18;
				Body.rotationPointY = f24;
				Body.rotationPointZ = f30;
				for (int l3 = 0; l3 < Bodypiece.Length; l3++)
				{
					Bodypiece[l3].rotateAngleX = f18;
					Bodypiece[l3].rotationPointY = f24;
					Bodypiece[l3].rotationPointZ = f30;
				}
				for (int l3 = 0; l3 < BodypieceNeck.Length; l3++)
				{
					BodypieceNeck[l3].rotateAngleX = NeckRotX + f18;
					BodypieceNeck[l3].rotationPointY = f24;
					BodypieceNeck[l3].rotationPointZ = f30;
				}


				if (isPegasus)
				{
					if (!isFlying)
					{
						for (int i4 = 0; i4 < LeftWing.Length; i4++)
						{
							LeftWing[i4].rotateAngleX = (float) (f18 + 1.5707964897155762D);
							LeftWing[i4].rotationPointY = f24 + 13F;
							LeftWing[i4].rotationPointZ = f30 - 3F;
						}

						for (int j4 = 0; j4 < RightWing.Length; j4++)
						{
							RightWing[j4].rotateAngleX = (float) (f18 + 1.5707964897155762D);
							RightWing[j4].rotationPointY = f24 + 13F;
							RightWing[j4].rotationPointZ = f30 - 3F;
						}
					}
					else
					{
						float f35 = 5.5F;
						float f38 = 3F;
						for (int l4 = 0; l4 < LeftWingExt.Length; l4++)
						{
							LeftWingExt[l4].rotateAngleX = (float) ((double) f18 + 1.5707964897155762D);
							LeftWingExt[l4].rotationPointY = f24 + f35;
							LeftWingExt[l4].rotationPointZ = f30 + f38;
						}

						float f41 = 6.5F;
						float f44 = 3F;
						for (int l5 = 0; l5 < RightWingExt.Length; l5++)
						{
							RightWingExt[l5].rotateAngleX = (float) ((double) f18 + 1.5707964897155762D);
							RightWingExt[l5].rotationPointY = f24 + f41;
							RightWingExt[l5].rotationPointZ = f30 + f44;
						}
					}
				}
				RightLeg.rotationPointZ = 10F;
				LeftLeg.rotationPointZ = 10F;
				RightLeg.rotationPointY = 8F;
				LeftLeg.rotationPointY = 8F;
				float f36 = (float) Math.Cos(f2 * 0.09F) * 0.05F + 0.05F;
				float f39 = (float) Math.Sin(f2 * 0.067F) * 0.05F;
				SteveArm.rotateAngleZ += f36;
				unicornarm.rotateAngleZ += f36;
				SteveArm.rotateAngleX += f39;
				unicornarm.rotateAngleX += f39;
				if (isPegasus && isFlying)
				{
					WingRotateAngleY = (float) Math.Sin(f2 * 0.067F * 8F) * 1.0F;
					WingRotateAngleZ = (float) Math.Sin(f2 * 0.067F * 8F) * 1.0F;
					for (int i5 = 0; i5 < LeftWingExt.Length; i5++)
					{
						LeftWingExt[i5].rotateAngleX = 2.5F;
						LeftWingExt[i5].rotateAngleZ = -WingRotateAngleZ - 4.712F - 0.4F;
					}

					for (int j5 = 0; j5 < RightWingExt.Length; j5++)
					{
						RightWingExt[j5].rotateAngleX = 2.5F;
						RightWingExt[j5].rotateAngleZ = WingRotateAngleZ + 4.712F + 0.4F;
					}
				}
				float f42;
				float f45;
				float f47;
				if (isSleeping)
				{
					f42 = 2.0F;
					f45 = 1.0F;
					f47 = 1.0F;
				}
				else
				{
					f42 = 0.0F;
					f45 = 0.0F;
					f47 = 0.0F;
				}
				head.rotationPointY = f42;
				head.rotationPointZ = f45;
				head.rotationPointX = f47;
				helmet.rotationPointY = f42;
				helmet.rotationPointZ = f45;
				helmet.rotationPointX = f47;
				headpiece[0].rotationPointY = f42;
				headpiece[0].rotationPointZ = f45;
				headpiece[0].rotationPointX = f47;
				headpiece[1].rotationPointY = f42;
				headpiece[1].rotationPointZ = f45;
				headpiece[1].rotationPointX = f47;
				headpiece[2].rotationPointY = f42;
				headpiece[2].rotationPointZ = f45;
				headpiece[2].rotationPointX = f47;

				if (isMale)
				{
					for (int i6 = 0; i6 < MuzzleMale.Length; i6++)
					{
						MuzzleMale[i6].rotationPointY = f42;
						MuzzleMale[i6].rotationPointX = f45;
						MuzzleMale[i6].rotationPointZ = f47;
					}
				}
				else
				{
					for (int i6 = 0; i6 < MuzzleFemale.Length; i6++)
					{
						MuzzleFemale[i6].rotationPointY = f42;
						MuzzleFemale[i6].rotationPointX = f45;
						MuzzleFemale[i6].rotationPointZ = f47;
					}
				}

				float f49 = 0.0F;
				float f51 = 8F;
				float f53 = -14F;
				float f55 = 0.0F - f49;
				float f57 = 8.8F - f51;
				float f59 = 0.0F - f53;
				float f61 = 0.5F * f1;
				tailstop = 0;
				tailstop = Tail.Length - (wantTail * 5);
				if (tailstop <= 1) tailstop = 0;
				for (int k6 = 0; k6 < tailstop; k6++)
				{
					Tail[k6].rotationPointX = f55;
					Tail[k6].rotationPointY = f57;
					Tail[k6].rotationPointZ = f59;
					if (rainboom)
						Tail[k6].rotateAngleX = 1.571F + 0.1F * (float) Math.Sin(f);
					else
						Tail[k6].rotateAngleX = f61;
				}

				tailstop = 0;
				tailstop = Tail.Length - (wantTail * 5);
				if (tailstop <= 1) tailstop = 0;
				for (int l6 = 0; l6 < tailstop; l6++)
				{
					if (!rainboom)
						Tail[l6].rotateAngleX += f39;
				}
			}
			LeftWingExt[2].rotateAngleX = LeftWingExt[2].rotateAngleX - 0.85F;
			LeftWingExt[3].rotateAngleX = LeftWingExt[3].rotateAngleX - 0.75F;
			LeftWingExt[4].rotateAngleX = LeftWingExt[4].rotateAngleX - 0.5F;
			LeftWingExt[6].rotateAngleX = LeftWingExt[6].rotateAngleX - 0.85F;
			RightWingExt[2].rotateAngleX = RightWingExt[2].rotateAngleX - 0.85F;
			RightWingExt[3].rotateAngleX = RightWingExt[3].rotateAngleX - 0.75F;
			RightWingExt[4].rotateAngleX = RightWingExt[4].rotateAngleX - 0.5F;
			RightWingExt[6].rotateAngleX = RightWingExt[6].rotateAngleX - 0.85F;
			Bodypiece[9].rotateAngleX = Bodypiece[9].rotateAngleX + 0.5F;
			Bodypiece[10].rotateAngleX = Bodypiece[10].rotateAngleX + 0.5F;
			Bodypiece[11].rotateAngleX = Bodypiece[11].rotateAngleX + 0.5F;
			Bodypiece[12].rotateAngleX = Bodypiece[12].rotateAngleX + 0.5F;
			Bodypiece[13].rotateAngleX = Bodypiece[13].rotateAngleX + 0.5F;

			if (rainboom)
			{
				tailstop = 0;
				tailstop = Tail.Length - (wantTail * 5);
				if (tailstop <= 1) tailstop = 0;
				for (int k1 = 0; k1 < tailstop; k1++)
				{
					Tail[k1].rotationPointY = Tail[k1].rotationPointY + 6F;
					Tail[k1].rotationPointZ = Tail[k1].rotationPointZ + 1.0F;
				}
			}
			if (isRiding)
			{
				float f19 = -10F;
				float f25 = -10F;
				head.rotationPointY = head.rotationPointY + f19;
				head.rotationPointZ = head.rotationPointZ + f25;
				headpiece[0].rotationPointY = headpiece[0].rotationPointY + f19;
				headpiece[0].rotationPointZ = headpiece[0].rotationPointZ + f25;
				headpiece[1].rotationPointY = headpiece[1].rotationPointY + f19;
				headpiece[1].rotationPointZ = headpiece[1].rotationPointZ + f25;
				headpiece[2].rotationPointY = headpiece[2].rotationPointY + f19;
				headpiece[2].rotationPointZ = headpiece[2].rotationPointZ + f25;

				if (isMale)
				{
					for (int l1 = 0; l1 < MuzzleMale.Length; l1++)
					{
						MuzzleMale[l1].rotationPointY = MuzzleMale[l1].rotationPointY + f19;
						MuzzleMale[l1].rotationPointZ = MuzzleMale[l1].rotationPointZ + f25;
					}
				}
				else
				{
					for (int l1 = 0; l1 < MuzzleFemale.Length; l1++)
					{
						MuzzleFemale[l1].rotationPointY = MuzzleFemale[l1].rotationPointY + f19;
						MuzzleFemale[l1].rotationPointZ = MuzzleFemale[l1].rotationPointZ + f25;
					}
				}

				helmet.rotationPointY = helmet.rotationPointY + f19;
				helmet.rotationPointZ = helmet.rotationPointZ + f25;
				Body.rotationPointY = Body.rotationPointY + f19;
				Body.rotationPointZ = Body.rotationPointZ + f25;
				for (int i2 = 0; i2 < Bodypiece.Length; i2++)
				{
					Bodypiece[i2].rotationPointY = Bodypiece[i2].rotationPointY + f19;
					Bodypiece[i2].rotationPointZ = Bodypiece[i2].rotationPointZ + f25;
				}

				LeftArm.rotationPointY = LeftArm.rotationPointY + f19;
				LeftArm.rotationPointZ = LeftArm.rotationPointZ + f25;
				rightarm.rotationPointY = rightarm.rotationPointY + f19;
				rightarm.rotationPointZ = rightarm.rotationPointZ + f25;
				LeftLeg.rotationPointY = LeftLeg.rotationPointY + f19;
				LeftLeg.rotationPointZ = LeftLeg.rotationPointZ + f25;
				RightLeg.rotationPointY = RightLeg.rotationPointY + f19;
				RightLeg.rotationPointZ = RightLeg.rotationPointZ + f25;
				tailstop = 0;
				tailstop = Tail.Length - (wantTail * 5);
				if (tailstop <= 1) tailstop = 0;
				for (int j2 = 0; j2 < tailstop; j2++)
				{
					Tail[j2].rotationPointY = Tail[j2].rotationPointY + f19;
					Tail[j2].rotationPointZ = Tail[j2].rotationPointZ + f25;
				}

				for (int k2 = 0; k2 < LeftWing.Length; k2++)
				{
					LeftWing[k2].rotationPointY = LeftWing[k2].rotationPointY + f19;
					LeftWing[k2].rotationPointZ = LeftWing[k2].rotationPointZ + f25;
				}

				for (int l2 = 0; l2 < RightWing.Length; l2++)
				{
					RightWing[l2].rotationPointY = RightWing[l2].rotationPointY + f19;
					RightWing[l2].rotationPointZ = RightWing[l2].rotationPointZ + f25;
				}

				for (int i3 = 0; i3 < LeftWingExt.Length; i3++)
				{
					LeftWingExt[i3].rotationPointY = LeftWingExt[i3].rotationPointY + f19;
					LeftWingExt[i3].rotationPointZ = LeftWingExt[i3].rotationPointZ + f25;
				}

				for (int j3 = 0; j3 < RightWingExt.Length; j3++)
				{
					RightWingExt[j3].rotationPointY = RightWingExt[j3].rotationPointY + f19;
					RightWingExt[j3].rotationPointZ = RightWingExt[j3].rotationPointZ + f25;
				}
			}
			if (isSleeping)
			{
				rightarm.rotationPointZ = rightarm.rotationPointZ + 6F;
				LeftArm.rotationPointZ = LeftArm.rotationPointZ + 6F;
				RightLeg.rotationPointZ = RightLeg.rotationPointZ - 8F;
				LeftLeg.rotationPointZ = LeftLeg.rotationPointZ - 8F;
				rightarm.rotationPointY = rightarm.rotationPointY + 2.0F;
				LeftArm.rotationPointY = LeftArm.rotationPointY + 2.0F;
				RightLeg.rotationPointY = RightLeg.rotationPointY + 2.0F;
				LeftLeg.rotationPointY = LeftLeg.rotationPointY + 2.0F;
			}
			if (aimedBow)
			{
				if (isUnicorn)
				{
					float f20 = 0.0F;
					float f26 = 0.0F;
					unicornarm.rotateAngleZ = 0.0F;
					unicornarm.rotateAngleY = -(0.1F - f20 * 0.6F) + head.rotateAngleY;
					unicornarm.rotateAngleX = 4.712F + head.rotateAngleX;
					unicornarm.rotateAngleX -= f20 * 1.2F - f26 * 0.4F;
					float f31 = 0;
					unicornarm.rotateAngleZ += (float) Math.Cos(f31 * 0.09F) * 0.05F + 0.05F;
					unicornarm.rotateAngleX += (float) Math.Sin(f31 * 0.067F) * 0.05F;
				}
				else
				{
					float f21 = 0.0F;
					float f27 = 0.0F;
					rightarm.rotateAngleZ = 0.0F;
					rightarm.rotateAngleY = -(0.1F - f21 * 0.6F) + head.rotateAngleY;
					rightarm.rotateAngleX = 4.712F + head.rotateAngleX;
					rightarm.rotateAngleX -= f21 * 1.2F - f27 * 0.4F;
					float f32 = 0;
					rightarm.rotateAngleZ += (float) Math.Cos(f32 * 0.09F) * 0.05F + 0.05F;
					rightarm.rotateAngleX += (float) Math.Sin(f32 * 0.067F) * 0.05F;
					rightarm.rotationPointZ = rightarm.rotationPointZ + 1.0F;
				}
			}
		}

		/*
		public void render(AniParams aniparams, boolean flag)
		{
				if(flag)
				{
						head.render(scale);
						headpiece[0].render(scale);
						headpiece[1].render(scale);
						if(isUnicorn)
						{
								headpiece[2].render(scale);
						}

				if(isMale)
				{
					for(int i = 0; i < MuzzleMale.length; i++)
					{
						MuzzleMale[i].render(scale);
					}
				} else {
					for(int i = 0; i < MuzzleFemale.length; i++)
					{
						MuzzleFemale[i].render(scale);
					}
				}

						helmet.render(scale);
						Body.render(scale);
						for(int j = 0; j < Bodypiece.length; j++)
						{
								Bodypiece[j].render(scale);
						}

						for(int j = 0; j < BodypieceNeck.length; j++)
						{
								BodypieceNeck[j].render(scale);
						}            
            
						LeftArm.render(scale);
						rightarm.render(scale);
						LeftLeg.render(scale);
						RightLeg.render(scale);
				int tailstop = 0;
				tailstop = Tail.length - (wantTail * 5);
				if(tailstop <= 1) {tailstop = 0;}
						for(int k = 0; k < tailstop; k++)
						{
								Tail[k].render(scale);
						}

						if(isPegasus)
						{
								if(isFlying || issneak)
								{
										for(int l = 0; l < LeftWingExt.length; l++)
										{
												LeftWingExt[l].render(scale);
										}

										for(int i1 = 0; i1 < RightWingExt.length; i1++)
										{
												RightWingExt[i1].render(scale);
										}

								} else
								{
										for(int j1 = 0; j1 < LeftWing.length; j1++)
										{
												LeftWing[j1].render(scale);
										}

										for(int k1 = 0; k1 < RightWing.length; k1++)
										{
												RightWing[k1].render(scale);
										}

								}
						}
				} else
				{
						SteveArm.render(scale);
				}
		}

		public void specials(RenderManager rendermanager, EntityPlayer entityplayer)
		{
				if(!isSleeping)
				{
						if(isUnicorn)
						{
								if(aimedBow)
								{
										renderDrop(rendermanager, entityplayer, unicornarm, 1.0F, 0.15F, 0.9375F, 0.0625F);
								} else
								{
										renderDrop(rendermanager, entityplayer, unicornarm, 1.0F, 0.35F, 0.5375F, -0.45F);
								}
						} else
						{
								renderDrop(rendermanager, entityplayer, rightarm, 1.0F, -0.0625F, 0.8375F, 0.0625F);
						}
				}
				renderPumpkin(rendermanager, entityplayer, head, 0.625F, 0.0F, -0.08F, -0.15F);
		}*/
	}

	public class ModelOzelot : ModelLoader.ModelBase
	{
		private readonly ModelLoader.ModelRenderer a;
		private readonly ModelLoader.ModelRenderer b;
		private readonly ModelLoader.ModelRenderer c;
		private readonly ModelLoader.ModelRenderer d;
		private readonly ModelLoader.ModelRenderer e;
		private readonly ModelLoader.ModelRenderer f;
		private readonly ModelLoader.ModelRenderer g;
		private readonly ModelLoader.ModelRenderer n;
		private int o;

		public ModelOzelot()
		{
			o = 0;
			g = new ModelLoader.ModelRenderer(this, "head", ModelPart.Head);
			g.setTextureOffset(0, 0).addBox("main", -2.5F, -2F, -3F, 5, 4, 5);
			g.setTextureOffset(0, 24).addBox("nose", -1.5F, 0.0F, -4F, 3, 2, 2);
			g.setTextureOffset(0, 10).addBox("ear1", -2F, -3F, 0.0F, 1, 1, 2);
			g.setTextureOffset(6, 10).addBox("ear2", 1.0F, -3F, 0.0F, 1, 1, 2);
			g.setRotationPoint(0.0F, 15F, -9F);
			n = new ModelLoader.ModelRenderer(this, 20, 0, ModelPart.Chest);
			n.addBox("Body", -2F, 3F, -8F, 4, 16, 6, 0.0F);
			n.setRotationPoint(0.0F, 12F, -10F);
			e = new ModelLoader.ModelRenderer(this, 0, 15, ModelPart.Chest);
			e.addBox("Tail", -0.5F, 0.0F, 0.0F, 1, 8, 1);
			e.rotateAngleX = 0.9F;
			e.setRotationPoint(0.0F, 15F, 8F);
			f = new ModelLoader.ModelRenderer(this, 4, 15, ModelPart.Chest);
			f.addBox("Tail", -0.5F, 0.0F, 0.0F, 1, 8, 1);
			f.setRotationPoint(0.0F, 20F, 14F);
			a = new ModelLoader.ModelRenderer(this, 8, 13, ModelPart.LeftArm);
			a.addBox("Leg", -1F, 0.0F, 1.0F, 2, 6, 2);
			a.setRotationPoint(1.1F, 18F, 5F);
			b = new ModelLoader.ModelRenderer(this, 8, 13, ModelPart.RightArm);
			b.addBox("Leg", -1F, 0.0F, 1.0F, 2, 6, 2);
			b.setRotationPoint(-1.1F, 18F, 5F);
			c = new ModelLoader.ModelRenderer(this, 40, 0, ModelPart.LeftLeg);
			c.addBox("Leg", -1F, 0.0F, 0.0F, 2, 10, 2);
			c.setRotationPoint(1.2F, 13.8F, -5F);
			d = new ModelLoader.ModelRenderer(this, 40, 0, ModelPart.RightLeg);
			d.addBox("Leg", -1F, 0.0F, 0.0F, 2, 10, 2);
			d.setRotationPoint(-1.2F, 13.8F, -5F);

			setup2(0, 0, 0, 0, 0, 0);
		}

		public void setup2(float f1, float f2, float f3, float f4, float f5, float f6)
		{
			setup(f1, f2, f3, f4, f5, f6);
			setup3(f1, f2, f3);
		}

		public void setup(float f1, float f2, float f3, float f4, float f5, float f6)
		{
			g.rotateAngleX = f5 / 57.29578F;
			g.rotateAngleY = f4 / 57.29578F;
			if (o != 3)
			{
				n.rotateAngleX = 1.570796F;
				if (o == 2)
				{
					a.rotateAngleX = (f1 * 0.6662F) * 1.0F * f2;
					b.rotateAngleX = (f1 * 0.6662F + 0.3F) * 1.0F * f2;
					c.rotateAngleX = (f1 * 0.6662F + 3.141593F + 0.3F) * 1.0F * f2;
					d.rotateAngleX = (f1 * 0.6662F + 3.141593F) * 1.0F * f2;
					f.rotateAngleX = 1.727876F + 0.3141593F * (f1) * f2;
				}
				else
				{
					a.rotateAngleX = (f1 * 0.6662F) * 1.0F * f2;
					b.rotateAngleX = (f1 * 0.6662F + 3.141593F) * 1.0F * f2;
					c.rotateAngleX = (f1 * 0.6662F + 3.141593F) * 1.0F * f2;
					d.rotateAngleX = (f1 * 0.6662F) * 1.0F * f2;
					if (o == 1)
						f.rotateAngleX = 1.727876F + 0.7853982F * (f1) * f2;
					else
						f.rotateAngleX = 1.727876F + 0.4712389F * (f1) * f2;
				}
			}
		}

		public void setup3(float f1, float f2, float f3)
		{
			if ( /*itemfishingrod.Z()*/true)
			{
				n.rotationPointY = 13F;
				g.rotationPointY = 17F;
				e.rotationPointY = 16F;
				e.rotationPointZ = 8F;
				f.rotationPointY = 16F;
				f.rotationPointZ = 16F;
				e.rotateAngleX = 1.570796F;
				f.rotateAngleX = 1.570796F;
				c.rotationPointY = d.rotationPointY = 13.8F;
				a.rotationPointY = b.rotationPointY = 18F;
				a.rotationPointZ = b.rotationPointZ = 5F;
				o = 0;
			}
			/*else if (false)
			{
				n.rotationPointY = 12F;
				g.rotationPointY = 15F;
				e.rotationPointY = 15F;
				f.rotationPointY = 15F;
				f.rotationPointZ = 16F;
				e.rotateAngleX = 1.570796F;
				f.rotateAngleX = 1.570796F;
				c.rotationPointY = d.rotationPointY = 13.8F;
				a.rotationPointY = b.rotationPointY = 18F;
				a.rotationPointZ = b.rotationPointZ = 5F;
				o = 2;
			}
			else if (false)
			{
				n.rotateAngleX = 0.7853982F;
				n.rotationPointY = 8F;
				n.rotationPointZ = -5F;
				g.rotationPointY = 11.7F;
				g.rotationPointZ = -8F;
				e.rotationPointY = 23F;
				e.rotationPointZ = 6F;
				f.rotationPointY = 22F;
				f.rotationPointZ = 13.2F;
				e.rotateAngleX = 1.727876F;
				f.rotateAngleX = 2.670354F;
				a.rotateAngleX = b.rotateAngleX = -1.570796F;
				a.rotationPointZ = b.rotationPointZ = 4F;
				c.rotateAngleX = d.rotateAngleX = -0.1570796F;
				c.rotationPointY = d.rotationPointY = 15.8F;
				c.rotationPointZ = d.rotationPointZ = -7F;
				a.rotationPointY = b.rotationPointY = 21F;
				a.rotationPointZ = b.rotationPointZ = 1.0F;
				o = 3;
			}
			else
			{
				n.rotationPointY = 12F;
				n.rotationPointZ = -10F;
				g.rotationPointY = 15F;
				g.rotationPointZ = -9F;
				e.rotationPointY = 15F;
				e.rotationPointZ = 8F;
				f.rotationPointY = 20F;
				f.rotationPointZ = 14F;
				c.rotationPointY = d.rotationPointY = 13.8F;
				a.rotationPointY = b.rotationPointY = 18F;
				a.rotationPointZ = b.rotationPointZ = 5F;
				e.rotateAngleX = 0.9F;
				o = 1;
			}*/
		}
	}

	public class ModelGolem : ModelLoader.ModelBase
	{
		public ModelLoader.ModelRenderer a;
		public ModelLoader.ModelRenderer b;
		public ModelLoader.ModelRenderer c;
		public ModelLoader.ModelRenderer d;
		public ModelLoader.ModelRenderer e;
		public ModelLoader.ModelRenderer f;

		public ModelGolem() :
			this(0.0f)
		{
		}

		public ModelGolem(float f1) :
			this(f1, -7F)
		{
		}

		public ModelGolem(float f1, float f2)
		{
			byte byte0 = 96;
			byte byte1 = 96;
			a = (new ModelLoader.ModelRenderer(this, ModelPart.Head)).setTextureSize(byte0, byte1);
			a.setRotationPoint(0.0F, 0.0F + f2, 0.0F);
			a.setTextureOffset(0, 0).addBox("Head", -4F, -12F, -7.5F, 8, 10, 8, f1);
			a.setTextureOffset(24, 0).addBox("Nose", -1F, -5F, -9.5F, 2, 4, 2, f1);
			b = (new ModelLoader.ModelRenderer(this, ModelPart.Chest)).setTextureSize(byte0, byte1);
			b.setRotationPoint(0.0F, 0.0F + f2, 0.0F);
			b.setTextureOffset(0, 40).addBox("Chest", -9F, -2F, -6F, 18, 12, 11, f1);
			b.setTextureOffset(0, 70).addBox("Stomach", -4.5F, 10F, -3F, 9, 5, 6, f1 + 0.5F);
			c = (new ModelLoader.ModelRenderer(this, ModelPart.RightArm)).setTextureSize(byte0, byte1);
			c.setRotationPoint(0.0F, -7F, 0.0F);
			c.setTextureOffset(60, 21).addBox("Right Arm", -13F, -2.5F, -3F, 4, 30, 6, f1);
			d = (new ModelLoader.ModelRenderer(this, ModelPart.LeftArm)).setTextureSize(byte0, byte1);
			d.setRotationPoint(0.0F, -7F, 0.0F);
			d.setTextureOffset(60, 58).addBox("Left Arm", 9F, -2.5F, -3F, 4, 30, 6, f1);
			e = (new ModelLoader.ModelRenderer(this, 0, 22, ModelPart.RightLeg)).setTextureSize(byte0, byte1);
			e.setRotationPoint(-4F, 18F + f2, 0.0F);
			e.setTextureOffset(37, 0).addBox("Right Leg", -3.5F, -3F, -3F, 6, 16, 5, f1);
			f = (new ModelLoader.ModelRenderer(this, 0, 22, ModelPart.LeftLeg)).setTextureSize(byte0, byte1);
			f.mirror = true;
			f.setTextureOffset(60, 0).setRotationPoint(4F, 18F + f2, 0.0F);
			f.addBox("Left Leg", -3.5F, -3F, -3F, 6, 16, 5, f1);

			setup1(0, 0, 0, 0, 0, 0);
			setup2(0, 0, 0);
		}

		public void setup1(float f1, float f2, float f3, float f4, float f5, float f6)
		{
			a.rotateAngleY = f4 / 57.29578F;
			a.rotateAngleX = f5 / 57.29578F;
			e.rotateAngleX = -1.5F * getValue(f1, 13F) * f2;
			f.rotateAngleX = 1.5F * getValue(f1, 13F) * f2;
			e.rotateAngleY = 0.0F;
			f.rotateAngleY = 0.0F;
		}

		public void setup2(float f1, float f2, float f3)
		{
			int i = 0;
			if (i > 0)
			{
				c.rotateAngleX = -2F + 1.5F * getValue(i - f3, 10F);
				d.rotateAngleX = -2F + 1.5F * getValue(i - f3, 10F);
			}
			else
			{
				int j = 0; //tg1.D_();
				if (j > 0)
				{
					c.rotateAngleX = -0.8F + 0.025F * getValue(j, 70F);
					d.rotateAngleX = 0.0F;
				}
				else
				{
					c.rotateAngleX = (-0.2F + 1.5F * getValue(f1, 13F)) * f2;
					d.rotateAngleX = (-0.2F - 1.5F * getValue(f1, 13F)) * f2;
				}
			}
		}

		private float getValue(float f1, float f2)
		{
			return (Math.Abs(f1 % f2 - f2 * 0.5F) - f2 * 0.25F) / (f2 * 0.25F);
		}
	}
}