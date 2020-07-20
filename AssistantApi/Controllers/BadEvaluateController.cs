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
	public class BadEvaluateController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-6
		/// message:删差评
		/// author:Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BadEvaluate/DeleteBadEvaluate"), HttpPost]
		public IHttpActionResult DeleteBadEvaluate([FromBody]JObject value)
		{
			int id = (int)value["UserId"];
			string review = value["Review"].ToString();
			decimal price = (decimal)value["Price"];

			BadEvaluate bad = new BadEvaluate
			{
				ReviewLink = review,
				State = 1,  //待确认
				Price = price,
				SubTime = DateTime.Now,
				UserId = id
			};
			entities.BadEvaluate.Add(bad);
			int result = entities.SaveChanges();
			if (result > 0)
			{
				return Ok(Respone.Success("上传成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		/// <summary>
		/// time:2020-7-6
		///  message:删差评列表展示
		///  author:Thomars
		/// </summary>
		/// <param name="UserId"></param>
		/// <returns></returns>
		[Route("api/BadEvaluate/GetBadEvaluate"), HttpGet]
		public HttpResponseMessage GetBadEvaluate(int UserId, int pageNum, int pagesize)
		{
			var query = from b in entities.BadEvaluate
				    where b.UserId == UserId
				    select new { b.Id,b.State,b.SubTime,b.ReviewLink };
			int count = query.Count();
			var row = query.OrderByDescending(i => i.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e=>new
			{
				Id=e.Id,
				ReviewLink=e.ReviewLink,
				State=e.State,
				SubTime=e.SubTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
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
