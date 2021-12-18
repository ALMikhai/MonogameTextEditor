using Microsoft.Xna.Framework.Input;

namespace MonogameTextEditor.TextEditor
{
    public class TextEditor
    {
        private readonly ITextCollection _text;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _oldKeyboardState;

        public TextEditor(ITextCollection collection)
        {
            _text = collection;
            _currentKeyboardState = Keyboard.GetState();
        }

        public void Update()
        {
            _oldKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            var pressedKeys = _currentKeyboardState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                if (_oldKeyboardState.IsKeyUp(key))
                {
                    switch (key)
                    {
                        case Keys.Escape:
                            // Exit();
                            break;
                        case Keys.Back:
                            _text.RemoveBackward();
                            break;
                        case Keys.Delete:
                            _text.RemoveForward();
                            break;
                        case Keys.Enter:
                            _text.InsertAtCaretPosition("\n");
                            break;
                        case Keys.Left:
                            _text.MoveCaretRight(-1);
                            break;
                        case Keys.Right:
                            _text.MoveCaretRight(1);
                            break;
                        case Keys.Up:
                            _text.MoveCaretDown(-1);
                            break;
                        case Keys.Down:
                            _text.MoveCaretDown(1);
                            break;
                        default:
                            _text.InsertAtCaretPosition(key.ToString());
                            break;
                    }
                }
            }
        }
    }
}
