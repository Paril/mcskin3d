using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClientTest;

namespace ServerTest
{
	public partial class ServerForm : Form
	{
		public ServerForm()
		{
			InitializeComponent();
		}

		Server _server;

		private void ServerForm_Load(object sender, EventArgs e)
		{
			_server = new Server(8888);
			_server.ReceivedReport += new EventHandler<ReceivedReportEventArgs>(_server_ReceivedReport);
			_server.ListenForReports();
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			_server.Close();

			base.OnFormClosed(e);
		}

		void _server_ReceivedReport(object sender, ReceivedReportEventArgs e)
		{
			Invoke((Action)delegate()
			{
				textBox1.Text = "Received Report @ " + DateTime.Now.ToString() + "\r\n\r\n" +
					"Software: " + e.Report.SoftwareInfo + "\r\n\r\n" +
					"Name: " + e.Report.Name + "\r\n" +
					"Email: " + e.Report.Email + "\r\n" +
					"Hardware: " + e.Report.HardwareInfo + "\r\n" +
					"Extra: " + e.Report.ExtraInfo + "\r\n" + "\r\n" +
					e.Report.Exception;
			});
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBox2.Text))
				_server.MaintenenceString = null;
			else
				_server.MaintenenceString = textBox2.Text;
		}
	}
}
