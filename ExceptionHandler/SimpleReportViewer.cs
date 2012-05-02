using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D.ExceptionHandler
{
	public partial class SimpleReportViewer : Form
	{
		public SimpleReportViewer()
		{
			InitializeComponent();
		}

		private void SimpleReportViewer_Load(object sender, EventArgs e)
		{
		}

		public static void ShowReportViewer(string report)
		{
			using (var viewer = new SimpleReportViewer())
			{
				viewer.StartPosition = FormStartPosition.CenterParent;
				viewer.textBox1.Text = report;
				viewer.languageProvider1.LanguageChanged(Editor.CurrentLanguage);
				viewer.ShowDialog();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
