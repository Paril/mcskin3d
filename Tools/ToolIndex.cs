using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace MCSkin3D
{
	public class ToolIndex
	{
		public string Name;
		public Keys DefaultKeys;
		public ToolStripMenuItem MenuItem;
		public ToolStripButton Button;
		public ITool Tool;
		public ToolOptionBase OptionsPanel;

		public ToolIndex(ITool tool, ToolOptionBase options, string name, Image image, Keys defaultKey)
		{
			Name = name;
			DefaultKeys = defaultKey;

			OptionsPanel = options;
			Tool = tool;
			MenuItem = new ToolStripMenuItem(Name, image);
			MenuItem.Text = name;
			MenuItem.Tag = this;
			Button = new ToolStripButton(image);
			Button.Text = name;
			Button.DisplayStyle = ToolStripItemDisplayStyle.Image;
			Button.Tag = this;
		}
	}
}
