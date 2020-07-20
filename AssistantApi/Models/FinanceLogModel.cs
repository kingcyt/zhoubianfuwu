using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantApi.Models
{
	public class FinanceLogModel
	{
		public int Id { get; set; }
		public string BusinessNumber { get; set; }
		public int PaymentState { get; set; }
		public int TransactionType { get; set; }
		public string TransactionAmount { get; set; }
		public string TransactionTime { get; set; }
		public string Remarks { get; set; }
		
	}
}