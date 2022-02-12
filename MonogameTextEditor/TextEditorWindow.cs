using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameTextEditor.TextEditor;
using TextEditor.Caret;
using TextEditor.CaretEditor;
using TextEditor.MultiUserEditor;
using TextEditor.SelectEditor;

namespace MonogameTextEditor
{
	public class TextEditorWindow : Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private SpriteFont font;

		private TextPresenter textPresenter;

		public TextEditorWindow(TextPresenter presenter)
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			textPresenter = presenter;
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Content.Load<SpriteFont>("myFont");
			textPresenter.GraphicsDevice = GraphicsDevice;
			textPresenter.Font = font;
		}

		protected override void Update(GameTime gameTime)
		{
			CmdObserver.Update();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.White);
			spriteBatch.Begin();

			textPresenter.Draw(spriteBatch);

			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
