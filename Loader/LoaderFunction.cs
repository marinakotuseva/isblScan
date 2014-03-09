/*
 * Date: 10.08.2012
 * Time: 21:22
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace isblTest
{
	/// <summary>
	/// Функция.
	/// </summary>
	public class Function : LoaderCommon
	{
		public Function(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private List<isblTest.Node> LoadGroups(isblTest.Node rootNode, char charSysFunc)
		{
			List<isblTest.Node> listGroups = new List<Node>();
			if(this.checkTableExist("MBRegUnit"))
			{
				SqlCommand command = new SqlCommand();
				command.Connection = this.connection;
				//command.CommandText = "select t.id, t.name from (select MBGrFunc.NGroup [id], Max(MBGrFunc.GrName) [name] from MBGrFunc join MBFunc on (MBGrFunc.NGroup = MBFunc.NGroup) where MBFunc.SysFunc=@funcCategory and not(Txt is null) group by MBGrFunc.NGroup) t order by t.name";
				command.CommandText = "select t.id, t.name from (select MBGrFunc.NGroup [id], Max(MBGrFunc.GrName) [name] from MBGrFunc join MBFunc on (MBGrFunc.NGroup = MBFunc.NGroup) where MBFunc.SysFunc=@funcCategory group by MBGrFunc.NGroup) t order by t.name";
				SqlParameter paramFuncCategory = new SqlParameter("@funcCategory", SqlDbType.Char, 1);
				paramFuncCategory.Value = charSysFunc;
				command.Parameters.Add(paramFuncCategory);
				command.Prepare();
				SqlDataReader reader = command.ExecuteReader();
				if(reader.HasRows)
				{
					while(reader.Read())
					{
						isblTest.Node node = new isblTest.Node();
						node.Parent = rootNode;
						node.Id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.Name = reader.GetString(1);
						}
						node.Nodes = new List<isblTest.Node>();
						rootNode.Nodes.Add(node);
						listGroups.Add(node);
					}
				}
				reader.Close();
			}
			return listGroups;
		}

		private string GetTypeName(string typeCode)
		{
			string typeName = "";
			switch (typeCode.ToUpper())
			{
				case "V":
					typeName = "Вариантный";
					break;
				case "Д":
					typeName = "Дата";
					break;
				case "Ч":
					typeName = "Дробное число";
					break;
				case "I":
					typeName = "Интерфейс";
					break;
				case "К":
					typeName = "Константа";
					break;
				case "L":
					typeName = "Логический";
					break;
				case "У":
					typeName = "Реквизит справочника";
					break;
				case "А":
					typeName = "Справочник";
					break;
				case "С":
					typeName = "Строка";
					break;
				case "M":
					typeName = "Тип справочника";
					break;
				case "Ц":
					typeName = "Целое число";
					break;
				default:
					typeName = "Неизвестный тип (" + typeCode + ")";
					break;
			}
			return typeName;
		}

		/// <summary>
		/// Загрузка параметров (реквизитов) функции
		/// </summary>
		/// <param name="rootNode">
		/// Узел дерева элементов, соотвествующий функции.
		/// A <see cref="isblTest.Node"/>
		/// </param>
		private void LoadRecvisites (isblTest.Node rootNode)
		{
			if (this.checkTableExist ("MBFuncRecv"))
			{
				SqlCommand command = new SqlCommand ();
				command.Connection = this.connection;
				command.CommandText = "SELECT [NumPar], [Ident], [Name], [Type], [ValueDef] FROM [MBFuncRecv] WHERE [FName] = @funcName";
				SqlParameter paramFuncName = new SqlParameter ("@funcName", SqlDbType.VarChar, 512);
				paramFuncName.Value = rootNode.Name;
				command.Parameters.Add (paramFuncName);
				command.Prepare ();
				SqlDataReader reader = command.ExecuteReader ();
				if (reader.HasRows)
				{
					isblTest.Node funcRecvNode = new isblTest.Node ();
					funcRecvNode.Name = "-=[ Параметры функции ]=-";
					funcRecvNode.Parent = rootNode;
					rootNode.Nodes.Add (funcRecvNode);
					while (reader.Read ())
					{
						//Номер параметра функции
						funcRecvNode.Text += reader.GetInt32 (0).ToString () + ".\t";
						//Идентификатор параметра
						if (!reader.IsDBNull (1))
						{
							funcRecvNode.Text += reader.GetString (1) + ".\t";
						}
						//Для внутреннего использования
						if (!reader.IsDBNull (2))
						{
							funcRecvNode.Text += reader.GetString (2) + ".\t";
						}
						//Тип параметра
						if (!reader.IsDBNull (3))
						{
							funcRecvNode.Text += this.GetTypeName(reader.GetString(3)) + ".\t";
						}
						funcRecvNode.Text += "\r\n";
					}
				}
				reader.Close();
			}
		}
		public isblTest.Node Load()
		{
			isblTest.Node listNode = null;
			if(this.checkTableExist("MBFunc"))
			{
				listNode = new isblTest.Node();
				listNode.Name = "Функция";
				listNode.Text = null;
				listNode.Parent = null;
				listNode.Nodes = new List<Node>();
				char[] charsSysUserFunc = {'P', 'S'};
				foreach(char charSysFunc in charsSysUserFunc)
				{
					isblTest.Node systemFuncNode = new isblTest.Node();
					if(charSysFunc == 'P')
					{
						systemFuncNode.Name = "Пользовательская";
					}
					else
					{
						systemFuncNode.Name = "Системная";
					}
					systemFuncNode.Text = null;
					systemFuncNode.Parent = listNode;
					systemFuncNode.Nodes = new List<Node>();
					listNode.Nodes.Add(systemFuncNode);
					
					List<isblTest.Node> listGroups = LoadGroups(systemFuncNode, charSysFunc);
					foreach(isblTest.Node groupNode in listGroups)
					{
						SqlCommand command = new SqlCommand();
						command.Connection = connection;
						//command.CommandText = "select XRecID, FName, Comment, Help, Txt from MBFunc where NGroup=@groupID and SysFunc=@sysFunc and not(Txt is null) order by FName";
						command.CommandText = "select XRecID, FName, Comment, Help, Txt from MBFunc where NGroup=@groupID and SysFunc=@sysFunc order by FName";
						SqlParameter paramGroupID = new SqlParameter("@groupID", SqlDbType.Int, 10);
						SqlParameter paramSysFunc = new SqlParameter("@sysFunc", SqlDbType.Char, 1);
						paramGroupID.Value = groupNode.Id;
						paramSysFunc.Value = charSysFunc;
						command.Parameters.Add(paramGroupID);
						command.Parameters.Add(paramSysFunc);
						command.Prepare();
						SqlDataReader reader = command.ExecuteReader();
						if(reader.HasRows)
						{
							while(reader.Read())
							{
								isblTest.Node functionNode = new isblTest.Node();
								functionNode.Nodes = new List<isblTest.Node>();
								functionNode.Parent = groupNode;
								//ИД
								functionNode.Id = reader.GetInt32(0);
								//Имя функции
								if(! reader.IsDBNull(1))
								{
									functionNode.Name = reader.GetString(1);
								}
								//Комментарий к функции
								if(! reader.IsDBNull(2))
								{
									isblTest.Node funcDescriptionNode = new isblTest.Node();
									funcDescriptionNode.Name = "-=[ Описание функции ]=-";
									funcDescriptionNode.Text = reader.GetString(2);
									funcDescriptionNode.Parent = functionNode;
									functionNode.Nodes.Add(funcDescriptionNode);
								}
								//Справка по функции
								/*
								if(! reader.IsDBNull(3))
								{
									isblTest.Node funcHelpNode = new isblTest.Node();
									funcHelpNode.name = "Справка по функции";
									funcHelpNode.text = reader.GetString(3);;
									funcHelpNode.parent = functionNode;
									functionNode.nodes.Add(funcHelpNode);
								}
								*/
								//Текст функции
								if(! reader.IsDBNull(4))
								{
									isblTest.Node funcTextNode = new isblTest.Node();
									funcTextNode.Name = "-=[ Текст функции ]=-";
									funcTextNode.Text = reader.GetString(4);
									funcTextNode.Parent = functionNode;
									functionNode.Nodes.Add(funcTextNode);
								}
								
								groupNode.Nodes.Add(functionNode);
							}
						}
						reader.Close();
						//Загрузка параметров функций для функций текущей группы
						foreach(isblTest.Node funcNode in groupNode.Nodes)
						{
							this.LoadRecvisites(funcNode);
						}
					}				

				}
			}
			return listNode;
		}

	}
}