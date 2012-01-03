//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paril.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.IO;

namespace MCSkin3D
{
	public static class ModelLoader
	{
		// 3 = front-top-left
		// 2 = front-top-right
		// 1 = front-bottom-right
		// 0 = front-bottom-left
		// 7 = back-top-left
		// 6 = back-top-right
		// 5 = back-bottom-right
		// 4 = back-bottom-left
		static Vector3[] CreateBox(float width, float height, float length)
		{
			Vector3[] vertices = new Vector3[8];

			width /= 2;
			height /= 2;
			length /= 2;

			vertices[0] = new Vector3(-width, -height, length);
			vertices[1] = new Vector3(width, -height, length);
			vertices[2] = new Vector3(width, height, length);
			vertices[3] = new Vector3(-width, height, length);

			vertices[4] = new Vector3(-width, -height, -length);
			vertices[5] = new Vector3(width, -height, -length);
			vertices[6] = new Vector3(width, height, -length);
			vertices[7] = new Vector3(-width, height, -length);

			return vertices;
		}

		static Vector3[] CreateBox(float size)
		{
			return CreateBox(size, size, size);
		}

		enum FaceLocation
		{
			Front,
			Back,
			Top,
			Bottom,
			Left,
			Right
		}

		static Vector3[] GetFace(FaceLocation location, Vector3[] box)
		{
			switch (location)
			{
			case FaceLocation.Front:
				return new Vector3[] { box[3], box[2], box[1], box[0] };
			case FaceLocation.Top:
				return new Vector3[] { box[7], box[6], box[2], box[3] };
			case FaceLocation.Bottom:
				return new Vector3[] { box[5], box[4], box[0], box[1] };
			case FaceLocation.Back:
				return new Vector3[] { box[4], box[5], box[6], box[7] };
			case FaceLocation.Left:
				return new Vector3[] { box[7], box[3], box[0], box[4] };
			case FaceLocation.Right:
				return new Vector3[] { box[2], box[6], box[5], box[1] };
			}

			return null;
		}

		static Vector2[] TexCoordBox(int x, int y, int w, int h)
		{
			const float sw = 64.0f;
			const float sh = 32.0f;

			float rx = x / sw;
			float ry = y / sh;
			float rw = w / sw;
			float rh = h / sh;

			return new Vector2[]
			{
				new Vector2(rx, ry),
				new Vector2(rx + rw, ry),
				new Vector2(rx + rw, ry + rh),
				new Vector2(rx, ry + rh),
			};
		}

		static Vector2[] TexCoordBoxPrecise(int x, int y, int w, int h,
			int i1, int i2, int i3, int i4)
		{
			var box = TexCoordBox(x, y, w, h);
			return new Vector2[] { box[i1], box[i2], box[i3], box[i4] };
		}

		static Vector2[] InvertCoords(Vector2[] coords)
		{
			return new Vector2[] { coords[3], coords[2], coords[1], coords[0] };
		}

		public static Dictionary<string, Model> Models = new Dictionary<string, Model>();

		public static void InvertBottomFaces()
		{
			foreach (var m in Models.Values)
				foreach (var mesh in m.Meshes)
					foreach (var face in mesh.Faces)
					{
						if (face.Downface)
						{
							float minY = 1, maxY = 0;

							for (int i = 0; i < 4; ++i)
							{
								if (face.TexCoords[i].Y < minY)
									minY = face.TexCoords[i].Y;
								if (face.TexCoords[i].Y > maxY)
									maxY = face.TexCoords[i].Y;
							}

							for (int i = 0; i < 4; ++i)
							{
								if (face.TexCoords[i].Y == minY)
									face.TexCoords[i].Y = maxY;
								else
									face.TexCoords[i].Y = minY;
							}
						}
					}
		}

		/*
		 * Magic from Java
		 */
		public class PositionTextureVertex
		{
			public Vector3 vector3D;
			public float texturePositionX;
			public float texturePositionY;

			public PositionTextureVertex(float f, float f1, float f2, float f3, float f4) :
				this(new Vector3(f, f1, f2), f3, f4)
			{
			}

			public PositionTextureVertex setTexturePosition(float f, float f1)
			{
				return new PositionTextureVertex(this, f, f1);
			}

			public PositionTextureVertex(PositionTextureVertex positiontexturevertex, float f, float f1)
			{
				vector3D = positiontexturevertex.vector3D;
				texturePositionX = f;
				texturePositionY = f1;
			}

			public PositionTextureVertex(Vector3 vec3d, float f, float f1)
			{
				vector3D = vec3d;
				texturePositionX = f;
				texturePositionY = f1;
			}
		}

		public class TexturedQuad
		{
			public PositionTextureVertex[] vertexPositions;
			public int nVertices;
			private bool invertNormal;

			public TexturedQuad(PositionTextureVertex[] apositiontexturevertex)
			{
				nVertices = 0;
				invertNormal = false;
				vertexPositions = apositiontexturevertex;
				nVertices = apositiontexturevertex.Length;
			}

			public TexturedQuad(PositionTextureVertex[] apositiontexturevertex, int i, int j, int k, int l, float f, float f1) :
				this(apositiontexturevertex)
			{
				float f2 = 0.0F / f;
				float f3 = 0.0F / f1;
				apositiontexturevertex[0] = apositiontexturevertex[0].setTexturePosition((float)k / f - f2, (float)j / f1 + f3);
				apositiontexturevertex[1] = apositiontexturevertex[1].setTexturePosition((float)i / f + f2, (float)j / f1 + f3);
				apositiontexturevertex[2] = apositiontexturevertex[2].setTexturePosition((float)i / f + f2, (float)l / f1 - f3);
				apositiontexturevertex[3] = apositiontexturevertex[3].setTexturePosition((float)k / f - f2, (float)l / f1 - f3);
			}

			public void flipFace()
			{
				PositionTextureVertex[] apositiontexturevertex = new PositionTextureVertex[vertexPositions.Length];
				for (int i = 0; i < vertexPositions.Length; i++)
					apositiontexturevertex[i] = vertexPositions[vertexPositions.Length - i - 1];

				vertexPositions = apositiontexturevertex;
			}
		}

		public class TextureOffset
		{

			public int field_40734_a;
			public int field_40733_b;

			public TextureOffset(int i, int j)
			{
				field_40734_a = i;
				field_40733_b = j;
			}
		}

		public class ModelRenderer
		{
			public float textureWidth;
			public float textureHeight;
			private int textureOffsetX;
			private int textureOffsetY;
			public float rotationPointX;
			public float rotationPointY;
			public float rotationPointZ;
			public float rotateAngleX;
			public float rotateAngleY;
			public float rotateAngleZ;
			private bool compiled;
			private int displayList;
			public bool mirror;
			public bool showModel;
			public bool isHidden;
			public List<ModelBox> cubeList;
			public List<ModelRenderer> childModels;
			public string boxName;
			private ModelBase baseModel;

			public VisiblePartFlags Flags;
			public bool Helmet;
			public bool Animate;

			public ModelRenderer(ModelBase b, VisiblePartFlags flags, bool helmet, bool animate) :
				this(b, null, flags, helmet, animate)
			{
			}

