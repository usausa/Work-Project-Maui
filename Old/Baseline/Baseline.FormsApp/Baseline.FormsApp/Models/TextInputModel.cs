namespace Baseline.FormsApp.Models
{
    using System;

    using Smart.ComponentModel;

    public class TextInputModel : NotificationObject
    {
        private string text = string.Empty;

        private string display = string.Empty;

        public int MaxLength { get; set; }

        public bool UseMask { get; set; }

        public char MaskChar { get; set; } = '*';

        public string Text
        {
            get => text;
            set
            {
                if (SetProperty(ref text, value ?? string.Empty))
                {
                    Display = UseMask ? new String(MaskChar, text.Length) : text;
                }
            }
        }

        public string Display
        {
            get => display;
            private set => SetProperty(ref display, value);
        }

        public void Clear()
        {
            Text = string.Empty;
        }

        public void Pop()
        {
            if (text.Length > 0)
            {
                Text = text.Substring(0, text.Length - 1);
            }
        }

        public void Push(string key)
        {
            if (text.Length + key.Length <= MaxLength)
            {
                Text = text + key;
            }
        }
    }
}
