using BackApi.MessageOut;
using BackApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BackApi.Controllers
{
	public class BackAroundUserController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-2
		/// message: 展示客户列表
		/// author：Thomars
		/// </summary>
		/// <param name="pagesize"></param>
		/// <param name="pageNum"></param>
		/// <param name="Name">客户姓名</param>
		/// <returns></returns>
		[Route("api/BackAroundUser/GetAroundUser"), HttpGet]
		public HttpResponseMessage GetAroundUser(int pagesize, int pageNum, string Name, string Types)
		{

			var json = string.Empty;
			var qu = (from u in entities.AroundUser
				  join ul in entities.AroundUserFinance on u.Id equals ul.UserId //into ab
												 // from us in ab.DefaultIfEmpty()
				  select new { ul.AccountBalance, u });

			if (string.IsNullOrWhiteSpace(Name) == false)
			{
				if (Types == "-1")
				{
					qu = qu.Where(e => e.u.Name.Contains(Name) && e.AccountBalance < 0);
				}
				else if (Types == "0")
				{
					qu = qu.Where(e => e.u.Name.Contains(Name));
				}
				else
				{
					qu = qu.Where(e => e.u.Name.Contains(Name) && e.AccountBalance > 0);
				}
			}
			else
			{
				if (Types == "-1")
				{
					qu = qu.Where(e => e.AccountBalance < 0);
				}
				else if (Types == "1")
				{
					qu = qu.Where(e => e.AccountBalance > 0);
				}
			}

			int count = qu.Count();
			var row = qu.OrderByDescending(i => i.u.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e => new
			{
				Id = e.u.Id,
				Name = e.u.Name,
				Phone = e.u.Phone,
				CustomerNumber = e.u.CustomerNumber,
				RegistrationTime = e.u.RegistrationTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				LoginTime = e.u.LoginTime == null ? "" : e.u.LoginTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				Enable = e.u.Enable,
				AccountBalance = e.AccountBalance,
			}).ToList();

			json = JsonConvert.SerializeObject(row);
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append("\"total\"");
			sb.Append(":");
			sb.Append("\"" + count + "\"");
			sb.Append(",");
			sb.Append("\"list\"");
			sb.Append(":");
			sb.Append(json);
			sb.Append("}");
			json = sb.ToString();
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}

		/// <summary>
		/// time: 2020-7-2
		/// message: 客户启用禁用
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackAroundUser/ChangeAroundUserState"), HttpPost]
		public IHttpActionResult ChangeAroundUserState([FromBody]JObject value)
		{
			int id = (int)value["Id"];
			int state = (int)value["State"];
			int result = 0;
			var aid = entities.AroundUser.Where(e => e.Id == id).FirstOrDefault();
			if (aid != null)
			{
				aid.Enable = state;
				DbEntityEntry entry = entities.Entry(aid);
				entry.State = System.Data.Entity.EntityState.Modified;
				result = entities.SaveChanges();
			}
			if (result > 0)
			{
				return Ok(Respone.Success("修改成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		/// <summary>
		/// time: 2020-7-2
		///  message: 客户充值扣款
		///  author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackAroundUser/ChangeAccountbalance"), HttpPost]
		public IHttpActionResult ChangeAccountbalance([FromBody]JObject value)
		{
			int result = 0;
			int sum = 0;
			int id = (int)value["Id"];
			decimal accountbalance = (decimal)value["Accountbalance"];
			string state = value["State"].ToString();

			var query = entities.AroundUserFinance.Where(c => c.UserId == id).FirstOrDefault();

			if (query != null)
			{
				if (state == "0")  //扣款
				{
					query.AccountBalance = query.AccountBalance - accountbalance;
					query.AccumulatedExpenditure = query.AccumulatedExpenditure + accountbalance;
					DbEntityEntry entry = entities.Entry(query);
					entry.State = System.Data.Entity.EntityState.Modified;
					result = entities.SaveChanges();

					if (result > 0)
					{
						TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
						AroundUserFinanceLog log = new AroundUserFinanceLog
						{
							BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
							UserId = query.UserId,
							PaymentState = 2,   //具体状态
							TransactionType = 1, //1扣款,2:充值,3退款
							TransactionTime = DateTime.Now,
							TransactionAmount = accountbalance,
							Remarks = "扣款支出"
						};
						entities.AroundUserFinanceLog.Add(log);
						sum = entities.SaveChanges();
					}
				}
				else  //充值
				{
					query.AccountBalance = query.AccountBalance + accountbalance;
					query.AccumulatedIncome = query.AccumulatedIncome + accountbalance;
					DbEntityEntry entry = entities.Entry(query);
					entry.State = System.Data.Entity.EntityState.Modified;
					result = entities.SaveChanges();
					if (result > 0)
					{
						TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
						AroundUserFinanceLog log = new AroundUserFinanceLog
						{
							BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
							UserId = query.UserId,
							PaymentState = 1,
							TransactionType = 2,
							TransactionTime = DateTime.Now,
							TransactionAmount = accountbalance,
							Remarks = "充值收入"
						};
						entities.AroundUserFinanceLog.Add(log);
						sum = entities.SaveChanges();
					}
				}
			}

			if (result > 0 && sum > 0)
			{
				return Ok(Respone.Success("修改成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		/// <summary>
		/// time: 2020-7-2
		/// message: 客户收支明细
		/// author：Thomars
		/// </summary>
		/// <param name="State">收支类型</param>
		/// <param name="KeyWord">搜索</param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="UserId">用户Id</param>
		/// <returns></returns>
		[Route("api/BackAroundUser/GetAroundUserByFinanceLog"), HttpGet]
		public HttpResponseMessage GetAroundUserByFinanceLog(int State, string KeyWord, int pageNum, int pagesize, int UserId)
		{
			int count = 0;
			if (string.IsNullOrWhiteSpace(KeyWord) == false && State > 0)
			{
				count = entities.AroundUserFinanceLog.Where(e => e.UserId == UserId && e.TransactionType == State && (e.BusinessNumber.Contains(KeyWord) || e.Remarks.Contains(KeyWord))).Count();
			}
			else if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				count = entities.AroundUserFinanceLog.Where(e => e.UserId == UserId && (e.BusinessNumber.Contains(KeyWord) || e.Remarks.Contains(KeyWord))).Count();
			}
			else if (State > 0)
			{
				count = entities.AroundUserFinanceLog.Where(e => e.UserId == UserId && e.TransactionType == State).Count();
			}
			else
			{
				count = entities.AroundUserFinanceLog.Where(e => e.UserId == UserId).Count();
			}
			var list = GetAroundUserByFinanceLogLog(State, KeyWord, pageNum, pagesize, UserId);
			var json = JsonConvert.SerializeObject(list);

			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append("\"total\"");
			sb.Append(":");
			sb.Append("\"" + count + "\"");
			sb.Append(",");
			sb.Append("\"list\"");
			sb.Append(":");
			sb.Append(json);
			sb.Append("}");
			json = sb.ToString();
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}

		public static IList<AroundUserByFinanceLog> GetAroundUserByFinanceLogLog(int State, string KeyWord, int pageNum, int pagesize, int UserId)
		{
			int rownum = pagesize * (pageNum - 1);
			string where = "";
			string sql = "  select al.*,af.AccountBalance ,af.AccumulatedExpenditure,af.AccumulatedIncome   from AroundUser as au left join AroundUserFinance as af on au.Id = af.UserId left join AroundUserFinanceLog as al on au.Id = al.UserId where 1=1 ";
			if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				where += " and (al.BusinessNumber like '%" + KeyWord + "%' or al.Remarks like '%" + KeyWord + "%')  and  au.Id=" + UserId;
			}
			if (State > 0)
			{
				where += " and al.TransactionType =" + State + " and  au.Id=" + UserId;
			}
			else
			{
				where += " and au.Id=" + UserId;
			}

			if (string.IsNullOrWhiteSpace(where) == false)
			{
				sql +=  where;
			}

			sql += " order by Id desc  OFFSET " + rownum + " ROWS FETCH NEXT " + pagesize + " ROWS ONLY ";
			string connstring = entities.Database.Connection.ConnectionString;
			using (SqlConnection conn = new SqlConnection(connstring))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				SqlDataReader dr = cmd.ExecuteReader();
				IList<AroundUserByFinanceLog> list = new List<AroundUserByFinanceLog>();
				while (dr.Read())
				{
					AroundUserByFinanceLog model = new AroundUserByFinanceLog();
					model.BusinessNumber = dr["BusinessNumber"].ToString();
					model.PaymentState = (int)dr["PaymentState"];
					model.TransactionType = (int)dr["TransactionType"];
					model.TransactionAmount = dr["TransactionAmount"].ToString();
					model.TransactionTime = dr["TransactionTime"].ToString();
					model.Remarks = dr["Remarks"].ToString();
					model.AccumulatedExpenditure = dr["AccumulatedExpenditure"].ToString();
					model.AccountBalance = dr["AccountBalance"].ToString();
					model.AccumulatedIncome = dr["AccumulatedIncome"].ToString();
					list.Add(model);
				}
				return list;
			}
		}
	}
}
