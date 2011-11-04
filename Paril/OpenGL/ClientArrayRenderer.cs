using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;

namespace Paril.OpenGL
{
	/// <summary>
	/// OpenGL renderer that renders using client-side array mode.
	/// 
	/// </summary>
	public class ClientArrayRenderer : Renderer
	{
		bool _compiled = false;

		public ClientArrayRenderer()
		{
			if (GL.GetString(StringName.Extensions).Contains("GL_EXT_compiled_vertex_array"))
				_compiled = true;
		}

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

			foreach (var face in mesh.Faces)
			{
				GL.VertexPointer(3, VertexPointerType.Float, 0, face.Vertices);
				GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, face.TexCoords);
				GL.ColorPointer(4, ColorPointerType.Float, 0, face.Colors);

				if (_compiled)
					GL.Ext.LockArrays(0, face.Indices.Length);
				GL.DrawElements(mesh.Mode, face.Indices.Length, DrawElementsType.UnsignedInt, face.Indices);
				if (_compiled)
					GL.Ext.UnlockArrays();
			}

			GL.PopMatrix();
		}

		protected override void PreRender()
		{
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
		}

		protected override void PostRender()
		{
			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.ColorArray);
			GL.DisableClientState(ArrayCap.TextureCoordArray);
		}
	}
}
