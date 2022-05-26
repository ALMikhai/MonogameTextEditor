using System;
using MonogameTextEditor.TextEditor.Caret;
using MonogameTextEditor.TextEditor.TextCollection;
using MonogameTextEditor.TextEditor.UndoSystem;

namespace MonogameTextEditor.TextEditor
{
	public class ReadOnlyEditor : ITextEditor
	{
		public ITextCollection Text { get; }

		public ICaret Caret { get; } = new MonogameTextEditor.TextEditor.Caret.Caret();

		public ICaret SelectionStart { get; } = new MonogameTextEditor.TextEditor.Caret.Caret();

		public ICaret SelectionEnd { get; } = new MonogameTextEditor.TextEditor.Caret.Caret();

		public UndoStack UndoStack { get; } = new UndoStack();

		public ReadOnlyEditor(ITextCollection text)
		{
			Text = text;
		}

		public bool MoveCaretRight(int n)
		{
			if (!HasSelection()) {
				return MoveCaretRight(Caret, Text, n);
			}
			var (firstCaret, secondCaret) = GetSortedCarets();
			Caret.AssignFrom(n < 0 ? firstCaret : secondCaret);
			ClearSelection();
			return true;
		}

		public bool MoveCaretDown(int n)
		{
			if (HasSelection()) {
				var (firstCaret, secondCaret) = GetSortedCarets();
				Caret.AssignFrom(n < 0 ? firstCaret : secondCaret);
				ClearSelection();
			}
			return MoveCaretDown(Caret, Text, n);
		}

		public string GetCurrentLine()
		{
			return Text[Caret.Line] + (Caret.Line + 1 == Text.Count ? string.Empty : "\n");
		}

		public char GetCurrentChar()
		{
			return Text.GetSymbol(Caret.Position);
		}

		public void MoveCaretToNextWord()
		{
			if (HasSelection()) {
				MoveCaretRight(1);
			}
			MoveCaretToNextWord(Caret, Text);
		}

		public void MoveCaretToPrevWord()
		{
			if (HasSelection()) {
				MoveCaretRight(-1);
			}
			MoveCaretToPrevWord(Caret, Text);
		}

		public void MoveCaretToLineEnd()
		{
			if (HasSelection()) {
				var (_, secondCaret) = GetSortedCarets();
				Caret.AssignFrom(secondCaret);
				ClearSelection();
			}
			MoveCaretToLineEnd(Caret, Text);
		}

		public void MoveCaretToLineStart()
		{
			if (HasSelection()) {
				var (firstCaret, _) = GetSortedCarets();
				Caret.AssignFrom(firstCaret);
				ClearSelection();
			}
			MoveCaretToLineStart(Caret, Text);
		}

		public void MoveCaretToTextStart()
		{
			if (HasSelection()) {
				ClearSelection();
			}
			Caret.SetPosition(0, 0);
		}

		public void MoveCaretToTextEnd()
		{
			if (HasSelection()) {
				ClearSelection();
			}
			int lastLineIndex = Text.Count - 1;
			Caret.SetPosition(lastLineIndex, Text[lastLineIndex].Length);
		}

		public void Reset()
		{
			Text.Reset();
			Caret.SetPosition(0, 0);
			ClearSelection();
			UndoStack.Reset();
		}

		public void MoveSelectionRight(int n)
		{
			if (!HasSelection()) {
				ClearSelection();
			}
			if (MoveCaretRight(Caret, Text, n)) {
				SelectionEnd.AssignFrom(Caret);
			}
		}

		public void MoveSelectionDown(int n)
		{
			if (!HasSelection()) {
				ClearSelection();
			}
			if (MoveCaretDown(Caret, Text, n)) {
				SelectionEnd.AssignFrom(Caret);
			}
		}

		public void ClearSelection()
		{
			SelectionStart.AssignFrom(Caret);
			SelectionEnd.AssignFrom(Caret);
		}

		public bool HasSelection()
		{
			return !SelectionStart.Equals(SelectionEnd);
		}

		public string GetSelectedText()
		{
			if (!HasSelection()) {
				return GetCurrentLine();
			}
			var (firstCaret, secondCaret) = GetSortedCarets();
			return Text.GetTextRange(firstCaret.Position, secondCaret.Position);
		}

