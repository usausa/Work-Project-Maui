namespace Smart.Data.Mapper.Metadata
{
    using System;
    using Smart.Data.Mapper.Namings;

    public sealed class StandardProcedureInfoFactory : IProcedureInfoFactory
    {
        private static class ProcedureInfoHolder<T>
        {
            public static ProcedureInfo Value { get; }

            static ProcedureInfoHolder()
            {
                // TODO
                Value = new ProcedureInfo(typeof(T));
            }
        }

        public ProcedureInfo Create<T>()
        {
            return ProcedureInfoHolder<T>.Value;
        }
    }
}
