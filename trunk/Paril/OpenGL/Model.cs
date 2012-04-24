using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using MCSkin3D;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Paril.OpenGL
{
	public class Model
	{
		public float DefaultHeight;
		public float DefaultWidth;
		public FileInfo File;
		public List<Mesh> Meshes = new List<Mesh>();
		public string Name;
		public bool[] PartsEnabled;
		public Editor.ModelToolStripMenuItem DropDownItem { get; set; }

		public string Path
		{
			get
			{
				if (File.Directory.FullName.Length < Environment.CurrentDirectory.Length + 1 + "Models/".Length)
					return Name;
				return
					File.Directory.FullName.Substring(Environment.CurrentDirectory.Length + 1 + "Models/".Length).Replace('\\', '/') +
					'/' + Name;
			}
		}

		// P: polygon support required? used bounds 'n stuff but, you know...
		public Rectangle GetTextureFaceBounds(Point p, Skin skin)
		{
			foreach (Mesh m in Meshes)
			{
				foreach (Face f in m.Faces)
				{
					var bounds = new Bounds(new Point(9999, 9999), new Point(-9999, -9999));

					foreach (Vector2 c in f.TexCoords)
					{
						var coord = new Vector2(c.X * skin.Width, c.Y * skin.Height);
						bounds.AddPoint(new Point((int)coord.X, (int)coord.Y));
					}

					if (bounds.ToRectangle().Contains(p))
						return bounds.ToRectangle();
				}
			}

			return new Rectangle();
		}

		public List<int> GetIntersectingParts(Point p, Skin skin)
		{
			int mesh = 0;
			var parts = new List<int>();

			foreach (Mesh m in Meshes)
			{
				foreach (Face f in m.Faces)
				{
					var bounds = new Bounds(new Point(9999, 9999), new Point(-9999, -9999));

					foreach (Vector2 c in f.TexCoords)
					{
						var coord = new Vector2(c.X * skin.Width, c.Y * skin.Height);
						bounds.AddPoint(new Point((int)coord.X, (int)coord.Y));
					}

					if (bounds.ToRectangle().Contains(p))
					{
						parts.Add(mesh);
						break;
					}
				}

				mesh++;
			}

			return parts;
		}

		/*public void Save(string fileName)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.Indent = true;
			settings.NewLineOnAttributes = false;
			settings.IndentChars = "\t";

			using (XmlWriter writer = XmlWriter.Create(fileName, settings))
			{
				writer.WriteStartElement("Model");
				writer.WriteAttributeString("Name", Name);
				writer.WriteAttributeString("DefaultWidth", DefaultWidth.ToString());
				writer.WriteAttributeString("DefaultHeight", DefaultHeight.ToString());

				foreach (var mesh in Meshes)
					mesh.Write(writer);

				writer.WriteEndElement();
			}
		}*/

		public static Vector3 TranslateVertex(Vector3 translate, Vector3 rotate, Vector3 pivot, Vector3 v)
		{
			Matrix4 m =
				Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotate.X)) *
				Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotate.Y)) *
				Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotate.Z)) *
				Matrix4.CreateTranslation(translate);

			return Vector3.Transform(v, m);
		}

		public static Model Load(string fileName)
		{
			if (new FileInfo(fileName).Extension == ".xml" &&
				new FileInfo(fileName.Substring(0, fileName.Length - 4) + ".gz.xml").Exists)
				return null;

			var model = new Model();

			var settings = new XmlReaderSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.CloseInput = true;

			var document = new XmlDocument();
			Stream inStream;

			if (fileName.EndsWith(".gz.xml"))
				inStream = new GZipStream(System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);
			else
				inStream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read);

			document.Load(inStream);

			model.Name = document.DocumentElement.Attributes["Name"].InnerText;

			if (document.DocumentElement.Attributes["DefaultWidth"] != null)
			{
				model.DefaultWidth = float.Parse(document.DocumentElement.Attributes["DefaultWidth"].InnerText,
												 CultureInfo.InvariantCulture);
			}
			if (document.DocumentElement.Attributes["DefaultHeight"] != null)
			{
				model.DefaultHeight = float.Parse(document.DocumentElement.Attributes["DefaultHeight"].InnerText,
												  CultureInfo.InvariantCulture);
			}

			foreach (XmlElement n in document.DocumentElement.ChildNodes)
			{
				var mesh = new Mesh(n.Attributes["Name"].InnerText);

				if (n.Attributes["Mode"] != null)
					mesh.Mode = (BeginMode)Enum.Parse(typeof(BeginMode), n.Attributes["Mode"].InnerText);
				if (n.Attributes["Translate"] != null)
					mesh.Translate = Mesh.StringToVertex3(n.Attributes["Translate"].InnerText);
				if (n.Attributes["Pivot"] != null)
					mesh.Pivot = Mesh.StringToVertex3(n.Attributes["Pivot"].InnerText);
				if (n.Attributes["IsHelmet"] != null)
					mesh.HasTransparency = bool.Parse(n.Attributes["IsHelmet"].InnerText);
				if (n.Attributes["FollowCursor"] != null)
					mesh.FollowCursor = bool.Parse(n.Attributes["FollowCursor"].InnerText);
				if (n.Attributes["RotateFactor"] != null)
					mesh.RotateFactor = float.Parse(n.Attributes["RotateFactor"].InnerText);
				if (n.Attributes["Rotate"] != null)
					mesh.Rotate = Mesh.StringToVertex3(n.Attributes["Rotate"].InnerText);
				if (n.Attributes["Part"] != null)
					mesh.Part = (ModelPart)Enum.Parse(typeof(ModelPart), n.Attributes["Part"].InnerText);

				mesh.Faces = new List<Face>();

				foreach (XmlElement faceNode in n.ChildNodes)
				{
					if (faceNode.Name != "Face")
						continue;

					var face = new Face();

					face.Vertices = new Vector3[faceNode["Vertices"].ChildNodes.Count];
					face.TexCoords = new Vector2[face.Vertices.Length];
					face.Indices = new int[face.Vertices.Length];
					face.Colors = new Color4[face.Vertices.Length];

					int i = 0;
					foreach (XmlElement vertexNode in faceNode["Vertices"])
						face.Vertices[i++] = Mesh.StringToVertex3(vertexNode.InnerText);

					i = 0;
					foreach (XmlElement vertexNode in faceNode["TexCoords"])
						face.TexCoords[i++] = Mesh.StringToVertex2(vertexNode.InnerText);

					i = 0;
					foreach (XmlElement vertexNode in faceNode["Indices"])
						face.Indices[i++] = int.Parse(vertexNode.InnerText);

					Vector3 zero = TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, face.Vertices[0]);
					Vector3 one = TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, face.Vertices[1]);
					Vector3 two = TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, face.Vertices[2]);

					Vector3 dir = Vector3.Cross(one - zero, two - zero);
					face.Normal = Vector3.Normalize(dir);

					dir = Vector3.Cross(face.Vertices[1] - face.Vertices[0], face.Vertices[2] - face.Vertices[0]);
					Vector3 realNormal = Vector3.Normalize(dir);

					if (realNormal == new Vector3(0, 1, 0))
						face.Downface = true;

					mesh.Faces.Add(face);
				}

				mesh.CalculateCenter();

				model.Meshes.Add(mesh);
			}

			inStream.Dispose();

			return model;
		}

		public static string SideFromNormal(Vector3 vector3)
		{
			var vecs = new[]
			           {
			           	new Vector3(0, 1, 0),
			           	new Vector3(0, -1, 0),
			           	new Vector3(1, 0, 0),
			           	new Vector3(-1, 0, 0),
			           	new Vector3(0, 0, 1),
			           	new Vector3(0, 0, -1)
			           };

			var names = new[]
			            {
			            	"Bottom",
			            	"Top",
			            	"Right",
			            	"Left",
			            	"Back",
			            	"Front"
			            };

			int closestIndex = -1;
			float closestDist = float.MaxValue;

			if (float.IsNaN(vector3.X) &&
				float.IsNaN(vector3.Y) &&
				float.IsNaN(vector3.Z))
				return "";

			for (int i = 0; i < 6; ++i)
			{
				float dist = (vector3 - vecs[i]).LengthSquared;

				if (dist < closestDist)
				{
					closestDist = dist;
					closestIndex = i;
				}
			}

			return names[closestIndex];
		}
	}
}
