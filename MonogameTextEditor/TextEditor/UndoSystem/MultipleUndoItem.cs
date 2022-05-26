using System.Collections.Generic;

namespace MonogameTextEditor.TextEditor.UndoSystem
{
	public class MultipleUndoItem : IUndoItem
	{
		public UndoItemPool PoolCreator { get; set; }

		private readonly List<IUndoItem> items = new List<IUndoItem>();

		public void Add(IUndoItem item)
		{
			items.Add(item);
		}

		void IUndoItem.Do()
		{
			foreach (var item in items) {
				item.Do();
			}
		}

		void IUndoItem.Undo()
		{
			for (int i = items.Count - 1; i >= 0; i--) {
				items[i].Undo();
			}
		}

		public void Release()
		{
			foreach (var item in items) {
				item.Release();
			}
		}
	}
}
