namespace MonogameTextEditor.TextEditor.UndoSystem
{
	public interface IUndoItem
	{
		public void Do();
		public void Undo();
	}
}
