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

        private string cachedString = "";
        private bool textUpdated = false;

        public void Insert(int line, int col, string text) {
            Text[line] = Text[line].Insert(col, text);
            textUpdated = true;
        }

        public void Remove(int line, int col, int lenght) {
            Text[line] = Text[line].Remove(col, lenght);
            textUpdated = true;
        }

        public void RemoveLine(int line) {
            Text.RemoveAt(line);
            textUpdated = true;
        }

        public void InsertLine(int line, string text) {
            Text.Insert(line, text);
            textUpdated = true;
        }

        public override string ToString() {
            if (textUpdated) {
                var builder = new StringBuilder();
                for (var i = 0; i < Text.Count; i++) {
                    var line = Text[i];
                    var isLastLine = i == Text.Count - 1;
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
