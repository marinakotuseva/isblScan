/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBLScan.ViewCode
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
		
		private List<Node> LoadGroups(Node rootNode)
		{
			List<Node> listGroups = new List<Node>();
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
						Node node = new Node();
						node.Id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.Name = reader.GetString(1);
						}
						if(! reader.IsDBNull(2))
						{
							node.Text = reader.GetString(2);
						}
						node.Nodes = new List<Node>();
						rootNode.Nodes.Add(node);
						listGroups.Add(node);
					}
				}
				reader.Close();
			}
			return listGroups;
		}
		private void LoadText(Node wizardNode)
		{
			string wizardText;
			if(checkTableExist("MBText"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select SearchCondition from MBText where SrcRecID=@wizardAnalit";
				SqlParameter wizardAnalit = new SqlParameter("@wizardAnalit", SqlDbType.Int);
				wizardAnalit.Value = wizardNode.Id;
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
		
		private void parseWizardText(string originText, Node wizardNode)
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

			
			Node wizardEventsNode = new Node();
			wizardEventsNode.Name = "События мастера";
			wizardEventsNode.Text = "";
			wizardEventsNode.Nodes = new List<Node>();
			//Загрузка "События мастера"
			while((this.lineIndex < this.linesWizardCount) && (linesWizardText[this.lineIndex].Replace("\r", "") != "    end>"))
			{
				//переходим к следующей строке
				this.lineIndex++;
				EventStruct eventInfo = this.parseEventWizard();
				Node wizardEventInfoNode = new Node();
				switch (eventInfo.name)
				{
				case "wetWizardBeforeSelection":
					wizardEventInfoNode.Name = "До выбора";
					break;
				case "wetWizardStart":
					wizardEventInfoNode.Name = "Начало";
					break;
				case "wetWizardFinish":
					wizardEventInfoNode.Name = "Завершение";
					break;
				default:
					wizardEventInfoNode.Name = "Неизвестное событие: " + eventInfo.name;
					break;
				}
				wizardEventInfoNode.Text = eventInfo.text;
				wizardEventsNode.Nodes.Add(wizardEventInfoNode);
			}
			wizardNode.Nodes.Add(wizardEventsNode);
			//Загрузка этапов мастера
			
		}
		public Node Load()
		{
			Node listNode = null;
			Int32 vidAnalitID = getVidAnalitID("WIZARDS");
			if(vidAnalitID >= 0)
			{
				listNode = new Node();
				listNode.Name = "Мастер действий";
				listNode.Text = null;
				listNode.Id = vidAnalitID;
				listNode.Nodes = new List<Node>();
				
				List<Node> listGroups = LoadGroups(listNode);
				foreach(Node groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.CommandText = "select Analit, NameAn, Soder from MBAnalit where Vid=@vidAnalit and HighLvl=@groupID";
					SqlParameter paramVidAnalit = new SqlParameter("@vidAnalit", SqlDbType.Int);
					SqlParameter paramGroupID = new SqlParameter("@groupID", SqlDbType.Int);
					paramVidAnalit.Value = vidAnalitID;
					paramGroupID.Value = groupNode.Id;
					command.Parameters.Add(paramVidAnalit);
					command.Parameters.Add(paramGroupID);
					command.Prepare();
					SqlDataReader reader = command.ExecuteReader();
					if(reader.HasRows)
					{
						while(reader.Read())
						{
							Node wizardNode = new Node();
							wizardNode.Id = reader.GetInt32(0);
							if(! reader.IsDBNull(1))
							{
								wizardNode.Name = reader.GetString(1);
							}
							if(! reader.IsDBNull(2))
							{
								wizardNode.Text = reader.GetString(2);
							}
							wizardNode.Nodes = new List<Node>();
							groupNode.Nodes.Add(wizardNode);
						}
					}
					reader.Close();
					foreach(Node wizardNode in groupNode.Nodes)
					{
						LoadText(wizardNode);
					}
				}				
			}
			return listNode;
		}
	}
}