			public ModelRenderer(ModelBase modelbase, string s, VisiblePartFlags flags, bool helmet, bool animate)
			{
				textureWidth = 64F;
				textureHeight = 32F;
				compiled = false;
				displayList = 0;
				mirror = false;
				showModel = true;
				isHidden = false;
				cubeList = new List<ModelBox>();
				baseModel = modelbase;
				modelbase.boxList.Add(this);
				boxName = s;
				setTextureSize(modelbase.textureWidth, modelbase.textureHeight);

				Flags = flags;
				Helmet = helmet;
				Animate = animate;
			}

			public ModelRenderer(ModelBase modelbase, int i, int j, VisiblePartFlags flags, bool helmet, bool animate) :
				this(modelbase, null, flags, helmet, animate)
			{
				setTextureOffset(i, j);
			}

			public void addChild(ModelRenderer modelrenderer)
			{
				if (childModels == null)
				{
					childModels = new List<ModelRenderer>();
				}
				childModels.Add(modelrenderer);
			}

			public ModelRenderer setTextureOffset(int i, int j)
			{
				textureOffsetX = i;
				textureOffsetY = j;
				return this;
			}

			public ModelRenderer addBox(String s, float f, float f1, float f2, int i, int j, int k)
			{
				s = (new StringBuilder()).Append(boxName).Append(".").Append(s).ToString();
				TextureOffset textureoffset = baseModel.func_40297_a(s);
				setTextureOffset(textureoffset.field_40734_a, textureoffset.field_40733_b);
				cubeList.Add((new ModelBox(this, textureOffsetX, textureOffsetY, f, f1, f2, i, j, k, 0.0F)).func_40671_a(s));
				return this;
			}

			public ModelRenderer addBox(float f, float f1, float f2, int i, int j, int k)
			{
				cubeList.Add(new ModelBox(this, textureOffsetX, textureOffsetY, f, f1, f2, i, j, k, 0.0F));
				return this;
			}

			public void addBox(float f, float f1, float f2, int i, int j, int k, float f3)
			{
				cubeList.Add(new ModelBox(this, textureOffsetX, textureOffsetY, f, f1, f2, i, j, k, f3));
			}

			public void setRotationPoint(float f, float f1, float f2)
			{
				rotationPointX = f;
				rotationPointY = f1;
				rotationPointZ = f2;
			}

			public ModelRenderer setTextureSize(int i, int j)
			{
				textureWidth = i;
				textureHeight = j;
				return this;
			}
		}

		public class ModelBox
		{

			public PositionTextureVertex[] field_40679_h;
			public TexturedQuad[] field_40680_i;
			public float field_40678_a;
			public float field_40676_b;
			public float field_40677_c;
			public float field_40674_d;
			public float field_40675_e;
			public float field_40672_f;
			public String field_40673_g;

