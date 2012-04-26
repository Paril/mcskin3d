using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MCSkin3D;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Paril.Drawing;

namespace Paril.OpenGL
{
	public class Mesh
	{
		public Bounds3 Bounds;
		public Vector3 Center;
		public List<Face> Faces;
		public bool FollowCursor;
		public bool HasTransparency;
		public BeginMode Mode;
		public string Name;

		public ModelPart Part;
		public Vector3 Pivot;
		public Vector3 Rotate;
		public float RotateFactor;
		public Texture Texture;
		public Vector3 Translate;
		public bool DrawTransparent;
		public IMeshUserData UserData;
		public Matrix4 Matrix;

		public Mesh(string name)
		{
			Bounds = new Bounds3(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
								 new Vector3(float.MinValue, float.MinValue, float.MinValue));
			Name = name;
		}

		public T GetUserData<T>() where
			T : IMeshUserData
		{
			return (T)UserData;
		}

		internal static string VertexToString(Vector3 v, string sep = " ")
		{
			return v.X.ToString(CultureInfo.InvariantCulture) + sep + v.Y.ToString(CultureInfo.InvariantCulture) + sep +
				   v.Z.ToString(CultureInfo.InvariantCulture);
		}

		internal static string VertexToString(Vector2 v, string sep = " ")
		{
			return v.X.ToString(CultureInfo.InvariantCulture) + sep + v.Y.ToString(CultureInfo.InvariantCulture);
		}

		internal static Vector3 StringToVertex3(string s)
		{
			string[] spl = s.Split(new[] { ' ', '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);

			return new Vector3(float.Parse(spl[0], CultureInfo.InvariantCulture),
							   float.Parse(spl[1], CultureInfo.InvariantCulture),
							   float.Parse(spl[2], CultureInfo.InvariantCulture));
		}

		internal static Vector2 StringToVertex2(string s)
		{
			string[] spl = s.Split(new[] { ' ', '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);

			return new Vector2(float.Parse(spl[0], CultureInfo.InvariantCulture),
							   float.Parse(spl[1], CultureInfo.InvariantCulture));
		}

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("Mesh");
			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Mode", Mode.ToString());
			writer.WriteAttributeString("Translate", VertexToString(Translate));
			writer.WriteAttributeString("Pivot", VertexToString(Pivot));
			writer.WriteAttributeString("IsHelmet", HasTransparency.ToString());
			writer.WriteAttributeString("FollowCursor", FollowCursor.ToString());
			writer.WriteAttributeString("RotateFactor", RotateFactor.ToString());
			writer.WriteAttributeString("Rotate", VertexToString(Rotate));
			writer.WriteAttributeString("Part", Part.ToString());

			foreach (Face f in Faces)
			{
				writer.WriteStartElement("Face");

				writer.WriteStartElement("Vertices");
				foreach (Vector3 v in f.Vertices)
					writer.WriteElementString("Vertex", VertexToString(v));
				writer.WriteEndElement();

				writer.WriteStartElement("TexCoords");
				foreach (Vector2 v in f.TexCoords)
					writer.WriteElementString("Coord", VertexToString(v));
				writer.WriteEndElement();

				writer.WriteStartElement("Indices");
				foreach (int v in f.Indices)
					writer.WriteElementString("Index", v.ToString());
				writer.WriteEndElement();

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		public void CalculateCenter()
		{
			int count = 0;
			Bounds = new Bounds3(new Vector3(9999, 9999, 9999), new Vector3(-9999, -9999, -9999));

			foreach (Face x in Faces)
			{
				foreach (Vector3 v in x.Vertices)
				{
					count++;

					Vector3 transformed = Model.TranslateVertex(Translate, Rotate, Pivot, v);
					Center += transformed;
					Bounds += transformed;
				}
			}

			Center /= count;
		}

		public void CalculateMatrix()
		{
			Matrix = 
				Matrix4.CreateTranslation(Translate) *
				Matrix4.CreateTranslation(-Pivot) *
				Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(Rotate.X)) *
				Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(Rotate.Y)) *
				Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.DegreesToRadians(Rotate.Z)) *
				Matrix4.CreateTranslation(Pivot);
		}
	}
}
