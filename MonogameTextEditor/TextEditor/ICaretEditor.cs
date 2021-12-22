using System;
using System.Collections.Generic;

namespace MonogameTextEditor.TextEditor {
    public interface ICaretEditor {
        ICaretPosition Caret { get; set; }
        ITextCollection TextContainer { get; set; }
        List<string> Text { get; }
        void RemoveForward();
        void RemoveBackward();
        void Insert(string s);
        bool MoveCaretRight(int n);
        bool MoveCaretDown(int n);
        string GetCurrentLine();
    }

    public class CaretEditor : ICaretEditor {
        public ICaretPosition Caret { get; set; } = new DrawableCaret();
        public ITextCollection TextContainer { get; set; } = new ArrayStringText();
        public List<string> Text => TextContainer.Text;

        public void RemoveForward() {
            if (Caret.Line + 1 < Text.Count && Caret.Col == Text[Caret.Line].Length) {
                var line = Text[Caret.Line + 1];
                TextContainer.RemoveLine(Caret.Line + 1);
                TextContainer.Insert(Caret.Line, Text[Caret.Line].Length, line);
            }
            else if (Caret.Col < Text[Caret.Line].Length) TextContainer.Remove(Caret.Line, Caret.Col, 1);
        }

        public void RemoveBackward() {
            if (MoveCaretRight(-1))
                RemoveForward();
        }

        public void Insert(string s) {
            var lines = s.Split('\n');

            var firstPart = Text[Caret.Line].Substring(0, Caret.Col);
            var secondPart = Text[Caret.Line].Substring(Caret.Col, Text[Caret.Line].Length - Caret.Col);

            TextContainer.Remove(Caret.Line, Caret.Col, Text[Caret.Line].Length - Caret.Col);
            TextContainer.Insert(Caret.Line, firstPart.Length, lines[0]);

            for (var i = 1; i < lines.Length; i++) {
                var line = lines[i];
                Caret.Line++;
                TextContainer.InsertLine(Caret.Line, line);
            }

            Caret.Col = Text[Caret.Line].Length;
            TextContainer.Insert(Caret.Line, Text[Caret.Line].Length, secondPart);
        }

        public bool MoveCaretRight(int n) {
            if (Caret.Col + n >= 0) {
                if (Caret.Col + n <= Text[Caret.Line].Length) {
                    Caret.Col += n;
                    return true;
                }

                if (MoveCaretDown(1)) {
                    Caret.Col = 0;
                    return true;
                }
            }
            else {
                if (MoveCaretDown(-1)) {
                    Caret.Col = Text[Caret.Line].Length;
                    return true;
                }
            }

            return false;
        }

        public bool MoveCaretDown(int n) {
            if (Caret.Line + n >= 0 && Caret.Line + n < Text.Count) {
                Caret.Line += n;

                // TODO replace to Mathf from Citrus.
                Caret.Col = Math.Min(Caret.Col, Text[Caret.Line].Length);

                return true;
            }

            return false;
        }

        public string GetCurrentLine() {
            return Text[Caret.Line];
        }
    }
}
