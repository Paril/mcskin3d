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

using System;

namespace MCSkin3D
{
	public partial class FloodFillOptions : ToolOptionBase
	{
		private bool _skipSet;

		public FloodFillOptions()
		{
			InitializeComponent();
		}

		private void DodgeBurnOptions_Load(object sender, EventArgs e)
		{
			SetThreshold(GlobalSettings.FloodFillThreshold);
		}

		private void SetThreshold(float f)
		{
			_skipSet = true;
			numericUpDown1.Value = (decimal) (f * 100.0f);
			trackBar1.Value = (int) (f * 100.0f);
			_skipSet = false;

			GlobalSettings.FloodFillThreshold = f;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			SetThreshold((float) numericUpDown1.Value / 100.0f);
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			SetThreshold(trackBar1.Value / 100.0f);
		}
	}
}