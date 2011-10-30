using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D
{
	public partial class AddSkinDialog : Form
	{
		public AddSkinDialog()
		{
			InitializeComponent();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		public string SkinName
		{
			get { return textBox1.Text; }
		}

		public string FileName
		{
			get { return textBox2.Text; }
		}

		private void button1_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Minecraft Skins|*.png";

				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					textBox2.Text = ofd.FileName;
			}
		}
	}
}
