using AssistantApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AssistantApi.Controllers
{
    public class NoticeController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-3
		/// message:前台公共展示
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/Notice/GetNotice"), HttpGet]
		public HttpResponseMessage GetNotice()
		{
			List<NoticeModel> list = new List<NoticeModel>();
			var json = string.Empty;
			var query = entities.Notice.ToList();
			foreach (var item in query)
			{
				NoticeModel notice = new NoticeModel();
				notice.Id = item.Id;
				notice.Title = item.Title;
				notice.Contenting = item.Contenting;
				notice.AddTime = item.AddTime.Value.ToString("yyyy-MM-dd hh:mm:ss");
				list.Add(notice);
			}
			json = JsonConvert.SerializeObject(list);
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}
    }
}
