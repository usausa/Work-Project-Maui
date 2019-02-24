namespace Smart.Data.Mapper.Parameters
{
    using System.Data;

    public sealed class DynamicParameterBuilder : IParameterBuilder
    {
        public bool Handle(ISqlMapperConfig config, IDbCommand cmd, object parameter)
        {
            if (parameter is IDynamicParameter dynamicParameter)
            {
                dynamicParameter.Build(config, cmd);
                return true;
            }

            return false;
        }
    }
}
