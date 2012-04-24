using System;
using System.Windows.Forms;

namespace Paril.Controls
{
	public partial class UndoRedoPanel : UserControl
	{
		private string _actionString;

		public UndoRedoPanel()
		{
			InitializeComponent();
			listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
		}

		public string ActionString
		{
			get { return _actionString; }
			set
			{
				_actionString = value;
				ResetString();
			}
		}

		public SelectionListBox ListBox
		{
			get { return listBox1; }
		}

		private void ResetString()
		{
			label1.Text = string.Format(ActionString, listBox1.SelectedIndices.Count);
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
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

		public int HighestItemSelected { get; private set; }

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
			int highItem = IndexFromPoint(e.Location);

			if (highItem != -1)
				SelectItems(highItem);

			base.OnMouseMove(e);
		}
	}
}