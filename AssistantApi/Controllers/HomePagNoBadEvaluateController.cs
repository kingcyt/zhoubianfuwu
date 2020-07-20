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
	public class HomePagNoBadEvaluateController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-6
		/// message:首页无差评
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/HomePagNoBadEvaluate/AddPagNoBadEvaluate"), HttpPost]
		public IHttpActionResult AddPagNoBadEvaluate([FromBody]JObject value)
		{
			int id = (int)value["UserId"];
			int countryId = (int)value["CountryId"];
			string asin = value["ASIN"].ToString();
			decimal price = (decimal)value["Price"];

			HomePagNoBadEvaluate homePag = new HomePagNoBadEvaluate
			{
				UserId = id,
				ASIN = asin,
				CountryId = countryId,
				State = 1,
				Price = price,
				BuyTime = DateTime.Now
			};
			entities.HomePagNoBadEvaluate.Add(homePag);
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
		/// message:首页无差评列表
		/// author：Thomars
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <returns></returns>
		[Route("api/HomePagNoBadEvaluate/GetPagNoBadEvaluate"), HttpGet]
		public HttpResponseMessage GetPagNoBadEvaluate(int UserId, int pageNum, int pagesize)
		{
			var query = from h in entities.HomePagNoBadEvaluate
				    join c in entities.Country on h.CountryId equals c.Id
				    where h.UserId == UserId
				    select new
				    {
					    h.ASIN,h.BuyTime,h.State,h.ListingPag,h.Id,c.CountryName
				    };
			int count = query.Count();
			var row=query.OrderByDescending(i =>i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e=>new
			{
				ASIN=e.ASIN,
				Country=e.CountryName,
				BuyTime = e.BuyTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				State = e.State,
				ListingPag=e.ListingPag,
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
