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
		public SwatchContainer()
		{
			InitializeComponent();
			swatchDisplayer1.ScrollBar = vScrollBar1;
		}

		public event EventHandler<SwatchChangedEventArgs> SwatchChanged
		{
			add
			{
				swatchDisplayer1.SwatchChanged += value;
			}

			remove
			{
				swatchDisplayer1.SwatchChanged -= value;
			}
		}

		private void SwatchContainer_Load(object sender, EventArgs e)
		{
			SetZoomAbility();
		}

		public void AddDirectory(string dir)
		{
			foreach (var swatchFile in Directory.GetFiles(dir, "*"))
			{
				var ext = Path.GetExtension(swatchFile);

				ISwatch swatch = null;

				if (ext.ToLower() == ".swtch")
					comboBox1.Items.Add(swatch = new MCSwatch(swatchFile));
				else if (ext.ToLower() == ".gpl" || ext.ToLower() == ".gimp")
					comboBox1.Items.Add(swatch = new GIMPSwatch(swatchFile));
				else if (ext.ToLower() == ".act")
					comboBox1.Items.Add(swatch = new ACTSwatch(swatchFile));
				else if (ext.ToLower() == ".aco")
					comboBox1.Items.Add(swatch = new ACOSwatch(swatchFile));

				if (swatch != null)
				{
					swatch.Load();

					if (comboBox1.SelectedItem == null && comboBox1.Items.Count != 0)
						comboBox1.SelectedIndex = 0;
				}
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem == null)
			{
				swatchDisplayer1.Swatch = null;
				return;
			}

			swatchDisplayer1.Swatch = (comboBox1.SelectedItem as ISwatch);
		}

		void SetZoomAbility()
		{
			toolStripButton1.Enabled = (swatchDisplayer1.Scale != 0);
		}

		public void ZoomOut()
		{
			swatchDisplayer1.ZoomOut();
			SetZoomAbility();
		}

		public void ZoomIn()
		{
			swatchDisplayer1.ZoomIn();
			SetZoomAbility();
		}

		public SwatchDisplayer SwatchDisplayer
		{
			get { return swatchDisplayer1; }
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			ZoomOut();
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			ZoomIn();
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{

		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{

		}

		private void toolStripButton5_Click(object sender, EventArgs e)
		{

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

		void RecalculateSize()
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

			_fitPerRow = (int)Math.Floor(((float)(Width - 3) / (float)(SwatchSize + 1)));

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
			set { _colors = value; _lastLeftSwatch = _lastRightSwatch = -1;  RecalculateSize(); }
		}
	}
}
