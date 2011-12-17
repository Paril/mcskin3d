using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MCSkin3D.Forms
{
	public partial class DirectoryList : Form
	{
		public DirectoryList()
		{
			InitializeComponent();
		}

		BindingList<string> _directories = new BindingList<string>();

		public BindingList<string> Directories
		{
			get { return _directories; }
		}

		private void DirectoryList_Load(object sender, EventArgs e)
		{
			listBox1.Comparer = new Paril.Controls.AlphanumComparatorFast();
			listBox1.DataSource = _directories;
			listBox1.SortList();
		}

		static string lastDir = Environment.CurrentDirectory;

		private void button1_Click(object sender, EventArgs e)
		{
			var browser = new DaveChambers.FolderBrowserDialogEx.FolderBrowserDialogEx();
			browser.SelectedPath = lastDir;
			browser.ShowNewFolderButton = true;
			browser.ShowEditbox = true;
			browser.StartPosition = FormStartPosition.CenterParent;

			if (browser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				lastDir = browser.SelectedPath;
				DirectoryInfo dir = new DirectoryInfo(lastDir);
				Directories.Add(dir.FullName);
			}

			listBox1.SortList();

			button3.Enabled = Directories.Count != 0;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			listBox1.BeginUpdate();
			var list = new object[listBox1.SelectedItems.Count];
			listBox1.SelectedItems.CopyTo(list, 0);
			listBox1.SelectedItems.Clear();

			foreach (var item in list)
				Directories.Remove((string)item);
			listBox1.EndUpdate();

			button3.Enabled = Directories.Count != 0;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}
