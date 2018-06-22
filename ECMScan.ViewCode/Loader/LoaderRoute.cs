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
using System.Linq;


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

    public void Load(List<IsbNode> isblList)
    {
      IsbNode listNode = null;
      listNode = new IsbNode("Типовой маршрут");

      var listGroups = LoadGroups(listNode);
      SqlCommand command = new SqlCommand();
      command.Connection = Connection;
      command.CommandText = @"
select MBAnalit.Analit
    , ltrim(MBAnalit.Kod)
    , MBAnalit.NameAn + ' (' + ltrim(MBAnalit.Kod) + ')'
    , MBText.SearchCondition 
    , (select max(prot.DateAct)
        from XProtokol prot 
        where prot.SrcObjID = 119 and prot.SrcRecID = MBAnalit.Analit) as LastUpd
    , MBAnalit.HighLvl
from MBAnalit
    join MBText on MBAnalit.Analit = MBText.SrcRecID and MBText.SrcObjID = 119
where MBAnalit.Vid = (select Vid from MBVidAn where Kod = 'STANDARD_ROUTES' or Kod = 'ТМТ') 
  and Sost = 'A'
order by MBAnalit.NameAn";
      command.Prepare();
      SqlDataReader reader = command.ExecuteReader();
      if (reader.HasRows)
      {
        while (reader.Read())
        {
          var routeNode = new IsbNode(reader.GetString(2));
          routeNode.Type = IsbNodeType.StandardRoute;
          routeNode.Id = reader.GetInt32(0);
          routeNode.Code = reader.GetString(1);
          // Схема
          if (!reader.IsDBNull(3))
          {
            var schemaBytes = (byte[])reader.GetValue(3);
            string schemaString = System.Text.Encoding.GetEncoding(1251).GetString(schemaBytes);
            RouteParser.ParseRoute(schemaString, routeNode);
          }
          if (!reader.IsDBNull(4))
          {
            routeNode.LastUpdate = reader.GetDateTime(4);
          }
          var groupId = reader.GetInt32(5);
          listGroups.First(n => n.Id == groupId).Nodes.Add(routeNode);
        }
      }
      reader.Close();
      isblList.Add(listNode);
    }
  }
}
