namespace MonogameTextEditor.TextEditor
{
    public interface ISelectEditor
    {
        ICaretEditor CaretEditor { get; set; }
        bool MoveSelectRight(int n);
        bool MoveSelectDown(int n);
        void RemoveSelect();
        void ClearSelect();
    }
}
