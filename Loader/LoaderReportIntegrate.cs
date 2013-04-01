/*
 * Date: 10.08.2012
 * Time: 21:23
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
namespace isblTest
{
	/// <summary>
	/// Отчёт (аналитический отчёт).
	/// </summary>
	public class ReportIntegrate : LoaderCommon
	{
		public ReportIntegrate(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		public isblTest.Node Load()
		{
			isblTest.Node listNode = null;
			if(this.checkTableExist("MBReports"))
			{
				listNode = new isblTest.Node();
				listNode.name = "Интегрированный отчёт";
				listNode.text = null;
				listNode.parent = null;
				listNode.nodes = new List<Node>();
				
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select XRecID, NameRpt, Comment, Exprn, Report, Viewer from MBReports where TypeRpt='MBAnalitV' order by NameRpt ASC";
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							isblTest.Node reportNode = new isblTest.Node();
							reportNode.parent = listNode;
							//ИД отчёта
							reportNode.id = reader.GetInt32(0);
							//Имя отчёта
							if(! reader.IsDBNull(1))
							{
								reportNode.name = reader.GetString(1);
							}
							//Описание отчёта
							if(! reader.IsDBNull(2))
							{
								reportNode.text = reader.GetString(2);
							}
							reportNode.nodes = new List<isblTest.Node>();
							//Шаблон отчёта
							if(! reader.IsDBNull(4))
							{
								SqlBytes sqlbytes = reader.GetSqlBytes(4);
								System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
								string scriptText = win1251.GetString(sqlbytes.Value);
								isblTest.Node reportTextNode = new isblTest.Node();
								reportTextNode.name = "-=[ Шаблон ]=-";
								reportTextNode.text = scriptText;
								reportTextNode.parent = reportNode;
								reportNode.nodes.Add(reportTextNode);
							}
							//Расчёт отчёта
							if(! reader.IsDBNull(3))
							{
								string templateText = reader.GetString(3);
								isblTest.Node reportTemplateNode = new isblTest.Node();
								reportTemplateNode.name = "-=[ Расчёт ]=-";
								reportTemplateNode.text = templateText;
								reportTemplateNode.parent = reportNode;
								reportNode.nodes.Add(reportTemplateNode);
							}
							listNode.nodes.Add(reportNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}

	}
}