			public ModelBox(ModelRenderer modelrenderer, int i, int j, float f, float f1, float f2, int k,
					int l, int i1, float f3)
			{
				field_40678_a = f;
				field_40676_b = f1;
				field_40677_c = f2;
				field_40674_d = f + (float)k;
				field_40675_e = f1 + (float)l;
				field_40672_f = f2 + (float)i1;
				field_40679_h = new PositionTextureVertex[8];
				field_40680_i = new TexturedQuad[6];
				float f4 = f + (float)k;
				float f5 = f1 + (float)l;
				float f6 = f2 + (float)i1;
				f -= f3;
				f1 -= f3;
				f2 -= f3;
				f4 += f3;
				f5 += f3;
				f6 += f3;
				if (modelrenderer.mirror)
				{
					float f7 = f4;
					f4 = f;
					f = f7;
				}
				PositionTextureVertex positiontexturevertex = new PositionTextureVertex(f, f1, f2, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex1 = new PositionTextureVertex(f4, f1, f2, 0.0F, 8F);
				PositionTextureVertex positiontexturevertex2 = new PositionTextureVertex(f4, f5, f2, 8F, 8F);
				PositionTextureVertex positiontexturevertex3 = new PositionTextureVertex(f, f5, f2, 8F, 0.0F);
				PositionTextureVertex positiontexturevertex4 = new PositionTextureVertex(f, f1, f6, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex5 = new PositionTextureVertex(f4, f1, f6, 0.0F, 8F);
				PositionTextureVertex positiontexturevertex6 = new PositionTextureVertex(f4, f5, f6, 8F, 8F);
				PositionTextureVertex positiontexturevertex7 = new PositionTextureVertex(f, f5, f6, 8F, 0.0F);
				field_40679_h[0] = positiontexturevertex;
				field_40679_h[1] = positiontexturevertex1;
				field_40679_h[2] = positiontexturevertex2;
				field_40679_h[3] = positiontexturevertex3;
				field_40679_h[4] = positiontexturevertex4;
				field_40679_h[5] = positiontexturevertex5;
				field_40679_h[6] = positiontexturevertex6;
				field_40679_h[7] = positiontexturevertex7;
				field_40680_i[0] = new TexturedQuad(new PositionTextureVertex[] {
					positiontexturevertex5, positiontexturevertex1, positiontexturevertex2, positiontexturevertex6
				}, i + i1 + k, j + i1, i + i1 + k + i1, j + i1 + l, modelrenderer.textureWidth, modelrenderer.textureHeight);
				field_40680_i[1] = new TexturedQuad(new PositionTextureVertex[] {
					positiontexturevertex, positiontexturevertex4, positiontexturevertex7, positiontexturevertex3
				}, i + 0, j + i1, i + i1, j + i1 + l, modelrenderer.textureWidth, modelrenderer.textureHeight);
				field_40680_i[2] = new TexturedQuad(new PositionTextureVertex[] {
					positiontexturevertex5, positiontexturevertex4, positiontexturevertex, positiontexturevertex1
				}, i + i1, j + 0, i + i1 + k, j + i1, modelrenderer.textureWidth, modelrenderer.textureHeight);
				field_40680_i[3] = new TexturedQuad(new PositionTextureVertex[] {
					positiontexturevertex2, positiontexturevertex3, positiontexturevertex7, positiontexturevertex6
				}, i + i1 + k, j + i1, i + i1 + k + k, j + 0, modelrenderer.textureWidth, modelrenderer.textureHeight);
				field_40680_i[4] = new TexturedQuad(new PositionTextureVertex[] {
					positiontexturevertex1, positiontexturevertex, positiontexturevertex3, positiontexturevertex2
				}, i + i1, j + i1, i + i1 + k, j + i1 + l, modelrenderer.textureWidth, modelrenderer.textureHeight);
				field_40680_i[5] = new TexturedQuad(new PositionTextureVertex[] {
					positiontexturevertex4, positiontexturevertex5, positiontexturevertex6, positiontexturevertex7
				}, i + i1 + k + i1, j + i1, i + i1 + k + i1 + k, j + i1 + l, modelrenderer.textureWidth, modelrenderer.textureHeight);
				if (modelrenderer.mirror)
				{
					for (int j1 = 0; j1 < field_40680_i.Length; j1++)
					{
						field_40680_i[j1].flipFace();
					}

				}
			}

			public ModelBox func_40671_a(String s)
			{
				field_40673_g = s;
				return this;
			}
		}

		public class Entity { }
		public class EntityLiving { }
		public class Map { }

		public abstract class ModelBase
		{

			public float onGround;
			public bool isRiding;
			public List<ModelRenderer> boxList;
			public bool field_40301_k;
			private Dictionary<string, TextureOffset> field_39000_a;
			public int textureWidth;
			public int textureHeight;

			public ModelBase()
			{
				isRiding = false;
				boxList = new List<ModelRenderer>();
				field_40301_k = true;
				field_39000_a = new Dictionary<string, TextureOffset>();
				textureWidth = 64;
				textureHeight = 32;
			}

			protected void setTextureOffset(String s, int i, int j)
			{
				field_39000_a.Add(s, new TextureOffset(i, j));
			}

			public TextureOffset func_40297_a(String s)
			{
				return (TextureOffset)field_39000_a[s];
			}

			public Model Compile(string name, float scale = 1)
			{
				var model = new Model();
				model.Name = name;

				foreach (var box in boxList)
				{
					Mesh mesh = new Mesh("Test");
					mesh.Faces = new List<Face>();
					mesh.Translate = new Vector3(box.rotationPointX, box.rotationPointY, box.rotationPointZ);
					mesh.Helmet = box.Helmet;
					mesh.Part = box.Flags;
					mesh.Rotate = new Vector3(MathHelper.RadiansToDegrees(box.rotateAngleX), MathHelper.RadiansToDegrees(box.rotateAngleY), MathHelper.RadiansToDegrees(box.rotateAngleZ));
					mesh.Pivot = mesh.Translate;

					if (box.Animate)
						mesh.RotateFactor = (box.mirror) ? -25 : 25;

					if (box.Flags == VisiblePartFlags.HelmetFlag || box.Flags == VisiblePartFlags.HeadFlag)
						mesh.FollowCursor = true;

					mesh.Mode = BeginMode.Quads;

					foreach (var face in box.cubeList)
					{
						int[] cwIndices = new int[] { 0, 1, 2, 3 };
						int[] cwwIndices = new int[] { 3, 2, 1, 0 };
						Color4[] colors = new Color4[] { Color4.White, Color4.White, Color4.White, Color4.White };

						if (box.Helmet)
						{
							foreach (var quad in face.field_40680_i)
							{
								List<Vector3> vertices = new List<Vector3>();
								List<Vector2> texcoords = new List<Vector2>();

								foreach (var x in quad.vertexPositions)
								{
									vertices.Add(x.vector3D * scale);
									texcoords.Add(new Vector2(x.texturePositionX, x.texturePositionY));
								}

								Face newFace = new Face(vertices.ToArray(), texcoords.ToArray(), colors, cwwIndices);
								mesh.Faces.Add(newFace);
							}
						}


						foreach (var quad in face.field_40680_i)
						{
							List<Vector3> vertices = new List<Vector3>();
							List<Vector2> texcoords = new List<Vector2>();

							foreach (var x in quad.vertexPositions)
							{
								vertices.Add(x.vector3D * scale);
								texcoords.Add(new Vector2(x.texturePositionX, x.texturePositionY));
							}

							Face newFace = new Face(vertices.ToArray(), texcoords.ToArray(), colors, cwIndices);
							mesh.Faces.Add(newFace);
						}
					}

					model.Meshes.Add(mesh);
				}

				return model;
			}
		}

		public class ModelQuadruped : ModelBase
		{
			public ModelRenderer head;
			public ModelRenderer body;
			public ModelRenderer leg1;
			public ModelRenderer leg2;
			public ModelRenderer leg3;
			public ModelRenderer leg4;
			protected float field_40331_g;
			protected float field_40332_n;

			public ModelQuadruped(int i, float f)
			{
				field_40331_g = 8F;
				field_40332_n = 4F;
				head = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				head.addBox(-4F, -4F, -8F, 8, 8, 8, f);
				head.setRotationPoint(0.0F, 18 - i, -6F);
				body = new ModelRenderer(this, 28, 8, VisiblePartFlags.ChestFlag, false, false);
				body.addBox(-5F, -10F, -7F, 10, 16, 8, f);
				body.setRotationPoint(0.0F, 17 - i, 2.0F);
				body.rotateAngleX = 1.570796F;
				leg1 = new ModelRenderer(this, 0, 16, VisiblePartFlags.LeftLegFlag, false, true);
				leg1.addBox(-2F, 0.0F, -2F, 4, i, 4, f);
				leg1.setRotationPoint(-3F, 24 - i, 7F);
				leg2 = new ModelRenderer(this, 0, 16, VisiblePartFlags.RightLegFlag, false, true);
				leg2.mirror = true;
				leg2.addBox(-2F, 0.0F, -2F, 4, i, 4, f);
				leg2.setRotationPoint(3F, 24 - i, 7F);
				leg3 = new ModelRenderer(this, 0, 16, VisiblePartFlags.LeftArmFlag, false, true);
				leg3.addBox(-2F, 0.0F, -2F, 4, i, 4, f);
				leg3.setRotationPoint(-3F, 24 - i, -5F);
				leg4 = new ModelRenderer(this, 0, 16, VisiblePartFlags.RightArmFlag, false, true);
				leg4.addBox(-2F, 0.0F, -2F, 4, i, 4, f);
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
				head.setTextureOffset(16, 16).addBox(-2F, 0.0F, -9F, 4, 3, 1, f);
				field_40331_g = 4F;
			}
		}


		public class ModelBiped : ModelBase
		{

			public ModelRenderer bipedHead;
			public ModelRenderer bipedHeadwear;
			public ModelRenderer bipedBody;
			public ModelRenderer bipedRightArm;
			public ModelRenderer bipedLeftArm;
			public ModelRenderer bipedRightLeg;
			public ModelRenderer bipedLeftLeg;
			public ModelRenderer bipedEars;
			public ModelRenderer bipedCloak;
			public int heldItemLeft;
			public int heldItemRight;
			public bool isSneak;
			public bool aimedBow;

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
				bipedHead = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				bipedHead.addBox(-4F, -8F, -4F, 8, 8, 8, f);
				bipedHead.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
				bipedHeadwear = new ModelRenderer(this, 32, 0, VisiblePartFlags.HelmetFlag, true, false);
				bipedHeadwear.addBox(-4F, -8F, -4F, 8, 8, 8, f + 0.5F);
				bipedHeadwear.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
				bipedBody = new ModelRenderer(this, 16, 16, VisiblePartFlags.ChestFlag, false, false);
				bipedBody.addBox(-4F, 0.0F, -2F, 8, 12, 4, f);
				bipedBody.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
				bipedRightArm = new ModelRenderer(this, 40, 16, VisiblePartFlags.RightArmFlag, false, true);
				bipedRightArm.addBox(-3F, -2F, -2F, 4, 12, 4, f);
				bipedRightArm.setRotationPoint(-5F, 2.0F + f1, 0.0F);
				bipedLeftArm = new ModelRenderer(this, 40, 16, VisiblePartFlags.LeftArmFlag, false, true);
				bipedLeftArm.mirror = true;
				bipedLeftArm.addBox(-1F, -2F, -2F, 4, 12, 4, f);
				bipedLeftArm.setRotationPoint(5F, 2.0F + f1, 0.0F);
				bipedRightLeg = new ModelRenderer(this, 0, 16, VisiblePartFlags.RightLegFlag, false, true);
				bipedRightLeg.addBox(-2F, 0.0F, -2F, 4, 12, 4, f);
				bipedRightLeg.setRotationPoint(-2F, 12F + f1, 0.0F);
				bipedLeftLeg = new ModelRenderer(this, 0, 16, VisiblePartFlags.LeftLegFlag, false, true);
				bipedLeftLeg.mirror = true;
				bipedLeftLeg.addBox(-2F, 0.0F, -2F, 4, 12, 4, f);
				bipedLeftLeg.setRotationPoint(2.0F, 12F + f1, 0.0F);
			}
		}

