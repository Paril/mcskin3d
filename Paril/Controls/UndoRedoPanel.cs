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

		public SelectionListBox ListBox
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

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			ListBox.SelectItems(-1);
		}
	}

	public class SelectionListBox : ListBox
	{
		public SelectionListBox()
		{
			SetStyle(ControlStyles.UserMouse, true);
		}

		public int HighestItemSelected
		{
			get;
			private set;
		}

		internal void SelectItems(int highItem)
		{
			if (HighestItemSelected <= highItem)
			{
				for (int i = HighestItemSelected; i <= highItem; ++i)
					SelectedIndices.Add(i);
			}
			else
			{
				for (int i = HighestItemSelected; i > highItem; --i)
					SelectedIndices.Remove(i);
			}

			HighestItemSelected = highItem;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var highItem = IndexFromPoint(e.Location);

			if (highItem != -1)
				SelectItems(highItem);

			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			//SelectItems(-1);

			base.OnMouseLeave(e);
		}
	}
}
