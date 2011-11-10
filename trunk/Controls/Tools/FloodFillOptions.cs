using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D
{
	public partial class FloodFillOptions : ToolOptionBase
	{
		public FloodFillOptions()
		{
			InitializeComponent();
		}

		private void DodgeBurnOptions_Load(object sender, EventArgs e)
		{
			SetThreshold(GlobalSettings.FloodFillThreshold);
		}

		bool _skipSet = false;
		void SetThreshold(float f)
		{
			_skipSet = true;
			numericUpDown1.Value = (decimal)(f * 100.0f);
			trackBar1.Value = (int)(f * 100.0f);
			_skipSet = false;

			GlobalSettings.FloodFillThreshold = f;
		}
		
		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			SetThreshold((float)numericUpDown1.Value / 100.0f);
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			SetThreshold((float)trackBar1.Value / 100.0f);
		}
	}
}
