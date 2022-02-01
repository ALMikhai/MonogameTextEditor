using System.Collections.Generic;
using System.Text;

namespace MonogameTextEditor.TextEditor
{
	public interface ITextCollection
	{
		void Insert(int line, int col, string text);
		void Remove(int line, int col, int lenght);
		void InsertLine(int line, string text);
		void RemoveLine(int line);
		string GetLine(int line);
		int GetLineLenght(int line);
		int GetLineCount();
		string this[int line] => GetLine(line);
	}

	public class ArrayStringText : ITextCollection
	{
		private readonly List<string> text = new List<string> { "" };
		private string cachedString = "";
		private bool textUpdated;

		public void Insert(int line, int col, string text)
		{
			this.text[line] = this.text[line].Insert(col, text);
			textUpdated = true;
		}

		public void Remove(int line, int col, int lenght)
		{
			text[line] = text[line].Remove(col, lenght);
			textUpdated = true;
		}

		public void RemoveLine(int line)
		{
			text.RemoveAt(line);
			textUpdated = true;
		}

		public string GetLine(int line)
		{
			return text[line];
		}

		public int GetLineLenght(int line)
		{
			return text[line].Length;
		}

		public int GetLineCount()
		{
			return text.Count;
		}

		public void InsertLine(int line, string text)
		{
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
					if (isLastLine)
						builder.Append(line);
					else
						builder.AppendLine(line);
				}
				cachedString = builder.ToString();
				textUpdated = false;
			}
			return cachedString;
		}
	}
}
