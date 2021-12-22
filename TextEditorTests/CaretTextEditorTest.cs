using MonogameTextEditor.TextEditor;
using NUnit.Framework;

namespace TextEditorTests {
    [TestFixture]
    public class CaretTextEditorTest {
        [Test]
        public void Create() {
            var editor = new CaretEditor();
            Assert.AreEqual(editor.TextContainer.ToString(), "");
            Assert.AreEqual(editor.Caret.Line, 0);
            Assert.AreEqual(editor.Caret.Col, 0);
        }

        [Test]
        public void InsertLine() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!");
            Assert.AreEqual(editor.Caret.Line, 0);
            Assert.AreEqual(editor.Caret.Col, 12);
        }

        [Test]
        public void InsertTwoLine() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.Insert("\n");
            editor.Insert("123");
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!\r\n123");
            Assert.AreEqual(editor.Caret.Line, 1);
            Assert.AreEqual(editor.Caret.Col, 3);
        }

        [Test]
        public void MoveCaretLeft() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.MoveCaretRight(-2);
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!");
            Assert.AreEqual(editor.Caret.Line, 0);
            Assert.AreEqual(editor.Caret.Col, 10);
        }

        [Test]
        public void MoveCaretRight() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.MoveCaretRight(-5);
            editor.MoveCaretRight(4);
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!");
            Assert.AreEqual(editor.Caret.Line, 0);
            Assert.AreEqual(editor.Caret.Col, 11);
        }

        [Test]
        public void MoveCaretUp() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.Insert("\n");
            editor.Insert("123");
            editor.Insert("\n");
            editor.Insert("321");
            editor.MoveCaretDown(-2);
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!\r\n123\r\n321");
            Assert.AreEqual(editor.Caret.Line, 0);
            Assert.AreEqual(editor.Caret.Col, 3);
        }

        [Test]
        public void MoveCaretDown() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.Insert("\n");
            editor.Insert("123");
            editor.Insert("\n");
            editor.Insert("321");
            editor.MoveCaretDown(-2);
            editor.MoveCaretDown(1);
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!\r\n123\r\n321");
            Assert.AreEqual(editor.Caret.Line, 1);
            Assert.AreEqual(editor.Caret.Col, 3);
        }

        [Test]
        public void MoveCaretRightAtEndOfLine() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.Insert("\n");
            editor.Insert("123");
            editor.Insert("\n");
            editor.Insert("321");
            editor.MoveCaretDown(-1);
            editor.MoveCaretRight(1);
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!\r\n123\r\n321");
            Assert.AreEqual(editor.Caret.Line, 2);
            Assert.AreEqual(editor.Caret.Col, 0);
        }

        [Test]
        public void MoveCaretLeftAtStartOfLine() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.Insert("\n");
            editor.Insert("123");
            editor.Insert("\n");
            editor.Insert("321");
            editor.MoveCaretRight(-4);
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!\r\n123\r\n321");
            Assert.AreEqual(editor.Caret.Line, 1);
            Assert.AreEqual(editor.Caret.Col, 3);
        }

        [Test]
        public void RemoveBackward() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.RemoveBackward();
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world");
            Assert.AreEqual(editor.Caret.Line, 0);
            Assert.AreEqual(editor.Caret.Col, 11);
        }

        [Test]
        public void RemoveForward() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.MoveCaretRight(-3);
            editor.RemoveForward();
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello word!");
            Assert.AreEqual(editor.Caret.Line, 0);
            Assert.AreEqual(editor.Caret.Col, 9);
        }

        [Test]
        public void RemoveBackwardAtStartOfLine() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.Insert("\n");
            editor.Insert("123");
            editor.Insert("\n");
            editor.Insert("321");
            editor.MoveCaretRight(-3);
            editor.RemoveBackward();
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!\r\n123321");
            Assert.AreEqual(editor.Caret.Line, 1);
            Assert.AreEqual(editor.Caret.Col, 3);
        }

        [Test]
        public void RemoveForwardAtEndOfLine() {
            var editor = new CaretEditor();
            editor.Insert("Hello world!");
            editor.Insert("\n");
            editor.Insert("123");
            editor.Insert("\n");
            editor.Insert("321");
            editor.MoveCaretDown(-1);
            editor.RemoveForward();
            Assert.AreEqual(editor.TextContainer.ToString(), "Hello world!\r\n123321");
            Assert.AreEqual(editor.Caret.Line, 1);
            Assert.AreEqual(editor.Caret.Col, 3);
        }
    }
}
