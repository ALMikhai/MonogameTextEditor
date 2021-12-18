using MonogameTextEditor.TextEditor;
using NUnit.Framework;

namespace TextEditorTests
{
    [TestFixture]
    public class ArrayStringTextTest
    {
        [Test]
        public void Create()
        {
            var text = new ArrayStringText();
            Assert.AreEqual(text.Text, "");
            Assert.AreEqual(text.Caret.Col, 0);
            Assert.AreEqual(text.Caret.Line, 0);
        }

        [Test]
        public void Insert1()
        {
            var text = new ArrayStringText();
            text.InsertAtCaretPosition("Hello world!");
            Assert.AreEqual(text.Text, "Hello world!");
            Assert.AreEqual(text.Caret.Col, 12);
            Assert.AreEqual(text.Caret.Line, 0);
        }

        [Test]
        public void Insert2()
        {
            var text = new ArrayStringText();
            text.InsertAtCaretPosition("Hello world!");
            text.InsertAtCaretPosition("\n");
            text.InsertAtCaretPosition("123");
            Assert.AreEqual(text.Text, "Hello world!\r\n123");
            Assert.AreEqual(text.Caret.Col, 3);
            Assert.AreEqual(text.Caret.Line, 1);
        }
    }
}
