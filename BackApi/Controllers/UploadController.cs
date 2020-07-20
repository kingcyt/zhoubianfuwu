using AssistantApi.MessageOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace BackApi.Controllers
{
	public class UploadController : ApiController
	{
		/// <summary>
		/// time:2020-7-13
		/// message:图片上传
		///  author:Thomars 
		/// </summary>
		/// <returns></returns>
		[Route("api/Upload/PictureUpload"), HttpPost]
		public IHttpActionResult PictureUpload()
		{
			string pric = "";
			var imageName1 = HttpContext.Current.Request.Files["image"];  // 从前台获取文件
			if (imageName1 != null)
			{
				string file = imageName1.FileName;

				string fileFormat = file.Split('.')[file.Split('.').Length - 1]; // 以“.”截取，获取“.”后面的文件后缀
				Regex imageFormat = new Regex(@"^(bmp)|(png)|(gif)|(jpg)|(jpeg)"); // 验证文件后缀的表达式
				if (string.IsNullOrEmpty(file) || !imageFormat.IsMatch(fileFormat)) // 验证后缀，判断文件是否是所要上传的格式
				{
					pric = "";   //没有上传图片
					return Ok(Respone.No("图片格式不正确"));
				}
				else
				{

					string timeStamp = DateTime.Now.Ticks.ToString(); // 获取当前时间的string类型
					string firstFileName = timeStamp.Substring(0, timeStamp.Length - 4); // 通过截取获得文件名
					string imageStr = "Images/AddServiceImage/"; // 获取保存图片的项目文件夹
					string uploadPath = HttpContext.Current.Server.MapPath("~/" + imageStr); // 将项目路径与文件夹合并
					string pictureFormat = file.Split('.')[file.Split('.').Length - 1];// 设置文件格式
					string fileName = firstFileName + "." + fileFormat;// 设置完整（文件名+文件格式） 
					string saveFile = uploadPath + fileName;//文件路径
					imageName1.SaveAs(saveFile);// 保存文件
					pric = imageStr + fileName;// 设置数据库保存图片的路径
					return Ok(Respone.Success(pric, "上传成功"));
				}
			}
			pric = "";   //没有上传图片
			return Ok(Respone.No("请上传图片"));
		}
	}
}
