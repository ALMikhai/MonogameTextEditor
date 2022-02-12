using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TextEditor.CaretEditor;

namespace MonogameTextEditor.TextEditor
{
	public class TextPresenter
	{
		private readonly ICaretEditor editor;
		private readonly SpriteFont font;
		private readonly Texture2D lineTexture;

		public TextPresenter(ICaretEditor editor, SpriteFont font, GraphicsDevice graphicsDevice)
		{
			this.editor = editor;
			this.font = font;
			lineTexture = new Texture2D(graphicsDevice, 1, 1);
			lineTexture.SetData(new[] { Color.White });
		}

		public virtual void Draw(SpriteBatch sb)
		{
			DrawText(sb);
			DrawCaret(sb);
		}

		private void DrawText(SpriteBatch sb)
		{
			sb.DrawString(font, editor.Text.ToString(), Vector2.Zero, Color.Black, 0, Vector2.Zero, 1.0f,
				SpriteEffects.None, 0.5f);
		}

		private void DrawCaret(SpriteBatch sb)
		{
			var caretHeight = font.MeasureString("!").Y;
			var line = editor.GetCurrentLine();
			var startPosition = font.MeasureString(line.Substring(0, editor.Caret.Col)) + Vector2.UnitX;
			startPosition.Y = caretHeight * editor.Caret.Line;
			var endPosition = startPosition;
			endPosition.Y += caretHeight;
			DrawLine(sb, startPosition, endPosition);
		}

		private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
		{
			var edge = end - start;
			sb.Draw(lineTexture,
				new Rectangle((int)start.X, (int)start.Y, 1, (int)edge.Length()),
				null,
				Color.Red,
				0,
				Vector2.Zero,
				SpriteEffects.None,
				0);
		}
	}
}
