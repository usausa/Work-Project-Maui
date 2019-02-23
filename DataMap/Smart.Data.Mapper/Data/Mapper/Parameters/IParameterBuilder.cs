namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Data;

    public interface IParameterBuilder
    {
        bool Handle(ISqlMapperConfig config, IDbCommand cmd, object value);
    }
}
