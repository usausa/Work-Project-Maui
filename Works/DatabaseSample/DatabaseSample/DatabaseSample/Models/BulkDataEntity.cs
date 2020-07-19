namespace DatabaseSample.Models
{
    using Smart.Data.Mapper.Attributes;

    public class BulkDataEntity
    {
        [PrimaryKey(1)]
        private string Key1 { get; set; }

        [PrimaryKey(2)]
        private string Key2 { get; set; }

        [PrimaryKey(3)]
        private string Key3 { get; set; }

        private int Value1 { get; set; }

        private int Value2 { get; set; }

        private int Value3 { get; set; }

        private int Value4 { get; set; }

        private int Value5 { get; set; }
    }
}
