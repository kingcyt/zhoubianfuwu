using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackApi.Models
{
	public class AroundUserByFinanceLog
	{
		
		//public int UserId { get; set; }
		public string BusinessNumber { get; set; }
		public int PaymentState { get; set; }
		public int TransactionType { get; set; }
		public string TransactionAmount { get; set; }
		public string TransactionTime { get; set; }
		public string Remarks { get; set; }
		public string AccumulatedExpenditure { get; set; }
		public string AccountBalance { get; set; }
		public string AccumulatedIncome { get; set; }
	}
}