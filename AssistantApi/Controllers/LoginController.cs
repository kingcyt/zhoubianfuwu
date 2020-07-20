using AssistantApi.Common;
using AssistantApi.MessageOut;
using EntitiesModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace AssistantApi.Controllers
{
    public class LoginController : ApiController
    {
		APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-2
		/// message: 登录
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/Login/UserLogin"), HttpPost]
		public IHttpActionResult UserLogin([FromBody]JObject value)
		{
			
			int result = 0;
			var phone = value["Phone"].ToString();
			var password = value["Password"].ToString();
			var code = value["Code"].ToString();
			if (Common.Cookie.getcookie("Code").ToUpper() != code.ToUpper())
			{
				return Ok(Respone.No("验证码不正确"));
			}

			var pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
			var user = entities.AroundUser.Where(u => u.Phone == phone && u.PassWord == pwd && u.Enable==1).FirstOrDefault();
			if (user == null)
			{
				return Ok(Respone.No("用户名或者密码错误"));
			}
			else
			{
				System.Web.HttpContext.Current.Session["UserId"] = user.Id;
				return Ok(Respone.Success(user.Id + "," + user.Name, "登录成功"));
			}

		}

		/// <summary>
		/// time: 2020-7-2
		/// message: 获取获取验证码
		/// author：Thomars
		/// </summary>
		/// <returns></returns>
		[Route("api/Login/CreateCheckCodeImage"), HttpGet]
		public HttpResponseMessage CreateCheckCodeImage()
		{

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			string checkCode = VierificationCodeServices.RndNum();
			var image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);  //12.5    22
			var g = Graphics.FromImage(image);

			try
			{
				//生成随机生成器
				var random = new Random();

				//清空图片背景色
				g.Clear(Color.White);

				//画图片的背景噪音线
				for (var i = 0; i < 25; i++)
				{
					var x1 = random.Next(image.Width);
					var x2 = random.Next(image.Width);
					var y1 = random.Next(image.Height);
					var y2 = random.Next(image.Height);

					g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
				}

				var font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
				var brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
				g.DrawString(checkCode, font, brush, 2, 2);

				//画图片的前景噪音点
				for (var i = 0; i < 100; i++)
				{
					var x = random.Next(image.Width);
					var y = random.Next(image.Height);

					image.SetPixel(x, y, Color.FromArgb(random.Next()));
				}

				//画图片的边框线
				g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

				var ms = new System.IO.MemoryStream();
				image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
				result.Content = new ByteArrayContent(ms.ToArray());
				result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/Gif");
				Common.Cookie.setcookieCode("Code", checkCode, 5);
				return result;
			}
			finally
			{
				g.Dispose();
				image.Dispose();
			}
		}
	}
}
