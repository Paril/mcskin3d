using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Paril.Controls
{
	public partial class UndoRedoPanel : UserControl
	{
		public UndoRedoPanel()
		{
			InitializeComponent();
			listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);
		}

		string _actionString;
		public string ActionString
		{
			get { return _actionString; }
			set { _actionString = value; ResetString(); }
		}

		public ListBox ListBox
		{
			get { return listBox1; }
		}

		void ResetString()
		{
			label1.Text = string.Format(ActionString, listBox1.SelectedIndices.Count);
		}

		void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			ResetString();
		}
	}
}
