using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantApi.Models
{
	public class NoticeModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Contenting { get; set; }
		public string AddTime { get; set; }
	}
}