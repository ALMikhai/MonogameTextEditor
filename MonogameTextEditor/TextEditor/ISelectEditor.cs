using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	}

	public class SelectEditor : ISelectEditor
	{
		public ICaretEditor CaretEditor { get; }
		public ICaretPosition StartPosition { get; } = new DrawableCaret();
		public ICaretPosition EndPosition { get; } = new DrawableCaret();
		public ITextCollection Text => CaretEditor.Text;

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
			if (HasSelection())
				RemoveSelect();
			CaretEditor.Insert(text);
		}

		public void RemoveSelect()
		{
			if (!HasSelection())
				return;
			var (firstCaret, secondCaret) = GetSortedCarets();
			Text.RemoveRange((firstCaret.Line, firstCaret.Col), (secondCaret.Line, secondCaret.Col));
			CaretEditor.Caret.AssignFrom(firstCaret);
			ClearSelection();
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
			if (HasSelection())
				RemoveSelect();
			else
				CaretEditor.RemoveForward();
		}

		public void RemoveBackward()
		{
			if (HasSelection())
				RemoveSelect();
			else
				CaretEditor.RemoveBackward();
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
			if (HasSelection())
				RemoveSelect();
			else {
				CaretEditor.MoveCaretToStartOfLine();
				var pos = CaretEditor.Caret;
				if (Text.GetLineCount() == pos.Line + 1)
					Text.Remove(pos.Line, 0, Text[pos.Line].Length);
				else
					Text.RemoveLine(pos.Line);
			}
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
			if (!HasSelection())
				MoveSelectToNextWord();
			RemoveSelect();
		}

		public void RemoveWordPrev()
		{
			if (!HasSelection())
				MoveSelectToPrevWord();
			RemoveSelect();
		}

		private (ICaretPosition firstCaret, ICaretPosition secondCaret) GetSortedCarets()
		{
			var firstCaret = StartPosition;
			var secondCaret = EndPosition;
			if (firstCaret > secondCaret)
				(firstCaret, secondCaret) = (secondCaret, firstCaret);
			return (firstCaret, secondCaret);
		}
	}
}
