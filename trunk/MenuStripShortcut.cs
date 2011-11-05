using System;
using System.Windows.Forms;
using Paril.Components.Shortcuts;

namespace MCSkin3D
{
	public class MenuStripShortcut : IShortcutImplementor
	{
		ToolStripMenuItem _menuItem;

		string _name;
		public string Name
		{
			get { return _name; }
		}

		Keys _keys;
		public Keys Keys
		{
			get { return _keys; }
			set { _keys = value; _menuItem.ShortcutKeyDisplayString = ShortcutEditor.KeysToString(_keys); }
		}

		public Action Pressed { get; set; }

		public bool CanEvaluate() { return true; }

		public MenuStripShortcut(ToolStripMenuItem item) :
			this(item, item.ShortcutKeys)
		{
			_menuItem.ShortcutKeys = 0;
		}

		public MenuStripShortcut(ToolStripMenuItem item, Keys keys)
		{
			_menuItem = item;
			_name = _menuItem.Text.Replace("&", "");
			Keys = keys;
		}

		public override string ToString()
		{
			return Name + " [" + ShortcutEditor.KeysToString(Keys) + "]";
		}
	}
}
