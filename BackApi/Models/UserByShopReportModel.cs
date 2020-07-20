using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class UserByShopReportModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int CountryId { get; set; }
		public string ShopId { get; set; }
		public int State { get; set; }
		public string ShopType { get; set; }
		public string Price { get; set; }
		public string BuyTime { get; set; }
		public string ShopEmail { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string CountryName { get; set; }
		public string CustomerNumber { get; set; }
	}

	public class ExcelShopReportModel
	{
		public int 编号 { get; set; }
		public string 店铺邮箱 { get; set; }
		public string 店铺ID { get; set; }
		public string 站点 { get; set; }
		public string 状态 { get; set; }
	}
}