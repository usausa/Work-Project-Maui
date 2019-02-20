namespace Smart.Data.Mapper.Metadata
{
    public sealed class StandardTableInfoFactory : ITableInfoFactory
    {
        public TableInfo Create<T>()
        {
            return new TableInfo();
        }
    }
}
