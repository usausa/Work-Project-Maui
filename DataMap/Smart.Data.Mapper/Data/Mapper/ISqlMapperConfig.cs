namespace Smart.Data.Mapper
{
    using System;
    using System.Data;
    using System.Reflection;

    using Smart.Data.Mapper.Handlers;

    public interface ISqlMapperConfig
    {
        Func<object, object> CreateGetter(PropertyInfo pi);

        Func<object, object> CreateParser(Type sourceType, Type destinationType);

        DbType LookupDbType(Type type, out ITypeHandler handler);

        Action<IDbCommand, object> CreateParameterBuilder(Type type);

        Func<IDataRecord, T> CreateMapper<T>();
    }
}
