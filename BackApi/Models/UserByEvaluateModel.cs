using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class UserByEvaluateModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int State { get; set; }
		public string Price { get; set; }
		public string CustomerNumber { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string CountryName { get; set; }
		public string ShopId { get; set; }
		public string Images { get; set; }
		public string KeyWord { get; set; }
		public string AddTimes { get; set; }
		public string Remarks { get; set; }
		public string ASIN { get; set; }
		public string Productlocation { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
	}
	public class EvaluateTaskModel
	{
		public int[] Id { get; set; }
		public int State { get; set; }
	}
}