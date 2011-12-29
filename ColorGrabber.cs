using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace MCSkin3D
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ColorPixel
	{
		[FieldOffset(0)]
		int _rgba;

		[FieldOffset(0)]
		byte _r;
		[FieldOffset(1)]
		byte _g;
		[FieldOffset(2)]
		byte _b;
		[FieldOffset(3)]
		byte _a;

		public ColorPixel(int rgba)
		{
			_r = _g = _b = _a = 0;
			_rgba = rgba;
		}

		public ColorPixel(byte r, byte g, byte b, byte a)
		{
			_rgba = 0;
			_r = r;
			_g = g;
			_b = b;
			_a = a;
		}

		public byte Red { get { return _r; } set { _r = value; } }
		public byte Green { get { return _g; } set { _g = value; } }
		public byte Blue { get { return _b; } set { _b = value; } }
		public byte Alpha { get { return _a; } set { _a = value; } }

		public int RGBA { get { return _rgba; } set { _rgba = value; } }
	}

	public interface IColorGrabber<T>
	{
		T Texture { get; set; }
		int Width { get; }
		int Height { get; }

		void Resize(int width, int height);
		void Load();
		void Save();

		ColorPixel this[int x, int y] { get; set; }
		ColorPixel[] Array { get; set; }
	}

	public struct ColorGrabber : IColorGrabber<int>
	{
		int _texture;
		ColorPixel[] _array;
		int _width, _height;

		public int Texture { get { return _texture; } set { _texture = value; } }
		public int Width { get { return _width; } }
		public int Height { get { return _height; } }

		public bool Valid
		{
			get { return _array != null; }
		}

		public ColorGrabber(int texture, int width, int height)
		{
			_texture = texture;
			_width = width;
			_height = height;
			_array = null;

			Resize(width, height);
		}

		public void Resize(int width, int height)
		{
			_array = new ColorPixel[width * height];
			_width = width;
			_height = height;
		}

		public void Load()
		{
			GL.BindTexture(TextureTarget.Texture2D, _texture);
			GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _array);
		}

		public void Save()
		{
			GL.BindTexture(TextureTarget.Texture2D, _texture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _array);
		}

		public ColorPixel this[int x, int y]
		{
			get { return _array[x + (y * _width)]; }
			set { _array[x + (y * _width)] = value; }
		}

		public ColorPixel[] Array
		{
			get { return _array; }
			set
			{
				value.CopyTo(_array, 0);
			}
		}
	}
}
