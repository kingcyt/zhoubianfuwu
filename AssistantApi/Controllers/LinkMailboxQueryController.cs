using AssistantApi.Common;
using AssistantApi.MessageOut;
using AssistantApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace AssistantApi.Controllers
{
    public class LinkMailboxQueryController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-14
		/// message:单个链接查邮箱
		/// author:Thomars 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/LinkMailboxQuery/AddLinkMailboxQuery"), HttpPost]
		public IHttpActionResult AddLinkMailboxQuery([FromBody]JObject value)
		{
			int uid = (int)value["Uid"];
			int countryId = (int)value["CountryId"];
			string reviewId = value["ReviewId"].ToString();
			decimal price = (decimal)value["Price"];

			LinkMailboxQuery mailboxQuery = new LinkMailboxQuery
			{
				UserId = uid,
				State = 1,
				ReviewLink = reviewId,
				SubTime = DateTime.Now,
				Price = price,
				CountryId = countryId
			};
			entities.LinkMailboxQuery.Add(mailboxQuery);
			int result = entities.SaveChanges();
			if (result > 0)
			{
				return Ok(Respone.Success("提交成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		/// <summary>
		/// time:2020-7-14
		/// message:链接查邮箱列表
		/// author:Thomars  
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="KeyWord"></param>
		/// <param name="State"></param>
		/// <returns></returns>
		[Route("api/LinkMailboxQuery/GetLinkMailboxQuery"), HttpGet]
		public HttpResponseMessage GetLinkMailboxQuery(int UserId, int pageNum, int pagesize, string KeyWord, int State)
		{
			var json = string.Empty;
			var query = from l in entities.LinkMailboxQuery
				    join u in entities.AroundUser on l.UserId equals u.Id
				    join c in entities.Country on l.CountryId equals c.Id
				    where l.UserId == UserId
				    select new { l.Id,l.ReviewLink,l.State,l.Grade,l.Comments,l.SellerName,l.BuyerEmail,l.BuyerName,l.SubTime, c.CountryName };
			if (string.IsNullOrWhiteSpace(KeyWord) == false)
			{
				query = query.Where(e => e.ReviewLink.Contains(KeyWord));
			}
			if (State > 0)
			{
				query = query.Where(e => e.State == State);
			}
			int count = query.Count();
			var rows = query.OrderByDescending(c => c.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e => new
			{
				Id = e.Id,
				CountryName = e.CountryName,
				ReviewLink = e.ReviewLink,
				State = e.State,
				Grade = e.Grade,
				Comments = e.Comments,
				SellerName = e.SellerName,
				BuyerName = e.BuyerName,
				BuyerEmail = e.BuyerEmail,
				SubTime = e.SubTime.Value.ToString("yyyy-MM-dd hh:mm:ss"),
			}).ToList();

			json = JsonConvert.SerializeObject(rows);
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
		/// time:2020-7-14
		/// message:链接查邮箱导入
		/// author:Thomars 
		/// </summary>
		/// <returns></returns>
		[Route("api/LinkMailboxQuery/ExcelLinkMailboxQuery"), HttpPost]
		public IHttpActionResult ExcelLinkMailboxQuery()
		{
			try
			{

				string uid = HttpContext.Current.Session["UserId"].ToString();
				int s = 0;
				int sum = 0;
				var pric = entities.NavBySubmeun.Where(e => e.ModularId == 12).Select(e => e.Price).FirstOrDefault();
				var filelist = HttpContext.Current.Request.Files;
				var bad = new List<ExcelLinkMailboxQueryModel>();
				var linkMail = new List<LinkMailboxQuery>();
				if (filelist.Count > 0)
				{
					for (var i = 0; i < filelist.Count; i++)
					{
						var file = filelist[i];
						var dataTable = ExcelHelp.ExcelToTableForXLSX(file.InputStream);//excel转成datatable
						bad = dataTable.ToDataList<ExcelLinkMailboxQueryModel>();//datatable转成list
					}
				}
				sum = bad.Count();
				foreach (var item in bad)
				{

					int cname = entities.Country.Where(e => e.CountryName == item.国家.Trim()).Select(e => e.Id).FirstOrDefault();
					if (cname > 0 && uid != null)
					{
						var box = new LinkMailboxQuery
						{
							UserId = Convert.ToInt32(uid),
							State = 1,
							Price = pric,
							ReviewLink = item.Review,
							CountryId = cname,
							SubTime = DateTime.Now
						};
						linkMail.Add(box);
						s++;
					}
					else
					{

						return Ok(Respone.No("EXCEL数据有误"));
					}
				}
				if (s == sum)
				{
					var dt = linkMail.ToDataTable();
					if (string.IsNullOrWhiteSpace(dt.TableName))
						dt.TableName = "LinkMailboxQuery";
					SqlBulkCopyHelper.SaveTable(dt);//批量插入
					var list = new { succeed = linkMail.Take(100).ToList() };
					return Ok(Respone.Success("导入成功"));
				}
				else
				{
					return Ok(Respone.No("导入失败"));
				}
			}
			catch (Exception ex)
			{
				return Ok(ex.ToString());
			}
		}
	}
}
