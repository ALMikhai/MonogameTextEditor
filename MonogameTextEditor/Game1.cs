using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameTextEditor.TextEditor;

namespace MonogameTextEditor
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private SpriteFont _font;

		private ICaretEditor _editor;
		private TextEditor.TextEditor _textEditor;
		private TextPresenter _textPresenter;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			_editor = new CaretEditor();
		}

		protected override void Initialize()
		{
			_textEditor = new TextEditor.TextEditor(_editor);
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_font = Content.Load<SpriteFont>("myFont");
			_textPresenter = new TextPresenter(_editor, _font, GraphicsDevice);
		}

		protected override void Update(GameTime gameTime)
		{
			_textEditor.Update();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);
			_spriteBatch.Begin();
			_textPresenter.Draw(_spriteBatch);
			_spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
