using System;

namespace Paril.Components
{
	public interface IUndoable
	{
		void Undo(object obj);
		void Redo(object obj);
	}
}
