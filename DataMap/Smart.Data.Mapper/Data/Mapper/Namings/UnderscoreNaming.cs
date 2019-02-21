namespace Smart.Data.Mapper.Namings
{
    using Smart.Text;

    public sealed class UnderscoreNaming : INaming
    {
        public static UnderscoreNaming Upper { get; } = new UnderscoreNaming(true);

        public static UnderscoreNaming Lower { get; } = new UnderscoreNaming(false);

        private readonly bool toUpper;

        private UnderscoreNaming(bool toUpper)
        {
            this.toUpper = toUpper;
        }

        public string Convert(string source) => Inflector.Underscore(source, toUpper);
    }
}