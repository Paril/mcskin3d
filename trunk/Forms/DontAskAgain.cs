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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D.Forms
{
	public partial class DontAskAgain : Form
	{
		public DontAskAgain()
		{
			InitializeComponent();
		}

		private void DontAskAgain_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Display a Don't Ask Again dialog, returns true if he clicked Yes.
		/// </summary>
		/// <param name="language">Language to use</param>
		/// <param name="labelValue">Label string</param>
		/// <param name="againValue">The current stored boolean and reference to the new one</param>
		/// <returns></returns>
		public static bool Show(Language.Language language, string labelValue, ref bool dontShow)
		{
			if (dontShow)
				return true;

			using (DontAskAgain form = new DontAskAgain())
			{
				form.StartPosition = FormStartPosition.CenterParent;
				form.label1.Text = labelValue;
				form.languageProvider1.LanguageChanged(language);

				form.ShowDialog();

				dontShow = form.checkBox1.Checked;

				return form.DialogResult == DialogResult.Yes;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Yes;
		}

		private void button1_Click(object sender, EventArgs e)
		{

			DialogResult = System.Windows.Forms.DialogResult.No;
		}
	}
}
