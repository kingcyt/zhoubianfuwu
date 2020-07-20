using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class UserByBugQueryModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int CountryId { get; set; }
		public string ServiceName { get; set; }
		public int State { get; set; }
		public string SellerIdOrAsin { get; set; }
		public string RegisteredEmail { get; set; }
		public string Price { get; set; }
		public string SubTime { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string CountryName { get; set; }
		public string CustomerNumber { get; set;  }
	}

	public class ExcelBugQueryModel
	{
		public int 编号 { get; set; }
		public string 服务名称 { get; set; }
		public string 站点 { get; set; }
		public string 注册邮箱 { get; set; }
		public string SellerIDASIN{get;set;}
		public string 状态 { get; set; }
	}
}
