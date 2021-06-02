namespace KeySample.FormsApp.Models.Input
{
    using System.Diagnostics.CodeAnalysis;

    public class TextInputParameter
    {
        [AllowNull]
        public string Title { get; init; }

        [AllowNull]
        public string Value { get; init; }

        public int MaxLength { get; init; }
    }
}