		public class ModelVillager : ModelBase
		{
			public ModelRenderer head;
			public ModelRenderer body;
			public ModelRenderer arms;
			public ModelRenderer field_40336_d;
			public ModelRenderer field_40337_e;
			public int field_40334_f;
			public int field_40335_g;
			public bool field_40341_n;
			public bool field_40342_o;

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
				head = (new ModelRenderer(this, VisiblePartFlags.HeadFlag, false, false)).setTextureSize(byte0, byte1);
				head.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
				head.setTextureOffset(0, 0).addBox(-4F, -10F, -4F, 8, 10, 8, f);
				head.setTextureOffset(24, 0).addBox(-1F, -3F, -6F, 2, 4, 2, f);
				arms = (new ModelRenderer(this, VisiblePartFlags.LeftArmFlag, false, false)).setTextureSize(byte0, byte1);
				arms.rotationPointY = 3F;
				arms.rotationPointZ = -1F;
				arms.rotateAngleX = -0.75F;
				arms.setTextureOffset(44, 22).addBox(-8F, -2F, -2F, 4, 8, 4, f);
				arms.setTextureOffset(44, 22).addBox(4F, -2F, -2F, 4, 8, 4, f);
				arms.setTextureOffset(40, 38).addBox(-4F, 2.0F, -2F, 8, 4, 4, f);
				field_40336_d = (new ModelRenderer(this, 0, 22, VisiblePartFlags.LeftLegFlag, false, true)).setTextureSize(byte0, byte1);
				field_40336_d.setRotationPoint(-2F, 12F + f1, 0.0F);
				field_40336_d.addBox(-2F, 0.0F, -2F, 4, 12, 4, f);
				field_40337_e = (new ModelRenderer(this, 0, 22, VisiblePartFlags.RightLegFlag, false, true)).setTextureSize(byte0, byte1);
				field_40337_e.mirror = true;
				field_40337_e.setRotationPoint(2.0F, 12F + f1, 0.0F);
				field_40337_e.addBox(-2F, 0.0F, -2F, 4, 12, 4, f);
				body = (new ModelRenderer(this, VisiblePartFlags.ChestFlag, false, false)).setTextureSize(byte0, byte1);
				body.setRotationPoint(0.0F, 0.0F + f1, 0.0F);
				body.setTextureOffset(16, 20).addBox(-4F, 0.0F, -3F, 8, 12, 6, f);
				body.setTextureOffset(0, 38).addBox(-4F, 0.0F, -3F, 8, 18, 6, f + 0.5F);
			}
		}

		public class ModelCreeper : ModelBase
		{
			public ModelRenderer head;
			public ModelRenderer unusedCreeperHeadwear;
			public ModelRenderer body;
			public ModelRenderer leg1;
			public ModelRenderer leg2;
			public ModelRenderer leg3;
			public ModelRenderer leg4;

			public ModelCreeper() :
				this(0)
			{
			}

			public ModelCreeper(float f)
			{
				int i = 4;
				head = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				head.addBox(-4F, -8F, -4F, 8, 8, 8, f);
				head.setRotationPoint(0.0F, i, 0.0F);
				//unusedCreeperHeadwear = new ModelRenderer(this, 32, 0);
				//unusedCreeperHeadwear.addBox(-4F, -8F, -4F, 8, 8, 8, f + 0.5F);
				//unusedCreeperHeadwear.setRotationPoint(0.0F, i, 0.0F);
				body = new ModelRenderer(this, 16, 16, VisiblePartFlags.ChestFlag, false, false);
				body.addBox(-4F, 0.0F, -2F, 8, 12, 4, f);
				body.setRotationPoint(0.0F, i, 0.0F);
				leg1 = new ModelRenderer(this, 0, 16, VisiblePartFlags.LeftLegFlag, false, true);
				leg1.addBox(-2F, 0.0F, -2F, 4, 6, 4, f);
				leg1.setRotationPoint(-2F, 12 + i, 4F);
				leg2 = new ModelRenderer(this, 0, 16, VisiblePartFlags.RightLegFlag, false, true);
				leg2.addBox(-2F, 0.0F, -2F, 4, 6, 4, f);
				leg2.setRotationPoint(2.0F, 12 + i, 4F);
				leg2.mirror = true;
				leg3 = new ModelRenderer(this, 0, 16, VisiblePartFlags.LeftArmFlag, false, true);
				leg3.addBox(-2F, 0.0F, -2F, 4, 6, 4, f);
				leg3.setRotationPoint(-2F, 12 + i, -4F);
				leg4 = new ModelRenderer(this, 0, 16, VisiblePartFlags.RightArmFlag, false, true);
				leg4.addBox(-2F, 0.0F, -2F, 4, 6, 4, f);
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
				head = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				head.addBox(-4F, -4F, -6F, 8, 8, 6, 0.0F);
				head.setRotationPoint(0.0F, 4F, -8F);
				head.setTextureOffset(22, 0).addBox(-5F, -5F, -4F, 1, 3, 1, 0.0F);
				head.setTextureOffset(22, 0).addBox(4F, -5F, -4F, 1, 3, 1, 0.0F);
				boxList.Remove(body);
				body = new ModelRenderer(this, 18, 4, VisiblePartFlags.ChestFlag, false, false);
				body.addBox(-6F, -10F, -7F, 12, 18, 10, 0.0F);
				body.setRotationPoint(0.0F, 5F, 2.0F);
				body.setTextureOffset(52, 0).addBox(-2F, 2.0F, -8F, 4, 6, 1);
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

		public class ModelChicken : ModelBase
		{
			public ModelRenderer head;
			public ModelRenderer body;
			public ModelRenderer rightLeg;
			public ModelRenderer leftLeg;
			public ModelRenderer rightWing;
			public ModelRenderer leftWing;
			public ModelRenderer bill;
			public ModelRenderer chin;

			public ModelChicken()
			{
				byte byte0 = 16;
				head = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, true, false);
				head.addBox(-2F, -6F, -2F, 4, 6, 3, 0.0F);
				head.setRotationPoint(0.0F, -1 + byte0, -4F);
				bill = new ModelRenderer(this, 14, 0, VisiblePartFlags.HeadFlag, false, false);
				bill.addBox(-2F, -4F, -4F, 4, 2, 2, 0.0F);
				bill.setRotationPoint(0.0F, -1 + byte0, -4F);
				chin = new ModelRenderer(this, 14, 4, VisiblePartFlags.HeadFlag, false, false);
				chin.addBox(-1F, -2F, -3F, 2, 2, 2, 0.0F);
				chin.setRotationPoint(0.0F, -1 + byte0, -4F);
				body = new ModelRenderer(this, 0, 9, VisiblePartFlags.ChestFlag, false, false);
				body.addBox(-3F, -4F, -3F, 6, 8, 6, 0.0F);
				body.setRotationPoint(0.0F, 0 + byte0, 0.0F);
				rightLeg = new ModelRenderer(this, 26, 0, VisiblePartFlags.RightLegFlag, true, true);
				rightLeg.addBox(-1F, 0.0F, -3F, 3, 5, 3);
				rightLeg.setRotationPoint(-2F, 3 + byte0, 1.0F);
				leftLeg = new ModelRenderer(this, 26, 0, VisiblePartFlags.LeftLegFlag, true, true);
				leftLeg.addBox(-1F, 0.0F, -3F, 3, 5, 3);
				leftLeg.setRotationPoint(1.0F, 3 + byte0, 1.0F);
				leftLeg.mirror = true;
				rightWing = new ModelRenderer(this, 24, 13, VisiblePartFlags.RightArmFlag, false, true);
				rightWing.addBox(0.0F, 0.0F, -3F, 1, 4, 6);
				rightWing.setRotationPoint(-4F, -3 + byte0, 0.0F);
				leftWing = new ModelRenderer(this, 24, 13, VisiblePartFlags.LeftArmFlag, false, true);
				leftWing.addBox(-1F, 0.0F, -3F, 1, 4, 6);
				leftWing.setRotationPoint(4F, -3 + byte0, 0.0F);
			}
		}

