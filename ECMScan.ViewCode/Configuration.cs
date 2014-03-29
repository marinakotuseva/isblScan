
using System;

namespace ISBLScan.ViewCode
{

	/// <summary>
	/// Класс для хранения конфигурации программы
	/// </summary>
	public static class Configuration
	{
		private static string nameSqlServer = "Sql Server:";
		private static string nameDataBase = "Data Base:";
		private static string nameLogin = "Login:";
		
		private static string configFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/ISBLScan.ViewCode.cfg";
		

		public static bool Load(out string sqlServer, out string dataBase, out string login)
		{
			sqlServer = "";
			dataBase = "";
			login = "";
			if(System.IO.File.Exists(configFilePath))
			{
				string[] configStrings = System.IO.File.ReadAllLines(configFilePath);
				foreach(string configString in configStrings)
				{
					if(configString.StartsWith(nameSqlServer))
					{
						sqlServer = configString.Remove(0, nameSqlServer.Length);
					}
					if(configString.StartsWith(nameDataBase))
					{
						dataBase = configString.Remove(0, nameDataBase.Length);
					}
					if(configString.StartsWith(nameLogin))
					{
						login = configString.Remove(0, nameLogin.Length);
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public static void Save(string sqlServer, string dataBase, string login)
		{
			System.IO.File.WriteAllText(configFilePath,
			                            String.Format("{0}{1}\n{2}{3}\n{4}{5}",
			                                          nameSqlServer, sqlServer,
			                                          nameDataBase, dataBase,
			                                          nameLogin, login));
			                                                
		}
	}
}
