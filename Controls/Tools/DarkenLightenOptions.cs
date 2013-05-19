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
using System.Drawing;

namespace MCSkin3D
{
	public partial class DarkenLightenOptions : ToolOptionBase
	{
		private bool _skipSet;

		public DarkenLightenOptions()
		{
			InitializeComponent();
		}

		public bool Inverted
		{
			get { return radioButton2.Checked; }
		}

		private void DodgeBurnOptions_Load(object sender, EventArgs e)
		{
			checkBox1.Checked = GlobalSettings.DarkenLightenIncremental;
			SetExposure(GlobalSettings.DarkenLightenExposure);
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			GlobalSettings.DarkenLightenIncremental = checkBox1.Checked;
		}

		private void SetExposure(float f)
		{
			_skipSet = true;
			numericUpDown1.Value = (decimal) (f * 100.0f);
			trackBar1.Value = (int) (f * 100.0f);
			_skipSet = false;

			GlobalSettings.DarkenLightenExposure = f;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			SetExposure((float) numericUpDown1.Value / 100.0f);
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			SetExposure(trackBar1.Value / 100.0f);
		}

		public override void BoxShown()
		{
			groupBox1.Controls.Add(Brushes.BrushBox);
			Brushes.BrushBox.Location = new Point(9, 19);
		}

		public override void BoxHidden()
		{
			Controls.Remove(Brushes.BrushBox);
		}
	}
}