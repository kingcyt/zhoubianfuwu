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
using System.Web.Http;

namespace BackApi.Controllers
{
    public class BackRoleController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		///time: 2020-6-30
		/// message:添加角色
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackRole/AddRole"), HttpPost]
		public IHttpActionResult AddRole([FromBody]JObject value)
		{
			int result = 0;
			string roleName = value["RoleName"].ToString();
			string remarks = value["Remarks"].ToString();

			var query = entities.Role.Where(e => e.RoleName == roleName).FirstOrDefault();
			if (query != null)
			{
				return Ok(Respone.No("该角色已存在！"));
			}
			Role role = new Role();
			role.RoleName = roleName;
			role.Remarks = remarks;
			entities.Role.Add(role);
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
		/// message:角色列表展示
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/BackRole/GetRole"), HttpGet]
		public HttpResponseMessage GetRole()
		{
			var query = entities.Role.ToList();
			var json = JsonConvert.SerializeObject(query);
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}

		/// <summary>
		///  time: 2020-6-30
		///  message:修改角色名称
		///  author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackRole/ChangeRoleName"), HttpPost]
		public IHttpActionResult ChangeRoleName([FromBody]JObject value)
		{
			int result = 0;
			int id = (int)value["Id"];
			string roleName = value["RoleName"].ToString();
			string remarks = value["Remarks"].ToString();
			var query = entities.Role.Where(e => e.Id != id).ToList();
			List<string> list = new List<string>();
			foreach (var item in query)
			{
				string name = item.RoleName;
				list.Add(name);
			}
			foreach (var item in list)
			{
				if (roleName == item)
				{
					return Ok(Respone.No("该角色已存在！"));
				}
			}
			var rid = entities.Role.Where(e => e.Id == id).FirstOrDefault();
			if (rid != null)
			{
				rid.RoleName = roleName;
				rid.Remarks = remarks;
				DbEntityEntry entry = entities.Entry(rid);
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
		/// message:角色id找菜单目录
		/// author：Thomars
		/// </summary>
		/// <param name="RoleId">角色id</param>
		/// <returns></returns>
		[Route("api/BackRole/GetRoleIdByNavigationBarId"), HttpGet]
		public HttpResponseMessage GetRoleIdByNavigationBarId(int RoleId)
		{
			var query = entities.RoleNavigationBar.Where(e => e.RoleId == RoleId).ToList();
			List<int> list = new List<int>();
			int s;
			foreach (var item in query)
			{
				s =(int) item.NavigationBarId;
				list.Add(s);
			}
			var json = JsonConvert.SerializeObject(list);
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}
	}
}
