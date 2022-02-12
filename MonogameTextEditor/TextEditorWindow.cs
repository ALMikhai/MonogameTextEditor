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
		public MultiUserEditor Editor { get; }

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private SpriteFont font;

		private MultiUserTextEditor textEditor;
		private MultiUserTextPresenter textPresenter;

		public TextEditorWindow()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Editor = new MultiUserEditor();
		}

		protected override void Initialize()
		{
			textEditor = new MultiUserTextEditor(Editor);
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Content.Load<SpriteFont>("myFont");
			textPresenter = new MultiUserTextPresenter(Editor, font, GraphicsDevice);
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
