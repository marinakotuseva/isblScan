/*
 * Date: 10.08.2012
 * Time: 21:23
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

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

    private List<IsbNode> LoadGroups(IsbNode rootNode)
    {
      var listGroups = new List<IsbNode>();
      SqlCommand command = new SqlCommand();
      command.Connection = Connection;
      command.CommandText = "select t.id, t.name from (select MBRegUnit.RegUnit [id], Max(MBRegUnit.Name) [name] from MBRegUnit join MBReports on (MBRegUnit.RegUnit = MBReports.RegUnit) where MBReports.TypeRpt='Function' group by MBRegUnit.RegUnit) t order by t.name";
      SqlDataReader reader = command.ExecuteReader();
      if (reader.HasRows)
      {
        while (reader.Read())
        {
          var node = new IsbNode();
          node.Id = reader.GetInt32(0);
          if (!reader.IsDBNull(1))
          {
            node.Name = reader.GetString(1);
          }
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
      listNode = new IsbNode();
      listNode.Name = "Сценарий (расчёт)";

      var listGroups = LoadGroups(listNode);
      SqlCommand command = new SqlCommand();
      command.Connection = Connection;
      command.CommandText = "select XRecID, NameRpt, Description, Report, LastUpd, RegUnit from MBReports where TypeRpt='Function' order by NameRpt";
      command.Prepare();
      SqlDataReader reader = command.ExecuteReader();
      if (reader.HasRows)
      {
        while (reader.Read())
        {
          var scriptNode = new IsbNode();
          scriptNode.Type = IsbNodeType.Script;

          scriptNode.Id = reader.GetInt32(0);
          if (!reader.IsDBNull(1))
          {
            scriptNode.Name = reader.GetString(1);
          }
          if (!reader.IsDBNull(2))
          {
            scriptNode.Text = reader.GetString(2);
          }

          if (!reader.IsDBNull(3))
          {
            SqlBytes sqlbytes = reader.GetSqlBytes(3);
            System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
            string scriptText = win1251.GetString(sqlbytes.Value);
            var scriptTextNode = new IsbNode();
            scriptTextNode.Name = "-=[ Текст сценария ]=-";
            scriptTextNode.Text = scriptText;
            scriptNode.Nodes.Add(scriptTextNode);
          }
          if (!reader.IsDBNull(4))
          {
            scriptNode.LastUpdate = reader.GetDateTime(4);
          }
          var groupId = reader.GetInt32(5);
          listGroups.First(n => n.Id == groupId).Nodes.Add(scriptNode);
        }
      }
      reader.Close();
      isblList.Add(listNode);
    }
  }
}
