namespace Business.FormsApp.Models
{
    using System;

    using SQLite;

    [Table("Test")]
    public class TestEntity
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Qty { get; set; }

        public decimal Price { get; set; }

        public DateTime? Time { get; set; }
    }
}
