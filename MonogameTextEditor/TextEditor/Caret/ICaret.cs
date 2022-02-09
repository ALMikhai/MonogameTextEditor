using System;

namespace MonogameTextEditor.TextEditor.Caret
{
	public interface ICaret : IEquatable<ICaret>, IComparable<ICaret>
	{
		int Line { get; set; }
		int Col { get; set; }
		void AssignFrom(ICaret c);
	}
}
