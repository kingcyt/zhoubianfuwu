using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantApi.Common
{
	public class Cookie
	{
		/// <summary>
		/// time: 2020-7-2
		/// message:更新cookie
		/// author：Thomars
		/// </summary>
		/// <param name="cname"></param>
		/// <returns></returns>
		public static void setcookieCode(string cname, string value, int exMM)
		{
			delcookie(cname);
			if (exMM > 0)  //0则是会话
				HttpContext.Current.Response.Cookies[cname].Expires = DateTime.Now.AddMinutes(exMM);
			HttpContext.Current.Response.Cookies[cname].Value = HttpUtility.UrlEncode(value, System.Text.Encoding.GetEncoding("utf-8"));
		}
		/// <summary>
		/// time: 2020-7-2
		/// message: 删除cookie
		/// author：Thomars
		/// </summary>
		/// <param name="cname"></param>
		public static void delcookie(string cname)
		{
			HttpCookie cok = HttpContext.Current.Request.Cookies[cname];
			if (cok != null)
			{
				TimeSpan ts = new TimeSpan(-1, 0, 0, 0);
				cok.Expires = DateTime.Now.Add(ts);//删除整个Cookie，只要把过期时间设置为现在
				HttpContext.Current.Response.AppendCookie(cok);
			}
		}
		/// <summary>
		/// time: 2020-7-2
		/// message: 获取cookie
		/// author：Thomars
		/// </summary>
		/// <param name="cname"></param>
		/// <returns></returns>
		public static string getcookie(string cname)
		{
			if (HttpContext.Current.Request.Cookies[cname] != null)
			{
				return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[cname].Value, System.Text.Encoding.GetEncoding("utf-8"));
			}
			return "";
		}
	}
}