		public class ModelSlime : ModelBase
		{
			ModelRenderer slimeBodies;
			ModelRenderer slimeRightEye;
			ModelRenderer slimeLeftEye;
			ModelRenderer slimeMouth;

			public ModelSlime(int i)
			{
				if(i > 0)
				{
					slimeBodies = new ModelRenderer(this, 0, 0, VisiblePartFlags.ChestFlag, false, false);
					slimeBodies.addBox(-3F, 17F, -3F, 6, 6, 6);
					slimeRightEye = new ModelRenderer(this, 32, 0, VisiblePartFlags.LeftArmFlag, false, false);
					slimeRightEye.addBox(-3.25F, 18F, -3.5F, 2, 2, 2);
					slimeLeftEye = new ModelRenderer(this, 32, 4, VisiblePartFlags.RightArmFlag, false, false);
					slimeLeftEye.addBox(1.25F, 18F, -3.5F, 2, 2, 2);
					slimeMouth = new ModelRenderer(this, 32, 8, VisiblePartFlags.RightArmFlag, false, false);
					slimeMouth.addBox(0.0F, 21F, -3.5F, 1, 1, 1);
				}
				slimeBodies = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				slimeBodies.addBox(-4F, 16F, -4F, 8, 8, 8);
			}
		}

		public class ModelSquid : ModelBase
		{
			ModelRenderer squidBody;
			ModelRenderer[] squidTentacles;

			public ModelSquid()
			{
				squidTentacles = new ModelRenderer[8];
				var byte0 = -16;
				squidBody = new ModelRenderer(this, 0, 0, VisiblePartFlags.ChestFlag, false, false);
				squidBody.addBox(-6F, -8F, -6F, 12, 16, 12);
				squidBody.rotationPointY += 24 + byte0;
				for(int i = 0; i < squidTentacles.Length; i++)
				{
					squidTentacles[i] = new ModelRenderer(this, 48, 0, VisiblePartFlags.LeftArmFlag, false, true);
					double d = ((double)i * 3.1415926535897931D * 2D) / (double)squidTentacles.Length;
					float f = (float)Math.Cos(d) * 5F;
					float f1 = (float)Math.Sin(d) * 5F;
					squidTentacles[i].addBox(-1F, 0.0F, -1F, 2, 18, 2);
					squidTentacles[i].rotationPointX = f;
					squidTentacles[i].rotationPointZ = f1;
					squidTentacles[i].rotationPointY = 31 + byte0;
					d = ((double)i * 3.1415926535897931D * -2D) / (double)squidTentacles.Length + 1.5707963267948966D;
					squidTentacles[i].rotateAngleY = (float)d;
				}
			}
		}

		public class ModelMagmaCube : ModelBase
		{
			ModelRenderer[] field_40345_a;
			ModelRenderer field_40344_b;

			public ModelMagmaCube()
			{
				field_40345_a = new ModelRenderer[8];
				for(int i = 0; i < field_40345_a.Length; i++)
				{
					byte byte0 = 0;
					int j = i;
					if(i == 2)
					{
						byte0 = 24;
						j = 10;
					} else
					if(i == 3)
					{
						byte0 = 24;
						j = 19;
					}
					field_40345_a[i] = new ModelRenderer(this, byte0, j, VisiblePartFlags.HeadFlag, false, false);
					field_40345_a[i].addBox(-4F, 16 + i, -4F, 8, 1, 8);
				}

				field_40344_b = new ModelRenderer(this, 0, 16, VisiblePartFlags.ChestFlag, false, false);
				field_40344_b.addBox(-2F, 18F, -2F, 4, 4, 4);
			}
		}

		public class ModelBlaze : ModelBase
		{
			private ModelRenderer[] field_40323_a;
			private ModelRenderer field_40322_b;

			public ModelBlaze()
			{
				field_40323_a = new ModelRenderer[12];
				for(int i = 0; i < field_40323_a.Length; i++)
				{
					field_40323_a[i] = new ModelRenderer(this, 0, 16, VisiblePartFlags.LeftArmFlag, false, false);
					field_40323_a[i].addBox(0.0F, 0.0F, 0.0F, 2, 8, 2);
				}

				field_40322_b = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				field_40322_b.addBox(-4F, -4F, -4F, 8, 8, 8);

				setRotationAngles(0, 0, 0, 0, 0, 0);
			}

