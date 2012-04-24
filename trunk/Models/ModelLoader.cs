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
using System.Drawing;
using System.Diagnostics;
using MCSkin3D.Models;
using System.Xml;

namespace MCSkin3D
{
	public static class ModelLoader
	{
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

			public TexturedQuad(PositionTextureVertex[] apositiontexturevertex)
			{
				nVertices = 0;
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
			public bool mirror;
			public bool showModel;
			public bool isHidden;
			public List<ModelBox> cubeList;
			public List<ModelRenderer> childModels;
			public string boxName;
			private ModelBase baseModel;

			public ModelPart Flags;

			public ModelRenderer(ModelBase b, ModelPart flags) :
				this(b, null, flags)
			{
			}

			public ModelRenderer(ModelBase modelbase, string s, ModelPart flags)
			{
				textureWidth = 64F;
				textureHeight = 32F;
				mirror = false;
				showModel = true;
				isHidden = false;
				cubeList = new List<ModelBox>();
				baseModel = modelbase;
				modelbase.boxList.Add(this);
				boxName = s;
				setTextureSize(modelbase.textureWidth, modelbase.textureHeight);

				Flags = flags;
			}

			public ModelRenderer(ModelBase modelbase, int i, int j, ModelPart flags) :
				this(modelbase, null, flags)
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

			public ModelRenderer addBox(string name, String s, float f, float f1, float f2, int i, int j, int k)
			{
				s = (new StringBuilder()).Append(boxName).Append(".").Append(s).ToString();
				TextureOffset textureoffset = baseModel.func_40297_a(s);
				setTextureOffset(textureoffset.field_40734_a, textureoffset.field_40733_b);
				cubeList.Add((new ModelBox(this, name, textureOffsetX, textureOffsetY, f, f1, f2, i, j, k, 0.0F)).func_40671_a(s));
				return this;
			}

			public ModelRenderer addBox(string name, float f, float f1, float f2, int i, int j, int k)
			{
				cubeList.Add(new ModelBox(this, name, textureOffsetX, textureOffsetY, f, f1, f2, i, j, k, 0.0F));
				return this;
			}

			public void addBox(string name, float f, float f1, float f2, int i, int j, int k, float f3)
			{
				cubeList.Add(new ModelBox(this, name, textureOffsetX, textureOffsetY, f, f1, f2, i, j, k, f3));
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

		public class PlaneRenderer
		{
			public float textureWidth;
			public float textureHeight;
			public PositionTextureVertex[] corners;
			public TexturedQuad[] faces;
			public int textureOffsetX;
			public int textureOffsetY;
			public float rotationPointX;
			public float rotationPointY;
			public float rotationPointZ;
			public float rotateAngleX;
			public float rotateAngleY;
			public float rotateAngleZ;
			public float x;
			public float y;
			public float z;
			public float xMax;
			public float yMax;
			public float zMax;
			public bool compiled;
			public int displayList;
			public bool mirror;
			public bool showModel;
			public bool isHidden;
			public string Name;
			public float Offset;
			public int Side;

			public ModelPart Flags;

			public PlaneRenderer(ModelBase modelbase, string name, int i, int j, ModelPart flags)
			{
				Name = name;
				this.textureWidth = 64.0F;
				this.textureHeight = 32.0F;
				this.compiled = false;
				this.displayList = 0;
				this.mirror = false;
				this.showModel = true;
				this.isHidden = false;
				this.textureOffsetX = i;
				this.textureOffsetY = j;
				modelbase.boxList.Add(this);

				Flags = flags;
			}

			public void addBackPlane(float f, float f1, float f2, int i, int j, int k)
			{
				addBackPlane(f, f1, f2, i, j, k, 0.0F);
			}

			public void addSidePlane(float f, float f1, float f2, int i, int j, int k)
			{
				addSidePlane(f, f1, f2, i, j, k, 0.0F);
			}

			public void addTopPlane(float f, float f1, float f2, int i, int j, int k)
			{
				addTopPlane(f, f1, f2, i, j, k, 0.0F);
			}

			public void addBackPlane(float f, float f1, float f2, int i, int j, int k, float f3)
			{
				Side = 0;
				Offset = f3;

				this.x = f;
				this.y = f1;
				this.z = f2;
				this.xMax = (f + i);
				this.yMax = (f1 + j);
				this.zMax = (f2 + k);
				this.corners = new PositionTextureVertex[8];
				this.faces = new TexturedQuad[1];
				float f4 = f + i;
				float f5 = f1 + j;
				float f6 = f2 + k;
				f -= f3;
				f1 -= f3;
				f2 -= f3;
				f4 += f3;
				f5 += f3;
				f6 += f3;
				if (this.mirror)
				{
					float f7 = f4;
					f4 = f;
					f = f7;
				}
				PositionTextureVertex positiontexturevertex = new PositionTextureVertex(f, f1, f2, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex1 = new PositionTextureVertex(f4, f1, f2, 0.0F, 8.0F);
				PositionTextureVertex positiontexturevertex2 = new PositionTextureVertex(f4, f5, f2, 8.0F, 8.0F);
				PositionTextureVertex positiontexturevertex3 = new PositionTextureVertex(f, f5, f2, 8.0F, 0.0F);
				PositionTextureVertex positiontexturevertex4 = new PositionTextureVertex(f, f1, f6, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex5 = new PositionTextureVertex(f4, f1, f6, 0.0F, 8.0F);
				PositionTextureVertex positiontexturevertex6 = new PositionTextureVertex(f4, f5, f6, 8.0F, 8.0F);
				PositionTextureVertex positiontexturevertex7 = new PositionTextureVertex(f, f5, f6, 8.0F, 0.0F);
				this.corners[0] = positiontexturevertex;
				this.corners[1] = positiontexturevertex1;
				this.corners[2] = positiontexturevertex2;
				this.corners[3] = positiontexturevertex3;
				this.corners[4] = positiontexturevertex4;
				this.corners[5] = positiontexturevertex5;
				this.corners[6] = positiontexturevertex6;
				this.corners[7] = positiontexturevertex7;
				this.faces[0] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex1, positiontexturevertex, positiontexturevertex3, positiontexturevertex2 }, this.textureOffsetX, this.textureOffsetY, this.textureOffsetX + i, this.textureOffsetY + j, this.textureWidth, this.textureHeight);

				if (this.mirror)
					this.faces[0].flipFace();
			}

			public void addSidePlane(float f, float f1, float f2, int i, int j, int k, float f3)
			{
				Side = 1;
				Offset = f3;
				
				this.x = f;
				this.y = f1;
				this.z = f2;
				this.xMax = (f + i);
				this.yMax = (f1 + j);
				this.zMax = (f2 + k);
				this.corners = new PositionTextureVertex[8];
				this.faces = new TexturedQuad[1];
				float f4 = f + i;
				float f5 = f1 + j;
				float f6 = f2 + k;
				f -= f3;
				f1 -= f3;
				f2 -= f3;
				f4 += f3;
				f5 += f3;
				f6 += f3;
				if (this.mirror)
				{
					float f7 = f4;
					f4 = f;
					f = f7;
				}
				PositionTextureVertex positiontexturevertex = new PositionTextureVertex(f, f1, f2, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex1 = new PositionTextureVertex(f4, f1, f2, 0.0F, 8.0F);
				PositionTextureVertex positiontexturevertex2 = new PositionTextureVertex(f4, f5, f2, 8.0F, 8.0F);
				PositionTextureVertex positiontexturevertex3 = new PositionTextureVertex(f, f5, f2, 8.0F, 0.0F);
				PositionTextureVertex positiontexturevertex4 = new PositionTextureVertex(f, f1, f6, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex5 = new PositionTextureVertex(f4, f1, f6, 0.0F, 8.0F);
				PositionTextureVertex positiontexturevertex6 = new PositionTextureVertex(f4, f5, f6, 8.0F, 8.0F);
				PositionTextureVertex positiontexturevertex7 = new PositionTextureVertex(f, f5, f6, 8.0F, 0.0F);
				this.corners[0] = positiontexturevertex;
				this.corners[1] = positiontexturevertex1;
				this.corners[2] = positiontexturevertex2;
				this.corners[3] = positiontexturevertex3;
				this.corners[4] = positiontexturevertex4;
				this.corners[5] = positiontexturevertex5;
				this.corners[6] = positiontexturevertex6;
				this.corners[7] = positiontexturevertex7;
				this.faces[0] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex5, positiontexturevertex1, positiontexturevertex2, positiontexturevertex6 }, this.textureOffsetX, this.textureOffsetY, this.textureOffsetX + k, this.textureOffsetY + j, this.textureWidth, this.textureHeight);

				if (this.mirror)
					this.faces[0].flipFace();
			}

			public void addTopPlane(float f, float f1, float f2, int i, int j, int k, float f3)
			{
				Side = 2;
				Offset = f3;

				this.x = f;
				this.y = f1;
				this.z = f2;
				this.xMax = (f + i);
				this.yMax = (f1 + j);
				this.zMax = (f2 + k);
				this.corners = new PositionTextureVertex[8];
				this.faces = new TexturedQuad[1];
				float f4 = f + i;
				float f5 = f1 + j;
				float f6 = f2 + k;
				f -= f3;
				f1 -= f3;
				f2 -= f3;
				f4 += f3;
				f5 += f3;
				f6 += f3;
				if (this.mirror)
				{
					float f7 = f4;
					f4 = f;
					f = f7;
				}
				PositionTextureVertex positiontexturevertex = new PositionTextureVertex(f, f1, f2, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex1 = new PositionTextureVertex(f4, f1, f2, 0.0F, 8.0F);
				PositionTextureVertex positiontexturevertex2 = new PositionTextureVertex(f4, f5, f2, 8.0F, 8.0F);
				PositionTextureVertex positiontexturevertex3 = new PositionTextureVertex(f, f5, f2, 8.0F, 0.0F);
				PositionTextureVertex positiontexturevertex4 = new PositionTextureVertex(f, f1, f6, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex5 = new PositionTextureVertex(f4, f1, f6, 0.0F, 8.0F);
				PositionTextureVertex positiontexturevertex6 = new PositionTextureVertex(f4, f5, f6, 8.0F, 8.0F);
				PositionTextureVertex positiontexturevertex7 = new PositionTextureVertex(f, f5, f6, 8.0F, 0.0F);
				this.corners[0] = positiontexturevertex;
				this.corners[1] = positiontexturevertex1;
				this.corners[2] = positiontexturevertex2;
				this.corners[3] = positiontexturevertex3;
				this.corners[4] = positiontexturevertex4;
				this.corners[5] = positiontexturevertex5;
				this.corners[6] = positiontexturevertex6;
				this.corners[7] = positiontexturevertex7;
				this.faces[0] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex5, positiontexturevertex4, positiontexturevertex, positiontexturevertex1 }, this.textureOffsetX, this.textureOffsetY, this.textureOffsetX + i, this.textureOffsetY + k, this.textureWidth, this.textureHeight);

				if (this.mirror)
					this.faces[0].flipFace();
			}

			public void setRotationPoint(float f, float f1, float f2)
			{
				this.rotationPointX = f;
				this.rotationPointY = f1;
				this.rotationPointZ = f2;
			}

			public PlaneRenderer setTextureSize(int i, int j)
			{
				this.textureWidth = i;
				this.textureHeight = j;
				return this;
			}
		}

		public class ModelBox
		{

			public PositionTextureVertex[] field_40679_h;
			public TexturedQuad[] field_40680_i;
			public float x;
			public float y;
			public float z;
			public float xMax;
			public float yMax;
			public float zMax;
			public String field_40673_g;
			public string Name;
			public int texX, texY;
			public float sizeOfs;

			public ModelBox(ModelRenderer modelrenderer, string name, int i, int j, float f, float f1, float f2, int k,
					int l, int i1, float f3)
			{
				sizeOfs = f3;
				texX = i;
				texY = j;
				Name = name;
				x = f;
				y = f1;
				z = f2;
				xMax = f + (float)k;
				yMax = f1 + (float)l;
				zMax = f2 + (float)i1;
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

		public class ModelBase
		{
			public float onGround;
			public bool isRiding;
			public List<object> boxList;
			public bool field_40301_k;
			private Dictionary<string, TextureOffset> field_39000_a;
			public int textureWidth;
			public int textureHeight;

			public ModelBase()
			{
				isRiding = false;
				boxList = new List<object>();
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

			public Model Compile(string name, float scale, float defaultWidth, float defaultHeight)
			{
				var model = new Model();
				model.Name = name;
				model.DefaultWidth = defaultWidth;
				model.DefaultHeight = defaultHeight;

				foreach (var boxObj in boxList)
				{
					if (boxObj is ModelRenderer)
					{
						ModelRenderer box = (ModelRenderer)boxObj;
						foreach (var face in box.cubeList)
						{
							Mesh mesh = new Mesh(face.Name);
							mesh.Faces = new List<Face>();
							mesh.Translate = new Vector3(box.rotationPointX, box.rotationPointY, box.rotationPointZ);
							mesh.Part = box.Flags;
							mesh.Rotate = new Vector3(MathHelper.RadiansToDegrees(box.rotateAngleX), MathHelper.RadiansToDegrees(box.rotateAngleY), MathHelper.RadiansToDegrees(box.rotateAngleZ));
							mesh.Pivot = mesh.Translate;

							mesh.Mode = BeginMode.Quads;

							int[] cwIndices = new int[] { 0, 1, 2, 3 };
							int[] cwwIndices = new int[] { 3, 2, 1, 0 };
							Color4[] colors = new Color4[] { Color4.White, Color4.White, Color4.White, Color4.White };

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

								var zero = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[0]);
								var one = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[1]);
								var two = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[2]);

								var dir = Vector3.Cross(one - zero, two - zero);
								newFace.Normal = Vector3.Normalize(dir);

								dir = Vector3.Cross(newFace.Vertices[1] - newFace.Vertices[0], newFace.Vertices[2] - newFace.Vertices[0]);
								var realNormal = Vector3.Normalize(dir);

								if (realNormal == new Vector3(0, 1, 0))
									newFace.Downface = true;

								mesh.Faces.Add(newFace);
							}

							mesh.CalculateCenter();

							model.Meshes.Add(mesh);
						}
					}
					else if (boxObj is PlaneRenderer)
					{
						PlaneRenderer box = (PlaneRenderer)boxObj;

						Mesh mesh = new Mesh(box.Name);
						mesh.Faces = new List<Face>();
						mesh.Translate = new Vector3(box.rotationPointX, box.rotationPointY, box.rotationPointZ);
						mesh.Part = box.Flags;
						mesh.Rotate = new Vector3(MathHelper.RadiansToDegrees(box.rotateAngleX), MathHelper.RadiansToDegrees(box.rotateAngleY), MathHelper.RadiansToDegrees(box.rotateAngleZ));
						mesh.Pivot = mesh.Translate;

						mesh.Mode = BeginMode.Quads;

						int[] cwIndices = new int[] { 0, 1, 2, 3 };
						int[] cwwIndices = new int[] { 3, 2, 1, 0 };
						Color4[] colors = new Color4[] { Color4.White, Color4.White, Color4.White, Color4.White };

						foreach (var quad in box.faces)
						{
							List<Vector3> vertices = new List<Vector3>();
							List<Vector2> texcoords = new List<Vector2>();

							foreach (var x in quad.vertexPositions)
							{
								vertices.Add(x.vector3D * scale);
								texcoords.Add(new Vector2(x.texturePositionX, x.texturePositionY));
							}

							Face newFace = new Face(vertices.ToArray(), texcoords.ToArray(), colors, cwIndices);

							var zero = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[0]);
							var one = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[1]);
							var two = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[2]);

							var dir = Vector3.Cross(one - zero, two - zero);
							newFace.Normal = Vector3.Normalize(dir);

							dir = Vector3.Cross(newFace.Vertices[1] - newFace.Vertices[0], newFace.Vertices[2] - newFace.Vertices[0]);
							var realNormal = Vector3.Normalize(dir);

							if (realNormal == new Vector3(0, 1, 0))
								newFace.Downface = true;

							mesh.Faces.Add(newFace);
						}

						mesh.CalculateCenter();

						model.Meshes.Add(mesh);
					}
				}

