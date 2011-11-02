using System;
using System.Windows.Forms;

namespace Paril.Components.Shortcuts
{
	public class ShortcutBase : IShortcutImplementor
	{
		string _name;
		public string Name
		{
			get { return _name; }
		}

		public Keys Keys { get; set; }
		public Action Pressed { get; set; }
		public bool CanEvaluate() { return true; }

		public ShortcutBase(string name, Keys keys)
		{
			_name = name;
			Keys = keys;
		}

		public override string ToString()
		{
			return _name + " [" + ShortcutEditor.KeysToString(Keys) + "]";
		}
	}
}
