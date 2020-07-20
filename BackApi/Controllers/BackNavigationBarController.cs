using BackApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BackApi.Controllers
{
    public class BackNavigationBarController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();
		protected DataTable dtTree = new DataTable();
		/// <summary>
		/// time: 2020-6-29
		/// message: 展示菜单列表
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/BackNavigationBar/GetNavigationBarList"), HttpGet]
		public HttpResponseMessage GetNavigationBarList()
		{
			
			var query = entities.NavigationBar.ToList();
			List<NavigationBarModel> allModel = new List<NavigationBarModel>();
			foreach (var item in query)
			{
				NavigationBarModel model = new NavigationBarModel();
				model.Id = item.Id;
				model.NavigationName = item.NavigationName;
				model.Fid = Convert.ToInt32( item.Pid);
				model.Path = item.Path;
				model.Icon = item.Icon;
				allModel.Add(model);
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
    }
}
