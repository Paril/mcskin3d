using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			HatchBrush brush = new HatchBrush(HatchStyle.LargeCheckerBoard, System.Drawing.Color.Gray, System.Drawing.Color.LightGray);

			e.Graphics.FillRectangle(brush, ClientRectangle);

			e.Graphics.FillPolygon(new SolidBrush(System.Drawing.Color.FromArgb(255, ForeColor)), new Point[]
 			{
				new Point(Width, 0),
				new Point(Width, Height),
				new Point(0, Height)
			});

			e.Graphics.FillPolygon(new SolidBrush(ForeColor), new Point[]
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
