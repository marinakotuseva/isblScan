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
	public class Report : LoaderCommon
	{
		public Report(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		private List<isblTest.Node> LoadGroups(isblTest.Node rootNode)
		{
			List<isblTest.Node> listGroups = new List<Node>();
			if(this.checkTableExist("MBRegUnit"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select t.id, t.name from (select MBRegUnit.RegUnit [id], Max(MBRegUnit.Name) [name] from MBRegUnit join MBReports on (MBRegUnit.RegUnit = MBReports.RegUnit) where MBReports.TypeRpt='MBAnAccRpt' group by MBRegUnit.RegUnit) t order by t.name";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						isblTest.Node node = new isblTest.Node();
						node.parent = rootNode;
						node.id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.name = reader.GetString(1);
						}
						node.nodes = new List<isblTest.Node>();
						rootNode.nodes.Add(node);
						listGroups.Add(node);
					}
				}
				reader.Close();
			}
			return listGroups;
		}

		public isblTest.Node Load()
		{
			isblTest.Node listNode = null;
			if(this.checkTableExist("MBReports"))
			{
				listNode = new isblTest.Node();
				listNode.name = "Аналитический отчёт";
				listNode.text = null;
				listNode.parent = null;
				listNode.nodes = new List<Node>();
				
				List<isblTest.Node> listGroups = LoadGroups(listNode);
				foreach(isblTest.Node groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select XRecID, NameRpt, Comment, Exprn, Report, Viewer from MBReports where TypeRpt='MBAnAccRpt' and RegUnit=@groupID order by NameRpt ASC";
					SqlParameter paramGroupID = new SqlParameter("@groupID", SqlDbType.Int, 10);
					paramGroupID.Value = groupNode.id;
					command.Parameters.Add(paramGroupID);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							isblTest.Node reportNode = new isblTest.Node();
							reportNode.parent = groupNode;
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
							groupNode.nodes.Add(reportNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}

	}
}
