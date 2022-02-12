using NUnit.Framework;
using TextEditor.TextCollection;

namespace TextEditorTests
{
	[TestFixture]
	public class ArrayStringTest
	{
		[Test]
		public void Create()
		{
			var text = new ArrayString();
			Assert.AreEqual(text.ToString(), "");
		}

		[Test]
		public void InsertLine()
		{
			var text = new ArrayString();
			text.Insert(0, 0, "Hello world!");
			Assert.AreEqual(text.ToString(), "Hello world!");
		}

		[Test]
		public void InsertTwoLine()
		{
			var text = new ArrayString();
			text.Insert(0, 0, "Hello world!");
			text.InsertLine(1, "123");
			Assert.AreEqual(text.ToString(), "Hello world!\n123");
		}

		[Test]
		public void RemoveSymbol()
		{
			var text = new ArrayString();
			text.Insert(0, 0, "Hello world!");
			text.Remove(0, 2, 3);
			Assert.AreEqual(text.ToString(), "He world!");
		}

		[Test]
		public void RemoveLine()
		{
			var text = new ArrayString();
			text.Insert(0, 0, "Hello world!");
			text.InsertLine(1, "123");
			text.InsertLine(1, "321");
			text.RemoveLine(0);
			Assert.AreEqual(text.ToString(), "321\n123");
		}
	}
}
