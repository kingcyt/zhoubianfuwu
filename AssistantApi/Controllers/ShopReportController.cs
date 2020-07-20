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
	public class ShopReportController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-7
		/// message:店铺体验报告
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/ShopReport/AddShopReport"), HttpPost]
		public IHttpActionResult AddShopReport([FromBody]JObject value)
		{
			int id = (int)value["UserId"];
			int countryId = (int)value["CountryId"];
			string shopEmail = value["ShopEmail"].ToString();
			string shopId = value["ShopId"].ToString();
			decimal price = (decimal)value["Price"];

			ShopReport report = new ShopReport
			{
				UserId = id,
				CountryId = countryId,
				ShopEmail = shopEmail,
				ShopId = shopId,
				Price = price,
				State = 1,
				BuyTime = DateTime.Now
			};
			entities.ShopReport.Add(report);
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
		/// time: 2020-7-7
		/// message:店铺体验报告列表
		/// author：Thomars
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <returns></returns>
		[Route("api/ShopReport/GetShopReport"), HttpGet]
		public HttpResponseMessage GetShopReport(int UserId, int pageNum, int pagesize)
		{
			var query = from s in entities.ShopReport
				    join c in entities.Country on s.CountryId equals c.Id
				    where s.UserId == UserId
				    select new { s.Id,s.ShopEmail,s.ShopId,s.ShopType,s.State,s.BuyTime,c.CountryName };
			int count = query.Count();
			var row = query.OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e=>new
			{
				Id=e.Id,
				ShopEmail = e.ShopEmail,
				ShopId=e.ShopId,
				Country=e.CountryName,
				ShopType = e.ShopType,
				State = e.State,
				BuyTime = e.BuyTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
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
