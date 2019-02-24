namespace Smart.Data.Mapper.Mappers
{
    public class ObjectResultMapper
    {
        // TODO Default?, Naming version?
        // TODO Naming Pascal, Camel, Snake Converter Func
        // Attribute only ?
        // Procedureとの共有？

        //private readonly Dictionary<string, IAccessor> mapAccessors = new Dictionary<string, IAccessor>(StringComparer.OrdinalIgnoreCase);

        //public DefaultTypeMetadata(Type type)
        //{
        //    foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        //    {
        //        if (accessor.CanWrite) // With Naming
        //        {
        //            mapAccessors[pi.Name] = accessor;
        //        }
        //    }
        //}

        //public IAccessor GetMapAccessor(string column)
        //{
        //    IAccessor accessor;
        //    mapAccessors.TryGetValue(column, out accessor);
        //    return accessor;
        //}
    }

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
