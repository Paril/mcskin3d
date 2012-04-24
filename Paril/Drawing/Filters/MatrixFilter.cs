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

namespace Paril.Drawing.Filters
{
	public interface IMatrixOperand<T>
	{
		int Width { get; }
		int Height { get; }
		T EmptyValue();
		T Multiply(T left, T right);
		T Multiply(T left, float right);
		T Divide(T left, float right);
		T Add(T left, float right);
		T Add(T left, T right);

		T ValueAt(int x, int y);
		void SetValueAt(int x, int y, T value);
	}

	public class FloatMatrixOperand : IMatrixOperand<float>
	{
		public float[,] Value;

		#region IMatrixOperand<float> Members

		public int Width
		{
			get { return Value.GetLength(0); }
		}

		public int Height
		{
			get { return Value.GetLength(1); }
		}

		public float EmptyValue()
		{
			return 0;
		}

		public float ValueAt(int x, int y)
		{
			return Value[x, y];
		}

		public void SetValueAt(int x, int y, float value)
		{
			Value[x, y] = value;
		}

		public float Multiply(float left, float right)
		{
			return left * right;
		}

		public float Divide(float left, float right)
		{
			return left / right;
		}

		public float Add(float left, float right)
		{
			return left + right;
		}

		#endregion

		public float Clone(float value)
		{
			return value;
		}
	}

	public abstract class MatrixFilter<T>
	{
		public abstract NMatrix Matrix { get; }

		public void Apply(IMatrixOperand<T> b)
		{
			// Avoid divide by zero errors
			if (Matrix.Factor == 0)
				return;

			var clone = new T[b.Width,b.Height];

			for (int y = 0; y < b.Height; ++y)
			{
				for (int x = 0; x < b.Width; ++x)
					clone[x, y] = b.ValueAt(x, y);
			}

			for (int y = Matrix.Matrix.GetLength(1) - 2; y < b.Height - (Matrix.Matrix.GetLength(1) - 1) / 2; ++y)
			{
				for (int x = Matrix.Matrix.GetLength(0) - 2; x < b.Width - (Matrix.Matrix.GetLength(0) - 1) / 2; ++x)
				{
					T valueHere = b.EmptyValue();

					for (int my = 0; my < Matrix.Matrix.GetLength(1); ++my)
					{
						for (int mx = 0; mx < Matrix.Matrix.GetLength(0); ++mx)
						{
							int suby = (Matrix.Matrix.GetLength(1) - 1) / 2;
							int subx = (Matrix.Matrix.GetLength(0) - 1) / 2;

							valueHere = b.Add(valueHere, b.Multiply(clone[x - (subx - mx), y - (suby - my)], Matrix[mx, my]));
						}
					}

					valueHere = b.Add(b.Divide(valueHere, Matrix.Factor), Matrix.Offset);

					b.SetValueAt(x, y, valueHere);
				}
			}
		}
	}
}