/*
 * Date: 10.08.2012
 * Time: 21:23
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Справочник (аналитика).
	/// </summary>
	public class Reference : LoaderCommon
	{
		public Reference(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

        /// <summary>
        /// Загрузка реквизитов справочника
        /// </summary>
        /// <param name="refNode">
        /// Ссылка на узел справочника
        /// A <see cref="Node"/>
        /// </param>
        void LoadRecvisite(IsbNode recvGroupNode)
		{
			if(this.CheckTableExist("MBVidAnRecv"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.Connection;
				command.CommandText = "SELECT [Name], [Kod], [Exprn], [InpExprn] FROM [MBVidAnRecv] WHERE [Vid]=@Vid AND [Razd]=@Razd AND ([Exprn] IS NOT NULL OR [InpExprn] IS NOT NULL) ORDER BY [NumRecv]";
				SqlParameter paramVid = new SqlParameter("@Vid", SqlDbType.Int);
				paramVid.Value = recvGroupNode.Id;
				SqlParameter paramRazd = new SqlParameter("@Razd", SqlDbType.NChar, 1);
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
			if(this.CheckTableExist("MBVidAnRecv"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.Connection;
				command.CommandText = "SELECT DISTINCT [Razd] FROM [MBVidAnRecv] WHERE [Vid] = @Vid AND ([Exprn] IS NOT NULL OR [InpExprn] IS NOT NULL) ORDER BY [Razd] DESC";
				SqlParameter paramVid = new SqlParameter("@Vid", SqlDbType.Int);
				paramVid.Value = refNode.Id;
				command.Parameters.Add(paramVid);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
				{
					while(reader.Read())
					{
					    var sectionCode = reader.GetString(0)[0];
                        var recvGroupNode = new IsbNode(ReferenceEventsParser.SectionCodeToName.ContainsKey(sectionCode) ? ReferenceEventsParser.SectionCodeToName[sectionCode] : "Неизвестно [" + sectionCode + "]");
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
		    var rootRefNode = new IsbNode("Тип справочника");
            if (this.CheckTableExist("MBVidAn"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select Vid, Name, Kod, Exprn, LastUpd, Comment from MBVidAn order by Name ASC";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						var refNode = new IsbNode();
                        refNode.Type = IsbNodeType.ReferenceType;
                        //ИД 
                        refNode.Id = reader.GetInt32(0);
						//Имя (Код)
						if((! reader.IsDBNull(1))&&(! reader.IsDBNull(2)))
						{
							refNode.Name = reader.GetString(1).Trim() + " (" + reader.GetString(2).Trim() + ")";
						}
                        if (!reader.IsDBNull(2))
                        {
                            refNode.Code = reader.GetString(2).Trim();
                        }
                        if (! reader.IsDBNull(3))
						{
                            ReferenceEventsParser.ParseEvents(reader.GetString(3).Trim(), refNode);
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
