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
						node.parent = rootNode;
						node.id = reader.GetInt32(0);
						if(! reader.IsDBNull(1))
						{
							node.name = reader.GetString(1);
						}
						node.nodes = new List<isblTest.Node>();
						rootNode.nodes.Add(node);
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
				paramFuncName.Value = rootNode.name;
				command.Parameters.Add (paramFuncName);
				command.Prepare ();
				SqlDataReader reader = command.ExecuteReader ();
				if (reader.HasRows)
				{
					isblTest.Node funcRecvNode = new isblTest.Node ();
					funcRecvNode.name = "-=[ Параметры функции ]=-";
					funcRecvNode.parent = rootNode;
					rootNode.nodes.Add (funcRecvNode);
					while (reader.Read ())
					{
						//Номер параметра функции
						funcRecvNode.text += reader.GetInt32 (0).ToString () + ".\t";
						//Идентификатор параметра
						if (!reader.IsDBNull (1))
						{
							funcRecvNode.text += reader.GetString (1) + ".\t";
						}
						//Для внутреннего использования
						if (!reader.IsDBNull (2))
						{
							funcRecvNode.text += reader.GetString (2) + ".\t";
						}
						//Тип параметра
						if (!reader.IsDBNull (3))
						{
							funcRecvNode.text += this.GetTypeName(reader.GetString(3)) + ".\t";
						}
						
					}
				}
			}
		}
		public isblTest.Node Load()
		{
			isblTest.Node listNode = null;
			if(this.checkTableExist("MBFunc"))
			{
				listNode = new isblTest.Node();
				listNode.name = "Функция";
				listNode.text = null;
				listNode.parent = null;
				listNode.nodes = new List<Node>();
				char[] charsSysUserFunc = {'P', 'S'};
				foreach(char charSysFunc in charsSysUserFunc)
				{
					isblTest.Node systemFuncNode = new isblTest.Node();
					if(charSysFunc == 'P')
					{
						systemFuncNode.name = "Пользовательская";
					}
					else
					{
						systemFuncNode.name = "Системная";
					}
					systemFuncNode.text = null;
					systemFuncNode.parent = listNode;
					systemFuncNode.nodes = new List<Node>();
					listNode.nodes.Add(systemFuncNode);
					
					List<isblTest.Node> listGroups = LoadGroups(systemFuncNode, charSysFunc);
					foreach(isblTest.Node groupNode in listGroups)
					{
						SqlCommand command = new SqlCommand();
						command.Connection = connection;
						//command.CommandText = "select XRecID, FName, Comment, Help, Txt from MBFunc where NGroup=@groupID and SysFunc=@sysFunc and not(Txt is null) order by FName";
						command.CommandText = "select XRecID, FName, Comment, Help, Txt from MBFunc where NGroup=@groupID and SysFunc=@sysFunc order by FName";
						SqlParameter paramGroupID = new SqlParameter("@groupID", SqlDbType.Int, 10);
						SqlParameter paramSysFunc = new SqlParameter("@sysFunc", SqlDbType.Char, 1);
						paramGroupID.Value = groupNode.id;
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
								functionNode.nodes = new List<isblTest.Node>();
								functionNode.parent = groupNode;
								//ИД
								functionNode.id = reader.GetInt32(0);
								//Имя функции
								if(! reader.IsDBNull(1))
								{
									functionNode.name = reader.GetString(1);
								}
								//Комментарий к функции
								if(! reader.IsDBNull(2))
								{
									isblTest.Node funcDescriptionNode = new isblTest.Node();
									funcDescriptionNode.name = "-=[ ОПИСАНИЕ ФУНКЦИИ ]=-";
									funcDescriptionNode.text = reader.GetString(2);
									funcDescriptionNode.parent = functionNode;
									functionNode.nodes.Add(funcDescriptionNode);
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
									funcTextNode.name = "-=[ ТЕКСТ ФУНКЦИИ ]=-";
									funcTextNode.text = reader.GetString(4);
									funcTextNode.parent = functionNode;
									functionNode.nodes.Add(funcTextNode);
								}
								
								groupNode.nodes.Add(functionNode);
							}
						}
						reader.Close();
					}				

				}
			}
			return listNode;
		}

	}
}