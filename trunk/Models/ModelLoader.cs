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

		public static void CompileHumanModel()
		{
			Mesh HeadMesh;
			Mesh HelmetMesh;
			Mesh ChestMesh;
			Mesh RightLegMesh;
			Mesh LeftLegMesh;
			Mesh RightArmMesh;
			Mesh LeftArmMesh;
			Model HumanModel;

			var box = CreateBox(8);
			var allWhite = new Color4[] { Color4.White, Color4.White, Color4.White, Color4.White };
			var cw = new int[] { 0, 1, 2, 3 };
			var ccw = new int[] { 3, 2, 1, 0 };

			var frontFace = new Face(GetFace(FaceLocation.Front, box), TexCoordBox(8, 8, 8, 8), allWhite, cw);
			var topFace = new Face(GetFace(FaceLocation.Top, box), TexCoordBox(8, 0, 8, 8), allWhite, cw);
			var bottomFace = new Face(GetFace(FaceLocation.Bottom, box), TexCoordBoxPrecise(24, 0, -8, 8, 3, 2, 1, 0), allWhite, cw);
			var backFace = new Face(GetFace(FaceLocation.Back, box), TexCoordBoxPrecise(32, 8, -8, 8, 3, 2, 1, 0), allWhite, cw);
			var leftFace = new Face(GetFace(FaceLocation.Left, box), TexCoordBoxPrecise(0, 8, 8, 8, 0, 1, 2, 3), allWhite, cw);
			var rightFace = new Face(GetFace(FaceLocation.Right, box), TexCoordBox(16, 8, 8, 8), allWhite, cw);

			HeadMesh = new Mesh("Head");
			HeadMesh.Mode = BeginMode.Quads;
			HeadMesh.Faces = new List<Face>(new Face[] { frontFace, topFace, bottomFace, backFace, leftFace, rightFace });
			HeadMesh.Translate = new Vector3(0, 10, 0);
			HeadMesh.Pivot = new Vector3(0, 4, 0);
			HeadMesh.FollowCursor = true;
			HeadMesh.Part = VisiblePartFlags.HeadFlag;

			box = CreateBox(9.0f);

			frontFace.Vertices = GetFace(FaceLocation.Front, box);
			frontFace.TexCoords = TexCoordBox(32 + 8, 8, 8, 8);

			topFace.Vertices = GetFace(FaceLocation.Top, box);
			topFace.TexCoords = TexCoordBox(32 + 8, 0, 8, 8);

			bottomFace.Vertices = GetFace(FaceLocation.Bottom, box);
			bottomFace.TexCoords = TexCoordBoxPrecise(32 + 24, 0, -8, 8, 3, 2, 1, 0);

			backFace.Vertices = GetFace(FaceLocation.Back, box);
			backFace.TexCoords = TexCoordBoxPrecise(32 + 32, 8, -8, 8, 3, 2, 1, 0);

			leftFace.Vertices = GetFace(FaceLocation.Left, box);
			leftFace.TexCoords = TexCoordBoxPrecise(32 + 0, 8, 8, 8, 0, 1, 2, 3);

			rightFace.Vertices = GetFace(FaceLocation.Right, box);
			rightFace.TexCoords = TexCoordBox(32 + 16, 8, 8, 8);

			HelmetMesh = new Mesh("Helmet");
			HelmetMesh.Mode = BeginMode.Quads;
			frontFace.Indices =
			topFace.Indices =
			bottomFace.Indices =
			backFace.Indices =
			leftFace.Indices =
			rightFace.Indices = ccw;

			HelmetMesh.Faces = new List<Face>(new Face[] { frontFace, topFace, bottomFace, backFace, leftFace, rightFace });

			frontFace.Indices =
			topFace.Indices =
			bottomFace.Indices =
			backFace.Indices =
			leftFace.Indices =
			rightFace.Indices = cw;

			HelmetMesh.Faces.AddRange(new Face[] { frontFace, topFace, bottomFace, backFace, leftFace, rightFace });

			HelmetMesh.Translate = new Vector3(0, 10, 0);
			HelmetMesh.Pivot = new Vector3(0, 4, 0);
			HelmetMesh.FollowCursor = true;
			HelmetMesh.Helmet = true;
			HelmetMesh.Part = VisiblePartFlags.HelmetFlag;

			frontFace.Indices =
			topFace.Indices =
			bottomFace.Indices =
			backFace.Indices =
			leftFace.Indices =
			rightFace.Indices = cw;

			box = CreateBox(8, 12, 4);

			frontFace.Vertices = GetFace(FaceLocation.Front, box);
			frontFace.TexCoords = TexCoordBox(20, 20, 8, 12);

			topFace.Vertices = GetFace(FaceLocation.Top, box);
			topFace.TexCoords = TexCoordBox(20, 16, 8, 4);

			bottomFace.Vertices = GetFace(FaceLocation.Bottom, box);
			bottomFace.TexCoords = TexCoordBoxPrecise(36, 16, -8, 4, 3, 2, 1, 0);

			backFace.Vertices = GetFace(FaceLocation.Back, box);
			backFace.TexCoords = TexCoordBoxPrecise(40, 20, -8, 12, 3, 2, 1, 0);

			leftFace.Vertices = GetFace(FaceLocation.Left, box);
			leftFace.TexCoords = TexCoordBox(16, 20, 4, 12);

			rightFace.Vertices = GetFace(FaceLocation.Right, box);
			rightFace.TexCoords = TexCoordBox(28, 20, 4, 12);

			ChestMesh = new Mesh("Chest");
			ChestMesh.Mode = BeginMode.Quads;
			ChestMesh.Faces = new List<Face>(new Face[] { frontFace, topFace, bottomFace, backFace, leftFace, rightFace });
			ChestMesh.Translate = new Vector3(0, 0, 0);
			ChestMesh.Part = VisiblePartFlags.ChestFlag;

			box = CreateBox(4, 12, 4);

			frontFace.Vertices = GetFace(FaceLocation.Front, box);
			frontFace.TexCoords = TexCoordBox(4, 20, 4, 12);

			topFace.Vertices = GetFace(FaceLocation.Top, box);
			topFace.TexCoords = TexCoordBox(4, 16, 4, 4);

			bottomFace.Vertices = GetFace(FaceLocation.Bottom, box);
			bottomFace.TexCoords = TexCoordBoxPrecise(12, 16, -4, 4, 3, 2, 1, 0);

			backFace.Vertices = GetFace(FaceLocation.Back, box);
			backFace.TexCoords = TexCoordBoxPrecise(16, 20, -4, 12, 3, 2, 1, 0);

			leftFace.Vertices = GetFace(FaceLocation.Left, box);
			leftFace.TexCoords = TexCoordBox(0, 20, 4, 12);

			rightFace.Vertices = GetFace(FaceLocation.Right, box);
			rightFace.TexCoords = TexCoordBox(8, 20, 4, 12);

			RightLegMesh = new Mesh("Right Leg");
			RightLegMesh.Mode = BeginMode.Quads;
			RightLegMesh.Faces = new List<Face>(new Face[] { frontFace, topFace, bottomFace, backFace, leftFace, rightFace });
			RightLegMesh.Translate = new Vector3(-2, -12, 0);
			RightLegMesh.Pivot = new Vector3(0, -6, 0);
			RightLegMesh.RotateFactor = -37;
			RightLegMesh.Part = VisiblePartFlags.RightLegFlag;

			frontFace.Vertices = GetFace(FaceLocation.Front, box);
			frontFace.TexCoords = TexCoordBox(8, 20, -4, 12);

			topFace.Vertices = GetFace(FaceLocation.Top, box);
			topFace.TexCoords = TexCoordBox(8, 16, -4, 4);

			bottomFace.Vertices = GetFace(FaceLocation.Bottom, box);
			bottomFace.TexCoords = TexCoordBoxPrecise(8, 16, 4, 4, 3, 2, 1, 0);

			backFace.Vertices = GetFace(FaceLocation.Back, box);
			backFace.TexCoords = TexCoordBoxPrecise(12, 20, 4, 12, 3, 2, 1, 0);

			leftFace.Vertices = GetFace(FaceLocation.Left, box);
			leftFace.TexCoords = TexCoordBox(12, 20, -4, 12);

			rightFace.Vertices = GetFace(FaceLocation.Right, box);
			rightFace.TexCoords = TexCoordBox(4, 20, -4, 12);

			LeftLegMesh = new Mesh("Left Leg");
			LeftLegMesh.Mode = BeginMode.Quads;
			LeftLegMesh.Faces = new List<Face>(new Face[] { frontFace, topFace, bottomFace, backFace, leftFace, rightFace });
			LeftLegMesh.Translate = new Vector3(2, -12, 0);
			LeftLegMesh.Pivot = new Vector3(0, -6, 0);
			LeftLegMesh.RotateFactor = 37;
			LeftLegMesh.Part = VisiblePartFlags.LeftLegFlag;

			frontFace.Vertices = GetFace(FaceLocation.Front, box);
			frontFace.TexCoords = TexCoordBox(44, 20, 4, 12);

			topFace.Vertices = GetFace(FaceLocation.Top, box);
			topFace.TexCoords = TexCoordBox(44, 16, 4, 4);

			bottomFace.Vertices = GetFace(FaceLocation.Bottom, box);
			bottomFace.TexCoords = TexCoordBoxPrecise(52, 16, -4, 4, 3, 2, 1, 0);

			backFace.Vertices = GetFace(FaceLocation.Back, box);
			backFace.TexCoords = TexCoordBoxPrecise(56, 20, -4, 12, 3, 2, 1, 0);

			leftFace.Vertices = GetFace(FaceLocation.Left, box);
			leftFace.TexCoords = TexCoordBox(40, 20, 4, 12);

			rightFace.Vertices = GetFace(FaceLocation.Right, box);
			rightFace.TexCoords = TexCoordBox(48, 20, 4, 12);

			RightArmMesh = new Mesh("Right Arm");
			RightArmMesh.Mode = BeginMode.Quads;
			RightArmMesh.Faces = new List<Face>(new Face[] { frontFace, topFace, bottomFace, backFace, leftFace, rightFace });
			RightArmMesh.Translate = new Vector3(-6, 0, 0);
			RightArmMesh.Pivot = new Vector3(0, 5, 0);
			RightArmMesh.RotateFactor = 37;
			RightArmMesh.Part = VisiblePartFlags.RightArmFlag;

			frontFace.Vertices = GetFace(FaceLocation.Front, box);
			frontFace.TexCoords = TexCoordBox(48, 20, -4, 12);

			topFace.Vertices = GetFace(FaceLocation.Top, box);
			topFace.TexCoords = TexCoordBox(48, 16, -4, 4);

			bottomFace.Vertices = GetFace(FaceLocation.Bottom, box);
			bottomFace.TexCoords = TexCoordBoxPrecise(48, 16, 4, 4, 3, 2, 1, 0);

			backFace.Vertices = GetFace(FaceLocation.Back, box);
			backFace.TexCoords = TexCoordBoxPrecise(52, 20, 4, 12, 3, 2, 1, 0);

			leftFace.Vertices = GetFace(FaceLocation.Left, box);
			leftFace.TexCoords = TexCoordBox(52, 20, -4, 12);

			rightFace.Vertices = GetFace(FaceLocation.Right, box);
			rightFace.TexCoords = TexCoordBox(44, 20, -4, 12);

			LeftArmMesh = new Mesh("Left Arm");
			LeftArmMesh.Mode = BeginMode.Quads;
			LeftArmMesh.Faces = new List<Face>(new Face[] { frontFace, topFace, bottomFace, backFace, leftFace, rightFace });
			LeftArmMesh.Translate = new Vector3(6, 0, 0);
			LeftArmMesh.Pivot = new Vector3(0, 5, 0);
			LeftArmMesh.RotateFactor = -37;
			LeftArmMesh.Part = VisiblePartFlags.LeftArmFlag;

			HumanModel = new Model();
			HumanModel.Name = "Human";
			HumanModel.Meshes.Add(HeadMesh);
			HumanModel.Meshes.Add(ChestMesh);
			HumanModel.Meshes.Add(RightArmMesh);
			HumanModel.Meshes.Add(LeftArmMesh);
			HumanModel.Meshes.Add(RightLegMesh);
			HumanModel.Meshes.Add(LeftLegMesh);
			HumanModel.Meshes.Add(HelmetMesh);

			HumanModel.Save("Models\\human.xml");
		}

		public static void LoadModels()
		{
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
