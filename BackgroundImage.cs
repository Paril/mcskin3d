using System;

namespace MCSkin3D
{
	class BackgroundImage
	{
		public int GLImage;
		public string Name;
		public  System.Windows.Forms.ToolStripMenuItem Item;

		public BackgroundImage(string name, int image)
		{
			Name = name;
			GLImage = image;
		}
	}
}
