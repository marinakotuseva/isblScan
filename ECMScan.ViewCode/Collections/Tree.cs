namespace ISBLScan.ViewCode
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
        /// Gets or sets a value indicating whether this <see cref="Node"/> is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets Список подузлов
        /// </summary>
        public List<Node> Nodes { get; set; }

        /// <summary>
        /// Gets or sets Признак того, что узел соотвествует поисковому запросу
        /// </summary>
        public bool IsMatch { get; set; }

        /// <summary>
        /// Gets or sets Признак того, что узел содержит подузлы, которые соответствуют поисковому запросу
        /// </summary>
        public bool IsContainsMatchedNode { get; set; }

        /// <summary>
        /// Gets or sets Дата последней модификации узла разработки (ISBL)
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        public Node()
        {
            Visible = true;
        }

        public Node Clone()
        {
            var Node = new Node();
            Node.Name = this.Name;
            Node.IsContainsMatchedNode = this.IsContainsMatchedNode;
            Node.IsMatch = this.IsMatch;
            Node.Text = this.Text;
            Node.Id = this.Id;
            Node.Code = this.Code;
            return Node;
        }

        public object Tag { get; set; }
    }
}
