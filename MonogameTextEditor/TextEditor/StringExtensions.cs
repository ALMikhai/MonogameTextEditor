using System;

namespace MonogameTextEditor.TextEditor
{
	public static class StringExtensions
	{
		public static LineSplitEnumerator SplitLines(this string str)
		{
			return new LineSplitEnumerator(str.AsSpan());
		}

		public ref struct LineSplitEnumerator
		{
			public ReadOnlySpan<char> Current { get; private set; }

			private ReadOnlySpan<char> str;
			private bool enumerationEnded;

			public LineSplitEnumerator(ReadOnlySpan<char> str)
			{
				this.str = str;
				Current = default;
				enumerationEnded = false;
			}

			public LineSplitEnumerator GetEnumerator()
			{
				return this;
			}

			public bool MoveNext()
			{
				var span = str;
				int index = span.IndexOf('\n');
				if (index == -1) {
					if (enumerationEnded) {
						return false;
					}
					str = ReadOnlySpan<char>.Empty;
					Current = span;
					enumerationEnded = true;
					return true;
				}
				Current = span.Slice(0, index);
				str = span.Slice(index + 1);
				return true;
			}
		}
	}
}
