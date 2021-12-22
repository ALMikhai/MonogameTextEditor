using MonogameTextEditor.TextEditor;
using NUnit.Framework;

namespace TextEditorTests {
    [TestFixture]
    public class SelectEditorTest {
        [Test]
        public void InsertWithoutSelection() {
            var editor = new SelectEditor(new CaretEditor());
            editor.Insert("Hello world!");
            Assert.AreEqual(editor.CaretEditor.TextContainer.ToString(), "Hello world!");
            Assert.IsFalse(editor.HasSelection());
        }

        [Test]
        public void SelectAtLine() {
            var editor = new SelectEditor(new CaretEditor());
            editor.Insert("Hello world!");
            editor.MoveSelectRight(-5);
            editor.MoveSelectRight(3);
            Assert.IsTrue(editor.HasSelection());
            Assert.AreEqual(editor.EndPosition.Col, 10);
            Assert.AreEqual(editor.StartPosition.Col, 12);
        }

        [Test]
        public void SelectMultiLine() {
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
        public void RemoveSelectionAfterMoveCaret() {
            var editor = new SelectEditor(new CaretEditor());
            editor.Insert("Hello world!");
            editor.MoveSelectRight(-5);
            Assert.IsTrue(editor.HasSelection());
            editor.MoveCaretRight(1);
            Assert.IsFalse(editor.HasSelection());
        }

        [Test]
        public void RemoveSelectionAfterRemove() {
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
        public void InsertWithSelection() {
            var editor = new SelectEditor(new CaretEditor());
            editor.Insert("Hello world!");
            editor.MoveSelectRight(-3);
            Assert.IsTrue(editor.HasSelection());
            editor.Insert(" 123");
            Assert.AreEqual(editor.CaretEditor.TextContainer.ToString(), "Hello wor 123");
            Assert.IsFalse(editor.HasSelection());
            Assert.AreEqual(editor.CaretEditor.Caret.Col, 13);
        }

        [Test]
        public void RemoveSelection() {
            var editor = new SelectEditor(new CaretEditor());
            editor.Insert("Hello world!");
            editor.MoveSelectRight(-2);
            Assert.IsTrue(editor.HasSelection());
            editor.RemoveForward();
            Assert.AreEqual(editor.CaretEditor.TextContainer.ToString(), "Hello worl");
            Assert.IsFalse(editor.HasSelection());

            editor.Insert("d!");
            editor.MoveSelectRight(-2);
            Assert.IsTrue(editor.HasSelection());
            editor.RemoveBackward();
            Assert.AreEqual(editor.CaretEditor.TextContainer.ToString(), "Hello worl");
            Assert.IsFalse(editor.HasSelection());
        }

        [Test]
        public void ClearSelection() {
            var editor = new SelectEditor(new CaretEditor());
            editor.Insert("Hello world!");
            editor.MoveSelectRight(-2);
            Assert.IsTrue(editor.HasSelection());
            editor.ClearSelection();
            Assert.IsFalse(editor.HasSelection());
            editor.Insert("123");
            Assert.AreEqual(editor.CaretEditor.TextContainer.ToString(), "Hello worl123d!");
        }
    }
}
