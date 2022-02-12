using System;
using MonogameTextEditor.TextEditor;
using TextEditor.CaretEditor;
using TextEditor.SelectEditor;

namespace MonogameTextEditor {
    public static class Program {
        [STAThread]
        private static void Main()
        {
            var selectEditor = new SelectEditor(new CaretEditor());
            var selectTextPresenter = new SelectTextPresenter(selectEditor);
            var selectTextEditor = new SelectTextEditor(selectEditor);
            using (var game = new TextEditorWindow(selectTextPresenter)) {
                game.Run();
            }
        }
    }
}
