//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
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
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using MCSkin3D.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Paril.OpenGL;

namespace MCSkin3D
{
	public static class ModelLoader
	{
		public static Dictionary<string, Model> Models = new Dictionary<string, Model>();

		public static void LoadModels()
		{
			if (!Directory.Exists(GlobalSettings.GetDataURI("Models")))
				return;

			var tcnParser = new ModelFormatTCN();

			foreach (string m in Directory.EnumerateFiles(GlobalSettings.GetDataURI("Models"), "*.*", SearchOption.AllDirectories))
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

		/*
		 * Magic from Java
		 */

		#region Nested type: ModelBase

		public class ModelBase
		{
			public float swingProgress;
			public bool isRiding;
			public bool isChild = true;
			public List<ModelRenderer> boxList = new List<ModelRenderer>();
			private Dictionary<string, TextureOffset> modelTextureMap = new Dictionary<string, TextureOffset>();
			public int textureWidth = 64;
			public int textureHeight = 32;

			public ModelRenderer getRandomModelBox(Random rand)
			{
				return (ModelRenderer)this.boxList[rand.Next(this.boxList.Count)];
			}

			protected virtual void setTextureOffset(string partName, int x, int y)
			{
				this.modelTextureMap.Add(partName, new TextureOffset(x, y));
			}

			public virtual TextureOffset getTextureOffset(string partName)
			{
				return (TextureOffset)this.modelTextureMap[partName];
			}

			/**
			 * Copies the angles from one object to another. This is used when objects should stay aligned with each other, like
			 * the hair over a players head.
			 */
			public static void copyModelAngles(ModelRenderer source, ModelRenderer dest)
			{
				dest.rotateAngleX = source.rotateAngleX;
				dest.rotateAngleY = source.rotateAngleY;
				dest.rotateAngleZ = source.rotateAngleZ;
				dest.rotationPointX = source.rotationPointX;
				dest.rotationPointY = source.rotationPointY;
				dest.rotationPointZ = source.rotationPointZ;
			}

			public virtual void setModelAttributes(ModelBase model)
			{
				this.swingProgress = model.swingProgress;
				this.isRiding = model.isRiding;
				this.isChild = model.isChild;
			}

			// Paril
			public void Save(string name, float scale, string fileName, string textureRef)
			{
				var settings = new XmlWriterSettings();
				settings.ConformanceLevel = ConformanceLevel.Fragment;
				settings.Indent = true;
				settings.NewLineOnAttributes = false;
				settings.IndentChars = "\t";
				settings.Encoding = Encoding.UTF8;

				Directory.CreateDirectory(Path.GetDirectoryName(fileName));

				var rootPos = Environment.ExpandEnvironmentVariables(@"%appdata%\.minecraft\versions\1.9.2\1.9.2\assets\minecraft\textures\entity\" + textureRef);

				using (XmlWriter writer = XmlWriter.Create(fileName, settings))
				{
					writer.WriteStartElement("Techne");
					{
						writer.WriteAttributeString("Version", "2.2");
						writer.WriteElementString("Author", "Mojang");
						writer.WriteElementString("DateCreated", "");
						writer.WriteElementString("Description", "Compiled from Minecraft source. Mojang pls don't sue :(");

						writer.WriteStartElement("Models");
						{
							writer.WriteStartElement("Model");
							{
								writer.WriteAttributeString("Texture", "none.png");
								writer.WriteElementString("BaseClass", "ModelBase");
								writer.WriteElementString("Name", name);

								if (!string.IsNullOrEmpty(textureRef))
									writer.WriteElementString("DefaultTexture", System.Convert.ToBase64String(File.ReadAllBytes(rootPos)));

								int tw = textureWidth, th = textureHeight;

								writer.WriteStartElement("Geometry");
								{
									writer.WriteStartElement("Folder");
									{
										writer.WriteAttributeString("Type", "f8bf7d5b-37bf-455b-93f9-b6f9e81620e1");
										writer.WriteAttributeString("Name", "Model");
										HashSet<ModelBox> rendered = new HashSet<ModelBox>();

										Action<ModelRenderer, ModelBox, Vector3, Vector3> renderRecursive = null;

										renderRecursive = (renderer, x, translation, rotation) =>
										{
											if (!renderer.showModel)
												return;

											if (rendered.Contains(x))
												return;

											rendered.Add(x);

											writer.WriteStartElement("Shape");
											{
												writer.WriteAttributeString("Type", "d9e621f7-957f-4b77-b1ae-20dcd0da7751");
												writer.WriteAttributeString("Name", x.boxName == null ? (renderer.boxName == null ? "Unknown" : renderer.boxName) : x.boxName);

												translation.X += renderer.rotationPointX;
												translation.Y += renderer.rotationPointY;
												translation.Z += renderer.rotationPointZ;

												rotation.X += renderer.rotateAngleX;
												rotation.Y += renderer.rotateAngleY;
												rotation.Z += renderer.rotateAngleZ;

												writer.WriteElementString("IsDecorative", "False");
												writer.WriteElementString("IsFixed", "False");
												writer.WriteElementString("IsMirrored", x.mirrored.ToString());
												writer.WriteElementString("IsSolid", renderer.isSolid.ToString());
												writer.WriteElementString("Offset",
																			x.posX1.ToString(CultureInfo.InvariantCulture) + "," +
																			x.posY1.ToString(CultureInfo.InvariantCulture) + "," +
																			x.posZ1.ToString(CultureInfo.InvariantCulture));
												writer.WriteElementString("Position", translation.X.ToString(CultureInfo.InvariantCulture) + "," +
																			translation.Y.ToString(CultureInfo.InvariantCulture) + "," +
																			translation.Z.ToString(CultureInfo.InvariantCulture));
												writer.WriteElementString("Rotation",
																			OpenTK.MathHelper.RadiansToDegrees(rotation.X).ToString(CultureInfo.InvariantCulture) + "," +
																			OpenTK.MathHelper.RadiansToDegrees(rotation.Y).ToString(CultureInfo.InvariantCulture) + "," +
																			OpenTK.MathHelper.RadiansToDegrees(rotation.Z).ToString(CultureInfo.InvariantCulture));
												writer.WriteElementString("Size", (x.posX2 - x.posX1).ToString(CultureInfo.InvariantCulture) + "," + (x.posY2 - x.posY1).ToString(CultureInfo.InvariantCulture) + "," + (x.posZ2 - x.posZ1).ToString(CultureInfo.InvariantCulture));
												writer.WriteElementString("TextureOffset", x.textureX + "," + x.textureY);

												// Paril: MCSkin3D additions
												writer.WriteElementString("Scale", x.sizeOfs.ToString(CultureInfo.InvariantCulture));
												writer.WriteElementString("Part", renderer.part.ToString());
												writer.WriteElementString("Hidden", renderer.isHidden.ToString());
												writer.WriteElementString("IsArmor", renderer.isArmor.ToString());

												writer.WriteEndElement();
											}

											if (renderer.childModels != null)
												foreach (var child in renderer.childModels)
													foreach (var box in child.cubeList)
														renderRecursive(child, box, translation, rotation);
										};

										foreach (ModelRenderer renderer in boxList)
										{
											if (renderer.parent != null)
												continue;

											foreach (ModelBox x in renderer.cubeList)
												renderRecursive(renderer, x, new Vector3(renderer.offsetX, renderer.offsetY, renderer.offsetZ), Vector3.Zero);

											tw = (int)System.Math.Max(renderer.textureWidth, tw);
											th = (int)System.Math.Max(renderer.textureHeight, th);
										}

										writer.WriteEndElement();
									}

									writer.WriteEndElement();
								}

								writer.WriteElementString("TextureSize", tw + "," + th);
								writer.WriteEndElement();
							}
							writer.WriteEndElement();
						}

						writer.WriteElementString("Name", name);

						writer.WriteEndElement();
					}
				}
			}

			public Model Compile(string name, float scale, float defaultWidth, float defaultHeight, string defaultTexture)
			{
				var model = new Model();
				model.Name = name;
				model.DefaultWidth = defaultWidth;
				model.DefaultHeight = defaultHeight;
				model.DefaultTexture = defaultTexture;

				foreach (ModelRenderer box in boxList)
				{
					foreach (ModelBox face in box.cubeList)
					{
						var mesh = new Mesh(face.boxName);
						mesh.Faces = new List<Face>();
						mesh.Translate = new Vector3(box.rotationPointX, box.rotationPointY, box.rotationPointZ);
						mesh.Part = box.part;
						mesh.Rotate = new Vector3(MathHelper.RadiansToDegrees(box.rotateAngleX),
													MathHelper.RadiansToDegrees(box.rotateAngleY),
													MathHelper.RadiansToDegrees(box.rotateAngleZ));
						mesh.Pivot = mesh.Translate;
						mesh.IsSolid = box.isSolid;
						mesh.IsArmor = box.isArmor;

						mesh.Mode = PrimitiveType.Quads;

						var cwIndices = new[] { 0, 1, 2, 3 };
						var cwwIndices = new[] { 3, 2, 1, 0 };

						foreach (TexturedQuad quad in face.quadList)
						{
							var vertices = new List<Vector3>();
							var texcoords = new List<Vector2>();

							foreach (PositionTextureVertex x in quad.vertexPositions)
							{
								var scaled = (x.vector3D * scale);
								vertices.Add(new Vector3((float)scaled.X, (float)scaled.Y, (float)scaled.Z));
								texcoords.Add(new Vector2(x.texturePositionX, x.texturePositionY));
							}

							var newFace = new Face(vertices.ToArray(), texcoords.ToArray(), cwwIndices);

							Vector3 zero = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[0]);
							Vector3 one = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[1]);
							Vector3 two = Model.TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, newFace.Vertices[2]);

							Vector3 dir = Vector3.Cross(one - zero, two - zero);
							newFace.Normal = Vector3.Normalize(dir);

							dir = Vector3.Cross(newFace.Vertices[1] - newFace.Vertices[0], newFace.Vertices[2] - newFace.Vertices[0]);
							Vector3 realNormal = Vector3.Normalize(dir);

							if (realNormal == new Vector3(0, 1, 0))
								newFace.Downface = true;

							mesh.Faces.Add(newFace);
						}

						mesh.CalculateCenter();
						mesh.CalculateMatrix();

						model.Meshes.Add(mesh);
					}
				}

				model.PartsEnabled = new bool[model.Meshes.Count];

				for (int i = 0; i < model.Meshes.Count; ++i)
					model.PartsEnabled[i] = true;

				return model;
			}
		}
		#endregion

