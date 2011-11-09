using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCSkin3D
{
	public partial class EraserOptions : ToolOptionBase
	{
		public EraserOptions()
		{
			InitializeComponent();
		}

		public override void BoxShown()
		{
			Controls.Add(Brushes.BrushBox);
			Brushes.BrushBox.Location = new Point(2, 2);
		}

		public override void BoxHidden()
		{
			Controls.Remove(Brushes.BrushBox);
		}
	}
}
