namespace MonogameTextEditor.TextEditor
{
	public interface ISelectEditor
	{
		ICaretEditor CaretEditor { get; }
		ICaretPosition StartPosition { get; }
		ICaretPosition EndPosition { get; }
		ITextCollection Text { get; }
		bool MoveSelectRight(int n);
		bool MoveSelectDown(int n);
		void Insert(string text);
		void RemoveSelect();
		void ClearSelection();
		bool HasSelection();
		void RemoveForward();
		void RemoveBackward();
		bool MoveCaretRight(int n);
		bool MoveCaretDown(int n);
		string GetSelectedText();
		void MoveSelectToNextWord();
		void MoveSelectToPrevWord();
		void MoveCaretToNextWord();
		void MoveCaretToPrevWord();
		void SelectAll();
		void MoveCaretToEndOfLine();
		void MoveCaretToStartOfLine();
		string CutSelectedText();
		void MoveSelectToStartLine();
		void MoveSelectToEndLine();
		void RemoveWordNext();
		void RemoveWordPrev();
		void Undo();
		void Redo();
	}

	public class SelectEditor : ISelectEditor
	{
		public ICaretEditor CaretEditor { get; }
		public ICaretPosition StartPosition { get; } = new DrawableCaret();
		public ICaretPosition EndPosition { get; } = new DrawableCaret();
		public ITextCollection Text => CaretEditor.Text;

		private readonly UndoStack undoStack = new UndoStack();

		public SelectEditor(ICaretEditor caretEditor)
		{
			CaretEditor = caretEditor;
		}

		public bool MoveSelectRight(int n)
		{
			if (!HasSelection())
				ClearSelection();
			if (CaretEditor.MoveCaretRight(n)) {
				EndPosition.AssignFrom(CaretEditor.Caret);
				return true;
			}
			return false;
		}

		public bool MoveSelectDown(int n)
		{
			if (!HasSelection())
				ClearSelection();
			if (CaretEditor.MoveCaretDown(n)) {
				EndPosition.AssignFrom(CaretEditor.Caret);
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

		public void RemoveSelect()
		{
			if (!HasSelection())
				return;
			undoStack.Do(GetRemoveSelectUndoItem());
		}

		public void ClearSelection()
		{
			StartPosition.AssignFrom(CaretEditor.Caret);
			EndPosition.AssignFrom(CaretEditor.Caret);
		}

		public bool HasSelection()
		{
			return !StartPosition.Equals(EndPosition);
		}

		public void RemoveForward()
		{
			if (HasSelection()) {
				RemoveSelect();
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
				RemoveSelect();
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

		public string GetSelectedText()
		{
			if (!HasSelection())
				return CaretEditor.GetCurrentLine();
			var (firstCaret, secondCaret) = GetSortedCarets();
			return Text.GetTextRange((firstCaret.Line, firstCaret.Col), (secondCaret.Line, secondCaret.Col));
		}

		public void MoveSelectToNextWord()
		{
			if (!HasSelection())
				ClearSelection();
			CaretEditor.MoveCaretToNextWord();
			EndPosition.AssignFrom(CaretEditor.Caret);
		}

		public void MoveSelectToPrevWord()
		{
			if (!HasSelection())
				ClearSelection();
			CaretEditor.MoveCaretToPrevWord();
			EndPosition.AssignFrom(CaretEditor.Caret);
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
			StartPosition.Line = 0;
			StartPosition.Col = 0;
			EndPosition.Line = Text.GetLineCount() - 1;
			EndPosition.Col = Text[EndPosition.Line].Length;
			CaretEditor.Caret.AssignFrom(EndPosition);
		}

		public void MoveCaretToEndOfLine()
		{
			if (HasSelection()) {
				var (_, secondCaret) = GetSortedCarets();
				CaretEditor.Caret.AssignFrom(secondCaret);
				ClearSelection();
			}
			CaretEditor.MoveCaretToEndOfLine();
		}

		public void MoveCaretToStartOfLine()
		{
			if (HasSelection()) {
				var (firstCaret, _) = GetSortedCarets();
				CaretEditor.Caret.AssignFrom(firstCaret);
				ClearSelection();
			}
			CaretEditor.MoveCaretToStartOfLine();
		}

		public string CutSelectedText()
		{
			var res = GetSelectedText();
			if (!HasSelection()) {
				var caretPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
				CaretEditor.MoveCaretToStartOfLine();
				var startPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
				MoveCaretToEndOfLine();
				if (Text.GetLineCount() > CaretEditor.Caret.Line + 1)
					MoveSelectRight(1);
				var endPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
				if (startPos.Line != endPos.Line || startPos.Col != endPos.Col)
					undoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, caretPos));
			} else
				RemoveSelect();
			return res;
		}

		public void MoveSelectToStartLine()
		{
			if (!HasSelection())
				StartPosition.AssignFrom(CaretEditor.Caret);
			CaretEditor.MoveCaretToStartOfLine();
			EndPosition.AssignFrom(CaretEditor.Caret);
		}

		public void MoveSelectToEndLine()
		{
			if (!HasSelection())
				StartPosition.AssignFrom(CaretEditor.Caret);
			CaretEditor.MoveCaretToEndOfLine();
			EndPosition.AssignFrom(CaretEditor.Caret);
		}

		public void RemoveWordNext()
		{
			if (HasSelection()) {
				RemoveSelect();
				return;
			}
			var startPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			CaretEditor.MoveCaretToNextWord();
			var endPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			if (startPos.Line != endPos.Line || startPos.Col != endPos.Col)
				undoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, startPos));
		}

