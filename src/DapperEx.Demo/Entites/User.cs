
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Demo
{

    [Table("Users")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnmUserGender Gender { get; set; }
        public int? Age { get; set; }
        public DateTime? OpTime { get; set; }
    }

    public enum EnmUserGender : int
    {
        男 = 1,
        女 = 2
    }
}
