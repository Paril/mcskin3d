using System;
using System.Windows.Forms;
using MCSkin3D;

namespace Paril.Components.Shortcuts
{
	public class ShortcutBase : IShortcutImplementor
	{
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

		public Keys Keys { get; set; }
		public Action Pressed { get; set; }
		public bool CanEvaluate() { return true; }

		public ShortcutBase(string name, Keys keys)
		{
			_saveName = _name = name;
			Keys = keys;
		}

		public override string ToString()
		{
			return _name + " [" + ShortcutEditor.KeysToString(Keys) + "]";
		}
	}
}
