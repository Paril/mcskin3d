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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paril.Drawing.Filters
{
	public class GaussianBlurFilter : MatrixFilter<float>
	{
		public override NMatrix Matrix
		{
			get
			{
				NMatrix m = new NMatrix(3, 3);
				m.SetAll(1);
				m[1, 1] = Weight;
				m[0, 1] = m[2, 1] = m[1, 0] = m[1, 2] = 2;
				m.Factor = Weight + 12;

				return m;
			}
		}

		public float Weight
		{
			get;
			set;
		}

		public GaussianBlurFilter(float weight)
		{
			Weight = weight;
		}
	}
}
