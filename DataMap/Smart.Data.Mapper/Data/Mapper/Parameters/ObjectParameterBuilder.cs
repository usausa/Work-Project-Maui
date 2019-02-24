namespace Smart.Data.Mapper.Parameters
{
    using System.Data;

    public sealed class ObjectParameterBuilder : IParameterBuilder
    {
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
        public bool Handle(ISqlMapperConfig config, IDbCommand cmd, object parameter)
        {
            // TODO
            return true;
        }
    }
}
