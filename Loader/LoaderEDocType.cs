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
	/// Description of LoaderEDocType.
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
			command.Connection = connection;
			command.CommandText = "select Razd from MBEDocTypeRecv where TypeID=@eDocTypeID and (not(Exprn is null) or not(InpExprn is null)) group by Razd order by Razd desc";
			SqlParameter paramEDocTypeID = new SqlParameter("@eDocTypeID", SqlDbType.Int, 10);
			paramEDocTypeID.Value = eDocTypeNode.id;
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
						groupNode.parent = eDocTypeNode;
						groupNode.nodes = new List<isblTest.Node>();
						string recvRazdel = reader.GetString(0);
						groupNode.text = recvRazdel;
						switch (recvRazdel)
						{
							case "Ш":
								groupNode.name = "Карточка";
								break;
							case "Т":
								groupNode.name = "Таблица";
								break;
							case "С":
								groupNode.name = "Таблица 2";
								break;
							case "Р":
								groupNode.name = "Таблица 3";
								break;
							case "О":
								groupNode.name = "Таблица 4";
								break;
							case "Н":
								groupNode.name = "Таблица 5";
								break;
							case "М":
								groupNode.name = "Таблица 6";
								break;
							case "К":
								groupNode.name = "Действие";
								break;
							default:
								groupNode.name = "Неизвестно ["+recvRazdel+"]";
								break;
						}
						eDocTypeNode.nodes.Add(groupNode);
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
					char charGroup = groupNode.text.ToCharArray()[0];
					groupNode.text = null;

					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select XRecID, Name, Exprn, InpExprn, Kod from MBEDocTypeRecv where TypeID=@eDocTypeID and Razd=@RazdID and (not(Exprn is null) or not(InpExprn is null)) order by Name";
					SqlParameter paramEDocTypeID = new SqlParameter("@eDocTypeID", SqlDbType.Int, 10);
					SqlParameter paramRazdID = new SqlParameter("@RazdID", SqlDbType.Char, 1);
					paramEDocTypeID.Value = eDocTypeNode.id;
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
							eDocRecvNode.parent = groupNode;
							//ИД
							eDocRecvNode.id = reader.GetInt32(0);
							//Код реквизита
							string strRecvCode = "";
							if(!reader.IsDBNull(4))
							{
								strRecvCode = reader.GetString(4);
							}							
							//Имя
							eDocRecvNode.name = "";
							if(!reader.IsDBNull(1))
							{
								eDocRecvNode.name = strRecvCode + " (" +reader.GetString(1)+")";
							}
							//Вычисление для реквизита и действия
							if(!reader.IsDBNull(2))
							{
								eDocRecvNode.text = "-=[ ВЫЧИСЛЕНИЕ ]=-\n"+
									reader.GetString(2)+
									"\n\n";
							}
							//Выбор из справочника для реквизита типа "справочник" или "строка"
							if(!reader.IsDBNull(3))
							{
								eDocRecvNode.text = eDocRecvNode.text+
									"-=[ ВЫБОР ИЗ СПРАВОЧНИКА ДЛЯ РЕКВИЗИТА ]=-\n"+
									reader.GetString(3);
							}
							
							groupNode.nodes.Add(eDocRecvNode);
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
					listNode.name = "Тип карточки электронного документа";
					listNode.text = null;
					listNode.parent = null;
					listNode.nodes = new List<Node>();
					
					while(reader.Read())
					{
						isblTest.Node eDocNode = new isblTest.Node();
						eDocNode.parent = listNode;
						//ИД 
						eDocNode.id = reader.GetInt32(0);
						//Имя 
						if(! reader.IsDBNull(1))
						{
							eDocNode.name = reader.GetString(1);
						}
						eDocNode.nodes = new List<isblTest.Node>();
						//Текст событий
						if(! reader.IsDBNull(2))
						{
							eDocNode.text = parseEventText(reader.GetString(2));
						}
						listNode.nodes.Add(eDocNode);
					}
				}
				reader.Close();
				foreach(isblTest.Node eDocNode in listNode.nodes)
				{
					LoadRecvisite(eDocNode);
				}
			}
			return listNode;
		}

	}
}
