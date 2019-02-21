namespace Smart.Data.Mapper.Namings
{
    public sealed class DefaultNaming : INaming
    {
        public static DefaultNaming Instance { get; } = new DefaultNaming();

        private DefaultNaming()
        {
        }

        public string Convert(string source) => source;
    }
}
