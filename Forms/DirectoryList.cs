using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DaveChambers.FolderBrowserDialogEx;
using Paril.Controls;

namespace MCSkin3D.Forms
{
	public partial class DirectoryList : Form
	{
		private static string lastDir = Environment.CurrentDirectory;
		private readonly BindingList<string> _directories = new BindingList<string>();

		public DirectoryList()
		{
			InitializeComponent();
		}

		public BindingList<string> Directories
		{
			get { return _directories; }
		}

		private void DirectoryList_Load(object sender, EventArgs e)
		{
			listBox1.Comparer = new AlphanumComparatorFast();
			listBox1.DataSource = _directories;
			listBox1.SortList();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var browser = new FolderBrowserDialogEx();
			browser.SelectedPath = lastDir;
			browser.ShowNewFolderButton = true;
			browser.ShowEditbox = true;
			browser.StartPosition = FormStartPosition.CenterParent;

			if (browser.ShowDialog(this) == DialogResult.OK)
			{
				lastDir = browser.SelectedPath;
				var dir = new DirectoryInfo(lastDir);
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

			foreach (object item in list)
				Directories.Remove((string) item);
			listBox1.EndUpdate();

			button3.Enabled = Directories.Count != 0;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}