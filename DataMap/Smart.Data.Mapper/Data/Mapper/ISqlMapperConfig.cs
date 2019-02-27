namespace Smart.Data.Mapper
{
    using System.Data;

    using Smart.Data.Mapper.Handlers;

    public interface ISqlMapperConfig
    {
        // Parameter

        DbType LookupDbType(object value, out ITypeHandler handler);

        // TODO
    }
}
