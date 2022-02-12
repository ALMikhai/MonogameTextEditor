using Grpc.Core;
using MonogameTextEditor;
using MonogameTextEditor.TextEditor;
using TextEditor.CaretEditor;
using TextEditor.SelectEditor;
using TextEditorProtos;

namespace TextEditorServer
{
	internal static class Program
	{
		private const string Host = "localhost";
		private const int Port = 50051;

		public static void Main(string[] args)
		{
			var selectEditor = new SelectEditor(new CaretEditor());
			var selectTextPresenter = new SelectTextPresenter(selectEditor);
			var selectTextEditor = new SelectTextEditor(selectEditor);
			using var textEditorWindow = new TextEditorWindow(selectTextPresenter);

			// Build a server
			var server = new Server {
				Services = { TextEditorService.BindService(new TextEditorServerImpl(selectEditor)) },
				Ports = { new ServerPort(Host, Port, ServerCredentials.Insecure) }
			};

			// Start server and editor
			server.Start();
			textEditorWindow.Run();

			server.ShutdownAsync().Wait();
		}
	}
}
