using MonogameTextEditor.TextEditor.Caret;

namespace MonogameTextEditor.TextEditor.UndoSystem
{
	public class InsertUndoItem : IUndoItem
	{
		public TextPosition CaretPos { get; set; }

		public string Text { get; set; }

		public EditorDecorator TextEditor { get; set; }

		public UndoItemPool PoolCreator { get; set; }

		public void Do()
		{
			TextEditor.ClearSelection();
			TextEditor.Caret.Line = CaretPos.Line;
			TextEditor.Caret.Col = CaretPos.Col;
			TextEditor.CommonInsert(Text);
		}

		public void Undo()
		{
			TextEditor.ClearSelection();
			TextEditor.Text.Remove(CaretPos, Text.Length);
			TextEditor.Caret.SetPosition(CaretPos);
		}

		public void Release()
		{
			Text = null;
			TextEditor = null;
			PoolCreator.Release(this);
		}
	}

	public class RemoveSelectUndoItem : IUndoItem
	{
		public TextPosition SelectionStart { get; set; }

		public TextPosition SelectionEnd { get; set; }

		public string Text { get; set; }

		public TextPosition FirstCaret { get; set; }

		public TextPosition SecondCaret { get; set; }

		public EditorDecorator TextEditor { get; set; }

		public UndoItemPool PoolCreator { get; set; }

		public void Do()
		{
			TextEditor.SelectionStart.Line = SelectionStart.Line;
			TextEditor.SelectionStart.Col = SelectionStart.Col;
			TextEditor.SelectionEnd.Line = SelectionEnd.Line;
			TextEditor.SelectionEnd.Col = SelectionEnd.Col;
			TextEditor.Text.RemoveRange(FirstCaret, SecondCaret);
			TextEditor.Caret.SetPosition(FirstCaret.Line, FirstCaret.Col);
			TextEditor.ClearSelection();
		}

		public void Undo()
		{
			TextEditor.ClearSelection();
			TextEditor.SelectionStart.Line = SelectionStart.Line;
			TextEditor.SelectionStart.Col = SelectionStart.Col;
			TextEditor.SelectionEnd.Line = SelectionEnd.Line;
			TextEditor.SelectionEnd.Col = SelectionEnd.Col;
			TextEditor.Text.Insert(FirstCaret, Text);
			TextEditor.SelectionStart.SetPosition(SelectionStart.Line, SelectionStart.Col);
			TextEditor.SelectionEnd.SetPosition(SelectionEnd.Line, SelectionEnd.Col);
			TextEditor.Caret.SetPosition(SelectionEnd.Line, SelectionEnd.Col);
		}

		public void Release()
		{
			Text = null;
			TextEditor = null;
			PoolCreator.Release(this);
		}
	}

	public class RemoveRangeUndoItem : IUndoItem
	{
		public TextPosition First { get; set; }

		public TextPosition Second { get; set; }

		public TextPosition UndoCaretPos { get; set; }

		public string Text { get; set; }

		public EditorDecorator TextEditor { get; set; }

		public UndoItemPool PoolCreator { get; set; }

		public void Do()
		{
			TextEditor.ClearSelection();
			TextEditor.Text.RemoveRange(First, Second);
			TextEditor.Caret.SetPosition(First.Line, First.Col);
		}

		public void Undo()
		{
			TextEditor.ClearSelection();
			TextEditor.Text.Insert(First, Text);
			TextEditor.Caret.SetPosition(UndoCaretPos.Line, UndoCaretPos.Col);
		}

		public void Release()
		{
			Text = null;
			TextEditor = null;
			PoolCreator.Release(this);
		}
	}
}
