/*
 * Date: 30.09.2012
 * Time: 16:52
 */
using System;
using System.Data;
using System.Data.SqlClient;

namespace isblTest
{
	/// <summary>
	/// Description of LoaderCommon.
	/// </summary>
	public class LoaderCommon
	{
		//Соединение с БД
		protected SqlConnection connection;

		public LoaderCommon(SqlConnection sqlConnect)
		{
			this.connection = sqlConnect;
		}
		
		public bool checkTableExist(string tableName)
		{
			bool flagTableExist;
			
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.CommandText = "select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @tableName";
			SqlParameter paramTableName = new SqlParameter("@tableName", SqlDbType.VarChar, 255);
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
		
		public Int32 getVidAnalitID(string vidAnalitKod)
		{
			Int32 vidAnalitID = -1;
			if(this.checkTableExist("MBVidAn"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select Vid from MBVidAn where Kod = @vidAnalitKod";
				SqlParameter paramVidAnalitKod = new SqlParameter("@vidAnalitKod", SqlDbType.VarChar, 255);
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
	}
}
