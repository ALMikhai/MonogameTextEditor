using System;

namespace MonogameTextEditor.TextEditor.Caret
{
	public interface ICaret : IEquatable<ICaret>, IComparable<ICaret>
	{
		event Action PositionUpdated;

		TextPosition Position { get; set; }

		int Line { get; set; }

		int Col { get; set; }

		/// <summary>
		/// When using this method, PositionUpdated event will be called.
		/// If you don't need to invoke an event, use the Line and Col properties.
		/// </summary>
		void SetPosition(int line, int col);

		void SetPosition(TextPosition pos);

		void AssignFrom(ICaret c);
	}
}
