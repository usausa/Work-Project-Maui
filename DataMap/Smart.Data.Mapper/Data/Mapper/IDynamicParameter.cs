namespace Smart.Data.Mapper
{
    using System.Data;

    public interface IDynamicParameter
    {
        void BuildParameters(IDbCommand cmd);
    }
}
