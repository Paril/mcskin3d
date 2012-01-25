using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Paril.Controls
{
	public class NumericUpDownMenuItem : ToolStripControlHost
	{
		Label _textLabel;
		public NumericUpDownMenuItem() :
			base(new Panel())
		{
			Control.Size = new Size(140, 12);

			_textLabel = new Label();
			Control.Controls.Add(_textLabel);
			_textLabel.Location = new Point(0, 0);
			_textLabel.AutoSize = false;
			_textLabel.Size = new System.Drawing.Size(TextRenderer.MeasureText(_textLabel.Text, _textLabel.Font).Width, 23);
			_textLabel.TextAlign = ContentAlignment.MiddleLeft;

			NumericBox = new NumericUpDown();
			NumericBox.Width = 62;
			Control.Controls.Add(NumericBox);
			NumericBox.Location = new Point(_textLabel.Location.X + _textLabel.Width + 2, 0);
			_textLabel.Height = NumericBox.Height;
			_textLabel.Location = new Point(0, (NumericBox.Height / 2) - (_textLabel.Height / 2));

			SetBounds(new Rectangle(Point.Empty, Size));
		}

		protected override void OnTextChanged(EventArgs e)
		{
			_textLabel.Text = Text;
			_textLabel.Size = new System.Drawing.Size(TextRenderer.MeasureText(_textLabel.Text, _textLabel.Font).Width, 23);
			NumericBox.Location = new Point(_textLabel.Location.X + _textLabel.Width + 4, 0);
			base.OnTextChanged(e);
		}

		public NumericUpDown NumericBox
		{
			get;
			private set;
		}

		protected override void OnBoundsChanged()
		{
			Height = 23;
			Width = 140;
			base.OnBoundsChanged();
		}
	}
}
