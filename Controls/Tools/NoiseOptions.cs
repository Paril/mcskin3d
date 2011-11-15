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
	public partial class NoiseOptions : ToolOptionBase
	{
		public NoiseOptions()
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
			SetExposure(GlobalSettings.NoiseSaturation);
		}

		bool _skipSet = false;
		void SetExposure(float f)
		{
			_skipSet = true;
			numericUpDown1.Value = (decimal)(f * 100.0f);
			trackBar1.Value = (int)(f * 100.0f);
			_skipSet = false;

			GlobalSettings.NoiseSaturation = f;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			SetExposure((float)numericUpDown1.Value / 100.0f);
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			if (_skipSet)
				return;

			SetExposure((float)trackBar1.Value / 100.0f);
		}

	}
}
