using System;
using MonogameTextEditor.TextEditor;
using MonogameTextEditor.TextEditor.TextCollection;
using NUnit.Framework;

namespace TextEditorTests
{
	[TestFixture]
	public class EditorDecoratorTests
	{
		[Test]
		public void InsertEmptyLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert(string.Empty);
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
			Assert.AreEqual(editor.UndoStack.Depth, 0);
		}

		[Test]
		public void InsertLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
		}

		[Test]
		public void InsertTwoLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.Insert("\n");
			editor.Insert("123");
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n123");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 3);
		}

		[Test]
		public void RemoveBackward()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.RemoveBackward();
			Assert.AreEqual(editor.Text.ToString(), "Hello world");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 11);
		}

		[Test]
		public void RemoveForward()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveCaretRight(-3);
			editor.RemoveForward();
			Assert.AreEqual(editor.Text.ToString(), "Hello word!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 9);
		}

		[Test]
		public void RemoveSelectedText()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.RemoveSelection();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			editor.MoveSelectionToPrevWord();
			editor.RemoveSelection();
			Assert.AreEqual(editor.Text.ToString(), "Hello world");
			editor.Undo();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
		}

		[Test]
		public void RemoveBackwardAtStartOfLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.Insert("\n");
			editor.Insert("123");
			editor.Insert("\n");
			editor.Insert("321");
			editor.MoveCaretRight(-3);
			editor.RemoveBackward();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n123321");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 3);
		}

		[Test]
		public void RemoveForwardAtEndOfLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.Insert("\n");
			editor.Insert("123");
			editor.Insert("\n");
			editor.Insert("321");
			editor.MoveCaretDown(-1);
			editor.RemoveForward();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n123321");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 3);
		}

		[Test]
		public void InsertWithoutSelection()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void RemoveSelectionAfterRemove()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
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
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-3);
			Assert.IsTrue(editor.HasSelection());
			editor.Insert(" 123");
			Assert.AreEqual(editor.Text.ToString(), "Hello wor 123");
			Assert.IsFalse(editor.HasSelection());
			Assert.AreEqual(editor.Caret.Col, 13);
		}

		[Test]
		public void RemoveSelection()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-2);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveForward();
			Assert.AreEqual(editor.Text.ToString(), "Hello worl");
			Assert.IsFalse(editor.HasSelection());
			editor.Insert("d!");
			editor.MoveSelectionRight(-2);
			Assert.IsTrue(editor.HasSelection());
			editor.RemoveBackward();
			Assert.AreEqual(editor.Text.ToString(), "Hello worl");
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void Cut()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.MoveSelectionToLineStart();
			editor.MoveSelectionRight(-4);
			string text = editor.Cut();
			Assert.AreEqual(text, "ld!\nHello world!");
			Assert.IsFalse(editor.HasSelection());
			Assert.AreEqual(editor.Text.ToString(), "Hello wor");
			editor.Insert(text);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world!");
			text = editor.Cut();
			Assert.AreEqual(text, "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.Insert(text);
			editor.Caret.Line = 0;
			editor.Caret.Col = 0;
			text = editor.Cut();
			Assert.AreEqual(text, "Hello world!\n");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			Assert.AreEqual(editor.Text.Count, 1);
		}

		[Test]
		public void RemoveNextOrPrevWord()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
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
			editor.RemoveNextWord();
			Assert.AreEqual(editor.Text.ToString(), "HellHell!");
		}

		[Test]
		public void RemoveNextOrPrevWordWithSelection()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.MoveSelectionToPrevWord();
			editor.MoveSelectionToPrevWord();
			editor.RemovePrevWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello ");
			editor.MoveSelectionDown(-1);
			editor.RemoveNextWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello ");
		}

		[Test]
		public void Stress()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			int n = 100000000;
			var rand = new Random();
			for (int i = 0; i < n; i++) {
				int action = rand.Next() % 8;
				switch (action) {
					case 0:
						editor.MoveCaretRight(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
						break;
					case 1:
						editor.MoveCaretDown(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
						break;
					case 2:
						editor.MoveCaretToNextWord();
						break;
					case 3:
						editor.MoveCaretToPrevWord();
						break;
					case 4:
						editor.MoveCaretToLineEnd();
						break;
					case 5:
						editor.MoveCaretToLineStart();
						break;
					case 6:
						editor.MoveSelectionRight(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
						break;
					case 7:
						editor.MoveSelectionDown(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
						break;
					case 8:
						editor.ClearSelection();
						break;
					case 9:
						editor.MoveSelectionToNextWord();
						break;
					case 10:
						editor.MoveSelectionToPrevWord();
						break;
					case 11:
						editor.SelectWordAtCaretPos();
						break;
					case 12:
						editor.SelectAll();
						break;
					case 13:
						editor.MoveSelectionToLineEnd();
						break;
					case 14:
						editor.MoveSelectionToLineStart();
						break;
					case 15:
						editor.Insert(Utils.BuildRandomString(rand.Next() % 10));
						break;
					case 16:
						editor.Insert("\n");
						break;
					case 17:
						for (int j = 0; j < 5; j++) {
							editor.RemoveBackward();
						}
						break;
					case 18:
						for (int j = 0; j < 5; j++) {
							editor.RemoveForward();
						}
						break;
					case 19:
						editor.RemoveSelection();
						break;
					case 20:
						editor.RemoveNextWord();
						break;
					case 21:
						editor.RemovePrevWord();
						break;
					case 22:
						editor.Undo();
						break;
					case 23:
						editor.Redo();
						break;
					case 24:
						editor.MoveSelectionToLineStart();
						break;
					case 25:
						editor.Cut();
						break;
				}
			}
			while (editor.UndoStack.CanRedo) {
				editor.Redo();
			}
			while (editor.UndoStack.CanUndo) {
				editor.Undo();
			}
		}
	}
}
