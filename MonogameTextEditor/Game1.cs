﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameTextEditor.TextEditor;
using MonogameTextEditor.TextEditor.TextCollection;

namespace MonogameTextEditor {
    public class Game1 : Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private readonly ITextEditor editor;
        private SelectTextEditor textEditor;
        private SelectTextPresenter textPresenter;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            editor = new EditorDecorator(new ReadOnlyEditor(new TextCollection()));
        }

        protected override void Initialize() {
            textEditor = new SelectTextEditor(editor);
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("myFont");
            textPresenter = new SelectTextPresenter(editor, font, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            CmdObserver.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            textPresenter.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
