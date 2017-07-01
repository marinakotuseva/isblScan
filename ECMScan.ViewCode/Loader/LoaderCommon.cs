/*
 * Date: 30.09.2012
 * Time: 16:52
 */
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Базовый класс, от которого наследуются другие загрузчики прикладной разрабоки.
	/// </summary>
	public class LoaderCommon
	{
		/// <summary>
		///Соединение с БД 
		/// </summary>
		protected SqlConnection Connection;

	    protected readonly Dictionary<Char, String> _sectionCodeToName = new Dictionary<char, string>()
	    {
	        {'Ш', "Карточка"},
	        {'Т', "Таблица"},
	        {'С', "Таблица 2"},
	        {'Р', "Таблица 3"},
	        {'О', "Таблица 4"},
	        {'Н', "Таблица 5"},
	        {'М', "Таблица 6"},
	        {'Q', "Таблица 7"},
	        {'W', "Таблица 8"},
	        {'U', "Таблица 9"},
	        {'R', "Таблица 10"},
	        {'I', "Таблица 11"},
	        {'Y', "Таблица 12"},
	        {'B', "Таблица 13"},
	        {'H', "Таблица 14"},
	        {'L', "Таблица 15"},
	        {'M', "Таблица 16"},
	        {'N', "Таблица 17"},
	        {'P', "Таблица 18"},
	        {'O', "Таблица 19"},
	        {'S', "Таблица 20"},
	        {'T', "Таблица 21"},
	        {'V', "Таблица 22"},
	        {'X', "Таблица 23"},
	        {'Z', "Таблица 24"},
	        {'К', "Действие"},
	        {'A', "Действие"},
	        {'C', "Карточка"},
	        {'D', "Таблица"},
	        {'E', "Таблица 2"},
	        {'F', "Таблица 3"},
	        {'G', "Таблица 4"},
	        {'J', "Таблица 5"},
	        {'K', "Таблица 6"}
	    };

        protected readonly Dictionary<string, String> _eventCodePartToTitle = new Dictionary<string, string>()
        {
            {"DIALOG.CREATE", "Диалог. Создание"},
            {"DIALOG.VALID_CLOSE_WITH_RESULT", "Диалог. Закрытие Возможность"},
            {"DIALOG.CLOSE", "Диалог. Закрытие"},
            {"FORM.DIALOG_SHOW", "Форма. Показ"},
            {"FORM.DIALOG_HIDE", "Форма. Скрытие"},

            {"OPEN", "Открытие"},
            {"CLOSE", "Закрытие"},
            {"DATASET", "Набор данных"},
            {"CARD", "Запись"},
            {"VALID_DELETE", "Удаление возможность"},
            {"BEFORE_DELETE", "Удаление до"},
            {"AFTER_DELETE", "Удаление после"},
            {"BEFORE_INSERT", "Добавление до"},
            {"AFTER_INSERT", "Добавление после"},
            {"VALID_UPDATE", "Сохранение возможность"},
            {"BEFORE_UPDATE", "Сохранение до"},
            {"AFTER_UPDATE", "Сохранение после"},
            {"BEFORE_CANCEL", "Отмена до"},
            {"AFTER_CANCEL", "Отмена после"},
            {"FORM", "Форма-карточка"},
            {"SHOW", "Показ"},
            {"HIDE", "Скрытие"},
            {"TABLE", "Таблица"}
        };

        protected readonly Dictionary<String, String> _wizardEventCodeToName = new Dictionary<string, string>()
        {
            {"wetWizardBeforeSelection", "До выбора"},
            {"wetWizardStart", "Старт"},
            {"wetWizardFinish", "Завершение"},
            {"wetStepStart", "Старт"},
            {"wetStepFinish", "Завершение"},
        };

        protected readonly string[] _eventIdsRegExPatterns = 
        {
            "DATASET{9AFC8FC7-30C4-4076-9076-6E09A49B791C}.[A-Z А-Я_]+",
            "CARD{2147B5A6-496E-4EFF-88D9-78970D889F1F}.[A-Z А-Я_]+",
            "FORM{B28D55C1-651A-46C9-AD4E-50E73EF213A8}.[A-Z А-Я_]+",
            "TABLE{D402E843-74B2-4DC1-BFFD-DE677B48452C}[0-9]*.[A-Z А-Я_]+",
            "DIALOG{3AA220D8-D906-4914-8586-F534A4C3767E}.[A-Z А-Я_]+",
            "КАРТОЧКА.[A-Z А-Я_]+",
            "НАБОР ДАННЫХ.[A-Z А-Я_]+",
            "ТАБЛИЦА[0-9]*.[A-Z А-Я_]+",
            "DATASET.[A-Z А-Я_]+",
            "CARD.[A-Z А-Я_]+",
            "FORM.[A-Z А-Я_]+",
            "TABLE[0-9]*.[A-Z А-Я_]+"
        };

        /// <summary>
        /// Конструктор базового класса для всех загрузчиков прикладной разработки.
        /// </summary>
        /// <param name="sqlConnect">
        /// Соединение с базой данных (должно быть предварительно открыто)
        /// </param>
        public LoaderCommon(SqlConnection sqlConnect)
		{
			this.Connection = sqlConnect;
		}
		
		/// <summary>
		/// Проверка наличия таблицы с указанным именем в базы данных 
		/// </summary>
		/// <param name="tableName">
		/// Имя таблицы
		/// </param>
		/// <returns>
		/// true - таблица, с указанным именем есть в базы данных; false - таблицы с указанным именем нет в базы данных.
		/// </returns>
		public bool CheckTableExist(string tableName)
		{
			bool flagTableExist;
			
			SqlCommand command = new SqlCommand();
			command.Connection = Connection;
			command.CommandText = "select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @tableName";
			SqlParameter paramTableName = new SqlParameter("@tableName", SqlDbType.NVarChar, 255);
			paramTableName.Value = tableName;
			command.Parameters.Add(paramTableName);
			command.DesignTimeVisible = false;
			command.Prepare();
			SqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
			if(reader.HasRows)
			{
				flagTableExist = true;
			}
			else
			{
				flagTableExist = false;
			}
			reader.Close();
			reader.Dispose();
			command.Dispose();
			return flagTableExist;			
		}
		
		/// <summary>
		/// Получение ИД для вида аналитики с указанным имененем.
		/// Часто используемый метод, поэтому вынесен в базовый класс.
		/// </summary>
		/// <param name="vidAnalitKod">
		/// Строка с кодом видам аналитики (значение колонки Kod в таблице MBVidAn)
		/// </param>
		/// <returns>
		/// null - аналитика с указанным кодом не найдена, либо ИД аналитики
		/// </returns>
		public int GetVidAnalitId(string vidAnalitKod)
		{
			int vidAnalitId = -1;
			if(this.CheckTableExist("MBVidAn"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select Vid from MBVidAn where Kod = @vidAnalitKod";
				SqlParameter paramVidAnalitKod = new SqlParameter("@vidAnalitKod", SqlDbType.NVarChar, 255);
				paramVidAnalitKod.Value = vidAnalitKod;
				command.Parameters.Add(paramVidAnalitKod);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					if(reader.Read())
					{
						if(!reader.IsDBNull(0))
						{
							vidAnalitId = reader.GetInt32(0);
						}
					}
				}
				reader.Close();
			}
			return vidAnalitId;
		}

        public void ParseEvents(string eventsText, IsbNode parent)
        {
            var pattern = $"^({String.Join("|", _eventIdsRegExPatterns)})";
            var events = Regex.Split(eventsText, pattern, RegexOptions.Multiline);
            if(events.Length > 0)
            {
                var eventsNode = new IsbNode("События");
                IsbNode eventNode = null;
                foreach (var eventText in events)
                {
                    if (!String.IsNullOrWhiteSpace(eventText))
                    {
                        if(Regex.IsMatch(eventText, pattern))
                        {
                            var eventName = Regex.Replace(eventText, "\\{[-\\w]+\\}", "");
                            foreach(var part in _eventCodePartToTitle.Keys)
                            {
                                eventName = eventName.Replace(part, _eventCodePartToTitle[part]);
                            }
                            eventNode = new IsbNode(eventName);
                        }
                        else
                        {
                            eventNode.Text = eventText.Substring(2);
                            eventsNode.Nodes.Add(eventNode);
                        }
                    }
                }
                parent.Nodes.Add(eventsNode);
            }
        }
	}
}
