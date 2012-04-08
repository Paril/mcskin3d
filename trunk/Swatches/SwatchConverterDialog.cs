using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D.Swatches
{
	public partial class SwatchConverterDialog : Form
	{
		public SwatchConverterDialog()
		{
			InitializeComponent();
		}

		public IList<string> SwatchFormats
		{
			get
			{
				List<string> swatches = new List<string>();

				foreach (var c in comboBox1.Items)
					swatches.Add((string)c);

				return swatches;
			}

			set
			{
				comboBox1.Items.Clear();

				foreach (var c in value)
					comboBox1.Items.Add(c);
			}
		}

		public int SelectedFormat
		{
			get { return comboBox1.SelectedIndex; }
			set { comboBox1.SelectedIndex = value; }
		}

		public string OldFormat
		{
			get { return label3.Text; }
			set { label3.Text = value; }
		}

		private void SwatchConverterDialog_Load(object sender, EventArgs e)
		{
			languageProvider1.LanguageChanged(Editor.CurrentLanguage);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}
