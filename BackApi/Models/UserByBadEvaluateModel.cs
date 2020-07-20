using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class UserByBadEvaluateModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int State { get; set; }
		public string Price { get; set; }
		public string SubTime { get; set; }
		public string CustomerNumber { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string ReviewLink { get; set; }
	}

	public class ChangeBadEvaluateModel
	{
		public int[] Id { get; set; }
		public int State { get; set; }
		public decimal Price { get; set; }
	}
}