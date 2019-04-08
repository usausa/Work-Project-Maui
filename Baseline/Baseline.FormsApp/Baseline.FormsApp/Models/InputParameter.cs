namespace Baseline.FormsApp.Models
{
    public class InputParameter<T>
    {
        public string Title { get; }

        public T Value { get; }

        public int MaxLength { get; }

        public bool UseMask { get; }

        public InputParameter(string title, T value, int maxLength, bool useMask)
        {
            Title = title;
            Value = value;
            MaxLength = maxLength;
            UseMask = useMask;
        }
    }
}
