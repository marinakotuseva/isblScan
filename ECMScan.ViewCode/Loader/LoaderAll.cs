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

		public EDocType			loaderEDocType;
		public Function			loaderFunction;
		public PerfomanceIndicator	loaderPerfomanceIndicator;
		public Reference			loaderReference;
		public Report				loaderReport;
		public ReportIntegrate		loaderReportInt;
		public Route				loaderRoute;
		public RouteBlock			loaderRouteBlock;
		public Script				loaderScript;
		public UserScript			loaderUserScript;
		public UserSearch			loaderUserSearch;
		public Wizard				loaderWizard;
		
		public string errorText;
		
		/// <summary>
		///Установка соединения с системой 
		/// </summary>
		/// <param name="server">Имя SQL Server</param>
		/// <param name="dataBase">Имя базы данных</param>
		/// <param name="login">Логин пользователя базы данных</param>
		/// <param name="password">Пароль пользователя базы данных</param>
		/// <returns>True - соединение успешно усановлено, False - соединение не установлено, текст ошибки соединения в поле errorText.</returns>
		public bool Connect(string server, string dataBase, string login, string password)
		{
			SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
			connBuilder.DataSource = server;
			connBuilder.Pooling = false;
			connBuilder.InitialCatalog = dataBase;
			connBuilder.ApplicationName = "ECMScan CodeView";
			connBuilder.UserID = login;
			connBuilder.Password = password;

			try {
				this.connection = new SqlConnection(connBuilder.ConnectionString);
				this.connection.Open();
				this.errorText = this.connection.ConnectionString;
				return true;
			} catch (Exception e) {
				this.errorText = e.Message;
				return false;
			}
		}
		
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
			loaderPerfomanceIndicator		= new PerfomanceIndicator(this.connection);
			loaderReference				= new Reference(this.connection);
			loaderReport				= new Report(this.connection);
			loaderReportInt				= new ReportIntegrate(this.connection);
			loaderRoute				= new Route(this.connection);
			loaderRouteBlock			= new RouteBlock(this.connection);
			loaderScript				= new Script(this.connection);
			loaderUserScript			= new UserScript(this.connection);
			loaderUserSearch			= new UserSearch(this.connection);
			loaderWizard				= new Wizard(this.connection);
			
			
			//Загрузка типов карточке электронных документов
			isblNode = loaderEDocType.Load();
			isblList.Add(isblNode);

			//Загрузка текстов функций
			isblNode = loaderFunction.Load();
			isblList.Add(isblNode);
			
			//Загрузка показателей эффективности
			isblNode = loaderPerfomanceIndicator.Load();
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

			//Загрузка типовых маршрутов (событий маршрутов)
			isblNode = loaderRoute.Load();
			isblList.Add(isblNode);

			//Загрузка вычислений блоков типовых маршрутов
			isblNode = loaderRouteBlock.Load();
			isblList.Add(isblNode);

			//Загрузка текстов расчётов (сценариев)
			isblNode = loaderScript.Load();
			isblList.Add(isblNode);
			
			//Загрузка пользовательских расчётов
			isblNode = loaderUserScript.Load();
			isblList.Add(isblNode);
			
			//Загрузка пользовательских поисков
			isblNode = loaderUserSearch.Load();
			isblList.Add(isblNode);

			//Загрузка вычислений мастеров действий
			isblNode = loaderWizard.Load();
			isblList.Add(isblNode);

			return isblList;
		}
	}
}
