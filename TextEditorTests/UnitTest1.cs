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
            var text = new CaretEditor();
            Assert.AreEqual(text.TextContainer.ToString(), "");
            Assert.AreEqual(text.Caret.Col, 0);
            Assert.AreEqual(text.Caret.Line, 0);
        }

        [Test]
        public void Insert1()
        {
            var text = new CaretEditor();
            text.Insert("Hello world!");
            Assert.AreEqual(text.TextContainer.ToString(), "Hello world!");
            Assert.AreEqual(text.Caret.Col, 12);
            Assert.AreEqual(text.Caret.Line, 0);
        }

        [Test]
        public void Insert2()
        {
            var text = new CaretEditor();
            text.Insert("Hello world!");
            text.Insert("\n");
            text.Insert("123");
            Assert.AreEqual(text.TextContainer.ToString(), "Hello world!\r\n123");
            Assert.AreEqual(text.Caret.Col, 3);
            Assert.AreEqual(text.Caret.Line, 1);
        }
    }
}
