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
    public class CountryController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();


		/// <summary>
		/// time: 2020-7-6
		/// message:国家列表展示
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/Country/GetCountryList"), HttpGet]
		public HttpResponseMessage GetCountryList()
		{
			var query = entities.Country.ToList();
			var json = JsonConvert.SerializeObject(query);
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}
    }
}
