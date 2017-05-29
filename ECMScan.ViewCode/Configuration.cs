
using System;

namespace ISBLScan.ViewCode
{

	/// <summary>
	/// Класс для хранения конфигурации программы
	/// </summary>
	public static class Configuration
	{
		private static string _nameSqlServer = "Sql Server:";
		private static string _nameDataBase = "Data Base:";
		private static string _nameLogin = "Login:";
		private static string _nameIsWinAuth = "IsWinAuth:";
		
		private static string _configFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/ISBLScan.ViewCode.cfg";
		

		public static bool Load(out string sqlServer, out string dataBase, out string login, out bool isWinAuth)
		{
			sqlServer = "";
			dataBase = "";
			login = "";
			isWinAuth = true;
			if(System.IO.File.Exists(_configFilePath))
			{
				string[] configStrings = System.IO.File.ReadAllLines(_configFilePath);
				foreach(string configString in configStrings)
				{
					if(configString.StartsWith(_nameSqlServer))
					{
						sqlServer = configString.Remove(0, _nameSqlServer.Length);
					}
					if(configString.StartsWith(_nameDataBase))
					{
						dataBase = configString.Remove(0, _nameDataBase.Length);
					}
					if(configString.StartsWith(_nameLogin))
					{
						login = configString.Remove(0, _nameLogin.Length);
					}
					if(configString.StartsWith(_nameIsWinAuth))
					{
						isWinAuth = Convert.ToBoolean(configString.Remove(0, _nameIsWinAuth.Length));
					}					
				}
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public static void Save(string sqlServer, string dataBase, string login, bool isWinAuth)
		{
			System.IO.File.WriteAllText(_configFilePath,
			                            String.Format("{0}{1}\n{2}{3}\n{4}{5}\n{6}{7}",
			                                          _nameSqlServer, sqlServer,
			                                          _nameDataBase, dataBase,
			                                          _nameLogin, login,
			                                          _nameIsWinAuth, isWinAuth));
			                                                
		}
	}
}
