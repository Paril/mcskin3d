using System;
using System.Windows.Forms;
using System.Drawing;

namespace MCSkin3D
{
	public interface ITool
	{
		void BeginClick(Skin skin, Point p, MouseEventArgs e);
		void MouseMove(Skin skin, MouseEventArgs e);
		bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y);
		bool RequestPreview(int[] pixels, Skin skin, int x, int y);
		void EndClick(Skin skin, MouseEventArgs e);
		string GetStatusLabelText();
	}
}
