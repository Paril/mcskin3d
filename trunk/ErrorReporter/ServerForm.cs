using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClientTest;
using System.IO;

namespace ServerTest
{
	public partial class ServerForm : Form
	{
		public ServerForm()
		{
			InitializeComponent();
		}

		Server _server;

		void Pop2(int tesT)
		{
			throw new InvalidOperationException();
		}

		void Pop()
		{
			Pop2(0);
		}

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

		class ReportWrapper
		{
			public ErrorReport Report;
			public DateTime Received;

			public string ExceptionType;
			public string ExceptionMessage;
			public List<string> ExceptionStack = new List<string>();

			public ReportWrapper(ErrorReport report)
			{
				Received = DateTime.Now;
				Report = report;

				int firstNewline = report.Exception.IndexOf("\r\n");
				string left = (firstNewline == -1) ? report.Exception : report.Exception.Substring(0, firstNewline);
				int comma = left.IndexOf(':');

				ExceptionType = left.Substring(0, comma).Trim();
				ExceptionMessage = left.Substring(comma + 1).Trim();

				if (firstNewline != -1)
				{
					string stack = report.Exception.Substring(firstNewline + 2);
					string[] stackList = stack.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

					ExceptionStack.AddRange(stackList);
				}
			}
		}

		List<ReportWrapper> _reports = new List<ReportWrapper>();

		void PopulateTreeview()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();

			Dictionary<string, List<ReportWrapper>> dateSorted = new Dictionary<string, List<ReportWrapper>>();

			foreach (var r in _reports)
			{
				string day = r.Received.ToString("d");

				if (!dateSorted.ContainsKey(day))
					dateSorted.Add(day, new List<ReportWrapper>());

				dateSorted[day].Add(r);
			}

			foreach (var x in dateSorted)
			{
				TreeNode sortNode = treeView1.Nodes.Add(x.Key, x.Key);

				foreach (var y in x.Value)
				{
					var node = sortNode.Nodes.Add(_reports.IndexOf(y).ToString(), (string.IsNullOrEmpty(y.Report.Name) ? "Anonymous" : y.Report.Name) + " @ " + y.Received.ToString());
					node.Tag = y;
				}
			}

			treeView1.EndUpdate();
		}

		void _server_ReceivedReport(object sender, ReceivedReportEventArgs e)
		{
			Invoke((Action)delegate()
			{
				var wrap = new ReportWrapper(e.Report);
				string fileName = wrap.Received.ToString("d_M_yyyy_h_m_s_tt");

				while (File.Exists(fileName))
					fileName += "_";

				fileName += ".txt";

				File.WriteAllText(fileName,
				"Received Report @ " + DateTime.Now.ToString() + "\r\n\r\n" +
					"Software: " + e.Report.SoftwareInfo + "\r\n\r\n" +
					"Name: " + e.Report.Name + "\r\n" +
					"Email: " + e.Report.Email + "\r\n" +
					"Hardware: " + e.Report.HardwareInfo + "\r\n" +
					"Extra: " + e.Report.ExtraInfo + "\r\n" + "\r\n" +
					e.Report.Exception);

				_reports.Add(wrap);
				PopulateTreeview();
			});
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			/*if (string.IsNullOrEmpty(textBox2.Text))
				_server.MaintenenceString = null;
			else
				_server.MaintenenceString = textBox2.Text;*/
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is ReportWrapper)
			{
				ReportWrapper wrap = (ReportWrapper)e.Node.Tag;

				textBox1.Text = "Received Report @ " + wrap.Received.ToString() + "\r\n\r\n" +
				"Software: " + wrap.Report.SoftwareInfo + "\r\n\r\n" +
				"Name: " + wrap.Report.Name + "\r\n" +
				"Email: " + wrap.Report.Email + "\r\n" +
				"Hardware: " + wrap.Report.HardwareInfo + "\r\n" +
				"Extra: " + wrap.Report.ExtraInfo + "\r\n" + "\r\n" +
				wrap.Report.Exception;
			}
		}

		string _maintenenceString = "";
		bool _maintenence = false;
		InputDialog _dlg;

		private void maintenenceModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			maintenenceModeToolStripMenuItem.Checked = !maintenenceModeToolStripMenuItem.Checked;

			if (!maintenenceModeToolStripMenuItem.Checked)
				_maintenence = false;
			else
			{
				_dlg = new InputDialog();
				_dlg.Show();
				_dlg.FormClosed += new FormClosedEventHandler(dlg_FormClosed);
			}
		}

		void dlg_FormClosed(object sender, FormClosedEventArgs e)
		{
			_maintenenceString = _dlg.String;
			_maintenence = true;
			_dlg = null;

			_server.MaintenenceString = _maintenenceString;
		}
	}
}
