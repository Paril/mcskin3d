using System.Drawing;

namespace MCSkin3D.Swatches
{
	public class NamedColor
	{
		public NamedColor(string name, Color color)
		{
			Name = name;
			Color = color;
		}

		public string Name { get; set; }
		public Color Color { get; set; }
	}
}