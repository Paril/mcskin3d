//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2011-2012 Altered Softworks & MCSkin3D Team
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
			_name = _saveName = _menuItem.Text.Replace("&", "");
			Keys = keys;
		}

		public override string ToString()
		{
			return Name + " [" + ShortcutEditor.KeysToString(Keys) + "]";
		}
	}
}
