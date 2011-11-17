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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paril.Components
{
	public class UndoBuffer
	{
		List<IUndoable> Undos = new List<IUndoable>();
		public int _depth = -1;
		public object Object;

		public int CurrentIndex
		{
			get
			{
				if (_depth == -1)
					return Undos.Count;
				return _depth;
			}

			set
			{
				_depth = value;

				if (_depth == Undos.Count)
					_depth = -1;
			}
		}

		public UndoBuffer(object obj)
		{
			Object = obj;
		}

		public void AddBuffer(IUndoable undoable)
		{
			if (CurrentIndex == Undos.Count)
				Undos.Add(undoable);
			else
			{
				Undos.RemoveRange(CurrentIndex, Undos.Count - CurrentIndex);
				Undos.Add(undoable);
				CurrentIndex = Undos.Count;
			}
		}

		public bool CanUndo
		{
			get { return CurrentIndex != 0; }
		}

		public bool CanRedo
		{
			get { return Undos.Count > CurrentIndex; }
		}

		public void Undo()
		{
			CurrentIndex--;
			Undos[CurrentIndex].Undo(Object);
		}

		public void Redo()
		{
			Undos[CurrentIndex].Redo(Object);
			CurrentIndex++;
		}

		public void Clear()
		{
			Undos.Clear();
			_depth = -1;
		}
	}
}
