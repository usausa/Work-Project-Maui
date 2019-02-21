namespace Smart.Data.Mapper
{
    public static partial class SqlMapper
    {
        private static class ProcedureSlot
        {
            public static int Slot { get; }

            static ProcedureSlot()
            {
                Slot = ExtensionSlotManager.Allocate();
            }
        }
    }
}
