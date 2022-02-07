using System;
using System.Collections.Generic;

namespace MonogameTextEditor.TextEditor
{
	public interface IUndoItem
	{
		public void Do();
		public void Undo();
	}

	public class SingleUndoItem : IUndoItem
	{
		private readonly Action @do;
		private readonly Action undo;

		public SingleUndoItem(Action @do, Action undo)
		{
			this.@do = @do;
			this.undo = undo;
		}

		void IUndoItem.Do()
		{
			@do();
		}

		void IUndoItem.Undo()
		{
			undo();
		}
	}

	public class MultipleUndoItem : IUndoItem
	{
		private readonly List<IUndoItem> items = new List<IUndoItem>();

		public void Add(IUndoItem item)
		{
			items.Add(item);
		}

		void IUndoItem.Do()
		{
			foreach (var item in items)
				item.Do();
		}

		void IUndoItem.Undo()
		{
			for (var i = items.Count - 1; i >= 0; i--)
				items[i].Undo();
		}
	}

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

		public IUndoItem PopUndo()
		{
			return undo.Pop();
		}

		public IUndoItem PopRedo()
		{
			return redo.Pop();
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
