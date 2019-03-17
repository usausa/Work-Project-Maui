namespace DataAccess.FormsApp.Handlers
{
    using System;
    using System.Data;

    using Smart.Data.Mapper.Handlers;

    public sealed class DateTimeTypeHandler : ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            throw new NotImplementedException();
        }

        public object Parse(Type destinationType, object value)
        {
            throw new NotImplementedException();
        }
    }
}
