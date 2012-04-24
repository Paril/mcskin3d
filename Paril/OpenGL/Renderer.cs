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
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using MCSkin3D;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Paril.Drawing;

namespace Paril.OpenGL
{
	public struct Face
	{
		public Color4[] Colors;
		public bool Downface;
		public int[] Indices;
		public Vector3 Normal;
		public Vector2[] TexCoords;
		public Vector3[] Vertices;

		public Face(Vector3[] vertices, Vector2[] texCoords, Color4[] colors, int[] indices) :
			this()
		{
			Vertices = vertices;
			TexCoords = texCoords;
			Colors = colors;
			Indices = indices;
		}

		public Rectangle TexCoordsToInteger(int width, int height)
		{
			float minX = 1, minY = 1, maxX = 0, maxY = 0;

			foreach (Vector2 x in TexCoords)
			{
				if (x.X < minX)
					minX = x.X;
				if (x.X > maxX)
					maxX = x.X;

				if (x.Y < minY)
					minY = x.Y;
				if (x.Y > maxY)
					maxY = x.Y;
			}

			return new Rectangle((int) (minX * width), (int) (minY * height), (int) ((maxX - minX) * width),
			                     (int) ((maxY - minY) * height));
		}

		public RectangleF TexCoordsToFloat(int width, int height)
		{
			float minX = 1, minY = 1, maxX = 0, maxY = 0;

			foreach (Vector2 x in TexCoords)
			{
				if (x.X < minX)
					minX = x.X;
				if (x.X > maxX)
					maxX = x.X;

				if (x.Y < minY)
					minY = x.Y;
				if (x.Y > maxY)
					maxY = x.Y;
			}

			return new RectangleF((minX * width), (minY * height), ((maxX - minX) * width), ((maxY - minY) * height));
		}
	}

	public struct Mesh
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

		public Mesh(string name) :
			this()
		{
			Bounds = new Bounds3(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
			                     new Vector3(float.MinValue, float.MinValue, float.MinValue));
			Name = name;
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
			string[] spl = s.Split(new[] {' ', '\r', '\n', ','}, StringSplitOptions.RemoveEmptyEntries);

			return new Vector3(float.Parse(spl[0], CultureInfo.InvariantCulture),
			                   float.Parse(spl[1], CultureInfo.InvariantCulture),
			                   float.Parse(spl[2], CultureInfo.InvariantCulture));
		}

		internal static Vector2 StringToVertex2(string s)
		{
			string[] spl = s.Split(new[] {' ', '\r', '\n', ','}, StringSplitOptions.RemoveEmptyEntries);

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
	}

	public struct Bounds
	{
		private Point _maxs;
		private Point _mins;

		public Bounds(Point mins, Point maxs) :
			this()
		{
			_mins = mins;
			_maxs = maxs;
		}

		public Point Mins
		{
			get { return _mins; }
			set { _mins = value; }
		}

		public Point Maxs
		{
			get { return _maxs; }
			set { _maxs = value; }
		}

		public void AddPoint(Point p)
		{
			if (p.X < _mins.X)
				_mins.X = p.X;
			if (p.Y < _mins.Y)
				_mins.Y = p.Y;

			if (p.X > _maxs.X)
				_maxs.X = p.X;
			if (p.Y > _maxs.Y)
				_maxs.Y = p.Y;
		}

		public static Bounds operator +(Bounds left, Rectangle right)
		{
			left.AddPoint(new Point(right.Left, right.Top));
			left.AddPoint(new Point(right.Right, right.Bottom));

			return left;
		}

		public Rectangle ToRectangle()
		{
			var r = new Rectangle();

			r.X = _mins.X;
			r.Y = _mins.Y;
			r.Width = _maxs.X - _mins.X;
			r.Height = _maxs.Y - _mins.Y;

			return r;
		}
	}

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
						bounds.AddPoint(new Point((int) coord.X, (int) coord.Y));
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
						bounds.AddPoint(new Point((int) coord.X, (int) coord.Y));
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
					mesh.Mode = (BeginMode) Enum.Parse(typeof (BeginMode), n.Attributes["Mode"].InnerText);
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
					mesh.Part = (ModelPart) Enum.Parse(typeof (ModelPart), n.Attributes["Part"].InnerText);

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

	public abstract class Renderer
	{
		private readonly List<Mesh> _opaqueMeshes = new List<Mesh>();
		private readonly List<Mesh> _transparentMeshes = new List<Mesh>();

		public void Sort()
		{
			if (GlobalSettings.RenderBenchmark)
				Editor._sortTimer.Start();

			_transparentMeshes.Sort(
				(left, right) =>
				{
					float leftDist = (Editor.CameraPosition - left.Center).Length;
					float rightDist = (Editor.CameraPosition - right.Center).Length;

					return rightDist.CompareTo(leftDist);
				}
				);

			if (GlobalSettings.RenderBenchmark)
				Editor._sortTimer.Stop();
		}

		/*private float lerp(float min, float max, float value)
		{
			return min + value * (max - min);
		}

		private Color4 GetHeatMap(int count, int index)
		{
			Color4 blue = Color4.Blue;
			Color4 red = Color4.Red;
			float l = lerp(0, 1.0f, index / (float)count);

			return new Color4(l, 0, (1 - l), 255);
		}*/

		public void Render()
		{
			Sort();

			if (GlobalSettings.RenderBenchmark)
				Editor._batchTimer.Start();

			PreRender();

			GL.Disable(EnableCap.CullFace);
			foreach (Mesh mesh in _opaqueMeshes)
				RenderMesh(mesh);
			GL.Enable(EnableCap.CullFace);

			GL.Enable(EnableCap.Blend);

			foreach (Mesh mesh in _transparentMeshes)
			{
				/*foreach (var f in mesh.Faces)
					for (int i = 0; i < f.Colors.Length; ++i)
						f.Colors[i] = GetHeatMap(TransparentMeshes.Count, TransparentMeshes.IndexOf(mesh));
				*/

				GL.CullFace(CullFaceMode.Back);
				RenderMesh(mesh);
				GL.CullFace(CullFaceMode.Front);
				RenderMesh(mesh);
			}

			GL.Disable(EnableCap.Blend);

			PostRender();

			if (GlobalSettings.RenderBenchmark)
				Editor._batchTimer.Stop();

			_opaqueMeshes.Clear();
			_transparentMeshes.Clear();
		}

		public void RenderWithoutTransparency()
		{
			GL.Disable(EnableCap.Blend);

			PreRender();

			GL.Disable(EnableCap.CullFace);
			foreach (Mesh mesh in _opaqueMeshes)
				RenderMesh(mesh);
			GL.Enable(EnableCap.CullFace);

			foreach (Mesh mesh in _transparentMeshes)
			{
				GL.CullFace(CullFaceMode.Back);
				RenderMesh(mesh);
				GL.CullFace(CullFaceMode.Front);
				RenderMesh(mesh);
			}

			PostRender();

			_opaqueMeshes.Clear();
			_transparentMeshes.Clear();
		}

		public void AddMesh(Mesh mesh)
		{
			if (mesh.HasTransparency || mesh.Faces[0].Colors[0].A != 1)
				_transparentMeshes.Add(mesh);
			else
				_opaqueMeshes.Add(mesh);
		}

		protected virtual void PreRender()
		{
		}

		protected virtual void PostRender()
		{
		}

		public abstract void RenderMesh(Mesh mesh);
	}
}