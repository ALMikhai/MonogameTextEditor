using TextEditor.Caret;
using TextEditor.CaretEditor;
using TextEditor.TextCollection;
using TextEditor.UndoSystem;

namespace TextEditor.SelectEditor
{
	public class SelectEditor : ISelectEditor
	{
		public event ISelectEditor.InsertText OnTextInsert;

		public event ISelectEditor.RemoveText OnTextRemove;

		public ICaretEditor CaretEditor { get; }

		public ICaret SelectionStart { get; } = new global::TextEditor.Caret.Caret();

		public ICaret SelectionEnd { get; } = new global::TextEditor.Caret.Caret();

		public ICaret Caret => CaretEditor.Caret;

		public ITextCollection Text => CaretEditor.Text;


		private readonly UndoStack undoStack = new UndoStack();

		public SelectEditor(ICaretEditor caretEditor)
		{
			CaretEditor = caretEditor;
		}

		public bool MoveSelectionRight(int n)
		{
			if (!HasSelection())
				ClearSelection();
			if (CaretEditor.MoveCaretRight(n)) {
				SelectionEnd.AssignFrom(CaretEditor.Caret);
				return true;
			}
			return false;
		}

		public bool MoveSelectionDown(int n)
		{
			if (!HasSelection())
				ClearSelection();
			if (CaretEditor.MoveCaretDown(n)) {
				SelectionEnd.AssignFrom(CaretEditor.Caret);
				return true;
			}
			return false;
		}

		public void Insert(string text)
		{
			if (string.IsNullOrEmpty(text))
				return;
			var undoItem = new MultipleUndoItem();
			if (HasSelection()) {
				var removeSelectUndoItem = GetRemoveSelectUndoItem();
				removeSelectUndoItem.Do();
				undoItem.Add(removeSelectUndoItem);
			}
			var insertUndoItem = GetInsertUndoItem(text);
			insertUndoItem.Do();
			undoItem.Add(insertUndoItem);
			undoStack.Add(undoItem);
		}

		public void RemoveSelection()
		{
			if (!HasSelection())
				return;
			undoStack.Do(GetRemoveSelectUndoItem());
		}

		public void ClearSelection()
		{
			SelectionStart.AssignFrom(CaretEditor.Caret);
			SelectionEnd.AssignFrom(CaretEditor.Caret);
		}

		public bool HasSelection() => !SelectionStart.Equals(SelectionEnd);

		public void RemoveForward()
		{
			if (HasSelection()) {
				RemoveSelection();
				return;
			}
			if (!MoveCaretRight(1))
				return;
			var secondPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			MoveCaretRight(-1);
			var firstPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			undoStack.Do(GetRemoveRangeUndoItem(firstPos, secondPos, firstPos));
		}

		public void RemoveBackward()
		{
			if (HasSelection()) {
				RemoveSelection();
				return;
			}
			if (!MoveCaretRight(-1))
				return;
			var firstPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			MoveCaretRight(1);
			var secondPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			undoStack.Do(GetRemoveRangeUndoItem(firstPos, secondPos, secondPos));
		}

		public bool MoveCaretRight(int n)
		{
			if (!HasSelection())
				return CaretEditor.MoveCaretRight(n);
			var (firstCaret, secondCaret) = GetSortedCarets();
			CaretEditor.Caret.AssignFrom(n < 0 ? firstCaret : secondCaret);
			ClearSelection();
			return true;
		}

		public bool MoveCaretDown(int n)
		{
			if (HasSelection()) {
				var (firstCaret, secondCaret) = GetSortedCarets();
				CaretEditor.Caret.AssignFrom(n < 0 ? firstCaret : secondCaret);
				ClearSelection();
			}
			return CaretEditor.MoveCaretDown(n);
		}
		public string GetCurrentLine() => CaretEditor.GetCurrentLine();

		public (int Line, int Col) GetNextWordPos() => CaretEditor.GetNextWordPos();

		public (int Line, int Col) GetPrevWordPos() => CaretEditor.GetPrevWordPos();

		public string GetSelectedText()
		{
			if (!HasSelection())
				return CaretEditor.GetCurrentLine();
			var (firstCaret, secondCaret) = GetSortedCarets();
			return Text.GetTextRange((firstCaret.Line, firstCaret.Col), (secondCaret.Line, secondCaret.Col));
		}

		public void MoveSelectionToNextWord()
		{
			if (!HasSelection())
				ClearSelection();
			CaretEditor.MoveCaretToNextWord();
			SelectionEnd.AssignFrom(CaretEditor.Caret);
		}

		public void MoveSelectionToPrevWord()
		{
			if (!HasSelection())
				ClearSelection();
			CaretEditor.MoveCaretToPrevWord();
			SelectionEnd.AssignFrom(CaretEditor.Caret);
		}

		public void MoveCaretToNextWord()
		{
			if (HasSelection())
				MoveCaretRight(1);
			CaretEditor.MoveCaretToNextWord();
		}

		public void MoveCaretToPrevWord()
		{
			if (HasSelection())
				MoveCaretRight(-1);
			CaretEditor.MoveCaretToPrevWord();
		}

		public void SelectAll()
		{
			SelectionStart.Line = 0;
			SelectionStart.Col = 0;
			SelectionEnd.Line = Text.GetLineCount() - 1;
			SelectionEnd.Col = Text[SelectionEnd.Line].Length;
			CaretEditor.Caret.AssignFrom(SelectionEnd);
		}

