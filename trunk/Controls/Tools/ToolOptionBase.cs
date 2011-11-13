using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace MCSkin3D
{
	public class ToolOptionBase : UserControl
	{
		public virtual void BoxShown() { }
		public virtual void BoxHidden() { }

		public ToolOptionBase()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			DoubleBuffered = true;
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
		}
	}
}