			public void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
			{
				float f6 = f2 * 3.141593F * -0.1F;
				for (int i = 0; i < 4; i++)
				{
					field_40323_a[i].rotationPointY = -2F + (float)Math.Cos(((float)(i * 2) + f2) * 0.25F);
					field_40323_a[i].rotationPointX = (float)Math.Cos(f6) * 9F;
					field_40323_a[i].rotationPointZ = (float)Math.Sin(f6) * 9F;
					f6 += 1.570796F;
				}

				f6 = 0.7853982F + f2 * 3.141593F * 0.03F;
				for (int j = 4; j < 8; j++)
				{
					field_40323_a[j].rotationPointY = 2.0F + (float)Math.Cos(((float)(j * 2) + f2) * 0.25F);
					field_40323_a[j].rotationPointX = (float)Math.Cos(f6) * 7F;
					field_40323_a[j].rotationPointZ = (float)Math.Sin(f6) * 7F;
					f6 += 1.570796F;
				}

				f6 = 0.4712389F + f2 * 3.141593F * -0.05F;
				for (int k = 8; k < 12; k++)
				{
					field_40323_a[k].rotationPointY = 11F + (float)Math.Cos(((float)k * 1.5F + f2) * 0.5F);
					field_40323_a[k].rotationPointX = (float)Math.Cos(f6) * 5F;
					field_40323_a[k].rotationPointZ = (float)Math.Sin(f6) * 5F;
					f6 += 1.570796F;
				}

				field_40322_b.rotateAngleY = f3 / 57.29578F;
				field_40322_b.rotateAngleX = f4 / 57.29578F;
			}
		}

		public class ModelSilverfish : ModelBase
		{

