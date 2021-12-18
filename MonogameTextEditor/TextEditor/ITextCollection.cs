using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonogameTextEditor.TextEditor
{
    public interface ITextCollection
    {
        ICaretPosition Caret { get; set; }
        string Text { get; set; }
        void RemoveForward();
        void RemoveBackward();
        void InsertAtCaretPosition(string s);
        bool MoveCaretRight(int n);
        bool MoveCaretDown(int n);
        string GetCurrentLine();
    }

    public class ArrayStringText : ITextCollection
    {
        private List<string> text;

        public ICaretPosition Caret { get; set; } = new DrawableCaret();

        public string Text
        {
            get
            {
                var builder = new StringBuilder();
                for (int i = 0; i < text.Count; i++)
                {
                    var line = text[i];
                    var isLastLine = i == text.Count - 1;
                    if (isLastLine)
                        builder.Append(line);
                    else
                        builder.AppendLine(line);
                }

                return builder.ToString();
            }
            set => text = value.Split('\n').ToList();
        }

        public ArrayStringText()
        {
            text = new List<string>() { "" };
        }

        public void RemoveForward()
        {
            if (Caret.Line + 1 < text.Count && Caret.Col == text[Caret.Line].Length)
            {
                var line = text[Caret.Line + 1];
                text.RemoveAt(Caret.Line + 1);
                text[Caret.Line] += line;
            } else if (Caret.Col < text[Caret.Line].Length)
            {
                text[Caret.Line] = text[Caret.Line].Remove(Caret.Col, 1);
            }
        }

        public void RemoveBackward()
        {
            if (MoveCaretRight(-1))
                RemoveForward();
        }

        public void InsertAtCaretPosition(string s)
        {
            var lines = s.Split('\n');

            var firstPart = text[Caret.Line].Substring(0, Caret.Col);
            var secondPart = text[Caret.Line].Substring(Caret.Col, text[Caret.Line].Length - Caret.Col);

            text[Caret.Line] = firstPart + lines[0];

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                Caret.Line++;
                text.Insert(Caret.Line, line);
            }

            Caret.Col = text[Caret.Line].Length;
            text[Caret.Line] += secondPart;
        }

        public bool MoveCaretDown(int n)
        {
            if (Caret.Line + n >= 0 && Caret.Line + n < text.Count){
                Caret.Line += n;

                // TODO replace to Mathf from Citrus.
                Caret.Col = Math.Min(Caret.Col, text[Caret.Line].Length);

                return true;
            }

            return false;
        }

        public bool MoveCaretRight(int n)
        {
            if (Caret.Col + n >= 0)
            {
                if (Caret.Col + n <= text[Caret.Line].Length)
                {
                    Caret.Col += n;
                    return true;
                }
                else
                {
                    if (MoveCaretDown(1))
                    {
                        Caret.Col = 0;
                        return true;
                    }
                }
            }
            else
            {
                if (MoveCaretDown(-1))
                {
                    Caret.Col = text[Caret.Line].Length;
                    return true;
                }
            }

            return false;
        }

        public string GetCurrentLine()
        {
            return text[Caret.Line];
        }
    }
}
