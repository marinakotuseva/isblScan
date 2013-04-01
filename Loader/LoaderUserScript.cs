/*
 * Date: 30.09.2012
 * Time: 16:31
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
namespace isblTest
{
	/// <summary>
	/// Пользовательский расчёт.
	/// </summary>
	public class UserScript : LoaderCommon
	{
		public UserScript(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private void LoadText(isblTest.Node userScriptNode)
		{
			if(checkTableExist("MBText"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select Tekst from MBText where SrcRecID=@userScriptAnalit";
				SqlParameter userScriptAnalit = new SqlParameter("@userScriptAnalit", SqlDbType.Int, 10);
				userScriptAnalit.Value = userScriptNode.id;
				command.Parameters.Add(userScriptAnalit);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					if(reader.Read())
					{
						if(!reader.IsDBNull(0))
						{
							SqlBytes sqlbytes = reader.GetSqlBytes(0);
							
							isblTest.Node userScriptText = new isblTest.Node();
							userScriptText.name = "-=[ Расчёт ]=-";
							userScriptText.parent = userScriptNode;
							userScriptNode.nodes.Add(userScriptText);
							System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
							userScriptText.text = win1251.GetString(sqlbytes.Value);
						}
					}
				}
				reader.Close();
			}
		}
		public isblTest.Node Load()
		{
			isblTest.Node listNode = null;
			Int32 vidAnalitID = getVidAnalitID("РСЧ");
			if(vidAnalitID >= 0)
			{
				listNode = new isblTest.Node();
				listNode.name = "Пользовательский расчёт";
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
							isblTest.Node userScriptNode = new isblTest.Node();
							userScriptNode.parent = listNode;
							userScriptNode.nodes = new List<isblTest.Node>();
							userScriptNode.id = reader.GetInt32(0);
							if(! reader.IsDBNull(1))
							{
								userScriptNode.name = reader.GetString(1);
							}
							if(! reader.IsDBNull(2))
							{
								userScriptNode.text = reader.GetString(2);
							}
							listNode.nodes.Add(userScriptNode);
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
