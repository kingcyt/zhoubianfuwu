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
	public class DifferentController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-20
		/// message:点赞点踩
		/// author:Thomars 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/Different/AddDifferent"), HttpPost]
		public IHttpActionResult AddDifferent([FromBody]JObject value)
		{
			int uid = (int)value["UserId"];
			decimal price = (decimal)value["Price"];
			int type = (int)value["Type"];
			string link = value["Link"].ToString();
			int estimateNumber = (int)value["EstimateNumber"];
			int actualNumber = (int)value["ActualNumber"];

			Different different = new Different
			{
				UserId = uid,
				State = 1,
				Type = type,
				Price = price,
				Link = link,
				EstimateNumber = estimateNumber,
				ActualNumber = actualNumber,
				AddTimes = DateTime.Now
			};
			entities.Different.Add(different);
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
		/// time:2020-7-20
		/// message:点赞点踩列表
		/// author:Thomars  
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="Type"></param>
		/// <returns></returns>
		[Route("api/Different/GetDifferent"), HttpGet]
		public HttpResponseMessage GetDifferent(int UserId, int pageNum, int pagesize, int Type)
		{
			var json = string.Empty;
			var query = from d in entities.Different
				    where d.UserId == UserId && d.Type == Type
				    select new { d.Id, d.Link, d.ActualNumber, d.AddTimes, d.EstimateNumber,d.BackNumber,d.State,d.Price };

			int count = query.Count();
			var rows = query.OrderByDescending(d => d.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e => new
			{
				Id = e.Id,
				Link = e.Link,
				ActualNumber = e.ActualNumber,
				EstimateNumber=e.EstimateNumber,
				BackNumber=e.BackNumber,
				State=e.State,
				Price=e.Price,
				AddTimes = e.AddTimes.Value.ToString("yyyy-MM-dd hh:mm:ss"),
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
