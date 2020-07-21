using AssistantApi.MessageOut;
using BackApi.Models;
using EntitiesModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace BackApi.Controllers
{
    public class BackAddShopCartController : ApiController
    {
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time:2020-7-16
		/// message:加购列表
		/// author:Thomars   
		/// </summary>
		/// <param name="pageNum"></param>
		/// <param name="pagesize"></param>
		/// <param name="Statetime"></param>
		/// <param name="Endtime"></param>
		/// <param name="State"></param>
		/// <param name="keyWord"></param>
		/// <returns></returns>
		[Route("api/BackAddShopCart/GetBackAddShopCart"), HttpGet]
		public HttpResponseMessage GetBackAddShopCart(int pageNum, int pagesize, string Statetime, string Endtime, int State, string keyWord)
		{
			int count = Sum(Statetime, Endtime, State, keyWord);
			var list = UserByAddShopCartList(pageNum, pagesize, Statetime, Endtime, State, keyWord);
			var json = JsonConvert.SerializeObject(list);
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
		public int Sum(string Statetime, string Endtime, int State, string keyWord)
		{
			string where = "";
			string sql = "select count(l.Id) from [dbo].[AddShopCart] AS l left join[dbo].[AroundUser] AS a on l.UserId=a.Id left join[dbo].[Country] AS c on l.CountryId= c.Id where 1=1 ";
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				where += "  and l.AddTimes >='" + Statetime + "' and l.AddTimes<= '" + Endtime + "'";
			}
			if (State > 0)
			{
				where += "  and l.State=" + State;
			}
			if (string.IsNullOrWhiteSpace(keyWord) == false)
			{
				where += " and  (a.Name like '%" + keyWord + "%' or a.Phone like '%" + keyWord + "%')";
			}
			if (string.IsNullOrWhiteSpace(where) == false)
			{
				sql += where;
			}
			string connstring = entities.Database.Connection.ConnectionString;
			using (SqlConnection conn = new SqlConnection(connstring))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				int result = int.Parse(cmd.ExecuteScalar().ToString());
				return result;
			}
		}

		public static IList<UserByAddShopCartModel> UserByAddShopCartList(int pageNum, int pagesize, string Statetime, string Endtime, int State, string keyWord)
		{
			int rownum = pagesize * (pageNum - 1);
			string where = "";
			string sql = "select l.*,a.CustomerNumber,a.Name,a.Phone,c.CountryName from [dbo].[AddShopCart] AS l left join[dbo].[AroundUser] AS a on l.UserId=a.Id left join[dbo].[Country] AS c on l.CountryId= c.Id where 1=1 ";
			if (string.IsNullOrWhiteSpace(Statetime) == false)
			{
				where += "  and l.AddTimes >='" + Statetime + "' and l.AddTimes<= '" + Endtime + "'";
			}
			if (State > 0)
			{
				where += "  and l.State=" + State;
			}
			if (string.IsNullOrWhiteSpace(keyWord) == false)
			{
				where += " and  (a.Name like '%" + keyWord + "%' or a.Phone like '%" + keyWord + "%')";
			}
			if (string.IsNullOrWhiteSpace(where) == false)
			{
				sql += where;
			}
			sql += " order by Id desc  OFFSET " + rownum + " ROWS FETCH NEXT " + pagesize + " ROWS ONLY ";
			string connstring = entities.Database.Connection.ConnectionString;
			using (SqlConnection conn = new SqlConnection(connstring))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				SqlDataReader dr = cmd.ExecuteReader();
				IList<UserByAddShopCartModel> list = new List<UserByAddShopCartModel>();
				while (dr.Read())
				{
					UserByAddShopCartModel model = new UserByAddShopCartModel();
					model.Id = (int)dr["Id"];
					model.UserId = (int)dr["UserId"];
					model.State = (int)dr["State"];
					model.Price = dr["Price"].ToString();
					model.CustomerNumber = dr["CustomerNumber"].ToString();
					model.Name = dr["Name"].ToString();
					model.Phone = dr["Phone"].ToString();
					model.CountryName = dr["CountryName"].ToString();
					model.ShopId = dr["ShopId"].ToString();
					model.Number = (int)dr["Number"];
					model.Actual = dr["Actual"].ToString();
					model.ASIN = dr["ASIN"].ToString();
					model.TotalPrice = dr["TotalPrice"].ToString();
					model.Images = dr["Images"].ToString();
					model.KeyWord = dr["KeyWord"].ToString();
					model.Productlocation = dr["Productlocation"].ToString();
					model.AddTimes = dr["AddTimes"].ToString();
					model.Remarks = dr["Remarks"].ToString();
					list.Add(model);
				}
				return list;
			}
		}

		/// <summary>
		/// time:2020-7-16
		/// message:修改加购状态或修改价格
		/// author:Thomars 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[Route("api/BackAddShopCart/ChangeUserByAddShopCartStateOrMoney"), HttpPost]
		public IHttpActionResult ChangeUserByAddShopCartStateOrMoney(ChangeBadEvaluateModel model)
		{
			int result = 0;
			if (model.State == 1)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.AddShopCart.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.State = 2;
						//DbEntityEntry entry = entities.Entry(query);
						//entry.State = System.Data.Entity.EntityState.Modified;
						result = entities.SaveChanges();
					}
				}
			}
			if (model.State == 2)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.AddShopCart.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.State = 4;
						//DbEntityEntry entry = entities.Entry(query);
						//entry.State = System.Data.Entity.EntityState.Modified;
						result = entities.SaveChanges();
					}
				}
			}
			if (model.State == 3)
			{
				for (int i = 0; i < model.Id.Length; i++)
				{
					int c = model.Id[i];
					var query = entities.AddShopCart.Where(e => e.Id == c).FirstOrDefault();
					if (query != null)
					{
						query.Price = model.Price;
						//DbEntityEntry entry = entities.Entry(query);
						//entry.State = System.Data.Entity.EntityState.Modified;
						result = entities.SaveChanges();
					}
				}
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

		/// <summary>
		/// time:2020-7-16
		/// message:加购上传文件
		/// author:Thomars  
		/// </summary>
		/// <returns></returns>
		[Route("api/BackAddShopCart/UserByAddShopCartPictureUpload"), HttpPost]
		public IHttpActionResult UserByAddShopCartPictureUpload()
		{
			string pric = "";
			var imageName1 = HttpContext.Current.Request.Files["image"];  // 从前台获取文件
			if (imageName1 != null)
			{
				string file = imageName1.FileName;

				string fileFormat = file.Split('.')[file.Split('.').Length - 1]; // 以“.”截取，获取“.”后面的文件后缀
												 //Regex imageFormat = new Regex(@"^(ZIP)|(RAR)|(7Z)|(CAB)"); // 验证文件后缀的表达式
				if (string.IsNullOrEmpty(file)) // 验证后缀，判断文件是否是所要上传的格式
				{
					pric = "";   //没有上传图片
					return Ok(Respone.No("上传文件名称不能为空"));
				}
				else
				{

					string timeStamp = DateTime.Now.Ticks.ToString(); // 获取当前时间的string类型
					string firstFileName = timeStamp.Substring(0, timeStamp.Length - 4); // 通过截取获得文件名
					string imageStr = "/Images/UploadFiles/"; // 获取保存图片的项目文件夹
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
			return Ok(Respone.No("请上传文件"));
		}

		/// <summary>
		/// time:2020-7-16
		/// message:End加购
		/// author:Thomars 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackAddShopCart/EndAddShopCartTask"), HttpPost]
		public IHttpActionResult EndAddShopCartTask([FromBody]JObject value)
		{
			try
			{
				int result = 0;
				decimal total = 0;
				int id = (int)value["Id"];
				int number = (int)value["Number"];
				var query = entities.AddShopCart.Where(e => e.Id == id).FirstOrDefault();
				if (query != null)
				{
					using (TransactionScope scope = new TransactionScope())
					{
						total = Convert.ToDecimal(query.Price) * number;
						query.State = 3;
						query.TotalPrice = total;
						query.Actual = number;
						DbEntityEntry entry = entities.Entry(query);
						entry.State = System.Data.Entity.EntityState.Modified;

						//账户扣款
						var customerFinance = entities.AroundUserFinance.Where(t => t.UserId == query.UserId).FirstOrDefault();
						customerFinance.AccountBalance = customerFinance.AccountBalance - query.TotalPrice;
						customerFinance.AccumulatedExpenditure = customerFinance.AccumulatedExpenditure + query.TotalPrice;
						DbEntityEntry entryc = entities.Entry(customerFinance);
						entryc.State = System.Data.Entity.EntityState.Modified;

						//日志
						TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
						AroundUserFinanceLog log = new AroundUserFinanceLog
						{
							BusinessNumber = Convert.ToInt64(ts.TotalMilliseconds).ToString(),
							UserId = query.UserId,
							PaymentState = 21,
							TransactionType = 1,  //1扣款,2:充值,3退款
							TransactionTime = DateTime.Now,
							TransactionAmount = query.TotalPrice,
							Remarks = "任务：" + query.Id + " 加购总额支出"

						};
						entities.AroundUserFinanceLog.Add(log);
						result = entities.SaveChanges();
						scope.Complete();
					}
				}
				if (result > 0)
				{
					return Ok(Respone.Success("加购任务已完成"));
				}
				else
				{
					return Ok(Respone.No("发生了点问题，请稍后再试"));
				}
			}
			catch (Exception ex)
			{
				return Ok(ex.ToString());
			}
		}

		/// <summary>
		/// time:2020-7-16
		/// message:加购文件地址
		/// author:Thomars 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/BackAddShopCart/AddShopCartFiles"), HttpPost]
		public IHttpActionResult AddShopCartFiles([FromBody]JObject value)
		{
			int result = 0;
			int id = (int)value["Id"];
			string files = value["Files"].ToString();
			var query = entities.AddShopCart.Where(e => e.Id == id).FirstOrDefault();
			if (query != null)
			{
				query.Images = files;
				result = entities.SaveChanges();
			}
			if (result > 0)
			{
				return Ok(Respone.Success("文件上传成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}
	}
}
