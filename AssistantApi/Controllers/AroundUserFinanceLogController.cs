using AssistantApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AssistantApi.Controllers
{
	public class AroundUserFinanceLogController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-3
		/// message:用户收支明细
		/// author：Thomars
		/// </summary>
		/// <param name="Statetime"></param>
		/// <param name="Endtime"></param>
		/// <param name="Userid"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="State"></param>
		/// <returns></returns>
		[Route("api/AroundUserFinanceLog/GetPay"), HttpGet]
		public HttpResponseMessage GetPay(string Statetime, string Endtime, int Userid, int pageNum, int pagesize, int State)
		{
			var a = from u in entities.AroundUser
				join l in entities.AroundUserFinance on u.Id equals l.UserId
				where u.Id == Userid
				select new { l.AccumulatedIncome, l.AccumulatedExpenditure, l.AccountBalance };
			var aj = JsonConvert.SerializeObject(a);
			int count = Sum(Statetime, Endtime, Userid, State);
			var list = GetPayList(Statetime, Endtime, Userid, pageNum, pagesize, State);
			var json = JsonConvert.SerializeObject(list);
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append("\"total\"");
			sb.Append(":");
			sb.Append("\"" + count + "\"");
			sb.Append(",");
			sb.Append("\"Account\"");
			sb.Append(":");
			sb.Append(aj);
			sb.Append(",");
			sb.Append("\"list\"");
			sb.Append(":");
			sb.Append(json);
			sb.Append("}");
			json = sb.ToString();
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}

		public int Sum(string Statetime, string Endtime, int Userid, int State)
		{
			string sql = " select count(l.Id) AS num from [dbo].[AroundUserFinanceLog] AS l where 1=1 ";
			string where = "";

			if (string.IsNullOrWhiteSpace(Statetime) == false && string.IsNullOrWhiteSpace(Endtime) == false)
			{
				where += " and  l.TransactionTime >='" + Statetime + "' and l.TransactionTime<='" + Endtime + "' and l.UserId =" + Userid;
			}
			if (State > 0)
			{
				where += " and  l.UserId =" + Userid + " and l.TransactionType=" + State;
			}
			else
			{
				where += " and l.UserId =" + Userid;
			}
			if (string.IsNullOrWhiteSpace(where) == false)
			{
				sql += where;
			}
			string connstring = entities.Database.Connection.ConnectionString;
			using (SqlConnection conn = new SqlConnection(connstring))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				int result = int.Parse(cmd.ExecuteScalar().ToString());
				return result;
			}
		}

		public static IList<FinanceLogModel> GetPayList(string Statetime, string Endtime, int Userid, int pageNum, int pagesize, int State)
		{
			int rownum = pagesize * (pageNum - 1);
			string where = "";
			string sql = "select l.*  from [AroundUserFinanceLog] AS l left join[dbo].[AroundUserFinance] AS c on l.UserId=c.UserId where 1=1 ";

			if (string.IsNullOrWhiteSpace(Statetime) == false && string.IsNullOrWhiteSpace(Endtime) == false)
			{
				where += " and l.TransactionTime >='" + Statetime + "' and l.TransactionTime<='" + Endtime + "' and l.Userid=" + Userid;
			}
			if (State > 0)
			{
				where += " and l.Userid=" + Userid + " and l.TransactionType=" + State;
			}
			else
			{
				where += " and l.Userid=" + Userid;
			}
			if (string.IsNullOrWhiteSpace(where) == false)
			{
				sql += where;
			}
			sql += " order by Id desc  OFFSET " + rownum + " ROWS FETCH NEXT " + pagesize + " ROWS ONLY ";
			string connstring = entities.Database.Connection.ConnectionString;
			using (SqlConnection conn = new SqlConnection(connstring))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				SqlDataReader dr = cmd.ExecuteReader();
				IList<FinanceLogModel> list = new List<FinanceLogModel>();
				while (dr.Read())
				{
					FinanceLogModel model = new FinanceLogModel();
					model.Id = (int)dr["Id"];
					model.BusinessNumber = dr["BusinessNumber"].ToString();
					model.PaymentState = (int)dr["PaymentState"];
					model.TransactionType = (int)dr["TransactionType"];
					model.TransactionTime = dr["TransactionTime"].ToString();
					model.TransactionAmount = dr["TransactionAmount"].ToString();
					model.Remarks = dr["Remarks"].ToString();
					list.Add(model);
				}
				return list;
			}
		}


	}
}
