using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistantApi.MessageOut
{
	public class Respone
	{
		public string Code { get; set; }

		public string Data { get; set; }

		public string Msg { get; set; }

		public static Respone Success(string msg)
		{
			return new Respone
			{
				Code = "ok",
				Data = string.Empty,
				Msg = msg
			};
		}

		public static Respone Success(string data, string msg)
		{
			return new Respone
			{
				Code = "ok",
				Data = data,
				Msg = msg
			};
		}



		public static Respone Error(string msg)
		{
			return new Respone
			{
				Code = "error",
				Data = string.Empty,
				Msg = msg
			};
		}

		public static Respone No(string msg)
		{
			return new Respone
			{
				Code = "no",
				Data = string.Empty,
				Msg = msg
			};
		}
	}
}