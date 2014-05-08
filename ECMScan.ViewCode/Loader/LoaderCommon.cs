/*
 * Date: 30.09.2012
 * Time: 16:52
 */
using System;
using System.Data;
using System.Data.SqlClient;

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
		protected SqlConnection connection;
		
		/// <summary>
		/// Конструктор базового класса для всех загрузчиков прикладной разработки.
		/// </summary>
		/// <param name="sqlConnect">
		/// Соединение с базой данных (должно быть предварительно открыто)
		/// </param>
		public LoaderCommon(SqlConnection sqlConnect)
		{
			this.connection = sqlConnect;
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
		public bool checkTableExist(string tableName)
		{
			bool flagTableExist;
			
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
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
		public int getVidAnalitID(string vidAnalitKod)
		{
			int vidAnalitID = -1;
			if(this.checkTableExist("MBVidAn"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
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
							vidAnalitID = reader.GetInt32(0);
						}
					}
				}
				reader.Close();
			}
			return vidAnalitID;
		}

        public string commentStrings(string comment)
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
