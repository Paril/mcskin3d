//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MCSkin3D.Swatches;

namespace MCSkin3D
{
	public partial class SwatchContainer : UserControl
	{
		private SwatchDisplayer _swatchDisplayer;

		public SwatchContainer()
		{
			InitializeComponent();

			// 
			// swatchDisplayer1
			// 
			_swatchDisplayer = new MCSkin3D.SwatchDisplayer();
			_swatchDisplayer.Dock = System.Windows.Forms.DockStyle.Fill;
			_swatchDisplayer.Location = new System.Drawing.Point(0, 0);
			_swatchDisplayer.Name = "swatchDisplayer1";
			_swatchDisplayer.Scale = 0;
			_swatchDisplayer.ScrollBar = null;
			_swatchDisplayer.Size = new System.Drawing.Size(251, 138);
			_swatchDisplayer.Swatch = null;
			_swatchDisplayer.TabIndex = 1;
			_swatchDisplayer.Text = "swatchDisplayer1";
			_swatchDisplayer.ScrollBar = vScrollBar1;

			textBox1.Location = comboBox1.Location;
			textBox1.Size = comboBox1.Size;

			this.panel1.Controls.Add(_swatchDisplayer);

			_swatchDisplayer.BringToFront();

			foreach (var f in _swatchNames)
			{
				var item = convertSwatchStripButton.DropDownItems.Add(f);
				item.Click += item_Click;
			}
		}

		public event EventHandler<SwatchChangedEventArgs> SwatchChanged
		{
			add
			{
				_swatchDisplayer.SwatchChanged += value;
			}

			remove
			{
				_swatchDisplayer.SwatchChanged -= value;
			}
		}

		private void SwatchContainer_Load(object sender, EventArgs e)
		{
			SetZoomAbility();
		}

		public void AddSwatches(IEnumerable<ISwatch> swatches)
		{
			foreach (var s in swatches)
				comboBox1.Items.Add(s);

			if (comboBox1.SelectedItem == null && comboBox1.Items.Count != 0)
				comboBox1.SelectedIndex = 0;
		}

		public void SaveSwatches()
		{
			foreach (ISwatch swatch in comboBox1.Items)
				swatch.Save();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem == null)
			{
				_swatchDisplayer.Swatch = null;
				return;
			}

			_swatchDisplayer.Swatch = (comboBox1.SelectedItem as ISwatch);
			SetCheckedItem();
		}

		void SetCheckedItem()
		{
			foreach (ToolStripMenuItem x in convertSwatchStripButton.DropDownItems)
				x.Checked = false;

			for (int i = 0; i < _swatchTypes.Length; ++i)
			{
				if (SwatchDisplayer.Swatch.GetType() == _swatchTypes[i])
				{
					((ToolStripMenuItem)convertSwatchStripButton.DropDownItems[i]).Checked = true;
					break;
				}
			}
		}

		void SetZoomAbility()
		{
			toolStripButton1.Enabled = (_swatchDisplayer.Scale != 0);
		}

		public void ZoomOut()
		{
			_swatchDisplayer.ZoomOut();
			SetZoomAbility();
		}

		public void ZoomIn()
		{
			_swatchDisplayer.ZoomIn();
			SetZoomAbility();
		}

		public SwatchDisplayer SwatchDisplayer
		{
			get { return _swatchDisplayer; }
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			ZoomOut();
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			ZoomIn();
		}

		public bool InEditMode { get { return editModeToolStripButton.Checked; } }

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		static Type[] _swatchTypes = new Type[] { typeof(ACOSwatch), typeof(ACTSwatch), typeof(GIMPSwatch), typeof(MCSwatch) };
		static string[] _swatchNames = new string[] { "Adobe Color (ACO)", "Adobe Color Table (ACT)", "GIMP Swatch (GPL)", "MCSkin3D Swatch (.swtch)" };
		static string[] _swatchFormatNames = new string[] { "ACO", "ACT", "GPL", "SWTCH" };

		public static string GetSwatchName(Type type)
		{
			for (int i = 0; i < _swatchTypes.Length; ++i)
				if (type == _swatchTypes[i])
					return _swatchNames[i];

			return "???";
		}

		public static string GetSwatchFormatName(Type type)
		{
			for (int i = 0; i < _swatchTypes.Length; ++i)
				if (type == _swatchTypes[i])
					return _swatchFormatNames[i];

			return "???";
		}

		void item_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			int selectedFormat = convertSwatchStripButton.DropDownItems.IndexOf(item);
			var newType = _swatchTypes[selectedFormat];

			if (newType == SwatchDisplayer.Swatch.GetType())
				return;

			var newPath = Path.GetDirectoryName(SwatchDisplayer.Swatch.FilePath) + '\\' + Path.GetFileNameWithoutExtension(SwatchDisplayer.Swatch.FilePath) + '.' + _swatchFormatNames[selectedFormat].ToLower();

			if (File.Exists(newPath))
			{
				System.Media.SystemSounds.Exclamation.Play();
				return;
			}

			ISwatch swatch = (ISwatch)newType.GetConstructors()[0].Invoke(new object[] { newPath });

