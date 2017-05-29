/*
 * Date: 10.08.2012
 * Time: 21:23
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Расчёт (сценарий).
	/// </summary>
	public class Script : LoaderCommon
	{
		public Script(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		private List<Node> LoadGroups(Node rootNode)
		{
			List<Node> listGroups = new List<Node>();
			if(this.CheckTableExist("MBRegUnit"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select t.id, t.name from (select MBRegUnit.RegUnit [id], Max(MBRegUnit.Name) [name] from MBRegUnit join MBReports on (MBRegUnit.RegUnit = MBReports.RegUnit) where MBReports.TypeRpt='Function' group by MBRegUnit.RegUnit) t order by t.name";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						Node node = new Node();
						node.Id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.Name = reader.GetString(1);
						}
						node.Nodes = new List<Node>();
						rootNode.Nodes.Add(node);
						listGroups.Add(node);
					}
				}
				reader.Close();
			}
			return listGroups;
		}

		public Node Load()
		{
			Node listNode = null;
			if(this.CheckTableExist("MBReports"))
			{
				listNode = new Node();
				listNode.Name = "Сценарий (расчёт)";
				listNode.Text = null;
				listNode.Nodes = new List<Node>();
				
				List<Node> listGroups = LoadGroups(listNode);
				foreach(Node groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = Connection;
					command.CommandText = "select XRecID, NameRpt, Comment, Report from MBReports where TypeRpt='Function' and RegUnit=@groupID order by NameRpt";
					SqlParameter paramGroupId = new SqlParameter("@groupID", SqlDbType.Int);
					paramGroupId.Value = groupNode.Id;
					command.Parameters.Add(paramGroupId);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							Node scriptNode = new Node();
							scriptNode.Id = reader.GetInt32(0);
							if(! reader.IsDBNull(1))
							{
								scriptNode.Name = reader.GetString(1);
							}
							if(! reader.IsDBNull(2))
							{
								scriptNode.Text = reader.GetString(2);
							}
							scriptNode.Nodes = new List<Node>();
							
							if(! reader.IsDBNull(3))
							{
								SqlBytes sqlbytes = reader.GetSqlBytes(3);
								System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
								string scriptText = win1251.GetString(sqlbytes.Value);
								Node scriptTextNode = new Node();
								scriptTextNode.Name = "-=[ Текст сценария ]=-";
								scriptTextNode.Text = scriptText;
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
