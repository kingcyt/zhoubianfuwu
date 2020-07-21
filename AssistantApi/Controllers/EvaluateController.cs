using AssistantApi.MessageOut;
using AssistantApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web.Http;

namespace AssistantApi.Controllers
{
	public class EvaluateController : ApiController
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		///time: 2020-7-20
		/// message:添加评论
		/// author：Thomars
		/// </summary>
		/// <param name="evaluateModel"></param>
		/// <returns></returns>
		[Route("api/Evaluate/AddEvaluate"), HttpPost]
		public IHttpActionResult AddEvaluate(EvaluateModel evaluateModel)
		{
			try
			{

				int result = 0;
				int sum = 0;
				if (evaluateModel.tieleAndMeaageModels != null && evaluateModel.tieleAndMeaageModels.Length > 0)
				{
					Evaluate e = new Evaluate();
					using (TransactionScope scope = new TransactionScope())
					{
						foreach (var item in evaluateModel.tieleAndMeaageModels)
						{
							e.UserId = evaluateModel.UserId;
							e.CountryId = evaluateModel.CountryId;
							e.ShopId = evaluateModel.ShopId;
							e.Title = item.Title;
							e.Message = item.Message;
							e.ASIN = evaluateModel.Asin;
							e.State = 1;
							e.Price = evaluateModel.Price;
							e.KeyWord = evaluateModel.KeyWord;
							e.Productlocation = evaluateModel.Productlocation;
							e.AddTimes = DateTime.Now;
							e.Remarks = evaluateModel.Remarks;
							entities.Evaluate.Add(e);
							result = entities.SaveChanges();
							sum++;
						}
						scope.Complete();
					}
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
			catch (Exception ex)
			{
				return Ok(Respone.No(ex.ToString()));
			}
		}

		/// <summary>
		///time: 2020-7-20
		/// message:评论列表
		/// author：Thomars 
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="Statetime"></param>
		/// <param name="Endtime"></param>
		/// <param name="keyWord"></param>
		/// <returns></returns>
		[Route("api/Evaluate/GetEvaluate"), HttpGet]
		public HttpResponseMessage GetEvaluate(int UserId, int pageNum, int pagesize, string Statetime, string Endtime, string keyWord)
		{
			var json = string.Empty;
			var query = from e in entities.Evaluate
				    join c in entities.Country on e.CountryId equals c.Id
				    where e.UserId == UserId
				    select new { e.Id, e.ShopId, c.CountryName, e.Title, e.Message, e.ASIN, e.State, e.Price, e.KeyWord, e.Productlocation, e.AddTimes, e.Remarks,e.Images };
			if (string.IsNullOrWhiteSpace(keyWord) == false)
			{
				query = query.Where(e => e.ShopId.Contains(keyWord) || e.ASIN.Contains(keyWord));
			}
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				DateTime startTime = DateTime.Parse(Statetime);
				DateTime endTime = DateTime.Parse(Endtime);
				query = query.Where(e => e.AddTimes >= startTime && e.AddTimes <= endTime);
			}
			int count = query.Count();
			var rows = query.OrderByDescending(e => e.Id).Skip(pagesize * (pageNum - 1)).Take(pagesize).ToList().Select(e => new
			{
				Id = e.Id,
				ShopId = e.ShopId,
				CountryName = e.CountryName,
				Title = e.Title,
				Message = e.Message,
				ASIN = e.ASIN,
				State = e.State,
				Price = e.Price,
				KeyWord = e.KeyWord,
				Images=e.Images,
				Productlocation = e.Productlocation,
				AddTimes = e.AddTimes.Value.ToString("yyyy-MM-dd hh:mm:ss"),
				Remarks = e.Remarks,
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
	}
}
