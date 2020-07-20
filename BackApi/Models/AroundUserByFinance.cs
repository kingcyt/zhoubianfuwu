using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class AroundUserByFinance
	{
		public int Id { get; set; }
		public string AccountBalance { get; set; }
		public string Phone { get; set; }
		public string Name { get; set; }
		public int Enable { get; set; }
		public int CustomerNumber { get; set; }
		public string RegistrationTime { get; set; }
		public string LoginTime { get; set; }
	}
}