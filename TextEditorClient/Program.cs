using System.Threading;
using Grpc.Core;
using MonogameTextEditor;
using MonogameTextEditor.TextEditor;
using TextEditor.CaretEditor;
using TextEditor.SelectEditor;
using TextEditorProtos;

namespace EditorClient
{
	internal static class Program
	{
		private const string Host = "localhost";
		private const int Port = 50051;
		private static readonly ConnectRequest request = new ConnectRequest { Id = "Alex" };

		private static TextEditorService.TextEditorServiceClient client;
		private static ISelectEditor readonlyEditor;

		public static void Main(string[] args)
		{
			var channel = new Channel(Host + ":" + Port, ChannelCredentials.Insecure);
			client = new TextEditorService.TextEditorServiceClient(channel);
			var response = client.GetText(request);

			readonlyEditor = new SelectEditor(new CaretEditor());
			var textPresenter = new SelectTextPresenter(readonlyEditor);
			var selectTextEditor = new ReadonlyTextEditor(readonlyEditor);
			var editorWindow = new TextEditorWindow(textPresenter);
			readonlyEditor.Insert(response.Text);
			SubscribeToTextInsert();
			SubscribeToTextRemove();
			editorWindow.Run();
			channel.ShutdownAsync().Wait();
		}

		private static async void SubscribeToTextInsert()
		{
			using var inserted = client.TextInserted(request);
			while (await inserted.ResponseStream.MoveNext(CancellationToken.None))
			{
				var serverMessage = inserted.ResponseStream.Current;
				readonlyEditor.Text.Insert(serverMessage.Line, serverMessage.Col, serverMessage.Text);
			}
		}

		private static async void SubscribeToTextRemove()
		{
			using var removed = client.TextRemoved(request);
			while (await removed.ResponseStream.MoveNext(CancellationToken.None))
			{
				var serverMessage = removed.ResponseStream.Current;
				readonlyEditor.Text.Remove(serverMessage.Line, serverMessage.Col, serverMessage.Lenght);
			}
		}
	}
}
