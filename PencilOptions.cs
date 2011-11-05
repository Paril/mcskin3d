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
	public partial class PencilOptions : UserControl
	{
		public PencilOptions()
		{
			InitializeComponent();
		}

		private void PencilOptions_Load(object sender, EventArgs e)
		{
			checkBox1.Checked = GlobalSettings.PencilIncremental;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			GlobalSettings.PencilIncremental = checkBox1.Checked;
		}
	}
}
