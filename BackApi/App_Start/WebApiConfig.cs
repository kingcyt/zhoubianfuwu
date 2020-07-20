using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BackApi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API 配置和服务

			// Web API 路由
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
			    name: "DefaultApi",
			    routeTemplate: "api/{controller}/{id}",
			    defaults: new { action = "Index", id = RouteParameter.Optional }
			);
		}
	}
}
