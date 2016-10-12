
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Dapper.Demo
{

    [Table("Users")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Gender { get; set; }
        public int? Age { get; set; }
        public DateTime? OpTime { get; set; }
    }
}
