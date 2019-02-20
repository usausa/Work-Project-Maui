namespace Smart.Data.Mapper.Metadata
{
    using Smart.Data.Mapper.Namings;

    public sealed class StandardProcedureInfoFactory : IProcedureInfoFactory
    {
        private readonly INaming naming;

        public StandardProcedureInfoFactory(INaming naming)
        {
            this.naming = naming;
        }

        public ProcedureInfo Create<T>()
        {
            return new ProcedureInfo();
        }
    }
}
