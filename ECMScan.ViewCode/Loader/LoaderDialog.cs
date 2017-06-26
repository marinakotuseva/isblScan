using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Прикладные диалоги.
	/// </summary>
	public class Dialog : LoaderCommon
	{
		public Dialog(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

        /// <summary>
        /// Загрузка реквизитов диалога
        /// </summary>
        /// <param name="recvGroupNode">
        /// Ссылка на узел диалога
        /// A <see cref="IsbNode"/>
        /// </param>
        void LoadRecvisite(IsbNode recvGroupNode)
		{
			if(this.CheckTableExist("SBDialogRequisiteLink"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.Connection;
				command.CommandText = "SELECT [Name], [Code], [ChangeEventHandlerText], [LookupEventHandlerText] FROM [SBDialogRequisiteLink] WHERE [DialogID]=@DialogID AND [Section]=@Section AND ([ChangeEventHandlerText] IS NOT NULL OR [LookupEventHandlerText] IS NOT NULL) ORDER BY [RequisiteNumber]";
				SqlParameter paramVid = new SqlParameter("@DialogID", SqlDbType.Int);
				paramVid.Value = recvGroupNode.Id;
				SqlParameter paramRazd = new SqlParameter("@Section", SqlDbType.NChar, 1);
				paramRazd.Value = recvGroupNode.Code;
				command.Parameters.Add(paramVid);
				command.Parameters.Add(paramRazd);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						var recvNode = new IsbNode();
						if(!reader.IsDBNull(0))
						{
							recvNode.Name = reader.GetString(0);
						}
						if(!reader.IsDBNull(1))
						{
							recvNode.Code = reader.GetString(1);
							recvNode.Name += " (" + recvNode.Code + ")";
						}
						if(!reader.IsDBNull(2))
						{
							var exprnRefRecvNode = new IsbNode();
							exprnRefRecvNode.Name = "-=[ Вычисление ]=-";
							exprnRefRecvNode.Text = reader.GetString(2);
							recvNode.Nodes.Add(exprnRefRecvNode);
						}
						if(!reader.IsDBNull(3))
						{
							var inpExprnRefRecvNode = new IsbNode();
							inpExprnRefRecvNode.Name = "-=[ Выбор из справочника ]=-";
							inpExprnRefRecvNode.Text = reader.GetString(3);
							recvNode.Nodes.Add(inpExprnRefRecvNode);
						}
						recvGroupNode.Nodes.Add(recvNode);
					}
				}
				reader.Close();
			}
		}

		/// <summary>
		/// Загрузка разделов реквизитов
		/// </summary>
		/// <param name="refNode">
		/// A <see cref="Node"/>
		/// </param>
		void LoadGroupRecv(IsbNode refNode)
		{
			if(this.CheckTableExist("SBDialogRequisiteLink"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.Connection;
				command.CommandText = "SELECT DISTINCT [Section] FROM [SBDialogRequisiteLink] WHERE [DialogID] = @DialogID AND ([LookupEventHandlerText] IS NOT NULL OR [ChangeEventHandlerText] IS NOT NULL) ORDER BY [Section] DESC";
				SqlParameter paramVid = new SqlParameter("@DialogID", SqlDbType.Int);
				paramVid.Value = refNode.Id;
				command.Parameters.Add(paramVid);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
				{
					while(reader.Read())
					{
					    var sectionCode = reader.GetString(0)[0];
                        var recvGroupNode = new IsbNode(_sectionCodeToName.ContainsKey(sectionCode) ? _sectionCodeToName[sectionCode] : "Неизвестно [" + sectionCode + "]");
					    recvGroupNode.Code = sectionCode.ToString();
					    recvGroupNode.Id = refNode.Id;

                        refNode.Nodes.Add(recvGroupNode);
                    }
                }
				reader.Close();
			    foreach (var node in refNode.Nodes.Where(n => n.Id != 0))
			    {
			        LoadRecvisite(node);
                }
			}
		}

		public IsbNode Load()
		{
		    var rootRefNode = new IsbNode("Диалог");
            if (this.CheckTableExist("SBDialog"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select XRecID, Name, Code, EventHandlersText, LastUpd, Comment from SBDialog order by Name ASC";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						var refNode = new IsbNode();
                        refNode.Type = IsbNodeType.Dialog;
                        //ИД 
                        refNode.Id = reader.GetInt32(0);
						//Имя (Код)
						if((! reader.IsDBNull(1))&&(! reader.IsDBNull(2)))
						{
							refNode.Name = reader.GetString(1).Trim() + " (" + reader.GetString(2).Trim() + ")";
						}
						if(! reader.IsDBNull(3))
						{
                            ParseEvents(reader.GetString(3).Trim(), refNode);
                        }
					    if (!reader.IsDBNull(4))
					    {
					        refNode.LastUpdate = reader.GetDateTime(4);
					    }
                        if (!reader.IsDBNull(5))
                        {
                            refNode.Text = reader.GetString(5).Trim();
                        }


                        rootRefNode.Nodes.Add(refNode);
					}
				}
				reader.Close();
			    foreach (var node in rootRefNode.Nodes)
			    {
			        LoadGroupRecv(node);
                }
			}
			return rootRefNode;
		}
	}
}
