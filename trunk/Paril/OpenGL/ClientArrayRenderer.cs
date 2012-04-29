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

using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System.Collections.Generic;
using System;

namespace Paril.OpenGL
{
	/// <summary>
	/// OpenGL renderer that renders using client-side array mode.
	/// 
	/// </summary>
	public class ClientArrayRenderer : Renderer
	{
		public ClientArrayRenderer()
		{
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
		}

		public override void RenderMesh(Mesh mesh)
		{
			mesh.Texture.Bind();

			GL.PushMatrix();

			GL.MultMatrix(ref mesh.Matrix);

			var data = mesh.GetUserData<ClientArrayMeshUserData>();

			GL.VertexPointer(3, VertexPointerType.Float, 0, data.VerticeArray);
			GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, data.TexCoordArray);
			GL.ColorPointer(4, ColorPointerType.Float, 0, data.ColorArray);

			GL.DrawElements(mesh.Mode, data.IndiceArray.Length, DrawElementsType.UnsignedByte, data.IndiceArray);

			GL.PopMatrix();
		}

		protected override void PreRender()
		{
		}

		protected override void PostRender()
		{
		}

		public override IMeshUserData CreateUserData(Mesh mesh)
		{
			ClientArrayMeshUserData data = new ClientArrayMeshUserData();

			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> texCoords = new List<Vector2>();
			List<byte> indices = new List<byte>();

			int totalCount = 0;
			foreach (var f in mesh.Faces)
			{
				vertices.AddRange(f.Vertices);
				texCoords.AddRange(f.TexCoords);

				foreach (var c in f.Indices)
					indices.Add((byte)(c + totalCount));

				totalCount += f.Vertices.Length;
			}

			data.VerticeArray = new float[vertices.Count * 3];
			data.TexCoordArray = new float[texCoords.Count * 2];
			data.IndiceArray = indices.ToArray();

			int vi = 0;
			foreach (var x in vertices)
			{
				data.VerticeArray[vi++] = x.X;
				data.VerticeArray[vi++] = x.Y;
				data.VerticeArray[vi++] = x.Z;
			}

			vi = 0;
			foreach (var x in texCoords)
			{
				data.TexCoordArray[vi++] = x.X;
				data.TexCoordArray[vi++] = x.Y;
			}

			return data;
		}

		public override void UpdateUserData(Mesh mesh)
		{
			List<Color4> colors = new List<Color4>();

			Color4 color = new Color4(1, 1, 1, mesh.DrawTransparent ? 0.25f : 1.0f); 
	
			foreach (var x in mesh.Faces)
				colors.AddRange(new Color4[] { color, color, color, color });

			ClientArrayMeshUserData data = mesh.GetUserData<ClientArrayMeshUserData>();

			data.ColorArray = new float[colors.Count * 4];

			int vi = 0;
			foreach (var x in colors)
			{
				data.ColorArray[vi++] = x.R;
				data.ColorArray[vi++] = x.G;
				data.ColorArray[vi++] = x.B;
				data.ColorArray[vi++] = x.A;
			}
		}
	}
}