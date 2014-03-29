/*
 * Date: 06.10.2012
 * Time: 10:54
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace isblTest
{
	/// <summary>
	/// Загрузчик прикладной разработки для типов карточек электронных документов (события и вычисления карточки и реквизитов документов)
	/// </summary>
	public class EDocType : LoaderCommon
	{
		public EDocType(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private List<isblTest.Node> LoadGroupRecvisite(isblTest.Node eDocTypeNode)
		{
			List<isblTest.Node> listGroups = new List<isblTest.Node>();
			
			SqlCommand command = new SqlCommand();
			command.CommandType = CommandType.Text;
			command.Connection = connection;
			command.CommandText = "select [Razd] from [MBEDocTypeRecv] where not(([Exprn] is null) and ([InpExprn] is null)) and ([TypeID] = @eDocTypeID ) group by [Razd] order by [Razd] desc";
			SqlParameter paramEDocTypeID = new SqlParameter("@eDocTypeID", SqlDbType.Int);
			paramEDocTypeID.Value = eDocTypeNode.Id;
			command.Parameters.Add(paramEDocTypeID);
			command.Prepare();
			SqlDataReader reader = command.ExecuteReader();
			if(reader.HasRows)
			{
				while(reader.Read())
				{
					if(! reader.IsDBNull(0))
					{
						isblTest.Node groupNode = new isblTest.Node();
						groupNode.Parent = eDocTypeNode;
						groupNode.Nodes = new List<isblTest.Node>();
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
		private void LoadRecvisite(isblTest.Node eDocTypeNode)
		{
			if(this.checkTableExist("MBEDocTypeRecv"))
			{
				List<isblTest.Node> listGroups = LoadGroupRecvisite(eDocTypeNode);
				foreach(isblTest.Node groupNode in listGroups)
				{
					char charGroup = groupNode.Text.ToCharArray()[0];
					groupNode.Text = null;

					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select [XRecID], [Name], [Kod], [Exprn], [InpExprn] from MBEDocTypeRecv where [TypeID] = @eDocTypeID and [Razd] = @RazdID and (not([Exprn] is null) or not([InpExprn] is null)) order by [Name]";
					SqlParameter paramEDocTypeID = new SqlParameter("@eDocTypeID", SqlDbType.Int);
					SqlParameter paramRazdID = new SqlParameter("@RazdID", SqlDbType.NChar, 1);
					paramEDocTypeID.Value = eDocTypeNode.Id;
					paramRazdID.Value = charGroup;
					command.Parameters.Add(paramEDocTypeID);
					command.Parameters.Add(paramRazdID);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							isblTest.Node eDocRecvNode = new isblTest.Node();
							eDocRecvNode.Parent = groupNode;
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
								isblTest.Node exprnEDocRecvNode = new isblTest.Node();
								exprnEDocRecvNode.Name = "-=[ Вычисление ]=-";
								exprnEDocRecvNode.Text = reader.GetString(3);
								exprnEDocRecvNode.Parent = eDocRecvNode;
								eDocRecvNode.Nodes.Add(exprnEDocRecvNode);
							}
							//Выбор из справочника для реквизита типа "справочник" или "строка"
							if(!reader.IsDBNull(4))
							{
								isblTest.Node eventEDocRecvNode = new isblTest.Node();
								eventEDocRecvNode.Name = "-=[ Выбор из справочника ]=-";
								eventEDocRecvNode.Text = reader.GetString(4);
								eventEDocRecvNode.Parent = eDocRecvNode;
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
		private string parseEventText(string eventText)
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
		
		public isblTest.Node Load()
		{
			isblTest.Node listNode = null;
			if(this.checkTableExist("MBEDocType"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select TypeID, Name, Exprn from MBEDocType order by Name ASC";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					listNode = new isblTest.Node();
					listNode.Name = "Тип карточки электронного документа";
					listNode.Text = null;
					listNode.Parent = null;
					listNode.Nodes = new List<Node>();
					
					while(reader.Read())
					{
						isblTest.Node eDocNode = new isblTest.Node();
						eDocNode.Parent = listNode;
						//ИД 
						eDocNode.Id = reader.GetInt32(0);
						//Имя 
						if(! reader.IsDBNull(1))
						{
							eDocNode.Name = reader.GetString(1);
						}
						eDocNode.Nodes = new List<isblTest.Node>();
						//Текст событий
						if(! reader.IsDBNull(2))
						{
							eDocNode.Text = parseEventText(reader.GetString(2));
						}
						listNode.Nodes.Add(eDocNode);
					}
				}
				reader.Close();
				foreach(isblTest.Node eDocNode in listNode.Nodes)
				{
					LoadRecvisite(eDocNode);
				}
			}
			return listNode;
		}

	}
}
