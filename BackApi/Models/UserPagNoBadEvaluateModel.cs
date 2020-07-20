using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class UserPagNoBadEvaluateModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int CountryId { get; set; }
		public string Price { get; set; }
		public string ASIN { get; set; }
		public string ListingPag { get; set; }
		public string BuyTime { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string CountryName { get; set; }
		public int State { get; set; }
		public string CustomerNumber { get; set; }
	}

	public class ExcelPagNoBadEvaluateModel
	{
		public int 编号 { get; set; }
		public string ASIN { get; set; }
		public string 站点 { get; set; }
		public string 状态 { get; set; }

	}
}