		public void MoveSelectionToNextWord()
		{
			if (!HasSelection()) {
				ClearSelection();
			}
			MoveCaretToNextWord(Caret, Text);
			SelectionEnd.AssignFrom(Caret);
		}

		public void MoveSelectionToPrevWord()
		{
			if (!HasSelection()) {
				ClearSelection();
			}
			MoveCaretToPrevWord(Caret, Text);
			SelectionEnd.AssignFrom(Caret);
		}

		public void SelectWordAtCaretPos()
		{
			if (HasSelection()) {
				ClearSelection();
			}
			(int left, int right) = Text.WordRangeAt(Caret.Position);
			SelectionStart.SetPosition(Caret.Line, left);
			SelectionEnd.SetPosition(Caret.Line, right);
			Caret.AssignFrom(SelectionEnd);
		}

		public void SelectAll()
		{
			SelectionStart.SetPosition(0, 0);
			SelectionEnd.SetPosition(Text.Count - 1, Text[^1].Length);
			Caret.AssignFrom(SelectionEnd);
		}

		public string Cut()
		{
			return GetSelectedText();
		}

		public void MoveSelectionToLineStart()
		{
			if (!HasSelection()) {
				SelectionStart.AssignFrom(Caret);
			}
			MoveCaretToLineStart(Caret, Text);
			SelectionEnd.AssignFrom(Caret);
		}

		public void MoveSelectionToLineEnd()
		{
			if (!HasSelection()) {
				SelectionStart.AssignFrom(Caret);
			}
			MoveCaretToLineEnd(Caret, Text);
			SelectionEnd.AssignFrom(Caret);
		}

		public (ICaret firstCaret, ICaret secondCaret) GetSortedCarets()
		{
			var firstCaret = SelectionStart;
			var secondCaret = SelectionEnd;
			if (firstCaret.CompareTo(secondCaret) > 0) {
				(firstCaret, secondCaret) = (secondCaret, firstCaret);
			}
			return (firstCaret, secondCaret);
		}

		public void RemoveNextWord()
		{
		}

		public void RemovePrevWord()
		{
		}

		public void Undo()
		{
		}

		public void Redo()
		{
		}

		public void HardUndo()
		{
		}

		public void RemoveSelection()
		{
		}

		public void RemoveForward()
		{
		}

		public void RemoveBackward()
		{
		}

		public void Insert(string s)
		{
		}

		public static bool MoveCaretRight(ICaret caret, ITextCollection text, int n)
		{
			var pos = caret.Position;
			bool forward = n > 0;
			int lineCount = text.Count;
			while (n != 0) {
				if (forward) {
					int offset = text[pos.Line].Length - pos.Col;
					if (offset < n) {
						if (lineCount > pos.Line + 1) {
							pos.Line += 1;
							pos.Col = 0;
							n -= offset + 1;
						} else {
							return false;
						}
					} else {
						pos.Col += n;
						n -= n;
					}
				} else {
					int offset = -pos.Col;
					if (offset > n) {
						if (0 <= pos.Line - 1) {
							pos.Line -= 1;
							pos.Col = text[pos.Line].Length;
							n -= offset - 1;
						} else {
							return false;
						}
					} else {
						pos.Col += n;
						n -= n;
					}
				}
			}
			caret.SetPosition(pos);
			return true;
		}

		public static bool MoveCaretDown(ICaret caret, ITextCollection text, int n)
		{
			if (caret.Line + n >= 0 && caret.Line + n < text.Count) {
				caret.SetPosition(caret.Line + n, Math.Min(caret.Col, text[caret.Line + n].Length));
				return true;
			}
			return false;
		}

		public static void MoveCaretToNextWord(ICaret caret, ITextCollection text)
		{
			var pos = text.NextWordAt(caret.Position);
			caret.SetPosition(pos);
		}

		public static void MoveCaretToPrevWord(ICaret caret, ITextCollection text)
		{
			var pos = text.PrevWordAt(caret.Position);
			caret.SetPosition(pos);
		}

		public static void MoveCaretToLineStart(ICaret caret, ITextCollection text)
		{
			caret.SetPosition(caret.Line, 0);
		}

		public static void MoveCaretToLineEnd(ICaret caret, ITextCollection text)
		{
			caret.SetPosition(caret.Line, text[caret.Line].Length);
		}
	}
}
