/*
 * Date: 10.08.2012
 * Time: 21:23
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBLScan.ViewCode
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
		/// A <see cref="Node"/>
		/// </param>
		void LoadRecvisite(Node recvGroupNode)
		{
			if(this.CheckTableExist("MBVidAnRecv"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.Connection;
				command.CommandText = "SELECT [Name], [Kod], [Exprn], [InpExprn] FROM [MBVidAnRecv] WHERE [Vid]=@Vid AND [Razd]=@Razd ORDER BY [NumRecv]";
				SqlParameter paramVid = new SqlParameter("@Vid", SqlDbType.Int);
				paramVid.Value = recvGroupNode.Id;
				SqlParameter paramRazd = new SqlParameter("@Razd", SqlDbType.NChar, 1);
				paramRazd.Value = recvGroupNode.Code;
				command.Parameters.Add(paramVid);
				command.Parameters.Add(paramRazd);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						Node recvNode = new Node();
						recvNode.Nodes = new List<Node>();
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
							Node exprnRefRecvNode = new Node();
							exprnRefRecvNode.Name = "-=[ Вычисление ]=-";
							exprnRefRecvNode.Text = reader.GetString(2);
							recvNode.Nodes.Add(exprnRefRecvNode);
						}
						if(!reader.IsDBNull(3))
						{
							Node inpExprnRefRecvNode = new Node();
							inpExprnRefRecvNode.Name = "-=[ Выбор из справочника ]=-";
							inpExprnRefRecvNode.Text = reader.GetString(3);
							recvNode.Nodes.Add(inpExprnRefRecvNode);
						}
						//Добавить реквизит в дерево, если у него есть вычисления (подузлы)
						if(recvNode.Nodes.Count > 0)
						{
							recvGroupNode.Nodes.Add(recvNode);
						}
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
		/// A <see cref="Node"/>
		/// </param>
		void LoadGroupRecv(Node refNode)
		{
			if(this.CheckTableExist("MBVidAnRecv"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.Connection;
				command.CommandText = "SELECT [Razd] FROM [MBVidAnRecv] WHERE [Vid] = @Vid GROUP BY [Razd] ORDER BY [Razd] DESC";
				SqlParameter paramVid = new SqlParameter("@Vid", SqlDbType.Int);
				paramVid.Value = refNode.Id;
				command.Parameters.Add(paramVid);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();

				Node refRecvNode = new Node();
				refRecvNode.Nodes = new List<Node>();

				if(reader.HasRows)
				{
					refRecvNode.Name = "-=[ Реквизиты и действия типа справочника ]=-";
					refRecvNode.Text = null;
					refNode.Nodes.Add(refRecvNode);

					while(reader.Read())
					{
						Node recvGroupNode = new Node();
						//Код раздела
						recvGroupNode.Code = reader.GetString(0);
						//Вид аналитики
						recvGroupNode.Id = refNode.Id;
						recvGroupNode.Name = GetGroupRecvName(recvGroupNode.Code[0]);
						recvGroupNode.Nodes = new List<Node>();
						refRecvNode.Nodes.Add(recvGroupNode);
					}
				}
				reader.Close();
				foreach(Node recvGroupNode in refRecvNode.Nodes)
				{
					this.LoadRecvisite(recvGroupNode);
				}
			}
		}

		public Node Load()
		{
			Node rootRefNode = null;
			if(this.CheckTableExist("MBVidAn"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = Connection;
				command.CommandText = "select Vid, Name, Kod, Comment, Exprn from MBVidAn order by Name ASC";
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					rootRefNode = new Node();
					rootRefNode.Name = "Тип справочника";
					rootRefNode.Text = null;
					rootRefNode.Nodes = new List<Node>();
					
					while(reader.Read())
					{
						Node refNode = new Node();
						refNode.Nodes = new List<Node>();
						//ИД 
						refNode.Id = reader.GetInt32(0);
						//Имя (Код)
						if((! reader.IsDBNull(1))&&(! reader.IsDBNull(2)))
						{
							refNode.Name = reader.GetString(1).Trim() + " (" + reader.GetString(2).Trim() + ")";
						}
						refNode.Nodes = new List<Node>();
						//Примечание к типу справочника
						if(! reader.IsDBNull(3))
						{
							refNode.Text = reader.GetString(3).Trim();
						}
						//События типа справочника
						if(! reader.IsDBNull(4))
						{
							Node refEventNode = new Node();
							refEventNode.Name = "-=[ События ]=-";
							refEventNode.Text = reader.GetString(4).Trim();
							refNode.Nodes.Add(refEventNode);
						}
						rootRefNode.Nodes.Add(refNode);
					}
				}
				reader.Close();

				foreach(Node refNode in rootRefNode.Nodes)
				{
					LoadGroupRecv(refNode);
				}

			}
			return rootRefNode;
		}
	}
}
