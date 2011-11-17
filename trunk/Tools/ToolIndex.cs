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

		public void SetMeAsTool()
		{
			Program.MainForm.SetSelectedTool(this);
		}
	}
}
