using System;

namespace MonogameTextEditor.TextEditor.Caret
{
	public struct TextPosition
	{
		public int Line { get; set; }

		public int Col { get; set; }

		public TextPosition(int line = 0, int col = 0)
		{
			Line = line;
			Col = col;
		}
	}

	public class Caret : ICaret
	{
		private TextPosition position;

		public event Action PositionUpdated;

		public TextPosition Position
		{
			get => position;
			set => position = value;
		}

		public int Line { get => position.Line; set => position.Line = value; }

		public int Col { get => position.Col; set => position.Col = value; }

		public void SetPosition(int line, int col)
		{
			Line = line;
			Col = col;
			PositionUpdated?.Invoke();
		}

		public void SetPosition(TextPosition pos)
		{
			Position = pos;
			PositionUpdated?.Invoke();
		}

		public void AssignFrom(ICaret c)
		{
			SetPosition(c.Line, c.Col);
		}

		public bool Equals(ICaret other)
		{
			return Line == other.Line && Col == other.Col;
		}

		public int CompareTo(ICaret other)
		{
			if (Equals(other)) {
				return 0;
			}
			if (Line > other.Line) {
				return 1;
			}
			if (Line == other.Line && Col > other.Col) {
				return 1;
			}
			return -1;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((Caret)obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Line, Col);
		}
	}
}
