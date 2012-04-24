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

namespace Paril.Components.Shortcuts
{
	public class ShortcutBase : IShortcutImplementor
	{
		private readonly string _saveName;
		private string _name;

		public ShortcutBase(string name, Keys keys)
		{
			_saveName = _name = name;
			Keys = keys;
		}

		#region IShortcutImplementor Members

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string SaveName
		{
			get { return _saveName; }
		}

		public Keys Keys { get; set; }
		public Action Pressed { get; set; }

		public bool CanEvaluate()
		{
			return true;
		}

		#endregion

		public override string ToString()
		{
			return _name + " [" + ShortcutEditor.KeysToString(Keys) + "]";
		}
	}
}