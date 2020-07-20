using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class UserByDifferentModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int State { get; set; }
		public string Price { get; set; }
		public string CustomerNumber { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string AddTimes { get; set; }
		public int Type { get; set; }
		public string Link { get; set; }
		public int EstimateNumber { get; set; }
		public int ActualNumber { get; set; }
		public int BackNumber { get; set; }
	}
}