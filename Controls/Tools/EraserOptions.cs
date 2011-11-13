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
			groupBox1.Controls.Add(Brushes.BrushBox);
			Brushes.BrushBox.Location = new Point((groupBox1.Width / 2) - (Brushes.BrushBox.Width / 2), (groupBox1.Height / 2) - (Brushes.BrushBox.Height / 2));
		}

		public override void BoxHidden()
		{
			groupBox1.Controls.Remove(Brushes.BrushBox);
		}

		private void EraserOptions_Load(object sender, EventArgs e)
		{

		}
	}
}
