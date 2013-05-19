//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Szotar.WindowsForms;

namespace Paril.Controls
{
	internal static class NativeStripInternals
	{
		internal static ToolStripAeroRenderer Renderer;

		static NativeStripInternals()
		{
			if (VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
				Renderer = new ToolStripAeroRenderer(ToolbarTheme.Toolbar);
		}
	}

	internal class NativeToolStrip : ToolStrip
	{
		public NativeToolStrip()
		{
			Renderer = NativeStripInternals.Renderer;
		}
	}

	internal class NativeToolStripContainer : ToolStripContainer
	{
		public NativeToolStripContainer()
		{
			TopToolStripPanel.Renderer = NativeStripInternals.Renderer;
			RightToolStripPanel.Renderer = NativeStripInternals.Renderer;
			LeftToolStripPanel.Renderer = NativeStripInternals.Renderer;
			BottomToolStripPanel.Renderer = NativeStripInternals.Renderer;
		}
	}

	internal class NativeMenuStrip : MenuStrip
	{
		public NativeMenuStrip()
		{
			Renderer = NativeStripInternals.Renderer;
		}
	}
}