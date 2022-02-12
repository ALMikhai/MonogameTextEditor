using System.Collections.Generic;
using TextEditor.Caret;
using TextEditor.SelectEditor;

namespace TextEditor.MultiUserEditor
{
	public class MultiUserEditor
	{
		// Must be thread safety.
		public Dictionary<string, ICaret> ExternalCarets { get; } = new Dictionary<string, ICaret>();
		public Dictionary<string, (ICaret, ICaret)> ExternalSelections { get; } = new Dictionary<string, (ICaret, ICaret)>();
		public ISelectEditor Editor { get; } = new SelectEditor.SelectEditor(new CaretEditor.CaretEditor());
	}
}
