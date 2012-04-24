using System.Drawing;
using System.Runtime.InteropServices;

namespace Paril.OpenGL
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ColorPixel
	{
		[FieldOffset(0)] private int _rgba;

		[FieldOffset(0)] private byte _r;
		[FieldOffset(1)] private byte _g;
		[FieldOffset(2)] private byte _b;
		[FieldOffset(3)] private byte _a;

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

		public byte Red
		{
			get { return _r; }
			set { _r = value; }
		}

		public byte Green
		{
			get { return _g; }
			set { _g = value; }
		}

		public byte Blue
		{
			get { return _b; }
			set { _b = value; }
		}

		public byte Alpha
		{
			get { return _a; }
			set { _a = value; }
		}

		public int RGBA
		{
			get { return _rgba; }
			set { _rgba = value; }
		}
	}

	public interface IColorGrabber<T>
	{
		T Texture { get; set; }
		int Width { get; }
		int Height { get; }
		ColorPixel this[int x, int y] { get; set; }
		ColorPixel[] Array { get; set; }

		void Resize(int width, int height);
		void Load();
		void Save();
	}

	public struct ColorGrabber : IColorGrabber<Texture>
	{
		#region Delegates

		public delegate void OnWriteDelegate(Point p, ColorPixel c);

		#endregion

		public OnWriteDelegate OnWrite;
		private ColorPixel[] _array;
		private int _height;
		private Texture _texture;
		private int _width;

		public ColorGrabber(Texture texture, int width, int height)
		{
			OnWrite = null;
			_texture = texture;
			_width = width;
			_height = height;
			_array = null;

			Resize(width, height);
		}

		public bool Valid
		{
			get { return _array != null; }
		}

		#region IColorGrabber<Texture> Members

		public Texture Texture
		{
			get { return _texture; }
			set { _texture = value; }
		}

		public int Width
		{
			get { return _width; }
		}

		public int Height
		{
			get { return _height; }
		}

		public void Resize(int width, int height)
		{
			_array = new ColorPixel[width * height];
			_width = width;
			_height = height;
		}

		public void Load()
		{
			if (_texture == null)
				return;

			_texture.Get(_array);
		}

		public void Save()
		{
			if (_texture == null)
				return;

			_texture.Upload(_array, _width, _height);
		}

		public ColorPixel this[int x, int y]
		{
			get { return _array[x + (y * _width)]; }
			set
			{
				_array[x + (y * _width)] = value;
				if (OnWrite != null) OnWrite(new Point(x, y), value);
			}
		}

		public ColorPixel[] Array
		{
			get { return _array; }
			set { value.CopyTo(_array, 0); }
		}

		#endregion
	}
}