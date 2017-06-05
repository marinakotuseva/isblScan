/*
 * Date: 30.09.2012
 * Time: 16:52
 */
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

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
	        {'A', "Action"},
	        {'C', "Card"},
	        {'D', "Table"},
	        {'E', "Table 2"},
	        {'F', "Table 3"},
	        {'G', "Table 4"},
	        {'J', "Table 5"},
	        {'K', "Table 6"}
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

        public string CommentStrings(string comment)
        {
            string[] delimeters = { "\n\r" };
            string[] commentArray = comment.Split(delimeters, StringSplitOptions.None);
            string result = "";
            foreach (string commentStr in commentArray)
            {
                if (!string.IsNullOrEmpty(result))
                    result += "\n\r";
                result += "/// " + commentStr;
            }
            return result;
        }
	}
}
