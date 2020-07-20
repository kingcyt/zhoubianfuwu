using AssistantApi.MessageOut;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AssistantApi.Controllers
{
    public class BugQueryController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-8
		/// message:死因报告
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BugQuery/AddBugQuery"), HttpPost]
		public IHttpActionResult AddBugQuery([FromBody]JObject value)
		{
			int id = (int)value["UserId"];
			int countryId = (int)value["CountryId"];
			string registeredEmail = value["RegisteredEmail"].ToString();
			string sellerIdOrASIN = value["SellerIdOrASIN"].ToString();
			decimal price = (decimal)value["Price"];
			string serviceName = value["ServiceName"].ToString();

			BugQuery bug = new BugQuery
			{
				UserId = id,
				CountryId = countryId,
				RegisteredEmail = registeredEmail,
				SellerIdOrAsin = sellerIdOrASIN,
				Price = price,
				State=1,
				ServiceName = serviceName,
				SubTime = DateTime.Now
			};
			entities.BugQuery.Add(bug);
			int result = entities.SaveChanges();
			if (result > 0)
			{
				return Ok(Respone.Success("提交成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}


		/// <summary>
		/// time: 2020-7-8
		/// message:死因报告列表
		/// author：Thomars
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <returns></returns>
		[Route("api/BugQuery/GetBugQuery"), HttpGet]
		public HttpResponseMessage GetBugQuery(int UserId, int pageNum, int pagesize)
		{
			var query=from b in entities.BugQuery
				  join c in entities.Country on b.CountryId equals c.Id
				  where b.UserId == UserId
				  select new { b.Id,b.RegisteredEmail,b.SellerIdOrAsin,b.ServiceName,b.SubTime,b.State, c.CountryName };
			int count = query.Count();
			var row = query.OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e => new
			{
				Id = e.Id,
				Countey = e.CountryName,
				RegisteredEmail = e.RegisteredEmail,
				SellerIdOrAsin = e.SellerIdOrAsin,
				ServiceName=e.ServiceName,
				SubTime = e.SubTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				State = e.State,
			}).ToList();

			var json = JsonConvert.SerializeObject(row);
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
