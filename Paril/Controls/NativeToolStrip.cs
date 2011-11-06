using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szotar.WindowsForms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;

namespace Paril.Controls
{
	static class NativeStripInternals
	{
		internal static ToolStripAeroRenderer Renderer = null;

		static NativeStripInternals()
		{
			if (VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser)
				Renderer = new ToolStripAeroRenderer(ToolbarTheme.Toolbar);
		}
	}

	class NativeToolStrip : ToolStrip
	{
		public NativeToolStrip()
		{
			Renderer = NativeStripInternals.Renderer;
		}
	}

	class NativeToolStripContainer : ToolStripContainer
	{
		public NativeToolStripContainer()
		{
			TopToolStripPanel.Renderer = NativeStripInternals.Renderer;
			RightToolStripPanel.Renderer = NativeStripInternals.Renderer;
			LeftToolStripPanel.Renderer = NativeStripInternals.Renderer;
			BottomToolStripPanel.Renderer = NativeStripInternals.Renderer;
		}
	}

	class NativeMenuStrip : MenuStrip
	{
		public NativeMenuStrip()
		{
			Renderer = NativeStripInternals.Renderer;
		}
	}
}
