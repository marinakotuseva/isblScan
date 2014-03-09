namespace isblTest
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Узел дерева элементов разработки
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Gets or sets ИД записи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Код записи, группы, вида, ...
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets Имя узла, отображаемое в дереве
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Текст отображаемый при выборе узла
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets Признак того, что узел явялется конечным
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// Gets or sets Список подузлов
        /// </summary>
        public List<Node> Nodes { get; set; }

        /// <summary>
        /// Gets or sets Список родительского узла
        /// </summary>
        public Node Parent { get; set; }
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
