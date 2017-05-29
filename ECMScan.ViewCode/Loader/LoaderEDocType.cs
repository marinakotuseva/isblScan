/*
 * Date: 06.10.2012
 * Time: 10:54
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

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
		
		private List<Node> LoadGroupRecvisite(Node eDocTypeNode)
		{
			List<Node> listGroups = new List<Node>();
			
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
						Node groupNode = new Node();
						groupNode.Nodes = new List<Node>();
						string recvRazdel = reader.GetString(0);
						groupNode.Text = recvRazdel;
						switch (recvRazdel)
						{
							case "Ш":
								groupNode.Name = "Карточка";
								break;
							case "Т":
								groupNode.Name = "Таблица";
								break;
							case "С":
								groupNode.Name = "Таблица 2";
								break;
							case "Р":
								groupNode.Name = "Таблица 3";
								break;
							case "О":
								groupNode.Name = "Таблица 4";
								break;
							case "Н":
								groupNode.Name = "Таблица 5";
								break;
							case "М":
								groupNode.Name = "Таблица 6";
								break;
							case "К":
								groupNode.Name = "Действие";
								break;
							default:
								groupNode.Name = "Неизвестно ["+recvRazdel+"]";
								break;
						}
						eDocTypeNode.Nodes.Add(groupNode);
						listGroups.Add(groupNode);
					}
				}
			}
			reader.Close();
			return listGroups;
		}
		private void LoadRecvisite(Node eDocTypeNode)
		{
			if(this.CheckTableExist("MBEDocTypeRecv"))
			{
				List<Node> listGroups = LoadGroupRecvisite(eDocTypeNode);
				foreach(Node groupNode in listGroups)
				{
					char charGroup = groupNode.Text.ToCharArray()[0];
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
							Node eDocRecvNode = new Node();
							eDocRecvNode.Nodes = new List<Node>();
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
								Node exprnEDocRecvNode = new Node();
								exprnEDocRecvNode.Name = "-=[ Вычисление ]=-";
								exprnEDocRecvNode.Text = reader.GetString(3);
								eDocRecvNode.Nodes.Add(exprnEDocRecvNode);
							}
							//Выбор из справочника для реквизита типа "справочник" или "строка"
							if(!reader.IsDBNull(4))
							{
								Node eventEDocRecvNode = new Node();
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
		
		//Выделение в тексте событий вида электронного документа, названий событий
		private string ParseEventText(string eventText)
		{
			string parseResult;
			parseResult = eventText;
			string[] eDocTypeEvents = {
				"КАРТОЧКА.ОТКРЫТИЕ",
				"КАРТОЧКА.ДОБАВЛЕНИЕ ПОСЛЕ",
				"КАРТОЧКА.СОХРАНЕНИЕ ВОЗМОЖНОСТЬ",
				"КАРТОЧКА.СОХРАНЕНИЕ ДО",
				"КАРТОЧКА.СОХРАНЕНИЕ ПОСЛЕ",
				
				"DATASET.OPEN",
				"CARD.OPEN",
				"CARD.AFTER_INSERT",
				"CARD.VALID_UPDATE",
				"CARD.BEFORE_UPDATE",
				"CARD.AFTER_UPDATE",
				"FORM.SHOW"				
			};
			int posEnd = 0;
			int posStart = 0;
			while(posStart < eventText.Length)
			{
				/********************************************************************
				 * Подсветка названий событий типов карточек электронных документов
				 ********************************************************************/
				foreach(string keyword in eDocTypeEvents)
				{
					if(posStart+keyword.Length <= parseResult.Length)
					{
						string testStr = parseResult.Substring(posStart, keyword.Length);
						if(testStr == keyword)
						{
							bool isKeyword = false;
							int additionalPos = 0;
							
							if(posStart+keyword.Length == parseResult.Length)
							{
								isKeyword = true;
							}
							else
							{
								string postChar = parseResult.Substring(posStart+keyword.Length, 1);
								if((postChar == "\n")||(postChar == "\r"))
								{
									isKeyword = true;
									additionalPos = 1;
								}
							}
							if(isKeyword)
							{
								posEnd = posStart+keyword.Length+additionalPos;
								parseResult = parseResult.Substring(0, posStart) + (posStart==0?"":"\n") +
									"-=[ " + keyword + " ]=-\n" + 
									parseResult.Substring(posEnd, parseResult.Length - posEnd);
								posStart = posEnd;
							}
						}
					}
				}
				posStart = posStart + 1;
			}
			return parseResult;
		}
		
		public Node Load()
		{
			Node listNode = null;
			if(this.CheckTableExist("MBEDocType"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select TypeID, Name, Exprn, LastUpd from MBEDocType order by Name ASC";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					listNode = new Node();
					listNode.Name = "Тип карточки электронного документа";
					listNode.Text = null;
					listNode.Nodes = new List<Node>();
					
					while(reader.Read())
					{
						Node eDocNode = new Node();
						eDocNode.Nodes = new List<Node>();
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
							eDocNode.Text = ParseEventText(reader.GetString(2));
						}
						//Дата последнего изменения
						if(!reader.IsDBNull(3))
						{
							eDocNode.LastUpdate = reader.GetDateTime(3);
						}

						listNode.Nodes.Add(eDocNode);
					}
				}
				reader.Close();
				foreach(Node eDocNode in listNode.Nodes)
				{
					LoadRecvisite(eDocNode);
				}
			}
			return listNode;
		}

	}
}
