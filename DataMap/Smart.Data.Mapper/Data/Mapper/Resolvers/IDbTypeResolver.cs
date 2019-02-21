namespace Smart.Data.Mapper.Resolvers
{
    using System;
    using System.Data;

    public interface IDbTypeResolver
    {
        bool TryResolve(Type type, out DbType dbType);
    }
}
