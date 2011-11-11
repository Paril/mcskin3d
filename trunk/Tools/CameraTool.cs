using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Paril.Compatibility;
using System.Windows.Forms;

namespace MCSkin3D
{
	public class CameraTool : ITool
	{
		public void BeginClick(Skin skin, Point p, MouseEventArgs e)
		{
			_oldMouse = e.Location;
		}

		Point _oldMouse;
		public void MouseMove(Skin skin, MouseEventArgs e)
		{
			var delta = new Point(e.X - _oldMouse.X, e.Y - _oldMouse.Y);

			if (e.Button == Program.MainForm.CameraRotate)
				Program.MainForm.RotateView(delta, 1);
			else if (e.Button == Program.MainForm.CameraZoom)
				Program.MainForm.ScaleView(delta, 1);

			_oldMouse = e.Location;
		}

		public bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y)
		{
			return false;
		}

		public bool RequestPreview(int[] pixels, Skin skin, int x, int y)
		{
			return false;
		}

		public bool EndClick(int[] pixels, Skin skin, MouseEventArgs e)
		{
			return false;
		}

		public string GetStatusLabelText()
		{
			return Editor.GetLanguageString("T_CAMERA");
		}
	}
}