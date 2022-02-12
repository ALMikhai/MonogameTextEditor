namespace TextEditor.TextCollection
{
	public interface ITextCollection
	{
		(int NewLine, int NewCol) Insert(int line, int col, string text);
		void Remove(int line, int col, int lenght);
		void InsertLine(int line, string text);
		void RemoveLine(int line);
		public void RemoveRange((int Line, int Col) left, (int Line, int Col) right);
		string GetLine(int line);
		string GetTextRange((int Line, int Col) left, (int Line, int Col) right);
		int GetLineLenght(int line);
		int GetLineCount();
		string this[int line] => GetLine(line);
		char GetSymbol(int line, int col);
	}
}
