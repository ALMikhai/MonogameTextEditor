using System;
using System.Text;
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

        [Test]
        public void Stress() {
            var editor = new CaretEditor();
            var n = 100000000;
            var rand = new Random();

            for (var i = 0; i < n; i++) {
                var action = rand.Next() % 8;
                switch (action) {
                    case 0:
                        editor.Insert(BuildRandomString(rand.Next() % 10));
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
                    default:
                        editor.MoveCaretRight(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
                        editor.MoveCaretDown(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
                        break;
                }
            }
        }

        private string BuildRandomString(int length) {
            var strBuild = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                var flt = random.NextDouble();
                var shift = Convert.ToInt32(Math.Floor(25 * flt));
                strBuild.Append(Convert.ToChar(shift + 65));
            }

            return strBuild.ToString();
        }
    }
}