		public void MoveCaretToLineEnd()
		{
			if (HasSelection()) {
				var (_, secondCaret) = GetSortedCarets();
				CaretEditor.Caret.AssignFrom(secondCaret);
				ClearSelection();
			}
			CaretEditor.MoveCaretToLineEnd();
		}

		public void MoveCaretToLineStart()
		{
			if (HasSelection()) {
				var (firstCaret, _) = GetSortedCarets();
				CaretEditor.Caret.AssignFrom(firstCaret);
				ClearSelection();
			}
			CaretEditor.MoveCaretToLineStart();
		}

		public string Cut()
		{
			var res = GetSelectedText();
			if (!HasSelection()) {
				var caretPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
				CaretEditor.MoveCaretToLineStart();
				var startPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
				MoveCaretToLineEnd();
				if (Text.GetLineCount() > CaretEditor.Caret.Line + 1)
					MoveSelectionRight(1);
				var endPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
				if (startPos.Line != endPos.Line || startPos.Col != endPos.Col)
					undoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, caretPos));
			} else
				RemoveSelection();
			return res;
		}

		public void MoveSelectionToLineStart()
		{
			if (!HasSelection())
				SelectionStart.AssignFrom(CaretEditor.Caret);
			CaretEditor.MoveCaretToLineStart();
			SelectionEnd.AssignFrom(CaretEditor.Caret);
		}

		public void MoveSelectionToLineEnd()
		{
			if (!HasSelection())
				SelectionStart.AssignFrom(CaretEditor.Caret);
			CaretEditor.MoveCaretToLineEnd();
			SelectionEnd.AssignFrom(CaretEditor.Caret);
		}

		public void RemoveNextWord()
		{
			if (HasSelection()) {
				RemoveSelection();
				return;
			}
			var startPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			CaretEditor.MoveCaretToNextWord();
			var endPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			if (startPos.Line != endPos.Line || startPos.Col != endPos.Col)
				undoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, startPos));
		}

		public void RemovePrevWord()
		{
			if (HasSelection()) {
				RemoveSelection();
				return;
			}
			var endPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			CaretEditor.MoveCaretToPrevWord();
			var startPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			if (startPos.Line != endPos.Line || startPos.Col != endPos.Col)
				undoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, endPos));
		}

		public void Undo() => undoStack.Undo();

		public void Redo() => undoStack.Redo();

		private (ICaret firstCaret, ICaret secondCaret) GetSortedCarets()
		{
			var firstCaret = SelectionStart;
			var secondCaret = SelectionEnd;
			if (firstCaret.CompareTo(secondCaret) > 0)
				(firstCaret, secondCaret) = (secondCaret, firstCaret);
			return (firstCaret, secondCaret);
		}

		private IUndoItem GetInsertUndoItem(string text)
		{
			var (line, col) = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			return new UndoItem(
				() => {
					ClearSelection();
					CaretEditor.Caret.Line = line;
					CaretEditor.Caret.Col = col;
					CaretEditor.Insert(text);
					OnTextInsert?.Invoke(line, col, text);
				},
				() => {
					ClearSelection();
					CaretEditor.Caret.Line = line;
					CaretEditor.Caret.Col = col;
					Text.Remove(line, col, text.Length);
					OnTextRemove?.Invoke(line, col, text.Length);
				});
		}

		private IUndoItem GetRemoveSelectUndoItem()
		{
			var selectPositions = (SelectionStart.Line, SelectionStart.Col, SelectionEnd.Line, SelectionEnd.Col);
			var selectedText = GetSelectedText();
			var (firstCaret, secondCaret) = GetSortedCarets();
			return new UndoItem(
				() => {
					SelectionStart.Line = selectPositions.Item1;
					SelectionStart.Col = selectPositions.Item2;
					SelectionEnd.Line = selectPositions.Item3;
					SelectionEnd.Col = selectPositions.Item4;
					Text.RemoveRange((firstCaret.Line, firstCaret.Col), (secondCaret.Line, secondCaret.Col));
					CaretEditor.Caret.AssignFrom(firstCaret);
					ClearSelection();
				},
				() => {
					ClearSelection();
					SelectionStart.Line = selectPositions.Item1;
					SelectionStart.Col = selectPositions.Item2;
					SelectionEnd.Line = selectPositions.Item3;
					SelectionEnd.Col = selectPositions.Item4;
					CaretEditor.Caret.AssignFrom(firstCaret);
					CaretEditor.Insert(selectedText);
					CaretEditor.Caret.AssignFrom(SelectionEnd);
				});
		}

		private IUndoItem GetRemoveRangeUndoItem((int Line, int Col) first, (int Line, int Col) second, (int Line, int Col) undoCaretPos)
		{
			var text = Text.GetTextRange(first, second);
			return new UndoItem(
				() => {
					ClearSelection();
					CaretEditor.Caret.Line = first.Line;
					CaretEditor.Caret.Col = first.Col;
					Text.RemoveRange(first, second);
				},
				() => {
					ClearSelection();
					CaretEditor.Caret.Line = undoCaretPos.Line;
					CaretEditor.Caret.Col = undoCaretPos.Col;
					Text.Insert(first.Line, first.Col, text);
				});
		}
	}
}
