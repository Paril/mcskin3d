using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;

namespace ClientTest
{
	public partial class ClientForm : Form
	{
		public ClientForm()
		{
			InitializeComponent();
		}

		public string GetHardwareInfo()
		{
			try
			{
				string info = "Video Info:\r\n";

				ManagementObjectSearcher searcher =
              new ManagementObjectSearcher("Select * from Win32_VideoController");

				foreach (ManagementObject video in searcher.Get())
				{
					foreach (var prop in video.Properties)
						if (prop.Value != null &&
							prop.Name != "SystemName")
							info += prop.Name + ": " + prop.Value.ToString() + "\r\n";
				}

				info += "\r\nMemory Info:\r\n";

				searcher =
				  new ManagementObjectSearcher("Select * from Win32_PhysicalMemoryArray");

				foreach (ManagementObject video in searcher.Get())
				{
					foreach (var prop in video.Properties)
						if (prop.Value != null &&
							prop.Name != "SystemName")
							info += prop.Name + ": " + prop.Value.ToString() + "\r\n";
				}

				info += "\r\nProcessor Info:\r\n";

				searcher =
				  new ManagementObjectSearcher("Select * from Win32_Processor");

				foreach (ManagementObject video in searcher.Get())
				{
					foreach (var prop in video.Properties)
						if (prop.Value != null &&
							prop.Name != "SystemName")
							info += prop.Name + ": " + prop.Value.ToString() + "\r\n";
				}

				return info;
			}
			catch (Exception ex)
			{
				return "Couldn't get hardware info: " + ex.ToString();
			}
		}

		public void InitSizes()
		{
			int oldHeight = label3.Height;

			using (Graphics g = CreateGraphics())
			{
				var size = g.MeasureString(label3.Text, label3.Font, label3.Width);
				label3.Size = new Size((int)size.Width, (int)size.Height);
			}

			int offs = label3.Height - oldHeight;
			panel4.Location = new Point(panel4.Location.X, panel4.Location.Y + offs);

			Height += offs;

			linkLabel1.Location = new Point(label2.Location.X + label2.Width - 4, label2.Location.Y);
		}

		Client _client;

		//private void button2_Click(object sender, EventArgs e)
		//{
			/*button2.Enabled = false;

			ErrorReport report = new ErrorReport();
			report.Exception = new ArgumentNullException("e").ToString();
			report.SoftwareInfo = Environment.OSVersion + "\r\n" + "x64: " + (Environment.Is64BitOperatingSystem ? "Yes" : "No") + "\r\n" + ".NET Version: " + Environment.Version.ToString() + "\r\n";

			report.Name = textBox2.Text;
			report.Email = textBox3.Text;

			report.HardwareInfo = checkBox1.Checked ? GetHardwareInfo() : null;
			report.ExtraInfo = textBox4.Text;

			string[] spl = textBox1.Text.Split(':');

			_client = new Client(report, spl[0], int.Parse(spl[1]));
			_client.SendFinished += new EventHandler<SendEventArgs>(_client_SendFinished);
			_client.SendToServer();*/
		//}

		void _client_SendFinished(object sender, SendEventArgs e)
		{
			/*Invoke((Action)delegate()
			{
				button2.Enabled = true;

				MessageBox.Show((e.Failed ? "Failed" : "Succeeded") + ": " + e.StatusString);
			});*/
		}

		private void ClientForm_Load(object sender, EventArgs e)
		{
			InitSizes();
		}

		private void panel2_Paint_1(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.White, 0, 0, panel2.Width, 0);
		}

		private void panel3_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(SystemPens.ControlLight, 0, 0, panel3.Width, 0);
			e.Graphics.DrawLine(SystemPens.ControlLight, 0, panel3.Height - 1, panel3.Width, panel3.Height - 1);
		}

		bool _sentReport = false;

		private void button1_Click(object sender, EventArgs e)
		{
			panel4.Visible = false;
			button1.Enabled = false;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
