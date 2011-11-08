using System;
using System.Windows.Forms;

namespace MCSkin3D
{
	public interface ITool
	{
		void BeginClick(Skin skin, MouseEventArgs e);
		void MouseMove(Skin skin, MouseEventArgs e);
		bool MouseMoveOnSkin(int[] pixels, Skin skin, int x, int y);
		bool RequestPreview(int[] pixels, Skin skin, int x, int y);
		void EndClick(Skin skin, MouseEventArgs e);
	}
}
