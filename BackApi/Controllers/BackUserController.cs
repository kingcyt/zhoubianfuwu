using BackApi.MessageOut;
using BackApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Security;

namespace BackApi.Controllers
{
    public class BackUserController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();



		/// <summary>
		///time: 2020-6-29
		/// message:添加后台用户
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackUser/AddBackUser"), HttpPost]
		public IHttpActionResult AddBackUser([FromBody]JObject value)
		{
			int result = 0;
			string name = value["Name"].ToString();
			string loginname = value["LoginName"].ToString();
			string password = value["Password"].ToString();
			string phone = value["Phone"].ToString();
			var pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");

			var query = entities.BackUser.Where(e => e.LoginName == loginname).FirstOrDefault();
			if (query != null)
			{
				return Ok(Respone.No("该账号已存在！"));
			}
			BackUser backUser = new BackUser();
			backUser.Name = name;
			backUser.LoginName = loginname;
			backUser.Password = pwd;
			backUser.Phone = phone;
			backUser.State = 1;
			entities.BackUser.Add(backUser);
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
		///time: 2020-6-29
		///message: 后台用户绑角色
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackUser/AddBackUserByRole"), HttpPost]
		public IHttpActionResult AddBackUserByRole(BackUserRoolModel roolModel)
		{
			int result = 0;
			if (roolModel.RoolId.Length > 0)
			{
				//判断用户所有角色有就删除
				var rid = entities.BackUserRole.Where(e => e.BackUserId == roolModel.Id).ToList();
				if (rid != null)
				{
					foreach (var item in rid)
					{
						entities.BackUserRole.Remove(item);
					}
				}
				BackUserRole backUserRole = new BackUserRole();
				foreach (var item in roolModel.RoolId)
				{


					backUserRole.BackUserId = roolModel.Id;
					backUserRole.RoleId = item;
					entities.BackUserRole.Add(backUserRole);
					result = entities.SaveChanges();
				}
			}
			else
			{
				return Ok(Respone.No("请选择用户角色"));
			}

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
		/// message: 通过用户id查看角色
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackUser/GetBackUserIdByRoleId"), HttpGet]
		public HttpResponseMessage GetBackUserIdByRoleId(int Id)
		{
			
			var roleid = entities.BackUserRole.Where(e => e.BackUserId == Id).ToList();
			List<int> list = new List<int>();
			int s;
			foreach (var item in roleid)
			{
				s = (int)item.RoleId;
				list.Add(s);
			}
			var json= JsonConvert.SerializeObject(list);
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}


		/// <summary>
		/// time: 2020-6-29
		/// message: 角色绑菜单
		/// author：Thomars
		/// </summary>
		/// <param name="roleByNavigationBarModel"></param>
		/// <returns></returns>
		[Route("api/BackUser/AddRoleNavigationBar"), HttpPost]
		public IHttpActionResult AddRoleNavigationBar(RoleByNavigationBarModel roleByNavigationBarModel)
		{
			int result = 0;
			if (roleByNavigationBarModel.NavigationBarId.Length > 0)
			{
				//判断角色所有的菜单有就删除
				var nid = entities.RoleNavigationBar.Where(e => e.RoleId == roleByNavigationBarModel.Id).ToList();
				if (nid != null)
				{
					foreach (var item in nid)
					{
						entities.RoleNavigationBar.Remove(item);
					}
				}
				//在添加
				RoleNavigationBar roleNavigationBar = new RoleNavigationBar();
				foreach (var item in roleByNavigationBarModel.NavigationBarId)
				{
					roleNavigationBar.RoleId = roleByNavigationBarModel.Id;
					roleNavigationBar.NavigationBarId = item;
					entities.RoleNavigationBar.Add(roleNavigationBar);
					result = entities.SaveChanges();
				}
			}
			else
			{
				return Ok(Respone.No("请选择菜单"));
			}

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
		/// time: 2020-6-29
		/// message: 查看用户权限
		/// author：Thomars
		/// </summary>
		/// <param name="Id"></param>
		/// <returns></returns>
		[Route("api/BackUser/GetBackUserNavigationBar"), HttpGet]
		public HttpResponseMessage GetBackUserNavigationBar(int Id)
		{
			IList<Models.NavigationBar> list = new List<Models.NavigationBar>();
			
			string sql = " select distinct n.* from [dbo].[BackUser] AS b left join BackUserRole AS br on b.Id = br.BackUserId left join RoleNavigationBar AS rn on br.RoleId = rn.RoleId left join NavigationBar AS n on rn.NavigationBarId = n.Id  where b.Id = " + Id + " and br.RoleId = rn.RoleId ";
			string connstring = entities.Database.Connection.ConnectionString;
			using (SqlConnection conn = new SqlConnection(connstring))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				SqlDataReader dr = cmd.ExecuteReader();
				
				while (dr.Read())
				{
					Models.NavigationBar model = new Models.NavigationBar();
					model.Id = (int)dr["Id"];
					model.Pid = (int)dr["Pid"];
					model.Path = dr["Path"].ToString();
					model.Icon = dr["Icon"].ToString();
					model.NavigationName = dr["NavigationName"].ToString();
					list.Add(model);
				}
				var ss = list;
			}
			

			List<NavigationBarModel> allModel = new List<NavigationBarModel>();
			foreach (var item in list)
			{
				NavigationBarModel model1 = new NavigationBarModel();
				model1.Id = item.Id;
				model1.NavigationName = item.NavigationName;
				model1.Fid = Convert.ToInt32(item.Pid);
				model1.Path = item.Path;
				model1.Icon = item.Icon;
				allModel.Add(model1);
			}

			var topItems = allModel.Where(e => e.Fid == 0).ToList(); //顶级分类  
			List<NavigationBarModel> topModels = new List<NavigationBarModel>();

			foreach (var item in topItems)
			{
				NavigationBarModel topModel = new NavigationBarModel();
				topModel.Id = item.Id;
				topModel.NavigationName = item.NavigationName;
				topModel.Fid = item.Fid;
				topModel.Path = item.Path;
				topModel.Icon = item.Icon;
				LoopToAppendChildren(allModel, topModel);
				topModels.Add(topModel);
			}
			string json = JsonConvert.SerializeObject(topModels);

			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}
		public void LoopToAppendChildren(List<NavigationBarModel> allList, NavigationBarModel curItem)
		{
			var subItems = allList.Where(e => e.Fid == curItem.Id).ToList();
			curItem.childs = new List<NavigationBarModel>();
			curItem.childs.AddRange(subItems);
			foreach (var subItem in subItems)
			{
				LoopToAppendChildren(allList, subItem);
			}
		}
		
		/// <summary>
		/// time: 2020-6-30
		/// message: 查看用户列表
		/// author：Thomars
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <returns></returns>
		[Route("api/BackUser/GetBackUserList"), HttpGet]
		public HttpResponseMessage GetBackUserList(string Name, int pageNum, int pagesize)
		{
			int rownum = pagesize * (pageNum - 1);
			string sql;
			int count = 0;
			List<BackUser> list = new List<BackUser>();
			var json = string.Empty;
			
			IList<BackModels> lists = new List<BackModels>();
			if (string.IsNullOrWhiteSpace(Name) == true)
			{
				 sql = "select distinct b2.*,(select stuff((select ',' + r.RoleName from BackUser AS b left join BackUserRole AS bu on b.Id = bu.BackUserId left join[Role] AS r on bu.RoleId = r.Id where b.Id = b2.id for xml path('')),1,1,'')) as RoleName  from BackUser AS b2 left join BackUserRole AS bu on b2.Id = bu.BackUserId left join[Role] AS r on bu.RoleId = r.Id";
			}
			else
			{
				sql = "select distinct b2.*,(select stuff((select ',' + r.RoleName from BackUser AS b left join BackUserRole AS bu on b.Id = bu.BackUserId left join[Role] AS r on bu.RoleId = r.Id where b.Id = b2.id for xml path('')),1,1,'')) as RoleName  from BackUser AS b2 left join BackUserRole AS bu on b2.Id = bu.BackUserId left join[Role] AS r on bu.RoleId = r.Id  where b2.Name like '%" + Name +"%'";
			}
			sql += " order by Id desc  OFFSET " + rownum + " ROWS FETCH NEXT " + pagesize + " ROWS ONLY ";
			if (string.IsNullOrWhiteSpace(Name) == true)
			{
				count = entities.BackUser.Count();
				string connstring = entities.Database.Connection.ConnectionString;
				using (SqlConnection conn = new SqlConnection(connstring))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand(sql, conn);
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						BackModels model = new BackModels();
						model.Id = (int)dr["Id"];
						model.LoginName = dr["LoginName"].ToString();
						model.Name = dr["Name"].ToString();
						model.Phone = dr["Phone"].ToString();
						model.State = (int)dr["State"];
						model.Remarks = dr["Remarks"].ToString();
						model.RoleName = dr["RoleName"].ToString();
						lists.Add(model);

					}

				}

			}
			else
			{
				count = entities.BackUser.Where(e=>e.Name.Contains(Name)).Count();
				string connstring = entities.Database.Connection.ConnectionString;
				using (SqlConnection conn = new SqlConnection(connstring))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand(sql, conn);
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						BackModels model = new BackModels();
						model.Id = (int)dr["Id"];
						model.LoginName = dr["LoginName"].ToString();
						model.Name = dr["Name"].ToString();
						model.Phone = dr["Phone"].ToString();
						model.State = (int)dr["State"];
						model.Remarks = dr["Remarks"].ToString();
						model.RoleName = dr["RoleName"].ToString();
						lists.Add(model);

					}

				}
			}
			json = JsonConvert.SerializeObject(lists);
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

		/// <summary>
		/// time: 2020-6-30
		/// message: 禁用启用用户
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/BackUser/ChangeBackUserState"), HttpPost]
		public IHttpActionResult ChangeBackUserState([FromBody]JObject value)
		{
			
			int result = 0;
			int id = (int)value["Id"];
			string name = value["Name"].ToString();
			string phone = value["Phone"].ToString();
			int state = (int)value["State"];
			var bid = entities.BackUser.Where(e => e.Id == id).FirstOrDefault();
			if (bid != null)
			{
				bid.State = state;
				bid.Name = name;
				bid.Phone = phone;
				DbEntityEntry entry = entities.Entry(bid);
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
		/// message: 重置密码
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackUser/ResetPassword"), HttpPost]
		public IHttpActionResult ResetPassword([FromBody]JObject value)
		{
			int result = 0;
			int id = (int)value["Id"];
			string password = value["Password"].ToString();
			if (string.IsNullOrWhiteSpace(password) == true)
			{
				return Ok(Respone.No("请输入密码"));
			}
			else
			{
				var pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
				var bid = entities.BackUser.Where(e => e.Id == id).FirstOrDefault();
				if (bid != null)
				{
					bid.Password = pwd;
					DbEntityEntry entry = entities.Entry(bid);
					entry.State = System.Data.Entity.EntityState.Modified;
					result = entities.SaveChanges();
				}
			}
			if (result > 0)
			{
				return Ok(Respone.Success("密码重置成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

    }
}
