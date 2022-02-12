using System.Windows.Forms;
using TextEditor.CaretEditor;
using TextEditor.SelectEditor;

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
			CmdObserver.MoveLineStart += caretEditor.MoveCaretToLineStart;
			CmdObserver.MoveLineEnd += caretEditor.MoveCaretToLineEnd;
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
			CmdObserver.SelectCharNext += () => selectEditor.MoveSelectionRight(1);
			CmdObserver.SelectCharPrev += () => selectEditor.MoveSelectionRight(-1);
			CmdObserver.SelectLineNext += () => selectEditor.MoveSelectionDown(1);
			CmdObserver.SelectLinePrev += () => selectEditor.MoveSelectionDown(-1);
			CmdObserver.MoveWordNext += selectEditor.MoveCaretToNextWord;
			CmdObserver.MoveWordPrev += selectEditor.MoveCaretToPrevWord;
			CmdObserver.SelectWordNext += selectEditor.MoveSelectionToNextWord;
			CmdObserver.SelectWordPrev += selectEditor.MoveSelectionToPrevWord;
			CmdObserver.SelectAll += selectEditor.SelectAll;
			CmdObserver.MoveLineStart += selectEditor.MoveCaretToLineStart;
			CmdObserver.MoveLineEnd += selectEditor.MoveCaretToLineEnd;
			CmdObserver.Cut += () => {
				var text = selectEditor.Cut();
				if (!string.IsNullOrEmpty(text))
					Clipboard.SetText(text);
			};
			CmdObserver.SelectLineStart += selectEditor.MoveSelectionToLineStart;
			CmdObserver.SelectLineEnd += selectEditor.MoveSelectionToLineEnd;
			// TODO Write with tangerine functions from WordUtils.
			// CmdObserver.SelectCurrentWord += selectEditor.SelectWordAtCaretPos;
			CmdObserver.DeleteWordNext += selectEditor.RemoveNextWord;
			CmdObserver.DeleteWordPrev += selectEditor.RemovePrevWord;
			CmdObserver.Undo += selectEditor.Undo;
			CmdObserver.Redo += selectEditor.Redo;
		}
	}
}
