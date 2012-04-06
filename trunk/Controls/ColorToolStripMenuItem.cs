using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MCSkin3D.Controls
{
	public class ColorToolStripMenuItem : ToolStripMenuItem
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.FillRectangle(new System.Drawing.SolidBrush(BackColor), new System.Drawing.Rectangle(ContentRectangle.X + 6, ContentRectangle.Y + 1, 17, 17));
			e.Graphics.DrawRectangle(Pens.Black, new System.Drawing.Rectangle(ContentRectangle.X + 6, ContentRectangle.Y + 1, 17, 17));
		}
	}
}
