using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paril.Drawing.Filters
{
	public class GaussianBlurFilter : MatrixFilter
	{
		public override NMatrix Matrix
		{
			get
			{
				NMatrix m = new NMatrix(3, 3);
				m.SetAll(1);
				m[1, 1] = Weight;
				m[1, 0] = m[0, 1] = m[2, 0] = m[0, 2] = 2;
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
