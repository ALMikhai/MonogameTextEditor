using MonogameTextEditor.TextEditor.Caret;
using MonogameTextEditor.TextEditor.CaretEditor;
using MonogameTextEditor.TextEditor.TextCollection;

namespace MonogameTextEditor.TextEditor.SelectEditor
{
	public interface ISelectEditor
	{
		ICaretEditor CaretEditor { get; }
		ICaret SelectionStart { get; }
		ICaret SelectionEnd { get; }
		ITextCollection Text { get; }
		bool MoveSelectionRight(int n);
		bool MoveSelectionDown(int n);
		void Insert(string text);
		void RemoveSelection();
		void ClearSelection();
		bool HasSelection();
		void RemoveForward();
		void RemoveBackward();
		bool MoveCaretRight(int n);
		bool MoveCaretDown(int n);
		string GetSelectedText();
		void MoveSelectionToNextWord();
		void MoveSelectionToPrevWord();
		void MoveCaretToNextWord();
		void MoveCaretToPrevWord();
		void SelectAll();
		void MoveCaretToLineEnd();
		void MoveCaretToLineStart();
		string Cut();
		void MoveSelectionToLineStart();
		void MoveSelectionToLineEnd();
		void RemoveNextWord();
		void RemovePrevWord();
		void Undo();
		void Redo();
	}
}
