namespace Smart.Data.Mapper.Parameters
{
    public class ObjectParameterBuilderFactory
    {
        //public class DynamicParameterParameterBuilder : IParameterBuilder
        //{
        //    public bool IsMatch(object param)
        //    {
        //        return param is IDynamicParameter;
        //    }

        //    public void BuildParameters(IDbCommand cmd, object param)
        //    {
        //        ((IDynamicParameter)param).BuildParameters(cmd);
        //    }
        //}

        // --

        //public class DictionaryParameterBuilder : IParameterBuilder
        //{
        //    private readonly DbTypeMap dbTypeMap;

        //    public DictionaryParameterBuilder(DbTypeMap dbTypeMap)
        //    {
        //        this.dbTypeMap = dbTypeMap;
        //    }

        //    public bool IsMatch(object param)
        //    {
        //        return param is IDictionary<string, object>;
        //    }

        //    public void BuildParameters(IDbCommand cmd, object param)
        //    {
        //        foreach (var keyValue in (IDictionary<string, Object>)param)
        //        {
        //            var parameter = cmd.CreateParameter();
        //            parameter.ParameterName = keyValue.Key;
        //            var value = keyValue.Value ?? DBNull.Value;
        //            if (value != DBNull.Value)
        //            {
        //                parameter.DbType = dbTypeMap.LookupDbType(value);
        //            }
        //            parameter.Value = value;

        //            cmd.Parameters.Add(parameter);
        //        }
        //    }
        //}

        // --

        //public class ObjectParameterBuilder : IParameterBuilder
        //{
        //    private readonly DbTypeMap dbTypeMap;

        //    private readonly ITypeMetadataFactory metadataFactory;

        //    public ObjectParameterBuilder(DbTypeMap dbTypeMap, ITypeMetadataFactory metadataFactory)
        //    {
        //        this.dbTypeMap = dbTypeMap;
        //        this.metadataFactory = metadataFactory;
        //    }

        //    public bool IsMatch(object param)
        //    {
        //        return true;
        //    }

        //    public void BuildParameters(IDbCommand cmd, object param)
        //    {
        //        foreach (var accessor in metadataFactory.Create(param.GetType()).GetParameterAccessors())
        //        {
        //            var parameter = cmd.CreateParameter();
        //            parameter.ParameterName = accessor.Name;
        //            var value = accessor.GetValue(param) ?? DBNull.Value;
        //            parameter.DbType = dbTypeMap.LookupDbType(accessor.Type);
        //            parameter.Value = value;

        //            cmd.Parameters.Add(parameter);
        //        }
        //    }
        //}

        //private readonly IList<IAccessor> parameterAccessors = new List<IAccessor>();

        //public DefaultTypeMetadata(Type type)
        //{
        //    foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        //    {
        //        var accessor = pi.ToAccessor();
        //        if (accessor.CanRead)
        //        {
        //            parameterAccessors.Add(accessor);
        //        }
        //    }
        //}

        //public IEnumerable<IAccessor> GetParameterAccessors()
        //{
        //    return parameterAccessors;
        //}
    }
}
