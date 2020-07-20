using AssistantApi.MessageOut;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackApi.Controllers
{
    public class BackNavBySubmeunController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();
		/// <summary>
		/// time: 2020-7-1
		/// message:添加模块信息
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackNavBySubmeun/AddSubmenu"), HttpPost]
		public IHttpActionResult AddSubmenu([FromBody]JObject value)
		{

			int result = 0;
			int modularId = (int)value["ModularId"];
			string submenuName = value["SubmenuName"].ToString();
			decimal price = (decimal)value["Price"];
			string remark = value["Remark"].ToString();
			NavBySubmeun submenu = new NavBySubmeun
			{
				ModularId = modularId,
				SubmenuName = submenuName,
				Price = price,
				Remark = remark,
				State = 1
			};
			entities.NavBySubmeun.Add(submenu);
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
		/// time: 2020-7-1
		/// message:修改模块信息
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackNavBySubmeun/ChangeSubmenu"), HttpPost]
		public IHttpActionResult ChangeSubmenu([FromBody]JObject value)
		{
			int result = 0;
			int id = (int)value["Id"];
			string submenuName = value["SubmenuName"].ToString();
			decimal price = (decimal)value["Price"];
			string remark = value["Remark"].ToString();
			int state = (int)value["State"];

			var query = entities.NavBySubmeun.Where(e => e.Id == id).FirstOrDefault();
			if (query != null)
			{
				query.SubmenuName = submenuName;
				query.Price = price;
				query.Remark = remark;
				query.State = state;
				DbEntityEntry entry = entities.Entry(query);
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
		/// time: 2020-7-1
		/// message:获取模块信息
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackNavBySubmeun/GetSubmenu"), HttpGet]
		public HttpResponseMessage GetSubmenu()
		{
			var json = string.Empty;
			var query = entities.NavBySubmeun.ToList();
			json = JsonConvert.SerializeObject(query);
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}
	}
}
