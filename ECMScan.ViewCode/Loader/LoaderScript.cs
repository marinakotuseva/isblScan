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
	/// Расчёт (сценарий).
	/// </summary>
	public class Script : LoaderCommon
	{
		public Script(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		private List<isblTest.Node> LoadGroups(isblTest.Node rootNode)
		{
			List<isblTest.Node> listGroups = new List<Node>();
			if(this.checkTableExist("MBRegUnit"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select t.id, t.name from (select MBRegUnit.RegUnit [id], Max(MBRegUnit.Name) [name] from MBRegUnit join MBReports on (MBRegUnit.RegUnit = MBReports.RegUnit) where MBReports.TypeRpt='Function' group by MBRegUnit.RegUnit) t order by t.name";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						isblTest.Node node = new isblTest.Node();
						node.Parent = rootNode;
						node.Id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.Name = reader.GetString(1);
						}
						node.Nodes = new List<isblTest.Node>();
						rootNode.Nodes.Add(node);
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
				listNode.Name = "Сценарий (расчёт)";
				listNode.Text = null;
				listNode.Parent = null;
				listNode.Nodes = new List<Node>();
				
				List<isblTest.Node> listGroups = LoadGroups(listNode);
				foreach(isblTest.Node groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select XRecID, NameRpt, Comment, Report from MBReports where TypeRpt='Function' and RegUnit=@groupID order by NameRpt";
					SqlParameter paramGroupID = new SqlParameter("@groupID", SqlDbType.Int);
					paramGroupID.Value = groupNode.Id;
					command.Parameters.Add(paramGroupID);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							isblTest.Node scriptNode = new isblTest.Node();
							scriptNode.Parent = groupNode;
							scriptNode.Id = reader.GetInt32(0);
							if(! reader.IsDBNull(1))
							{
								scriptNode.Name = reader.GetString(1);
							}
							if(! reader.IsDBNull(2))
							{
								scriptNode.Text = reader.GetString(2);
							}
							scriptNode.Nodes = new List<isblTest.Node>();
							
							if(! reader.IsDBNull(3))
							{
								SqlBytes sqlbytes = reader.GetSqlBytes(3);
								System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
								string scriptText = win1251.GetString(sqlbytes.Value);
								isblTest.Node scriptTextNode = new isblTest.Node();
								scriptTextNode.Name = "-=[ Текст сценария ]=-";
								scriptTextNode.Text = scriptText;
								scriptTextNode.Parent = scriptNode;
								scriptNode.Nodes.Add(scriptTextNode);
							}
							groupNode.Nodes.Add(scriptNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}
	}
}
