using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameTextEditor.TextEditor;
using TextEditor.CaretEditor;
using TextEditor.SelectEditor;

namespace MonogameTextEditor {
    public class TextEditorWindow : Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private readonly ISelectEditor editor;
        private SelectTextEditor textEditor;
        private SelectTextPresenter textPresenter;

        public TextEditorWindow() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            editor = new SelectEditor(new CaretEditor());
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
