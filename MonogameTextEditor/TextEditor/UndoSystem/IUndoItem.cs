namespace MonogameTextEditor.TextEditor.UndoSystem
{
	public interface IUndoItem
	{
		public UndoItemPool PoolCreator { get; set; }

		public void Do();

		public void Undo();

		public void Release();
	}
}
