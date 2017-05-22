/*
 * Date: 30.09.2012
 * Time: 14:59
 */
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Загрузчик прикладной разработки. Устанавливает соединение с системой и вызывает загрузчики конкретных компонент.
	/// </summary>
	public class Loader
	{
		public Loader()
		{
		}
		
		private SqlConnection connection;

		public EDocType			    loaderEDocType;
		public Function			    loaderFunction;
		public Reference			loaderReference;
		public Report				loaderReport;
		public ReportIntegrate		loaderReportInt;
		public Route				loaderRoute;
		public RouteBlock			loaderRouteBlock;
		public Script				loaderScript;
		public Wizard				loaderWizard;
        public CustomCalculations   loaderCustom;

        public string errorText;
		
		/// <summary>
		/// Установка соединения с системой 
		/// </summary>
		/// <param name="server">Имя SQL Server</param>
		/// <param name="dataBase">Имя базы данных</param>
		/// <param name="login">Логин пользователя базы данных</param>
		/// <param name="password">Пароль пользователя базы данных</param>
		/// <returns>True - соединение успешно усановлено, False - соединение не установлено, текст ошибки соединения в поле errorText.</returns>
		public bool Connect(string server, string dataBase, string login = "", string password = "", bool isWinAuth = false)
		{
			SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
			connBuilder.DataSource = server;
			connBuilder.Pooling = false;
			connBuilder.InitialCatalog = dataBase;
			connBuilder.ApplicationName = "ISBLScan.ViewCode";
			if(isWinAuth)
			{
				connBuilder.IntegratedSecurity = true;
			}
			else
			{
				connBuilder.UserID = login;
				connBuilder.Password = password;
			}
			try {
				this.connection = new SqlConnection(connBuilder.ConnectionString);
				this.connection.Open();
				this.errorText = null;
                //tryLoadAndExecuteDebugSQLScript(connection);
				return true;
			} catch (Exception e) {
				this.errorText = e.Message;
				return false;
			}
		}

        /// <summary>
        /// Отладочный метод. Вариант того, как вывести SQL-скрипты из тела утилиты в SQL-файлы, которые проще править и поддерживать.
        /// TODO: отладить такой способ выполнения скриптов, перевести работу с SQL на такой вариант выполнения или отказаться, выпилить метод.
        /// </summary>
        /// <param name="sqlConnection"></param>
        //void tryLoadAndExecuteDebugSQLScript(SqlConnection sqlConnection)
        //{
        //    try
        //    {
        //        string debugSQLScript = System.IO.File.ReadAllText(string.Format("ISBLScan.ViewCode.DebugScript.{0}.sql", sqlConnection.Database));
        //        SqlCommand command = new SqlCommand();
        //        command.Connection = sqlConnection;
        //        command.CommandText = debugSQLScript;
        //        command.ExecuteNonQuery();
        //    }
        //    catch (SqlException ex)
        //    {
        //        //TODO: добавить обработку исключений
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO: добавить обработку исключений
        //    }
        //}

		/// <summary>
		///Отключиться от базы данных 
		/// </summary>
		public void disconnect()
		{
			this.connection.Close();
		}
		
		/// <summary>
		///Зарузка списка узлов, для их отображения в дереве элементов 
		/// </summary>
		/// <returns>Список узлов</returns>
		public List<Node> Load()
		{
			List<Node> isblList = new List<Node>();
			Node isblNode;
			
			loaderEDocType				= new EDocType(this.connection);
			loaderFunction				= new Function(this.connection);
			loaderReference				= new Reference(this.connection);
			loaderReport				= new Report(this.connection);
			loaderReportInt				= new ReportIntegrate(this.connection);
			loaderRoute				    = new Route(this.connection);
			loaderRouteBlock			= new RouteBlock(this.connection);
			loaderScript				= new Script(this.connection);
			loaderWizard				= new Wizard(this.connection);
            loaderCustom                = new CustomCalculations(this.connection);


            //Загрузка типов карточке электронных документов
            isblNode = loaderEDocType.Load();
            isblList.Add(isblNode);

            //Загрузка текстов функций
            isblNode = loaderFunction.Load();
            isblList.Add(isblNode);

            //Загрузка текстов событий справочников, вычислений реквизитов, расчётов на форме
            isblNode = loaderReference.Load();
            isblList.Add(isblNode);

            //Загрузка отчётов (шаблонов и расчётов)
            isblNode = loaderReport.Load();
            isblList.Add(isblNode);

            //Загрузка интегрированных отчётов (шаблонов и расчётов)
            isblNode = loaderReportInt.Load();
            isblList.Add(isblNode);

            //Загрузка типовых маршрутов(событий маршрутов)
            isblNode = loaderRoute.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений блоков типовых маршрутов
            isblNode = loaderRouteBlock.Load();
            isblList.Add(isblNode);

            //Загрузка текстов расчётов (сценариев)
            isblNode = loaderScript.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений мастеров действий
            isblNode = loaderWizard.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений из справочников
            isblNode = loaderCustom.Load();
            isblList.Add(isblNode);

            return isblList;
		}
	}
}
