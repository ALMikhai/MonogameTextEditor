using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTextEditor.TextEditor
{
    public class TextPresenter
    {
        private readonly ICaretEditor _editor;
        private readonly SpriteFont _font;
        private Texture2D _lineTexture;

        public TextPresenter(ICaretEditor editor, SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _editor = editor;
            _font = font;

            _lineTexture = new Texture2D(graphicsDevice, 1, 1);
            _lineTexture.SetData(new[] { Color.White });
        }

        public void Draw(SpriteBatch sb)
        {
            DrawText(sb);
            DrawCaret(sb);
        }

        private void DrawText(SpriteBatch sb)
        {
            sb.DrawString(_font, _editor.TextContainer.ToString(), Vector2.Zero, Color.Black, 0, Vector2.Zero, 1.0f,
                SpriteEffects.None, 0.5f);
        }

        private void DrawCaret(SpriteBatch sb)
        {
            var caretHeight = _font.MeasureString("!").Y;
            var line = _editor.GetCurrentLine();
            var startPosition = _font.MeasureString(line.Substring(0, _editor.Caret.Col)) + Vector2.UnitX;
            startPosition.Y = caretHeight * _editor.Caret.Line;
            var endPosition = startPosition;
            endPosition.Y += caretHeight;
            DrawLine(sb, startPosition, endPosition);
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            sb.Draw(_lineTexture,
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
