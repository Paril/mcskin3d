using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Paril.OpenGL;

namespace MCSkin3D.Controls
{
	public class ModelToolStripMenuItem : ToolStripMenuItem
	{
		public Model Model;
		public bool IsMain;
		public Action<Model> Callback;

		public ModelToolStripMenuItem(Model model, bool main, Action<Model> callback) :
			base(model.Name)
		{
			Name = model.Name;
			Model = model;
			IsMain = main;
			Callback = callback;

			if (IsMain)
				Model.DropDownItem = this;
		}

		protected override void OnClick(EventArgs e)
		{
			Callback(Model);
			base.OnClick(e);
		}
	}
}
