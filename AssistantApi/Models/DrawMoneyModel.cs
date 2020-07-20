using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantApi.Models
{
	public class DrawMoneyModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string RemoveMoenyNumber { get; set; }
		public int? RemoveMoenyStae { get; set; }
		public decimal? RemoveMoeny { get; set; }
		public string RemoveMoenyTime { get; set; }
		public string Bank { get; set; }
		public string BankName { get; set; }
		public string BankAccount { get; set; }
		public string TransactionTime { get; set; }
		public string Remarks { get; set; }
	}

	public class UserByDrawMoneyModel
	{
		public int Id { get; set; }
		public string CustomerId { get; set; }
		public string RemoveMoenyNumber { get; set; }
		public string RemoveMoneyState { get; set; }
		public string RemoveMoeny { get; set; }
		public string RemoveMoenyTime { get; set; }
		public string Bank { get; set; }
		public string BankName { get; set; }
		public string BankAccount { get; set; }
		public string Phone { get; set; }
		public string Name { get; set; }
	}
}