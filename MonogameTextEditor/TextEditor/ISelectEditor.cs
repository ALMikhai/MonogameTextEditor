using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonogameTextEditor.TextEditor {
    public interface ISelectEditor {
        ICaretEditor CaretEditor { get; }
        ICaretPosition StartPosition { get; }
        ICaretPosition EndPosition { get; }
        List<string> Text { get; }
        bool MoveSelectRight(int n);
        bool MoveSelectDown(int n);
        void Insert(string text);
        void RemoveSelect();
        void ClearSelection();
        bool HasSelection();
        void RemoveForward();
        void RemoveBackward();
        bool MoveCaretRight(int n);
        bool MoveCaretDown(int n);
        string GetSelectedText();
        void MoveSelectToNextWord();
        void MoveSelectToPrevWord();
        void MoveCaretToNextWord();
        void MoveCaretToPrevWord();
        void SelectAll();
        void MoveCaretToEndOfLine();
        void MoveCaretToStartOfLine();
        string CutSelectedText();
    }

    public class SelectEditor : ISelectEditor {
        public ICaretEditor CaretEditor { get; }
        public ICaretPosition StartPosition { get; } = new DrawableCaret();
        public ICaretPosition EndPosition { get; } = new DrawableCaret();
        public List<string> Text => CaretEditor.Text;

        public SelectEditor(ICaretEditor caretEditor) {
            CaretEditor = caretEditor;
        }

        public bool MoveSelectRight(int n) {
            if (!HasSelection())
                ClearSelection();

            if (CaretEditor.MoveCaretRight(n)) {
                EndPosition.AssignFrom(CaretEditor.Caret);
                return true;
            }

            return false;
        }

        public bool MoveSelectDown(int n) {
            if (!HasSelection())
                ClearSelection();

            if (CaretEditor.MoveCaretDown(n)) {
                EndPosition.AssignFrom(CaretEditor.Caret);
                return true;
            }

            return false;
        }

        public void Insert(string text) {
            if (HasSelection())
                RemoveSelect();

            CaretEditor.Insert(text);
        }

        public void RemoveSelect() {
            if (!HasSelection())
                return;

            var (firstCaret, secondCaret) = GetSortedCarets();

            if (firstCaret.Line == secondCaret.Line) {
                CaretEditor.TextContainer.Remove(firstCaret.Line, firstCaret.Col, secondCaret.Col - firstCaret.Col);
            } else {
                CaretEditor.TextContainer.Remove(firstCaret.Line, firstCaret.Col,
                    Text[firstCaret.Line].Length - firstCaret.Col);

                for (var i = firstCaret.Line + 1; i < secondCaret.Line; i++) {
                    CaretEditor.TextContainer.RemoveLine(firstCaret.Line + 1);
                }

                CaretEditor.TextContainer.Remove(firstCaret.Line + 1, 0, secondCaret.Col);
                CaretEditor.TextContainer.Insert(firstCaret.Line, firstCaret.Col, Text[firstCaret.Line + 1]);
                CaretEditor.TextContainer.RemoveLine(firstCaret.Line + 1);
            }

            CaretEditor.Caret.AssignFrom(firstCaret);
            ClearSelection();
        }

        public void ClearSelection() {
            StartPosition.AssignFrom(CaretEditor.Caret);
            EndPosition.AssignFrom(CaretEditor.Caret);
        }

        public bool HasSelection() => !StartPosition.Equals(EndPosition);

        public void RemoveForward() {
            if (HasSelection())
                RemoveSelect();
            else
                CaretEditor.RemoveForward();
        }

        public void RemoveBackward() {
            if (HasSelection())
                RemoveSelect();
            else
                CaretEditor.RemoveBackward();
        }

        public bool MoveCaretRight(int n) {
            if (!HasSelection())
                return CaretEditor.MoveCaretRight(n);
            var (firstCaret, secondCaret) = GetSortedCarets();
            CaretEditor.Caret.AssignFrom(n < 0 ? firstCaret : secondCaret);
            ClearSelection();
            return true;
        }

        public bool MoveCaretDown(int n) {
            if (HasSelection()) {
                var (firstCaret, secondCaret) = GetSortedCarets();
                CaretEditor.Caret.AssignFrom(n < 0 ? firstCaret : secondCaret);
                ClearSelection();
            }

            return CaretEditor.MoveCaretDown(n);
        }

        public string GetSelectedText() {
            if (!HasSelection())
                return CaretEditor.GetCurrentLine();

            var res = new StringBuilder();
            var (firstCaret, secondCaret) = GetSortedCarets();

            if (firstCaret.Line == secondCaret.Line) {
                res.Append(Text[firstCaret.Line][firstCaret.Col..secondCaret.Col]);
            } else {
                res.AppendLine(Text[firstCaret.Line][firstCaret.Col..Text[firstCaret.Line].Length]);

                for (var i = firstCaret.Line + 1; i < secondCaret.Line; i++) {
                    res.AppendLine(Text[i]);
                }

                res.Append(Text[secondCaret.Line][0..secondCaret.Col]);
            }

            return res.ToString();
        }

        public void MoveSelectToNextWord() {
            if (!HasSelection())
                ClearSelection();

            CaretEditor.MoveCaretToNextWord();
            EndPosition.AssignFrom(CaretEditor.Caret);
        }

        public void MoveSelectToPrevWord() {
            if (!HasSelection())
                ClearSelection();

            CaretEditor.MoveCaretToPrevWord();
            EndPosition.AssignFrom(CaretEditor.Caret);
        }

        public void MoveCaretToNextWord() {
            if (HasSelection())
                ClearSelection();

            CaretEditor.MoveCaretToNextWord();
        }

        public void MoveCaretToPrevWord() {
            if (HasSelection())
                ClearSelection();

            CaretEditor.MoveCaretToPrevWord();
        }

        public void SelectAll() {
            StartPosition.Line = 0;
            StartPosition.Col = 0;
            EndPosition.Line = Text.Count - 1;
            EndPosition.Col = Text.Last().Length;
            CaretEditor.Caret.AssignFrom(EndPosition);
        }

        public void MoveCaretToEndOfLine() {
            if (HasSelection()) {
                var (_, secondCaret) = GetSortedCarets();
                CaretEditor.Caret.AssignFrom(secondCaret);
                ClearSelection();
            }

            CaretEditor.MoveCaretToEndOfLine();
        }

        public void MoveCaretToStartOfLine() {
            if (HasSelection()) {
                var (firstCaret, _) = GetSortedCarets();
                CaretEditor.Caret.AssignFrom(firstCaret);
                ClearSelection();
            }

            CaretEditor.MoveCaretToStartOfLine();
        }

        public string CutSelectedText() {
            var res = GetSelectedText();
            if (HasSelection()) {
                RemoveSelect();
            }
            else {
                CaretEditor.MoveCaretToStartOfLine();
                var pos = CaretEditor.Caret;
                if (Text.Count == pos.Line + 1) {
                    CaretEditor.TextContainer.Remove(pos.Line, 0, Text[pos.Line].Length);
                }
                else {
                    CaretEditor.TextContainer.RemoveLine(pos.Line);
                }
            }
            return res;
        }

        private (ICaretPosition firstCaret, ICaretPosition fecondCaret) GetSortedCarets() {
            var firstCaret = StartPosition;
            var secondCaret = EndPosition;

            if (firstCaret > secondCaret)
                (firstCaret, secondCaret) = (secondCaret, firstCaret);

            return (firstCaret, secondCaret);
        }
    }
}