				model.PartsEnabled = new bool[model.Meshes.Count];

				for (int i = 0; i < model.Meshes.Count; ++i)
					model.PartsEnabled[i] = true;

				return model;
			}

			public void Save(string name, float scale, int defaultWidth, int defaultHeight, string fileName)
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.ConformanceLevel = ConformanceLevel.Fragment;
				settings.Indent = true;
				settings.NewLineOnAttributes = false;
				settings.IndentChars = "\t";

				using (XmlWriter writer = XmlWriter.Create(fileName, settings))
				{
					writer.WriteStartElement("Techne");
					writer.WriteAttributeString("Version", "2.2");
					writer.WriteElementString("Author", "");
					writer.WriteElementString("DateCreated", "");
					writer.WriteElementString("Description", "");

					writer.WriteStartElement("Models");
					writer.WriteStartElement("Model");
					writer.WriteAttributeString("texture", "none.png");

					writer.WriteElementString("BaseClass", "ModelBase");

					writer.WriteStartElement("Geometry");

					writer.WriteStartElement("Folder");
					writer.WriteAttributeString("type", "f8bf7d5b-37bf-455b-93f9-b6f9e81620e1");
					writer.WriteAttributeString("Name", "Model");

					foreach (var mesh in boxList)
					{
						if (mesh is ModelRenderer)
						{
							ModelRenderer renderer = (ModelRenderer)mesh;

							foreach (var x in renderer.cubeList)
							{
								writer.WriteStartElement("Shape");
								writer.WriteAttributeString("type", "d9e621f7-957f-4b77-b1ae-20dcd0da7751");
								writer.WriteAttributeString("name", x.Name);

								writer.WriteStartElement("Animation");
								writer.WriteElementString("AnimationAngles", "0,0,0");
								writer.WriteElementString("AnimationDuration", "0,0,0");
								writer.WriteElementString("AnimationType", "0,0,0");
								writer.WriteEndElement();

								writer.WriteElementString("IsDecorative", "False");
								writer.WriteElementString("IsFixed", "False");
								writer.WriteElementString("IsMirrored", renderer.mirror.ToString());
								writer.WriteElementString("Offset", renderer.rotationPointX + "," + renderer.rotationPointY + "," + renderer.rotationPointZ);
								writer.WriteElementString("Position", x.x + "," + x.y + "," + x.z);
								writer.WriteElementString("Rotation", MathHelper.RadiansToDegrees(renderer.rotateAngleX) + "," + MathHelper.RadiansToDegrees(renderer.rotateAngleY) + "," + MathHelper.RadiansToDegrees(renderer.rotateAngleZ));
								writer.WriteElementString("Size", (x.xMax - x.x) + "," + (x.yMax - x.y) + "," + (x.zMax - x.z));
								writer.WriteElementString("TextureOffset", x.texX + "," + x.texY);
								
								// Paril: MCSkin3D additions
								writer.WriteElementString("Scale", x.sizeOfs.ToString());
								writer.WriteElementString("Part", renderer.Flags.ToString());

								writer.WriteEndElement();
							}
						}
						else if (mesh is PlaneRenderer)
						{
							PlaneRenderer renderer = (PlaneRenderer)mesh;

							writer.WriteStartElement("Shape");
							writer.WriteAttributeString("type", "ab894c83-e399-4236-808b-25a78d56f5e1");
							writer.WriteAttributeString("name", renderer.Name);

							writer.WriteElementString("IsMirrored", renderer.mirror.ToString());
							writer.WriteElementString("Offset", renderer.rotationPointX + "," + renderer.rotationPointY + "," + renderer.rotationPointZ);
							writer.WriteElementString("Position", renderer.x + "," + renderer.y + "," + renderer.z);
							writer.WriteElementString("Rotation", MathHelper.RadiansToDegrees(renderer.rotateAngleX) + "," + MathHelper.RadiansToDegrees(renderer.rotateAngleY) + "," + MathHelper.RadiansToDegrees(renderer.rotateAngleZ));
							writer.WriteElementString("Size", (renderer.xMax - renderer.x) + "," + (renderer.yMax - renderer.y) + "," + (renderer.zMax - renderer.z));
							writer.WriteElementString("TextureOffset", renderer.textureOffsetX + "," + renderer.textureOffsetY);

							writer.WriteElementString("Scale", renderer.Offset.ToString());
							writer.WriteElementString("Part", renderer.Flags.ToString());
							
							writer.WriteElementString("Side", renderer.Side.ToString());

							writer.WriteEndElement();
						}
					}

					writer.WriteEndElement();

					writer.WriteEndElement();

					writer.WriteElementString("GlScale", "1,1,1");
					writer.WriteElementString("Name", name);
					writer.WriteElementString("TextureSize", defaultWidth + "," + defaultHeight);

					writer.WriteEndElement();
					writer.WriteEndElement();

					writer.WriteElementString("Name", name);

					writer.WriteEndElement();
				}
			}
		}

		public static void LoadModels()
		{
			new ModelPig().Save("Pig", 1, 64, 32, "Models\\Mobs\\Passive\\Pig.xml");
			new ModelBiped().Save("Human", 1, 64, 32, "Models\\Mobs\\Passive\\Human.xml");
			new ModelVillager().Save("Villager", 1, 64, 64, "Models\\Mobs\\Passive\\Villager.xml");
			new ModelCow().Save("Cow", 1, 64, 32, "Models\\Mobs\\Passive\\Cow.xml");
			new ModelChicken().Save("Chicken", 1, 64, 32, "Models\\Mobs\\Passive\\Chicken.xml");
			new ModelSquid().Save("Squid", 1, 64, 32, "Models\\Mobs\\Passive\\Squid.xml");
			new ModelWolf().Save("Wolf", 1, 64, 32, "Models\\Mobs\\Passive\\Wolf.xml");
			new ModelSheep1().Save("Sheep Fur", 1, 64, 32, "Models\\Mobs\\Passive\\Sheep Fur.xml");
			new ModelSheep2().Save("Sheep", 1, 64, 32, "Models\\Mobs\\Passive\\Sheep.xml");
			new ModelSnowMan().Save("SnowMan", 1, 64, 64, "Models\\Mobs\\Passive\\SnowMan.xml");

			new ModelChest().Save("Chest", 1, 64, 64, "Models\\Other\\Chest.xml");
			new ModelLargeChest().Save("Large Chest", 1, 128, 64, "Models\\Other\\LargeChest.xml");
			new ModelBoat().Save("Boat", 1, 64, 32, "Models\\Other\\Boat.xml");
			new SignModel().Save("Sign", 1, 64, 32, "Models\\Other\\Sign.xml");
			new ModelBook().Save("Book", 1, 64, 32, "Models\\Other\\Book.xml");
			new ModelMinecart().Save("Minecart", 1, 64, 32, "Models\\Other\\Minecart.xml");
			new ModelEnderCrystal().Save("Ender Crystal", 1, 128, 64, "Models\\Other\\EnderCrystal.xml");

			new ModelCreeper().Save("Creeper", 1, 64, 32, "Models\\Mobs\\Hostile\\Creeper.xml");
			new ModelSlime(0).Save("Tiny Slime", 1, 64, 32, "Models\\Mobs\\Hostile\\TinySlime.xml");
			new ModelSlime(16).Save("Small Slime", 2, 64, 32, "Models\\Mobs\\Hostile\\SmallSlime.xml");
			new ModelSlime(16).Save("Medium Slime", 3, 64, 32, "Models\\Mobs\\Hostile\\MediumSlime.xml");
			new ModelSlime(16).Save("Huge Slime", 4, 64, 32, "Models\\Mobs\\Hostile\\HugeSlime.xml");
			new ModelMagmaCube().Save("Tiny Magma Cube", 1, 64, 32, "Models\\Mobs\\Hostile\\TinyMagmaCube.xml");
			new ModelMagmaCube().Save("Small Magma Cube", 2, 64, 32, "Models\\Mobs\\Hostile\\SmallMagmaCube.xml");
			new ModelMagmaCube().Save("Medium Magma Cube", 3, 64, 32, "Models\\Mobs\\Hostile\\MediumMagmaCube.xml");
			new ModelMagmaCube().Save("Huge Magma Cube", 4, 64, 32, "Models\\Mobs\\Hostile\\HugeMagmaCube.xml");
			new ModelBlaze().Save("Blaze", 1, 64, 32, "Models\\Mobs\\Hostile\\Blaze.xml");
			new ModelSilverfish().Save("Silverfish", 1, 64, 32, "Models\\Mobs\\Hostile\\Silverfish.xml");
			new ModelEnderman().Save("Enderman", 1, 64, 32, "Models\\Mobs\\Hostile\\Enderman.xml");
			new ModelGhast().Save("Ghast", 1, 64, 32, "Models\\Mobs\\Hostile\\Ghast.xml");
			new ModelSpider().Save("Spider", 1, 64, 32, "Models\\Mobs\\Hostile\\Spider.xml");
			new ModelZombie().Save("Zombie", 1, 64, 32, "Models\\Mobs\\Hostile\\Zombie.xml");
			new ModelSkeleton().Save("Skeleton", 1, 64, 32, "Models\\Mobs\\Hostile\\Skeleton.xml");
			new ModelCloak().Save("Cloak", 1, 64, 32, "Models\\Other\\Cloak.xml");
			new ModelArmor().Save("Armor", 1, 64, 32, "Models\\Other\\Armor.xml");

			new ModelOzelot().Save("Ozelot", 1, 64, 32, "Models\\Mobs\\Passive\\Ozelot.xml");
			new ModelGolem().Save("Golem", 1, 128, 128, "Models\\Mobs\\Passive\\Golem.xml");

			new pm_Pony().init(true, true).Save("Pony", 1, 64, 32, "Models\\Mine Little Pony\\Pony.xml");
			new pm_newPonyAdv().init(0, 0).Save("New Pony", 1, 64, 32, "Models\\Mine Little Pony\\New Pony.xml");

			Directory.CreateDirectory("Models");

			ModelFormatTCN tcnParser = new ModelFormatTCN();

			foreach (var m in Directory.GetFiles("Models", "*.*", SearchOption.AllDirectories))
			{	
				try
				{
					Model model = tcnParser.Load(m);

					if (model == null)
						continue;

					model.File = new FileInfo(m);
					Models.Add(model.Path, model);
				}
				catch
				{
				}
			}
		}

		public static Model GetModelForPath(string p)
		{
			if (!Models.ContainsKey(p))
				return null;

			return Models[p];
		}
	}
}
