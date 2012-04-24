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
using System.Drawing;

namespace MCSkin3D
{
	public partial class EraserOptions : ToolOptionBase
	{
		public EraserOptions()
		{
			InitializeComponent();
		}

		public override void BoxShown()
		{
			groupBox1.Controls.Add(Brushes.BrushBox);
			Brushes.BrushBox.Location = new Point(9, 19);
		}

		public override void BoxHidden()
		{
			groupBox1.Controls.Remove(Brushes.BrushBox);
		}

		private void EraserOptions_Load(object sender, EventArgs e)
		{
		}
	}
}