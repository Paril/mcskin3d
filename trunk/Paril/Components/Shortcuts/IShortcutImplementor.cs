using System;
using System.Windows.Forms;

namespace Paril.Components.Shortcuts
{
	public interface IShortcutImplementor
	{
		string Name { get; set; }
		string SaveName { get; }
		Keys Keys { get; set; }
		Action Pressed { get; set; }

		bool CanEvaluate();
	}
}
