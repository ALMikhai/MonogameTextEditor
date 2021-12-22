using System;

namespace MonogameTextEditor.TextEditor {
    public interface ICaretPosition {
        int Line { get; set; }
        int Col { get; set; }
        bool IsVisible { get; set; }
        ICaretPosition Clone();
        void AssignFrom(ICaretPosition c);

        public static bool operator <(ICaretPosition a, ICaretPosition b) {
            if (a.Line < b.Line)
                return true;

            if (a.Line == b.Line && a.Col < b.Col)
                return true;

            return false;
        }

        public static bool operator >(ICaretPosition a, ICaretPosition b) {
            if (a == b)
                return false;

            if (a.Line > b.Line)
                return true;

            if (a.Line == b.Line && a.Col > b.Col)
                return true;

            return false;
        }
    }

    public class DrawableCaret : ICaretPosition {
        public int Line { get; set; }
        public int Col { get; set; }
        public bool IsVisible { get; set; } = true;

        public ICaretPosition Clone() {
            return new DrawableCaret { Line = Line, Col = Col, IsVisible = IsVisible };
        }

        public void AssignFrom(ICaretPosition c) {
            Line = c.Line;
            Col = c.Col;
            IsVisible = true;
        }

        public static bool operator <(DrawableCaret a, DrawableCaret b) {
            if (a.Line < b.Line)
                return true;

            if (a.Line == b.Line && a.Col < b.Col)
                return true;

            return false;
        }

        public static bool operator >(DrawableCaret a, DrawableCaret b) {
            if (a == b)
                return false;

            if (a.Line > b.Line)
                return true;

            if (a.Line == b.Line && a.Col > b.Col)
                return true;

            return false;
        }

        public bool Equals(DrawableCaret other) {
            return Line == other.Line && Col == other.Col && IsVisible == other.IsVisible;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DrawableCaret)obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Line, Col, IsVisible);
        }
    }
}
