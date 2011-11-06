using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D
{
	public partial class PleaseWait : Form
	{
		public PleaseWait()
		{
			InitializeComponent();
		}

		protected override CreateParams CreateParams
		{

			get
			{

				CreateParams param = base.CreateParams;

				param.ClassStyle = param.ClassStyle | 0x200;

				return param;

			}

		}

		private void PleaseWait_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void PleaseWait_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Abort;
			Close();
		}
	}
}
