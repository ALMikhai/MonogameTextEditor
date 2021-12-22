using System.Collections.Generic;
using System.Text;

namespace MonogameTextEditor.TextEditor {
    public interface ITextCollection {
        List<string> Text { get; }
        void Insert(int line, int col, string text);
        void Remove(int line, int col, int lenght);
        void InsertLine(int line, string text);
        void RemoveLine(int line);
    }

    public class ArrayStringText : ITextCollection {
        public List<string> Text { get; } = new List<string> { "" };

        public void Insert(int line, int col, string text) {
            Text[line] = Text[line].Insert(col, text);
        }

        public void Remove(int line, int col, int lenght) {
            Text[line] = Text[line].Remove(col, lenght);
        }

        public void RemoveLine(int line) {
            Text.RemoveAt(line);
        }

        public void InsertLine(int line, string text) {
            Text.Insert(line, text);
        }

        public override string ToString() {
            var builder = new StringBuilder();
            for (var i = 0; i < Text.Count; i++) {
                var line = Text[i];
                var isLastLine = i == Text.Count - 1;
                if (isLastLine)
                    builder.Append(line);
                else
                    builder.AppendLine(line);
            }

            return builder.ToString();
        }
    }
}