			private ModelRenderer[] silverfishBodyParts;
			private ModelRenderer[] silverfishWings;
			private float[] field_35399_c;
			private static int[,] silverfishBoxLength = {
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
			private static int[,] silverfishTexturePositions = {
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

			public ModelSilverfish()
			{
				field_35399_c = new float[7];
				silverfishBodyParts = new ModelRenderer[7];
				float f = -3.5F;
				for (int i = 0; i < silverfishBodyParts.Length; i++)
				{
					silverfishBodyParts[i] = new ModelRenderer(this, silverfishTexturePositions[i, 0], silverfishTexturePositions[i, 1], VisiblePartFlags.ChestFlag, false, false);
					silverfishBodyParts[i].addBox((float)silverfishBoxLength[i, 0] * -0.5F, 0.0F, (float)silverfishBoxLength[i, 2] * -0.5F, silverfishBoxLength[i, 0], silverfishBoxLength[i, 1], silverfishBoxLength[i, 2]);
					silverfishBodyParts[i].setRotationPoint(0.0F, 24 - silverfishBoxLength[i, 1], f);
					field_35399_c[i] = f;
					if (i < silverfishBodyParts.Length - 1)
					{
						f += (float)(silverfishBoxLength[i, 2] + silverfishBoxLength[i + 1, 2]) * 0.5F;
					}
				}

				silverfishWings = new ModelRenderer[3];
				silverfishWings[0] = new ModelRenderer(this, 20, 0, VisiblePartFlags.HeadFlag, true, false);
				silverfishWings[0].addBox(-5F, 0.0F, (float)silverfishBoxLength[2, 2] * -0.5F, 10, 8, silverfishBoxLength[2, 2]);
				silverfishWings[0].setRotationPoint(0.0F, 16F, field_35399_c[2]);
				silverfishWings[1] = new ModelRenderer(this, 20, 11, VisiblePartFlags.HeadFlag, true, false);
				silverfishWings[1].addBox(-3F, 0.0F, (float)silverfishBoxLength[4, 2] * -0.5F, 6, 4, silverfishBoxLength[4, 2]);
				silverfishWings[1].setRotationPoint(0.0F, 20F, field_35399_c[4]);
				silverfishWings[2] = new ModelRenderer(this, 20, 18, VisiblePartFlags.HeadFlag, true, false);
				silverfishWings[2].addBox(-3F, 0.0F, (float)silverfishBoxLength[4, 2] * -0.5F, 6, 5, silverfishBoxLength[1, 2]);
				silverfishWings[2].setRotationPoint(0.0F, 19F, field_35399_c[1]);
			}
		}

		public class ModelEnderman : ModelBiped
		{
			public bool isCarrying;
			public bool isAttacking;

			public ModelEnderman() :
				base(0.0F, -14F)
			{
				isCarrying = false;
				isAttacking = false;
				float f = -14F;
				float f1 = 0.0F;
				bipedHead.Helmet = true;
				boxList.Remove(bipedHeadwear);
				bipedHeadwear = new ModelRenderer(this, 0, 16, VisiblePartFlags.HelmetFlag, true, false);
				bipedHeadwear.addBox(-4F, -8F, -4F, 8, 8, 8, f1 - 0.5F);
				bipedHeadwear.setRotationPoint(0.0F, 0.0F + f, 0.0F);
				boxList.Remove(bipedHead);
				boxList.Add(bipedHead);
				boxList.Remove(bipedBody);
				bipedBody = new ModelRenderer(this, 32, 16, VisiblePartFlags.ChestFlag, false, false);
				bipedBody.addBox(-4F, 0.0F, -2F, 8, 12, 4, f1);
				bipedBody.setRotationPoint(0.0F, 0.0F + f, 0.0F);
				boxList.Remove(bipedRightArm);
				bipedRightArm = new ModelRenderer(this, 56, 0, VisiblePartFlags.RightArmFlag, false, true);
				bipedRightArm.addBox(-1F, -2F, -1F, 2, 30, 2, f1);
				bipedRightArm.setRotationPoint(-5F, 2.0F + f, 0.0F);
				boxList.Remove(bipedLeftArm);
				bipedLeftArm = new ModelRenderer(this, 56, 0, VisiblePartFlags.LeftArmFlag, false, true);
				bipedLeftArm.mirror = true;
				bipedLeftArm.addBox(-1F, -2F, -1F, 2, 30, 2, f1);
				bipedLeftArm.setRotationPoint(5F, 2.0F + f, 0.0F);
				boxList.Remove(bipedRightLeg);
				bipedRightLeg = new ModelRenderer(this, 56, 0, VisiblePartFlags.RightLegFlag, false, true);
				bipedRightLeg.addBox(-1F, 0.0F, -1F, 2, 30, 2, f1);
				bipedRightLeg.setRotationPoint(-2F, 12F + f, 0.0F);
				boxList.Remove(bipedLeftLeg);
				bipedLeftLeg = new ModelRenderer(this, 56, 0, VisiblePartFlags.LeftLegFlag, false, true);
				bipedLeftLeg.mirror = true;
				bipedLeftLeg.addBox(-1F, 0.0F, -1F, 2, 30, 2, f1);
				bipedLeftLeg.setRotationPoint(2.0F, 12F + f, 0.0F);
			}
		}

		public class ModelWolf : ModelBase
		{
			public ModelRenderer wolfHeadMain;
			public ModelRenderer wolfBody;
			public ModelRenderer wolfLeg1;
			public ModelRenderer wolfLeg2;
			public ModelRenderer wolfLeg3;
			public ModelRenderer wolfLeg4;
			ModelRenderer wolfTail;
			ModelRenderer wolfMane;

			public ModelWolf()
			{
				float f = 0.0F;
				float f1 = 13.5F;
				wolfHeadMain = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				wolfHeadMain.addBox(-3F, -3F, -2F, 6, 6, 4, f);
				wolfHeadMain.setRotationPoint(-1F, f1, -7F);
				wolfBody = new ModelRenderer(this, 18, 14, VisiblePartFlags.ChestFlag, false, false);
				wolfBody.addBox(-4F, -2F, -3F, 6, 9, 6, f);
				wolfBody.setRotationPoint(0.0F, 14F, 2.0F);
				wolfMane = new ModelRenderer(this, 21, 0, VisiblePartFlags.HelmetFlag, false, false);
				wolfMane.addBox(-4F, -3F, -3F, 8, 6, 7, f);
				wolfMane.setRotationPoint(-1F, 14F, 2.0F);
				wolfLeg1 = new ModelRenderer(this, 0, 18, VisiblePartFlags.LeftLegFlag, false, true);
				wolfLeg1.addBox(-1F, 0.0F, -1F, 2, 8, 2, f);
				wolfLeg1.setRotationPoint(-2.5F, 16F, 7F);
				wolfLeg2 = new ModelRenderer(this, 0, 18, VisiblePartFlags.RightLegFlag, false, true);
				wolfLeg2.addBox(-1F, 0.0F, -1F, 2, 8, 2, f);
				wolfLeg2.setRotationPoint(0.5F, 16F, 7F);
				wolfLeg3 = new ModelRenderer(this, 0, 18, VisiblePartFlags.LeftArmFlag, false, true);
				wolfLeg3.addBox(-1F, 0.0F, -1F, 2, 8, 2, f);
				wolfLeg3.setRotationPoint(-2.5F, 16F, -4F);
				wolfLeg4 = new ModelRenderer(this, 0, 18, VisiblePartFlags.RightArmFlag, false, true);
				wolfLeg4.addBox(-1F, 0.0F, -1F, 2, 8, 2, f);
				wolfLeg4.setRotationPoint(0.5F, 16F, -4F);
				wolfTail = new ModelRenderer(this, 9, 18, VisiblePartFlags.ChestFlag, false, false);
				wolfTail.addBox(-1F, 0.0F, -1F, 2, 8, 2, f);
				wolfTail.setRotationPoint(-1F, 12F, 8F);
				wolfHeadMain.setTextureOffset(16, 14).addBox(-3F, -5F, 0.0F, 2, 2, 1, f);
				wolfHeadMain.setTextureOffset(16, 14).addBox(1.0F, -5F, 0.0F, 2, 2, 1, f);
				wolfHeadMain.setTextureOffset(0, 10).addBox(-1.5F, 0.0F, -5F, 3, 3, 4, f);

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
					wolfLeg1.rotateAngleX = (float)Math.Cos(f * 0.6662F) * 1.4F * f1;
					wolfLeg2.rotateAngleX = (float)Math.Cos(f * 0.6662F + 3.141593F) * 1.4F * f1;
					wolfLeg3.rotateAngleX = (float)Math.Cos(f * 0.6662F + 3.141593F) * 1.4F * f1;
					wolfLeg4.rotateAngleX = (float)Math.Cos(f * 0.6662F) * 1.4F * f1;
			}
		}

		public class ModelGhast : ModelBase
		{
			ModelRenderer body;
			ModelRenderer[] tentacles;

			public ModelGhast()
			{
				tentacles = new ModelRenderer[9];
				int byte0 = -16;
				body = new ModelRenderer(this, 0, 0, VisiblePartFlags.ChestFlag, false, false);
				body.addBox(-8F, -8F, -8F, 16, 16, 16);
				body.rotationPointY += 24 + byte0;
				Random random = new Random(1660);
				for(int i = 0; i < tentacles.Length; i++)
				{
					tentacles[i] = new ModelRenderer(this, 0, 0, VisiblePartFlags.LeftLegFlag, false, false);
					float f = (((((float)(i % 3) - (float)((i / 3) % 2) * 0.5F) + 0.25F) / 2.0F) * 2.0F - 1.0F) * 5F;
					float f1 = (((float)(i / 3) / 2.0F) * 2.0F - 1.0F) * 5F;
					int j = random.Next(7) + 8;
					tentacles[i].addBox(-1F, 0.0F, -1F, 2, j, 2);
					tentacles[i].rotationPointX = f;
					tentacles[i].rotationPointZ = f1;
					tentacles[i].rotationPointY = 31 + byte0;
				}

				setRotationAngles(0, 0, 123456, 0, 0, 0);
			}

			public void setRotationAngles(float f, float f1, float f2, float f3, float f4, float f5)
			{
				for(int i = 0; i < tentacles.Length; i++)
				{
					tentacles[i].rotateAngleX = 0.2F * (float)Math.Sin(f2 * 0.3F + (float)i) + 0.4F;
				}
			}
		}

		public class ModelSpider : ModelBase
		{
			public ModelRenderer spiderHead;
			public ModelRenderer spiderNeck;
			public ModelRenderer spiderBody;
			public ModelRenderer spiderLeg1;
			public ModelRenderer spiderLeg2;
			public ModelRenderer spiderLeg3;
			public ModelRenderer spiderLeg4;
			public ModelRenderer spiderLeg5;
			public ModelRenderer spiderLeg6;
			public ModelRenderer spiderLeg7;
			public ModelRenderer spiderLeg8;

			public ModelSpider()
			{
				float f = 0.0F;
				int i = 15;
				spiderHead = new ModelRenderer(this, 32, 4, VisiblePartFlags.HeadFlag, false, false);
				spiderHead.addBox(-4F, -4F, -8F, 8, 8, 8, f);
				spiderHead.setRotationPoint(0.0F, 0 + i, -3F);
				spiderNeck = new ModelRenderer(this, 0, 0, VisiblePartFlags.HelmetFlag, false, false);
				spiderNeck.addBox(-3F, -3F, -3F, 6, 6, 6, f);
				spiderNeck.setRotationPoint(0.0F, i, 0.0F);
				spiderBody = new ModelRenderer(this, 0, 12, VisiblePartFlags.ChestFlag, false, false);
				spiderBody.addBox(-5F, -4F, -6F, 10, 8, 12, f);
				spiderBody.setRotationPoint(0.0F, 0 + i, 9F);
				spiderLeg1 = new ModelRenderer(this, 18, 0, VisiblePartFlags.LeftArmFlag, false, false);
				spiderLeg1.addBox(-15F, -1F, -1F, 16, 2, 2, f);
				spiderLeg1.setRotationPoint(-4F, 0 + i, 2.0F);
				spiderLeg2 = new ModelRenderer(this, 18, 0, VisiblePartFlags.LeftArmFlag, false, true);
				spiderLeg2.addBox(-1F, -1F, -1F, 16, 2, 2, f);
				spiderLeg2.setRotationPoint(4F, 0 + i, 2.0F);
				spiderLeg3 = new ModelRenderer(this, 18, 0, VisiblePartFlags.LeftArmFlag, false, true);
				spiderLeg3.addBox(-15F, -1F, -1F, 16, 2, 2, f);
				spiderLeg3.setRotationPoint(-4F, 0 + i, 1.0F);
				spiderLeg4 = new ModelRenderer(this, 18, 0, VisiblePartFlags.LeftArmFlag, false, true);
				spiderLeg4.addBox(-1F, -1F, -1F, 16, 2, 2, f);
				spiderLeg4.setRotationPoint(4F, 0 + i, 1.0F);
				spiderLeg5 = new ModelRenderer(this, 18, 0, VisiblePartFlags.LeftArmFlag, false, true);
				spiderLeg5.addBox(-15F, -1F, -1F, 16, 2, 2, f);
				spiderLeg5.setRotationPoint(-4F, 0 + i, 0.0F);
				spiderLeg6 = new ModelRenderer(this, 18, 0, VisiblePartFlags.LeftArmFlag, false, true);
				spiderLeg6.addBox(-1F, -1F, -1F, 16, 2, 2, f);
				spiderLeg6.setRotationPoint(4F, 0 + i, 0.0F);
				spiderLeg7 = new ModelRenderer(this, 18, 0, VisiblePartFlags.LeftArmFlag, false, true);
				spiderLeg7.addBox(-15F, -1F, -1F, 16, 2, 2, f);
				spiderLeg7.setRotationPoint(-4F, 0 + i, -1F);
				spiderLeg8 = new ModelRenderer(this, 18, 0, VisiblePartFlags.LeftArmFlag, false, true);
				spiderLeg8.addBox(-1F, -1F, -1F, 16, 2, 2, f);
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
				float f9 = (float)-(Math.Cos(f * 0.6662F * 2.0F + 0.0F) * 0.4F) * f1;
				float f10 =(float) (-Math.Cos(f * 0.6662F * 2.0F + 3.141593F) * 0.4F) * f1;
				float f11 =(float) (-Math.Cos(f * 0.6662F * 2.0F + 1.570796F) * 0.4F) * f1;
				float f12 =(float) (-Math.Cos(f * 0.6662F * 2.0F + 4.712389F) * 0.4F) * f1;
				float f13 =(float) Math.Abs(Math.Sin(f * 0.6662F + 0.0F) * 0.4F) * f1;
				float f14 =(float) Math.Abs(Math.Sin(f * 0.6662F + 3.141593F) * 0.4F) * f1;
				float f15 =(float) Math.Abs(Math.Sin(f * 0.6662F + 1.570796F) * 0.4F) * f1;
				float f16 =(float) Math.Abs(Math.Sin(f * 0.6662F + 4.712389F) * 0.4F) * f1;
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
				head = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				head.addBox(-3F, -4F, -4F, 6, 6, 6, 0.6F);
				head.setRotationPoint(0.0F, 6F, -8F);
				boxList.Remove(body);
				body = new ModelRenderer(this, 28, 8, VisiblePartFlags.ChestFlag, false, false);
				body.addBox(-4F, -10F, -7F, 8, 16, 6, 1.75F);
				body.setRotationPoint(0.0F, 5F, 2.0F);
				body.rotateAngleX = 1.570796F;
				float f = 0.5F;
				boxList.Remove(leg1);
				leg1 = new ModelRenderer(this, 0, 16, VisiblePartFlags.LeftLegFlag, false, true);
				leg1.addBox(-2F, 0.0F, -2F, 4, 6, 4, f);
				leg1.setRotationPoint(-3F, 12F, 7F);
				boxList.Remove(leg2);
				leg2 = new ModelRenderer(this, 0, 16, VisiblePartFlags.RightLegFlag, false, true);
				leg2.addBox(-2F, 0.0F, -2F, 4, 6, 4, f);
				leg2.setRotationPoint(3F, 12F, 7F);
				boxList.Remove(leg3);
				leg3 = new ModelRenderer(this, 0, 16, VisiblePartFlags.LeftArmFlag, false, true);
				leg3.addBox(-2F, 0.0F, -2F, 4, 6, 4, f);
				leg3.setRotationPoint(-3F, 12F, -5F);
				boxList.Remove(leg4);
				leg4 = new ModelRenderer(this, 0, 16, VisiblePartFlags.RightArmFlag, false, true);
				leg4.addBox(-2F, 0.0F, -2F, 4, 6, 4, f);
				leg4.setRotationPoint(3F, 12F, -5F);
			}
		}

		public class ModelSheep2 : ModelQuadruped
		{
			public ModelSheep2() :
				base(12, 0.0F)
			{
				boxList.Remove(head);
				head = new ModelRenderer(this, 0, 0, VisiblePartFlags.HeadFlag, false, false);
				head.addBox(-3F, -4F, -6F, 6, 6, 8, 0.0F);
				head.setRotationPoint(0.0F, 6F, -8F);
				boxList.Remove(body);
				body = new ModelRenderer(this, 28, 8, VisiblePartFlags.ChestFlag, false, false);
				body.addBox(-4F, -10F, -7F, 8, 16, 6, 0.0F);
				body.setRotationPoint(0.0F, 5F, 2.0F);
				body.rotateAngleX = 1.570796F;
			}
		}

		public static void LoadModels()
		{
			new ModelPig().Compile("Pig").Save("Models\\Pig.xml");
			new ModelBiped().Compile("Human").Save("Models\\Human.xml");
			new ModelVillager().Compile("Villager").Save("Models\\Villager.xml");
			new ModelCreeper().Compile("Creeper").Save("Models\\Creeper.xml");
			new ModelCow().Compile("Cow").Save("Models\\Cow.xml");
			new ModelChicken().Compile("Chicken").Save("Models\\Chicken.xml");
			new ModelSlime(0).Compile("Tiny Slime").Save("Models\\TinySlime.xml");
			new ModelSlime(1).Compile("Small Slime", 2).Save("Models\\SmallSlime.xml");
			new ModelSlime(1).Compile("Medium Slime", 3).Save("Models\\MediumSlime.xml");
			new ModelSlime(1).Compile("Huge Slime", 4).Save("Models\\HugeSlime.xml");
			new ModelSquid().Compile("Squid").Save("Models\\Squid.xml");
			new ModelMagmaCube().Compile("Tiny Magma Cube").Save("Models\\TinyMagmaCube.xml");
			new ModelMagmaCube().Compile("Small Magma Cube", 2).Save("Models\\SmallMagmaCube.xml");
			new ModelMagmaCube().Compile("Medium Magma Cube", 3).Save("Models\\MediumMagmaCube.xml");
			new ModelMagmaCube().Compile("Huge Magma Cube", 4).Save("Models\\HugeMagmaCube.xml");
			new ModelBlaze().Compile("Blaze").Save("Models\\Blaze.xml");
			new ModelSilverfish().Compile("Silverfish").Save("Models\\Silverfish.xml");
			new ModelEnderman().Compile("Enderman").Save("Models\\Enderman.xml");
			new ModelWolf().Compile("Wolf").Save("Models\\Wolf.xml");
			new ModelGhast().Compile("Ghast", 1).Save("Models\\Ghast.xml");
			new ModelSpider().Compile("Spider").Save("Models\\Spider.xml");
			new ModelSheep1().Compile("Sheep Fur").Save("Models\\Sheep Fur.xml");
			new ModelSheep2().Compile("Sheep").Save("Models\\Sheep.xml");

			Directory.CreateDirectory("Models");

			foreach (var m in Directory.GetFiles("Models", "*.xml"))
			{
				try
				{
					Model model = Model.Load(m);
					Models.Add(model.Name, model);
				}
				catch
				{
				}
			}
		}
	}
}
