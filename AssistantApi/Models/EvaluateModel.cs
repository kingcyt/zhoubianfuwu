using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantApi.Models
{
	public class EvaluateModel
	{
		public int UserId { get; set; }
		public int CountryId { get; set; }
		public string ShopId { get; set; }
		public string Asin { get; set; }
		public decimal Price { get; set; }
		public string KeyWord { get; set; }
		public string Productlocation { get; set; }
		public string Remarks { get; set; }
		public TieleAndMeaageModel[] tieleAndMeaageModels { get; set; }
	}
	public class TieleAndMeaageModel
	{
		public string Title { get; set; }
		public string Message { get; set; }
	}
}