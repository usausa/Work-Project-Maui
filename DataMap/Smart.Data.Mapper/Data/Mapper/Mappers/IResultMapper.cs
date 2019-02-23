namespace Smart.Data.Mapper.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public interface IResultMapper
    {
        bool IsMatch(Type type);

        // TODO factory, converter, handler ?
        IEnumerable<T> Map<T>(ISqlMapperConfig config, IDataReader reader);
    }

    //public interface IQueryHandler
    //    bool IsMatch(Type type);
    //    IEnumerable<T> Handle<T>(Func<T> factory, IDataReader reader, ObjectConverter converter);
    //}
}
