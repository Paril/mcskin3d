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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Paril.Controls.Color
{
	public class ColorPreview : Control
	{
		public ColorPreview()
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			DoubleBuffered = true;
		}

		public override System.Drawing.Color ForeColor
		{
			get { return base.ForeColor; }
			set
			{
				base.ForeColor = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var brush = new HatchBrush(HatchStyle.LargeCheckerBoard, System.Drawing.Color.Gray, System.Drawing.Color.LightGray);

			e.Graphics.FillRectangle(brush, ClientRectangle);

			e.Graphics.FillPolygon(new SolidBrush(System.Drawing.Color.FromArgb(255, ForeColor)), new[]
			                                                                                      {
			                                                                                      	new Point(Width, 0),
			                                                                                      	new Point(Width, Height),
			                                                                                      	new Point(0, Height)
			                                                                                      });

			e.Graphics.FillPolygon(new SolidBrush(ForeColor), new[]
			                                                  {
			                                                  	new Point(0, 0),
			                                                  	new Point(Width, 0),
			                                                  	new Point(0, Height)
			                                                  });

			ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Etched);

			base.OnPaint(e);
		}
	}
}