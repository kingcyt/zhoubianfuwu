using EntitiesModel;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AssistantApi.Controllers
{
    public class UsubmenuController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();


		/// <summary>
		/// time: 2020-7-6
		/// message:展示子菜单信息
		/// author：Thomars
		/// </summary>
		/// <param name="ModularId"></param>
		/// <returns></returns>
		[Route("api/Usubmenu/GetSubmenu"), HttpGet]
		public HttpResponseMessage GetSubmenu(int ModularId)
		{
			var query = entities.NavBySubmeun.Where(e => e.ModularId == ModularId).FirstOrDefault();
			var json = JsonConvert.SerializeObject(query);
			return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
		}
    }
}
