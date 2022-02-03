using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using MonogameTextEditor.TextEditor;
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
			editor.MoveSelectRight(-5);
			editor.MoveSelectRight(3);
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.EndPosition.Col, 10);
			Assert.AreEqual(editor.StartPosition.Col, 12);
		}

		[Test]
		public void SelectMultiLine()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!\nAbc\n321");
			editor.MoveCaretRight(-1);
			editor.MoveSelectDown(-2);
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.EndPosition.Line, 0);
			Assert.AreEqual(editor.EndPosition.Col, 2);
			Assert.AreEqual(editor.StartPosition.Line, 2);
			Assert.AreEqual(editor.StartPosition.Col, 2);
		}

		[Test]
		public void RemoveSelectionAfterMoveCaret()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectRight(-5);
			Assert.IsTrue(editor.HasSelection());
			editor.MoveCaretRight(1);
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void RemoveSelectionAfterRemove()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectRight(-1);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveBackward();
			Assert.IsFalse(editor.HasSelection());
			editor.MoveSelectRight(-1);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveForward();
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void InsertWithSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectRight(-3);
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
			editor.MoveSelectRight(-2);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveForward();
			Assert.AreEqual(editor.CaretEditor.Text.ToString(), "Hello worl");
			Assert.IsFalse(editor.HasSelection());

			editor.Insert("d!");
			editor.MoveSelectRight(-2);
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
			editor.MoveSelectRight(-2);
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
			editor.MoveSelectRight(-2);
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.GetSelectedText(), "d!");
			editor.MoveCaretRight(2);
			editor.Insert("\nHello\nHello");
			editor.MoveSelectDown(-2);
			Assert.AreEqual(editor.GetSelectedText(), " world!\r\nHello\r\nHello");
			editor.ClearSelection();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world!\n");
		}

		[Test]
		public void MoveSelectToWord()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveSelectToPrevWord();
			Assert.AreEqual(editor.EndPosition.Line, 1);
			Assert.AreEqual(editor.EndPosition.Col, 12);
			editor.MoveSelectToPrevWord();
			Assert.AreEqual(editor.EndPosition.Line, 1);
			Assert.AreEqual(editor.EndPosition.Col, 6);
			editor.MoveSelectToPrevWord();
			Assert.AreEqual(editor.EndPosition.Line, 1);
			Assert.AreEqual(editor.EndPosition.Col, 0);
			editor.MoveSelectToPrevWord();
			Assert.AreEqual(editor.EndPosition.Line, 0);
			Assert.AreEqual(editor.EndPosition.Col, 17);
			editor.MoveSelectToPrevWord();
			Assert.AreEqual(editor.EndPosition.Line, 0);
			Assert.AreEqual(editor.EndPosition.Col, 12);
			editor.MoveSelectToNextWord();
			Assert.AreEqual(editor.EndPosition.Line, 0);
			Assert.AreEqual(editor.EndPosition.Col, 17);
			editor.MoveSelectToNextWord();
			Assert.AreEqual(editor.EndPosition.Line, 1);
			Assert.AreEqual(editor.EndPosition.Col, 0);
			Assert.AreEqual(editor.StartPosition.Line, 1);
			Assert.AreEqual(editor.StartPosition.Col, 17);
		}

		[Test]
		public void SelectAll()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.SelectAll();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world Hello\r\nworld Hello world");
			editor.RemoveSelect();
			Assert.AreEqual(editor.Text.ToString(), "");
		}

		[Test]
		public void CaretPositionAfterMoveCaret()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.CaretEditor.Caret.Line = 0;
			editor.CaretEditor.Caret.Col = 0;
			editor.MoveSelectToNextWord();
			editor.MoveSelectToNextWord();
			editor.MoveSelectToNextWord();
			editor.MoveCaretRight(-1);
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			editor.MoveSelectToNextWord();
			editor.MoveSelectToNextWord();
			editor.MoveSelectToNextWord();
			editor.MoveCaretRight(1);
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 17);
			editor.MoveSelectToPrevWord();
			editor.MoveCaretDown(1);
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 17);
			editor.MoveSelectToPrevWord();
			editor.MoveCaretDown(-1);
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 12);
		}

		[Test]
		public void MoveSelectToStartAndEndOfLine()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!");
			editor.MoveSelectToStartLine();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world!");
			editor.ClearSelection();
			editor.MoveCaretToNextWord();
			editor.MoveSelectToEndLine();
			Assert.AreEqual(editor.GetSelectedText(), "world!");
			editor.MoveSelectToStartLine();
			Assert.AreEqual(editor.GetSelectedText(), "Hello ");
		}

		[Test]
		public void MoveSelectToStartAndEndOfLineWithSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveSelectToStartLine();
			editor.MoveSelectRight(-1);
			editor.MoveSelectToPrevWord();
			editor.MoveSelectToStartLine();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world Hello\r\nworld Hello world");
			Assert.AreEqual(editor.EndPosition.Line, 0);
			Assert.AreEqual(editor.EndPosition.Col, 0);
			Assert.AreEqual(editor.StartPosition.Line, 1);
			Assert.AreEqual(editor.StartPosition.Col, 17);
			editor.MoveSelectToEndLine();
			Assert.AreEqual(editor.GetSelectedText(), "\r\nworld Hello world");
			Assert.AreEqual(editor.EndPosition.Line, 0);
			Assert.AreEqual(editor.EndPosition.Col, 17);
			Assert.AreEqual(editor.StartPosition.Line, 1);
			Assert.AreEqual(editor.StartPosition.Col, 17);
		}

		[Test]
		public void MoveCaretToWordWithSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveCaretToPrevWord();
			editor.MoveSelectToPrevWord();
			editor.MoveCaretToNextWord();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 17);
			Assert.IsFalse(editor.HasSelection());
			editor.MoveSelectToPrevWord();
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
			editor.MoveSelectToStartLine();
			editor.MoveSelectRight(-4);
			var text = editor.CutSelectedText();
			Assert.AreEqual(text, "ld!\r\nHello world!");
			Assert.IsFalse(editor.HasSelection());
			Assert.AreEqual(editor.Text.ToString(), "Hello wor");
			editor.Insert(text);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\r\nHello world!");
			text = editor.CutSelectedText();
			Assert.AreEqual(text, "Hello world!");
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			editor.Insert(text);
			editor.CaretEditor.Caret.Line = 0;
			editor.CaretEditor.Caret.Col = 0;
			text = editor.CutSelectedText();
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
			editor.RemoveWordPrev();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\r\nHello !");
			editor.MoveCaretRight(-2);
			editor.RemoveWordNext();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\r\nHell!");
			editor.MoveCaretDown(-1);
			editor.RemoveWordNext();
			editor.RemoveWordNext();
			editor.RemoveWordNext();
			Assert.AreEqual(editor.Text.ToString(), "HellHell!");
		}

		[Test]
		public void RemoveNextOrPrevWordWithSelection()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!\nHello world!");
			editor.MoveSelectToPrevWord();
			editor.RemoveWordPrev();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\r\nHello ");
			editor.MoveSelectDown(-1);
			editor.RemoveWordNext();
			Assert.AreEqual(editor.Text.ToString(), "Hello ");
		}

		[Test]
		public void MoveCaretToStartAndEndOfLine()
		{
			var editor = new SelectEditor(new CaretEditor());
			editor.Insert("Hello world!\nHello world!");
			editor.SelectAll();
			editor.MoveCaretToStartOfLine();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 0);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			editor.SelectAll();
			editor.MoveCaretToEndOfLine();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 12);
			editor.MoveCaretToStartOfLine();
			Assert.AreEqual(editor.CaretEditor.Caret.Line, 1);
			Assert.AreEqual(editor.CaretEditor.Caret.Col, 0);
			editor.MoveCaretToEndOfLine();
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
						editor.RemoveSelect();
						break;
					default:
						var choose = rand.Next() % 2;
						if (choose == 1) {
							editor.MoveCaretRight(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
							editor.MoveCaretDown(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
						} else {
							editor.MoveSelectRight(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
							editor.MoveSelectDown(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
						}
						break;
				}
			}
		}
	}
}
