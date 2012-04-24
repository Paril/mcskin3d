using System.Drawing;
using System.Windows.Forms;

namespace MCSkin3D.Controls
{
	public class ColorToolStripMenuItem : ToolStripMenuItem
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.FillRectangle(new SolidBrush(BackColor),
			                         new Rectangle(ContentRectangle.X + 6, ContentRectangle.Y + 1, 17, 17));
			e.Graphics.DrawRectangle(Pens.Black, new Rectangle(ContentRectangle.X + 6, ContentRectangle.Y + 1, 17, 17));
		}
	}
}