		#region Nested type: ModelBox

		public class ModelBox
		{
			/**
			 * The (x,y,z) vertex positions and (u,v) texture coordinates for each of the 8 points on a cube
			 */
			public PositionTextureVertex[] vertexPositions;

			/** An array of 6 TexturedQuads, one for each face of a cube */
			public TexturedQuad[] quadList;

			/** X vertex coordinate of lower box corner */
			public float posX1;

			/** Y vertex coordinate of lower box corner */
			public float posY1;

			/** Z vertex coordinate of lower box corner */
			public float posZ1;

			/** X vertex coordinate of upper box corner */
			public float posX2;

			/** Y vertex coordinate of upper box corner */
			public float posY2;

			/** Z vertex coordinate of upper box corner */
			public float posZ2;
			public string boxName = "Unknown";

			public int textureX, textureY;
			public float sizeOfs;
			public bool mirrored;

			public ModelBox(ModelRenderer renderer, int p_i46359_2_, int p_i46359_3_, float p_i46359_4_, float p_i46359_5_, float p_i46359_6_, int p_i46359_7_, int p_i46359_8_, int p_i46359_9_, float p_i46359_10_) :
				this(renderer, p_i46359_2_, p_i46359_3_, p_i46359_4_, p_i46359_5_, p_i46359_6_, p_i46359_7_, p_i46359_8_, p_i46359_9_, p_i46359_10_, renderer.mirror)
			{
			}

