using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BackApi.Common
{
	public static class ExcelHelp
	{
		/// <summary>
		/// excel文件流转化成datatable
		/// </summary>
		public static DataTable ExcelToTableForXLSX(Stream fileStream, bool haveNote = false)
		{
			var dt = new DataTable();
			using (var fs = fileStream)
			{
				var xssfworkbook = new XSSFWorkbook(fs);
				var sheet = xssfworkbook.GetSheetAt(0);
				//表头  判断是否包含备注
				var firstRowNum = sheet.FirstRowNum;
				if (haveNote)
				{
					firstRowNum += 1;
				}
				var header = sheet.GetRow(firstRowNum);
				var columns = new List<int>();
				for (var i = 0; i < header.LastCellNum; i++)
				{
					var obj = GetValueTypeForXLSX(header.GetCell(i) as XSSFCell);
					if (obj == null || obj.ToString() == string.Empty)
					{
						dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
						//continue;
					}
					else
						dt.Columns.Add(new DataColumn(obj.ToString()));
					columns.Add(i);
				}
				//数据
				for (var i = firstRowNum + 1; i <= sheet.LastRowNum; i++)
				{
					var dr = dt.NewRow();
					var hasValue = false;
					if (sheet.GetRow(i) == null)
					{
						continue;
					}
					foreach (var j in columns)
					{
						var cell = sheet.GetRow(i).GetCell(j);
						if (cell != null && cell.CellType == CellType.Numeric)
						{
							//NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
							if (DateUtil.IsCellDateFormatted(cell)) //日期类型
							{
								dr[j] = cell.DateCellValue;
							}
							else //其他数字类型
							{
								dr[j] = cell.NumericCellValue;
							}
						}
						else
						{
							dr[j] = GetValueTypeForXLSX(sheet.GetRow(i).GetCell(j) as XSSFCell);
						}
						if (dr[j] != null && dr[j].ToString() != string.Empty)
						{
							hasValue = true;
						}
					}
					if (hasValue)
					{
						dt.Rows.Add(dr);
					}
				}
			}
			return dt;
		}

		/// <summary>
		/// 获取单元格类型(xlsx)
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		private static object GetValueTypeForXLSX(XSSFCell cell)
		{
			if (cell == null)
				return null;
			switch (cell.CellType)
			{

				case CellType.Blank: //BLANK:
					return null;
				case CellType.Boolean: //BOOLEAN:
					return cell.BooleanCellValue;
				case CellType.Numeric: //NUMERIC:
					return cell.NumericCellValue;
				case CellType.String: //STRING:
					return cell.StringCellValue;
				case CellType.Error: //ERROR:
					return cell.ErrorCellValue;
				case CellType.Formula: //FORMULA:
				default:
					return "=" + cell.CellFormula;
			}
		}

		/// <summary>
		/// Convert a List{T} to a DataTable.
		/// </summary>
		public static DataTable ToDataTable<T>(this List<T> items)
		{
			var tb = new DataTable(typeof(T).Name);

			PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (PropertyInfo prop in props)
			{
				Type t = GetCoreType(prop.PropertyType);
				tb.Columns.Add(prop.Name, t);
			}

			foreach (T item in items)
			{
				var values = new object[props.Length];

				for (int i = 0; i < props.Length; i++)
				{
					values[i] = props[i].GetValue(item, null);
				}

				tb.Rows.Add(values);
			}

			return tb;
		}

		public static Type GetCoreType(Type t)
		{
			if (t != null && IsNullable(t))
			{
				if (!t.IsValueType)
				{
					return t;
				}
				else
				{
					return Nullable.GetUnderlyingType(t);
				}
			}
			else
			{
				return t;
			}
		}
		public static bool IsNullable(Type t)
		{
			return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		/// <summary>
		/// DataTable转成List
		/// </summary>
		public static List<T> ToDataList<T>(this DataTable dt)
		{
			var list = new List<T>();
			var plist = new List<PropertyInfo>(typeof(T).GetProperties());
			foreach (DataRow item in dt.Rows)
			{
				var s = Activator.CreateInstance<T>();
				for (var i = 0; i < dt.Columns.Count; i++)
				{
					var info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
					if (info != null)
					{
						try
						{
							if (!Convert.IsDBNull(item[i]))
							{
								object v = null;
								if (info.PropertyType.ToString().Contains("System.Nullable"))
								{
									v = Convert.ChangeType(item[i], Nullable.GetUnderlyingType(info.PropertyType));
								}
								else
								{
									v = Convert.ChangeType(item[i], info.PropertyType);
								}
								info.SetValue(s, v, null);
							}
						}
						catch (Exception ex)
						{
							throw new Exception("字段[" + info.Name + "]转换出错," + ex.Message);
						}
					}
				}
				list.Add(s);
			}
			return list;
		}

	}
}