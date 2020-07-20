using BackApi.MessageOut;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Security;

namespace BackApi.Controllers
{
    public class BackLoginController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-6-29
		/// message:后台用户登录
		/// author:Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackLogin/UserLogin"), HttpPost]
		public IHttpActionResult UserLogin([FromBody]JObject value)
		{
			int result = 0;
			var name = string.Empty;
			var password = string.Empty;
			if (value != null)
			{
				if (value["LoginName"] == null)
				{
					return Ok(Respone.No("用户名不能为空"));
				}
				name = value["LoginName"].ToString();
				if (value["Password"] == null)
				{
					return Ok(Respone.No("密码不能为空"));
				}
				password = value["Password"].ToString();

			}
			else
			{
				return null;
			}
			var pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
			var user = entities.BackUser.Where(u => u.LoginName == name && u.Password == pwd && u.State ==1).FirstOrDefault();
			if (user == null)
			{
				return Ok(Respone.No("用户名或者密码错误"));
			}
			else
			{
				//Dictionary<int, string> us = new Dictionary<int, string>();
				//us.Add(user.Id, user.Name);
				
				//var json = JsonConvert.SerializeObject(us);
				//var s=json.Replace("\"", "");
				return Ok(Respone.Success(user.Id+","+user.Name, "登录成功"));
			}

		}
	}
}
