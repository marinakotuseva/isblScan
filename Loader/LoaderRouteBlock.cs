/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace isblTest
{
	/// <summary>
	/// Блок типового маршрута.
	/// </summary>
	public class RouteBlock : LoaderCommon
	{
		public RouteBlock(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		private List<isblTest.Node> LoadGroups(isblTest.Node rootNode)
		{
			List<isblTest.Node> listGroups = new List<Node>();
			if(this.checkTableExist("SBRouteBlockGroup"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select t.id, t.name from (select SBRouteBlockGroup.XrecID [id], Max(SBRouteBlockGroup.Name) [name] from SBRouteBlockGroup join SBRouteBlock on (SBRouteBlockGroup.XRecID = SBRouteBlock.BlockGroup)  group by SBRouteBlockGroup.XRecID) t order by t.name";
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
			if(this.checkTableExist("SBRouteBlock"))
			{
				listNode = new isblTest.Node();
				listNode.name = "Блок типового маршрута";
				listNode.text = null;
				listNode.parent = null;
				listNode.nodes = new List<Node>();
				
				List<isblTest.Node> listGroups = LoadGroups(listNode);
				foreach(isblTest.Node groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select XRecID, Name, Comment, Properties from SBRouteBlock where BlockGroup=@groupID order by Name";
					SqlParameter paramGroupID = new SqlParameter("@groupID", SqlDbType.Int, 10);
					paramGroupID.Value = groupNode.id;
					command.Parameters.Add(paramGroupID);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							isblTest.Node routeBlockNode = new isblTest.Node();
							routeBlockNode.parent = groupNode;
							//ИД
							routeBlockNode.id = reader.GetInt32(0);
							//Имя
							if(! reader.IsDBNull(1))
							{
								routeBlockNode.name = reader.GetString(1);
							}
							//Комментарий
							if(! reader.IsDBNull(2))
							{
								routeBlockNode.text = reader.GetString(2);
							}
							routeBlockNode.nodes = new List<isblTest.Node>();
							//Свойства
							if(! reader.IsDBNull(3))
							{
								SqlBytes sqlbytes = reader.GetSqlBytes(3);
								System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
								string routeBlockProperties = win1251.GetString(sqlbytes.Value);
								isblTest.Node routeBlockPropNode = new isblTest.Node();
								routeBlockPropNode.name = "Свойства";
								routeBlockPropNode.text = routeBlockProperties;
								routeBlockPropNode.parent = routeBlockNode;
								routeBlockNode.nodes.Add(routeBlockPropNode);
							}
							groupNode.nodes.Add(routeBlockNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}
	}
}
