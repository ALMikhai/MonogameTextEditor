using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MonogameTextEditor.TextEditor.Caret;

namespace MonogameTextEditor.TextEditor.TextCollection
{
	public interface ITextCollection : IReadOnlyList<string>
	{
		event Action TextChanged;

		TextPosition Insert(TextPosition pos, string text);

		void Remove(TextPosition begin, int length);

		public void RemoveRange(TextPosition begin, TextPosition end);

		string GetTextRange(TextPosition begin, TextPosition end);

		char GetSymbol(TextPosition pos);

		int GetTextLength();

		void Reset();

		void Reset(string text);

		TextPosition NextWordAt(TextPosition pos);

		TextPosition PrevWordAt(TextPosition pos);

		(int Left, int Right) WordRangeAt(TextPosition pos);

		/// <summary>
		/// Reports the position of the first occurrence of the specified string in this text.
		/// </summary>
		/// <param name="value">The string to seek.</param>
		/// <param name="toLower">Flag for converting text to lowercase.</param>
		/// <returns>The position of value if that string is found, or null if it is not.</returns>
		TextPosition? IndexOf([NotNull] string value, bool toLower = false);
	}
}
