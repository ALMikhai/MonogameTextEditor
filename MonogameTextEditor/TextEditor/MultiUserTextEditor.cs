using TextEditor.MultiUserEditor;

namespace MonogameTextEditor.TextEditor
{
	public class MultiUserTextEditor
	{
		public MultiUserTextEditor(MultiUserEditor editor)
		{
			new SelectTextEditor(editor.Editor);
		}
	}
}
