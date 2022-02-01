using System;
using System.Collections.Generic;

namespace MonogameTextEditor.TextEditor {
    public interface ICaretEditor {
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

    public class CaretEditor : ICaretEditor {
        public ICaretPosition Caret { get; } = new DrawableCaret();
        public ITextCollection Text { get; } = new ArrayStringText();

        public void RemoveForward() {
            var curLineLenght = Text[Caret.Line].Length;
            if (Caret.Line + 1 < Text.GetLineCount() && Caret.Col == curLineLenght) {
                var line = Text[Caret.Line + 1];
                Text.RemoveLine(Caret.Line + 1);
                Text.Insert(Caret.Line, curLineLenght, line);
            }
            else if (Caret.Col < curLineLenght) Text.Remove(Caret.Line, Caret.Col, 1);
        }

        public void RemoveBackward() {
            if (MoveCaretRight(-1))
                RemoveForward();
        }

        public void Insert(string s) {
            var lines = s.Split('\n');
            var firstPart = Text[Caret.Line].Substring(0, Caret.Col);
            var secondPart = Text[Caret.Line].Substring(Caret.Col, Text[Caret.Line].Length - Caret.Col);
            Text.Remove(Caret.Line, Caret.Col, Text[Caret.Line].Length - Caret.Col);
            Text.Insert(Caret.Line, firstPart.Length, lines[0]);
            for (var i = 1; i < lines.Length; i++) {
                var line = lines[i];
                Caret.Line++;
                Text.InsertLine(Caret.Line, line);
            }
            Caret.Col = Text[Caret.Line].Length;
            Text.Insert(Caret.Line, Text[Caret.Line].Length, secondPart);
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
            if (Caret.Line + n >= 0 && Caret.Line + n < Text.GetLineCount()) {
                Caret.Line += n;

                // TODO replace to Mathf from Citrus.
                Caret.Col = Math.Min(Caret.Col, Text[Caret.Line].Length);

                return true;
            }

            return false;
        }

        public void MoveCaretToNextWord() {
            var pos = GetNextWordPos();
            Caret.Col = pos.Col;
            Caret.Line = pos.Line;
        }

        public void MoveCaretToPrevWord() {
            var pos = GetPrevWordPos();
            Caret.Col = pos.Col;
            Caret.Line = pos.Line;
        }

        public void MoveCaretToEndOfLine() {
            Caret.Col = Text[Caret.Line].Length;
        }

        public void MoveCaretToStartOfLine() {
            Caret.Col = 0;
        }

        public (int Line, int Col) GetNextWordPos() {
            var pos = (Caret.Line, Caret.Col);
            var line = Text[pos.Line];
            if (line.Length == pos.Col) {
                return Text.GetLineCount() == (pos.Line + 1) ? (pos.Line, pos.Col) : (pos.Line + 1, 0);
            }
            while (line.Length > pos.Col && !char.IsWhiteSpace(line[pos.Col])) {
                pos.Col++;
            }
            while (line.Length > pos.Col && char.IsWhiteSpace(line[pos.Col])) {
                pos.Col++;
            }

            return pos;
        }

        public (int Line, int Col) GetPrevWordPos() {
            var pos = (Caret.Line, Caret.Col);
            var line = Text[pos.Line];
            if (0 == pos.Col) {
                return 0 == pos.Line ? (pos.Line, pos.Col) : (pos.Line - 1, Text[pos.Line - 1].Length);
            }
            while (0 < pos.Col && char.IsWhiteSpace(line[pos.Col - 1])) {
                pos.Col--;
            }
            while (0 < pos.Col && !char.IsWhiteSpace(line[pos.Col - 1])) {
                pos.Col--;
            }

            return pos;
        }

        public string GetCurrentLine() {
            return Text[Caret.Line] + (Caret.Line + 1 == Text.GetLineCount() ? "" : "\n");
        }
    }
}
