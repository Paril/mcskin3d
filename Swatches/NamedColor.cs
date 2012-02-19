using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MCSkin3D.Swatches
{
	public struct NamedColor
	{
		public string Name { get; set; }
		public Color Color { get; set; }

		public NamedColor(string name, Color color) :
			this()
		{
			Name = name;
			Color = color;
		}
	}
}
