using System;
using Grpc.Core;
using MonogameTextEditor;
using TextEditorProtos;

namespace EditorClient
{
	internal static class Program
	{
		private const string Host = "localhost";
		private const int Port = 50051;

		public static void Main(string[] args)
		{
			// Create a channel
			var channel = new Channel(Host + ":" + Port, ChannelCredentials.Insecure);

			// Create a client with the channel
			var client = new TextEditorService.TextEditorServiceClient(channel);

			// Create a request
			var request = new RequestText();

			// Send the request
			var response = client.GetText(request);

			using var textEditorWindow = new TextEditorWindow();
			var multiUserEditor = textEditorWindow.Editor;
			var selectEditor = multiUserEditor.Editor;

			selectEditor.Insert(response.Text);
			textEditorWindow.Run();

			// Shutdown
			channel.ShutdownAsync().Wait();
		}
	}
}
