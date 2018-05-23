using Dapper.Demo;
using DapperEx.MySql;
using DapperEx.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DapperEx.Sqlite.BulkInserts;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DapperEx.Demo.Tests
{
    /// <summary>
    /// yx_detail_order
    /// </summary>
    [Table("yx_order_detail"), Serializable]
    public class OrderDetail
    {
        /// <summary>
        /// Id
        /// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        /// <summary>
        /// 主订单Id
        /// </summary>
        public long OrderMainId { get; set; }
        /// <summary>
        /// 主订单编号
        /// </summary>
		public string MainOrderCode { get; set; }
        /// <summary>
        /// 子订单编号
        /// </summary>
		public string DetailOrderCode { get; set; }
        /// <summary>
        /// 购买者ID
        /// </summary>
		public long BuyUserId { get; set; }
        /// <summary>
        /// 仓库Id
        /// </summary>
		public long DepotInfoId { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
		public DateTime CreateTime { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
		public DateTime? PayTime { get; set; }
        /// <summary>
        /// 支付流水号
        /// </summary>
		public string PaymentNo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
		public EnmOrderDetailState Status { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
		public string LinkMan { get; set; }
        /// <summary>
        /// 收货人手机
        /// </summary>
		public string Mobile { get; set; }
        /// <summary>
        /// 省份Id
        /// </summary>
		public long? ProvinceId { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
		public string ProvinceName { get; set; }
        /// <summary>
        /// 城市Id
        /// </summary>
		public long? CityId { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
		public string CityName { get; set; }
        /// <summary>
        /// 区域Id
        /// </summary>
		public long? DistrictId { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
		public string DistrictName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
		public string FullAddress { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
		public long ProductId { get; set; }
        /// <summary>
        /// 产品规格名称
        /// </summary>
		public string ProductSpecificationName { get; set; }
        /// <summary>
        /// 产品主图
        /// </summary>
		public string ProductImg { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
		public string ProductName { get; set; }
        /// <summary>
        /// 购买时单个商品的金额
        /// </summary>
		public decimal ProductPrice { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>

        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 实际需要支付金额
        /// </summary>

        public decimal PaymentAmount { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
		public int ProductCnt { get; set; }
        /// <summary>
        /// 产品的总运费
        /// </summary>
		public decimal Freight { get; set; }
        /// <summary>
        /// ProductSecificationId
        /// </summary>
		public long ProductSecificationId { get; set; }
        /// <summary>
        /// 订单取消来源 0  未取消，1供应商，2采购商，3系统
        /// </summary>
		public int CancelSource { get; set; }
        /// <summary>
        /// 订单取消时间
        /// </summary>
		public DateTime? CancelTime { get; set; }
        /// <summary>
        /// 订单取消原因
        /// </summary>
		public string CancelContent { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
		public DateTime? DeliveryTime { get; set; }
        /// <summary>
        /// 订单收货时间
        /// </summary>
		public DateTime? ReceivingTime { get; set; }
        /// <summary>
        /// 发货地址，0 供应商地址，1采购商自己的地址,2:取传过来的发货人信息
        /// </summary>
		public int DeliveryType { get; set; }
        /// <summary>
        /// 已经发货数量
        /// </summary>
		public int DeliverCnt { get; set; }
        /// <summary>
        /// DeliveryLinkMan
        /// </summary>
		public string DeliveryLinkMan { get; set; }
        /// <summary>
        /// 发货人手机
        /// </summary>
		public string DeliveryMobile { get; set; }
        /// <summary>
        /// 发货人地址
        /// </summary>
		public string DeliveryAddress { get; set; }
        /// <summary>
        /// 结算状态 0:未结算 1:已结算
        /// </summary>
		public int SettlementState { get; set; }

        /// <summary>
        /// 仓库用户Id
        /// </summary>
        public long DeliveryUserId { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string DepotName { get; set; }

        /// <summary>
        /// 订单类型 1：普通订单  2：联盟订单 3：大宗采购 4：联盟大宗 5：转发订单 
        /// 1、2：一件代发
        /// 3、4：大宗采购
        /// 5：转发订单（农商易订单）
        /// 
        /// PS:修改会影响定时任务（慎改）
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 卖家备注
        /// </summary>
        public string SelllerReark { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
        public string BuyRemark { get; set; }

        /// <summary>
        /// 全额退款状态
        /// </summary>
        public EnmOrderRefundState RefundState { get; set; }

        /// <summary>
        /// 售后状态
        /// </summary>
        public EnmOrderDetailAfterState AfterState { get; set; }

        /// <summary>
        /// 销售价格（微商记账使用）
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 是否拨单0：未拨单、1：已拨单
        /// </summary>
        public int Distribution { get; set; }
        /// <summary>
        /// 拨单时间
        /// </summary>
        public DateTime? DistributionTime { get; set; }

        /// <summary>
        /// 是否删除 默认1 未删除 ,0 已删除
        /// </summary>
        public int? IsDelete { get; set; } = 1;
        /// <summary>
        /// 订单是否可结算 0：否 1：是
        /// </summary>
        public int UnSettlement { get; set; } = 0;
        /// <summary>
        /// 是否已经评论
        /// </summary>
        public int IsCommnet { get; set; } = 0;
    }

    public enum EnmOrderRefundState : int
    {
        无退款 = 0,
        退款中 = 1,
        退款成功 = 2,
        退款失败 = 3,
        取消退款 = 4
    }

    public enum EnmOrderDetailAfterState : int
    {
        无售后 = 0,
        售后未完成 = 1,
        售后已完成 = 2
    }

    /// <summary>
    /// 子订单状态
    /// </summary>
    public enum EnmOrderDetailState : int
    {
        未支付 = 0,
        已支付待发货 = 1,
        已发货待确认 = 2,
        部分发货 = 3,
        确认收货完成订单 = 4,//完结
        关闭订单 = 5,//完结
        未支付确认价格 = 6,
        退款关闭 = 7//完结
    }
    public class Test
    {

        public static void Init(DapperDbContext db)
        {
            //var user = db.Query<User>(x => x.Name == "张三").FirstOrDefault();
            //Add(db);
            //Delete(db);
            Update(db, 200202, EnmUserGender.男);
            //Query(db);
        }

        

        private static void Add(DapperDbContext db)
        {
            var user = new User
            {
                Age = 1,
                Gender = EnmUserGender.女,
                Name = "张三",
                OpTime = DateTime.Now,
                Gender1 = true
            };
            db.Add<User>(user);
        }

        private static void Update(DapperDbContext db,int id, EnmUserGender g)
        {
            //var user = db.Query<User>(x => x.Name == "张三").FirstOrDefault();
            //user.Age = 20;

            var now = DateTime.Now;
            UpdateState(db, 3, EnmOrderDetailState.确认收货完成订单);
            //db.Update<User>(x => x.Id == id, x => new User { Gender = g, OpTime=now });
            //db.Update(user);
            //db.Update<User>(x => x.Name == "张三", x => new User { Gender = EnmUserGender.男 });
        }


        public static int UpdateState(DapperDbContext db, long orderId, EnmOrderDetailState state)
        {

            var now = DateTime.Now;
            return db.Update<OrderDetail>(x => x.Id == orderId, x => new OrderDetail { Status = state, ReceivingTime = now });

        }

        private static void Delete(DapperDbContext db)
        {
            var user = db.Query<User>(x => x.Name == "张三").FirstOrDefault();
            db.Delete(user);
            Add(db);
            db.Delete<User>(x => x.Name == "张三");
            Add(db);
            db.DeleteAll<User>();
            Add(db);
        }

        private static void Query(DapperDbContext db)
        {
            

            var a1 = db.Query<User>(x => x.Name == "张三").Select(x => new { title = x.Name }).ToList();

            var a2 = db.Query<User>(x => x.Id > 0).ToList();

            var a3 = db.SqlQueryDynamic(" select * from users ", null);

            var a4 = db.Query<User>(x => x.Id == 12).FirstOrDefault();

            var a5 = db.Query<User>(x => x.Id > 1).OrderBy(x => x.Id).ThenByDescending(x => x.Age).ToList();
            var total = 0;

            var a6 = db.Query<User>(x => x.Id > 1).OrderBy(x => x.Id).ThenByDescending(x => x.Age).ToPageList(1, 5, out total);

            var a7 = db.Query<User>(x => x.Id > 0).GroupBy(x => x.Age).Select(x => x.Id).ToList();

            var a8 = db.Query<User>(x => x.Id > 0).Max(x => x.Id);

            var a9 = db.Query<User>().Where("age=1").FirstOrDefault();
            var id = 10;
            var a10 = db.Query<User>(x=>x.Id==id).FirstOrDefault();

            var g = true;
            var a11 = db.Query<User>(x => x.Gender1 && x.Gender1==g).FirstOrDefault();
        }
    }
}
