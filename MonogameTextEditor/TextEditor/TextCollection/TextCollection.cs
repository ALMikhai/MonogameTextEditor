using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using MonogameTextEditor.TextEditor.Caret;

namespace MonogameTextEditor.TextEditor.TextCollection
{
	public class TextCollection : ITextCollection
	{
		public event Action TextChanged;

		private readonly List<string> text = new List<string> {
			string.Empty,
		};
		private string cachedString = string.Empty;
		private bool cachedStringDirty;

		public TextCollection()
		{
			Reset();
			TextChanged += OnTextChanged;
		}

		public TextPosition Insert(TextPosition pos, string text)
		{
			if (!IsValidPosition(pos)) {
				throw new ArgumentException("Insert position is not valid");
			}
			string firstPart = this.text[pos.Line].Substring(0, pos.Col);
			string secondPart = this.text[pos.Line].Substring(pos.Col, this.text[pos.Line].Length - pos.Col);
			Remove(pos, this.text[pos.Line].Length - pos.Col);
			var lineEnumerator = text.SplitLines();
			lineEnumerator.MoveNext();
			this.text[pos.Line] = this.text[pos.Line].Insert(firstPart.Length, lineEnumerator.Current.ToString());
			foreach (var line in lineEnumerator) {
				pos.Line++;
				InsertLine(pos.Line, line.ToString());
			}
			pos.Col = this.text[pos.Line].Length;
			this.text[pos.Line] = this.text[pos.Line].Insert(this.text[pos.Line].Length, secondPart);
			TextChanged?.Invoke();
			return pos;
		}

		public void Remove(TextPosition begin, int length)
		{
			if (!IsValidPosition(begin)) {
				throw new ArgumentException("Remove position is not valid");
			}
			var end = begin;
			while (true) {
				int offset = text[end.Line].Length - end.Col;
				if (length <= offset) {
					end.Col += length;
					break;
				}
				if (end.Line + 1 >= text.Count) {
					throw new ArgumentException("The length of the remove is greater than the length of the text");
				}
				length -= offset + 1;
				++end.Line;
				end.Col = 0;
			}
			RemoveRange(begin, end);
		}

		public void RemoveRange(TextPosition begin, TextPosition end)
		{
			if (!IsValidPosition(begin) || !IsValidPosition(end)) {
				throw new IndexOutOfRangeException("Remove range go beyond text size");
			}
			if (begin.Line == end.Line) {
				text[begin.Line] = text[begin.Line].Remove(begin.Col, end.Col - begin.Col);
			} else {
				text[begin.Line] = text[begin.Line].Remove(begin.Col, text[begin.Line].Length - begin.Col);
				text.RemoveRange(begin.Line + 1, end.Line - begin.Line - 1);
				text[begin.Line + 1] = text[begin.Line + 1].Remove(0, end.Col);
				Insert(begin, text[begin.Line + 1]);
				RemoveLine(begin.Line + 1);
			}
			TextChanged?.Invoke();
		}

		public string GetTextRange(TextPosition begin, TextPosition end)
		{
			if (!IsValidPosition(begin) || !IsValidPosition(end)) {
				throw new IndexOutOfRangeException("Range go beyond text size");
			}
			string result;
			if (begin.Line == end.Line) {
				result = text[begin.Line][begin.Col..end.Col];
			} else {
				var sb = new StringBuilder();
				sb.Append(text[begin.Line][begin.Col..text[begin.Line].Length]);
				sb.Append('\n');
				for (int i = begin.Line + 1; i < end.Line; i++) {
					sb.Append(text[i]);
					sb.Append('\n');
				}
				sb.Append(text[end.Line][..end.Col]);
				result = sb.ToString();
			}
			return result;
		}

		public char GetSymbol(TextPosition pos)
		{
			if (!IsValidPosition(pos)) {
				throw new ArgumentException("Position is not valid");
			}
			return pos.Col == text[pos.Line].Length ? pos.Line == text.Count - 1 ? '\0' : '\n'
				: text[pos.Line][pos.Col];
		}

		public int GetTextLength()
		{
			if (!cachedStringDirty) {
				return cachedString.Length;
			}
			return text.Sum(s => s.Length) + text.Count - 1;
		}

		public IEnumerator<string> GetEnumerator()
		{
			return text.GetEnumerator();
		}

		public override string ToString()
		{
			if (cachedStringDirty) {
				var sb = new StringBuilder();
				for (int i = 0; i < text.Count; i++) {
					string line = text[i];
					bool isLastLine = i == text.Count - 1;
					sb.Append(line);
					if (!isLastLine) {
						sb.Append('\n');
					}
				}
				cachedString = sb.ToString();
				cachedStringDirty = false;
			}
			return cachedString;
		}

		public void Reset()
		{
			text.Clear();
			text.Add(string.Empty);
			cachedString = string.Empty;
			cachedStringDirty = false;
			TextChanged?.Invoke();
		}

		public void Reset(string text)
		{
			this.text.Clear();
			this.text.Add(string.Empty);
			Insert(new TextPosition(), text);
			cachedString = text;
			cachedStringDirty = false;
			TextChanged?.Invoke();
		}

