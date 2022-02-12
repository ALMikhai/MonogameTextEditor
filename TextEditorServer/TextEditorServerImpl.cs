using System.Threading.Tasks;
using Grpc.Core;
using TextEditor.SelectEditor;
using TextEditorProtos;

namespace TextEditorServer
{
	public class TextEditorServerImpl : TextEditorService.TextEditorServiceBase
	{
		public ISelectEditor Editor { get; }

		public TextEditorServerImpl(ISelectEditor editor)
		{
			Editor = editor;
		}

		public override Task<EditorText> GetText(RequestText request, ServerCallContext context)
		{
			return Task.FromResult(new EditorText() { Text = Editor.Text.ToString() });
		}
	}
}
