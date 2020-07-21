using AssistantApi.MessageOut;
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
using System.Transactions;
using System.Web.Http;

namespace BackApi.Controllers
{
	public class BackEvaluateController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-21
		/// message:上评列表
		/// author:Thomars  
		/// </summary>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="Statetime"></param>
		/// <param name="Endtime"></param>
		/// <param name="State"></param>
		/// <param name="keyWord"></param>
		/// <returns></returns>
		[Route("api/BackEvaluate/GetBackEvaluate"), HttpGet]
		public HttpResponseMessage GetBackEvaluate(int pageNum, int pagesize, string Statetime, string Endtime, int State, string keyWord)
		{
			int count = Sum(Statetime, Endtime, State, keyWord);
			var list = UserByEvaluateList(pageNum, pagesize, Statetime, Endtime, State, keyWord);
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

		public int Sum(string Statetime, string Endtime, int State, string keyWord)
		{
			string where = "";
			string sql = "select count(l.Id) from [dbo].[Evaluate] AS l left join[dbo].[AroundUser] AS a on l.UserId=a.Id left join[dbo].[Country] AS c on l.CountryId= c.Id where 1=1 ";
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				where += "  and l.AddTimes >='" + Statetime + "' and l.AddTimes<= '" + Endtime + "'";
			}
			if (State > 0)
			{
				where += "  and l.State=" + State;
			}
			if (string.IsNullOrWhiteSpace(keyWord) == false)
			{
				where += " and  (a.Name like '%" + keyWord + "%' or a.Phone like '%" + keyWord + "%')";
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

		public static IList<UserByEvaluateModel> UserByEvaluateList(int pageNum, int pagesize, string Statetime, string Endtime, int State, string keyWord)
		{
			int rownum = pagesize * (pageNum - 1);
			string where = "";
			string sql = "select l.*,a.CustomerNumber,a.Name,a.Phone,c.CountryName from [dbo].[Evaluate] AS l left join[dbo].[AroundUser] AS a on l.UserId=a.Id left join[dbo].[Country] AS c on l.CountryId= c.Id where 1=1 ";
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				where += "  and l.AddTimes >='" + Statetime + "' and l.AddTimes<= '" + Endtime + "'";
			}
			if (State > 0)
			{
				where += "  and l.State=" + State;
			}
			if (string.IsNullOrWhiteSpace(keyWord) == false)
			{
				where += " and  (a.Name like '%" + keyWord + "%' or a.Phone like '%" + keyWord + "%')";
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
				IList<UserByEvaluateModel> list = new List<UserByEvaluateModel>();
				while (dr.Read())
				{
					UserByEvaluateModel model = new UserByEvaluateModel();
					model.Id = (int)dr["Id"];
					model.UserId = (int)dr["UserId"];
					model.State = (int)dr["State"];
					model.Price = dr["Price"].ToString();
					model.CustomerNumber = dr["CustomerNumber"].ToString();
					model.Name = dr["Name"].ToString();
					model.Phone = dr["Phone"].ToString();
					model.CountryName = dr["CountryName"].ToString();
					model.ShopId = dr["ShopId"].ToString();
					model.ASIN = dr["ASIN"].ToString();
					model.Images = dr["Images"].ToString();
					model.KeyWord = dr["KeyWord"].ToString();
					model.Productlocation = dr["Productlocation"].ToString();
					model.AddTimes = dr["AddTimes"].ToString();
					model.Remarks = dr["Remarks"].ToString();
					model.Title = dr["Title"].ToString();
					model.Message = dr["Message"].ToString();
					list.Add(model);
				}
				return list;
			}
		}

		/// <summary>
		/// time:2020-7-21
		/// message:修改上评状态或修改价格
		/// author:Thomars 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[Route("api/BackEvaluate/ChangeUserByEvaluateStateOrMoney"), HttpPost]
		public IHttpActionResult ChangeUserByEvaluateStateOrMoney(ChangeBadEvaluateModel model)
		{
			int result = 0;
			if (model.State == 1)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.Evaluate.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.State = 2;
						result = entities.SaveChanges();
					}
				}
			}
			if (model.State == 2)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.Evaluate.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.State = 5;
						result = entities.SaveChanges();
					}
				}
			}
			if (model.State == 3)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.Evaluate.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.Price = model.Price;
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
		/// time:2020-7-21
		/// message:End上评
		/// author:Thomars  
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		[Route("api/BackEvaluate/EndEvaluateTask"), HttpPost]
		public IHttpActionResult EndEvaluateTask(EvaluateTaskModel task)
		{
			try
			{
				int result = 0;
				using (TransactionScope scope = new TransactionScope())
				{
					foreach (var item in task.Id)
					{

						var query = entities.Evaluate.Where(e => e.Id == item).FirstOrDefault();
						if (query != null)
						{
							query.State = 3;
							DbEntityEntry entry = entities.Entry(query);
							entry.State = System.Data.Entity.EntityState.Modified;

							//账户扣款
							var customerFinance = entities.AroundUserFinance.Where(t => t.UserId == query.UserId).FirstOrDefault();
							customerFinance.AccountBalance = customerFinance.AccountBalance - query.Price;
							customerFinance.AccumulatedExpenditure = customerFinance.AccumulatedExpenditure + query.Price;
							DbEntityEntry entryc = entities.Entry(customerFinance);
							entryc.State = System.Data.Entity.EntityState.Modified;

							//日志
							TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
							AroundUserFinanceLog log = new AroundUserFinanceLog
							{
								BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
								UserId = query.UserId,
								PaymentState = 25,
								TransactionType = 1,  //1扣款,2:充值,3退款
								TransactionTime = DateTime.Now,
								TransactionAmount = query.Price,
								Remarks = "任务：" + query.Id + " 上评总额支出"
							};
							entities.AroundUserFinanceLog.Add(log);
						}
					}
					 result= entities.SaveChanges();
					scope.Complete();
				}
				if (result > 0)
				{
					return Ok(Respone.Success("任务已完成"));
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

		/// <summary>
		/// time:2020-7-21
		/// message:上评图片地址
		/// author:Thomars 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackEvaluate/UpdateImages"), HttpPost]
		public IHttpActionResult UpdateImages([FromBody]JObject value)
		{
			int result = 0;
			int id = (int)value["Id"];
			string files = value["Images"].ToString();
			var query = entities.Evaluate.Where(e => e.Id == id).FirstOrDefault();
			if (query != null)
			{
				query.Images = files;
				result = entities.SaveChanges();
			}
			if (result > 0)
			{
				return Ok(Respone.Success("图片上传成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

	}
}
