using System;
using MonogameTextEditor.TextEditor;
using MonogameTextEditor.TextEditor.Caret;
using MonogameTextEditor.TextEditor.TextCollection;
using NUnit.Framework;

namespace TextEditorTests
{
	[TestFixture]
	public class ReadOnlyEditorTests
	{
		[Test]
		public void Create()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
		}

		[Test]
		public void MoveCaretLeft()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveCaretRight(-2);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 10);
		}

		[Test]
		public void MoveCaretRight()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveCaretRight(-5);
			editor.MoveCaretRight(4);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 11);
		}

		[Test]
		public void MoveCaretUp()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.Insert("\n");
			editor.Insert("123");
			editor.Insert("\n");
			editor.Insert("321");
			editor.MoveCaretDown(-2);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n123\n321");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 3);
		}

		[Test]
		public void MoveCaretDown()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.Insert("\n");
			editor.Insert("123");
			editor.Insert("\n");
			editor.Insert("321");
			editor.MoveCaretDown(-2);
			editor.MoveCaretDown(1);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n123\n321");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 3);
		}

		[Test]
		public void MoveCaretToNextAndPrevLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.MoveCaretRight(-16);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 9);
			editor.MoveCaretRight(16);
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 12);
		}

		[Test]
		public void MoveCaretRightAtEndOfLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.Insert("\n");
			editor.Insert("123");
			editor.Insert("\n");
			editor.Insert("321");
			editor.MoveCaretDown(-1);
			editor.MoveCaretRight(1);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n123\n321");
			Assert.AreEqual(editor.Caret.Line, 2);
			Assert.AreEqual(editor.Caret.Col, 0);
		}

		[Test]
		public void MoveCaretLeftAtStartOfLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.Insert("\n");
			editor.Insert("123");
			editor.Insert("\n");
			editor.Insert("321");
			editor.MoveCaretRight(-4);
			Assert.AreEqual(editor.Text.ToString(), "Hello world!\n123\n321");
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 3);
		}

		[Test]
		public void MoveCaretToWord()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveCaretToPrevWord();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.MoveCaretToPrevWord();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			editor.MoveCaretToPrevWord();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.MoveCaretToPrevWord();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 17);
			editor.MoveCaretToPrevWord();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.MoveCaretToNextWord();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 17);
			editor.MoveCaretToNextWord();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 0);
		}

		[Test]
		public void MoveCaretToStartOfLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveCaretToLineStart();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.MoveCaretToNextWord();
			editor.MoveCaretToLineStart();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
		}

		[Test]
		public void MoveCaretToEndOfLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveCaretToLineStart();
			editor.MoveCaretToLineEnd();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.MoveCaretToPrevWord();
			editor.MoveCaretToLineEnd();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
		}

		[Test]
		public void GetCurrentChar()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			Assert.AreEqual(editor.GetCurrentChar(), '\0');
			editor.MoveCaretRight(-1);
			Assert.AreEqual(editor.GetCurrentChar(), '!');
		}

		[Test]
		public void Reset()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			Assert.AreEqual(editor.Text.ToString(), "Hello world!");
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.Reset();
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
		}

		[Test]
		public void SelectAtLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-5);
			editor.MoveSelectionRight(3);
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.SelectionEnd.Col, 10);
			Assert.AreEqual(editor.SelectionStart.Col, 12);
		}

		[Test]
		public void ClearSelection()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-2);
			Assert.IsTrue(editor.HasSelection());
			editor.ClearSelection();
			Assert.IsFalse(editor.HasSelection());
			editor.Insert("123");
			Assert.AreEqual(editor.Text.ToString(), "Hello worl123d!");
		}

		[Test]
		public void MoveSelectToWord()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
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
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.SelectAll();
			Assert.AreEqual(editor.GetSelectedText(), "Hello world Hello\nworld Hello world");
			editor.RemoveSelection();
			Assert.AreEqual(editor.Text.ToString(), string.Empty);
		}

		[Test]
		public void CaretPositionAfterMoveCaret()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.Caret.Line = 0;
			editor.Caret.Col = 0;
			editor.MoveSelectionToNextWord();
			editor.MoveSelectionToNextWord();
			editor.MoveSelectionToNextWord();
			editor.MoveCaretRight(-1);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.MoveSelectionToNextWord();
			editor.MoveSelectionToNextWord();
			editor.MoveSelectionToNextWord();
			editor.MoveCaretRight(1);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 17);
			editor.MoveSelectionToPrevWord();
			editor.MoveCaretDown(1);
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 17);
			editor.MoveSelectionToPrevWord();
			editor.MoveCaretDown(-1);
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 12);
		}

		[Test]
		public void MoveSelectToStartAndEndOfLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
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
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
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
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveCaretToPrevWord();
			editor.MoveSelectionToPrevWord();
			editor.MoveCaretToNextWord();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 17);
			Assert.IsFalse(editor.HasSelection());
			editor.MoveSelectionToPrevWord();
			editor.MoveCaretToPrevWord();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 6);
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void MoveCaretToTextStart()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world Hello\nworld Hello world");
			editor.MoveCaretToTextStart();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			Assert.IsFalse(editor.HasSelection());
			editor.SelectAll();
			Assert.IsTrue(editor.HasSelection());
			editor.MoveCaretToTextStart();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void MoveCaretToTextEnd()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world Hello\nworld Hello world");
			var caretPos = new Caret();
			caretPos.AssignFrom(editor.Caret);
			editor.MoveCaretToTextEnd();
			Assert.AreEqual(caretPos, editor.Caret);
			Assert.IsFalse(editor.HasSelection());
			editor.MoveCaretToTextStart();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			Assert.IsFalse(editor.HasSelection());
			editor.MoveSelectionToNextWord();
			Assert.IsTrue(editor.HasSelection());
			editor.MoveCaretToTextEnd();
			Assert.AreEqual(caretPos, editor.Caret);
		}

		[Test]
		public void SelectMultiLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
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
		public void MoveCaretToStartAndEndOfLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\nHello world!");
			editor.SelectAll();
			editor.MoveCaretToLineStart();
			Assert.AreEqual(editor.Caret.Line, 0);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.SelectAll();
			editor.MoveCaretToLineEnd();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 12);
			editor.MoveCaretToLineStart();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 0);
			editor.MoveCaretToLineEnd();
			Assert.AreEqual(editor.Caret.Line, 1);
			Assert.AreEqual(editor.Caret.Col, 12);
		}

		[Test]
		public void RemoveSelectionAfterMoveCaret()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!");
			editor.MoveSelectionRight(-5);
			Assert.IsTrue(editor.HasSelection());
			editor.MoveCaretRight(1);
			Assert.IsFalse(editor.HasSelection());
		}

		[Test]
		public void GetCurrentLine()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Insert("Hello world!\n123");
			Assert.AreEqual(editor.GetCurrentLine(), "123");
			editor.MoveCaretToLineStart();
			editor.MoveCaretRight(-1);
			Assert.AreEqual(editor.GetCurrentLine(), "Hello world!\n");
		}

		[Test]
		public void SelectWordAtCaretPos()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.SelectWordAtCaretPos();
			Assert.IsFalse(editor.HasSelection());
			editor.Insert("Hello world!\n123");
			editor.MoveSelectionRight(-1);
			editor.SelectWordAtCaretPos();
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.SelectionEnd.Line, 1);
			Assert.AreEqual(editor.SelectionEnd.Col, 3);
			Assert.AreEqual(editor.SelectionStart.Line, 1);
			Assert.AreEqual(editor.SelectionStart.Col, 0);
			editor.MoveCaretRight(-1);
			editor.MoveCaretRight(-1);
			editor.SelectWordAtCaretPos();
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 12);
			Assert.AreEqual(editor.SelectionStart.Line, 0);
			Assert.AreEqual(editor.SelectionStart.Col, 11);
			editor.MoveCaretRight(-1);
			editor.MoveCaretToPrevWord();
			editor.SelectWordAtCaretPos();
			Assert.IsTrue(editor.HasSelection());
			Assert.AreEqual(editor.SelectionEnd.Line, 0);
			Assert.AreEqual(editor.SelectionEnd.Col, 11);
			Assert.AreEqual(editor.SelectionStart.Line, 0);
			Assert.AreEqual(editor.SelectionStart.Col, 6);
		}

		[Test]
		public void Stress()
		{
			var editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
			editor.Text.Insert(
				new TextPosition(),
				"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut " +
				"labore et dolore magna aliqua. Rutrum quisque non tellus orci. Tortor pretium viverra suspendisse " +
				"potenti nullam ac. Leo a diam sollicitudin tempor id. Lobortis scelerisque fermentum dui faucibus " +
				"in ornare. Quis varius quam quisque id. Parturient montes nascetur ridiculus mus. Pharetra sit " +
				"amet aliquam id diam maecenas ultricies. Proin fermentum leo vel orci porta non. Diam ut venenatis " +
				"tellus in metus vulputate eu scelerisque. Tortor aliquam nulla facilisi cras fermentum odio eu " +
				"feugiat pretium. Quisque id diam vel quam elementum pulvinar etiam non.\nAt ultrices mi tempus " +
				"imperdiet nulla malesuada pellentesque elit. Vel orci porta non pulvinar neque laoreet " +
				"suspendisse. Vel pretium lectus quam id leo. Est ante in nibh mauris cursus mattis molestie a. " +
				"Aenean sed adipiscing diam donec. Enim ut tellus elementum sagittis vitae et. Sagittis aliquam " +
				"malesuada bibendum arcu vitae elementum curabitur vitae nunc. Pretium vulputate sapien nec " +
				"sagittis aliquam malesuada bibendum arcu. Ultrices in iaculis nunc sed augue lacus viverra. " +
				"Nec feugiat nisl pretium fusce id. Consectetur adipiscing elit ut aliquam purus sit amet. " +
				"Non blandit massa enim nec. Blandit libero volutpat sed cras ornare arcu dui. Duis tristique " +
				"sollicitudin nibh sit. Est ante in nibh mauris cursus mattis molestie. Nisl vel pretium lectus " +
				"quam id leo in vitae. Vel facilisis volutpat est velit egestas dui id."
			);
			int n = 10000000;
			var rand = new Random();
			for (int i = 0; i < n; i++) {
				int action = rand.Next() % 16;
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
					default:
						editor.MoveSelectionToLineStart();
						break;
				}
			}
			Assert.IsFalse(editor.UndoStack.CanUndo);
			Assert.AreEqual(editor.UndoStack.Depth, 0);
		}
	}
}
