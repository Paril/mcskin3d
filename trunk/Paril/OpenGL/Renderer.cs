using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System.IO;
using System.Xml;

namespace Paril.OpenGL
{
	public static class RenderState
	{
		static int _curTex = 0;
		public static void BindTexture(int texID)
		{
			if (_curTex == texID)
				return;

			GL.BindTexture(TextureTarget.Texture2D, texID);
			_curTex = texID;
		}
	}

	public struct Face
	{
		public Vector3[] Vertices;
		public Vector2[] TexCoords;
		public Color4[] Colors;
		public int[] Indices;

		public Face(Vector3[] vertices, Vector2[] texCoords, Color4[] colors, int[] indices) :
			this()
		{
			Vertices = vertices;
			TexCoords = texCoords;
			Colors = colors;
			Indices = indices;
		}
	}

	public struct Mesh
	{
		public string Name;
		public BeginMode Mode;
		public List<Face> Faces;
		public int Texture;

		public Vector3 Translate, Rotate;
		public Vector3 Pivot;

		public bool Helmet;
		public bool FollowCursor;
		public float RotateFactor;

		public Mesh(string name) :
			this()
		{
			Name = name;
		}

		internal static string VertexToString(Vector3 v)
		{
			return v.X + " " + v.Y + " " + v.Z;
		}

		internal static string VertexToString(Vector2 v)
		{
			return v.X + " " + v.Y;
		}

		internal static Vector3 StringToVertex3(string s)
		{
			var spl = s.Split();

			return new Vector3(float.Parse(spl[0]), float.Parse(spl[1]), float.Parse(spl[2]));
		}

		internal static Vector2 StringToVertex2(string s)
		{
			var spl = s.Split();

			return new Vector2(float.Parse(spl[0]), float.Parse(spl[1]));
		}

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("Mesh");
			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Mode", Mode.ToString());
			writer.WriteAttributeString("Translate", VertexToString(Translate));
			writer.WriteAttributeString("Pivot", VertexToString(Pivot));
			writer.WriteAttributeString("IsHelmet", Helmet.ToString());
			writer.WriteAttributeString("FollowCursor", FollowCursor.ToString());
			writer.WriteAttributeString("RotateFactor", RotateFactor.ToString());
			writer.WriteAttributeString("Part", Part.ToString());

			foreach (var f in Faces)
			{
				writer.WriteStartElement("Face");

				writer.WriteStartElement("Vertices");
				foreach (var v in f.Vertices)
					writer.WriteElementString("Vertex", VertexToString(v));
				writer.WriteEndElement();

				writer.WriteStartElement("TexCoords");
				foreach (var v in f.TexCoords)
					writer.WriteElementString("Coord", VertexToString(v));
				writer.WriteEndElement();

				writer.WriteStartElement("Indices");
				foreach (var v in f.Indices)
					writer.WriteElementString("Index", v.ToString());
				writer.WriteEndElement();

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		public MCSkin3D.VisiblePartFlags Part;
	}

	public class Model
	{
		public List<Mesh> Meshes = new List<Mesh>();
		public string Name;

		public void Save(string fileName)
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

				foreach (var mesh in Meshes)
					mesh.Write(writer);

				writer.WriteEndElement();
			}
		}

		public static Model Load(string fileName)
		{
			Model model = new Model();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.CloseInput = true;

			XmlDocument document = new XmlDocument();
			document.Load(fileName);

			model.Name = document.DocumentElement.Attributes["Name"].InnerText;

			foreach (XmlElement n in document.DocumentElement.ChildNodes)
			{
				Mesh mesh = new Mesh(n.Attributes["Name"].InnerText);

				if (n.Attributes["Mode"] != null)
					mesh.Mode = (BeginMode)Enum.Parse(typeof(BeginMode), n.Attributes["Mode"].InnerText);
				if (n.Attributes["Translate"] != null)
					mesh.Translate = Mesh.StringToVertex3(n.Attributes["Translate"].InnerText);
				if (n.Attributes["Pivot"] != null)
					mesh.Pivot = Mesh.StringToVertex3(n.Attributes["Pivot"].InnerText);
				if (n.Attributes["IsHelmet"] != null)
					mesh.Helmet = bool.Parse(n.Attributes["IsHelmet"].InnerText);
				if (n.Attributes["FollowCursor"] != null)
					mesh.FollowCursor = bool.Parse(n.Attributes["FollowCursor"].InnerText);
				if (n.Attributes["RotateFactor"] != null)
					mesh.RotateFactor = float.Parse(n.Attributes["RotateFactor"].InnerText);
				if (n.Attributes["Part"] != null)
					mesh.Part = (MCSkin3D.VisiblePartFlags)Enum.Parse(typeof(MCSkin3D.VisiblePartFlags), n.Attributes["Part"].InnerText);

				mesh.Faces = new List<Face>();

				foreach (XmlElement faceNode in n.ChildNodes)
				{
					if (faceNode.Name != "Face")
						continue;

					Face face = new Face();

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

					mesh.Faces.Add(face);
				}

				model.Meshes.Add(mesh);
			}

			return model;
		}
	}

	public abstract class Renderer
	{
		public List<Mesh> Meshes = new List<Mesh>();

		public void Render()
		{
			// FIXME: sort!

			PreRender();

			foreach (var mesh in Meshes)
				RenderMesh(mesh);
			
			PostRender();

			Meshes.Clear();
		}

		protected virtual void PreRender() { }
		protected virtual void PostRender() { }
		public abstract void RenderMesh(Mesh mesh);
	}
}
