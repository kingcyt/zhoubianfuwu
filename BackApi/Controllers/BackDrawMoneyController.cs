using AssistantApi.MessageOut;
using AssistantApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BackApi.Controllers
{
    public class BackDrawMoneyController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-3
		/// message:提现列表
		/// author：Thomars
		/// </summary>
		/// <param name="KeyWord"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="State"></param>
		/// <returns></returns>
		[Route("api/BackDrawMoney/GetUserWithdrawalList"), HttpGet]
		public HttpResponseMessage GetUserWithdrawalList(string KeyWord, int pageNum, int pagesize, int? State)
		{
			List<UserByDrawMoneyModel> list = new List<UserByDrawMoneyModel>();
			int count = 0;
			var json = string.Empty;
			if (string.IsNullOrWhiteSpace(KeyWord) == false && State != null)
			{

				count = (from d in entities.DrawMoney
					 join c in entities.AroundUser on d.UserId equals c.Id
					 where (c.Name.Contains(KeyWord) || c.Phone.Contains(KeyWord)) && d.State == State
					 orderby d.Id descending
					 select new { d, c }).Count();

				var query = (from d in entities.DrawMoney
					     join c in entities.AroundUser on d.UserId equals c.Id
					     where ( c.Name.Contains(KeyWord) || c.Phone.Contains(KeyWord)) && d.State == State
					     orderby d.Id descending
					     select new { d, c }).Skip(pagesize * (pageNum - 1)).Take(pagesize);
				foreach (var item in query)
				{
					UserByDrawMoneyModel model = new UserByDrawMoneyModel();
					model.Id = item.d.Id;
					model.RemoveMoenyNumber = item.d.RemoveMoenyNumber;
					model.Name = item.c.Name;
					model.CustomerId = item.d.UserId.ToString();
					model.Phone = item.c.Phone;
					model.RemoveMoeny = item.d.RemoveMoeny.ToString();
					model.RemoveMoneyState = item.d.State.ToString();
					model.RemoveMoenyTime = item.d.RemoveMoenyTime.Value.ToString("yyyy-MM-dd hh:mm:ss");
					model.Bank = item.d.Bank;
					model.BankName = item.d.BankName;
					model.BankAccount = item.d.BankAccount;
					list.Add(model);
				}
				json = JsonConvert.SerializeObject(list);
			}
			else if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				count = (from d in entities.DrawMoney
					 join c in entities.AroundUser on d.UserId equals c.Id
					 where  c.Name.Contains(KeyWord) || c.Phone.Contains(KeyWord)
					 orderby d.Id descending
					 select new { d, c }).Count();

				var query = (from d in entities.DrawMoney
					     join c in entities.AroundUser on d.UserId equals c.Id
					     where  c.Name.Contains(KeyWord) || c.Phone.Contains(KeyWord)
					     orderby d.Id descending
					     select new { d, c }).Skip(pagesize * (pageNum - 1)).Take(pagesize);
				foreach (var item in query)
				{
					UserByDrawMoneyModel model = new UserByDrawMoneyModel();
					model.Id = item.d.Id;
					model.RemoveMoenyNumber = item.d.RemoveMoenyNumber;
					model.Name = item.c.Name;
					model.CustomerId = item.d.UserId.ToString();
					model.Phone = item.c.Phone;
					model.RemoveMoeny = item.d.RemoveMoeny.ToString();
					model.RemoveMoneyState = item.d.State.ToString();
					model.RemoveMoenyTime = item.d.RemoveMoenyTime.Value.ToString("yyyy-MM-dd hh:mm:ss");
					model.Bank = item.d.Bank;
					model.BankName = item.d.BankName;
					model.BankAccount = item.d.BankAccount;
					list.Add(model);
				}
				json = JsonConvert.SerializeObject(list);
			}
			else if (State != null)
			{
				count = (from d in entities.DrawMoney
					 join c in entities.AroundUser on d.UserId equals c.Id
					 where d.State == State
					 orderby d.Id descending
					 select new { d, c }).Count();

				var query = (from d in entities.DrawMoney
					     join c in entities.AroundUser on d.UserId equals c.Id
					     where d.State == State
					     orderby d.Id descending
					     select new { d, c }).Skip(pagesize * (pageNum - 1)).Take(pagesize);
				foreach (var item in query)
				{
					UserByDrawMoneyModel model = new UserByDrawMoneyModel();
					model.Id = item.d.Id;
					model.RemoveMoenyNumber = item.d.RemoveMoenyNumber;
					model.Name = item.c.Name;
					model.CustomerId = item.d.UserId.ToString();
					model.Phone = item.c.Phone;
					model.RemoveMoeny = item.d.RemoveMoeny.ToString();
					model.RemoveMoneyState = item.d.State.ToString();
					model.RemoveMoenyTime = item.d.RemoveMoenyTime.Value.ToString("yyyy-MM-dd hh:mm:ss");
					model.Bank = item.d.Bank;
					model.BankName = item.d.BankName;
					model.BankAccount = item.d.BankAccount;
					list.Add(model);
				}
				json = JsonConvert.SerializeObject(list);
			}
			else
			{
				count = (from d in entities.DrawMoney
					 join c in entities.AroundUser on d.UserId equals c.Id
					 orderby d.Id descending
					 select new { d, c }).Count();

				var query = (from d in entities.DrawMoney
					     join c in entities.AroundUser on d.UserId equals c.Id
					     orderby d.Id descending
					     select new { d, c }).Skip(pagesize * (pageNum - 1)).Take(pagesize);
				foreach (var item in query)
				{
					UserByDrawMoneyModel model = new UserByDrawMoneyModel();
					model.Id = item.d.Id;
					model.RemoveMoenyNumber = item.d.RemoveMoenyNumber;
					model.Name = item.c.Name;
					model.CustomerId = item.d.UserId.ToString();
					model.Phone = item.c.Phone;
					model.RemoveMoeny = item.d.RemoveMoeny.ToString();
					model.RemoveMoneyState = item.d.State.ToString();
					model.RemoveMoenyTime = item.d.RemoveMoenyTime.Value.ToString("yyyy-MM-dd hh:mm:ss");
					model.Bank = item.d.Bank;
					model.BankName = item.d.BankName;
					model.BankAccount = item.d.BankAccount;
					list.Add(model);
				}
				json = JsonConvert.SerializeObject(list);
			}
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
		/// time: 2020-7-3
		/// message:提现列表改动版
		/// author：Thomars
		/// </summary>
		/// <param name="KeyWord"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="State"></param>
		/// <returns></returns>
		[Route("api/BackDrawMoney/GetUserByWithdrawalList"), HttpGet]
		public HttpResponseMessage GetUserByWithdrawalList(string KeyWord, int pageNum, int pagesize, int? State)
		{
			var json = string.Empty;
			var query = (from d in entities.DrawMoney
				     join c in entities.AroundUser on d.UserId equals c.Id
				     orderby d.Id descending
				     select new { d, c });
			if (string.IsNullOrWhiteSpace(KeyWord) == false && State != null)
			{
				query = query.Where(c => (c.c.Name.Contains(KeyWord) || c.c.Phone.Contains(KeyWord)) && c.d.State == State);
			}
			else if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				query = query.Where(c => c.c.Name.Contains(KeyWord) || c.c.Phone.Contains(KeyWord));
			}
			else if (State != null)
			{
				query = query.Where(c => c.d.State == State);
			}
			int count = query.Count();
			var rows = query.OrderByDescending(c => c.d.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e => new
			{
				Id = e.d.Id,
				RemoveMoenyNumber = e.d.RemoveMoenyNumber,
				Name = e.c.Name,
				CustomerId = e.d.UserId,
				Phone = e.c.Phone,
				RemoveMoeny = e.d.RemoveMoeny,
				RemoveMoneyState = e.d.State,
				CustomerNumber=e.c.CustomerNumber,
				RemoveMoenyTime = e.d.RemoveMoenyTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				Bank = e.d.Bank,
				BankName = e.d.BankName,
				BankAccount = e.d.BankAccount,

			}).ToList();
			json = JsonConvert.SerializeObject(rows);
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
		/// time: 2020-7-3
		/// message:提现
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackDrawMoney/ChangeDrawMoenyState"), HttpPost]
		public IHttpActionResult ChangeDrawMoenyState([FromBody]JObject value)
		{
			int result = 0;
			int id = (int)value["Id"];
			int state = (int)value["State"];
			var query = entities.DrawMoney.Where(d => d.Id == id).FirstOrDefault();

			if (query != null && state ==2)
			{
				//提现成功
				query.State = 2;
				DbEntityEntry entry = entities.Entry(query);
				entry.State = System.Data.Entity.EntityState.Modified;
				result = entities.SaveChanges();

				//日志表记录
				if (result > 0)
				{
					var model = entities.AroundUserFinance.Where(c => c.UserId == query.UserId).FirstOrDefault();
					if (model != null)
					{
						model.AccountBalance = model.AccountBalance - query.RemoveMoeny;
						model.AccumulatedExpenditure = model.AccumulatedExpenditure + query.RemoveMoeny;
						DbEntityEntry entry1 = entities.Entry(model);
						entry1.State = System.Data.Entity.EntityState.Modified;
						result = entities.SaveChanges();

						TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
						AroundUserFinanceLog log = new AroundUserFinanceLog
						{
							BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
							UserId = model.UserId,
							PaymentState = 3,
							TransactionType = 1,
							TransactionTime = DateTime.Now,
							TransactionAmount = query.RemoveMoeny,
							Remarks = "提现支出"
						};
						entities.AroundUserFinanceLog.Add(log);
						result = entities.SaveChanges();
					}
				}
			}
			else
			{
				query.State = 3;
				DbEntityEntry entry = entities.Entry(query);
				entry.State = System.Data.Entity.EntityState.Modified;
				result = entities.SaveChanges();
				return Ok(Respone.Success("提现失败"));
			}
			if (result > 0)
			{
				return Ok(Respone.Success("提现成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}
    }
}
