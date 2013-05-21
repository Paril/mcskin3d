//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
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

using MCSkin3D.Swatches;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace MCSkin3D
{
	public partial class SwatchContainer : UserControl
	{
		private static readonly Type[] _swatchTypes = new[]
		                                              {
		                                              	typeof (ACOSwatch), typeof (ACTSwatch), typeof (GIMPSwatch),
		                                              	typeof (MCSwatch)
		                                              };

		private static readonly string[] _swatchNames = new[]
		                                                {
		                                                	"Adobe Color (ACO)", "Adobe Color Table (ACT)", "GIMP Swatch (GPL)",
		                                                	"MCSkin3D Swatch (.swtch)"
		                                                };

		private static readonly string[] _swatchFormatNames = new[] {"ACO", "ACT", "GPL", "SWTCH"};
		private readonly SwatchDisplayer _swatchDisplayer;
		private bool _creatingSwatch;

		public SwatchContainer()
		{
			InitializeComponent();

			// 
			// swatchDisplayer1
			// 
			_swatchDisplayer = new SwatchDisplayer();
			_swatchDisplayer.Dock = DockStyle.Fill;
			_swatchDisplayer.Location = new Point(0, 0);
			_swatchDisplayer.Name = "swatchDisplayer1";
			_swatchDisplayer.Scale = 0;
			_swatchDisplayer.ScrollBar = null;
			_swatchDisplayer.Size = new Size(251, 138);
			_swatchDisplayer.Swatch = null;
			_swatchDisplayer.TabIndex = 1;
			_swatchDisplayer.Text = "swatchDisplayer1";
			_swatchDisplayer.ScrollBar = vScrollBar1;

			textBox1.Location = comboBox1.Location;
			textBox1.Size = comboBox1.Size;

			panel1.Controls.Add(_swatchDisplayer);

			_swatchDisplayer.BringToFront();

			foreach (string f in _swatchNames)
			{
				ToolStripItem item = convertSwatchStripButton.DropDownItems.Add(f);
				item.Click += item_Click;
			}
		}

		public SwatchDisplayer SwatchDisplayer
		{
			get { return _swatchDisplayer; }
		}

		public bool InEditMode
		{
			get { return editModeToolStripButton.Checked; }
		}

		public bool SwatchRenameTextBoxHasFocus
		{
			get { return textBox1.ContainsFocus; }
		}

		public event EventHandler<SwatchChangedEventArgs> SwatchChanged
		{
			add { _swatchDisplayer.SwatchChanged += value; }

			remove { _swatchDisplayer.SwatchChanged -= value; }
		}

		private void SwatchContainer_Load(object sender, EventArgs e)
		{
			SetZoomAbility();
		}

		public void AddSwatches(IEnumerable<ISwatch> swatches)
		{
			foreach (ISwatch s in swatches)
				comboBox1.Items.Add(s);

			if (comboBox1.SelectedItem == null && comboBox1.Items.Count != 0)
				comboBox1.SelectedIndex = 0;
		}

		public void SaveSwatches()
		{
			foreach (ISwatch swatch in comboBox1.Items)
			{
				if (swatch.Dirty)
				{
					swatch.Save();
					MessageBox.Show("Swatch " + swatch.Name + " saved");
				}
			}
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

		private void SetCheckedItem()
		{
			foreach (ToolStripMenuItem x in convertSwatchStripButton.DropDownItems)
				x.Checked = false;

			for (int i = 0; i < _swatchTypes.Length; ++i)
			{
				if (SwatchDisplayer.Swatch.GetType() == _swatchTypes[i])
				{
					((ToolStripMenuItem) convertSwatchStripButton.DropDownItems[i]).Checked = true;
					break;
				}
			}
		}

		private void SetZoomAbility()
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

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			ZoomOut();
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			ZoomIn();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
		}

		public static string GetSwatchName(Type type)
		{
			for (int i = 0; i < _swatchTypes.Length; ++i)
			{
				if (type == _swatchTypes[i])
					return _swatchNames[i];
			}

			return "???";
		}

		public static string GetSwatchFormatName(Type type)
		{
			for (int i = 0; i < _swatchTypes.Length; ++i)
			{
				if (type == _swatchTypes[i])
					return _swatchFormatNames[i];
			}

			return "???";
		}

		private void item_Click(object sender, EventArgs e)
		{
			var item = (ToolStripMenuItem) sender;
			int selectedFormat = convertSwatchStripButton.DropDownItems.IndexOf(item);
			Type newType = _swatchTypes[selectedFormat];

			if (newType == SwatchDisplayer.Swatch.GetType())
				return;

			string newPath = Path.GetDirectoryName(SwatchDisplayer.Swatch.FilePath) + '\\' +
			                 Path.GetFileNameWithoutExtension(SwatchDisplayer.Swatch.FilePath) + '.' +
			                 _swatchFormatNames[selectedFormat].ToLower();

			if (File.Exists(newPath))
			{
				SystemSounds.Exclamation.Play();
				return;
			}

			var swatch = (ISwatch) newType.GetConstructors()[0].Invoke(new object[] {newPath});

			foreach (NamedColor c in SwatchDisplayer.Swatch)
				swatch.Add(c);

			swatch.Save();

			int index = comboBox1.Items.IndexOf(SwatchDisplayer.Swatch);

			SwatchDisplayer.Swatch.Name = null; // just to check if we broke it later or not
			SwatchDisplayer.Swatch.FilePath = null;

			comboBox1.Items[index] = swatch;

			SetCheckedItem();
		}

		private void BeginRename()
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
					var newSwatch = new MCSwatch(GlobalSettings.GetDataURI("Swatches\\" + textBox1.Text + ".swtch"));
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
				var swatch = (ISwatch) comboBox1.Items[e.Index];
				int rightSide = TextRenderer.MeasureText(swatch.Format, comboBox1.Font).Width;

				var bounds = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - rightSide, e.Bounds.Height);

				TextRenderer.DrawText(e.Graphics, swatch.Name, comboBox1.Font, bounds,
				                      (e.State & DrawItemState.Selected) == 0 ? comboBox1.ForeColor : SystemColors.HighlightText,
				                      TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
				TextRenderer.DrawText(e.Graphics, swatch.Format, comboBox1.Font,
				                      new Rectangle(bounds.X + (e.Bounds.Width - rightSide), bounds.Y, rightSide, bounds.Height),
				                      (e.State & DrawItemState.Selected) == 0 ? comboBox1.ForeColor : SystemColors.HighlightText,
				                      TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
			}

			e.DrawFocusRectangle();
		}

		private void deleteSwatchToolStripButton_Click(object sender, EventArgs e)
		{
			ISwatch swatch = SwatchDisplayer.Swatch;

			if (swatch == null)
				return;

			if (
				MessageBox.Show(Editor.GetLanguageString("M_SWATCHQUESTION"), Editor.GetLanguageString("M_SWATCHQUESTION_CAPTION"),
				                MessageBoxButtons.YesNo) == DialogResult.No)
				return;

			File.Delete(swatch.FilePath);
			comboBox1.Items.Remove(swatch);
		}

		private void addSwatchToolStripButton_Click(object sender, EventArgs e)
		{
			PerformAddSwatch();
		}

		private void removeSwatchToolStripButton_Click(object sender, EventArgs e)
		{
			PerformRemoveSwatch();
		}

		private void convertSwatchStripButton_ButtonClick(object sender, EventArgs e)
		{
			convertSwatchStripButton.ShowDropDown();
		}

		private void editModeToolStripButton_Click(object sender, EventArgs e)
		{
			ToggleEditMode();
		}

		public void PerformAddSwatch()
		{
			ISwatch swatch = SwatchDisplayer.Swatch;

			if (swatch == null)
				return;

			swatch.Add(new NamedColor("New Color", Editor.MainForm.ColorPanel.SelectedColor.RGB));
			SwatchDisplayer.RecalculateSize();
		}

		public void PerformRemoveSwatch()
		{
			ISwatch swatch = SwatchDisplayer.Swatch;

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

		public void ToggleEditMode()
		{
			editModeToolStripButton.Checked = !editModeToolStripButton.Checked;
		}
	}

	public class SwatchChangedEventArgs : EventArgs
	{
		public MouseButtons Button;
		public Color Swatch;

		public SwatchChangedEventArgs(Color sw, MouseButtons but)
		{
			Swatch = sw;
			Button = but;
		}
	}

	public class SwatchDisplayer : Control
	{
		private ISwatch _colors;
		private int _fitPerRow;
		private int _lastLeftSwatch = -1, _lastRightSwatch = -1;
		private int _rows;
		private VScrollBar _sb;

		private int _scale;

		public SwatchDisplayer()
		{
			SetStyle(
				ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse | ControlStyles.UserPaint |
				ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;
		}

		public VScrollBar ScrollBar
		{
			get { return _sb; }
			set
			{
				_sb = value;
				if (_sb != null) _sb.Scroll += _sb_Scroll;
			}
		}

		public new int Scale
		{
			get { return _scale; }
			set
			{
				_scale = value;
				RecalculateSize();
			}
		}

		public int SwatchSize
		{
			get { return 10 + Scale; }
		}

		public bool HasPrimaryColor
		{
			get { return _lastLeftSwatch != -1; }
		}

		public bool HasSecondaryColor
		{
			get { return _lastRightSwatch != -1; }
		}

		public Color PrimaryColor
		{
			get
			{
				if (Swatch == null) return Color.White;
				return Swatch[_lastLeftSwatch].Color;
			}
			set { Swatch[_lastLeftSwatch].Color = value; }
		}

		public Color SecondaryColor
		{
			get
			{
				if (Swatch == null) return Color.White;
				return Swatch[_lastRightSwatch].Color;
			}
			set { Swatch[_lastRightSwatch].Color = value; }
		}

		public int PrimaryColorIndex
		{
			get
			{
				if (Swatch == null) return -1;
				return _lastLeftSwatch;
			}
			set { _lastLeftSwatch = value; }
		}

		public int SecondaryColorIndex
		{
			get
			{
				if (Swatch == null) return -1;
				return _lastRightSwatch;
			}
			set { _lastRightSwatch = value; }
		}

		public ISwatch Swatch
		{
			get { return _colors; }
			set
			{
				_colors = value;
				_lastLeftSwatch = _lastRightSwatch = -1;
				RecalculateSize();
			}
		}

		public event EventHandler<SwatchChangedEventArgs> SwatchChanged;

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			Parent.SizeChanged += Parent_SizeChanged;
		}

		private void Parent_SizeChanged(object sender, EventArgs e)
		{
			RecalculateSize();
		}

		private void _sb_Scroll(object sender, ScrollEventArgs e)
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

			_fitPerRow = (int) Math.Floor(((Width) / (float) (SwatchSize + 1)));

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

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			int row = e.X / (SwatchSize + 1);

			if (row >= _fitPerRow)
				return;

			int col = (e.Y / (SwatchSize + 1)) + ScrollBar.Value;

			if (row + (col * _fitPerRow) >= Swatch.Count)
				return;

			int lastSwatch = row + (col * _fitPerRow);
			if (e.Button == MouseButtons.Left)
				_lastLeftSwatch = lastSwatch;
			else
				_lastRightSwatch = lastSwatch;

			Invalidate();

			if (SwatchChanged == null)
				return;

			SwatchChanged(this, new SwatchChangedEventArgs(Swatch[lastSwatch].Color, e.Button));
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

					e.Graphics.FillRectangle(new SolidBrush(Swatch[index].Color),
					                         new Rectangle(1 + (i * (SwatchSize + 1)), 1 + (y * (SwatchSize + 1)), SwatchSize,
					                                       SwatchSize));

					if (_lastLeftSwatch == index)
					{
						e.Graphics.DrawRectangle(new Pen(Color.Yellow, 1),
						                         new Rectangle(1 + (i * (SwatchSize + 1)), 1 + (y * (SwatchSize + 1)), SwatchSize - 1,
						                                       SwatchSize - 1));
					}
					else if (_lastRightSwatch == index)
					{
						e.Graphics.DrawRectangle(new Pen(Color.Red, 1),
						                         new Rectangle(1 + (i * (SwatchSize + 1)), 1 + (y * (SwatchSize + 1)), SwatchSize - 1,
						                                       SwatchSize - 1));
					}
					else
					{
						e.Graphics.DrawRectangle(Pens.Black,
						                         new Rectangle(1 + (i * (SwatchSize + 1)), 1 + (y * (SwatchSize + 1)), SwatchSize - 1,
						                                       SwatchSize - 1));
					}
					index++;
				}

				y++;
			}
		}
	}
}