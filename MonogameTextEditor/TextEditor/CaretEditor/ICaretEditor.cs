using MonogameTextEditor.TextEditor.Caret;
using MonogameTextEditor.TextEditor.TextCollection;

namespace MonogameTextEditor.TextEditor.CaretEditor
{
	public interface ICaretEditor
	{
		ICaret Caret { get; }
		ITextCollection Text { get; }
		void RemoveForward();
		void RemoveBackward();
		void Insert(string s);
		bool MoveCaretRight(int n);
		bool MoveCaretDown(int n);
		string GetCurrentLine();
		(int Line, int Col) GetNextWordPos();
		(int Line, int Col) GetPrevWordPos();
		void MoveCaretToNextWord();
		void MoveCaretToPrevWord();
		void MoveCaretToLineEnd();
		void MoveCaretToLineStart();
	}
}
