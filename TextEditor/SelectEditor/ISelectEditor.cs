using System.Threading;
using TextEditor.Caret;
using TextEditor.CaretEditor;
using TextEditor.TextCollection;

namespace TextEditor.SelectEditor
{
	public interface ISelectEditor : ICaretEditor
	{
		ICaretEditor CaretEditor { get; }
		ICaret SelectionStart { get; }
		ICaret SelectionEnd { get; }
		// ITextCollection Text { get; }
		bool MoveSelectionRight(int n);
		bool MoveSelectionDown(int n);
		// void Insert(string text);
		void RemoveSelection();
		void ClearSelection();
		bool HasSelection();
		// void RemoveForward();
		// void RemoveBackward();
		// bool MoveCaretRight(int n);
		// bool MoveCaretDown(int n);
		string GetSelectedText();
		void MoveSelectionToNextWord();
		void MoveSelectionToPrevWord();
		// void MoveCaretToNextWord();
		// void MoveCaretToPrevWord();
		void SelectAll();
		// void MoveCaretToLineEnd();
		// void MoveCaretToLineStart();
		string Cut();
		void MoveSelectionToLineStart();
		void MoveSelectionToLineEnd();
		void RemoveNextWord();
		void RemovePrevWord();
		void Undo();
		void Redo();
		delegate void InsertText(int line, int col, string text);
		delegate void RemoveText(int line, int col, int lenght);
		event InsertText OnTextInsert;
		event RemoveText OnTextRemove;
	}
}