			foreach (var c in SwatchDisplayer.Swatch)
				swatch.Add(c);

			swatch.Save();

			var index = comboBox1.Items.IndexOf(SwatchDisplayer.Swatch);

			SwatchDisplayer.Swatch.Name = null; // just to check if we broke it later or not
			SwatchDisplayer.Swatch.FilePath = null;

			comboBox1.Items[index] = swatch;

			SetCheckedItem();
		}

		bool _creatingSwatch = false;

		public bool SwatchRenameTextBoxHasFocus
		{
			get { return textBox1.ContainsFocus; }
		}

		void BeginRename()
		{
			textBox1.Visible = true;
			textBox1.Focus();
		}

		private void newSwatchToolStripButton_Click(object sender, EventArgs e)
		{
			textBox1.Text = "";
			BeginRename();
			_creatingSwatch = true;
		}

		private void renameSwatchToolStripButton3_Click(object sender, EventArgs e)
		{
			if (SwatchDisplayer.Swatch == null)
				return;

			textBox1.Text = SwatchDisplayer.Swatch.Name;
			BeginRename();
			_creatingSwatch = false;
		}

		private void textBox1_Leave(object sender, EventArgs e)
		{
			textBox1.Clear();
			textBox1.Visible = false;
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				if (!_creatingSwatch)
					SwatchDisplayer.Swatch.Name = textBox1.Text;
				else
				{
					MCSwatch newSwatch = new MCSwatch("Swatches\\" + textBox1.Text + ".swtch");
					newSwatch.Save();

					comboBox1.Items.Add(newSwatch);
					comboBox1.SelectedItem = newSwatch;
				}

				e.Handled = true;
				textBox1.Visible = false;

				comboBox1.Refresh();
			}
		}

		private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();

			if (e.Index != -1)
			{
				var swatch = (ISwatch)comboBox1.Items[e.Index];
				var rightSide = TextRenderer.MeasureText(swatch.Format, comboBox1.Font).Width;

				Rectangle bounds = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - rightSide, e.Bounds.Height);

				TextRenderer.DrawText(e.Graphics, swatch.Name, comboBox1.Font, bounds, (e.State & DrawItemState.Selected) == 0 ? comboBox1.ForeColor : SystemColors.HighlightText, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
				TextRenderer.DrawText(e.Graphics, swatch.Format, comboBox1.Font, new Rectangle(bounds.X + (e.Bounds.Width - rightSide), bounds.Y, rightSide, bounds.Height), (e.State & DrawItemState.Selected) == 0 ? comboBox1.ForeColor : SystemColors.HighlightText, TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
			}

			e.DrawFocusRectangle();
		}

		private void deleteSwatchToolStripButton_Click(object sender, EventArgs e)
		{
			var swatch = SwatchDisplayer.Swatch;

			if (swatch == null)
				return;

			if (MessageBox.Show(Editor.GetLanguageString("M_SWATCHQUESTION"), Editor.GetLanguageString("M_SWATCHQUESTION_CAPTION"), MessageBoxButtons.YesNo) == DialogResult.No)
				return;

			File.Delete(swatch.FilePath);
			comboBox1.Items.Remove(swatch);
		}

		private void addSwatchToolStripButton_Click(object sender, EventArgs e)
		{
			var swatch = SwatchDisplayer.Swatch;

			if (swatch == null)
				return;

			swatch.Add(new NamedColor("New Color", Editor.MainForm.ColorPanel.SelectedColor.RGB));
			SwatchDisplayer.RecalculateSize();
		}

		private void removeSwatchToolStripButton_Click(object sender, EventArgs e)
		{
			var swatch = SwatchDisplayer.Swatch;

			if (swatch == null)
				return;

			if (SwatchDisplayer.HasPrimaryColor)
			{
				swatch.RemoveAt(SwatchDisplayer.PrimaryColorIndex);

				if (swatch.Count == 0)
					SwatchDisplayer.PrimaryColorIndex = SwatchDisplayer.SecondaryColorIndex = -1;
				else
				{
					if (SwatchDisplayer.PrimaryColorIndex >= swatch.Count)
						SwatchDisplayer.PrimaryColorIndex = swatch.Count - 1;

					if (SwatchDisplayer.SecondaryColorIndex >= swatch.Count)
						SwatchDisplayer.SecondaryColorIndex = swatch.Count - 1;
				}

				SwatchDisplayer.RecalculateSize();
			}
		}

		private void convertSwatchStripButton_ButtonClick(object sender, EventArgs e)
		{
			convertSwatchStripButton.ShowDropDown();
		}
	}

	public class SwatchChangedEventArgs : EventArgs
	{
		public Color Swatch;
		public MouseButtons Button;

		public SwatchChangedEventArgs(Color sw, MouseButtons but)
		{
			Swatch = sw;
			Button = but;
		}
	}

	public class SwatchDisplayer : Control
	{
		System.Windows.Forms.VScrollBar _sb;
		public System.Windows.Forms.VScrollBar ScrollBar
		{
			get { return _sb; }
			set { _sb = value; if (_sb != null) _sb.Scroll += new ScrollEventHandler(_sb_Scroll); }
		}
		
		public SwatchDisplayer()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;
		}

		public event EventHandler<SwatchChangedEventArgs> SwatchChanged;

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			Parent.SizeChanged += new System.EventHandler(Parent_SizeChanged);
		}

		void Parent_SizeChanged(object sender, EventArgs e)
		{
			RecalculateSize();
		}

		int _scale = 0;
		public new int Scale
		{
			get { return _scale; }
			set { _scale = value; RecalculateSize(); }
		}

		int _rows = 0, _fitPerRow = 0;

		public int SwatchSize
		{
			get { return 10 + Scale; }
		}

		void _sb_Scroll(object sender, ScrollEventArgs e)
		{
			Invalidate();
		}

		public void RecalculateSize()
		{
			if (Parent == null)
				return;

			Location = new Point(0, 0);

			Size = new Size(Parent.Width - 18, Parent.Height);

			if (_colors == null || _colors.Count == 0)
			{
				_rows = 0;
				Invalidate();
				return;
			}

			_fitPerRow = (int)Math.Floor(((float)(Width) / (float)(SwatchSize + 1)));

			if (_fitPerRow == 0)
			{
				_rows = 0;
				Invalidate();
				return;
			}

			_rows = (_colors.Count / _fitPerRow) + 1;

			if (((_rows) * (SwatchSize + 1)) + 1 > Height)
			{
				int maxRows = (Height) / (SwatchSize + 1);

				ScrollBar.Maximum = 10 + (_rows - (maxRows));
			}
			else
				ScrollBar.Maximum = 0;

			Invalidate();
		}

		public void ZoomIn()
		{
			Scale++;
		}

		public void ZoomOut()
		{
			if (Scale != 0)
				Scale--;
		}

		int _lastLeftSwatch = -1, _lastRightSwatch = -1;

		public bool HasPrimaryColor { get { return _lastLeftSwatch != -1; } }
		public bool HasSecondaryColor { get { return _lastRightSwatch != -1; } }

		public Color PrimaryColor
		{
			get { if (Swatch == null) return Color.White; return Swatch[_lastLeftSwatch].Color; }
			set { Swatch[_lastLeftSwatch].Color = value; }
		}

		public Color SecondaryColor
		{
			get { if (Swatch == null) return Color.White; return Swatch[_lastRightSwatch].Color; }
			set { Swatch[_lastRightSwatch].Color = value; }
		}

		public int PrimaryColorIndex
		{
			get { if (Swatch == null) return -1; return _lastLeftSwatch; }
			set { _lastLeftSwatch = value; }
		}

		public int SecondaryColorIndex
		{
			get { if (Swatch == null) return -1; return _lastRightSwatch; }
			set { _lastRightSwatch = value; }
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			int row = e.X / (SwatchSize + 1);

			if (row >= _fitPerRow)
				return;

			int col = (e.Y / (SwatchSize + 1)) + ScrollBar.Value;

			if (row + (col * _fitPerRow) >= Swatch.Count)
				return;

			var lastSwatch = row + (col * _fitPerRow);
			if (e.Button == MouseButtons.Left)
				_lastLeftSwatch = lastSwatch;
			else
				_lastRightSwatch = lastSwatch;

			Invalidate();

			if (SwatchChanged == null)
				return;

			SwatchChanged(this, new SwatchChangedEventArgs(Swatch[lastSwatch].Color, e.Button));
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			BackColor = Enabled ? SystemColors.Window : SystemColors.Control;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_rows == 0)
				return;

			int index = _fitPerRow * _sb.Value;
			int y = 0;
			while (index < Swatch.Count)
			{
				for (int i = 0; i < _fitPerRow; ++i)
				{
					if (index >= Swatch.Count)
						break;

					e.Graphics.FillRectangle(new SolidBrush(Swatch[index].Color), new Rectangle(1 + (i * (SwatchSize + 1)), 1 + (y * (SwatchSize + 1)), SwatchSize, SwatchSize));

					if (_lastLeftSwatch == index)
						e.Graphics.DrawRectangle(new Pen(Color.Yellow, 1), new Rectangle(1 + (i * (SwatchSize + 1)), 1 + (y * (SwatchSize + 1)), SwatchSize - 1, SwatchSize - 1));
					else if (_lastRightSwatch == index)
						e.Graphics.DrawRectangle(new Pen(Color.Red, 1), new Rectangle(1 + (i * (SwatchSize + 1)), 1 + (y * (SwatchSize + 1)), SwatchSize - 1, SwatchSize - 1));
					else
						e.Graphics.DrawRectangle(Pens.Black, new Rectangle(1 + (i * (SwatchSize + 1)), 1 + (y * (SwatchSize + 1)), SwatchSize - 1, SwatchSize - 1));
					index++;
				}

				y++;
			}
		}

		ISwatch _colors;

		public ISwatch Swatch
		{
			get { return _colors; }
			set { _colors = value; _lastLeftSwatch = _lastRightSwatch = -1; RecalculateSize(); }
		}
	}
}
