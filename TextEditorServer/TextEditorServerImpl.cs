using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using MonogameTextEditor.TextEditor;
using TextEditor.MultiUserEditor;
using TextEditor.SelectEditor;
using TextEditorProtos;

namespace TextEditorServer
{
	public class TextEditorServerImpl : TextEditorService.TextEditorServiceBase
	{
		private ISelectEditor editor;
		private Dictionary<string, IServerStreamWriter<InsertText>> insertTextResponses;
		private Dictionary<string, IServerStreamWriter<RemoveText>> removeTextResponses;

		public TextEditorServerImpl(ISelectEditor editor)
		{
			this.editor = editor;
			insertTextResponses = new Dictionary<string, IServerStreamWriter<InsertText>>();
			removeTextResponses = new Dictionary<string, IServerStreamWriter<RemoveText>>();

			this.editor.OnTextInsert += EditorOnOnTextInsert;
			this.editor.OnTextRemove += EditorOnOnTextRemove;
		}

		private async void EditorOnOnTextRemove(int line, int col, int lenght)
		{
			var removeText = new RemoveText() {Line = line, Col = col, Lenght = lenght};
			foreach (var writer in removeTextResponses) {
				await writer.Value.WriteAsync(removeText);
			}
		}

		private async void EditorOnOnTextInsert(int line, int col, string text)
		{
			var insertText = new InsertText() {Line = line, Col = col, Text = text};
			foreach (var writer in insertTextResponses) {
				await writer.Value.WriteAsync(insertText);
			}
		}

		public override Task<EditorText> GetText(ConnectRequest request, ServerCallContext context)
		{
			return Task.FromResult(new EditorText() { Text = editor.Text.ToString() });
		}

		public override Task TextInserted(ConnectRequest request, IServerStreamWriter<InsertText> responseStream, ServerCallContext context)
		{
			if (!insertTextResponses.ContainsKey(request.Id)) {
				insertTextResponses.Add(request.Id, responseStream);
			}
			while (true) { }
		}

		public override Task TextRemoved(ConnectRequest request, IServerStreamWriter<RemoveText> responseStream, ServerCallContext context)
		{
			if (!removeTextResponses.ContainsKey(request.Id)) {
				removeTextResponses.Add(request.Id, responseStream);
			}
			while (true) { }
		}
	}
}
