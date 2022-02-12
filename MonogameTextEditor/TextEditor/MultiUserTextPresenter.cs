using Microsoft.Xna.Framework.Graphics;
using TextEditor.MultiUserEditor;

namespace MonogameTextEditor.TextEditor
{
	public class MultiUserTextPresenter : SelectTextPresenter
	{
		public MultiUserEditor Editor { get; }

		public MultiUserTextPresenter(MultiUserEditor editor, SpriteFont font, GraphicsDevice graphicsDevice) : base(editor.Editor, font, graphicsDevice)
		{
			Editor = editor;
		}

		public override void Draw(SpriteBatch sb)
		{
			foreach (var (_, caret) in Editor.ExternalCarets)
				DrawCaret(sb, caret);
			foreach (var (_, (caret1, caret2)) in Editor.ExternalSelections)
				DrawSelection(sb, caret1, caret2);
			base.Draw(sb);
		}
	}
}
