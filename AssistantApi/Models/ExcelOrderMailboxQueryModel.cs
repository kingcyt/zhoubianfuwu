using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantApi.Models
{
	public class ExcelOrderMailboxQueryModel
	{
		public string 国家 { get; set; }
		public string ASIN { get; set; }
		public string 订单号 { get; set; }
	}
}