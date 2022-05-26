using System.Collections.Generic;

namespace MonogameTextEditor.TextEditor.UndoSystem
{
	public class UndoStack
	{
		public int MaxDepth { get; set; }

		public int Depth => undo.Count;

		public bool CanUndo => undo.Count > 0;

		public bool CanRedo => redo.Count > 0;

		private LinkedList<IUndoItem> undo;
		private LinkedList<IUndoItem> redo;

		public UndoStack(int maxDepth = 50)
		{
			MaxDepth = maxDepth;
			Reset();
		}

		public void Reset()
		{
			if (undo != null) {
				foreach (var item in undo) {
					item.Release();
				}
			}
			if (redo != null) {
				foreach (var item in redo) {
					item.Release();
				}
			}
			undo = new LinkedList<IUndoItem>();
			redo = new LinkedList<IUndoItem>();
		}

		public void Do(IUndoItem item)
		{
			item.Do();
			Add(item);
		}

		public void Add(IUndoItem item)
		{
			undo.AddFirst(item);
			foreach (var undoItem in redo) {
				undoItem.Release();
			}
			redo.Clear();
			if (undo.Count > MaxDepth) {
				var undoItem = undo.Last.Value;
				undoItem.Release();
				undo.RemoveLast();
			}
		}

		public void Undo()
		{
			if (!CanUndo) {
				return;
			}
			var item = undo.First.Value;
			undo.RemoveFirst();
			item.Undo();
			redo.AddFirst(item);
		}

		public void Redo()
		{
			if (!CanRedo) {
				return;
			}
			var item = redo.First.Value;
			redo.RemoveFirst();
			item.Do();
			undo.AddFirst(item);
		}

		public void HardUndo()
		{
			if (!CanUndo) {
				return;
			}
			var item = undo.First.Value;
			undo.RemoveFirst();
			item.Undo();
			item.Release();
		}
	}
}
