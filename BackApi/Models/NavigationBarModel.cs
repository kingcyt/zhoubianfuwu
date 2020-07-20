using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class NavigationBarModel
	{
		public int Id;
		public string NavigationName;
		public int Fid;
		public string Path;
		public string Icon;
		public List<NavigationBarModel> childs;
	}

	public class NavigationBar
	{
		public int Id { get; set; }
		public int Pid { get; set; }
		public string NavigationName { get; set; }
		public string Path { get; set; }
		public string Icon { get; set; }
	}

	public class BackModels
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string LoginName { get; set; }
		public string Phone { get; set; }
		public int State { get; set; }
		public string Remarks { get; set; }
		public string RoleName { get; set; }
	}
}