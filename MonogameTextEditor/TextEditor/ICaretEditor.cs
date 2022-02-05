using System;
using System.Collections.Generic;

namespace MonogameTextEditor.TextEditor
{
	public interface ICaretEditor
	{
		ICaretPosition Caret { get; }
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
		void MoveCaretToEndOfLine();
		void MoveCaretToStartOfLine();
	}

	public class CaretEditor : ICaretEditor
	{
		public ICaretPosition Caret { get; } = new DrawableCaret();
		public ITextCollection Text { get; } = new ArrayStringText();

		public void RemoveForward()
		{
			try {
				Text.Remove(Caret.Line, Caret.Col, 1);
			} catch (ArgumentException e) {
				if (e.Message != "The length of the remove is greater than the length of the text")
					throw;
			}
		}

		public void RemoveBackward()
		{
			if (MoveCaretRight(-1))
				RemoveForward();
		}

		public void Insert(string s)
		{
			var (newLine, newCol) = Text.Insert(Caret.Line, Caret.Col, s);
			Caret.Col = newCol;
			Caret.Line = newLine;
		}

		// TODO Rewrite it.
		public bool MoveCaretRight(int n)
		{
			var pos = (Caret.Line, Caret.Col);
			var forward = n > 0;
			var lineCount = Text.GetLineCount();
			while (n != 0) {
				if (forward) {
					var offset = Text.GetLineLenght(pos.Line) - pos.Col;
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
					var offset = -pos.Col;
					if (offset > n) {
						if (0 <= pos.Line - 1) {
							pos.Line -= 1;
							pos.Col = Text.GetLineLenght(pos.Line);
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
			Caret.Line = pos.Line;
			Caret.Col = pos.Col;
			return true;
		}

		public bool MoveCaretDown(int n)
		{
			if (Caret.Line + n >= 0 && Caret.Line + n < Text.GetLineCount()) {
				Caret.Line += n;
				// TODO replace to Mathf from Citrus.
				Caret.Col = Math.Min(Caret.Col, Text[Caret.Line].Length);
				return true;
			}
			return false;
		}

		public void MoveCaretToNextWord()
		{
			var pos = GetNextWordPos();
			Caret.Col = pos.Col;
			Caret.Line = pos.Line;
		}

		public void MoveCaretToPrevWord()
		{
			var pos = GetPrevWordPos();
			Caret.Col = pos.Col;
			Caret.Line = pos.Line;
		}

		public void MoveCaretToEndOfLine()
		{
			Caret.Col = Text[Caret.Line].Length;
		}

		public void MoveCaretToStartOfLine()
		{
			Caret.Col = 0;
		}

		public (int Line, int Col) GetNextWordPos()
		{
			var pos = (Caret.Line, Caret.Col);
			var line = Text[pos.Line];
			if (line.Length == pos.Col)
				return Text.GetLineCount() == pos.Line + 1 ? (pos.Line, pos.Col) : (pos.Line + 1, 0);
			while (line.Length > pos.Col && !char.IsWhiteSpace(line[pos.Col]))
				pos.Col++;
			while (line.Length > pos.Col && char.IsWhiteSpace(line[pos.Col]))
				pos.Col++;
			return pos;
		}

		public (int Line, int Col) GetPrevWordPos()
		{
			var pos = (Caret.Line, Caret.Col);
			var line = Text[pos.Line];
			if (0 == pos.Col)
				return 0 == pos.Line ? (pos.Line, pos.Col) : (pos.Line - 1, Text[pos.Line - 1].Length);
			while (0 < pos.Col && char.IsWhiteSpace(line[pos.Col - 1]))
				pos.Col--;
			while (0 < pos.Col && !char.IsWhiteSpace(line[pos.Col - 1]))
				pos.Col--;
			return pos;
		}

		public string GetCurrentLine()
		{
			return Text[Caret.Line] + (Caret.Line + 1 == Text.GetLineCount() ? "" : "\n");
		}
	}
}
