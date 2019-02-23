namespace Smart.Data.Mapper.Mappers
{
    public class DictionaryResultMapper
    {
    }

    //public class DictionaryQueryHandler : IQueryHandler
    //{
    //    private static readonly Type TargetType = typeof(IDictionary<string, object>);

    //    public bool IsMatch(Type type)
    //    {
    //        return TargetType.IsAssignableFrom(type);
    //    }

    //    public IEnumerable<T> Handle<T>(Func<T> factory, IDataReader reader, ObjectConverter converter)
    //    {
    //        var columns = new string[reader.FieldCount];
    //        for (var i = 0; i < columns.Length; i++)
    //        {
    //            columns[i] = reader.GetName(i);
    //        }

    //        while (reader.Read())
    //        {
    //            var entity = (IDictionary<string, object>)factory();

    //            for (var i = 0; i < columns.Length; i++)
    //            {
    //                var column = columns[i];

    //                entity[column] = reader.IsDBNull(i) ? null : reader[column];
    //            }

    //            yield return (T)entity;
    //        }
    //    }
    //}
}
