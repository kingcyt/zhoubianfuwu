﻿using AssistantApi.MessageOut;
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
	public class BackDifferentController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-20
		/// message:点赞点踩列表
		/// author:Thomars  
		/// </summary>
		/// <param name="Type"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="Statetime"></param>
		/// <param name="Endtime"></param>
		/// <param name="State"></param>
		/// <param name="keyWord"></param>
		/// <returns></returns>
		[Route("api/BackDifferent/GetBackDifferent"), HttpGet]
		public HttpResponseMessage GetBackDifferent(int Type,int pageNum, int pagesize, string Statetime, string Endtime, int State, string keyWord)
		{
			int count = Sum(Type, Statetime, Endtime, State, keyWord);
			var list = UserByDifferentList(Type, pageNum, pagesize, Statetime, Endtime, State, keyWord);
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

		public int Sum(int Type, string Statetime, string Endtime, int State, string keyWord)
		{
			string where = "";
			string sql = "select count(l.Id) from [dbo].[Different] AS l left join[dbo].[AroundUser] AS a on l.UserId=a.Id  where Type= "+ Type +" and 1=1 ";
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

		public static IList<UserByDifferentModel> UserByDifferentList(int Type, int pageNum, int pagesize, string Statetime, string Endtime, int State, string keyWord)
		{
			int rownum = pagesize * (pageNum - 1);
			string where = "";
			string sql = "select l.*,a.CustomerNumber,a.Name,a.Phone from [dbo].[Different] AS l left join[dbo].[AroundUser] AS a on l.UserId=a.Id  where Type= " + Type + " and 1=1 ";
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
				IList<UserByDifferentModel> list = new List<UserByDifferentModel>();
				while (dr.Read())
				{
					UserByDifferentModel model = new UserByDifferentModel();
					model.Id = (int)dr["Id"];
					model.UserId = (int)dr["UserId"];
					model.State = (int)dr["State"];
					model.Price = dr["Price"].ToString();
					model.CustomerNumber = dr["CustomerNumber"].ToString();
					model.Name = dr["Name"].ToString();
					model.Phone = dr["Phone"].ToString();
					model.AddTimes = dr["AddTimes"].ToString();
					model.Link = dr["Link"].ToString();
					model.Type = (int)dr["Type"];
					model.EstimateNumber = (int)dr["EstimateNumber"];
					model.ActualNumber = (int)dr["ActualNumber"];
					model.BackNumber = dr["BackNumber"].ToString();
					list.Add(model);
				}
				return list;
			}
		}

		/// <summary>
		/// time:2020-7-20
		/// message:修改点赞点踩状态或修改价格
		/// author:Thomars   
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[Route("api/BackDifferent/ChangeUserByDifferentStateOrMoney"), HttpPost]
		public IHttpActionResult ChangeUserByDifferentStateOrMoney(ChangeBadEvaluateModel model)
		{
			int result = 0;
			if (model.State == 1)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.Different.Where(e => e.Id == c).FirstOrDefault();
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
					var query = entities.Different.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.State = 4;
						result = entities.SaveChanges();
					}
				}
			}
			if (model.State == 3)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.Different.Where(e => e.Id == c).FirstOrDefault();
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
		/// time:2020-7-20
		/// message:END点赞点踩
		/// author:Thomars 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackDifferent/EndDifferentTask"), HttpPost]
		public IHttpActionResult EndDifferentTask([FromBody]JObject value)
		{
			try
			{
				int result = 0;
				decimal total = 0;
				int id = (int)value["Id"];
				int number = (int)value["Number"];
				int type = (int)value["Type"];
				int DifferentState = 0;
				string remarks = "";
				var query = entities.Different.Where(e => e.Id == id).FirstOrDefault();
				if (query != null)
				{
					using (TransactionScope scope = new TransactionScope())
					{
						total = Convert.ToDecimal(query.Price) * number;
						query.State = 3;
						query.TotalPrice = total;
						query.BackNumber = number;
						DbEntityEntry entry = entities.Entry(query);
						entry.State = System.Data.Entity.EntityState.Modified;

						//账户扣款
						var customerFinance = entities.AroundUserFinance.Where(t => t.UserId == query.UserId).FirstOrDefault();
						customerFinance.AccountBalance = customerFinance.AccountBalance - query.TotalPrice;
						customerFinance.AccumulatedExpenditure = customerFinance.AccumulatedExpenditure + query.TotalPrice;
						DbEntityEntry entryc = entities.Entry(customerFinance);
						entryc.State = System.Data.Entity.EntityState.Modified;

						//日志
						TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
						if (type == 1)
						{
							DifferentState = 23;
							remarks = " 点赞总额支出";
						}
						else
						{
							DifferentState = 24;
							remarks = " 点踩总额支出";
						}
						AroundUserFinanceLog log = new AroundUserFinanceLog
						{
							BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
							UserId = query.UserId,
							PaymentState = DifferentState,
							TransactionType = 1,  //1扣款,2:充值,3退款
							TransactionTime = DateTime.Now,
							TransactionAmount = query.TotalPrice,
							Remarks = "任务：" + query.Id + remarks

						};
						entities.AroundUserFinanceLog.Add(log);
						result = entities.SaveChanges();
						scope.Complete();
					}
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
	}
}
