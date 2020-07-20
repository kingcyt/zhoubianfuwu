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
    public class ShopCpcReportController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();
		/// <summary>
		/// time: 2020-7-7
		/// message:店铺CPC报告
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/ShopCpcReport/AddShopCpcReport"), HttpPost]
		public IHttpActionResult AddShopCpcReport([FromBody]JObject value)
		{
			int id = (int)value["UserId"];
			int countryId = (int)value["CountryId"];
			string shopId = value["ShopId"].ToString();
			string brand = value["Brand"].ToString();
			decimal price = (decimal)value["Price"];

			ShopCpcReport shopCpc = new ShopCpcReport
			{
				UserId = id,
				CountryId = countryId,
				ShopId = shopId,
				Price = price,
				State = 1,
				Brand = brand,
				BuyTime = DateTime.Now

			};

			entities.ShopCpcReport.Add(shopCpc);
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
		/// message:店铺CPC报告列表
		/// author：Thomars 
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <returns></returns>
		[Route("api/ShopCpcReport/GetShopCpcReport"), HttpGet]
		public HttpResponseMessage GetShopCpcReport(int UserId, int pageNum, int pagesize)
		{
			var query = from s in entities.ShopCpcReport
				    join c in entities.Country on s.CountryId equals c.Id
				    where s.UserId == UserId
				    select new { s.Id,s.Brand,s.State,s.ShopCpcType,s.BuyTime,s.ShopId, c.CountryName };
			int count = query.Count();
			var row = query.OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e => new
			{
				Id = e.Id,
				Country = e.CountryName,
				Brand = e.Brand,
				State = e.State,
				ShopCpcType = e.ShopCpcType,
				BuyTime = e.BuyTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				ShopId=e.ShopId,
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
