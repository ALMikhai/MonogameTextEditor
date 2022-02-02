using System.Windows.Forms;

namespace MonogameTextEditor.TextEditor
{
	public class CaretTextEditor
	{
		private readonly ICaretEditor caretEditor;

		public CaretTextEditor(ICaretEditor caretEditor)
		{
			this.caretEditor = caretEditor;
			CmdObserver.BackSpace += caretEditor.RemoveBackward;
			CmdObserver.Delete += caretEditor.RemoveForward;
			CmdObserver.Enter += () => caretEditor.Insert("\n");
			CmdObserver.MoveCharPrev += () => caretEditor.MoveCaretRight(-1);
			CmdObserver.MoveCharNext += () => caretEditor.MoveCaretRight(1);
			CmdObserver.MoveLinePrev += () => caretEditor.MoveCaretDown(-1);
			CmdObserver.MoveLineNext += () => caretEditor.MoveCaretRight(1);
			CmdObserver.OnTextInsert += caretEditor.Insert;
			CmdObserver.Copy += () => {
				var text = caretEditor.GetCurrentLine();
				if (!string.IsNullOrEmpty(text))
					Clipboard.SetText(text);
			};
			CmdObserver.Paste += () => caretEditor.Insert(Clipboard.GetText());
			CmdObserver.MoveWordNext += caretEditor.MoveCaretToNextWord;
			CmdObserver.MoveWordPrev += caretEditor.MoveCaretToPrevWord;
			CmdObserver.MoveLineStart += caretEditor.MoveCaretToStartOfLine;
			CmdObserver.MoveLineEnd += caretEditor.MoveCaretToEndOfLine;
		}
	}

	public class SelectTextEditor
	{
		private readonly ISelectEditor selectEditor;

		public SelectTextEditor(ISelectEditor selectEditor)
		{
			this.selectEditor = selectEditor;
			CmdObserver.Enter += () => selectEditor.Insert("\n");
			CmdObserver.MoveCharPrev += () => selectEditor.MoveCaretRight(-1);
			CmdObserver.MoveCharNext += () => selectEditor.MoveCaretRight(1);
			CmdObserver.MoveLinePrev += () => selectEditor.MoveCaretDown(-1);
			CmdObserver.MoveLineNext += () => selectEditor.MoveCaretDown(1);
			CmdObserver.OnTextInsert += selectEditor.Insert;
			CmdObserver.BackSpace += selectEditor.RemoveBackward;
			CmdObserver.Delete += selectEditor.RemoveForward;
			CmdObserver.Copy += () => {
				var text = selectEditor.GetSelectedText();
				if (!string.IsNullOrEmpty(text))
					Clipboard.SetText(text);
			};
			CmdObserver.Paste += () => selectEditor.Insert(Clipboard.GetText());
			CmdObserver.SelectCharNext += () => selectEditor.MoveSelectRight(1);
			CmdObserver.SelectCharPrev += () => selectEditor.MoveSelectRight(-1);
			CmdObserver.SelectLineNext += () => selectEditor.MoveSelectDown(1);
			CmdObserver.SelectLinePrev += () => selectEditor.MoveSelectDown(-1);
			CmdObserver.MoveWordNext += selectEditor.MoveCaretToNextWord;
			CmdObserver.MoveWordPrev += selectEditor.MoveCaretToPrevWord;
			CmdObserver.SelectWordNext += selectEditor.MoveSelectToNextWord;
			CmdObserver.SelectWordPrev += selectEditor.MoveSelectToPrevWord;
			CmdObserver.SelectAll += selectEditor.SelectAll;
			CmdObserver.MoveLineStart += selectEditor.MoveCaretToStartOfLine;
			CmdObserver.MoveLineEnd += selectEditor.MoveCaretToEndOfLine;
			CmdObserver.Cut += () => {
				var text = selectEditor.CutSelectedText();
				if (!string.IsNullOrEmpty(text))
					Clipboard.SetText(text);
			};
			CmdObserver.SelectLineStart += selectEditor.MoveSelectToStartLine;
			CmdObserver.SelectLineEnd += selectEditor.MoveSelectToEndLine;
			// TODO Write with tangerine functions from WordUtils.
			// CmdObserver.SelectCurrentWord += selectEditor.SelectWordAtCaretPos;
			CmdObserver.DeleteWordNext += selectEditor.RemoveWordNext;
			CmdObserver.DeleteWordPrev += selectEditor.RemoveWordPrev;
		}
	}
}
