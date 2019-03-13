namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Data;

    public sealed class ProcedureParameterBuilderFactory : IParameterBuilderFactory
    {
        public static ProcedureParameterBuilderFactory Instance { get; } = new ProcedureParameterBuilderFactory();

        private ProcedureParameterBuilderFactory()
        {
        }

        public bool IsMatch(Type type)
        {
            // TODO
            return false;
        }

        public Action<IDbCommand, object> CreateBuilder(ISqlMapperConfig config, Type type)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
