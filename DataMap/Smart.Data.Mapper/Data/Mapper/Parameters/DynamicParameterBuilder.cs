namespace Smart.Data.Mapper.Parameters
{
    using System.Data;

    public sealed class DynamicParameterBuilder : IParameterBuilder
    {
        public bool Handle(ISqlMapperConfig config, IDbCommand cmd, object value)
        {
            if (value is IDynamicParameter parameter)
            {
                parameter.Build(config, cmd);
                return true;
            }

            return false;
        }
    }
}
