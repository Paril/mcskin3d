//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
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

using System.Drawing;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class ToolIndex
	{
		public ToolStripButton Button;
		public Keys DefaultKeys;
		public ToolStripMenuItem MenuItem;
		public string Name;
		public ToolOptionBase OptionsPanel;
		public ITool Tool;

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
			Editor.MainForm.SetSelectedTool(this);
		}
	}
}