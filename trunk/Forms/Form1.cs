using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D.Forms
{
	public partial class ImportSite : Form
	{
		public ImportSite()
		{
			InitializeComponent();
		}

		private void ImportSite_Load(object sender, EventArgs e)
		{

		}

		public new string Show()
		{
			if (ShowDialog() == DialogResult.Cancel)
				return null;

			return textBox1.Text;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}
