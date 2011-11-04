using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;

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
			RenderState.BindTexture(mesh.Texture);

			GL.PushMatrix();

			GL.Translate(mesh.Pivot);
			GL.Rotate(mesh.Rotate.X, 1, 0, 0);
			GL.Rotate(mesh.Rotate.Y, 0, 1, 0);
			GL.Rotate(mesh.Rotate.Z, 0, 0, 1);
			GL.Translate(-mesh.Pivot);

			GL.Translate(mesh.Translate);

			GL.Begin(mesh.Mode);
			foreach (var face in mesh.Faces)
			{
				foreach (var index in face.Indices)
				{
					GL.Color4(face.Colors[index]);
					GL.TexCoord2(face.TexCoords[index]);
					GL.Vertex3(face.Vertices[index]);
				}
			}
			GL.End();

			GL.PopMatrix();
		}
	}
}