			public ModelBox(ModelRenderer renderer, int textureX, int textureY, float p_i46301_4_, float p_i46301_5_, float p_i46301_6_, int p_i46301_7_, int p_i46301_8_, int p_i46301_9_, float p_i46301_10_, bool p_i46301_11_)
			{
				this.textureX = textureX;
				this.textureY = textureY;
				this.sizeOfs = p_i46301_10_;
				this.mirrored = p_i46301_11_;

				this.posX1 = p_i46301_4_;
				this.posY1 = p_i46301_5_;
				this.posZ1 = p_i46301_6_;
				this.posX2 = p_i46301_4_ + (float)p_i46301_7_;
				this.posY2 = p_i46301_5_ + (float)p_i46301_8_;
				this.posZ2 = p_i46301_6_ + (float)p_i46301_9_;
				this.vertexPositions = new PositionTextureVertex[8];
				this.quadList = new TexturedQuad[6];
				float f = p_i46301_4_ + (float)p_i46301_7_;
				float f1 = p_i46301_5_ + (float)p_i46301_8_;
				float f2 = p_i46301_6_ + (float)p_i46301_9_;
				p_i46301_4_ = p_i46301_4_ - p_i46301_10_;
				p_i46301_5_ = p_i46301_5_ - p_i46301_10_;
				p_i46301_6_ = p_i46301_6_ - p_i46301_10_;
				f = f + p_i46301_10_;
				f1 = f1 + p_i46301_10_;
				f2 = f2 + p_i46301_10_;

				if (p_i46301_11_)
				{
					float f3 = f;
					f = p_i46301_4_;
					p_i46301_4_ = f3;
				}

				PositionTextureVertex positiontexturevertex7 = new PositionTextureVertex(p_i46301_4_, p_i46301_5_, p_i46301_6_, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex = new PositionTextureVertex(f, p_i46301_5_, p_i46301_6_, 0.0F, 8.0F);
				PositionTextureVertex positiontexturevertex1 = new PositionTextureVertex(f, f1, p_i46301_6_, 8.0F, 8.0F);
				PositionTextureVertex positiontexturevertex2 = new PositionTextureVertex(p_i46301_4_, f1, p_i46301_6_, 8.0F, 0.0F);
				PositionTextureVertex positiontexturevertex3 = new PositionTextureVertex(p_i46301_4_, p_i46301_5_, f2, 0.0F, 0.0F);
				PositionTextureVertex positiontexturevertex4 = new PositionTextureVertex(f, p_i46301_5_, f2, 0.0F, 8.0F);
				PositionTextureVertex positiontexturevertex5 = new PositionTextureVertex(f, f1, f2, 8.0F, 8.0F);
				PositionTextureVertex positiontexturevertex6 = new PositionTextureVertex(p_i46301_4_, f1, f2, 8.0F, 0.0F);
				this.vertexPositions[0] = positiontexturevertex7;
				this.vertexPositions[1] = positiontexturevertex;
				this.vertexPositions[2] = positiontexturevertex1;
				this.vertexPositions[3] = positiontexturevertex2;
				this.vertexPositions[4] = positiontexturevertex3;
				this.vertexPositions[5] = positiontexturevertex4;
				this.vertexPositions[6] = positiontexturevertex5;
				this.vertexPositions[7] = positiontexturevertex6;
				this.quadList[0] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex4, positiontexturevertex, positiontexturevertex1, positiontexturevertex5 }, textureX + p_i46301_9_ + p_i46301_7_, textureY + p_i46301_9_, textureX + p_i46301_9_ + p_i46301_7_ + p_i46301_9_, textureY + p_i46301_9_ + p_i46301_8_, renderer.textureWidth, renderer.textureHeight);
				this.quadList[1] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex7, positiontexturevertex3, positiontexturevertex6, positiontexturevertex2 }, textureX, textureY + p_i46301_9_, textureX + p_i46301_9_, textureY + p_i46301_9_ + p_i46301_8_, renderer.textureWidth, renderer.textureHeight);
				this.quadList[2] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex4, positiontexturevertex3, positiontexturevertex7, positiontexturevertex }, textureX + p_i46301_9_, textureY, textureX + p_i46301_9_ + p_i46301_7_, textureY + p_i46301_9_, renderer.textureWidth, renderer.textureHeight);
				this.quadList[3] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex1, positiontexturevertex2, positiontexturevertex6, positiontexturevertex5 }, textureX + p_i46301_9_ + p_i46301_7_, textureY + p_i46301_9_, textureX + p_i46301_9_ + p_i46301_7_ + p_i46301_7_, textureY, renderer.textureWidth, renderer.textureHeight);
				this.quadList[4] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex, positiontexturevertex7, positiontexturevertex2, positiontexturevertex1 }, textureX + p_i46301_9_, textureY + p_i46301_9_, textureX + p_i46301_9_ + p_i46301_7_, textureY + p_i46301_9_ + p_i46301_8_, renderer.textureWidth, renderer.textureHeight);
				this.quadList[5] = new TexturedQuad(new PositionTextureVertex[] { positiontexturevertex3, positiontexturevertex4, positiontexturevertex5, positiontexturevertex6 }, textureX + p_i46301_9_ + p_i46301_7_ + p_i46301_9_, textureY + p_i46301_9_, textureX + p_i46301_9_ + p_i46301_7_ + p_i46301_9_ + p_i46301_7_, textureY + p_i46301_9_ + p_i46301_8_, renderer.textureWidth, renderer.textureHeight);

				if (p_i46301_11_)
				{
					for (int i = 0; i < this.quadList.Length; ++i)
					{
						this.quadList[i].flipFace();
					}
				}
			}

