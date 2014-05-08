/*
 * Date: 10.08.2012
 * Time: 21:22
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Функция.
	/// </summary>
	public class Function : LoaderCommon
	{
		public Function (SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		
		private List<Node> LoadGroups (Node rootNode, char charSysFunc)
		{
			List<Node> listGroups = new List<Node> ();
			if (this.checkTableExist ("MBRegUnit")) 
			{
				SqlCommand command = new SqlCommand ();
				command.Connection = this.connection;
				//command.CommandText = "select t.id, t.name from (select MBGrFunc.NGroup [id], Max(MBGrFunc.GrName) [name] from MBGrFunc join MBFunc on (MBGrFunc.NGroup = MBFunc.NGroup) where MBFunc.SysFunc=@funcCategory and not(Txt is null) group by MBGrFunc.NGroup) t order by t.name";
				command.CommandText = @"
select t.id, t.name, t.Comment, t.LastUpd
from
(
    select MBGrFunc.NGroup [id], Max(MBGrFunc.GrName) [name], Max(MBGrFunc.LastUpd) [LastUpd], Max(CAST(MBGrFunc.Comment as NVARCHAR(MAX))) [Comment]
    from MBGrFunc 
    join MBFunc on (MBGrFunc.NGroup = MBFunc.NGroup) 
    where MBFunc.SysFunc=@funcCategory 
    group by MBGrFunc.NGroup
) t 
order by t.name";
				SqlParameter paramFuncCategory = new SqlParameter ("@funcCategory", SqlDbType.NChar, 1);
				paramFuncCategory.Value = charSysFunc;
				command.Parameters.Add (paramFuncCategory);
				command.Prepare ();
				SqlDataReader reader = command.ExecuteReader ();
				if (reader.HasRows) 
				{
					while (reader.Read()) 
					{
						Node node = new Node ();
						node.Parent = rootNode;
						//Номер группы функций
						node.Id = reader.GetInt32 (0);
						//Имя группы функций
						if (! reader.IsDBNull (1)) {
							node.Name = reader.GetString (1);
						}
						//Примечание
						if (!reader.IsDBNull (2)) {
							node.Text = reader.GetString (2);
						}
						//Дата и время последнего изменения
						if (!reader.IsDBNull (3)) {
							node.LastUpdate = reader.GetDateTime (3);
						}

						node.Nodes = new List<Node> ();
						rootNode.Nodes.Add (node);
						listGroups.Add (node);
					}
				}
				reader.Close ();
			}
			return listGroups;
		}

		private string GetTypeName (string typeCode)
		{
			string typeName = "";
			switch (typeCode.ToUpper ()) 
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
		/// A <see cref="Node"/>
		/// </param>
		private void LoadRecvisites (Node rootNode)
		{
			if (this.checkTableExist ("MBFuncRecv")) 
			{
				SqlCommand command = new SqlCommand ();
				command.Connection = this.connection;
				command.CommandText = "SELECT [NumPar], [Ident], [Name], [Type], [ValueDef] FROM [MBFuncRecv] WHERE [FName] = @funcName";
				SqlParameter paramFuncName = new SqlParameter ("@funcName", SqlDbType.NVarChar, 512);
				paramFuncName.Value = rootNode.Name;
				command.Parameters.Add (paramFuncName);
				command.Prepare ();
				SqlDataReader reader = command.ExecuteReader ();
				if (reader.HasRows) 
				{
					Node funcRecvNode = new Node ();
					funcRecvNode.Name = "-=[ Параметры функции ]=-";
					funcRecvNode.Parent = rootNode;
					rootNode.Nodes.Add (funcRecvNode);
					while (reader.Read ()) 
					{
						//Номер параметра функции
						funcRecvNode.Text += reader.GetInt32 (0).ToString () + ".\t";
						//Идентификатор параметра
						if (!reader.IsDBNull (1)) {
							funcRecvNode.Text += reader.GetString (1) + ".\t";
						}
						//Для внутреннего использования
						if (!reader.IsDBNull (2)) {
							funcRecvNode.Text += reader.GetString (2) + ".\t";
						}
						//Тип параметра
						if (!reader.IsDBNull (3)) {
							funcRecvNode.Text += this.GetTypeName (reader.GetString (3)) + ".\t";
						}
						funcRecvNode.Text += "\r\n";
					}
					funcRecvNode.LastUpdate = rootNode.LastUpdate;
				}
				reader.Close ();
			}
		}

		public Node Load ()
		{
			Node listNode = null;
			if (this.checkTableExist ("MBFunc")) {
				listNode = new Node ();
				listNode.Name = "Функция";
				listNode.Text = null;
				listNode.Parent = null;
				listNode.Nodes = new List<Node> ();
				char[] charsSysUserFunc = {'P', 'S'};
				foreach (char charSysFunc in charsSysUserFunc) 
				{
					Node systemFuncNode = new Node ();
					if (charSysFunc == 'P') {
						systemFuncNode.Name = "Пользовательская";
					} else {
						systemFuncNode.Name = "Системная";
					}
					systemFuncNode.Text = null;
					systemFuncNode.Parent = listNode;
					systemFuncNode.Nodes = new List<Node> ();
					listNode.Nodes.Add (systemFuncNode);
					
					List<Node> listGroups = LoadGroups (systemFuncNode, charSysFunc);
					foreach (Node groupNode in listGroups) 
					{
						SqlCommand command = new SqlCommand ();
						command.Connection = connection;
						//command.CommandText = "select XRecID, FName, Comment, Help, Txt from MBFunc where NGroup=@groupID and SysFunc=@sysFunc and not(Txt is null) order by FName";
						command.CommandText = "select XRecID, FName, Comment, Help, Txt, LastUpd from MBFunc where NGroup=@groupID and SysFunc=@sysFunc order by FName";
						SqlParameter paramGroupID = new SqlParameter ("@groupID", SqlDbType.Int);
						SqlParameter paramSysFunc = new SqlParameter ("@sysFunc", SqlDbType.NChar, 1);
						paramGroupID.Value = groupNode.Id;
						paramSysFunc.Value = charSysFunc;
						command.Parameters.Add (paramGroupID);
						command.Parameters.Add (paramSysFunc);
						command.Prepare ();
						SqlDataReader reader = command.ExecuteReader ();
						if (reader.HasRows) 
						{
							while (reader.Read()) 
							{
								Node functionNode = new Node ();
								functionNode.Nodes = new List<Node> ();
								functionNode.Parent = groupNode;
								//ИД
								functionNode.Id = reader.GetInt32 (0);
								//Имя функции
								if (! reader.IsDBNull (1)) {
									functionNode.Name = reader.GetString (1);
								}
								//Комментарий к функции
								if (! reader.IsDBNull (2)) {
									Node funcDescriptionNode = new Node ();
									funcDescriptionNode.Name = "-=[ Описание функции ]=-";
									funcDescriptionNode.Text = reader.GetString (2);
									funcDescriptionNode.Parent = functionNode;
									functionNode.Nodes.Add (funcDescriptionNode);
								}
								//Справка по функции
								/*
								if(! reader.IsDBNull(3))
								{
									Node funcHelpNode = new Node();
									funcHelpNode.name = "Справка по функции";
									funcHelpNode.text = reader.GetString(3);;
									funcHelpNode.parent = functionNode;
									functionNode.nodes.Add(funcHelpNode);
								}
								*/
								//Текст функции
								if (! reader.IsDBNull (4)) {
									Node funcTextNode = new Node ();
									funcTextNode.Name = "-=[ Текст функции ]=-";
									funcTextNode.Text = reader.GetString (4);
									funcTextNode.Parent = functionNode;
									functionNode.Nodes.Add (funcTextNode);
								}
								//Дата и время последнего изменения
								if (!reader.IsDBNull (5)) {
									DateTime lastUpdate = reader.GetDateTime (5);
									functionNode.LastUpdate = lastUpdate;
									foreach (Node functionSubNode in functionNode.Nodes) {
										functionSubNode.LastUpdate = lastUpdate;
									}
								}
								groupNode.Nodes.Add (functionNode);
							}
						}
						reader.Close ();
						//Загрузка параметров функций для функций текущей группы
						foreach (Node funcNode in groupNode.Nodes) {
							this.LoadRecvisites (funcNode);
						}
					}				

				}
			}
			return listNode;
		}

	}
}