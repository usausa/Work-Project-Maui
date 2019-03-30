namespace DataAccess.FormsApp.Models
{
    using System;

    public class TestEntity
    {
        public long Id { get; set; }

        // varchar
        public string StringValue { get; set; }

        // integer
        public int IntValue { get; set; }

        // integer
        public long LongValue { get; set; }

        // float
        public double DoubleValue { get; set; }

        // float
        public decimal DecimalValue { get; set; }

        // integer
        public bool BoolValue { get; set; }

        // integer
        public DateTimeOffset DateTimeOffsetValue { get; set; }
    }
}