#if RENDER
		public void render(VertexBuffer renderer, float scale)
		{
			for (int i = 0; i < this.quadList.length; ++i)
			{
				this.quadList[i].draw(renderer, scale);
			}
		}
#endif

			public ModelBox setBoxName(string name)
			{
				this.boxName = name;
				return this;
			}
		}

		#endregion

		#region Nested type: ModelRenderer

		public class ModelRenderer
		{
			/** The size of the texture file's width in pixels. */
			public float textureWidth;

			/** The size of the texture file's height in pixels. */
			public float textureHeight;

			/** The X offset into the texture used for displaying this model */
			private int textureOffsetX;

			/** The Y offset into the texture used for displaying this model */
			private int textureOffsetY;
			public float rotationPointX;
			public float rotationPointY;
			public float rotationPointZ;
			public float rotateAngleX;
			public float rotateAngleY;
			public float rotateAngleZ;

			/** The GL display list rendered by the Tessellator for this model */
			public bool showModel;

			/** Hides the model. */
			public bool isHidden;
			public List<ModelBox> cubeList;
			public List<ModelRenderer> childModels;
			public string boxName;
			private ModelBase baseModel;
			public float offsetX;
			public float offsetY;
			public float offsetZ;

			public ModelRenderer parent;

			public ModelPart part;
			public bool isSolid = false;
			public bool isArmor = false;
			public float scale = 1;
			public bool mirror = false;

			public ModelRenderer(ModelBase model, string boxNameIn, ModelPart part = ModelPart.None)
			{
				this.textureWidth = 64.0F;
				this.textureHeight = 32.0F;
				this.showModel = true;
				this.cubeList = new List<ModelBox>();
				this.baseModel = model;
				model.boxList.Add(this);
				this.boxName = boxNameIn;
				this.setTextureSize(model.textureWidth, model.textureHeight);
				this.part = part;
			}

			public ModelRenderer(ModelBase model, ModelPart part = ModelPart.None) :
				this(model, (string)null, part)
			{
			}

			public ModelRenderer(ModelBase model, int texOffX, int texOffY, ModelPart part = ModelPart.None) :
				this(model, part)
			{
				this.setTextureOffset(texOffX, texOffY);
			}

			/**
			 * Sets the current box's rotation points and rotation angles to another box.
			 */
			public void addChild(ModelRenderer renderer)
			{
				if (this.childModels == null)
				{
					this.childModels = new List<ModelRenderer>();
				}

				this.childModels.Add(renderer);
				renderer.parent = this;
			}

			public ModelRenderer setTextureOffset(int x, int y)
			{
				this.textureOffsetX = x;
				this.textureOffsetY = y;
				return this;
			}

			public ModelRenderer addBox(string partName, float offX, float offY, float offZ, int width, int height, int depth, bool mirror = false)
			{
				partName = this.boxName + "." + partName;
				TextureOffset textureoffset = this.baseModel.getTextureOffset(partName);
				this.setTextureOffset(textureoffset.textureOffsetX, textureoffset.textureOffsetY);
				this.cubeList.Add((new ModelBox(this, this.textureOffsetX, this.textureOffsetY, offX, offY, offZ, width, height, depth, 0.0F, mirror)).setBoxName(partName));
				return this;
			}

			public ModelRenderer addBox(float offX, float offY, float offZ, int width, int height, int depth, string name = null)
			{
				this.cubeList.Add(new ModelBox(this, this.textureOffsetX, this.textureOffsetY, offX, offY, offZ, width, height, depth, 0.0F).setBoxName(name));
				return this;
			}

			public ModelRenderer addBox(float p_178769_1_, float p_178769_2_, float p_178769_3_, int p_178769_4_, int p_178769_5_, int p_178769_6_, bool p_178769_7_, float scale, string name = null)
			{
				this.cubeList.Add(new ModelBox(this, this.textureOffsetX, this.textureOffsetY, p_178769_1_, p_178769_2_, p_178769_3_, p_178769_4_, p_178769_5_, p_178769_6_, scale, p_178769_7_).setBoxName(name));
				return this;
			}

			public ModelRenderer addBox(float p_178769_1_, float p_178769_2_, float p_178769_3_, int p_178769_4_, int p_178769_5_, int p_178769_6_, bool p_178769_7_, string name = null)
			{
				this.cubeList.Add(new ModelBox(this, this.textureOffsetX, this.textureOffsetY, p_178769_1_, p_178769_2_, p_178769_3_, p_178769_4_, p_178769_5_, p_178769_6_, 0.0F, p_178769_7_).setBoxName(name));
				return this;
			}

			/**
			 * Creates a textured box. Args: originX, originY, originZ, width, height, depth, scaleFactor.
			 */
			public void addBox(float p_78790_1_, float p_78790_2_, float p_78790_3_, int width, int height, int depth, float scaleFactor, string name = null)
			{
				this.cubeList.Add(new ModelBox(this, this.textureOffsetX, this.textureOffsetY, p_78790_1_, p_78790_2_, p_78790_3_, width, height, depth, scaleFactor).setBoxName(name));
			}

			public void setRotationPoint(float rotationPointXIn, float rotationPointYIn, float rotationPointZIn)
			{
				this.rotationPointX = rotationPointXIn;
				this.rotationPointY = rotationPointYIn;
				this.rotationPointZ = rotationPointZIn;
			}

#if RENDER
		public void render(float p_78785_1_)
		{
			if (!this.isHidden)
			{
				if (this.showModel)
				{
					if (!this.compiled)
					{
						this.compileDisplayList(p_78785_1_);
					}

					GlStateManager.translate(this.offsetX, this.offsetY, this.offsetZ);

					if (this.rotateAngleX == 0.0F && this.rotateAngleY == 0.0F && this.rotateAngleZ == 0.0F)
					{
						if (this.rotationPointX == 0.0F && this.rotationPointY == 0.0F && this.rotationPointZ == 0.0F)
						{
							GlStateManager.callList(this.displayList);

							if (this.childModels != null)
							{
								for (int k = 0; k < this.childModels.size(); ++k)
								{
									((ModelRenderer)this.childModels.get(k)).render(p_78785_1_);
								}
							}
						}
						else
						{
							GlStateManager.translate(this.rotationPointX * p_78785_1_, this.rotationPointY * p_78785_1_, this.rotationPointZ * p_78785_1_);
							GlStateManager.callList(this.displayList);

							if (this.childModels != null)
							{
								for (int j = 0; j < this.childModels.size(); ++j)
								{
									((ModelRenderer)this.childModels.get(j)).render(p_78785_1_);
								}
							}

							GlStateManager.translate(-this.rotationPointX * p_78785_1_, -this.rotationPointY * p_78785_1_, -this.rotationPointZ * p_78785_1_);
						}
					}
					else
					{
						GlStateManager.pushMatrix();
						GlStateManager.translate(this.rotationPointX * p_78785_1_, this.rotationPointY * p_78785_1_, this.rotationPointZ * p_78785_1_);

						if (this.rotateAngleZ != 0.0F)
						{
							GlStateManager.rotate(this.rotateAngleZ * (180F / (float)Math.PI), 0.0F, 0.0F, 1.0F);
						}

						if (this.rotateAngleY != 0.0F)
						{
							GlStateManager.rotate(this.rotateAngleY * (180F / (float)Math.PI), 0.0F, 1.0F, 0.0F);
						}

						if (this.rotateAngleX != 0.0F)
						{
							GlStateManager.rotate(this.rotateAngleX * (180F / (float)Math.PI), 1.0F, 0.0F, 0.0F);
						}

						GlStateManager.callList(this.displayList);

						if (this.childModels != null)
						{
							for (int i = 0; i < this.childModels.size(); ++i)
							{
								((ModelRenderer)this.childModels.get(i)).render(p_78785_1_);
							}
						}

						GlStateManager.popMatrix();
					}

					GlStateManager.translate(-this.offsetX, -this.offsetY, -this.offsetZ);
				}
			}
		}

		public void renderWithRotation(float p_78791_1_)
		{
			if (!this.isHidden)
			{
				if (this.showModel)
				{
					if (!this.compiled)
					{
						this.compileDisplayList(p_78791_1_);
					}

					GlStateManager.pushMatrix();
					GlStateManager.translate(this.rotationPointX * p_78791_1_, this.rotationPointY * p_78791_1_, this.rotationPointZ * p_78791_1_);

					if (this.rotateAngleY != 0.0F)
					{
						GlStateManager.rotate(this.rotateAngleY * (180F / (float)Math.PI), 0.0F, 1.0F, 0.0F);
					}

					if (this.rotateAngleX != 0.0F)
					{
						GlStateManager.rotate(this.rotateAngleX * (180F / (float)Math.PI), 1.0F, 0.0F, 0.0F);
					}

					if (this.rotateAngleZ != 0.0F)
					{
						GlStateManager.rotate(this.rotateAngleZ * (180F / (float)Math.PI), 0.0F, 0.0F, 1.0F);
					}

					GlStateManager.callList(this.displayList);
					GlStateManager.popMatrix();
				}
			}
		}

		/**
		 * Allows the changing of Angles after a box has been rendered
		 */
		public void postRender(float scale)
		{
			if (!this.isHidden)
			{
				if (this.showModel)
				{
					if (!this.compiled)
					{
						this.compileDisplayList(scale);
					}

					if (this.rotateAngleX == 0.0F && this.rotateAngleY == 0.0F && this.rotateAngleZ == 0.0F)
					{
						if (this.rotationPointX != 0.0F || this.rotationPointY != 0.0F || this.rotationPointZ != 0.0F)
						{
							GlStateManager.translate(this.rotationPointX * scale, this.rotationPointY * scale, this.rotationPointZ * scale);
						}
					}
					else
					{
						GlStateManager.translate(this.rotationPointX * scale, this.rotationPointY * scale, this.rotationPointZ * scale);

						if (this.rotateAngleZ != 0.0F)
						{
							GlStateManager.rotate(this.rotateAngleZ * (180F / (float)Math.PI), 0.0F, 0.0F, 1.0F);
						}

						if (this.rotateAngleY != 0.0F)
						{
							GlStateManager.rotate(this.rotateAngleY * (180F / (float)Math.PI), 0.0F, 1.0F, 0.0F);
						}

						if (this.rotateAngleX != 0.0F)
						{
							GlStateManager.rotate(this.rotateAngleX * (180F / (float)Math.PI), 1.0F, 0.0F, 0.0F);
						}
					}
				}
			}
		}

		/**
		 * Compiles a GL display list for this model
		 */
		private void compileDisplayList(float scale)
		{
			this.displayList = GLAllocation.generateDisplayLists(1);
			GlStateManager.glNewList(this.displayList, 4864);
			VertexBuffer vertexbuffer = Tessellator.getInstance().getBuffer();

			for (int i = 0; i < this.cubeList.size(); ++i)
			{
				((ModelBox)this.cubeList.get(i)).render(vertexbuffer, scale);
			}

			GlStateManager.glEndList();
			this.compiled = true;
		}
#endif

			/**
			 * Returns the model renderer with the new texture parameters.
			 */
			public ModelRenderer setTextureSize(int textureWidthIn, int textureHeightIn)
			{
				this.textureWidth = (float)textureWidthIn;
				this.textureHeight = (float)textureHeightIn;
				return this;
			}
		}

		#endregion

		#region Nested type: PositionTextureVertex

		public class PositionTextureVertex
		{
			public Vector3d vector3D;
			public float texturePositionX;
			public float texturePositionY;

			public PositionTextureVertex(float p_i1158_1_, float p_i1158_2_, float p_i1158_3_, float p_i1158_4_, float p_i1158_5_) :
				this(new Vector3d((double)p_i1158_1_, (double)p_i1158_2_, (double)p_i1158_3_), p_i1158_4_, p_i1158_5_)
			{
			}

			public PositionTextureVertex setTexturePosition(float p_78240_1_, float p_78240_2_)
			{
				return new PositionTextureVertex(this, p_78240_1_, p_78240_2_);
			}

			public PositionTextureVertex(PositionTextureVertex textureVertex, float texturePositionXIn, float texturePositionYIn)
			{
				this.vector3D = textureVertex.vector3D;
				this.texturePositionX = texturePositionXIn;
				this.texturePositionY = texturePositionYIn;
			}

			public PositionTextureVertex(Vector3d p_i47091_1_, float p_i47091_2_, float p_i47091_3_)
			{
				this.vector3D = p_i47091_1_;
				this.texturePositionX = p_i47091_2_;
				this.texturePositionY = p_i47091_3_;
			}
		}

		#endregion

		#region Nested type: TextureOffset

		public class TextureOffset
		{
			/** The x coordinate offset of the texture */
			public int textureOffsetX;

			/** The y coordinate offset of the texture */
			public int textureOffsetY;

			public TextureOffset(int textureOffsetXIn, int textureOffsetYIn)
			{
				this.textureOffsetX = textureOffsetXIn;
				this.textureOffsetY = textureOffsetYIn;
			}
		}

		#endregion

		#region Nested type: TexturedQuad

		public class TexturedQuad
		{
			public PositionTextureVertex[] vertexPositions;
			public int nVertices;

			public TexturedQuad(PositionTextureVertex[] vertices)
			{
				this.vertexPositions = vertices;
				this.nVertices = vertices.Length;
			}

			public TexturedQuad(PositionTextureVertex[] vertices, int texcoordU1, int texcoordV1, int texcoordU2, int texcoordV2, float textureWidth, float textureHeight) :
				this(vertices)
			{
				float f = 0.0F / textureWidth;
				float f1 = 0.0F / textureHeight;
				vertices[0] = vertices[0].setTexturePosition((float)texcoordU2 / textureWidth - f, (float)texcoordV1 / textureHeight + f1);
				vertices[1] = vertices[1].setTexturePosition((float)texcoordU1 / textureWidth + f, (float)texcoordV1 / textureHeight + f1);
				vertices[2] = vertices[2].setTexturePosition((float)texcoordU1 / textureWidth + f, (float)texcoordV2 / textureHeight - f1);
				vertices[3] = vertices[3].setTexturePosition((float)texcoordU2 / textureWidth - f, (float)texcoordV2 / textureHeight - f1);
			}

			public void flipFace()
			{
				PositionTextureVertex[] apositiontexturevertex = new PositionTextureVertex[this.vertexPositions.Length];

				for (int i = 0; i < this.vertexPositions.Length; ++i)
				{
					apositiontexturevertex[i] = this.vertexPositions[this.vertexPositions.Length - i - 1];
				}

				this.vertexPositions = apositiontexturevertex;
			}

#if RENDER
		/**
		 * Draw this primitve. This is typically called only once as the generated drawing instructions are saved by the
		 * renderer and reused later.
		 */
		public void draw(VertexBuffer renderer, float scale)
		{
			Vec3d vec3d = this.vertexPositions[1].vector3D.subtractReverse(this.vertexPositions[0].vector3D);
			Vec3d vec3d1 = this.vertexPositions[1].vector3D.subtractReverse(this.vertexPositions[2].vector3D);
			Vec3d vec3d2 = vec3d1.crossProduct(vec3d).normalize();
			float f = (float)vec3d2.xCoord;
			float f1 = (float)vec3d2.yCoord;
			float f2 = (float)vec3d2.zCoord;

			if (this.invertNormal)
			{
				f = -f;
				f1 = -f1;
				f2 = -f2;
			}

			renderer.begin(7, DefaultVertexFormats.OLDMODEL_POSITION_TEX_NORMAL);

			for (int i = 0; i < 4; ++i)
			{
				PositionTextureVertex positiontexturevertex = this.vertexPositions[i];
				renderer.pos(positiontexturevertex.vector3D.xCoord * (double)scale, positiontexturevertex.vector3D.yCoord * (double)scale, positiontexturevertex.vector3D.zCoord * (double)scale).tex((double)positiontexturevertex.texturePositionX, (double)positiontexturevertex.texturePositionY).normal(f, f1, f2).endVertex();
			}

			Tessellator.getInstance().draw();
		}
#endif
		}

		#endregion
	}
}