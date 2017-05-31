using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
		
		private SqlConnection _connection;

		public EDocType			    LoaderEDocType;
		public Function			    LoaderFunction;
		public Reference			LoaderReference;
		public Report				LoaderReport;
		public ReportIntegrate		LoaderReportInt;
		public Route				LoaderRoute;
		public RouteBlock			LoaderRouteBlock;
		public Script				LoaderScript;
		public Wizard				LoaderWizard;
        public CustomCalculations   LoaderCustom;

        public string ErrorText;
		
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
				this._connection = new SqlConnection(connBuilder.ConnectionString);
				this._connection.Open();
				this.ErrorText = null;
                //tryLoadAndExecuteDebugSQLScript(connection);
				return true;
			} catch (Exception e) {
				this.ErrorText = e.Message;
				return false;
			}
		}

		/// <summary>
		///Отключиться от базы данных 
		/// </summary>
		public void Disconnect()
		{
			this._connection.Close();
		}
		
		/// <summary>
		///Зарузка списка узлов, для их отображения в дереве элементов 
		/// </summary>
		/// <returns>Список узлов</returns>
		public List<Node> Load(List<Node> isblList)
		{
			Node isblNode;
			
			LoaderEDocType				= new EDocType(this._connection);
			LoaderFunction				= new Function(this._connection);
			LoaderReference				= new Reference(this._connection);
			LoaderReport				= new Report(this._connection);
			LoaderReportInt				= new ReportIntegrate(this._connection);
			LoaderRoute				    = new Route(this._connection);
			LoaderRouteBlock			= new RouteBlock(this._connection);
			LoaderScript				= new Script(this._connection);
			LoaderWizard				= new Wizard(this._connection);
            LoaderCustom                = new CustomCalculations(this._connection);


            //Загрузка типов карточке электронных документов
            isblNode = LoaderEDocType.Load();
            isblList.Add(isblNode);

            //Загрузка текстов функций
            isblNode = LoaderFunction.Load();
            isblList.Add(isblNode);

            //Загрузка текстов событий справочников, вычислений реквизитов, расчётов на форме
            isblNode = LoaderReference.Load();
            isblList.Add(isblNode);

            //Загрузка отчётов (шаблонов и расчётов)
            isblNode = LoaderReport.Load();
            isblList.Add(isblNode);

            //Загрузка интегрированных отчётов (шаблонов и расчётов)
            isblNode = LoaderReportInt.Load();
            isblList.Add(isblNode);

            //Загрузка типовых маршрутов(событий маршрутов)
            isblNode = LoaderRoute.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений блоков типовых маршрутов
            isblNode = LoaderRouteBlock.Load();
            isblList.Add(isblNode);

            //Загрузка текстов расчётов (сценариев)
            isblNode = LoaderScript.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений мастеров действий
            isblNode = LoaderWizard.Load();
            isblList.Add(isblNode);

            //Загрузка вычислений из справочников
            isblNode = LoaderCustom.Load();
            isblList.Add(isblNode);

            return isblList;
		}
	}
}
