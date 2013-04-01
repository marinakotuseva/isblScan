/*
 * Date: 30.09.2012
 * Time: 16:32
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
namespace isblTest
{
	/// <summary>
	/// Пользовательский поиск.
	/// </summary>

	public class UserSearch : LoaderCommon
	{
		public UserSearch(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private void LoadText(isblTest.Node userSeachNode)
		{
			if(checkTableExist("MBText"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select SearchCondition, BeforeSearch, Tekst from MBText where SrcRecID=@userSearchAnalit";
				SqlParameter userSearchAnalit = new SqlParameter("@userSearchAnalit", SqlDbType.Int, 10);
				userSearchAnalit.Value = userSeachNode.id;
				command.Parameters.Add(userSearchAnalit);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					if(reader.Read())
					{
						if(!reader.IsDBNull(0))
						{
							SqlBytes sqlbytes = reader.GetSqlBytes(0);
							
							isblTest.Node userSearchSearchConditionNode = new isblTest.Node();
							userSearchSearchConditionNode.name = "-=[ Критерии поиска ]=-";
							userSearchSearchConditionNode.parent = userSeachNode;
							userSeachNode.nodes.Add(userSearchSearchConditionNode);
							
							System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
							userSearchSearchConditionNode.text = win1251.GetString(sqlbytes.Value);
						}
						if(!reader.IsDBNull(1))
						{
							SqlBytes sqlbytes = reader.GetSqlBytes(1);
							
							isblTest.Node userSearchBeforeSearchNode = new isblTest.Node();
							userSearchBeforeSearchNode.name = "-=[ Обработчик события \"До поиска\" ]=-";
							userSearchBeforeSearchNode.parent = userSeachNode;
							userSeachNode.nodes.Add(userSearchBeforeSearchNode);

							System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
							userSearchBeforeSearchNode.text = win1251.GetString(sqlbytes.Value);
						}
						if(!reader.IsDBNull(2))
						{
							SqlBytes sqlbytes = reader.GetSqlBytes(2);
							
							isblTest.Node userSearchTextNode = new isblTest.Node();
							userSearchTextNode.name = "-=[ Текст ]=-";
							userSearchTextNode.parent = userSeachNode;
							userSeachNode.nodes.Add(userSearchTextNode);

							System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
							userSearchTextNode.text = win1251.GetString(sqlbytes.Value);
						}
					}
				}
				reader.Close();
			}
		}
		public isblTest.Node Load()
		{
			isblTest.Node listNode = null;
			Int32 vidAnalitID = getVidAnalitID("ПСК");
			if(vidAnalitID >= 0)
			{
				listNode = new isblTest.Node();
				listNode.name = "Пользовательский поиск";
				listNode.text = null;
				listNode.id = vidAnalitID;
				listNode.parent = null;
				listNode.nodes = new List<Node>();
				
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select Analit, NameAn, Prim from MBAnalit where Vid=@vidAnalit order by NameAn";
					SqlParameter paramVidAnalit = new SqlParameter("@vidAnalit", SqlDbType.Int, 10);
					paramVidAnalit.Value = vidAnalitID;
					command.Parameters.Add(paramVidAnalit);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							isblTest.Node userSearchNode = new isblTest.Node();
							userSearchNode.parent = listNode;
							userSearchNode.nodes = new List<isblTest.Node>();
							userSearchNode.id = reader.GetInt32(0);
							if(! reader.IsDBNull(1))
							{
								userSearchNode.name = reader.GetString(1);
							}
							if(! reader.IsDBNull(2))
							{
								userSearchNode.text = reader.GetString(2);
							}
							listNode.nodes.Add(userSearchNode);
						}
					}
					reader.Close();
					foreach(isblTest.Node userScriptNode in listNode.nodes)
					{
						LoadText(userScriptNode);
					}
				}				
			}
			return listNode;
		}

	}
}


