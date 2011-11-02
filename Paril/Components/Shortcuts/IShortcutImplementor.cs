using System;
using System.Windows.Forms;

namespace Paril.Components.Shortcuts
{
	public interface IShortcutImplementor
	{
		string Name { get; }
		Keys Keys { get; set; }
		Action Pressed { get; set; }

		bool CanEvaluate();
	}
}
