using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace MonogameTextEditor.TextEditor
{
    public class TextEditor
    {
        private readonly ICaretEditor _editor;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _oldKeyboardState;

        public TextEditor(ICaretEditor editor)
        {
            _editor = editor;
            _currentKeyboardState = Keyboard.GetState();
        }

        public void Update()
        {
            _oldKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            var pressedKeys = _currentKeyboardState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                if (pressedKeys.Contains(Keys.LeftControl) && _oldKeyboardState.IsKeyUp(key))
                {
                    switch (key)
                    {
                        case Keys.V:
                            _editor.Insert(Clipboard.GetText());
                            break;
                        case Keys.C:
                            Clipboard.SetText(_editor.GetCurrentLine());
                            break;
                    }
                } else if (_oldKeyboardState.IsKeyUp(key))
                {
                    switch (key)
                    {
                        case Keys.Escape:
                            // Exit();
                            break;
                        case Keys.Back:
                            _editor.RemoveBackward();
                            break;
                        case Keys.Delete:
                            _editor.RemoveForward();
                            break;
                        case Keys.Enter:
                            _editor.Insert("\n");
                            break;
                        case Keys.Left:
                            _editor.MoveCaretRight(-1);
                            break;
                        case Keys.Right:
                            _editor.MoveCaretRight(1);
                            break;
                        case Keys.Up:
                            _editor.MoveCaretDown(-1);
                            break;
                        case Keys.Down:
                            _editor.MoveCaretDown(1);
                            break;
                        default:
                            _editor.Insert(key.ToString());
                            break;
                    }
                }
            }
        }
    }
}
