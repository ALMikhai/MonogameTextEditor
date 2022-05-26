using MonogameTextEditor.TextEditor.Caret;
using MonogameTextEditor.TextEditor.TextCollection;
using MonogameTextEditor.TextEditor.UndoSystem;

namespace MonogameTextEditor.TextEditor
{
	public interface ITextEditor
	{
		ITextCollection Text { get; }

		ICaret Caret { get; }

		ICaret SelectionStart { get; }

		ICaret SelectionEnd { get; }

		UndoStack UndoStack { get; }

		void RemoveForward();

		void RemoveBackward();

		void Insert(string s);

		bool MoveCaretRight(int n);

		bool MoveCaretDown(int n);

		string GetCurrentLine();

		char GetCurrentChar();

		void MoveCaretToNextWord();

		void MoveCaretToPrevWord();

		void MoveCaretToLineEnd();

		void MoveCaretToLineStart();

		void MoveCaretToTextStart();

		void MoveCaretToTextEnd();

		void Reset();

		void MoveSelectionRight(int n);

		void MoveSelectionDown(int n);

		void RemoveSelection();

		void ClearSelection();

		bool HasSelection();

		string GetSelectedText();

		void MoveSelectionToNextWord();

		void MoveSelectionToPrevWord();

		void SelectWordAtCaretPos();

		void SelectAll();

		string Cut();

		void MoveSelectionToLineStart();

		void MoveSelectionToLineEnd();

		void RemoveNextWord();

		void RemovePrevWord();

		void Undo();

		void Redo();

		void HardUndo();

		public (ICaret firstCaret, ICaret secondCaret) GetSortedCarets();
	}
}
