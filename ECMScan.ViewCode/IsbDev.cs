using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Прикладная разработка
    /// </summary>
    public class IsbDev
    {
        /// <summary>
        /// Дерево элементов прикладной разработки
        /// </summary>
        public List<IsbNode> Nodes = new List<IsbNode>();

        public void Search(string[] searchStrArray, bool caseSensitive, bool regExp, bool findAll)
        {
            foreach (var node in Nodes)
            {
                FilterNode(node, searchStrArray, caseSensitive, regExp, findAll);
            }
        }

        /// <summary>
        /// Рекурсивный поиск по дереву разработки
        /// </summary>
        /// <param name="node"></param>
        /// <param name="searchStrArray"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="regExp"></param>
        /// <param name="findAll"></param>
        /// <returns></returns>
        bool FilterNode(IsbNode node, string[] searchStrArray, bool caseSensitive, bool regExp, bool findAll)
        {
            bool isFound = false;
            //Сначала выделим текущий элемент так, как будто в нём ничего не найдено
            node.IsMatch = false;
            node.IsContainsMatchedNode = false;
            if (node.Nodes != null)
            {
                foreach (var subNode in node.Nodes)
                {
                    if (FilterNode(subNode, searchStrArray, caseSensitive, regExp, findAll))
                    {
                        node.IsContainsMatchedNode = true;
                        isFound = true;
                    }
                }
            }
            string isblText = node.Text;
            string nodeName = node.Name;
            bool searchSatisfied = false;
            if (regExp)
            {
                RegexOptions regExpOptions = caseSensitive ? RegexOptions.Compiled : RegexOptions.Compiled | RegexOptions.IgnoreCase;
                foreach (string searchPhrase in searchStrArray)
                {
                    Regex regEx = new Regex(searchPhrase, regExpOptions);
                    if ((!string.IsNullOrEmpty(isblText) && regEx.IsMatch(isblText)) || regEx.IsMatch(nodeName))
                    {
                        searchSatisfied = true;
                        if (!findAll) break;
                    }
                    else
                    {
                        if (findAll)
                        {
                            searchSatisfied = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                var comparation = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

                foreach (string searchPhrase in searchStrArray)
                {
                    if (
                        (!string.IsNullOrEmpty(isblText) && isblText.IndexOf(searchPhrase, 0, comparation) >= 0) ||
                        (!string.IsNullOrEmpty(nodeName) && nodeName.IndexOf(searchPhrase, 0, comparation) >= 0)
                    )
                    {
                        searchSatisfied = true;
                        if (!findAll) break;
                    }
                    else
                    {
                        if (findAll)
                        {
                            searchSatisfied = false;
                            break;
                        }
                    }
                }
            }
            if (searchSatisfied)
            {
                node.IsMatch = true;
                isFound = true;
            }
            return isFound;
        }
    }

    /// <summary>
    /// Узел дерева элементов разработки
    /// </summary>
    public class IsbNode
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
        /// Gets or sets Список подузлов
        /// </summary>
        public List<IsbNode> Nodes { get; set; } = new List<IsbNode>();

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

        public IsbNode(string name)
        {
            Name = name;
        }
        public IsbNode()
        {
        }
    }
}
