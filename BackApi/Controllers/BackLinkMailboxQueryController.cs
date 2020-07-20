using AssistantApi.Common;
using AssistantApi.MessageOut;
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
    public class BackLinkMailboxQueryController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-14
		/// message:链接查邮箱列表
		/// author:Thomars
		/// </summary>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="Statetime"></param>
		/// <param name="Endtime"></param>
		/// <param name="State"></param>
		/// <param name="KeyWord"></param>
		/// <returns></returns>
		[Route("api/BackLinkMailboxQuery/GetUserByLinkMailboxQuery"), HttpGet]
		public HttpResponseMessage GetUserByLinkMailboxQuery(int pageNum, int pagesize, string Statetime, string Endtime, int State, string KeyWord)
		{
			int count = Sum(Statetime, Endtime, State, KeyWord);
			var list = UserByLinkMailboxQueryList(pageNum, pagesize, Statetime, Endtime, State, KeyWord);
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

		public int Sum(string Statetime, string Endtime, int State, string KeyWord)
		{
			string where = "";
			string sql = " select count(l.Id) from [dbo].[LinkMailboxQuery] AS l left join[dbo].[AroundUser] AS a on l.UserId=a.Id left join[dbo].[Country] AS c on l.CountryId= c.Id where 1=1 ";
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				where += "  and l.SubTime >='" + Statetime + "' and l.SubTime<= '" + Endtime + "'";
			}
			if (State > 0)
			{
				where += "  and l.State=" + State;
			}
			if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				where += " and  (a.Name like '%" + KeyWord + "%' or a.Phone like '%" + KeyWord + "%')";
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
		public static IList<UserByLinkMailboxQueryModel> UserByLinkMailboxQueryList(int pageNum, int pagesize, string Statetime, string Endtime, int State, string KeyWord)
		{
			int rownum = pagesize * (pageNum - 1);
			string where = "";
			string sql = "select l.*,a.CustomerNumber,a.Name,a.Phone,c.CountryName from [dbo].[LinkMailboxQuery] AS l left join[dbo].[AroundUser] AS a on l.UserId=a.Id left join[dbo].[Country] AS c on l.CountryId= c.Id where 1=1 ";
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				where += "  and l.SubTime >='" + Statetime + "' and l.SubTime<= '" + Endtime + "'";
			}
			if (State > 0)
			{
				where += "  and l.State=" + State;
			}
			if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				where += " and  (a.Name like '%" + KeyWord + "%' or a.Phone like '%" + KeyWord + "%')";
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
				IList<UserByLinkMailboxQueryModel> list = new List<UserByLinkMailboxQueryModel>();
				while (dr.Read())
				{
					UserByLinkMailboxQueryModel model = new UserByLinkMailboxQueryModel();
					model.Id = (int)dr["Id"];
					model.UserId = (int)dr["UserId"];
					model.State = (int)dr["State"];
					model.Price = dr["Price"].ToString();
					model.SubTime = dr["SubTime"].ToString();
					model.CustomerNumber = dr["CustomerNumber"].ToString();
					model.Name = dr["Name"].ToString();
					model.Phone = dr["Phone"].ToString();
					model.ReviewLink = dr["ReviewLink"].ToString();
					model.Grade = dr["Grade"].ToString();
					model.Comments = dr["Comments"].ToString();
					model.SellerName = dr["SellerName"].ToString();
					model.BuyerName = dr["BuyerName"].ToString();
					model.BuyerEmail = dr["BuyerEmail"].ToString();
					model.CountryName = dr["CountryName"].ToString();
					list.Add(model);
				}
				return list;
			}
		}

		/// <summary>
		/// time:2020-7-14
		/// message:修改链接查邮箱状态或修改价格
		/// author:Thomars
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[Route("api/BackLinkMailboxQuery/ChangeUserByLinkMailboxQueryStateOrMoney"), HttpPost]
		public IHttpActionResult ChangeUserByLinkMailboxQueryStateOrMoney(ChangeBadEvaluateModel model)
		{
			int result = 0;
			if (model.State == 1)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.LinkMailboxQuery.Where(e => e.Id == c).FirstOrDefault();
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
							PaymentState = 17,
							TransactionType = 1,
							TransactionTime = DateTime.Now,
							TransactionAmount = query.Price,
							Remarks = "任务：" + query.Id + " 链接查邮箱支出"

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
					var query = entities.LinkMailboxQuery.Where(e => e.Id == c).FirstOrDefault();
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
					var query = entities.LinkMailboxQuery.Where(e => e.Id == c).FirstOrDefault();
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
		/// time:2020-7-15
		/// message:链接查邮箱Excel导入
		/// author:Thomars  
		/// </summary>
		/// <returns></returns>
		[Route("api/BackLinkMailboxQuery/ExcelUserByLinkMailboxQuery"), HttpPost]
		public IHttpActionResult ExcelUserByLinkMailboxQuery()
		{
			int sum = 0;
			int s = 0;

			var filelist = HttpContext.Current.Request.Files;
			var bad = new List<ExcelUserByLinkMailboxQuery>();
			if (filelist.Count > 0)
			{
				for (var i = 0; i < filelist.Count; i++)
				{
					var file = filelist[i];
					var dataTable = ExcelHelp.ExcelToTableForXLSX(file.InputStream);//excel转成datatable
					bad = dataTable.ToDataList<ExcelUserByLinkMailboxQuery>();//datatable转成list
				}
			}
			using (TransactionScope scope = new TransactionScope())
			{
				try
				{
					sum = bad.Count();
					foreach (var item in bad)
					{

						var bid = entities.LinkMailboxQuery.Where(b => b.Id == item.编号).FirstOrDefault();
						if (item.状态.Equals("1"))
						{
							bid.State = 3;
							bid.Grade = item.星级;
							bid.Comments = item.评论内容;
							bid.BuyerEmail = item.买家邮箱;
							bid.BuyerName = item.买家姓名;
							DbEntityEntry entry = entities.Entry(bid);
							entry.State = System.Data.Entity.EntityState.Modified;
							// link = entities.SaveChanges()<0;

							var customerFinance = entities.AroundUserFinance.Where(t => t.UserId == bid.UserId).FirstOrDefault();
							customerFinance.AccountBalance = customerFinance.AccountBalance - bid.Price;
							customerFinance.AccumulatedExpenditure = customerFinance.AccumulatedExpenditure + bid.Price;
							DbEntityEntry entryc = entities.Entry(customerFinance);
							entryc.State = System.Data.Entity.EntityState.Modified;
							//Finance= entities.SaveChanges()<0;


							//记录流水
							TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
							AroundUserFinanceLog log = new AroundUserFinanceLog
							{
								BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
								UserId = bid.UserId,
								PaymentState = 18,
								TransactionType = 1,  //1扣款,2:充值,3退款
								TransactionTime = DateTime.Now,
								TransactionAmount = bid.Price,
								Remarks = "任务：" + bid.Id + " 链接查邮箱退款"

							};
							entities.AroundUserFinanceLog.Add(log);
						}
						else if (item.状态.Equals("0"))
						{
							bid.State = 4;
							DbEntityEntry entry = entities.Entry(bid);
							entry.State = System.Data.Entity.EntityState.Modified;
							//link = entities.SaveChanges() < 0;


							//FinanceLog=entities.SaveChanges()<0;
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
				catch (Exception ex)
				{
					return Ok(Respone.No(ex.ToString()));
				}
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
	}
}
