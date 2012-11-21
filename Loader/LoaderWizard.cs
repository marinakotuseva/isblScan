/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace isblTest
{
	struct EventStruct
	{
		public string name;
		public string text;
	}
	/// <summary>
	/// Мастер действий.
	/// </summary>
	public class Wizard : LoaderCommon
	{
		private string originWizardText;
		private string[] linesWizardText;
		int linesWizardCount;
		int lineIndex;	
		public Wizard(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private List<isblTest.Node> LoadGroups(isblTest.Node rootNode)
		{
			List<isblTest.Node> listGroups = new List<Node>();
			Int32 vidAnalitID = getVidAnalitID("WIZARD_GROUPS");
			if(vidAnalitID >= 0)
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select Analit, NameAn, Soder from MBAnalit where Vid=@vidAnalit";
				SqlParameter paramVidAnalit = new SqlParameter("@vidAnalit", SqlDbType.Int);
				paramVidAnalit.Value = vidAnalitID;
				command.Parameters.Add(paramVidAnalit);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						isblTest.Node node = new isblTest.Node();
						node.parent = rootNode;
						node.id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.name = reader.GetString(1);
						}
						if(! reader.IsDBNull(2))
						{
							node.text = reader.GetString(2);
						}
						node.nodes = new List<isblTest.Node>();
						rootNode.nodes.Add(node);
						listGroups.Add(node);
					}
				}
				reader.Close();
			}
			return listGroups;
		}
		private void LoadText(isblTest.Node wizardNode)
		{
			string wizardText;
			if(checkTableExist("MBText"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select SearchCondition from MBText where SrcRecID=@wizardAnalit";
				SqlParameter wizardAnalit = new SqlParameter("@wizardAnalit", SqlDbType.Int, 10);
				wizardAnalit.Value = wizardNode.id;
				command.Parameters.Add(wizardAnalit);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					if(reader.Read())
					{
						if(!reader.IsDBNull(0))
						{
							SqlBytes sqlbytes = reader.GetSqlBytes(0);
							
							System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
							wizardText = win1251.GetString(sqlbytes.Value);
							this.parseWizardText(wizardText, wizardNode);
						}
					}
				}
				reader.Close();
			}
		}
		private string parseISBLText(string strIndent)
		{
			string isblText = "";
			while((this.linesWizardText[this.lineIndex].StartsWith(strIndent + "    "))&&(this.lineIndex < this.linesWizardCount))
			{
				//Получение отдельной строки текста
				string isblTextLine = this.linesWizardText[this.lineIndex].Trim().Replace("\r", "");
				//если строка не последняя, то она заканчивается на " +", его надо удалить
				if(isblTextLine.EndsWith(" +"))
				{
					isblTextLine = isblTextLine.Remove(isblTextLine.Length-2);
				}
				//разбор текста
				int charIndex = 0;
				while(charIndex < isblTextLine.Length)
				{
					char currentChar = isblTextLine[charIndex];
					switch (currentChar)
					{
					case '\'':
						//разбираем строку
						charIndex++;
						while((charIndex < isblTextLine.Length) && (isblTextLine[charIndex] != '\''))
						{
							isblText = isblText + isblTextLine[charIndex];
							charIndex++;
						}
						//пропускаем закрывающую кавычку
						charIndex++;
						break;
					case '#':
						charIndex++;
						String strCharCode = "";
						while((charIndex < isblTextLine.Length) && (isblTextLine[charIndex] >= '0') && (isblTextLine[charIndex] <= '9'))
						{
							strCharCode = strCharCode + isblTextLine[charIndex];
							charIndex++;
						}
						isblText = isblText + Char.ConvertFromUtf32(System.Convert.ToInt32(strCharCode));
						break;
					default:
						isblText = isblText + isblTextLine[charIndex];
						charIndex++;
						break;
					}
				}
				this.lineIndex++;
			}
			return isblText;
		}
		private EventStruct parseEventWizard()
		{
			EventStruct res = new EventStruct();
			//Проверка, что текущая строка равна "item"
			if(this.linesWizardText[this.lineIndex].Replace("\r", "").Trim() == "item")
			{
				//определение величины отступа
				string strIndent = this.linesWizardText[this.lineIndex].Remove(this.linesWizardText[this.lineIndex].IndexOf("item"));
				while((!this.linesWizardText[this.lineIndex].StartsWith(strIndent + "end"))&&(this.lineIndex < this.linesWizardCount))
				{
					if(this.linesWizardText[this.lineIndex].StartsWith(strIndent + "  ISBLText = "))
					{
						this.lineIndex++;
						res.text = this.parseISBLText(strIndent);
					}
					else
					if(this.linesWizardText[this.lineIndex].StartsWith(strIndent + "  EventType = "))
					{
						string strEventTypeSuffix = "EventType = ";
						res.name = this.linesWizardText[this.lineIndex].Trim().Remove(0, strEventTypeSuffix.Length);
						this.lineIndex++;
					}
					else
						this.lineIndex++;
				}
			}
			return res;
		}
		
		private void parseWizardText(string originText, isblTest.Node wizardNode)
		{
			this.originWizardText = originText;
		
			/**********************************************************************
			 * Структура свойств мастера находится в структуре, похожей на
			 * Delphi Form. Все теги расположены в отдельных строках.
			 * Значения тегов в формате Unicode. Свойствам мастера присуща
			 * Объектная структура. Иерархия соблюдается за счёт табуляции
			 * тегов. Если значение тега очень длинное то оно разбивается на
			 * несколько строк с указанием в конце строки символа конкатенации(+)
			 **********************************************************************/
			
			/**********************************************************************
			 * Сначала идёт три "События мастера":
			 * 	До выбора
			 * 	Начало
			 * 	Завершение
			 * 
			 * Потом идут этапы мастера, например "Этап 1: Запрос параметров совещания"
			 * 	Этап содержит два события:
			 * 		Начало
			 * 		Завершение
			 * 	А также события:
			 * 		Previous
			 * 		Next
			 * 		Finish
			 * 		Cancel
			 * 	Ещё есть событие:
			 * 		ОК
			 **********************************************************************/
			char[] charDelimeters = {'\n'};
			this.linesWizardText = originWizardText.Split(charDelimeters);
			this.linesWizardCount = linesWizardText.Length;
			this.lineIndex = 0;
			//Поиск начала секции с событиями маршрута
			while((this.lineIndex < this.linesWizardCount) && (linesWizardText[this.lineIndex].Replace("\r", "") != "  Events = <"))
			{
				this.lineIndex = this.lineIndex + 1;
			}

			
			isblTest.Node wizardEventsNode = new isblTest.Node();
			wizardEventsNode.name = "События мастера";
			wizardEventsNode.text = "";
			wizardEventsNode.parent = wizardNode;
			wizardEventsNode.nodes = new List<isblTest.Node>();
			//Загрузка "События мастера"
			while((this.lineIndex < this.linesWizardCount) && (linesWizardText[this.lineIndex].Replace("\r", "") != "    end>"))
			{
				//переходим к следующей строке
				this.lineIndex++;
				EventStruct eventInfo = this.parseEventWizard();
				isblTest.Node wizardEventInfoNode = new isblTest.Node();
				switch (eventInfo.name)
				{
				case "wetWizardBeforeSelection":
					wizardEventInfoNode.name = "До выбора";
					break;
				case "wetWizardStart":
					wizardEventInfoNode.name = "Начало";
					break;
				case "wetWizardFinish":
					wizardEventInfoNode.name = "Завершение";
					break;
				default:
					wizardEventInfoNode.name = "Неизвестное событие: " + eventInfo.name;
					break;
				}
				wizardEventInfoNode.text = eventInfo.text;
				wizardEventInfoNode.parent = wizardEventsNode;
				wizardEventsNode.nodes.Add(wizardEventInfoNode);
			}
			wizardNode.nodes.Add(wizardEventsNode);
		}
		public isblTest.Node Load()
		{
			isblTest.Node listNode = null;
			Int32 vidAnalitID = getVidAnalitID("WIZARDS");
			if(vidAnalitID >= 0)
			{
				listNode = new isblTest.Node();
				listNode.name = "Мастер действий";
				listNode.text = null;
				listNode.id = vidAnalitID;
				listNode.parent = null;
				listNode.nodes = new List<Node>();
				
				List<isblTest.Node> listGroups = LoadGroups(listNode);
				foreach(isblTest.Node groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select Analit, NameAn, Soder from MBAnalit where Vid=@vidAnalit and HighLvl=@groupID";
					SqlParameter paramVidAnalit = new SqlParameter("@vidAnalit", SqlDbType.Int, 10);
					SqlParameter paramGroupID = new SqlParameter("@groupID", SqlDbType.Int, 10);
					paramVidAnalit.Value = vidAnalitID;
					paramGroupID.Value = groupNode.id;
					command.Parameters.Add(paramVidAnalit);
					command.Parameters.Add(paramGroupID);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							isblTest.Node wizardNode = new isblTest.Node();
							wizardNode.parent = groupNode;
							wizardNode.id = reader.GetInt32(0);
							if(! reader.IsDBNull(1))
							{
								wizardNode.name = reader.GetString(1);
							}
							if(! reader.IsDBNull(2))
							{
								wizardNode.text = reader.GetString(2);
							}
							wizardNode.nodes = new List<isblTest.Node>();
							groupNode.nodes.Add(wizardNode);
						}
					}
					reader.Close();
					foreach(isblTest.Node wizardNode in groupNode.nodes)
					{
						LoadText(wizardNode);
					}
				}				
			}
			return listNode;
		}
	}
}
