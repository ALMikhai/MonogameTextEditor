namespace MonogameTextEditor.TextEditor
{
    public interface ICaretPosition
    {
        int Line { get; set; }
        int Col { get; set; }
        bool IsVisible { get; set; }
        ICaretPosition Clone();
        void AssignFrom(ICaretPosition c);
    }

    public class DrawableCaret : ICaretPosition
    {
        public int Line { get; set; } = 0;
        public int Col { get; set; } = 0;
        public bool IsVisible { get; set; } = true;

        public ICaretPosition Clone()
        {
            return new DrawableCaret() { Line = Line, Col = Col, IsVisible = IsVisible };
        }

        public void AssignFrom(ICaretPosition c)
        {
            Line = c.Line;
            Col = c.Col;
            IsVisible = true;
        }
    }
}
