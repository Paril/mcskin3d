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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Paril.Controls
{
	public class VisibleSplitContainer : SplitContainer
	{
		int _numGripBumps = 5;
		[DefaultValue(5)]
		public int NumGripBumps
		{
			get { return _numGripBumps; }
			set { _numGripBumps = value; Invalidate(); }
		}

		void DrawBump(Graphics g, int x, int y)
		{
			g.FillRectangle(Brushes.Gray, x + 1, y + 1, 2, 2);
			g.FillRectangle(Brushes.White, x, y, 2, 2);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (!IsSplitterFixed)
			{
				if (Orientation == System.Windows.Forms.Orientation.Horizontal)
					for (int i = 0, x = (Width / 2) - ((_numGripBumps * 3) / 2); i < _numGripBumps; ++i, x += (3 + 1))
						DrawBump(e.Graphics, x, SplitterDistance);
				else
					for (int i = 0, y = (Height / 2) - ((_numGripBumps * 3) / 2); i < _numGripBumps; ++i, y += (3 + 1))
						DrawBump(e.Graphics, SplitterDistance, y);
			}

			base.OnPaint(e);
		}
	}
}
