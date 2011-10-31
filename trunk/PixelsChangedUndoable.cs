using System;
using Paril.Components;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace MCSkin3D
{
	public class PixelsChangedUndoable : IUndoable
	{
		public Color NewColor;
		public Dictionary<Point, Color> Points = new Dictionary<Point, Color>();

		public void Undo(object obj)
		{
			Skin skin = (Skin)obj;

			GL.BindTexture(TextureTarget.Texture2D, GlobalDirtiness.CurrentSkin);
			int[] array = new int[64 * 32];
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

			foreach (var kvp in Points)
			{
				var p = kvp.Key;
				var color = kvp.Value;
				array[p.X + (64 * p.Y)] = color.R | (color.G << 8) | (color.B << 16) | (color.A << 24);
			}

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
		}

		public void Redo(object obj)
		{
			Skin skin = (Skin)obj;

			GL.BindTexture(TextureTarget.Texture2D, GlobalDirtiness.CurrentSkin);
			int[] array = new int[64 * 32];
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);

			foreach (var p in Points.Keys)
				array[p.X + (64 * p.Y)] = NewColor.R | (NewColor.G << 8) | (NewColor.B << 16) | (NewColor.A << 24);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 64, 32, 0, PixelFormat.Rgba, PixelType.UnsignedByte, array);
		}
	}
}
