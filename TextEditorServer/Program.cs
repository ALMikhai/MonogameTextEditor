using System;
using Grpc.Core;
using TextEditorProtos;

namespace TextEditorServer
{
	internal static class Program
	{
		private const string Host = "localhost";
		private const int Port = 50051;

		public static void Main(string[] args)
		{
			// Build a server
			var server = new Server {
				Services = { TextEditorService.BindService(new TextEditorServerImpl()) },
				Ports = { new ServerPort(Host, Port, ServerCredentials.Insecure) }
			};

			// Start server
			server.Start();

			Console.WriteLine("GreeterServer listening on port " + Port);
			Console.WriteLine("Press any key to stop the server...");
			Console.ReadKey();

			server.ShutdownAsync().Wait();
		}
	}
}
