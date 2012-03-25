using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MCSkin3D.Swatches
{
	public class NamedColor
	{
		public string Name { get; set; }
		public Color Color { get; set; }

		public NamedColor(string name, Color color)
		{
			Name = name;
			Color = color;
		}
	}
}
