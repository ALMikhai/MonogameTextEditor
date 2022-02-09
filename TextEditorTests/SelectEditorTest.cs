using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using MonogameTextEditor.TextEditor;
using MonogameTextEditor.TextEditor.CaretEditor;
using MonogameTextEditor.TextEditor.SelectEditor;
using NUnit.Framework;

namespace TextEditorTests
{
	[TestFixture]
	public class SelectEditorTest
	{
		[Test]
		public void InsertWithoutSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			Assert.AreEqual(editor.CaretEditor.Text.ToString(), "Hello world!");
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void SelectAtLine()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-5);
			editor.MoveSelectionRight(3);
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.SelectionEnd.Col, 10);
			Assert.AreEqual(editor.SelectionStart.Col, 12);
		}

		[Test]
		public void SelectMultiLine()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!\nAbc\n321");
			editor.MoveCaretRight(-1);
			editor.MoveSelectionDown(-2);
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 2);
			Assert.AreEqual(editor.SelectionStart.Line, 2);
			Assert.AreEqual(editor.SelectionStart.Col, 2);
		}

		[Test]
		public void RemoveSelectionAfterMoveCaret()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-5);
			Assert.IsTrue(editor.HasSelection());
			editor.MoveCaretRight(1);
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void RemoveSelectionAfterRemove()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-1);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveBackward();
			Assert.IsFalse(editor.HasSelection());
			editor.MoveSelectionRight(-1);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveForward();
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void InsertWithSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-3);
			Assert.IsTrue(editor.HasSelection());
			editor.Insert(" 123");
			Assert.AreEqual(editor.CaretEditor.Text.ToString(), "Hello wor 123");
			Assert.IsFalse(editor.HasSelection());
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 13);
		}

		[Test]
		public void RemoveSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-2);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveForward();
			Assert.AreEqual(editor.CaretEditor.Text.ToString(), "Hello worl");
			Assert.IsFalse(editor.HasSelection());

			editor.Insert("d!");
			editor.MoveSelectionRight(-2);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveBackward();
			Assert.AreEqual(editor.CaretEditor.Text.ToString(), "Hello worl");
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void ClearSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-2);
			Assert.IsTrue(editor.HasSelection());
			editor.ClearSelection();
			Assert.IsFalse(editor.HasSelection());
			editor.Insert("123");
			Assert.AreEqual(editor.CaretEditor.Text.ToString(), "Hello worl123d!");
		}

		[Test]
		public void GetSelectedText()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-2);
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.GetSelectedText(), "d!");
			editor.MoveCaretRight(2);
			editor.Insert("\nHello\nHello");
			editor.MoveSelectionDown(-2);
			Assert.AreEqual(editor.GetSelectedText(), " world!\nHello\nHello");
			editor.ClearSelection();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world!\n");
		}

		[Test]
		public void MoveSelectToWord()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveSelectionToPrevWord();
			Assert.AreEqual(editor.SelectionEnd.Line, 1);
			Assert.AreEqual(editor.SelectionEnd.Col, 12);
			editor.MoveSelectionToPrevWord();
			Assert.AreEqual(editor.SelectionEnd.Line, 1);
			Assert.AreEqual(editor.SelectionEnd.Col, 6);
			editor.MoveSelectionToPrevWord();
			Assert.AreEqual(editor.SelectionEnd.Line, 1);
			Assert.AreEqual(editor.SelectionEnd.Col, 0);
			editor.MoveSelectionToPrevWord();
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 17);
			editor.MoveSelectionToPrevWord();
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 12);
			editor.MoveSelectionToNextWord();
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 17);
			editor.MoveSelectionToNextWord();
			Assert.AreEqual(editor.SelectionEnd.Line, 1);
			Assert.AreEqual(editor.SelectionEnd.Col, 0);
			Assert.AreEqual(editor.SelectionStart.Line, 1);
			Assert.AreEqual(editor.SelectionStart.Col, 17);
		}

		[Test]
		public void SelectAll()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.SelectAll();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world Hello\nworld Hello world");
			editor.RemoveSelection();
			Assert.AreEqual(editor.Text.ToString(), "");
		}

		[Test]
		public void CaretPositionAfterMoveCaret()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.CaretEditor.Caret.Line = 0;
			editor.CaretEditor.Caret.Col = 0;
			editor.MoveSelectionToNextWord();
			editor.MoveSelectionToNextWord();
			editor.MoveSelectionToNextWord();
			editor.MoveCaretRight(-1);
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			editor.MoveSelectionToNextWord();
			editor.MoveSelectionToNextWord();
			editor.MoveSelectionToNextWord();
			editor.MoveCaretRight(1);
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 17);
			editor.MoveSelectionToPrevWord();
			editor.MoveCaretDown(1);
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 17);
			editor.MoveSelectionToPrevWord();
			editor.MoveCaretDown(-1);
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 12);
		}

		[Test]
		public void MoveSelectToStartAndEndOfLine()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectionToLineStart();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world!");
			editor.ClearSelection();
			editor.MoveCaretToNextWord();
			editor.MoveSelectionToLineEnd();
			Assert.AreEqual(editor.GetSelectedText(), "world!");
			editor.MoveSelectionToLineStart();
			Assert.AreEqual(editor.GetSelectedText(), "Hello ");
		}

		[Test]
		public void MoveSelectToStartAndEndOfLineWithSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveSelectionToLineStart();
			editor.MoveSelectionRight(-1);
			editor.MoveSelectionToPrevWord();
			editor.MoveSelectionToLineStart();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world Hello\nworld Hello world");
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 0);
			Assert.AreEqual(editor.SelectionStart.Line, 1);
			Assert.AreEqual(editor.SelectionStart.Col, 17);
			editor.MoveSelectionToLineEnd();
			Assert.AreEqual(editor.GetSelectedText(), "\nworld Hello world");
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 17);
			Assert.AreEqual(editor.SelectionStart.Line, 1);
			Assert.AreEqual(editor.SelectionStart.Col, 17);
		}

		[Test]
		public void MoveCaretToWordWithSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveCaretToPrevWord();
			editor.MoveSelectionToPrevWord();
			editor.MoveCaretToNextWord();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 17);
			Assert.IsFalse(editor.HasSelection());
			editor.MoveSelectionToPrevWord();
			editor.MoveCaretToPrevWord();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 6);
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void Cut()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!\nHello world!");
			editor.MoveSelectionToLineStart();
			editor.MoveSelectionRight(-4);
			var text = editor.Cut();
			Assert.AreEqual(text, "ld!\nHello world!");
			Assert.IsFalse(editor.HasSelection());
			Assert.AreEqual(editor.Text.ToString(), "Hello wor");
			editor.Insert(text);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world!");
			text = editor.Cut();
			Assert.AreEqual(text, "Hello world!");
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			editor.Insert(text);
			editor.CaretEditor.Caret.Line = 0;
			editor.CaretEditor.Caret.Col = 0;
			text = editor.Cut();
			Assert.AreEqual(text, "Hello world!\n");
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			Assert.AreEqual(editor.Text.GetLineCount(), 1);
		}

		[Test]
		public void RemoveNextOrPrevWord()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!\nHello world!");
			editor.MoveCaretRight(-1);
			editor.RemovePrevWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello !");
			editor.MoveCaretRight(-2);
			editor.RemoveNextWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHell!");
			editor.MoveCaretDown(-1);
			editor.RemoveNextWord();
			editor.RemoveNextWord();
			editor.RemoveNextWord();
			Assert.AreEqual(editor.Text.ToString(), "HellHell!");
		}

		[Test]
		public void RemoveNextOrPrevWordWithSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!\nHello world!");
			editor.MoveSelectionToPrevWord();
			editor.RemovePrevWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello ");
			editor.MoveSelectionDown(-1);
			editor.RemoveNextWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello ");
		}

		[Test]
		public void MoveCaretToStartAndEndOfLine()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!\nHello world!");
			editor.SelectAll();
			editor.MoveCaretToLineStart();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			editor.SelectAll();
			editor.MoveCaretToLineEnd();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 12);
			editor.MoveCaretToLineStart();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			editor.MoveCaretToLineEnd();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 12);
		}

		[Test]
		public void Stress()
		{
			var editor = new SelectEditor(new CaretEditor());
			var n = 10000000;
			var rand = new Random();
			for (var i = 0; i < n; i++) {
				var action = rand.Next() % 8;
				switch (action) {
					case 0:
						editor.Insert(Utils.BuildRandomString(rand.Next() % 10));
						break;
					case 1:
						editor.Insert("\n");
						break;
					case 2:
						for (var j = 0; j < 5; j++)
							editor.RemoveBackward();
						break;
					case 3:
						for (var j = 0; j < 5; j++)
							editor.RemoveForward();
						break;
					case 4:
						editor.ClearSelection();
						break;
					case 5:
						editor.RemoveSelection();
						break;
					default:
						var choose = rand.Next() % 2;
						if (choose == 1) {
							editor.MoveCaretRight(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
							editor.MoveCaretDown(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
						} else {
							editor.MoveSelectionRight(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
							editor.MoveSelectionDown(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
						}
						break;
				}
			}
		}
	}
}
