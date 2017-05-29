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
		public string Name;
		public string Text;
	}
	/// <summary>
	/// Мастер действий.
	/// </summary>
	public class Wizard : LoaderCommon
	{
		private string _originWizardText;
		private string[] _linesWizardText;
		int _linesWizardCount;
		int _lineIndex;	
		public Wizard(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private List<Node> LoadGroups(Node rootNode)
		{
			List<Node> listGroups = new List<Node>();
			Int32 vidAnalitId = GetVidAnalitId("WIZARD_GROUPS");
			if(vidAnalitId >= 0)
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select Analit, NameAn, Soder from MBAnalit where Vid=@vidAnalit";
				SqlParameter paramVidAnalit = new SqlParameter("@vidAnalit", SqlDbType.Int);
				paramVidAnalit.Value = vidAnalitId;
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
			if(CheckTableExist("MBText"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
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
							this.ParseWizardText(wizardText, wizardNode);
						}
					}
				}
				reader.Close();
			}
		}
		private string ParseIsblText(string strIndent)
		{
			string isblText = "";
			while((this._linesWizardText[this._lineIndex].StartsWith(strIndent + "    "))&&(this._lineIndex < this._linesWizardCount))
			{
				//Получение отдельной строки текста
				string isblTextLine = this._linesWizardText[this._lineIndex].Trim().Replace("\r", "");
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
				this._lineIndex++;
			}
			return isblText;
		}
		private EventStruct ParseEventWizard()
		{
			EventStruct res = new EventStruct();
			//Проверка, что текущая строка равна "item"
			if(this._linesWizardText[this._lineIndex].Replace("\r", "").Trim() == "item")
			{
				//определение величины отступа
				string strIndent = this._linesWizardText[this._lineIndex].Remove(this._linesWizardText[this._lineIndex].IndexOf("item"));
				while((!this._linesWizardText[this._lineIndex].StartsWith(strIndent + "end"))&&(this._lineIndex < this._linesWizardCount))
				{
					if(this._linesWizardText[this._lineIndex].StartsWith(strIndent + "  ISBLText = "))
					{
						this._lineIndex++;
						res.Text = this.ParseIsblText(strIndent);
					}
					else
					if(this._linesWizardText[this._lineIndex].StartsWith(strIndent + "  EventType = "))
					{
						string strEventTypeSuffix = "EventType = ";
						res.Name = this._linesWizardText[this._lineIndex].Trim().Remove(0, strEventTypeSuffix.Length);
						this._lineIndex++;
					}
					else
						this._lineIndex++;
				}
			}
			return res;
		}
		
		private void ParseWizardText(string originText, Node wizardNode)
		{
			this._originWizardText = originText;
		
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
			this._linesWizardText = _originWizardText.Split(charDelimeters);
			this._linesWizardCount = _linesWizardText.Length;
			this._lineIndex = 0;
			//Поиск начала секции с событиями маршрута
			while((this._lineIndex < this._linesWizardCount) && (_linesWizardText[this._lineIndex].Replace("\r", "") != "  Events = <"))
			{
				this._lineIndex = this._lineIndex + 1;
			}

			
			Node wizardEventsNode = new Node();
			wizardEventsNode.Name = "События мастера";
			wizardEventsNode.Text = "";
			wizardEventsNode.Nodes = new List<Node>();
			//Загрузка "События мастера"
			while((this._lineIndex < this._linesWizardCount) && (_linesWizardText[this._lineIndex].Replace("\r", "") != "    end>"))
			{
				//переходим к следующей строке
				this._lineIndex++;
				EventStruct eventInfo = this.ParseEventWizard();
				Node wizardEventInfoNode = new Node();
				switch (eventInfo.Name)
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
					wizardEventInfoNode.Name = "Неизвестное событие: " + eventInfo.Name;
					break;
				}
				wizardEventInfoNode.Text = eventInfo.Text;
				wizardEventsNode.Nodes.Add(wizardEventInfoNode);
			}
			wizardNode.Nodes.Add(wizardEventsNode);
			//Загрузка этапов мастера
			
		}
		public Node Load()
		{
			Node listNode = null;
			Int32 vidAnalitId = GetVidAnalitId("WIZARDS");
			if(vidAnalitId >= 0)
			{
				listNode = new Node();
				listNode.Name = "Мастер действий";
				listNode.Text = null;
				listNode.Id = vidAnalitId;
				listNode.Nodes = new List<Node>();
				
				List<Node> listGroups = LoadGroups(listNode);
				foreach(Node groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = Connection;
					command.CommandText = "select Analit, NameAn, Soder from MBAnalit where Vid=@vidAnalit and HighLvl=@groupID";
					SqlParameter paramVidAnalit = new SqlParameter("@vidAnalit", SqlDbType.Int);
					SqlParameter paramGroupId = new SqlParameter("@groupID", SqlDbType.Int);
					paramVidAnalit.Value = vidAnalitId;
					paramGroupId.Value = groupNode.Id;
					command.Parameters.Add(paramVidAnalit);
					command.Parameters.Add(paramGroupId);
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
