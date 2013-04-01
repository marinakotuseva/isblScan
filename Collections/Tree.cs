/*
 * Author: Smirnov Vjacheslav
 * Email: owasp@ya.ru
 * Date: 11.08.2012
 * Time: 0:44
 */
using System;
using System.Collections.Generic;

namespace isblTest
{
	public class Node
	{
		/// <summary>
		///ИД записи, группы, вида, ...
		/// </summary>
		public int id;
		/// <summary>
		///Код записи, группы, вида, ...
		/// </summary>
		public string code;
		/// <summary>
		///Имя узла, отображаемое в дереве
		/// </summary>
		public string name;
		/// <summary>
		///Текст отображаемый при выборе узла
		/// </summary>
		public string text;
		/// <summary>
		///Признак того, что узел явялется конечным
		/// </summary>
		public bool flag;
		/// <summary>
		///Список подузлов
		/// </summary>
		public List<Node> nodes;
		/// <summary>
		///Список родительского узла
		/// </summary>
		public Node parent;
		
	}
	/// <summary>
	/// Description of Tree.
	/// </summary>
	public class Tree
	{
		public Tree()
		{
			
		}
	}
}