		public TextPosition NextWordAt(TextPosition pos)
		{
			if (!IsValidPosition(pos)) {
				throw new ArgumentException("Position is not valid");
			}
			string line = text[pos.Line];
			if (line.Length == pos.Col) {
				return text.Count == pos.Line + 1 ? new TextPosition(pos.Line, pos.Col)
					: new TextPosition(pos.Line + 1);
			}
			pos.Col = WordUtils.NextWord(line, pos.Col);
			return pos;
		}

		public TextPosition PrevWordAt(TextPosition pos)
		{
			if (!IsValidPosition(pos)) {
				throw new ArgumentException("Position is not valid");
			}
			string line = text[pos.Line];
			if (0 == pos.Col) {
				return 0 == pos.Line ? new TextPosition(pos.Line, pos.Col)
					: new TextPosition(pos.Line - 1, text[pos.Line - 1].Length);
			}
			pos.Col = WordUtils.PreviousWord(line, pos.Col);
			return pos;
		}

		public (int Left, int Right) WordRangeAt(TextPosition pos)
		{
			if (!IsValidPosition(pos)) {
				throw new ArgumentException("Position is not valid");
			}
			string line = text[pos.Line];
			var res = WordUtils.WordAt(line, pos.Col);
			return res;
		}

		public TextPosition? IndexOf([NotNull] string value, bool toLower = false)
		{
			if (string.IsNullOrEmpty(value)) {
				return null;
			}
			for (int i = 0; i < text.Count; i++) {
				string s = toLower ? text[i].ToLower() : text[i];
				int res = s.IndexOf(value, StringComparison.Ordinal);
				if (res != -1) {
					return new TextPosition(i, res);
				}
			}
			return null;
		}

		public int Count => text.Count;

		public string this[int index]
		{
			get
			{
				if (!IsValidPosition(new TextPosition(index))) {
					throw new ArgumentException("Position is not valid");
				}
				return text[index];
			}
		}

		private void InsertLine(int line, string text)
		{
			if (!IsValidPosition(new TextPosition(line - 1))) {
				throw new ArgumentException("Insert position is not valid");
			}
			if (text.Contains('\n')) {
				throw new ArgumentException("Trying to insert multiple lines instead of one");
			}
			this.text.Insert(line, text);
		}

		private void RemoveLine(int line)
		{
			if (!IsValidPosition(new TextPosition(line))) {
				throw new ArgumentException("Remove position is not valid");
			}
			text.RemoveAt(line);
		}

		private void OnTextChanged()
		{
			cachedStringDirty = true;
		}

		private bool IsValidPosition(TextPosition pos)
		{
			return pos.Line >= 0 && pos.Col >= 0 &&
				pos.Line < text.Count && pos.Col <= text[pos.Line].Length;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static class WordUtils
		{
			private enum CharClass
			{
				Begin,
				Space,
				Punctuation,
				Word,
				Other,
				End,
			}

			private static CharClass GetCharClassAt(string text, int pos)
			{
				if (pos < 0) {
					return CharClass.Begin;
				}
				if (pos >= text.Length) {
					return CharClass.End;
				}
				char symbol = text[pos];
				if (char.IsWhiteSpace(symbol)) {
					return CharClass.Space;
				}
				if (char.IsPunctuation(symbol) || char.IsSeparator(symbol) || char.IsSymbol(symbol)) {
					return CharClass.Punctuation;
				}
				if (char.IsLetterOrDigit(symbol)) {
					return CharClass.Word;
				}
				return CharClass.Other;
			}

			public static int PreviousWord(string text, int pos)
			{
				if (pos <= 0) {
					return 0;
				}
				--pos;
				for (var ccRight = GetCharClassAt(text, pos); pos > 0; --pos) {
					var ccLeft = GetCharClassAt(text, pos - 1);
					if (ccLeft != ccRight && ccRight != CharClass.Space) {
						break;
					}
					ccRight = ccLeft;
				}
				return pos;
			}

			public static int NextWord(string text, int pos)
			{
				if (pos >= text.Length) {
					return text.Length;
				}
				++pos;
				for (var ccLeft = GetCharClassAt(text, pos - 1); pos < text.Length; ++pos) {
					var ccRight = GetCharClassAt(text, pos);
					if (ccRight != ccLeft && ccRight != CharClass.Space) {
						break;
					}
					ccLeft = ccRight;
				}
				return pos;
			}

			public static (int Left, int Right) WordAt(string text, int pos)
			{
				(int Left, int Right) result = (pos, pos);
				var currentClass = GetCharClassAt(text, pos);
				var prevClass = GetCharClassAt(text, pos - 1);
				switch (currentClass) {
					case CharClass.Space:
						currentClass = prevClass != CharClass.Begin ? prevClass : currentClass;
						break;
					case CharClass.End when prevClass == CharClass.Begin:
						return result;
					case CharClass.End:
						currentClass = prevClass;
						break;
				}
				while (GetCharClassAt(text, result.Left - 1) == currentClass) {
					--result.Left;
				}
				while (GetCharClassAt(text, result.Right) == currentClass) {
					++result.Right;
				}
				return result;
			}
		}
	}
}
