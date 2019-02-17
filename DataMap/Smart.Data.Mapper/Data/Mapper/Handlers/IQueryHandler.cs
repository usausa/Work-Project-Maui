namespace Smart.Data.Mapper.Handlers
{
    public interface IQueryHandler
    {
    }

    //public interface IQueryHandler
    //{
    //    bool IsMatch(Type type);

    //    IEnumerable<T> Handle<T>(Func<T> factory, IDataReader reader, ObjectConverter converter);
    //}

    //--

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

    //--

    //public class ObjectQueryHandler : IQueryHandler
    //{
    //    private readonly ITypeMetadataFactory metadataFactory;

    //    public ObjectQueryHandler(ITypeMetadataFactory metadataFactory)
    //    {
    //        this.metadataFactory = metadataFactory;
    //    }

    //    public bool IsMatch(Type type)
    //    {
    //        return true;
    //    }

    //    public IEnumerable<T> Handle<T>(Func<T> factory, IDataReader reader, ObjectConverter converter)
    //    {
    //        using (reader)
    //        {
    //            var metadata = metadataFactory.Create(typeof(T));

    //            var columns = new string[reader.FieldCount];
    //            var accessors = new IAccessor[reader.FieldCount];
    //            for (var i = 0; i < columns.Length; i++)
    //            {
    //                var name = reader.GetName(i);
    //                columns[i] = name;
    //                accessors[i] = metadata.GetMapAccessor(name);
    //            }

    //            while (reader.Read())
    //            {
    //                var entity = factory();

    //                for (var i = 0; i < columns.Length; i++)
    //                {
    //                    var accessor = accessors[i];
    //                    if (accessor == null)
    //                    {
    //                        continue;
    //                    }

    //                    if (reader.IsDBNull(i))
    //                    {
    //                        accessor.SetValue(entity, accessor.Type.GetDefaultValue());
    //                    }
    //                    else
    //                    {
    //                        var value = reader[columns[i]];
    //                        if (accessor.Type != value.GetType())
    //                        {
    //                            value = converter.Convert(value, accessor.Type);
    //                        }

    //                        accessor.SetValue(entity, value);
    //                    }
    //                }

    //                yield return entity;
    //            }
    //        }
    //    }
    //}
}
