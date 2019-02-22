namespace Smart.Data.Mapper.Mappers
{
    public class ObjectResultMapperFactory
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
}
