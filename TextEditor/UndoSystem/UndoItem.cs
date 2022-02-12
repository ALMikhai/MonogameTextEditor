using System;

namespace TextEditor.UndoSystem
{
	public class UndoItem : IUndoItem
	{
		private readonly Action @do;
		private readonly Action undo;

		public UndoItem(Action @do, Action undo)
		{
			this.@do = @do;
			this.undo = undo;
		}

		void IUndoItem.Do() => @do();

		void IUndoItem.Undo() => undo();
	}
}
