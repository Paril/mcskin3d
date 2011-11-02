using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MCSkin3D
{
	public partial class NameChange : Form
	{
		public NameChange()
		{
			InitializeComponent();
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

		public string SkinName
		{
			get { return textBox1.Text; }
			set { textBox1.Text = value; }
		}

		private void NameChange_Load(object sender, EventArgs e)
		{

		}

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			//e.Handled = true;
		}

		private void textBox1_KeyUp(object sender, KeyEventArgs e)
		{
			//e.Handled = true;
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && Path.GetInvalidFileNameChars().Contains(e.KeyChar))
			{
				e.Handled = true;
				//toolTipController1.ShowHint("A skin can't contain the following characters:\r\n           \\ / : * ? < > |", textBox1, DevExpress.Utils.ToolTipLocation.TopCenter);
				System.Media.SystemSounds.Beep.Play();		
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
