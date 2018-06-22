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
	}
}
