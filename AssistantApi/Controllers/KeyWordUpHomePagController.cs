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
	public class KeyWordUpHomePagController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();
		/// <summary>
		/// time: 2020-7-6
		/// message:关键词上首页
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/KeyWordUpHomePag/AddKeyWordUpHomePag"), HttpPost]
		public IHttpActionResult AddKeyWordUpHomePag([FromBody]JObject value)
		{
			int id = (int)value["UserId"];
			int countryId = (int)value["CountryId"];
			string asin = value["ASIN"].ToString();
			decimal price = (decimal)value["Price"];
			string keyWord = value["KeyWord"].ToString();

			KeyWordUpHomePag key = new KeyWordUpHomePag
			{
				UserId = id,
				ASIN = asin,
				Price = price,
				KeyWord = keyWord,
				State = 1,
				CountryId = countryId,
				SubTime = DateTime.Now
			};

			entities.KeyWordUpHomePag.Add(key);
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
		/// time: 2020-7-6
		/// message:关键词上首页列表
		/// author：Thomars
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <returns></returns>
		[Route("api/KeyWordUpHomePag/GetKeyWordUpHomePag"), HttpGet]
		public HttpResponseMessage GetKeyWordUpHomePag(int UserId, int pageNum, int pagesize)
		{
			var query = from k in entities.KeyWordUpHomePag
				    join c in entities.Country on k.CountryId equals c.Id
				    where k.UserId == UserId
				    select new { k.Id,k.ASIN,k.KeyWord,k.SubTime,k.State,k.KeywordImage,k.Price,c.CountryName };
			int count = query.Count();
			var row=query.OrderByDescending(i=>i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e=>new
			{
				Id=e.Id,
				Country=e.CountryName,
				ASIN=e.ASIN,
				KeyWord=e.KeyWord,
				SubTime= e.SubTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				State=e.State,
				KeywordImage=e.KeywordImage,
				Price=e.Price,
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
