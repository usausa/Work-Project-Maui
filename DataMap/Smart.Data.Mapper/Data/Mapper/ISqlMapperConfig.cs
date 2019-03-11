namespace Smart.Data.Mapper
{
    using System;
    using System.Data;
    using System.Reflection;

    using Smart.Data.Mapper.Handlers;

    public interface ISqlMapperConfig
    {
        Func<object, object> CreateGetter(PropertyInfo pi);

        // Parameter

        DbType LookupDbType(Type type, out ITypeHandler handler);

        bool BuildCommand(IDbCommand cmd, object param);

        Func<object, object> CreateParser(Type sourceType, Type destinationType);

        // TODO
    }
}
