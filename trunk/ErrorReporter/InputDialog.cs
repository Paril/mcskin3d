using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServerTest
{
	public partial class InputDialog : Form
	{
		public InputDialog()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		public string String { get { return textBox1.Text; } }

		public static string GetInput()
		{
			using (InputDialog dia = new InputDialog())
			{
				var dlg = dia.ShowDialog();

				if (dlg == DialogResult.OK)
					return dia.textBox1.Text;
				return null;
			}
		}
	}
}
