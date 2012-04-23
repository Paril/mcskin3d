using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paril.OpenGL;
using OpenTK;
using System.Xml;
using System.IO;

namespace MCSkin3D.Models
{
	class TCNRenderBase
	{
		public bool IsMirrored;
		public Vector3 Offset;
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Size;
		public float Scale;
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
					Scale = float.Parse(child.InnerText);
				else if (name == "textureoffset")
					TextureOffset = Mesh.StringToVertex2(child.InnerText);
			}
		}
	}

	class TCNRenderBox : TCNRenderBase
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

	class TCNRenderPlane : TCNRenderBase
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

	class TCNShape
	{
		public string type;
		public string name;

		public TCNRenderBase RenderData;

		static Dictionary<string, Type> _guidMap = new Dictionary<string, Type>();
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

	class TCNFolder
	{
		public string type;
		public string Name;

		public List<TCNShape> Shapes = new List<TCNShape>();

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
					TCNShape shape = new TCNShape();
					shape.Parse(child);
					Shapes.Add(shape);
				}
			}
		}
	}

	class TCNModel
	{
		public string texture;
		public string BaseClass;

		public List<TCNFolder> Geometry = new List<TCNFolder>();

		public Vector3 GlScale;
		public string Name;
		public Vector2 TextureSize;

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
					foreach (XmlNode folderChild in child.ChildNodes)
					{
						if (folderChild.Name.ToLower() == "folder")
						{
							TCNFolder folder = new TCNFolder();
							folder.Parse(folderChild);
							Geometry.Add(folder);
						}
					}
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

	class TCNFile
	{
		public string Version;
		public string Author;
		public string DateCreated;
		public string Description;

		public List<TCNModel> Models = new List<TCNModel>();

		public string Name;
		public string PreviewImage;
		public string ProjectName;
		public string ProjectType;

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
							TCNModel model = new TCNModel();
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
			MCSkin3D.ModelLoader.ModelBase mb = new ModelLoader.ModelBase();

			foreach (var x in Models)
			{
				foreach (var y in x.Geometry)
				{
					foreach (var z in y.Shapes)
					{
						mb.textureWidth = (int)x.TextureSize.X;
						mb.textureHeight = (int)x.TextureSize.Y;

						if (z.RenderData == null)
							continue;

						if (z.RenderData is TCNRenderBox)
						{
							var box = (TCNRenderBox)z.RenderData;

							MCSkin3D.ModelLoader.ModelRenderer renderer = new ModelLoader.ModelRenderer(mb, (int)box.TextureOffset.X, (int)box.TextureOffset.Y, VisiblePartFlags.ChestFlag, false, false);
							renderer.setRotationPoint(box.Offset.X, box.Offset.Y, box.Offset.Z);
							renderer.rotateAngleX = MathHelper.DegreesToRadians(box.Rotation.X);
							renderer.rotateAngleY = MathHelper.DegreesToRadians(box.Rotation.Y);
							renderer.rotateAngleZ = MathHelper.DegreesToRadians(box.Rotation.Z);
							renderer.mirror = box.IsMirrored;
							renderer.addBox(z.name, box.Position.X, box.Position.Y, box.Position.Z, (int)box.Size.X, (int)box.Size.Y, (int)box.Size.Z, box.Scale);
						}
						else if (z.RenderData is TCNRenderPlane)
						{
							var box = (TCNRenderPlane)z.RenderData;

							MCSkin3D.ModelLoader.PlaneRenderer renderer = new ModelLoader.PlaneRenderer(mb, z.name, (int)box.TextureOffset.X, (int)box.TextureOffset.Y, VisiblePartFlags.ChestFlag, false, false);
							renderer.setRotationPoint(box.Offset.X, box.Offset.Y, box.Offset.Z);
							renderer.rotateAngleX = MathHelper.DegreesToRadians(box.Rotation.X);
							renderer.rotateAngleY = MathHelper.DegreesToRadians(box.Rotation.Y);
							renderer.rotateAngleZ = MathHelper.DegreesToRadians(box.Rotation.Z);
							renderer.mirror = box.IsMirrored;

							switch (box.Side)
							{
								case 0:
									renderer.addBackPlane(box.Position.X, box.Position.Y, box.Position.Z, (int)box.Size.X, (int)box.Size.Y, (int)box.Size.Z, box.Scale);
									break;
								case 1:
									renderer.addSidePlane(box.Position.X, box.Position.Y, box.Position.Z, (int)box.Size.X, (int)box.Size.Y, (int)box.Size.Z, box.Scale);
									break;
								case 2:
									renderer.addTopPlane(box.Position.X, box.Position.Y, box.Position.Z, (int)box.Size.X, (int)box.Size.Y, (int)box.Size.Z, box.Scale);
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
		public Model Load(string fileName)
		{
			XmlDocument document = new XmlDocument();

			if (fileName.EndsWith(".tcn"))
			{
				ICSharpCode.SharpZipLib.Zip.ZipFile file = new ICSharpCode.SharpZipLib.Zip.ZipFile(new FileStream(fileName, FileMode.Open, FileAccess.Read));
				var model = file.GetEntry("model.xml");

				document.Load(file.GetInputStream(model));
			}
			else if (fileName.EndsWith(".xml"))
				document.Load(fileName);
			else
				throw new FileLoadException();

			TCNFile tcnModel = new TCNFile();
			tcnModel.Parse(document.DocumentElement);

			return tcnModel.Convert();
		}
	}
}
