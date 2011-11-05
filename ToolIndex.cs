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
		public Control OptionsPanel;

		public ToolIndex(ITool tool, Control options, string name, Image image, Keys defaultKey)
		{
			Name = name;
			DefaultKeys = defaultKey;

			OptionsPanel = options;
			Tool = tool;
			MenuItem = new ToolStripMenuItem(Name, image);
			MenuItem.Tag = this;
			Button = new ToolStripButton(image);
			Button.ToolTipText = name;
			Button.Tag = this;
		}
	}
}
