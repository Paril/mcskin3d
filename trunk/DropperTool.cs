using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class DropperTool : ITool
	{
		public void BeginClick(Skin skin, MouseEventArgs e)
		{
		}

		public void MouseMove(Skin skin, MouseEventArgs e)
		{
		}

		public bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			var pixNum = x + (skin.Width * y);
			var c = pixels[pixNum];
			var oldColor = Color.FromArgb((c >> 24) & 0xFF, (c >> 0) & 0xFF, (c >> 8) & 0xFF, (c >> 16) & 0xFF);

			if ((Control.ModifierKeys & Keys.Shift) != 0)
				Program.MainForm.UnselectedColor = oldColor;
			else
				Program.MainForm.SelectedColor = oldColor;
			return false;
		}

		public bool RequestPreview(int[] pixels, Skin skin, int x, int y)
		{
			return false;
		}

		public void EndClick(Skin skin, MouseEventArgs e)
		{
		}
	}
}
