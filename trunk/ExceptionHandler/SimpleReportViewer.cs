using System;
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
				viewer.ShowDialog();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
