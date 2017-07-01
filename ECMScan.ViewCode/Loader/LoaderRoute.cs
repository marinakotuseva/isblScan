/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        private List<IsbNode> LoadGroups(IsbNode rootNode)
        {
            var listGroups = new List<IsbNode>();
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
                    var node = new IsbNode(reader.GetString(1));
                    // ИД группы маршрутов
                    node.Id = reader.GetInt32(0);

                    rootNode.Nodes.Add(node);
                    listGroups.Add(node);
                }
            }
            reader.Close();
            return listGroups;
        }

        public IsbNode Load()
		{
		    IsbNode listNode = null;
            listNode = new IsbNode("Типовой маршрут");

           var listGroups = LoadGroups(listNode);
            foreach (var groupNode in listGroups)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = Connection;
                command.CommandText = @"
select MBAnalit.Analit
    , MBAnalit.NameAn + ' (' + ltrim(MBAnalit.Kod) + ')'
    , MBText.SearchCondition 
    , (select max(prot.DateAct)
        from XProtokol prot 
        where prot.SrcObjID = 119 and prot.SrcRecID = MBAnalit.Analit) as LastUpd
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
                        var routeNode = new IsbNode(reader.GetString(1));
                        routeNode.Type = IsbNodeType.StandardRoute;
                        // ИД
                        routeNode.Id = reader.GetInt32(0);
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
                                    var eventNode = new IsbNode();
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
                                        var routeStringNode = new IsbNode(property.Attributes["Description"].Value);
                                        routeStringNode.Text = propertyString;
                                        routeNode.Nodes.Add(routeStringNode);
                                    }
                                }
                            }

                            var blocks = schema.SelectNodes("//Block");
                            foreach (XmlNode block in blocks)
                            {
                                var blockNode = new IsbNode();
                                var nameProperty = block.SelectSingleNode("Properties/Property[@Type = '2' and @Name = 'Name']/Value/Value");
                                if (nameProperty != null) blockNode.Name = block.Attributes["ID"].Value + ". " + GetNodeString(nameProperty);
                                else blockNode.Name = block.Attributes["ID"].Value;

                                var properties = block.SelectNodes("Properties/Property[@Type = '2' and @Name != 'Name']");
                                foreach (XmlNode property in properties)
                                {
                                    var propertyStringNode = property.SelectSingleNode("Value/Value");
                                    if (propertyStringNode != null)
                                    {
                                        var propertyString = GetNodeString(propertyStringNode);
                                        if (!System.String.IsNullOrEmpty(propertyString))
                                        {
                                            var blockStringNode = new IsbNode(property.Attributes["Description"].Value);
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
                        if (!reader.IsDBNull(3))
                        {
                            routeNode.LastUpdate = reader.GetDateTime(3);
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
