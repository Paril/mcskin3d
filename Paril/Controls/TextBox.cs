using System;
using System.Drawing;
using System.Windows.Forms;

namespace Paril.Controls
{
	public class TextBox : System.Windows.Forms.UserControl
	{
		System.Windows.Forms.TextBox _textBox;
		Label _label;

		public TextBox()
		{
			_textBox = new System.Windows.Forms.TextBox();
			_textBox.Dock = DockStyle.Fill;
			_textBox.GotFocus += _textBox_GotFocus;
			_textBox.LostFocus += _textBox_LostFocus;
			Controls.Add(_textBox);

			_label = new Label();
			_label.ForeColor = SystemColors.GrayText;
			_label.Cursor = Cursors.IBeam;
			_label.MouseDown += new MouseEventHandler(_label_MouseDown);
			Controls.Add(_label);

			_label.BringToFront();
		}

		void _label_MouseDown(object sender, MouseEventArgs e)
		{
			_textBox.Focus();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			_label.Size = new Size(_textBox.Size.Width - 4, _textBox.Size.Height - 4);
			_label.Location = new Point(2, 2);
		}

		void _textBox_LostFocus(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(_textBox.Text))
				_label.Visible = true;
		}

		void _textBox_GotFocus(object sender, EventArgs e)
		{
			_label.Visible = false;
		}

		public string EmptyText
		{
			get { return _label.Text; }
			set { _label.Text = value; Invalidate(); }
		}

		public override string Text
		{
			get { return _textBox.Text; }
			set	{ _textBox.Text = value; }
		}
	}
}
