using Microsoft.Xna.Framework.Graphics;
using TextEditor.MultiUserEditor;

namespace MonogameTextEditor.TextEditor
{
	public class MultiUserTextPresenter : SelectTextPresenter
	{
		private readonly MultiUserEditor editor;

		public MultiUserTextPresenter(MultiUserEditor editor, SpriteFont font = null, GraphicsDevice graphicsDevice = null) : base(editor.Editor, font, graphicsDevice)
		{
			this.editor = editor;
		}

		public override void Draw(SpriteBatch sb)
		{
			foreach (var (_, caret) in editor.ExternalCarets)
				DrawCaret(sb, caret);
			foreach (var (_, (caret1, caret2)) in editor.ExternalSelections)
				DrawSelection(sb, caret1, caret2);
			base.Draw(sb);
		}
	}
}
