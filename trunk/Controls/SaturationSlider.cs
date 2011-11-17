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

namespace Paril.Controls.Color
{
	public partial class SaturationSlider : Control
	{
		#region Component Designer generated code
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion

		public SaturationSlider()
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer | 
				ControlStyles.UserMouse | 
				ControlStyles.UserPaint | 
				ControlStyles.AllPaintingInWmPaint, true);
		}

		Devcorp.Controls.Design.HSL _color;

		public Devcorp.Controls.Design.HSL Color
		{
			get { return _color; }
			set { _color = value; Invalidate(); }
		}

		int _curLum = 0;

		public int CurrentLum
		{
			get { return _curLum; }
			set { _curLum = value; OnLumChanged(EventArgs.Empty); Invalidate(); }
		}

		protected virtual void OnLumChanged(EventArgs e)
		{
			if (LumChanged != null)
				LumChanged(this, e);
		}

		public event EventHandler LumChanged;

		void CheckClick(MouseEventArgs e)
		{
			Rectangle borderThing = new Rectangle(0, 8, Width - 8, Height - 16);

			if (e.Y <= borderThing.Y)
				CurrentLum = 240;
			else if (e.Y >= borderThing.Y + borderThing.Height)
				CurrentLum = 0;
			else
			{
				float div = (float)(e.Y - 8) / (float)borderThing.Height;

				CurrentLum = 240 - (int)(div * 240);
			}

			Invalidate();
		}

		bool _down = false;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			_down = true;
			CheckClick(e);
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_down)
				CheckClick(e);
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_down = false;
			CheckClick(e);
			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Rectangle borderThing = new Rectangle(0, 8, Width - 8, Height - 16);

			var half = Color;
			half.Luminance = 0.5f;
			ColorSpaceRenderer.GenerateColorSlider(e.Graphics, Devcorp.Controls.Design.ColorSpaceHelper.HSLtoRGB(half).ToColor(), borderThing);

			ControlPaint.DrawBorder3D(e.Graphics, borderThing, Border3DStyle.SunkenOuter);

			float inc = (float)(Height - 18) / 240.0f;

			float invLum = (float)(240 - _curLum);

			e.Graphics.FillPolygon(Brushes.Black,
				new Point[]
				{
					new Point(Width - 7, 6 + (int)(invLum * inc) + 2),
					new Point(Width - 1, 12 + (int)(invLum * inc) + 2),
					new Point(Width - 1, 0 + (int)(invLum * inc) + 2),
				}
			);
		}
	}
}
