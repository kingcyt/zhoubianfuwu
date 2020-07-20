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
    public class BackCountryController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-1
		/// message:添加国家
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackCountry/AddCountry"), HttpPost]
		public IHttpActionResult AddCountry([FromBody]JObject value)
		{
			int result = 0;
			string countryName = value["CountryName"].ToString();
			string counteyShorthand = value["CounteyShorthand"].ToString();

			var query = entities.Country.Where(e => e.CountryName == countryName || e.CounteyShorthand == counteyShorthand).FirstOrDefault();
			if (query != null)
			{
				return Ok(Respone.No("国家名称或简写已存在！"));
			}
			Country country = new Country();
			country.CountryName = countryName;
			country.CounteyShorthand = counteyShorthand;
			country.State = 1;
			entities.Country.Add(country);
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
		/// message:国家列表展示
		/// author：Thomars
		/// </summary>
		/// <param name="CountryName"></param>
		/// <returns></returns>
		[Route("api/BackCountry/GetCountry"), HttpGet]
		public HttpResponseMessage GetCountry(string CountryName)
		{
			var json = string.Empty;
			if (string.IsNullOrWhiteSpace(CountryName) == true)
			{
				var query = entities.Country.ToList();
				json = JsonConvert.SerializeObject(query);
			}
			else
			{
				var query = entities.Country.Where(e=>e.CountryName.Contains(CountryName)).ToList();
				json = JsonConvert.SerializeObject(query);
			}
			
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}

		/// <summary>
		/// time: 2020-7-1
		/// message:修改国家信息
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackCountry/ChangeCountry"), HttpPost]
		public IHttpActionResult ChangeCountry([FromBody]JObject value)
		{
			int result = 0;
			string countryName = value["CountryName"].ToString();
			string counteyShorthand = value["CounteyShorthand"].ToString();
			int state = (int)value["State"];
			int id = (int)value["Id"];
			var query = entities.Country.Where(e => e.Id != id).ToList();
			Dictionary<string, string> d = new Dictionary<string, string>();
			foreach (var item in query)
			{
				string name = item.CountryName;
				string shorthand = item.CounteyShorthand;
				d.Add(name, shorthand);
				
			}
			foreach (var item in d)
			{
				if (countryName == item.Key || counteyShorthand == item.Value)
				{
					return Ok(Respone.No("已存在国家或简称"));
				}
			}

			var cid = entities.Country.Where(e => e.Id == id).FirstOrDefault();
			if (cid != null)
			{
				cid.CounteyShorthand = counteyShorthand;
				cid.CountryName = countryName;
				cid.State = state;

				DbEntityEntry entry = entities.Entry(cid);
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
    }
}
