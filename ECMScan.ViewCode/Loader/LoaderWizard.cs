/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Мастер действий.
	/// </summary>
	public class Wizard : LoaderCommon
	{
		public Wizard(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private List<IsbNode> LoadGroups(IsbNode rootNode)
		{
			var listGroups = new List<IsbNode>();
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
						var node = new IsbNode();
						node.Id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.Name = reader.GetString(1);
						}
						if(! reader.IsDBNull(2))
						{
							node.Text = reader.GetString(2);
						}
						node.Nodes = new List<IsbNode>();
						rootNode.Nodes.Add(node);
						listGroups.Add(node);
					}
				}
				reader.Close();
			}
			return listGroups;
		}
		
		private void ParseWizardText(string originText, IsbNode wizardNode)
		{

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
            var parsedWizardDfm = DfmParser.Parse(originText);

            // Wizard Events
            var events = parsedWizardDfm.Nodes.Where(n => n.PropertyName == "Events").First().Nodes;
            foreach(var ev in events){
                var textNode = ev.Nodes.Where(n => n.PropertyName == "ISBLText").FirstOrDefault();
                if(textNode != null)
                {
                    var wizardEventNode = new IsbNode();
                    wizardEventNode.Text = (string)textNode.PropertyValue;
                    var eventNameNode = ev.Nodes.Where(n => n.PropertyName == "EventType").FirstOrDefault();
                    if (eventNameNode != null)
                    {
                        var eventCode = (string)eventNameNode.PropertyValue;
                        wizardEventNode.Name =  _wizardEventCodeToName.ContainsKey(eventCode) ? _wizardEventCodeToName[eventCode] : eventCode;
                    }
                    else
                    {
                        wizardEventNode.Name = "Неизвестное событие";
                    }

                    wizardNode.Nodes.Add(wizardEventNode);
                }
            }


            foreach (var step in parsedWizardDfm.Nodes.Where(n => n.PropertyClass?.EndsWith("StepList") ?? false).First().Nodes.Where(n => n.PropertyClass?.EndsWith("WizardStep") ?? false))
            {
                var wizardStep = new IsbNode("");
      
                foreach (var eventNode in step.Nodes.Where(n => n.PropertyName == "Events").First().Nodes)
                {
                    var stepEventNode = new IsbNode();
                    foreach (var eventParam in eventNode.Nodes)
                    {
                        if (eventParam.PropertyName == "ISBLText")
                        {
                            stepEventNode.Text = (string)eventParam.PropertyValue;
                        }
                        if (eventParam.PropertyName == "EventType")
                        {
                            var eventCode = (string)eventParam.PropertyValue;
                            stepEventNode.Name = _wizardEventCodeToName.ContainsKey(eventCode) ? _wizardEventCodeToName[eventCode] : eventCode;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(stepEventNode.Text))
                    {
                        wizardStep.Nodes.Add(stepEventNode);
                    }
                }

                foreach (var actionNode in step.Nodes.Where(n => n.PropertyClass?.EndsWith("WizardActionList") ?? false).First().Nodes.Where(n => n.PropertyClass?.EndsWith("WizardAction") ?? false))
                {
                    var actionEventNode = new IsbNode();
                    foreach (var eventNode in actionNode.Nodes.Where(n => n.PropertyName == "Events").First().Nodes)
                    {
                        var textNode = eventNode.Nodes.Where(n => n.PropertyName == "ISBLText").FirstOrDefault();
                        if (textNode != null)
                        {
                            actionEventNode.Text = (string)textNode.PropertyValue;
                            actionEventNode.Name = (string)actionNode.Nodes.Where(n => n.PropertyName == "Title").First().PropertyValue;
                            wizardStep.Nodes.Add(actionEventNode);
                        }
                    }
                }

                if (wizardStep.Nodes.Count > 0)
                {
                    wizardStep.Name = (string)step.Nodes.Where(n => n.PropertyName == "Title").First().PropertyValue;
                    wizardNode.Nodes.Add(wizardStep);
                }
            }
        }


		public IsbNode Load()
		{
            System.Text.Encoding win1251 = System.Text.Encoding.GetEncoding(1251);
            IsbNode listNode = null;
			Int32 vidAnalitId = GetVidAnalitId("WIZARDS");
			if(vidAnalitId >= 0)
			{
				listNode = new IsbNode();
				listNode.Name = "Мастер действий";
				listNode.Text = null;
				listNode.Id = vidAnalitId;
				listNode.Nodes = new List<IsbNode>();
				
				var listGroups = LoadGroups(listNode);
				foreach(var groupNode in listGroups)
				{
					SqlCommand command = new SqlCommand();
					command.Connection = Connection;
					command.CommandText = @"
select MBAnalit.Analit
    , MBAnalit.NameAn
    , MBAnalit.Soder 
    , MBText.SearchCondition 
    , (select max(prot.DateAct)
        from XProtokol prot 
        where prot.SrcObjID = 119 and prot.SrcRecID = MBAnalit.Analit) as LastUpd
from MBAnalit 
    join MBText on MBText.SrcRecID = MBAnalit.Analit and MBText.SrcObjID = 119
where Vid=@vidAnalit 
    and HighLvl=@groupID";
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
							var wizardNode = new IsbNode();
							wizardNode.Id = reader.GetInt32(0);
							if(! reader.IsDBNull(1))
							{
								wizardNode.Name = reader.GetString(1);
							}
							if(! reader.IsDBNull(2))
							{
								wizardNode.Text = reader.GetString(2);
							}
                            if (!reader.IsDBNull(3))
                            {
                                SqlBytes sqlbytes = reader.GetSqlBytes(3);
                                try
                                {
                                    ParseWizardText(win1251.GetString(sqlbytes.Value), wizardNode);
                                }
                                catch (Exception e)
                                {
                                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                                    MessageBoxIcon icon = MessageBoxIcon.Information;
                                    MessageBox.Show("Ошибка парсинга схемы мастера " + wizardNode.Name, "Ошибка разбора схемы мастера", buttons, icon);
                                }
                            }
                            if (!reader.IsDBNull(4))
						    {
						        wizardNode.LastUpdate = reader.GetDateTime(4);
						    }
                            groupNode.Nodes.Add(wizardNode);
						}
					}
					reader.Close();
				}				
			}
			return listNode;
		}
	}
}
