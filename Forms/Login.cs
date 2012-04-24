//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
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
			set
			{
				checkBox1.Checked = value;
				CheckAutoLogin();
			}
		}

		public bool AutoLogin
		{
			get { return checkBox2.Checked; }
			set { checkBox2.Checked = value; }
		}

		private void CheckAutoLogin()
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
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			CheckAutoLogin();
		}
	}
}