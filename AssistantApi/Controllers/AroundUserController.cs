using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using AssistantApi.MessageOut;
using EntitiesModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace AssistantApi.Controllers
{
    public class AroundUserController : ApiController
    {
		APeripheralServicesEntities entities = new APeripheralServicesEntities();

		/// <summary>
		/// time: 2020-7-2
		/// message: 注册
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/AroundUser/Register"), HttpPost]
		public IHttpActionResult Register([FromBody]JObject value)
		{
			int result = 0;
			int number = 10000;
			string name = value["Name"].ToString();
			string phone = value["Phone"].ToString();
			string passWord = value["PassWord"].ToString();
			var pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(passWord, "MD5");
			string phoneCode = value["PhoneCode"].ToString();
			if (Common.Cookie.getcookie("Code") != phoneCode)
			{
				return Ok(Respone.No("手机验证码错误"));
			}
			var user = entities.AroundUser.Where(e => e.Phone == phone).FirstOrDefault();
			if (user != null)
			{
				return Ok(Respone.No("手机号已存在"));
			}
			else
			{
				int count = entities.AroundUser.Count();
				if (count > 0)
				{
					var lastId = entities.AroundUser.AsEnumerable().Last().CustomerNumber;
					number = (int)lastId + 1;
				}
				AroundUser aroundUser = new AroundUser
				{
					Phone = phone,
					Name = name,
					Enable = 1,
					PassWord = pwd,
					CustomerNumber = number,
					RegistrationTime = DateTime.Now
				};
				entities.AroundUser.Add(aroundUser);
				result = entities.SaveChanges();
				if (result > 0)
				{
					AroundUserFinance aroundUserFinance = new AroundUserFinance
					{
						UserId = aroundUser.Id,
						AccountBalance = 0,
						AccumulatedIncome = 0,
						AccumulatedExpenditure = 0
					};
					entities.AroundUserFinance.Add(aroundUserFinance);
					result = entities.SaveChanges();
				}

			}
			if (result > 0)
			{
				return Ok(Respone.Success("注册成功"));
			}
			else
			{
				return Ok(Respone.No("发生了点问题，请稍后再试"));
			}
		}

		/// <summary>
		/// time: 2020-7-2
		/// message: 密码重置
		/// author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/AroundUser/ResetPwd"), HttpPost]
		public IHttpActionResult ResetPwd([FromBody]JObject value)
		{
			int result = 0;
			int id = (int)value["Id"];
			string oldPassWord = value["OldPassWord"].ToString();
			string newPassWord = value["NewPassWord"].ToString();
			string confirmPassWord = value["ConfirmPassWord"].ToString();

			if (newPassWord != confirmPassWord)
			{
				return Ok(Respone.No("新密码不一致"));
			}
			var pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(oldPassWord, "MD5");
			
			var user = entities.AroundUser.Where(e => e.Id == id && e.PassWord == pwd).FirstOrDefault();
			if (user == null)
			{
				return Ok(Respone.No("原密码不正确"));
			}
			var newpwd = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassWord, "MD5");
			user.PassWord = newpwd;
			result = entities.SaveChanges();
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
		///  time: 2020-7-2
		///  message: 注册发送验证码
		///  author：Thomars
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[Route("api/AroundUser/zCSend"), HttpPost]
		public IHttpActionResult zCSend([FromBody]JObject value)
		{

			string phone = value["Phone"].ToString();
			return Sendmessage(phone, "SMS_190095485");
		}

		
		/// <summary>
		/// time: 2020-7-2
		/// message: 短信验证码
		/// author：Thomars
		/// </summary>
		/// <param name="cname"></param>
		/// <returns></returns>
		public string Code()
		{
			ArrayList MyArray = new ArrayList();
			Random random = new Random();
			string str = null;
			//循环的次数     
			int Nums = 4;
			while (Nums > 0)
			{
				int i = random.Next(1, 9);

				if (MyArray.Count < 4)
				{
					MyArray.Add(i);
				}

				Nums -= 1;
			}
			for (int j = 0; j <= MyArray.Count - 1; j++)
			{
				str += MyArray[j].ToString();
			}
			return str;
		}
		/// <summary>
		/// time: 2020-7-2
		/// message: 判断手机号合法
		/// author：Thomars
		/// </summary>
		/// <param name="cname"></param>
		/// <returns></returns>
		public bool IsMobi(string str)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(str, @"^1[3456789]\d{9}$", RegexOptions.IgnoreCase);
		}
		
		/// <summary>
		/// time: 2020-7-2
		///  message:发送短信封装方法
		///  author：Thomars
		/// </summary>
		/// <param name="PhoneNumbers">手机号码</param>
		/// <param name="SignName">短信签名</param>
		/// <param name="TemplateCode">模版编号</param>	
		/// <param name="TemplateParam">模版</param>
		/// <returns></returns>
		public string SendSms(string PhoneNumbers, string SignName, string TemplateCode, string TemplateParam)
		{
			try
			{
				String product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
				String domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
				String accessKeyId = "LTAI4GDFrkcmPmLMV9mPbrxa";//"LTAIYcJupvlyI3Wj";//你的accessKeyId，参考本文档步骤2
				String accessKeySecret = "g8LY92a1cFv0eDHJVOOq7IVMIu2A0E";//"wwWGIV226n7O0Hmxyrah4zDqRq70RO";//你的accessKeySecret，参考本文档步骤2
				IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
				//IAcsClient client = new DefaultAcsClient(profile);
				// SingleSendSmsRequest request = new SingleSendSmsRequest();
				//初始化ascClient,暂时不支持多region（请勿修改）
				DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
				IAcsClient acsClient = new DefaultAcsClient(profile);
				SendSmsRequest request = new SendSmsRequest();

				//必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式，发送国际/港澳台消息时，接收号码格式为00+国际区号+号码，如“0085200000000”
				request.PhoneNumbers = PhoneNumbers;
				//必填:短信签名-可在短信控制台中找到
				request.SignName = SignName;
				//必填:短信模板-可在短信控制台中找到
				request.TemplateCode = TemplateCode;
				//可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
				request.TemplateParam = TemplateParam;// "{\"name\":\"Tom\"， \"code\":\"123\"}";
								      //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
				request.OutId = "yourOutId";//可以忽略
							    //请求失败这里会抛ClientException异常
				SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
				return sendSmsResponse.Message;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public IHttpActionResult Sendmessage(string phone, string TemplateCode)
		{
			try
			{
				var result = string.Empty;
				var stre = HttpContext.Current.Request.InputStream;
				var jsonstr = new StreamReader(stre).ReadToEnd();
				var PhoneNumbers = phone;
				var code = Code();
				bool model = IsMobi(PhoneNumbers);
				var SignName = "触动力";
				var TemplateParam = "{\"code\":\"" + code + "\"}";
				if (model == true)
				{
					var sms = SendSms(PhoneNumbers, SignName, TemplateCode, TemplateParam);//执行发送短信

					if (sms == "OK")//状态去阿里云看看成功状态返回什么 填写返回成功状态的状态值
					{

						//将信息存入cookie
						Common.Cookie.setcookieCode("Code", code, 5);
						return Ok(Respone.Success("发送成功", "验证码已发送"));//短信发送成功，返回到前台验证码  这样就能和前台用户输入的验证码做对比  

					}
					else
					{
						return Ok(Respone.Success("发送失败", sms));

					}
				}
				else
				{
					return Ok(Respone.No("手机号错误"));

				}
			}
			catch (Exception ex)
			{
				return Ok(Respone.No(ex.Message));//系统异常  返回3

			}
		}
    }
}
	

