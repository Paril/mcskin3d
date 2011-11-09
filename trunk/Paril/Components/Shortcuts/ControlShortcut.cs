using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Paril.Components.Shortcuts
{
	/// <summary>
	/// Defines a shortcut that requires its control to be focused in order to work.
	/// </summary>
	public class ControlShortcut : IShortcutImplementor
	{
		Control _owner;
		public Control Control
		{
			get { return _owner; }
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		string _saveName;
		public string SaveName
		{
			get { return _saveName; }
		}

		Keys _keys;
		public Keys Keys
		{
			get { return _keys; }
			set { _keys = value; }
		}

		public Action Pressed { get; set; }

		public bool CanEvaluate()
		{
			return Control.Focused;
		}

		public ControlShortcut(string name, Keys keys, Control owner)
		{
			_owner = owner;
			_name = _saveName = name;
			Keys = keys;
		}

		public override string ToString()
		{
			return Name + " [" + ShortcutEditor.KeysToString(Keys) + "]";
		}

	}
}
