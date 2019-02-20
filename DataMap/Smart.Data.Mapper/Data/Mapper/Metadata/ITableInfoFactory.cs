namespace Smart.Data.Mapper.Metadata
{
    public interface ITableInfoFactory
    {
        TableInfo Create<T>();
    }
}
