/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Xml;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Типовой маршрут.
	/// </summary>
	public class Route : LoaderCommon
	{
		public Route(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

        private List<Node> LoadGroups(Node rootNode)
        {
            List<Node> listGroups = new List<Node>();
            SqlCommand command = new SqlCommand();
            command.Connection = this.Connection;
            command.CommandText = @"
select Analit, NameAn
from MbAnalitSpr 
where Vid = (select Vid from MBVidAn where Kod = 'STANDARD_ROUTE_GROUPS' or Kod = 'ГТМ')
order by NameAn";
            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Node node = new Node();
                    // ИД группы маршрутов
                    node.Id = reader.GetInt32(0);
                    // Имя группы маршрутов
                    node.Name = reader.GetString(1);
                    node.Nodes = new List<Node>();
                    rootNode.Nodes.Add(node);
                    listGroups.Add(node);
                }
            }
            reader.Close();
            return listGroups;
        }

        public Node Load()
		{
            Node listNode = null;
            listNode = new Node();
            listNode.Name = "Типовой маршрут";
            listNode.Text = null;
            listNode.Nodes = new List<Node>();

            List<Node> listGroups = LoadGroups(listNode);
            foreach (Node groupNode in listGroups)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandText = @"
select MBAnalit.Analit
    , MBAnalit.NameAn + ' (' + ltrim(MBAnalit.Kod) + ')'
    , MBText.SearchCondition  
from MBAnalit
    join MBText on MBAnalit.Analit = MBText.SrcRecID and MBText.SrcObjID = 119
where MBAnalit.HighLvl=@groupID
    and MBAnalit.Vid = (select Vid from MBVidAn where Kod = 'STANDARD_ROUTES' or Kod = 'ТМТ') 
order by MBAnalit.NameAn";
                SqlParameter paramGroupId = new SqlParameter("@groupID", SqlDbType.Int);
                paramGroupId.Value = groupNode.Id;
                command.Parameters.Add(paramGroupId);
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Node routeNode = new Node();
                        routeNode.Nodes = new List<Node>();

                        // ИД
                        routeNode.Id = reader.GetInt32(0);
                        // Имя
                        routeNode.Name = reader.GetString(1);
                        // Схема
                        if (!reader.IsDBNull(2))
                        {
                            var schemaBytes = (byte[])reader.GetValue(2);
                            string schemaString = System.Text.Encoding.GetEncoding(1251).GetString(schemaBytes);
                            var schema = new XmlDocument();
                            schema.LoadXml(schemaString);
                            var events = schema.SelectNodes("/Settings/Event/node()");
                            var eventNameToTitle = new Dictionary<String, String>()
                            {
                                {"InitScript", "Начало выбора"},
                                {"Script", "Завершение выбора"},
                                {"TaskStart", "Возможность старта"},
                                {"TaskAbortPossibility", "Возможность прекращения"},
                                {"TaskAbort", "Прекращение"}
                            };
                            foreach (XmlNode eventXmlNode in events)
                            {
                                var eventString = GetNodeString(eventXmlNode);
                                if (!System.String.IsNullOrEmpty(eventString))
                                {
                                    Node eventNode = new Node();
                                    eventNode.Name = eventNameToTitle.ContainsKey(eventXmlNode.Name) ? eventNameToTitle[eventXmlNode.Name] : eventXmlNode.Name;
                                    eventNode.Text = eventString;
                                    routeNode.Nodes.Add(eventNode);
                                }
                            }
                            var routeProperties = schema.SelectNodes("/Settings/Properties/Property[@Type = '2' and @Name != 'Name']");
                            foreach (XmlNode property in routeProperties)
                            {
                                var propertyStringNode = property.SelectSingleNode("Value/Value");
                                if (propertyStringNode != null)
                                {
                                    var propertyString = GetNodeString(propertyStringNode);
                                    if (!System.String.IsNullOrEmpty(propertyString))
                                    {
                                        Node routeStringNode = new Node();
                                        routeStringNode.Name = property.Attributes["Description"].Value;
                                        routeStringNode.Text = propertyString;
                                        routeNode.Nodes.Add(routeStringNode);
                                    }
                                }
                            }

                            var blocks = schema.SelectNodes("//Block");
                            foreach (XmlNode block in blocks)
                            {
                                Node blockNode = new Node();
                                var nameProperty = block.SelectSingleNode("Properties/Property[@Type = '2' and @Name = 'Name']/Value/Value");
                                if (nameProperty != null) blockNode.Name = block.Attributes["ID"].Value + ". " + GetNodeString(nameProperty);
                                else blockNode.Name = block.Attributes["ID"].Value;
                                
                                blockNode.Nodes = new List<Node>();

                                var properties = block.SelectNodes("Properties/Property[@Type = '2' and @Name != 'Name']");
                                foreach (XmlNode property in properties)
                                {
                                    var propertyStringNode = property.SelectSingleNode("Value/Value");
                                    if (propertyStringNode != null)
                                    {
                                        var propertyString = GetNodeString(propertyStringNode);
                                        if (!System.String.IsNullOrEmpty(propertyString))
                                        {
                                            Node blockStringNode = new Node();
                                            blockStringNode.Name = property.Attributes["Description"].Value;
                                            blockStringNode.Text = propertyString;
                                            blockNode.Nodes.Add(blockStringNode);
                                        }
                                    }
                                }

                                if (blockNode.Nodes.Count > 0)
                                {
                                    routeNode.Nodes.Add(blockNode);
                                }
                                else blockNode = null;

                            }
                        }
                        groupNode.Nodes.Add(routeNode);
                    }
                }
                reader.Close();
            }
            return listNode;
        }

        private string GetNodeString(XmlNode node)
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
