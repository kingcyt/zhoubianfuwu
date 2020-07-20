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
	public class AddShopCartController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-16
		/// message:加购
		/// author:Thomars  
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/AddShopCart/AddtoShopCart"), HttpPost]
		public IHttpActionResult AddtoShopCart([FromBody]JObject value)
		{
			int uid = (int)value["UserId"];
			int countryId = (int)value["CountryId"];
			decimal price = (decimal)value["Price"];
			string shopId = value["ShopId"].ToString();
			int number = (int)value["Number"];
			string asin = value["ASIN"].ToString();
			string keyWord = value["KeyWord"].ToString();
			string productlocation = value["Productlocation"].ToString();
			string remarks = value["Remarks"].ToString();

			var addShopCart = new AddShopCart
			{
				UserId = uid,
				State = 1,
				AddTimes = DateTime.Now,
				Price = price,
				CountryId = countryId,
				ShopId = shopId,
				Number = number,
				ASIN = asin,
				KeyWord = keyWord,
				Productlocation = productlocation,
				Remarks = remarks
			};
			entities.AddShopCart.Add(addShopCart);
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
		/// time:2020-7-16
		/// message:加购展示
		/// author:Thomars  
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="keyWord"></param>
		/// <param name="State"></param>
		/// <returns></returns>
		[Route("api/AddShopCart/GetAddShopCart"), HttpGet]
		public HttpResponseMessage GetAddShopCart(int UserId, int pageNum, int pagesize, string keyWord, int State)
		{
			var json = string.Empty;
			var query = from l in entities.AddShopCart
				    join u in entities.AroundUser on l.UserId equals u.Id
				    join c in entities.Country on l.CountryId equals c.Id
				    where l.UserId == UserId
				    select new { l.Id,l.ShopId,l.Number,l.Actual,l.ASIN,l.State,l.Price,l.Images,l.KeyWord,l.Productlocation,l.AddTimes,l.Remarks, c.CountryName };
			if (string.IsNullOrWhiteSpace(keyWord) == false)
			{
				query = query.Where(e => e.ShopId.Contains(keyWord) || e.ASIN.Contains(keyWord));
			}
			if (State > 0)
			{
				query = query.Where(e => e.State == State);
			}
			int count = query.Count();
			var rows = query.OrderByDescending(c => c.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e => new
			{
				Id = e.Id,
				CountryName = e.CountryName,
				ShopId = e.ShopId,
				Number = e.Number,
				Actual = e.Actual,
				ASIN = e.ASIN,
				State = e.State,
				Price = e.Price,
				Images = e.Images,
				KeyWord = e.KeyWord,
				Productlocation = e.Productlocation,
				AddTimes = e.AddTimes.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				Remarks = e.Remarks,
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
