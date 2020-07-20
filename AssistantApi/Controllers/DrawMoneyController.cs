using AssistantApi.MessageOut;
using AssistantApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AssistantApi.Controllers
{
    public class DrawMoneyController : ApiController
    {
		static DateTime stime;
		static DateTime etime;
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-3
		/// message:用户提现
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/DrawMoney/UserWithdrawal"), HttpPost]
		public IHttpActionResult UserWithdrawal([FromBody]JObject value)
		{
			int result = 0;
			string bank = value["Bank"].ToString();
			string bankName = value["BankName"].ToString();
			int id = (int)value["Id"];
			string bankAccount = value["BankAccount"].ToString();
			decimal removeMoney = (decimal)value["RemoveMoney"];
			var finance = entities.AroundUserFinance.Where(e => e.UserId == id).FirstOrDefault();
			if (removeMoney > Convert.ToDecimal( finance.AccountBalance))
			{
				return Ok(Respone.No("余额不足"));
			}

			TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			DrawMoney drawMoney = new DrawMoney
			{
				UserId = id,
				RemoveMoeny = removeMoney,
				State = 1,
				RemoveMoenyTime = DateTime.Now,
				Bank = bank,
				BankName = bankName,
				BankAccount = bankAccount,
				RemoveMoenyNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString()
			};
			entities.DrawMoney.Add(drawMoney);
			result = entities.SaveChanges();
			if (result > 0)
			{
				return Ok(Respone.Success("提交成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		#region
		/// <summary>
		/// time: 2020-7-3
		///  message:提现列表
		///  author：Thomars
		/// </summary>
		/// <param name="statetime"></param>
		/// <param name="endtime"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="state"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		[Route("api/DrawMoney/GetUserWithdrawal"), HttpGet]
		public HttpResponseMessage GetUserWithdrawal(string Statetime, string Endtime,  int pageNum, int pagesize, int? State, int UserId)
		{
			List<DrawMoneyModel> list = new List<DrawMoneyModel>();
			if (string.IsNullOrWhiteSpace(Statetime) == false && string.IsNullOrWhiteSpace(Endtime) == false)
			{
				stime = DateTime.Parse(Statetime);
				etime = DateTime.Parse(Endtime);
			}
			var json = string.Empty;
			int count = 0;
			if (State != 0 && string.IsNullOrWhiteSpace(Statetime) == false && string.IsNullOrWhiteSpace(Endtime) == false)
			{
				count = entities.DrawMoney.Where(e => e.UserId == UserId && e.State == State && DbFunctions.TruncateTime(e.RemoveMoenyTime) >= stime && DbFunctions.TruncateTime(e.RemoveMoenyTime) <= etime).Count();
				var query = entities.DrawMoney.Where(e => e.UserId == UserId && e.State == State && DbFunctions.TruncateTime(e.RemoveMoenyTime) >= stime && DbFunctions.TruncateTime(e.RemoveMoenyTime) <= etime).OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList();
				foreach (var item in query)
				{
					DrawMoneyModel model = new DrawMoneyModel();
					model.Id = item.Id;
					model.UserId = (int)item.UserId;
					model.RemoveMoenyNumber = item.RemoveMoenyNumber;
					model.RemoveMoenyStae = item.State;
					model.RemoveMoeny = item.RemoveMoeny;
					model.RemoveMoenyTime = item.RemoveMoenyTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
					model.Bank = item.Bank;
					model.BankName = item.BankName;
					model.BankAccount = item.BankAccount;
					model.Remarks = item.Remarks;
					list.Add(model);
				}
				json = JsonConvert.SerializeObject(list);
			}
			else if (State != 0)
			{
				count = entities.DrawMoney.Where(e => e.UserId == UserId && e.State == State).Count();
				var query = entities.DrawMoney.Where(e => e.UserId == UserId && e.State == State).OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList();
				foreach (var item in query)
				{
					DrawMoneyModel model = new DrawMoneyModel();
					model.Id = item.Id;
					model.UserId = (int)item.UserId;
					model.RemoveMoenyNumber = item.RemoveMoenyNumber;
					model.RemoveMoenyStae = item.State;
					model.RemoveMoeny = item.RemoveMoeny;
					model.RemoveMoenyTime = item.RemoveMoenyTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
					model.Bank = item.Bank;
					model.BankName = item.BankName;
					model.BankAccount = item.BankAccount;
					model.Remarks = item.Remarks;
					list.Add(model);
				}
				json = JsonConvert.SerializeObject(list);

			}
			else if (string.IsNullOrWhiteSpace(Statetime) == false && string.IsNullOrWhiteSpace(Endtime) == false)
			{
				count = entities.DrawMoney.Where(e => e.UserId == UserId && DbFunctions.TruncateTime(e.RemoveMoenyTime) >= stime && DbFunctions.TruncateTime(e.RemoveMoenyTime) <= etime).Count();
				var query = entities.DrawMoney.Where(e => e.UserId == UserId && DbFunctions.TruncateTime(e.RemoveMoenyTime) >= stime && DbFunctions.TruncateTime(e.RemoveMoenyTime) <= etime).OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList();
				foreach (var item in query)
				{
					DrawMoneyModel model = new DrawMoneyModel();
					model.Id = item.Id;
					model.UserId = (int)item.UserId;
					model.RemoveMoenyNumber = item.RemoveMoenyNumber;
					model.RemoveMoenyStae = item.State;
					model.RemoveMoeny = item.RemoveMoeny;
					model.RemoveMoenyTime = item.RemoveMoenyTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
					model.Bank = item.Bank;
					model.BankName = item.BankName;
					model.BankAccount = item.BankAccount;
					model.Remarks = item.Remarks;
					list.Add(model);
				}
				json = JsonConvert.SerializeObject(list);
			}
			else
			{
				count = entities.DrawMoney.Where(e => e.UserId == UserId ).Count();
				var query = entities.DrawMoney.Where(e => e.UserId == UserId ).OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList();
				foreach (var item in query)
				{
					DrawMoneyModel model = new DrawMoneyModel();
					model.Id = item.Id;
					model.UserId = (int)item.UserId;
					model.RemoveMoenyNumber = item.RemoveMoenyNumber;
					model.RemoveMoenyStae = item.State;
					model.RemoveMoeny = item.RemoveMoeny;
					model.RemoveMoenyTime = item.RemoveMoenyTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
					model.Bank = item.Bank;
					model.BankName = item.BankName;
					model.BankAccount = item.BankAccount;
					model.Remarks = item.Remarks;
					list.Add(model);
				}
				json = JsonConvert.SerializeObject(list);
			}
			json = JsonConvert.SerializeObject(list);
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
		#endregion

		/// <summary>
		/// time: 2020-7-3
		///  message:提现列表改动版
		///  author：Thomars
		/// </summary>
		/// <param name="statetime"></param>
		/// <param name="endtime"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="state"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		[Route("api/DrawMoney/GetUserByWithdrawal"), HttpGet]
		public HttpResponseMessage GetUserByWithdrawal(string Statetime, string Endtime, int pageNum, int pagesize, int? State, int UserId)
		{
			var json = string.Empty;
			if (string.IsNullOrWhiteSpace(Statetime) == false && string.IsNullOrWhiteSpace(Endtime) == false)
			{
				stime = DateTime.Parse(Statetime);
				etime = DateTime.Parse(Endtime);
			}
			
			var query = from d in entities.DrawMoney
				    where d.UserId == UserId
				    select new
				    {
					    Id = d.Id,
					    UserId = d.UserId,
					    RemoveMoenyNumber = d.RemoveMoenyNumber,
					    State = d.State,
					    RemoveMoeny = d.RemoveMoeny,
					    RemoveMoenyTime = d.RemoveMoenyTime,
					    Bank = d.Bank,
					    BankName = d.BankName,
					    BankAccount = d.BankAccount,
					    Remarks = d.Remarks,
				    };
			if (string.IsNullOrWhiteSpace(Statetime) == false && string.IsNullOrWhiteSpace(Endtime) == false && State != null)
			{
				query = query.Where(e => e.State == State && DbFunctions.TruncateTime(e.RemoveMoenyTime) >= stime && DbFunctions.TruncateTime(e.RemoveMoenyTime) <= etime);
			}
			else if (string.IsNullOrWhiteSpace(Statetime) == false && string.IsNullOrWhiteSpace(Endtime) == false)
			{
				query = query.Where(e => DbFunctions.TruncateTime(e.RemoveMoenyTime) >= stime && DbFunctions.TruncateTime(e.RemoveMoenyTime) <= etime);
			}
			else if (State != null)
			{
				query = query.Where(e => e.State == State);
			}
			int count = query.Count();
			var rows = query.OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(d => new
			{
				Id = d.Id,
				UserId = d.UserId,
				RemoveMoenyNumber = d.RemoveMoenyNumber,
				State = d.State,
				RemoveMoeny = d.RemoveMoeny,
				RemoveMoenyTime = d.RemoveMoenyTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
				Bank = d.Bank,
				BankName = d.BankName,
				BankAccount = d.BankAccount,
				Remarks = d.Remarks,
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
    }
}
