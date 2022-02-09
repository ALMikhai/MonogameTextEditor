using System;

namespace MonogameTextEditor.TextEditor.Caret
{
	public class Caret : ICaret
	{
		public int Line { get; set; }

		public int Col { get; set; }

		public void AssignFrom(ICaret c)
		{
			Line = c.Line;
			Col = c.Col;
		}

		public bool Equals(ICaret other) => Line == other.Line && Col == other.Col;

		public int CompareTo(ICaret other)
		{
				if (Equals(other))
					return 0;
				if (Line > other.Line)
					return 1;
				if (Line == other.Line && Col > other.Col)
					return 1;
				return -1;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Caret)obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Line, Col);
		}
	}
}
