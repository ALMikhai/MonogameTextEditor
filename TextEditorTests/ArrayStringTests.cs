using System;
using MonogameTextEditor.TextEditor.Caret;
using MonogameTextEditor.TextEditor.TextCollection;
using NUnit.Framework;

namespace TextEditorTests
{
	[TestFixture]
	public class ArrayStringTest
	{
		[Test]
		public void Create()
		{
			var text = new TextCollection();
			Assert.AreEqual(text.ToString(), string.Empty);
		}

		[Test]
		public void InsertLine()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!");
			Assert.AreEqual(text.ToString(), "Hello world!");
		}

		[Test]
		public void InsertTwoLine()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n123");
			Assert.AreEqual(text.ToString(), "Hello world!\n123");
		}

		[Test]
		public void RemoveSymbol()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!");
			text.Remove(new TextPosition(0, 2), 3);
			Assert.AreEqual(text.ToString(), "He world!");
		}

		[Test]
		public void RemoveLine()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			text.Remove(new TextPosition(), 13);
			Assert.AreEqual(text.ToString(), "321\n123");
		}

		[Test]
		public void InvalidInsert()
		{
			var text = new TextCollection();
			Assert.Catch<ArgumentException>(() => text.Insert(new TextPosition(1, 1), "Hello world!"));
		}

		[Test]
		public void InvalidGetText()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			Assert.Catch<IndexOutOfRangeException>(() =>
				text.GetTextRange(new TextPosition(), new TextPosition(5, 5)));
			Assert.Catch<IndexOutOfRangeException>(() =>
				text.GetTextRange(new TextPosition(5, 5), new TextPosition(5, 7)));
		}

		[Test]
		public void InvalidRemoveRange()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			Assert.Catch<IndexOutOfRangeException>(() =>
				text.RemoveRange(new TextPosition(), new TextPosition(5, 5)));
			Assert.Catch<IndexOutOfRangeException>(() =>
				text.RemoveRange(new TextPosition(5, 5), new TextPosition(5, 7)));
		}

		[Test]
		public void InvalidRemove()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			Assert.Catch<ArgumentException>(() => text.Remove(new TextPosition(5, 5), 1));
			Assert.Catch<ArgumentException>(() => text.Remove(new TextPosition(), 100));
		}

		[Test]
		public void InvalidGetLine()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			Assert.Catch<ArgumentException>(() => {
				var a = text[5].Length;
			});
		}

		[Test]
		public void InvalidGetLineLength()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			Assert.Catch<ArgumentException>(() => {
				var a = text[5].Length;
			});
		}

		[Test]
		public void GetSymbol()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			Assert.AreEqual(text.GetSymbol(new TextPosition()), 'H');
			Assert.AreEqual(text.GetSymbol(new TextPosition(0, 12)), '\n');
			Assert.AreEqual(text.GetSymbol(new TextPosition(2, 3)), '\0');
		}

		[Test]
		public void InvalidGetSymbol()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			Assert.Catch<ArgumentException>(() => text.GetSymbol(new TextPosition(5, 5)));
		}

		[Test]
		public void Clear()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n321\n123");
			text.Reset();
			Assert.AreEqual(text.ToString(), string.Empty);
		}

		[Test]
		public void GetLength()
		{
			var text = new TextCollection();
			text.Insert(new TextPosition(), "Hello world!\n123");
			Assert.AreEqual(text.GetTextLength(), 16);
			text.Reset();
			text.ToString();
			Assert.AreEqual(text.GetTextLength(), 0);
		}
	}
}
