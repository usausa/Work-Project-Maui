namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Data;

    public sealed class DynamicParameterBuilderFactory : IParameterBuilderFactory
    {
        public bool IsMatch(Type type)
        {
            return typeof(IDynamicParameter).IsAssignableFrom(type);
        }

        public Action<IDbCommand, object> CreateBuilder(SqlMapperConfig config, Type type)
        {
            return (cmd, parameter) => ((IDynamicParameter)parameter).Build(config, cmd);
        }
    }
}