		public void RemoveWordPrev()
		{
			if (HasSelection()) {
				RemoveSelect();
				return;
			}
			var endPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			CaretEditor.MoveCaretToPrevWord();
			var startPos = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			if (startPos.Line != endPos.Line || startPos.Col != endPos.Col)
				undoStack.Do(GetRemoveRangeUndoItem(startPos, endPos, endPos));
		}

		public void Undo()
		{
			undoStack.Undo();
		}

		public void Redo()
		{
			undoStack.Redo();
		}

		private (ICaretPosition firstCaret, ICaretPosition secondCaret) GetSortedCarets()
		{
			var firstCaret = StartPosition;
			var secondCaret = EndPosition;
			if (firstCaret > secondCaret)
				(firstCaret, secondCaret) = (secondCaret, firstCaret);
			return (firstCaret, secondCaret);
		}

		private IUndoItem GetInsertUndoItem(string text)
		{
			var (line, col) = (CaretEditor.Caret.Line, CaretEditor.Caret.Col);
			return new SingleUndoItem(
				() => {
					ClearSelection();
					CaretEditor.Caret.Line = line;
					CaretEditor.Caret.Col = col;
					CaretEditor.Insert(text);
				},
				() => {
					ClearSelection();
					CaretEditor.Caret.Line = line;
					CaretEditor.Caret.Col = col;
					Text.Remove(line, col, text.Length);
				});
		}

		private IUndoItem GetRemoveSelectUndoItem()
		{
			var selectPositions = (StartPosition.Line, StartPosition.Col, EndPosition.Line, EndPosition.Col);
			var selectedText = GetSelectedText();
			var (firstCaret, secondCaret) = GetSortedCarets();
			return new SingleUndoItem(
				() => {
					StartPosition.Line = selectPositions.Item1;
					StartPosition.Col = selectPositions.Item2;
					EndPosition.Line = selectPositions.Item3;
					EndPosition.Col = selectPositions.Item4;
					Text.RemoveRange((firstCaret.Line, firstCaret.Col), (secondCaret.Line, secondCaret.Col));
					CaretEditor.Caret.AssignFrom(firstCaret);
					ClearSelection();
				},
				() => {
					ClearSelection();
					StartPosition.Line = selectPositions.Item1;
					StartPosition.Col = selectPositions.Item2;
					EndPosition.Line = selectPositions.Item3;
					EndPosition.Col = selectPositions.Item4;
					CaretEditor.Caret.AssignFrom(firstCaret);
					CaretEditor.Insert(selectedText);
					CaretEditor.Caret.AssignFrom(EndPosition);
				});
		}

		private IUndoItem GetRemoveRangeUndoItem((int Line, int Col) first, (int Line, int Col) second, (int Line, int Col) undoCaretPos)
		{
			var text = Text.GetTextRange(first, second);
			return new SingleUndoItem(
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
