/*
 * Date: 10.08.2012
 * Time: 21:23
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace isblTest
{
	/// <summary>
	/// Справочник (аналитика).
	/// </summary>
	public class Reference : LoaderCommon
	{
		public Reference(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		/// <summary>
		/// Загрузка реквизитов справочника
		/// </summary>
		/// <param name="refNode">
		/// Ссылка на узел справочника
		/// A <see cref="isblTest.Node"/>
		/// </param>
		void LoadRecvisite(isblTest.Node recvGroupNode)
		{
			if(this.checkTableExist("MBVidAnRecv"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.connection;
				command.CommandText = "SELECT [Name], [Kod], [Exprn], [InpExprn] FROM [MBVidAnRecv] WHERE [Vid]=@Vid AND [Razd]=@Razd ORDER BY [NumRecv]";
				SqlParameter paramVid = new SqlParameter("@Vid", SqlDbType.Int, 10);
				paramVid.Value = recvGroupNode.id;
				SqlParameter paramRazd = new SqlParameter("@Razd", SqlDbType.Char, 1);
				paramRazd.Value = recvGroupNode.code;
				command.Parameters.Add(paramVid);
				command.Parameters.Add(paramRazd);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						isblTest.Node recvNode = new isblTest.Node();
						recvNode.parent = recvGroupNode;
						recvNode.nodes = new List<isblTest.Node>();
						if(!reader.IsDBNull(0))
						{
							recvNode.name = reader.GetString(0);
						}
						if(!reader.IsDBNull(1))
						{
							recvNode.code = reader.GetString(1);
							recvNode.name += " (" + recvNode.code + ")";
						}
						if(!reader.IsDBNull(2))
						{
							isblTest.Node exprnRefRecvNode = new isblTest.Node();
							exprnRefRecvNode.name = "-=[ Вычисление ]=-";
							exprnRefRecvNode.text = reader.GetString(2);
							exprnRefRecvNode.parent = recvNode;
							recvNode.nodes.Add(exprnRefRecvNode);
						}
						if(!reader.IsDBNull(3))
						{
							isblTest.Node inpExprnRefRecvNode = new isblTest.Node();
							inpExprnRefRecvNode.name = "-=[ Выбор из справочника ]=-";
							inpExprnRefRecvNode.text = reader.GetString(3);
							inpExprnRefRecvNode.parent = recvNode;
							recvNode.nodes.Add(inpExprnRefRecvNode);
						}
						recvGroupNode.nodes.Add(recvNode);
					}
				}
				reader.Close();
			}
		}


		/// <summary>
		///Получение имени раздела реквизитов/действий по коду
		/// </summary>
		/// <param name="groupCode">
		/// Код раздела реквизитов/действий
		/// </param>
		/// <returns>
		/// Имя раздела реквизитов/действий
		/// </returns>
		string GetGroupRecvName(char groupCode)
		{
			string groupName = "";
			switch (groupCode)
			{
			case 'Ш':
				groupName = "Карточка";
				break;
			case 'Т':
				groupName = "Таблица";
				break;
			case 'С':
				groupName = "Таблица 2";
				break;
			case 'Р':
				groupName = "Таблица 3";
				break;
			case 'О':
				groupName = "Таблица 4";
				break;
			case 'Н':
				groupName = "Таблица 5";
				break;
			case 'М':
				groupName = "Таблица 6";
				break;
			case 'Q':
				groupName = "Таблица 7";
				break;
			case 'W':
				groupName = "Таблица 8";
				break;
			case 'U':
				groupName = "Таблица 9";
				break;
			case 'R':
				groupName = "Таблица 10";
				break;
			case 'I':
				groupName = "Таблица 11";
				break;
			case 'Y':
				groupName = "Таблица 12";
				break;
			case 'K':
				groupName = "Действие";
				break;
			default:
				groupName = "Неизвестный раздел (" + groupCode + ")";
				break;
			}
			return groupName;
		}

		/// <summary>
		/// Загрузка разделов реквизитов
		/// </summary>
		/// <param name="refNode">
		/// A <see cref="isblTest.Node"/>
		/// </param>
		void LoadGroupRecv(isblTest.Node refNode)
		{
			if(this.checkTableExist("MBVidAnRecv"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.connection;
				command.CommandText = "SELECT [Razd] FROM [MBVidAnRecv] WHERE [Vid] = @Vid GROUP BY [Razd] ORDER BY [Razd] DESC";
				SqlParameter paramVid = new SqlParameter("@Vid", SqlDbType.Int, 10);
				paramVid.Value = refNode.id;
				command.Parameters.Add(paramVid);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();

				isblTest.Node refRecvNode = new isblTest.Node();
				refRecvNode.nodes = new List<isblTest.Node>();

				if(reader.HasRows)
				{
					refRecvNode.name = "-=[ Реквизиты и действия типа справочника ]=-";
					refRecvNode.text = null;
					refRecvNode.parent = refNode;
					refNode.nodes.Add(refRecvNode);

					while(reader.Read())
					{
						isblTest.Node recvGroupNode = new isblTest.Node();
						//Код раздела
						recvGroupNode.code = reader.GetString(0);
						//Вид аналитики
						recvGroupNode.id = refNode.id;
						recvGroupNode.name = GetGroupRecvName(recvGroupNode.code[0]);
						recvGroupNode.parent = refRecvNode;
						recvGroupNode.nodes = new List<isblTest.Node>();
						refRecvNode.nodes.Add(recvGroupNode);
					}
				}
				reader.Close();
				foreach(isblTest.Node recvGroupNode in refRecvNode.nodes)
				{
					this.LoadRecvisite(recvGroupNode);
				}
			}
		}

		public isblTest.Node Load()
		{
			isblTest.Node rootRefNode = null;
			if(this.checkTableExist("MBVidAn"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connection;
				command.CommandText = "select Vid, Name, Kod, Comment, Exprn from MBVidAn order by Name ASC";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					rootRefNode = new isblTest.Node();
					rootRefNode.name = "Тип справочника";
					rootRefNode.text = null;
					rootRefNode.parent = null;
					rootRefNode.nodes = new List<Node>();
					
					while(reader.Read())
					{
						isblTest.Node refNode = new isblTest.Node();
						refNode.parent = rootRefNode;
						refNode.nodes = new List<isblTest.Node>();
						//ИД 
						refNode.id = reader.GetInt32(0);
						//Имя (Код)
						if((! reader.IsDBNull(1))&&(! reader.IsDBNull(2)))
						{
							refNode.name = reader.GetString(1).Trim() + " (" + reader.GetString(2).Trim() + ")";
						}
						refNode.nodes = new List<isblTest.Node>();
						//Примечание к типу справочника
						if(! reader.IsDBNull(3))
						{
							refNode.text = reader.GetString(3).Trim();
						}
						//События типа справочника
						if(! reader.IsDBNull(4))
						{
							isblTest.Node refEventNode = new isblTest.Node();
							refEventNode.name = "-=[ События ]=-";
							refEventNode.text = reader.GetString(4).Trim();
							refEventNode.parent = refNode;
							refNode.nodes.Add(refEventNode);
						}
						rootRefNode.nodes.Add(refNode);
					}
				}
				reader.Close();

				foreach(isblTest.Node refNode in rootRefNode.nodes)
				{
					LoadGroupRecv(refNode);
				}

			}
			return rootRefNode;
		}
	}
}
