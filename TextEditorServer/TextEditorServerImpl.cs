using System.Threading.Tasks;
using Grpc.Core;
using TextEditorProtos;

namespace TextEditorServer
{
	public class TextEditorServerImpl : TextEditorService.TextEditorServiceBase
	{
		public override Task<EditorText> GetText(RequestText request, ServerCallContext context)
		{
			return Task.FromResult(new EditorText() { Text = "Hello world!" });
		}
	}
}
