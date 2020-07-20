using AssistantApi.MessageOut;
using BackApi.Common;
using BackApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace BackApi.Controllers
{
	public class BackBugQueryController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-9
		/// message:死因查询列表
		/// author:Thomars 
		/// </summary>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="Statetime"></param>
		/// <param name="Endtime"></param>
		/// <param name="State"></param>
		/// <param name="KeyWord"></param>
		/// <param name="CountryId"></param>
		/// <returns></returns>
		[Route("api/BackBugQuery/GetUserByBugQuery"), HttpGet]
		public HttpResponseMessage GetUserByBugQuery(int pageNum, int pagesize, string Statetime, string Endtime, int State, string KeyWord, int CountryId)
		{
			int count = Sum(Statetime, Endtime, State, KeyWord, CountryId);
			var list = UserByBugQueryList(pageNum, pagesize, Statetime, Endtime, State, KeyWord, CountryId);
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
		public int Sum(string Statetime, string Endtime, int State, string KeyWord, int CountryId)
		{
			string where = "";
			string sql = " select count(k.Id)  from BugQuery AS k left join[dbo].[AroundUser] AS a on k.UserId=a.Id left join[dbo].[Country] as c on k.CountryId= c.Id where 1=1 ";

			if (State > 0)
			{
				where += " and k.State = " + State;
			}
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				where += " and k.SubTime >='" + Statetime + "' and k.SubTime<= '" + Endtime + "'";
			}
			if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				where += "  and (a.Name like '%" + KeyWord + "%' or a.Phone like '%" + KeyWord + "%')";
			}
			if (CountryId > 0)
			{
				where += " and k.CountryId=" + CountryId;
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

		public static IList<UserByBugQueryModel> UserByBugQueryList(int pageNum, int pagesize, string Statetime, string Endtime, int State, string KeyWord, int CountryId)
		{
			int rownum = pagesize * (pageNum - 1);
			string where = "";
			string sql = " select k.*,a.Name,a.CustomerNumber,a.Phone,c.CountryName   from BugQuery AS k left join[dbo].[AroundUser] AS a on k.UserId=a.Id left join[dbo].[Country] as c on k.CountryId= c.Id where 1=1 ";
			if (State > 0)
			{
				where += " and k.State = " + State;
			}
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				where += " and k.SubTime >='" + Statetime + "' and k.SubTime<= '" + Endtime + "'";
			}
			if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				where += "  and (a.Name like '%" + KeyWord + "%' or a.Phone like '%" + KeyWord + "%')";
			}
			if (CountryId > 0)
			{
				where += " and k.CountryId=" + CountryId;
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
				IList<UserByBugQueryModel> list = new List<UserByBugQueryModel>();
				while (dr.Read())
				{
					UserByBugQueryModel model = new UserByBugQueryModel();
					model.Id = (int)dr["Id"];
					model.UserId = (int)dr["UserId"];
					model.CountryId = (int)dr["CountryId"];
					model.ServiceName = dr["ServiceName"].ToString();
					model.State = (int)dr["State"];
					model.SellerIdOrAsin = dr["SellerIdOrAsin"].ToString();
					model.RegisteredEmail = dr["RegisteredEmail"].ToString();
					model.Price = dr["Price"].ToString();
					model.SubTime = dr["SubTime"].ToString();
					model.Name = dr["Name"].ToString();
					model.Phone = dr["Phone"].ToString();
					model.CountryName = dr["CountryName"].ToString();
					model.CustomerNumber = dr["CustomerNumber"].ToString();
					list.Add(model);
				}
				return list;
			}
		}

		/// <summary>
		/// time:2020-7-9
		/// message:死因查询改价格或者改状态
		/// author:Thomars 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[Route("api/BackBugQuery/ChangeUserByBugQueryStateOrMoney"), HttpPost]
		public IHttpActionResult ChangeUserByBugQueryStateOrMoney(ChangeBadEvaluateModel model)
		{
			int result = 0;
			if (model.State == 1)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.BugQuery.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.State = 2;
						DbEntityEntry entry = entities.Entry(query);
						entry.State = System.Data.Entity.EntityState.Modified;
						result = entities.SaveChanges();

						var customerFinance = entities.AroundUserFinance.Where(t => t.UserId == query.UserId).FirstOrDefault();
						customerFinance.AccountBalance = customerFinance.AccountBalance - query.Price;
						customerFinance.AccumulatedExpenditure = customerFinance.AccumulatedExpenditure + query.Price;
						DbEntityEntry entryc = entities.Entry(customerFinance);
						entryc.State = System.Data.Entity.EntityState.Modified;
						entities.SaveChanges();

						//记录流水
						TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
						AroundUserFinanceLog log = new AroundUserFinanceLog
						{
							BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
							UserId = query.UserId,
							PaymentState = 9,
							TransactionType = 1,
							TransactionTime = DateTime.Now,
							TransactionAmount = query.Price,
							Remarks = "任务：" + query.Id + " 死因查询支出"

						};
						entities.AroundUserFinanceLog.Add(log);
						result = entities.SaveChanges();
					}
				}
			}
			else if (model.State == 2)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.BugQuery.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.State = 5;
						DbEntityEntry entry = entities.Entry(query);
						entry.State = System.Data.Entity.EntityState.Modified;
						result = entities.SaveChanges();
					}
				}
			}
			else
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.BugQuery.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.Price = model.Price;
						DbEntityEntry entry = entities.Entry(query);
						entry.State = System.Data.Entity.EntityState.Modified;
						result = entities.SaveChanges();
					}
				}
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
		/// time:2020-7-10
		/// message:死因查询Excel导入
		/// author:Thomars 
		/// </summary>
		/// <returns></returns>
		[Route("api/BackBugQuery/ExcelBugQuery"), HttpPost]
		public IHttpActionResult ExcelBugQuery()
		{
			int s = 0;
			int sum = 0;
			try
			{
				var filelist = HttpContext.Current.Request.Files;
				var bad = new List<ExcelBugQueryModel>();
				if (filelist.Count > 0)
				{
					for (var i = 0; i < filelist.Count; i++)
					{
						var file = filelist[i];
						var dataTable = ExcelHelp.ExcelToTableForXLSX(file.InputStream);//excel转成datatable
						bad = dataTable.ToDataList<ExcelBugQueryModel>();//datatable转成list
					}
				}
				sum = bad.Count();
				using (TransactionScope scope = new TransactionScope())
				{
					foreach (var item in bad)
					{
						var bid = entities.BugQuery.Where(b => b.Id == item.编号).FirstOrDefault();
						if (item.状态.Equals("0"))
						{
							if (bid != null)
							{
								bid.State = 4;
								DbEntityEntry entry = entities.Entry(bid);
								entry.State = System.Data.Entity.EntityState.Modified;
								//entities.SaveChanges();

								//退款
								var customerFinance = entities.AroundUserFinance.Where(t => t.UserId == bid.UserId).FirstOrDefault();
								customerFinance.AccountBalance = customerFinance.AccountBalance + bid.Price;
								customerFinance.AccumulatedExpenditure = customerFinance.AccumulatedExpenditure - bid.Price;
								DbEntityEntry entryc = entities.Entry(customerFinance);
								entryc.State = System.Data.Entity.EntityState.Modified;
								//entities.SaveChanges();

								//记录流水
								TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
								AroundUserFinanceLog log = new AroundUserFinanceLog
								{
									BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
									UserId = bid.UserId,
									PaymentState = 16,
									TransactionType = 3,
									TransactionTime = DateTime.Now,
									TransactionAmount = bid.Price,
									Remarks = "任务：" + bid.Id + " 死因查询退款"

								};
								entities.AroundUserFinanceLog.Add(log);
								//entities.SaveChanges();
							}
						}
						else if (item.状态.Equals("1"))
						{

							if (bid != null)
							{
								bid.State = 3;
								DbEntityEntry entry = entities.Entry(bid);
								entry.State = System.Data.Entity.EntityState.Modified;
								//entities.SaveChanges();
							}
						}
						else
						{
							return Ok(Respone.No("上传失败"));
						}
						s++;
					}
					entities.SaveChanges();
					scope.Complete();
				}
				if (s == sum)
				{
					return Ok(Respone.Success("导入成功"));
				}
				else
				{
					return Ok(Respone.No("发生了点问题，请稍后再试"));
				}
			}
			catch (Exception ex)
			{
				return Ok(ex.ToString());
			}
		}
	}
}
