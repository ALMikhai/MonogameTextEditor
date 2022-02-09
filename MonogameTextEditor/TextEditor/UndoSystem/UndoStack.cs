using System.Collections.Generic;

namespace MonogameTextEditor.TextEditor.UndoSystem
{
	public class UndoStack
	{
		public bool CanUndo => undo.Count > 0;

		public bool CanRedo => redo.Count > 0;

		private Stack<IUndoItem> undo;
		private Stack<IUndoItem> redo;

		public UndoStack()
		{
			Reset();
		}

		public void Reset()
		{
			undo = new Stack<IUndoItem>();
			redo = new Stack<IUndoItem>();
		}

		public void Do(IUndoItem item)
		{
			item.Do();
			undo.Push(item);
			redo.Clear();
		}

		public void Add(IUndoItem item)
		{
			undo.Push(item);
			redo.Clear();
		}

		public void Undo()
		{
			if (CanUndo) {
				var item = undo.Pop();
				item.Undo();
				redo.Push(item);
			}
		}

		public void Redo()
		{
			if (CanRedo) {
				var item = redo.Pop();
				item.Do();
				undo.Push(item);
			}
		}
	}
}
