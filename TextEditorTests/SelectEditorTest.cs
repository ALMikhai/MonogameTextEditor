using System;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;
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

        [Test]
        public void Stress() {
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
                        }
                        else {
                            editor.MoveSelectRight(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
                            editor.MoveSelectDown(rand.Next() % 7 * (rand.Next() % 2 == 1 ? -1 : 1));
                        }
                        break;
                }
            }
        }
    }
}
