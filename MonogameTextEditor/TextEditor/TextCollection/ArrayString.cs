using System;
using System.Collections.Generic;
using System.Text;

namespace MonogameTextEditor.TextEditor.TextCollection
{
	// TODO Maybe need to make own exception class to avoid misunderstandings.
	public class ArrayString : ITextCollection
	{
		private readonly List<string> text = new List<string> { "" };
		private string cachedString = "";
		private bool textUpdated;

		public (int NewLine, int NewCol) Insert(int line, int col, string text)
		{
			if (!IsValidPosition((line, col)))
				throw new ArgumentException("Insert position is not valid");
			var lines = text.Replace("\r\n", "\n").Split('\n');
			var firstPart = this.text[line].Substring(0, col);
			var secondPart = this.text[line].Substring(col, this.text[line].Length - col);
			Remove(line, col, this.text[line].Length - col);
			this.text[line] = this.text[line].Insert(firstPart.Length, lines[0]);
			for (var i = 1; i < lines.Length; i++) {
				var str = lines[i];
				line++;
				InsertLine(line, str);
			}
			col = this.text[line].Length;
			this.text[line] = this.text[line].Insert(this.text[line].Length, secondPart);
			textUpdated = true;
			return (line, col);
		}

		public void Remove(int line, int col, int lenght)
		{
			if (!IsValidPosition((line, col)))
				throw new ArgumentException("Remove position is not valid");
			var secondLine = line;
			var secondCol = col;
			while (true) {
				var offset = text[secondLine].Length - secondCol;
				if (lenght <= offset) {
					secondCol += lenght;
					break;
				}
				if (secondLine + 1 >= text.Count)
					throw new ArgumentException("The length of the remove is greater than the length of the text");
				lenght -= offset + 1;
				++secondLine;
				secondCol = 0;
			}
			RemoveRange((line, col), (secondLine, secondCol));
			textUpdated = true;
		}

		public void RemoveLine(int line)
		{
			if (!IsValidPosition((line, 0)))
				throw new ArgumentException("Remove position is not valid");
			text.RemoveAt(line);
			textUpdated = true;
		}

		public void RemoveRange((int Line, int Col) left, (int Line, int Col) right)
		{
			if (!IsValidPosition(left) || !IsValidPosition(right))
				throw new IndexOutOfRangeException("Remove range go beyond text size");
			if (left.Line == right.Line)
				text[left.Line] = text[left.Line].Remove(left.Col, right.Col - left.Col);
			else {
				text[left.Line] = text[left.Line].Remove(left.Col, text[left.Line].Length - left.Col);
				for (var i = left.Line + 1; i < right.Line; i++)
					RemoveLine(left.Line + 1);
				text[left.Line + 1] = text[left.Line + 1].Remove(0, right.Col);
				Insert(left.Line, left.Col, text[left.Line + 1]);
				RemoveLine(left.Line + 1);
			}
			textUpdated = true;
		}

		public string GetTextRange((int Line, int Col) left, (int Line, int Col) right)
		{
			if (!IsValidPosition(left) || !IsValidPosition(right))
				throw new IndexOutOfRangeException("Range go beyond text size");
			var res = new StringBuilder();
			if (left.Line == right.Line)
				res.Append(text[left.Line][left.Col..right.Col]);
			else {
				res.Append(text[left.Line][left.Col..text[left.Line].Length]);
				res.Append('\n');
				for (var i = left.Line + 1; i < right.Line; i++) {
					res.Append(text[i]);
					res.Append('\n');
				}
				res.Append(text[right.Line][..right.Col]);
			}
			return res.ToString();
		}

		public string GetLine(int line)
		{
			if (!IsValidPosition((line, 0)))
				throw new ArgumentException("Insert position is not valid");
			return text[line];
		}

		public int GetLineLenght(int line)
		{
			if (!IsValidPosition((line, 0)))
				throw new ArgumentException("Position is not valid");
			return text[line].Length;
		}

		public int GetLineCount() => text.Count;

		public char GetSymbol(int line, int col)
		{
			if (!IsValidPosition((line, col)))
				throw new ArgumentException("Position is not valid");
			return text[line].Length == col ? '\n' : text[line][col];
		}

		public void InsertLine(int line, string text)
		{
			if (!IsValidPosition((line - 1, 0)))
				throw new ArgumentException("Insert position is not valid");
			if (text.Contains('\n'))
				throw new ArgumentException("Trying to insert multiple lines instead of one");
			this.text.Insert(line, text);
			textUpdated = true;
		}

		public override string ToString()
		{
			if (textUpdated) {
				var builder = new StringBuilder();
				for (var i = 0; i < text.Count; i++) {
					var line = text[i];
					var isLastLine = i == text.Count - 1;
					builder.Append(line);
					if (!isLastLine)
						builder.Append('\n');
				}
				cachedString = builder.ToString();
				textUpdated = false;
			}
			return cachedString;
		}

		private bool IsValidPosition((int Line, int Col) pos) => pos.Line >= 0 && pos.Col >= 0 &&
			pos.Line < text.Count && pos.Col <= text[pos.Line].Length;
	}
}
