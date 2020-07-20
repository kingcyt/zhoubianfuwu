using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class BackUserRoolModel
	{
		public int Id { get; set; }
		public int[] RoolId { get; set; }
	}

	public class RoleByNavigationBarModel
	{
		public int Id { get; set; }
		public int[] NavigationBarId { get; set; }
	}
}