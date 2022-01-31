using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace MonogameTextEditor.TextEditor {
    public static class CmdObserver {
        private static KeyboardState currentKeyboardState = Keyboard.GetState();
        private static KeyboardState oldKeyboardState;

        public static event Action Undo;
        public static event Action Redo;
        public static event Action SelectAll;
        public static event Action Cut;
        public static event Action Copy;
        public static event Action Paste;
        public static event Action Delete;
        public static event Action MoveCharPrev;
        public static event Action MoveCharNext;
        public static event Action MoveWordPrev;
        public static event Action MoveWordNext;
        public static event Action MoveLinePrev;
        public static event Action MoveLineNext;
        public static event Action MoveLineStart;
        public static event Action MoveLineEnd;
        public static event Action SelectCharPrev;
        public static event Action SelectCharNext;
        public static event Action SelectLinePrev;
        public static event Action SelectLineNext;
        public static event Action SelectWordPrev;
        public static event Action SelectWordNext;
        public static event Action SelectLineStart;
        public static event Action SelectLineEnd;
        public static event Action SelectCurrentWord;
        public static event Action DeleteWordPrev;
        public static event Action DeleteWordNext;
        public static event Action Enter;
        public static event Action BackSpace;

        public delegate void InsertText(string s);
        public static event InsertText OnTextInsert;

        public static void Update() {
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            var pressedKeys = currentKeyboardState.GetPressedKeys();
            foreach (var key in pressedKeys)
                if (pressedKeys.Contains(Keys.LeftControl) && pressedKeys.Contains(Keys.LeftShift)) {
                    if (oldKeyboardState.IsKeyUp(key))
                        switch (key) {
                            case Keys.Left:
                                SelectWordPrev?.Invoke();
                                break;
                            case Keys.Right:
                                SelectWordNext?.Invoke();
                                break;
                        }
                }
                else if (pressedKeys.Contains(Keys.LeftControl)) {
                    if (oldKeyboardState.IsKeyUp(key))
                        switch (key) {
                            case Keys.V:
                                Paste?.Invoke();
                                break;
                            case Keys.C:
                                Copy?.Invoke();
                                break;
                            case Keys.Left:
                                MoveWordPrev?.Invoke();
                                break;
                            case Keys.Right:
                                MoveWordNext?.Invoke();
                                break;
                            case Keys.A:
                                SelectAll?.Invoke();
                                break;
                        }
                }
                else if (pressedKeys.Contains(Keys.LeftShift)) {
                    if (oldKeyboardState.IsKeyUp(key))
                        switch (key) {
                            case Keys.Left:
                                SelectCharPrev?.Invoke();
                                break;
                            case Keys.Right:
                                SelectCharNext?.Invoke();
                                break;
                            case Keys.Up:
                                SelectLinePrev?.Invoke();
                                break;
                            case Keys.Down:
                                SelectLineNext?.Invoke();
                                break;
                        }
                }
                else if (oldKeyboardState.IsKeyUp(key)) {
                    switch (key) {
                        case Keys.Back:
                            BackSpace?.Invoke();
                            break;
                        case Keys.Delete:
                            Delete?.Invoke();
                            break;
                        case Keys.Enter:
                            Enter?.Invoke();
                            break;
                        case Keys.Left:
                            MoveCharPrev?.Invoke();
                            break;
                        case Keys.Right:
                            MoveCharNext?.Invoke();
                            break;
                        case Keys.Up:
                            MoveLinePrev?.Invoke();
                            break;
                        case Keys.Down:
                            MoveLineNext?.Invoke();
                            break;
                        case Keys.Space:
                            OnTextInsert?.Invoke(" ");
                            break;
                        case Keys.Home:
                            MoveLineStart?.Invoke();
                            break;
                        case Keys.End:
                            MoveLineEnd?.Invoke();
                            break;
                        default:
                            OnTextInsert?.Invoke(key.ToString());
                            break;
                    }
                }
        }
    }
}
