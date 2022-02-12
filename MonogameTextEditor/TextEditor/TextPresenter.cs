using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TextEditor.Caret;
using TextEditor.CaretEditor;

namespace MonogameTextEditor.TextEditor
{
	public class TextPresenter
	{
		public SpriteFont Font { get; set; }

		public GraphicsDevice GraphicsDevice
		{
			get => graphicsDevice;
			set
			{
				graphicsDevice = value;
				LineTexture = new Texture2D(graphicsDevice, 1, 1);
				LineTexture.SetData(new[] { Color.White });
			}
		}

		public Texture2D LineTexture { get; set; }

		private readonly ICaretEditor editor;
		private GraphicsDevice graphicsDevice;

		public TextPresenter(ICaretEditor editor, SpriteFont font = null, GraphicsDevice graphicsDevice = null)
		{
			this.editor = editor;
		}

		public virtual void Draw(SpriteBatch sb)
		{
			DrawText(sb);
			DrawCaret(sb, editor.Caret);
		}

		private void DrawText(SpriteBatch sb)
		{
			sb.DrawString(Font, editor.Text.ToString(), Vector2.Zero, Color.Black, 0, Vector2.Zero, 1.0f,
				SpriteEffects.None, 0.5f);
		}

		protected void DrawCaret(SpriteBatch sb, ICaret caret)
		{
			var caretHeight = Font.MeasureString("!").Y;
			var line = editor.Text[caret.Line];
			var startPosition = Font.MeasureString(line.Substring(0, caret.Col)) + Vector2.UnitX;
			startPosition.Y = caretHeight * caret.Line;
			var endPosition = startPosition;
			endPosition.Y += caretHeight;
			DrawLine(sb, startPosition, endPosition);
		}

		private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
		{
			var edge = end - start;
			sb.Draw(LineTexture,
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
