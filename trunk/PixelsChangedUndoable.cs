using System;
using Paril.Components;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Paril.Compatibility;
using Paril.OpenGL;

namespace MCSkin3D
{
	public class PixelsChangedUndoable : IUndoable
	{
		public Dictionary<Point, Tuple<Color, Color>> Points = new Dictionary<Point, Tuple<Color, Color>>();

		public void Undo(object obj)
		{
			Skin skin = (Skin)obj;

			RenderState.BindTexture(GlobalDirtiness.CurrentSkin);
			int[] array = new int[skin.Width * skin.Height];
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

			foreach (var kvp in Points)
			{
				var p = kvp.Key;
				var color = kvp.Value;
				array[p.X + (skin.Width * p.Y)] = color.Item1.R | (color.Item1.G << 8) | (color.Item1.B << 16) | (color.Item1.A << 24);
			}

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, skin.Width, skin.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
		}

		public void Redo(object obj)
		{
			Skin skin = (Skin)obj;

			RenderState.BindTexture(GlobalDirtiness.CurrentSkin);
			int[] array = new int[skin.Width * skin.Height];
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

			foreach (var kvp in Points)
			{
				var p = kvp.Key;
				var color = kvp.Value;
				array[p.X + (skin.Width * p.Y)] = color.Item2.R | (color.Item2.G << 8) | (color.Item2.B << 16) | (color.Item2.A << 24);
			}

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, skin.Width, skin.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
		}
	}
}
