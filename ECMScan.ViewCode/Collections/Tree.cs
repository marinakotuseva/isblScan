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
        /// Gets or sets Признак того, что узел явялется конечным
        /// </summary>
        public bool Flag { get; set; }
	
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
        /// Gets or sets Количество совпадений
        /// </summary>
        public int? MatchCount { get; set; }


        int? _ChildMathCount;
        /// <summary>
        /// Gets or sets Количество совпадений с учётом дочерних узлов
        /// </summary>
        public int? ChildMathCount
        {
            get
            {
                return _ChildMathCount;
            }

            set
            {
                if (value.HasValue)
                {
                    if (_ChildMathCount.HasValue)
                    {
                        if (Parent.ChildMathCount.HasValue)
                        {
                            int distance = value.Value - _ChildMathCount.Value;
                            Parent.ChildMathCount += distance;
                        }
                        else
                        {
                            Parent.ChildMathCount = value;
                        }
                        _ChildMathCount = value;
                    }
                    else
                    {
                        if (Parent.ChildMathCount.HasValue)
                        {
                            int distance = value.Value;
                            Parent.ChildMathCount += distance;
                        }
                        else
                        {
                            Parent.ChildMathCount = value;
                        }
                        _ChildMathCount = value;
                    }
                }
                else
                {
                    _ChildMathCount = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Признак того, что узел содержит подузлы, которые соответствуют поисковому запросу
        /// </summary>
        public bool IsContainsMatchedNode { get; set; }

        /// <summary>
        /// Gets or sets Дата последней модификации узла разработки (ISBL)
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// Gets or sets Список родительского узла
        /// </summary>
        public Node Parent { get; set; }

        public int ChildCount
        {
            get
            {
                int count = 0;
                if (Nodes != null)
                {
                    count = Nodes.Count;
                    foreach(Node childrenNode in Nodes)
                    {
                        count += childrenNode.ChildCount;
                    }
                }
                return count;
            }
        }

	    public Node()
	    {
			    Visible = true;
			    Flag = true;
	    }

        public string TimeStamp { get; internal set; }

        string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffffzzz");
        }

        public object Tag { get; set; }
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
