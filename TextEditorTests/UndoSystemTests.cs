using System;
using MonogameTextEditor.TextEditor;
using MonogameTextEditor.TextEditor.TextCollection;
using MonogameTextEditor.TextEditor.UndoSystem;
using NUnit.Framework;

namespace TextEditorTests
{
	[TestFixture]
	public class UndoSystemTests
	{
		private class EmptyUndoItem : IUndoItem
		{
			public UndoItemPool PoolCreator { get; set; }

			public void Do()
			{
			}

			public void Undo()
			{
			}

			public void Release()
			{
				PoolCreator.Release(this);
			}
		}

		[Test]
		public void InsertWithoutSelection()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.Insert("\n");
			editor.Insert("ab");
			editor.Insert("abc");
			editor.MoveCaretToLineStart();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nababc");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nab");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 2);
			editor.Undo();
			editor.Undo();
			editor.MoveCaretToLineStart();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.Redo();
			editor.Redo();
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nababc");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 5);
		}

		[Test]
		public void InsertWithSelection()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveSelectionToPrevWord();
			editor.MoveSelectionToPrevWord();
			editor.Insert("asd\nqwe");
			Assert.AreEqual(editor.Text.ToString(), "Hello asd\nqwe");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 3);
			Assert.IsFalse(editor.HasSelection());
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 6);
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 6);
			Assert.AreEqual(editor.SelectionStart.Line, 0);
			Assert.AreEqual(editor.SelectionStart.Col, 12);
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			Assert.IsFalse(editor.HasSelection());
			editor.Undo();
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			Assert.IsFalse(editor.HasSelection());
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello asd\nqwe");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 3);
			Assert.IsFalse(editor.HasSelection());
			editor.MoveCaretToLineStart();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 6);
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 6);
			Assert.AreEqual(editor.SelectionStart.Line, 0);
			Assert.AreEqual(editor.SelectionStart.Col, 12);
		}

		[Test]
		public void RemoveSelection()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.MoveSelectionToPrevWord();
			editor.MoveSelectionToPrevWord();
			editor.RemoveSelection();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello ");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			Assert.IsFalse(editor.HasSelection());
			editor.SelectAll();
			editor.RemoveSelection();
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			Assert.IsFalse(editor.HasSelection());
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello ");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			Assert.AreEqual(editor.SelectionEnd.Line, 1);
			Assert.AreEqual(editor.SelectionEnd.Col, 6);
			Assert.AreEqual(editor.SelectionStart.Line, 0);
			Assert.AreEqual(editor.SelectionStart.Col, 0);
			Assert.IsTrue(editor.HasSelection());
			editor.MoveCaretRight(1);
			editor.MoveSelectionRight(-2);
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world!");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			Assert.AreEqual(editor.SelectionEnd.Line, 1);
			Assert.AreEqual(editor.SelectionEnd.Col, 6);
			Assert.AreEqual(editor.SelectionStart.Line, 1);
			Assert.AreEqual(editor.SelectionStart.Col, 12);
			editor.SelectAll();
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello ");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			Assert.IsFalse(editor.HasSelection());
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void RemoveForwardAndBackward()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.RemoveBackward();
			editor.RemoveBackward();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello worl");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 10);
			editor.MoveCaretToLineStart();
			editor.MoveCaretRight(-1);
			editor.RemoveForward();
			editor.RemoveForward();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!ello worl");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.SelectAll();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!Hello worl");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			Assert.IsFalse(editor.HasSelection());
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello worl");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.SelectAll();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 11);
			Assert.IsFalse(editor.HasSelection());
			editor.RemoveForward();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world!");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.Redo();
			editor.SelectAll();
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello worl");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 10);
			editor.Redo();
			editor.SelectAll();
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!ello worl");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
		}

		[Test]
		public void RemoveForwardAndBackwardEmpty()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("a");
			editor.RemoveBackward();
			editor.RemoveBackward();
			editor.RemoveBackward();
			editor.RemoveBackward();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "a");
			editor.MoveCaretRight(-1);
			editor.RemoveForward();
			editor.RemoveForward();
			editor.RemoveForward();
			editor.RemoveForward();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "a");
		}

		[Test]
		public void CutWithoutSelection()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.MoveCaretToPrevWord();
			editor.MoveCaretToPrevWord();
			string text = editor.Cut();
			Assert.AreEqual(text, "Hello world!");
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.SelectAll();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world!");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			Assert.IsFalse(editor.HasSelection());
			editor.SelectAll();
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 0);
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void CutEmpty()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.Cut();
			editor.Cut();
			editor.Cut();
			editor.Cut();
			editor.Cut();
			editor.Cut();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
		}

		[Test]
		public void RemoveWord()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.MoveCaretRight(-1);
			editor.RemovePrevWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello !");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			editor.MoveCaretRight(-4);
			editor.RemoveNextWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHe!");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 2);
			editor.SelectAll();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello !");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 2);
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world!");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 11);
			editor.MoveCaretRight(1);
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello !");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			editor.SelectAll();
			editor.Redo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHe!");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 2);
		}

		[Test]
		public void RemoveWordEmpty()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveCaretToPrevWord();
			editor.MoveCaretToPrevWord();
			editor.RemoveNextWord();
			editor.RemoveNextWord();
			editor.RemoveNextWord();
			editor.RemoveNextWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello ");
			editor.Undo();
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			editor.RemovePrevWord();
			editor.RemovePrevWord();
			editor.RemovePrevWord();
			editor.RemovePrevWord();
			Assert.AreEqual(editor.Text.ToString(), "world!");
			editor.Undo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
		}

		[Test]
		public void HardUndo()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.RemovePrevWord();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world");
			Assert.IsFalse(editor.UndoStack.CanRedo);
			editor.HardUndo();
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\nHello world!");
			Assert.IsFalse(editor.UndoStack.CanRedo);
		}

		[Test]
		public void StackDepth()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.RemovePrevWord();
			Assert.AreEqual(editor.UndoStack.Depth, 2);
			editor.Undo();
			Assert.AreEqual(editor.UndoStack.Depth, 1);
			editor.Redo();
			Assert.AreEqual(editor.UndoStack.Depth, 2);
		}

		[Test]
		public void UndoStackDepth()
		{
			var stack = new UndoStack(10);
			var undoItemPool = new UndoItemPool();
			int n = 10000000;
			var rand = new Random();
			for (int i = 0; i < n; i++) {
				int action = rand.Next() % 4;
				switch (action) {
					case 0:
						stack.Add(undoItemPool.Acquire<EmptyUndoItem>());
						break;
					case 1:
						stack.Do(undoItemPool.Acquire<EmptyUndoItem>());
						break;
					case 2:
						stack.Undo();
						break;
					case 3:
						stack.Redo();
						break;
				}
				if (stack.Depth > stack.MaxDepth) {
					throw new Exception();
				}
			}
		}

		[Test]
		public void Stress()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			int n = 10000000;
			var rand = new Random();
			for (int i = 0; i < n; i++) {
				int action = rand.Next() % 11;
				switch (action) {
					case 0:
						editor.Insert(Utils.BuildRandomString(rand.Next() % 10));
						break;
					case 1:
						editor.Insert("\n");
						break;
					case 2:
						for (int j = 0; j < 5; j++) {
							editor.RemoveBackward();
						}
						break;
					case 3:
						for (int j = 0; j < 5; j++) {
							editor.RemoveForward();
						}
						break;
					case 4:
						editor.ClearSelection();
						break;
					case 5:
						editor.RemoveSelection();
						break;
					case 6:
						editor.Undo();
						break;
					case 7:
						editor.Redo();
						break;
					default:
						int choose = rand.Next() % 2;
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
