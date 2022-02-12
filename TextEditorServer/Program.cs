using Grpc.Core;
using MonogameTextEditor;
using TextEditorProtos;

namespace TextEditorServer
{
	internal static class Program
	{
		private const string Host = "localhost";
		private const int Port = 50051;

		public static void Main(string[] args)
		{
			using var textEditorWindow = new TextEditorWindow();
			var multiUserEditor = textEditorWindow.Editor;
			var selectEditor = multiUserEditor.Editor;

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
