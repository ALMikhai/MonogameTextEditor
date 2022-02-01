using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTextEditor.TextEditor
{
	public class SelectTextPresenter : TextPresenter
	{
		private readonly ISelectEditor editor;
		private readonly SpriteFont font;
		private readonly Texture2D lineTexture;

		public SelectTextPresenter(ISelectEditor editor, SpriteFont font, GraphicsDevice graphicsDevice) : base(
			editor.CaretEditor, font, graphicsDevice)
		{
			this.editor = editor;
			this.font = font;
			lineTexture = new Texture2D(graphicsDevice, 1, 1);
			lineTexture.SetData(new[] { Color.White });
		}

		public override void Draw(SpriteBatch sb)
		{
			base.Draw(sb);
			DrawSelection(sb);
		}

		private void DrawSelection(SpriteBatch sb)
		{
			if (!editor.HasSelection())
				return;
			var firstCaret = editor.StartPosition;
			var secondCaret = editor.EndPosition;
			if (firstCaret > secondCaret)
				(firstCaret, secondCaret) = (secondCaret, firstCaret);
			var caretHeight = font.MeasureString("!").Y;
			if (firstCaret.Line != secondCaret.Line)
				for (var i = firstCaret.Line; i <= secondCaret.Line; i++) {
					var isFirstLine = i == firstCaret.Line;
					var isLastLine = i == secondCaret.Line;
					var line = editor.Text[i];
					if (isFirstLine) {
						var leftUp = font.MeasureString(line.Substring(0, firstCaret.Col)) + Vector2.UnitX;
						leftUp.Y = caretHeight * i;
						var leftDown = leftUp;
						leftDown.Y += caretHeight;
						var rightUp = font.MeasureString(line) + Vector2.UnitX;
						DrawRect(sb, leftUp, leftDown, (int)(rightUp.X - leftUp.X));
					} else if (isLastLine) {
						var leftUp = Vector2.UnitX;
						leftUp.Y = caretHeight * i;
						var leftDown = leftUp;
						leftDown.Y += caretHeight;
						var rightUp = font.MeasureString(line.Substring(0, secondCaret.Col)) + Vector2.UnitX;
						DrawRect(sb, leftUp, leftDown, (int)(rightUp.X - leftUp.X));
					} else {
						var leftUp = Vector2.UnitX;
						leftUp.Y = caretHeight * i;
						var leftDown = leftUp;
						leftDown.Y += caretHeight;
						var rightUp = font.MeasureString(line) + Vector2.UnitX;
						DrawRect(sb, leftUp, leftDown, (int)(rightUp.X - leftUp.X));
					}
				}
			else {
				var firstLine = editor.Text[firstCaret.Line];
				var leftUp = font.MeasureString(firstLine.Substring(0, firstCaret.Col)) + Vector2.UnitX;
				leftUp.Y = caretHeight * firstCaret.Line;
				var leftDown = leftUp;
				leftDown.Y += caretHeight;
				var rightUp = font.MeasureString(firstLine.Substring(0, secondCaret.Col)) + Vector2.UnitX;
				DrawRect(sb, leftUp, leftDown, (int)(rightUp.X - leftUp.X));
			}
		}

		private void DrawRect(SpriteBatch sb, Vector2 start, Vector2 end, int lenght)
		{
			var edge = end - start;
			sb.Draw(lineTexture,
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
