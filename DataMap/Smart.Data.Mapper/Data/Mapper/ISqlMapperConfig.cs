namespace Smart.Data.Mapper
{
    using System;
    using System.Data;
    using System.Reflection;

    public interface ISqlMapperConfig
    {
        Func<T> CreateFactory<T>();

        Func<object, object> CreateGetter(PropertyInfo pi);

        Action<object, object> CreateSetter(PropertyInfo pi);

        Func<string, string> GetNameConverter();

        Func<object, object> CreateParser(Type sourceType, Type destinationType);

        TypeHandleEntry LookupTypeHandle(Type type);

        Action<IDbCommand, object> CreateParameterBuilder(Type type);

        Func<IDataRecord, T> CreateMapper<T>(IDataReader reader);
    }
}
