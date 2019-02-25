namespace Smart.Data.Mapper
{
    using System;
    using System.Data;

    public interface ISqlMapperConfig
    {
        // Parameter

        DbType LookupDbType(object value);

        // TODO
    }
}
