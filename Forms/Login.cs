using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D
{
	public partial class Login : Form
	{
		public Login()
		{
			InitializeComponent();
		}

		public string Username
		{
			get { return textBox1.Text; }
			set { textBox1.Text = value; }
		}

		public string Password
		{
			get { return maskedTextBox1.Text; }
			set { maskedTextBox1.Text = value; }
		}

		public bool Remember
		{
			get { return checkBox1.Checked; }
			set { checkBox1.Checked = value; CheckAutoLogin(); }
		}

		public bool AutoLogin
		{
			get { return checkBox2.Checked; }
			set { checkBox2.Checked = value; }
		}

		void CheckAutoLogin()
		{
			if (!Remember)
			{
				checkBox2.Checked = false;
				checkBox2.Enabled = false;
			}
			else
				checkBox2.Enabled = true;
		}

		private void Login_Load(object sender, EventArgs e)
		{
			CheckAutoLogin();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			CheckAutoLogin();
		}
	}
}
