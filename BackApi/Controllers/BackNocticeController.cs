using BackApi.MessageOut;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BackApi.Controllers
{
    public class BackNocticeController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-6-30
		/// message: 添加公告
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/BackNoctice/AddBackNoctice"), HttpPost]
		public IHttpActionResult AddBackNoctice([FromBody]JObject value)
		{
			int result = 0;
			string title = value["Titles"].ToString();
			string contenting = value["Contenting"].ToString();

			Notice nt = new Notice();
			nt.Title = title;
			nt.Contenting = contenting;
			nt.AddTime = DateTime.Now;
			entities.Notice.Add(nt);
			result = entities.SaveChanges();

			if (result > 0)
			{
				return Ok(Respone.Success("添加成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		/// <summary>
		/// time: 2020-6-30
		/// message: 修改公告
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/BackNoctice/ChangerNotice"), HttpPost]
		public IHttpActionResult ChangerNotice([FromBody]JObject value)
		{
			int result = 0;
			int id = (int)value["Id"];
			string title = value["Titles"].ToString();
			string contenting = value["Contenting"].ToString();
			var nid = entities.Notice.Where(e => e.Id == id).FirstOrDefault();
			if (nid != null)
			{
				nid.Title = title;
				nid.Contenting = contenting;
				DbEntityEntry entry = entities.Entry(nid);
				entry.State = System.Data.Entity.EntityState.Modified;
				result = entities.SaveChanges();
			}

			if (result > 0)
			{
				return Ok(Respone.Success("修改成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		/// <summary>
		/// time: 2020-6-30
		/// message: 展示公告列表
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/BackNoctice/GetNotice"), HttpGet]
		public HttpResponseMessage GetNotice(int pagesize, int pageNum)
		{
			var json = string.Empty;
			int count = entities.Notice.Count();
			var query = entities.Notice.OrderByDescending(e => e.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize);
			json = JsonConvert.SerializeObject(query);
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
