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
	public class AsinKeyWordController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-6
		/// message:ASIN关键词报告
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/AsinKeyWord/AddAsinKeyWord"), HttpPost]
		public IHttpActionResult AddAsinKeyWord([FromBody]JObject value)
		{
			int id = (int)value["UserId"];
			int countryId = (int)value["CountryId"];
			string asin = value["ASIN"].ToString();
			decimal price = (decimal)value["Price"];

			AsinKeyWord asinKey = new AsinKeyWord
			{
				UserId = id,
				CountryId = countryId,
				ASIN = asin,
				State = 1,
				Price = price,
				BuyTime = DateTime.Now
			};
			entities.AsinKeyWord.Add(asinKey);
			int result = entities.SaveChanges();
			if (result > 0)
			{
				return Ok(Respone.Success("购买成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		/// <summary>
		/// time: 2020-7-6
		/// message:ASIN关键词报告列表
		/// author：Thomars 
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <returns></returns>
		[Route("api/AsinKeyWord/GetAsinKeyWord"), HttpGet]
		public HttpResponseMessage GetAsinKeyWord(int UserId, int pageNum, int pagesize)
		{
			var query = from a in entities.AsinKeyWord
				    join c in entities.Country on  a.CountryId equals c.Id
				    where a.UserId == UserId
				    select new { a.Id,a.ASIN,a.State,a.AsinType,a.BuyTime,c.CountryName };
			int count = query.Count();
			var row=query.OrderByDescending(i=>i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e=> new
			{
				Id=e.Id,
				Country=e.CountryName,
				ASIN=e.ASIN,
				State=e.State,
				AsinType=e.AsinType,
				BuyTime=e.BuyTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
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
