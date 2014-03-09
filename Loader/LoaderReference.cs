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
				paramVid.Value = recvGroupNode.Id;
				SqlParameter paramRazd = new SqlParameter("@Razd", SqlDbType.Char, 1);
				paramRazd.Value = recvGroupNode.Code;
				command.Parameters.Add(paramVid);
				command.Parameters.Add(paramRazd);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						isblTest.Node recvNode = new isblTest.Node();
						recvNode.Parent = recvGroupNode;
						recvNode.Nodes = new List<isblTest.Node>();
						if(!reader.IsDBNull(0))
						{
							recvNode.Name = reader.GetString(0);
						}
						if(!reader.IsDBNull(1))
						{
							recvNode.Code = reader.GetString(1);
							recvNode.Name += " (" + recvNode.Code + ")";
						}
						if(!reader.IsDBNull(2))
						{
							isblTest.Node exprnRefRecvNode = new isblTest.Node();
							exprnRefRecvNode.Name = "-=[ Вычисление ]=-";
							exprnRefRecvNode.Text = reader.GetString(2);
							exprnRefRecvNode.Parent = recvNode;
							recvNode.Nodes.Add(exprnRefRecvNode);
						}
						if(!reader.IsDBNull(3))
						{
							isblTest.Node inpExprnRefRecvNode = new isblTest.Node();
							inpExprnRefRecvNode.Name = "-=[ Выбор из справочника ]=-";
							inpExprnRefRecvNode.Text = reader.GetString(3);
							inpExprnRefRecvNode.Parent = recvNode;
							recvNode.Nodes.Add(inpExprnRefRecvNode);
						}
						recvGroupNode.Nodes.Add(recvNode);
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
				paramVid.Value = refNode.Id;
				command.Parameters.Add(paramVid);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();

				isblTest.Node refRecvNode = new isblTest.Node();
				refRecvNode.Nodes = new List<isblTest.Node>();

				if(reader.HasRows)
				{
					refRecvNode.Name = "-=[ Реквизиты и действия типа справочника ]=-";
					refRecvNode.Text = null;
					refRecvNode.Parent = refNode;
					refNode.Nodes.Add(refRecvNode);

					while(reader.Read())
					{
						isblTest.Node recvGroupNode = new isblTest.Node();
						//Код раздела
						recvGroupNode.Code = reader.GetString(0);
						//Вид аналитики
						recvGroupNode.Id = refNode.Id;
						recvGroupNode.Name = GetGroupRecvName(recvGroupNode.Code[0]);
						recvGroupNode.Parent = refRecvNode;
						recvGroupNode.Nodes = new List<isblTest.Node>();
						refRecvNode.Nodes.Add(recvGroupNode);
					}
				}
				reader.Close();
				foreach(isblTest.Node recvGroupNode in refRecvNode.Nodes)
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
					rootRefNode.Name = "Тип справочника";
					rootRefNode.Text = null;
					rootRefNode.Parent = null;
					rootRefNode.Nodes = new List<Node>();
					
					while(reader.Read())
					{
						isblTest.Node refNode = new isblTest.Node();
						refNode.Parent = rootRefNode;
						refNode.Nodes = new List<isblTest.Node>();
						//ИД 
						refNode.Id = reader.GetInt32(0);
						//Имя (Код)
						if((! reader.IsDBNull(1))&&(! reader.IsDBNull(2)))
						{
							refNode.Name = reader.GetString(1).Trim() + " (" + reader.GetString(2).Trim() + ")";
						}
						refNode.Nodes = new List<isblTest.Node>();
						//Примечание к типу справочника
						if(! reader.IsDBNull(3))
						{
							refNode.Text = reader.GetString(3).Trim();
						}
						//События типа справочника
						if(! reader.IsDBNull(4))
						{
							isblTest.Node refEventNode = new isblTest.Node();
							refEventNode.Name = "-=[ События ]=-";
							refEventNode.Text = reader.GetString(4).Trim();
							refEventNode.Parent = refNode;
							refNode.Nodes.Add(refEventNode);
						}
						rootRefNode.Nodes.Add(refNode);
					}
				}
				reader.Close();

				foreach(isblTest.Node refNode in rootRefNode.Nodes)
				{
					LoadGroupRecv(refNode);
				}

			}
			return rootRefNode;
		}
	}
}
