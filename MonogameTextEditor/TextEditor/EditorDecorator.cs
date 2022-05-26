using MonogameTextEditor.TextEditor.Caret;
using MonogameTextEditor.TextEditor.TextCollection;
using MonogameTextEditor.TextEditor.UndoSystem;

namespace MonogameTextEditor.TextEditor
{
	public class EditorDecorator : ITextEditor
	{
		public ITextCollection Text => editor.Text;

		public ICaret Caret => editor.Caret;

		public ICaret SelectionStart => editor.SelectionStart;

		public ICaret SelectionEnd => editor.SelectionEnd;

		public UndoStack UndoStack => editor.UndoStack;

		private readonly ITextEditor editor;
		private UndoItemPool undoItemPool;

		public EditorDecorator(ITextEditor editor)
		{
			this.editor = editor;
			undoItemPool = new UndoItemPool();
		}

		public void RemoveForward()
		{
			if (HasSelection()) {
				RemoveSelection();
				return;
			}
			if (!MoveCaretRight(1)) {
				return;
			}
			var secondPos = Caret.Position;
			MoveCaretRight(-1);
			var firstPos = Caret.Position;
			UndoStack.Do(GetRemoveRangeUndoItem(firstPos, secondPos, firstPos));
		}

		public void RemoveBackward()
		{
			if (HasSelection()) {
				RemoveSelection();
				return;
			}
			if (!MoveCaretRight(-1)) {
				return;
			}
			var firstPos = Caret.Position;
			MoveCaretRight(1);
			var secondPos = Caret.Position;
			UndoStack.Do(GetRemoveRangeUndoItem(firstPos, secondPos, secondPos));
		}

		public void Insert(string s)
		{
			if (string.IsNullOrEmpty(s)) {
				return;
			}
			var undoItem = new MultipleUndoItem();
			if (HasSelection()) {
				var removeSelectUndoItem = GetRemoveSelectUndoItem();
				removeSelectUndoItem.Do();
				undoItem.Add(removeSelectUndoItem);
			}
			var insertUndoItem = GetInsertUndoItem(s);
			insertUndoItem.Do();
			undoItem.Add(insertUndoItem);
			UndoStack.Add(undoItem);
		}

		public void RemoveNextWord()
		{
			if (HasSelection()) {
				RemoveSelection();
				return;
			}
			var startPos = Caret.Position;
			ReadOnlyEditor.MoveCaretToNextWord(Caret, Text);
			var endPos = Caret.Position;
			if (startPos.Line != endPos.Line || startPos.Col != endPos.Col) {
				UndoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, startPos));
			}
		}

		public void RemovePrevWord()
		{
			if (HasSelection()) {
				RemoveSelection();
				return;
			}
			var endPos = Caret.Position;
			ReadOnlyEditor.MoveCaretToPrevWord(Caret, Text);
			var startPos = Caret.Position;
			if (startPos.Line != endPos.Line || startPos.Col != endPos.Col) {
				UndoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, endPos));
			}
		}

		public void Undo()
		{
			UndoStack.Undo();
		}

		public void Redo()
		{
			UndoStack.Redo();
		}

		public void HardUndo()
		{
			UndoStack.HardUndo();
		}

		public void RemoveSelection()
		{
			if (!HasSelection()) {
				return;
			}
			UndoStack.Do(GetRemoveSelectUndoItem());
		}

		public string Cut()
		{
			string result = GetSelectedText();
			if (!HasSelection()) {
				var caretPos = Caret.Position;
				ReadOnlyEditor.MoveCaretToLineStart(Caret, Text);
				var startPos = Caret.Position;
				MoveCaretToLineEnd();
				if (Text.Count > Caret.Line + 1) {
					MoveCaretRight(1);
				}
				var endPos = Caret.Position;
				if (startPos.Line != endPos.Line || startPos.Col != endPos.Col) {
					UndoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, caretPos));
				}
			} else {
				RemoveSelection();
			}
			return result;
		}

		public bool MoveCaretRight(int n)
		{
			return editor.MoveCaretRight(n);
		}

		public bool MoveCaretDown(int n)
		{
			return editor.MoveCaretDown(n);
		}

		public string GetCurrentLine()
		{
			return editor.GetCurrentLine();
		}

		public char GetCurrentChar()
		{
			return editor.GetCurrentChar();
		}

		public void MoveCaretToNextWord()
		{
			editor.MoveCaretToNextWord();
		}

		public void MoveCaretToPrevWord()
		{
			editor.MoveCaretToPrevWord();
		}

		public void MoveCaretToLineEnd()
		{
			editor.MoveCaretToLineEnd();
		}

		public void MoveCaretToLineStart()
		{
			editor.MoveCaretToLineStart();
		}

		public void MoveCaretToTextStart()
		{
			editor.MoveCaretToTextStart();
		}

		public void MoveCaretToTextEnd()
		{
			editor.MoveCaretToTextEnd();
		}

		public void Reset()
		{
			editor.Reset();
		}

		public void MoveSelectionRight(int n)
		{
			editor.MoveSelectionRight(n);
		}

		public void MoveSelectionDown(int n)
		{
			editor.MoveSelectionDown(n);
		}

		public void ClearSelection()
		{
			editor.ClearSelection();
		}

		public bool HasSelection()
		{
			return editor.HasSelection();
		}

		public string GetSelectedText()
		{
			return editor.GetSelectedText();
		}

		public void MoveSelectionToNextWord()
		{
			editor.MoveSelectionToNextWord();
		}

		public void MoveSelectionToPrevWord()
		{
			editor.MoveSelectionToPrevWord();
		}

		public void SelectWordAtCaretPos()
		{
			editor.SelectWordAtCaretPos();
		}

		public void SelectAll()
		{
			editor.SelectAll();
		}

		public void MoveSelectionToLineStart()
		{
			editor.MoveSelectionToLineStart();
		}

		public void MoveSelectionToLineEnd()
		{
			editor.MoveSelectionToLineEnd();
		}

		public (ICaret firstCaret, ICaret secondCaret) GetSortedCarets()
		{
			return editor.GetSortedCarets();
		}

		public void CommonInsert(string s)
		{
			var newPosition = Text.Insert(Caret.Position, s);
			Caret.SetPosition(newPosition);
		}

		private IUndoItem GetInsertUndoItem(string text)
		{
			var undoItem = undoItemPool.Acquire<InsertUndoItem>();
			undoItem.CaretPos = Caret.Position;
			undoItem.Text = text.Replace("\r\n", "\n");
			undoItem.TextEditor = this;
			return undoItem;
		}

		private IUndoItem GetRemoveSelectUndoItem()
		{
			var undoItem = undoItemPool.Acquire<RemoveSelectUndoItem>();
			undoItem.SelectionStart = SelectionStart.Position;
			undoItem.SelectionEnd = SelectionEnd.Position;
			undoItem.Text = GetSelectedText();
			var (firstCaret, secondCaret) = GetSortedCarets();
			undoItem.FirstCaret = firstCaret.Position;
			undoItem.SecondCaret = secondCaret.Position;
			undoItem.TextEditor = this;
			return undoItem;
		}

		private IUndoItem GetRemoveRangeUndoItem(
			TextPosition first,
			TextPosition second,
			TextPosition undoCaretPos
		)
		{
			var undoItem = undoItemPool.Acquire<RemoveRangeUndoItem>();
			undoItem.Text = Text.GetTextRange(first, second);
			undoItem.First = first;
			undoItem.Second = second;
			undoItem.UndoCaretPos = undoCaretPos;
			undoItem.TextEditor = this;
			return undoItem;
		}
	}
}
