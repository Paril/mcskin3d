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

using OpenTK.Graphics.OpenGL;

namespace Paril.OpenGL
{
	/// <summary>
	/// OpenGL renderer that renders using immediate mode.
	/// 
	/// </summary>
	public class ImmediateRenderer : Renderer
	{
		public override void RenderMesh(Mesh mesh)
		{
			mesh.Texture.Bind();

			GL.PushMatrix();

			GL.MultMatrix(ref mesh.Matrix);

			if (mesh.DrawTransparent)
				GL.Color4((byte)255, (byte)255, (byte)255, (byte)63);
			else
				GL.Color4((byte)255, (byte)255, (byte)255, (byte)255);

			GL.Begin(mesh.Mode);
			foreach (Face face in mesh.Faces)
			{
				foreach (int index in face.Indices)
				{
					GL.TexCoord2(face.TexCoords[index]);
					GL.Vertex3(face.Vertices[index]);
				}
			}
			GL.End();

			GL.Color4((byte)255, (byte)255, (byte)255, (byte)255);

			GL.PopMatrix();
		}
	}
}