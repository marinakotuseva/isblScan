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
				userSearchAnalit.Value = userSeachNode.Id;
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
							userSearchSearchConditionNode.Name = "-=[ Критерии поиска ]=-";
							userSearchSearchConditionNode.Parent = userSeachNode;
							userSeachNode.Nodes.Add(userSearchSearchConditionNode);
							
							System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
							userSearchSearchConditionNode.Text = win1251.GetString(sqlbytes.Value);
						}
						if(!reader.IsDBNull(1))
						{
							SqlBytes sqlbytes = reader.GetSqlBytes(1);
							
							isblTest.Node userSearchBeforeSearchNode = new isblTest.Node();
							userSearchBeforeSearchNode.Name = "-=[ Обработчик события \"До поиска\" ]=-";
							userSearchBeforeSearchNode.Parent = userSeachNode;
							userSeachNode.Nodes.Add(userSearchBeforeSearchNode);

							System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
							userSearchBeforeSearchNode.Text = win1251.GetString(sqlbytes.Value);
						}
						if(!reader.IsDBNull(2))
						{
							SqlBytes sqlbytes = reader.GetSqlBytes(2);
							
							isblTest.Node userSearchTextNode = new isblTest.Node();
							userSearchTextNode.Name = "-=[ Текст ]=-";
							userSearchTextNode.Parent = userSeachNode;
							userSeachNode.Nodes.Add(userSearchTextNode);

							System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
							userSearchTextNode.Text = win1251.GetString(sqlbytes.Value);
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
				listNode.Name = "Пользовательский поиск";
				listNode.Text = null;
				listNode.Id = vidAnalitID;
				listNode.Parent = null;
				listNode.Nodes = new List<Node>();
				
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
							userSearchNode.Parent = listNode;
							userSearchNode.Nodes = new List<isblTest.Node>();
							userSearchNode.Id = reader.GetInt32(0);
							if(! reader.IsDBNull(1))
							{
								userSearchNode.Name = reader.GetString(1);
							}
							if(! reader.IsDBNull(2))
							{
								userSearchNode.Text = reader.GetString(2);
							}
							listNode.Nodes.Add(userSearchNode);
						}
					}
					reader.Close();
					foreach(isblTest.Node userScriptNode in listNode.Nodes)
					{
						LoadText(userScriptNode);
					}
				}				
			}
			return listNode;
		}

	}
}


