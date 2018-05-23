
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Demo
{

	[Table("yx_search_history"), Serializable]
    public class SearchHistory
    {
        /// <summary>
        /// 主键
        /// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
		public string KeyWord { get; set; }
        /// <summary>
        /// 搜索类型 0：类目1：产品2：URL
        /// </summary>
		public int Type { get; set; }
        /// <summary>
        /// 系统1:APP2:微信3:其它
        /// </summary>
		public int SearchSystem { get; set; }
        /// <summary>
        /// IP
        /// </summary>
		public string Ip { get; set; }
        /// <summary>
        /// 无登陆为0，登录填写用户ID
        /// </summary>
		public long HnUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
		public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
		public DateTime? ModifyTime { get; set; }
    }

    [Table("Users")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnmUserGender Gender { get; set; }
        public int? Age { get; set; }
        public bool Gender1 { get; set; }
        public DateTime? OpTime { get; set; }
    }

    public enum EnmUserGender : int
    {
        男 = 1,
        女 = 2
    }
}
