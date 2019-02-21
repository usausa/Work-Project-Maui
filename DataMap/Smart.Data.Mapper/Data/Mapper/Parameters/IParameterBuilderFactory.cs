namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Data;

    public interface IParameterBuilderFactory
    {
        bool IsMatch(Type type);

        Action<IDbCommand, object> CreateBuilder(Type type);
    }
}
