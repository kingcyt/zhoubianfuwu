using EntitiesModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AssistantApi.Common
{
	public class SqlBulkCopyHelper
	{
		static APeripheralServicesEntities entities = new APeripheralServicesEntities();
		public static void SaveTable(DataTable dtTable)
		{
			try
			{
				string connstring = entities.Database.Connection.ConnectionString;
				//var connectionString = ConfigurationManager.ConnectionStrings["APeripheralServicesEntities"].ToString();
				var sbc = new SqlBulkCopy(connstring, SqlBulkCopyOptions.UseInternalTransaction) { BulkCopyTimeout = 5000 };
				sbc.DestinationTableName = dtTable.TableName;
				sbc.WriteToServer(dtTable);
			}
			catch (Exception ex)
			{
				ex.ToString();
				//处理异常
			}
			finally
			{
				//sqlcmd.Clone();
				//srcConnection.Close();
				//desConnection.Close();
			}
		}
	}
}