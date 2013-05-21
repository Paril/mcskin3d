using ICSharpCode.SharpZipLib.Zip;
using OpenTK;
using Paril.OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace MCSkin3D.Models
{
	internal class TCNRenderBase
	{
		public bool IsMirrored;
		public Vector3 Offset;
		public ModelPart Part;
		public Vector3 Position;
		public Vector3 Rotation;
		public float Scale;
		public Vector3 Size;
		public Vector2 TextureOffset;

		public virtual void Parse(XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				string name = child.Name.ToLower();

				if (name == "animation")
					continue; // skip animation
				else if (name == "ismirrored")
					IsMirrored = bool.Parse(child.InnerText);
				else if (name == "offset")
					Offset = Mesh.StringToVertex3(child.InnerText);
				else if (name == "position")
					Position = Mesh.StringToVertex3(child.InnerText);
				else if (name == "rotation")
					Rotation = Mesh.StringToVertex3(child.InnerText);
				else if (name == "size")
					Size = Mesh.StringToVertex3(child.InnerText);
				else if (name == "scale")
					Scale = float.Parse(child.InnerText, CultureInfo.InvariantCulture.NumberFormat);
				else if (name == "textureoffset")
					TextureOffset = Mesh.StringToVertex2(child.InnerText);
				else if (name == "part")
					Part = (ModelPart)Enum.Parse(typeof(ModelPart), child.InnerText);
			}
		}
	}

	internal class TCNRenderBox : TCNRenderBase
	{
		public bool IsDecorative;
		public bool IsFixed;

		public override void Parse(XmlNode node)
		{
			base.Parse(node);

			foreach (XmlNode child in node.ChildNodes)
			{
				string name = child.Name.ToLower();

				if (name == "isdecorative")
					IsDecorative = bool.Parse(child.InnerText);
				else if (name == "isfixed")
					IsFixed = bool.Parse(child.InnerText);
			}
		}
	}

	internal class TCNRenderPlane : TCNRenderBase
	{
		public int Side;

		public override void Parse(XmlNode node)
		{
			base.Parse(node);

			foreach (XmlNode child in node.ChildNodes)
			{
				string name = child.Name.ToLower();

				if (name == "side")
					Side = int.Parse(child.InnerText);
			}
		}
	}

	internal class TCNShape
	{
		private static readonly Dictionary<string, Type> _guidMap = new Dictionary<string, Type>();
		public TCNRenderBase RenderData;
		public string name;
		public string type;

		static TCNShape()
		{
			_guidMap.Add("d9e621f7-957f-4b77-b1ae-20dcd0da7751", typeof(TCNRenderBox));
			_guidMap.Add("ab894c83-e399-4236-808b-25a78d56f5e1", typeof(TCNRenderPlane));
		}

		public void Parse(XmlNode node)
		{
			foreach (XmlAttribute a in node.Attributes)
			{
				if (a.Name.ToLower() == "type")
					type = a.Value;
				else if (a.Name.ToLower() == "name")
					name = a.Value;
			}

			Type renderType = null;

			if (_guidMap.TryGetValue(type, out renderType))
			{
				RenderData = (TCNRenderBase)renderType.GetConstructors()[0].Invoke(null);
				RenderData.Parse(node);
			}
		}
	}

	internal class TCNFolder
	{
		public string Name;

		public List<TCNShape> Shapes = new List<TCNShape>();
		public string type;

		public void Parse(XmlNode node)
		{
			foreach (XmlAttribute a in node.Attributes)
			{
				if (a.Name.ToLower() == "type")
					type = a.Value;
				else if (a.Name.ToLower() == "name")
					Name = a.Value;
			}

			foreach (XmlNode child in node.ChildNodes)
			{
				string name = child.Name.ToLower();

				if (name == "shape")
				{
					var shape = new TCNShape();
					shape.Parse(child);
					Shapes.Add(shape);
				}
			}
		}
	}

	internal class TCNModel
	{
		public string BaseClass;

		public List<TCNFolder> Geometry = new List<TCNFolder>();

		public Vector3 GlScale;
		public string Name;
		public Vector2 TextureSize;
		public string texture;

		public void Parse(XmlNode node)
		{
			foreach (XmlAttribute a in node.Attributes)
			{
				if (a.Name.ToLower() == "texture")
					texture = a.Value;
			}

			foreach (XmlNode child in node.ChildNodes)
			{
				string name = child.Name.ToLower();

				if (name == "baseclass")
					BaseClass = child.InnerText;
				else if (name == "geometry")
				{
					TCNFolder rootFolder = new TCNFolder();

					foreach (XmlNode folderChild in child.ChildNodes)
					{
						if (folderChild.Name.ToLower() == "folder")
						{
							var folder = new TCNFolder();
							folder.Parse(folderChild);
							Geometry.Add(folder);
						}
						else if (folderChild.Name.ToLower() == "shape")
						{
							var shape = new TCNShape();
							shape.Parse(folderChild);
							rootFolder.Shapes.Add(shape);
						}
					}

					if (rootFolder.Shapes.Count != 0)
						Geometry.Add(rootFolder);
				}
				else if (name == "glscale")
					GlScale = Mesh.StringToVertex3(child.InnerText);
				else if (name == "name")
					Name = child.InnerText;
				else if (name == "texturesize")
					TextureSize = Mesh.StringToVertex2(child.InnerText);
			}
		}
	}

	internal class TCNFile
	{
		public string Author;
		public string DateCreated;
		public string Description;

		public List<TCNModel> Models = new List<TCNModel>();

		public string Name;
		public string PreviewImage;
		public string ProjectName;
		public string ProjectType;
		public string Version;

		public void Parse(XmlNode node)
		{
			if (node.Name != "Techne")
				return;

			foreach (XmlAttribute a in node.Attributes)
			{
				if (a.Name.ToLower() == "version")
					Version = a.Value;
			}

			foreach (XmlNode child in node.ChildNodes)
			{
				string name = child.Name.ToLower();

				if (name == "author")
					Author = child.InnerText;
				else if (name == "datecreated")
					DateCreated = child.InnerText;
				else if (name == "description")
					Description = child.InnerText;
				else if (name == "models")
				{
					foreach (XmlNode modelChild in child.ChildNodes)
					{
						if (modelChild.Name.ToLower() == "model")
						{
							var model = new TCNModel();
							model.Parse(modelChild);
							Models.Add(model);
						}
					}
				}
				else if (name == "name")
					Name = child.InnerText;
				else if (name == "previewimage")
					PreviewImage = child.InnerText;
				else if (name == "projectname")
					ProjectName = child.InnerText;
				else if (name == "projecttype")
					ProjectType = child.InnerText;
			}
		}

		public Model Convert()
		{
			var mb = new ModelLoader.ModelBase();

			foreach (TCNModel x in Models)
			{
				foreach (TCNFolder y in x.Geometry)
				{
					foreach (TCNShape z in y.Shapes)
					{
						mb.textureWidth = (int)x.TextureSize.X;
						mb.textureHeight = (int)x.TextureSize.Y;

						if (z.RenderData == null)
							continue;

						if (z.RenderData is TCNRenderBox)
						{
							var box = (TCNRenderBox)z.RenderData;

							var renderer = new ModelLoader.ModelRenderer(mb, (int)box.TextureOffset.X, (int)box.TextureOffset.Y, box.Part);
							renderer.mirror = box.IsMirrored;
							renderer.addBox(z.name, box.Offset.X, box.Offset.Y, box.Offset.Z, (int)box.Size.X, (int)box.Size.Y,
											(int)box.Size.Z, box.Scale);
							renderer.setRotationPoint(box.Position.X, box.Position.Y, box.Position.Z);
							renderer.rotateAngleX = MathHelper.DegreesToRadians(box.Rotation.X);
							renderer.rotateAngleY = MathHelper.DegreesToRadians(box.Rotation.Y);
							renderer.rotateAngleZ = MathHelper.DegreesToRadians(box.Rotation.Z);
						}
						else if (z.RenderData is TCNRenderPlane)
						{
							var box = (TCNRenderPlane)z.RenderData;

							var renderer = new ModelLoader.PlaneRenderer(mb, z.name, (int)box.TextureOffset.X, (int)box.TextureOffset.Y,
																		 box.Part);
							renderer.mirror = box.IsMirrored;
							renderer.setRotationPoint(box.Position.X, box.Position.Y, box.Position.Z);
							renderer.rotateAngleX = MathHelper.DegreesToRadians(box.Rotation.X);
							renderer.rotateAngleY = MathHelper.DegreesToRadians(box.Rotation.Y);
							renderer.rotateAngleZ = MathHelper.DegreesToRadians(box.Rotation.Z);

							switch (box.Side)
							{
								case 0:
									renderer.addBackPlane(box.Offset.X, box.Offset.Y, box.Offset.Z, (int)box.Size.X, (int)box.Size.Y,
														  (int)box.Size.Z, box.Scale);
									break;
								case 1:
									renderer.addSidePlane(box.Offset.X, box.Offset.Y, box.Offset.Z, (int)box.Size.X, (int)box.Size.Y,
														  (int)box.Size.Z, box.Scale);
									break;
								case 2:
									renderer.addTopPlane(box.Offset.X, box.Offset.Y, box.Offset.Z, (int)box.Size.X, (int)box.Size.Y,
														 (int)box.Size.Z, box.Scale);
									break;
							}
						}
					}
				}
			}

			return mb.Compile(Name, 1, (int)Models[0].TextureSize.X, (int)Models[0].TextureSize.Y);
		}
	}

	public class ModelFormatTCN : IModelFormat
	{
		#region IModelFormat Members

		public Model Load(string fileName)
		{
			var document = new XmlDocument();

			if (fileName.EndsWith(".tcn"))
			{
				var file = new ZipFile(new FileStream(fileName, FileMode.Open, FileAccess.Read));

				var enumerator = file.GetEnumerator();

				while (enumerator.MoveNext())
				{
					if (((ZipEntry)enumerator.Current).Name.EndsWith(".xml"))
					{
						document.Load(file.GetInputStream((ZipEntry)enumerator.Current));
						break;
					}
				}
			}
			else if (fileName.EndsWith(".xml"))
				document.Load(fileName);
			else
				throw new FileLoadException();

			var tcnModel = new TCNFile();
			tcnModel.Parse(document.DocumentElement);

			return tcnModel.Convert();
		}

		#endregion
	}
}