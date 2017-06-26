/*
 * Date: 06.10.2012
 * Time: 10:54
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
	/// Загрузчик прикладной разработки для типов карточек электронных документов (события и вычисления карточки и реквизитов документов)
	/// </summary>
	public class EDocType : LoaderCommon
	{
		public EDocType(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private List<IsbNode> LoadGroupRecvisite(IsbNode eDocTypeNode)
		{
			var listGroups = new List<IsbNode>();
			
			SqlCommand command = new SqlCommand();
			command.CommandType = CommandType.Text;
			command.Connection = Connection;
			command.CommandText = "select [Razd] from [MBEDocTypeRecv] where not(([Exprn] is null) and ([InpExprn] is null)) and ([TypeID] = @eDocTypeID ) group by [Razd] order by [Razd] desc";
			SqlParameter paramEDocTypeId = new SqlParameter("@eDocTypeID", SqlDbType.Int);
			paramEDocTypeId.Value = eDocTypeNode.Id;
			command.Parameters.Add(paramEDocTypeId);
			command.Prepare();
			SqlDataReader reader = command.ExecuteReader();
			if(reader.HasRows)
			{
				while(reader.Read())
				{
					if(! reader.IsDBNull(0))
					{
					    var sectionCode = reader.GetString(0)[0];
                        var groupNode = new IsbNode(_sectionCodeToName.ContainsKey(sectionCode) ? _sectionCodeToName[sectionCode] : "Неизвестно [" + sectionCode + "]");
					    groupNode.Code = sectionCode.ToString();
                        eDocTypeNode.Nodes.Add(groupNode);
						listGroups.Add(groupNode);
					}
				}
			}
			reader.Close();
			return listGroups;
		}
		private void LoadRecvisite(IsbNode eDocTypeNode)
		{
			if(CheckTableExist("MBEDocTypeRecv"))
			{
				List<IsbNode> listGroups = LoadGroupRecvisite(eDocTypeNode);
				foreach(var groupNode in listGroups)
				{
					char charGroup = groupNode.Code[0];
					groupNode.Text = null;

					SqlCommand command = new SqlCommand();
					command.Connection = Connection;
					command.CommandText = "select [XRecID], [Name], [Kod], [Exprn], [InpExprn] from MBEDocTypeRecv where [TypeID] = @eDocTypeID and [Razd] = @RazdID and (not([Exprn] is null) or not([InpExprn] is null)) order by [Name]";
					SqlParameter paramEDocTypeId = new SqlParameter("@eDocTypeID", SqlDbType.Int);
					SqlParameter paramRazdId = new SqlParameter("@RazdID", SqlDbType.NChar, 1);
					paramEDocTypeId.Value = eDocTypeNode.Id;
					paramRazdId.Value = charGroup;
					command.Parameters.Add(paramEDocTypeId);
					command.Parameters.Add(paramRazdId);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							var eDocRecvNode = new IsbNode();
							eDocRecvNode.Nodes = new List<IsbNode>();
							//ИД
							eDocRecvNode.Id = reader.GetInt32(0);
							//Имя
							if(!reader.IsDBNull(1))
							{
								eDocRecvNode.Name = reader.GetString(1);
							}
							//Код реквизита
							if(!reader.IsDBNull(2))
							{
								eDocRecvNode.Code = reader.GetString(2);
								eDocRecvNode.Name = string.Format("{0} ({1})", eDocRecvNode.Name, eDocRecvNode.Code);
							}							
							//Вычисление для реквизита и действия
							if(!reader.IsDBNull(3))
							{
								var exprnEDocRecvNode = new IsbNode();
								exprnEDocRecvNode.Name = "-=[ Вычисление ]=-";
								exprnEDocRecvNode.Text = reader.GetString(3);
								eDocRecvNode.Nodes.Add(exprnEDocRecvNode);
							}
							//Выбор из справочника для реквизита типа "справочник" или "строка"
							if(!reader.IsDBNull(4))
							{
								var eventEDocRecvNode = new IsbNode();
								eventEDocRecvNode.Name = "-=[ Выбор из справочника ]=-";
								eventEDocRecvNode.Text = reader.GetString(4);
								eDocRecvNode.Nodes.Add(eventEDocRecvNode);
							}
							//Добавление в дерево элементов, если есть вычисления
							if(eDocRecvNode.Nodes.Count > 0)
							{
								groupNode.Nodes.Add(eDocRecvNode);
							}
						}
					}
					reader.Close();
				}
			}
		}
		
		public IsbNode Load()
		{
		    IsbNode listNode = null;
			if(this.CheckTableExist("MBEDocType"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select TypeID, Name, Exprn, LastUpd, Comment from MBEDocType order by Name ASC";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					listNode = new IsbNode();
					listNode.Name = "Тип карточки электронного документа";
					listNode.Text = null;
					listNode.Nodes = new List<IsbNode>();
					
					while(reader.Read())
					{
					    var eDocNode = new IsbNode();
                        eDocNode.Type = IsbNodeType.EDocType;
                        //ИД 
                        eDocNode.Id = reader.GetInt32(0);
						//Имя 
						if(! reader.IsDBNull(1))
						{
							eDocNode.Name = reader.GetString(1);
						}
						//Текст событий
						if(! reader.IsDBNull(2))
						{
                            ParseEvents(reader.GetString(2).Trim(), eDocNode);
                        }
						//Дата последнего изменения
						if(!reader.IsDBNull(3))
						{
							eDocNode.LastUpdate = reader.GetDateTime(3);
						}
                        //Комментарий
                        if (!reader.IsDBNull(4))
                        {
                            eDocNode.Text = reader.GetString(4);
                        }

                        listNode.Nodes.Add(eDocNode);
					}
				}
				reader.Close();
				foreach(var eDocNode in listNode.Nodes.Where(n => n.Id != 0))
				{
					LoadRecvisite(eDocNode);
				}
			}
			return listNode;
		}

	}
}
