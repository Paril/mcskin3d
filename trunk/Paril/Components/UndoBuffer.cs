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
