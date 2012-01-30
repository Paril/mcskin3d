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
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Drawing;
using MCSkin3D;
using Paril.Drawing;
using System.IO.Compression;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Reflection;

namespace Paril.OpenGL
{
	public struct Face
	{
		public Vector3[] Vertices;
		public Vector2[] TexCoords;
		public Color4[] Colors;
		public int[] Indices;
		public bool Downface;
		public Vector3 Normal;

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

			foreach (var x in TexCoords)
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

			return new Rectangle((int)(minX * width), (int)(minY * height), (int)((maxX - minX) * width), (int)((maxY - minY) * height));
		}
	}

	public struct Mesh
	{
		public string Name;
		public BeginMode Mode;
		public List<Face> Faces;
		public Texture Texture;

		public Vector3 Translate, Rotate;
		public Vector3 Pivot;

		public bool Helmet;
		public bool FollowCursor;
		public float RotateFactor;
		public Vector3 Center;
		public Bounds3 Bounds;

		public MCSkin3D.VisiblePartFlags Part;
		public bool AllowTransparency;

		public Mesh(string name) :
			this()
		{
			Bounds = new Bounds3(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));
			Name = name;
		}

		internal static string VertexToString(Vector3 v)
		{
			return v.X.ToString(CultureInfo.InvariantCulture) + " " + v.Y.ToString(CultureInfo.InvariantCulture) + " " + v.Z.ToString(CultureInfo.InvariantCulture);
		}

		internal static string VertexToString(Vector2 v)
		{
			return v.X.ToString(CultureInfo.InvariantCulture) + " " + v.Y.ToString(CultureInfo.InvariantCulture);
		}

		internal static Vector3 StringToVertex3(string s)
		{
			var spl = s.Split();

            return new Vector3(float.Parse(spl[0], CultureInfo.InvariantCulture), float.Parse(spl[1], CultureInfo.InvariantCulture), float.Parse(spl[2], CultureInfo.InvariantCulture));
		}

		internal static Vector2 StringToVertex2(string s)
		{
			var spl = s.Split();

            return new Vector2(float.Parse(spl[0], CultureInfo.InvariantCulture), float.Parse(spl[1], CultureInfo.InvariantCulture));
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
			writer.WriteAttributeString("Rotate", VertexToString(Rotate));
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

		public void CalculateCenter()
		{
			int count = 0;
			Bounds = new Bounds3(new Vector3(9999, 9999, 9999), new Vector3(-9999, -9999, -9999));

			foreach (var x in Faces)
			{
				foreach (var v in x.Vertices)
				{
					count++;

					var m =
					//Matrix4.CreateTranslation(-mesh.Pivot) *
			Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotate.X)) *
				Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotate.Y)) *
				Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotate.Z)) *
						//Matrix4.CreateTranslation(mesh.Pivot) *
				Matrix4.CreateTranslation(Translate);

					Center += Vector3.Transform(v, m);
					Bounds += Vector3.Transform(v, m);
				}
			}

			Center /= count;
		}
	}

	public struct Bounds
	{
		Point _mins, _maxs;
		public Point Mins { get { return _mins; } set { _mins = value; } }
		public Point Maxs { get { return _maxs; } set { _maxs = value; } }

		public Bounds(Point mins, Point maxs) :
			this()
		{
			_mins = mins;
			_maxs = maxs;
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

		public Rectangle ToRectangle()
		{
			Rectangle r = new Rectangle();

			r.X = _mins.X;
			r.Y = _mins.Y;
			r.Width = _maxs.X - _mins.X;
			r.Height = _maxs.Y - _mins.Y;

			return r;
		}
	}

	public class Model
	{
		public List<Mesh> Meshes = new List<Mesh>();
		public string Name;
		public float AspectRatio;
		public FileInfo File;

		// P: polygon support required? used bounds 'n stuff but, you know...
		public Rectangle GetTextureFaceBounds(Point p, Skin skin)
		{
			Rectangle b = new Rectangle();

			foreach (var m in Meshes)
			{
				foreach (var f in m.Faces)
				{
					Bounds bounds = new Bounds(new Point(9999, 9999), new Point(-9999, -9999));

					foreach (var c in f.TexCoords)
					{
						var coord = new Vector2(c.X * skin.Width, c.Y * skin.Height);
						bounds.AddPoint(new Point((int)coord.X, (int)coord.Y));
					}

					if (bounds.ToRectangle().Contains(p))
						return bounds.ToRectangle();
				}
			}

			return b;
		}

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
				writer.WriteAttributeString("AspectRatio", AspectRatio.ToString());

				foreach (var mesh in Meshes)
					mesh.Write(writer);

				writer.WriteEndElement();
			}
		}

		static Vector3 TranslateVertex(Vector3 Translate, Vector3 Rotate, Vector3 Pivot, Vector3 v)
		{
			var m =
				//Matrix4.CreateTranslation(-mesh.Pivot) *
				Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotate.X)) *
				Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotate.Y)) *
				Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotate.Z)) *
				//Matrix4.CreateTranslation(mesh.Pivot) *
				Matrix4.CreateTranslation(Translate);

			return Vector3.Transform(v, m);
		}

		public static Model Load(string fileName)
		{
			if (new FileInfo(fileName).Extension == ".xml" &&
				new FileInfo(fileName.Substring(0, fileName.Length - 4) + ".gz.xml").Exists)
				return null;

			Model model = new Model();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.CloseInput = true;

			XmlDocument document = new XmlDocument();
			Stream inStream = null;

			if (fileName.EndsWith(".gz.xml"))
				inStream = new GZipStream(System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);
			else
				inStream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read);

			document.Load(inStream);

			model.Name = document.DocumentElement.Attributes["Name"].InnerText;

			if (document.DocumentElement.Attributes["AspectRatio"] != null)
				model.AspectRatio = float.Parse(document.DocumentElement.Attributes["AspectRatio"].InnerText, CultureInfo.InvariantCulture);

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
				if (n.Attributes["Rotate"] != null)
					mesh.Rotate = Mesh.StringToVertex3(n.Attributes["Rotate"].InnerText);
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

					var zero = TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, face.Vertices[0]);
					var one = TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, face.Vertices[1]);
					var two = TranslateVertex(mesh.Translate, mesh.Rotate, mesh.Pivot, face.Vertices[2]);

					var dir = Vector3.Cross(one - zero, two - zero);
					face.Normal = Vector3.Normalize(dir);

					dir = Vector3.Cross(face.Vertices[1] - face.Vertices[0], face.Vertices[2] - face.Vertices[0]);
					var realNormal = Vector3.Normalize(dir);

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

		public Editor.ModelToolStripMenuItem DropDownItem { get; set; }

		static Font _silkScreen = null;
		static PrivateFontCollection _collection;

		public Bitmap GenerateOverlay(Color lineColor, Color textColor, float aspect, int scale, int lineWidth)
		{
			if (aspect == 0)
				aspect = 1;

			Size baseSize = new Size((int)scale, (int)(scale / aspect));
			if (_silkScreen == null)
			{
				_collection = new PrivateFontCollection();
				byte[] fontdata = MCSkin3D.Properties.Resources.slkscr;
				unsafe
				{
					fixed (byte * pFontData = fontdata)
					{
						_collection.AddMemoryFont((System.IntPtr)pFontData, fontdata.Length);
					}
				}

				_silkScreen = new Font(_collection.Families[0], 6);
			}

			Bitmap b = new Bitmap(baseSize.Width, baseSize.Height);
			var pen = new Pen(lineColor, lineWidth);

			using (Graphics g = Graphics.FromImage(b))
			{
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

				foreach (var x in Meshes)
				{
					foreach (var y in x.Faces)
					{
						Bounds bounds = new Bounds(new Point(9999, 9999), new Point(-9999, -9999));

						foreach (var p in y.TexCoords)
							bounds.AddPoint(new Point((int)(p.X * (b.Width - 1)), (int)(p.Y * (b.Height - 1))));

						var rect = bounds.ToRectangle();
						rect.X += lineWidth - 1;
						rect.Y += lineWidth - 1;
						rect.Width -= lineWidth - 1;
						rect.Height -= lineWidth - 1;
						g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
						g.FillRectangle(new SolidBrush(Color.FromArgb(0, 255, 255, 255)), rect);
						g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

						var polygon = new Point[]
						{
							new Point(rect.X, rect.Y),
							new Point(rect.X + rect.Width, rect.Y),
							new Point(rect.X + rect.Width, rect.Y + rect.Height),
							new Point(rect.X, rect.Y + rect.Height),
						};

						g.DrawPolygon(pen, polygon);

						string side = SideFromNormal(y.Normal);
						string str = x.Name + " " + side;

						rect.X++;
						rect.Y++;

						var measured = g.MeasureString(str, _silkScreen, rect.Size, StringFormat.GenericDefault);
						g.DrawString(str, _silkScreen, new SolidBrush(textColor), rect, StringFormat.GenericDefault);
					}
				}
			}

			return b;
		}

		public static string SideFromNormal(Vector3 vector3)
		{
			Vector3[] vecs = new Vector3[]
			{
				new Vector3(0, 1, 0),
				new Vector3(0, -1, 0),
				new Vector3(1, 0, 0),
				new Vector3(-1, 0, 0),
				new Vector3(0, 0, 1),
				new Vector3(0, 0, -1),
			};

			string[] names = new string[]
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
				var dist = (vector3 - vecs[i]).LengthSquared;

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
		List<Mesh> OpaqueMeshes = new List<Mesh>();
		List<Mesh> TransparentMeshes = new List<Mesh>();

		public void Sort()
		{
			/*OpaqueMeshes.Sort(
				(left, right) =>
				{
					var leftDist = (left.Center - Editor.CameraPosition).LengthFast;
					var rightDist = (right.Center - Editor.CameraPosition).LengthFast;

					return leftDist.CompareTo(rightDist);
				}
			);*/

			TransparentMeshes.Sort(
				(left, right) =>
				{
					var leftDist = (Editor.CameraPosition - left.Center).Length;
					var rightDist = (Editor.CameraPosition - right.Center).Length;

					return rightDist.CompareTo(leftDist);
				}
			);
		}

		public void Render()
		{
			Sort();

			PreRender();

			foreach (var mesh in OpaqueMeshes)
				RenderMesh(mesh);
			GL.Enable(EnableCap.Blend);
			foreach (var mesh in TransparentMeshes)
				RenderMesh(mesh);
			GL.Disable(EnableCap.Blend);
	
			PostRender();

			OpaqueMeshes.Clear();
			TransparentMeshes.Clear();
		}

		public void RenderWithoutTransparency()
		{
			GL.Disable(EnableCap.Blend);

			PreRender();

			foreach (var mesh in OpaqueMeshes)
				RenderMesh(mesh);
			foreach (var mesh in TransparentMeshes)
				RenderMesh(mesh);

			PostRender();

			OpaqueMeshes.Clear();
			TransparentMeshes.Clear();
		}

		public void AddMesh(Mesh mesh)
		{
			if (mesh.Helmet || mesh.Faces[0].Colors[0].A != 1)
				TransparentMeshes.Add(mesh);
			else
				OpaqueMeshes.Add(mesh);
		}

		protected virtual void PreRender() { }
		protected virtual void PostRender() { }
		public abstract void RenderMesh(Mesh mesh);
	}
}
