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

namespace Paril.Drawing.Filters
{
	public struct NMatrix
	{
		private readonly float[,] _matrix;
		private float _factor;
		private float _offset;

		public NMatrix(int width, int height) :
			this()
		{
			_matrix = new float[width,height];

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

		public float Factor
		{
			get { return _factor; }
			set { _factor = value; }
		}

		public float Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}

		public void SetAll(float p)
		{
			for (int y = 0; y < Matrix.GetLength(0); ++y)
			{
				for (int x = 0; x < Matrix.GetLength(1); ++x)
					_matrix[x, y] = p;
			}
		}
	}
}