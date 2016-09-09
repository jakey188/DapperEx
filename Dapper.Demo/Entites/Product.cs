
using System;
using Dapper.Contrib.Extensions;

namespace Dapper.Demo.Entites
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        public double UnitPrice { get; set; }

        public int ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public DateTime AddTime { get; set; }
    }
}
