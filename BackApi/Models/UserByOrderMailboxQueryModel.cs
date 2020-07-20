using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class UserByOrderMailboxQueryModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int State { get; set; }
		public string Price { get; set; }
		public string SubTime { get; set; }
		public string CustomerNumber { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string OrderNumber { get; set; }
		public string ASIN { get; set; }
		public string ConsigneeName { get; set; }
		public string BuyerName { get; set; }
		public string BuyerEmail { get; set; }
		public string CountryName { get; set; }
	}

	public class ExcelUserByOrderMailboxQuery
	{
		public int 编号 { get; set; }
		public string 国家 { get; set; }
		public string 订单号 { get; set; }
		public string ASIN { get; set; }
		public string 买家姓名 { get; set; }
		public string 买家邮箱 { get; set; }
		public string 状态 { get; set; }
	}
}