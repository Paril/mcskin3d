using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paril.Drawing.Filters
{
	public struct NMatrix
	{
		float[,] _matrix;
		float _factor;
		float _offset;

		public NMatrix(int width, int height) :
			this()
		{
			_matrix = new float[width, height];

			_factor = 1;
			_offset = 0;
			_matrix[(width - 1) / 2, (height - 1) / 2] = 1;
		}

		public NMatrix(int size) :
			this(size, size)
		{
		}

		public float this[int x, int y]
		{
			get { return _matrix[x, y]; }
			set { _matrix[x, y] = value; }
		}

		public float[,] Matrix
		{
			get { return _matrix; }
		}

		public float Factor { get { return _factor; } set { _factor = value; } }
		public float Offset { get { return _offset; } set { _offset = value; } }

		public void SetAll(float p)
		{
			for (int y = 0; y < Matrix.GetLength(0); ++y)
				for (int x = 0; x < Matrix.GetLength(1); ++x)
					_matrix[x, y] = p;
		}
	}
}
