using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using TextEditor.Caret;
using TextEditor.SelectEditor;

namespace MonogameTextEditor.TextEditor
{
	public class SelectTextPresenter : TextPresenter
	{
		private readonly ISelectEditor editor;

		public SelectTextPresenter(ISelectEditor editor, SpriteFont font = null, GraphicsDevice graphicsDevice = null) : base(
			editor.CaretEditor, font, graphicsDevice)
		{
			this.editor = editor;
		}

		public override void Draw(SpriteBatch sb)
		{
			base.Draw(sb);
			DrawSelection(sb, editor.SelectionStart, editor.SelectionEnd);
		}

		protected void DrawSelection(SpriteBatch sb, ICaret firstCaret, ICaret secondCaret)
		{
			if (firstCaret.Equals(secondCaret))
				return;
			if (firstCaret.CompareTo(secondCaret) > 0)
				(firstCaret, secondCaret) = (secondCaret, firstCaret);
			var caretHeight = Font.MeasureString("!").Y;
			if (firstCaret.Line != secondCaret.Line)
				for (var i = firstCaret.Line; i <= secondCaret.Line; i++) {
					var isFirstLine = i == firstCaret.Line;
					var isLastLine = i == secondCaret.Line;
					var line = editor.Text[i];
					if (isFirstLine) {
						var leftUp = Font.MeasureString(line.Substring(0, firstCaret.Col)) + Vector2.UnitX;
						leftUp.Y = caretHeight * i;
						var leftDown = leftUp;
						leftDown.Y += caretHeight;
						var rightUp = Font.MeasureString(line) + Vector2.UnitX;
						DrawRect(sb, leftUp, leftDown, (int)(rightUp.X - leftUp.X));
					} else if (isLastLine) {
						var leftUp = Vector2.UnitX;
						leftUp.Y = caretHeight * i;
						var leftDown = leftUp;
						leftDown.Y += caretHeight;
						var rightUp = Font.MeasureString(line.Substring(0, secondCaret.Col)) + Vector2.UnitX;
						DrawRect(sb, leftUp, leftDown, (int)(rightUp.X - leftUp.X));
					} else {
						var leftUp = Vector2.UnitX;
						leftUp.Y = caretHeight * i;
						var leftDown = leftUp;
						leftDown.Y += caretHeight;
						var rightUp = Font.MeasureString(line) + Vector2.UnitX;
						DrawRect(sb, leftUp, leftDown, (int)(rightUp.X - leftUp.X));
					}
				}
			else {
				var firstLine = editor.Text[firstCaret.Line];
				var leftUp = Font.MeasureString(firstLine.Substring(0, firstCaret.Col)) + Vector2.UnitX;
				leftUp.Y = caretHeight * firstCaret.Line;
				var leftDown = leftUp;
				leftDown.Y += caretHeight;
				var rightUp = Font.MeasureString(firstLine.Substring(0, secondCaret.Col)) + Vector2.UnitX;
				DrawRect(sb, leftUp, leftDown, (int)(rightUp.X - leftUp.X));
			}
		}

		private void DrawRect(SpriteBatch sb, Vector2 start, Vector2 end, int lenght)
		{
			var edge = end - start;
			sb.Draw(LineTexture,
				new Rectangle((int)start.X, (int)start.Y, lenght, (int)edge.Length()),
				null,
				new Color(Color.Blue, 0.3f),
				0,
				Vector2.Zero,
				SpriteEffects.None,
				0);
		}
	}
}
