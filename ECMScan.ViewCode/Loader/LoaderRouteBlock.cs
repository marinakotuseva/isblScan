/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Xml;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Блок типового маршрута.
	/// </summary>
	public class RouteBlock : LoaderCommon
	{
		public RouteBlock(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		private List<Node> LoadGroups(Node rootNode)
		{
			List<Node> listGroups = new List<Node>();
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
						Node node = new Node();
						node.Parent = rootNode;
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
			if(this.checkTableExist("SBRouteBlock"))
			{
				listNode = new Node();
				listNode.Name = "Блок типового маршрута";
				listNode.Text = null;
				listNode.Parent = null;
				listNode.Nodes = new List<Node>();
				
				List<Node> listGroups = LoadGroups(listNode);
				foreach(Node groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select XRecID, Name, Comment, Properties from SBRouteBlock where BlockGroup=@groupID order by Name";
					SqlParameter paramGroupID = new SqlParameter("@groupID", SqlDbType.Int);
					paramGroupID.Value = groupNode.Id;
					command.Parameters.Add(paramGroupID);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							Node routeBlockNode = new Node();
							routeBlockNode.Parent = groupNode;
							//ИД
							routeBlockNode.Id = reader.GetInt32(0);
							//Имя
							if(! reader.IsDBNull(1))
							{
								routeBlockNode.Name = reader.GetString(1);
							}
							//Комментарий
							if(! reader.IsDBNull(2))
							{
								routeBlockNode.Text = reader.GetString(2);
							}
							routeBlockNode.Nodes = new List<Node>();
							//Свойства
							if(! reader.IsDBNull(3))
							{
                                var schemaBytes = (byte[])reader.GetValue(3);
                                string schemaString = System.Text.Encoding.GetEncoding(1251).GetString(schemaBytes);
                                var schema = new XmlDocument();
                                schema.LoadXml(schemaString);

                                var events = schema.SelectNodes("/Settings/Event/node()");
                                foreach (XmlNode eventXMLNode in events)
                                {
                                    var eventString = getNodeString(eventXMLNode);
                                    if (!System.String.IsNullOrEmpty(eventString))
                                    {
                                        Node eventNode = new Node();
                                        eventNode.Name = eventXMLNode.Name;
                                        eventNode.Text = eventString;
                                        eventNode.Parent = routeBlockNode;
                                        routeBlockNode.Nodes.Add(eventNode);
                                    }
                                }

                                var properties = schema.SelectNodes("//Properties/Property[@Type = '2' and @Name != 'Name']");
                                foreach (XmlNode property in properties)
                                {
                                    var propertyStringNode = property.SelectSingleNode("Value/Value");
                                    if (propertyStringNode != null)
                                    {
                                        var propertyString = getNodeString(propertyStringNode);
                                        if (!System.String.IsNullOrEmpty(propertyString))
                                        {
                                            Node blockStringNode = new Node();
                                            blockStringNode.Name = property.Attributes["Description"].Value;
                                            blockStringNode.Text = propertyString;
                                            blockStringNode.Parent = routeBlockNode;
                                            routeBlockNode.Nodes.Add(blockStringNode);
                                        }
                                    }
                                }
                            }
							groupNode.Nodes.Add(routeBlockNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}

        private string getNodeString(XmlNode node)
        {
            string parsedString = "";
            parsedString = node.InnerText;
            parsedString = parsedString.Replace("{5314B05F-CF9F-4F66-99EC-24992A5FB114}", "");
            try
            {
                byte[] data = Convert.FromBase64String(parsedString);
                parsedString = System.Text.Encoding.GetEncoding(1251).GetString(data);
            }
            catch { }
            return parsedString;
        }
    }
}
