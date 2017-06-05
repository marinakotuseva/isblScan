using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Color = System.Drawing.Color;

namespace ISBLScan.ViewCode
{
    public class SearchNode
    {
        /// <summary>
        /// Gets or sets Имя узла, отображаемое в дереве
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Элемент разработки
        /// </summary>
        public IsbNode IsbNode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SearchNode"/> is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets Признак того, что узел соотвествует поисковому запросу
        /// </summary>
        public bool IsMatch { get; set; }

        /// <summary>
        /// Gets or sets Список подузлов
        /// </summary>
        public List<SearchNode> Nodes { get; set; } = new List<SearchNode>();

        public SearchNode(IsbNode isbNode)
        {
            Name = isbNode.Name;
            IsbNode = isbNode;
            Visible = isbNode.IsMatch || isbNode.IsContainsMatchedNode;
            IsMatch = isbNode.IsMatch;
        }
    }

    public class Search : IDisposable
    {
        public IsbDev IsbDev { get; set; }
        public List<SearchNode> Nodes { get; set; } = new List<SearchNode>();
        public ICSharpCode.AvalonEdit.TextEditor SearchCriteriaTextEditor { get; set; }
        public ICSharpCode.AvalonEdit.TextEditor TextEditor { get; set; }
        private bool regExp { get; set; }
        private bool caseSensitive { get; set; }

        public bool RegExp
        {
            get { return regExp; }
            set
            {
                regExp = value;
                MarkSearchStrings();
            }
        }

        public bool CaseSensitive
        {
            get { return regExp; }
            set
            {
                caseSensitive = value;
                MarkSearchStrings();
            }
        }

        public bool FindAll { get; set; }

        string[] _searchStrs = new string[] { };

        public Search(IsbDev sourceDev)
        {
            IsbDev = sourceDev;
            BuildSearchNodes(true);

            SearchCriteriaTextEditor = new ICSharpCode.AvalonEdit.TextEditor
            {
                Options =
                {
                    ShowEndOfLine = true,
                    ShowSpaces = true,
                    ShowTabs = true
                }
            };

            SearchCriteriaTextEditor.TextChanged += SearchCriteriaChanged;

            TextEditor = new ICSharpCode.AvalonEdit.TextEditor
            {
                Options =
                {
                    ConvertTabsToSpaces = true,
                    ShowEndOfLine = true,
                    ShowSpaces = true,
                    ShowTabs = true,
                    AllowScrollBelowDocument = false,
                    EnableRectangularSelection = true
                },
                ShowLineNumbers = true,
                Text = "",
                IsReadOnly = true
            };

            var syntaxHighlightingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ISBL.xshd");
            using (Stream s = File.OpenRead(syntaxHighlightingFilePath))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        public void Process()
        {
            if (_searchStrs.Length > 0)
            {
                IsbDev.Search(_searchStrs, caseSensitive, regExp, FindAll);
                BuildSearchNodes();
            }
            else
            {
                BuildSearchNodes(true);
            }
            System.Diagnostics.Debug.WriteLine(Nodes.Count);
            GC.Collect();
        }

        /// <summary>
        /// Подсветка строк поиска в текущем тексте разработки
        /// </summary>
        public void MarkSearchStrings()
        {
            if (regExp)
            {
                MarkSearchStringsRegExp(_searchStrs);
            }
            else
            {
                MarkSearchStrings(_searchStrs);
            }
            SearchCriteriaTextEditor.TextArea.TextView.Redraw();
        }

        public void MarkSearchStringsRegExp(string[] regExpArray)
        {
            RegexOptions regExpOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

            //Подсветка искомого текста
            if (regExpArray.Length > 0)
            {
                var text = TextEditor.Text;

                for (int indexRegExpStrings = 0; indexRegExpStrings < regExpArray.Length; indexRegExpStrings++)
                {
                    string hlStr = regExpArray[indexRegExpStrings];
                    if (hlStr != "")
                    {
                        var regEx = new Regex(hlStr, regExpOptions);
                        var regExpFindResults = regEx.Matches(text);

                        foreach (var match in regExpFindResults)
                        {
                            //TextMarker marker = new TextMarker(
                            //    match.Index
                            //    , match.Length
                            //    , TextMarkerType.SolidBlock
                            //    , Color.FromArgb(255, 156, 255, 156) // светло-зелёный
                            //    , Color.FromArgb(255, 18, 10, 143) // ультрамарин
                            //    );
                            //marker.ToolTip = hlStr;
                            //TextEditor.Document.MarkerStrategy.AddMarker(marker);

                            //Bookmark mark = new Bookmark(
                            //    TextEditor.Document
                            //    , TextEditor.Document.OffsetToPosition(match.Index)
                            //    , false);
                            //TextEditor.Document.BookmarkManager.AddMark(mark);

                            //if (!isCentered)
                            //{
                            //    var lineNumber = TextEditor.Document.GetLineNumberForOffset(match.Index);
                            //    isCentered = true;
                            //}
                        }
                    }
                }
            }

        }

        public void MarkSearchStrings(string[] findArray)
        {
            StringComparison comparation = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

            //Подсветка искомого текста
            if (findArray.Length > 0)
            {
                String text = TextEditor.Text;

                foreach (string findStr in findArray)
                {
                    //Подстветка строк

                    char[] charsDelimeters = { '\n', '\t', ' ', '\r' };
                    string[] strs = findStr.Split(charsDelimeters, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string str in strs)
                    {
                        string hlStr = str.Trim();
                        if (hlStr != "")
                        {
                            var posEnd = 0;
                            var posStart = text.IndexOf(hlStr, 0, comparation);
                            while (posStart >= 0)
                            {
                                posEnd = posStart + hlStr.Length - 1;
                                if (posEnd >= 0)
                                {
                                    //TextMarker marker = new TextMarker(
                                    //    posStart
                                    //    , posEnd - posStart + 1
                                    //    , TextMarkerType.SolidBlock
                                    //    , Color.FromArgb(255, 156, 255, 156) // светло-зелёный
                                    //    , Color.FromArgb(255, 18, 10, 143) // ультрамарин
                                    //    );
                                    //marker.ToolTip = hlStr;
                                    //TextEditor.Document.MarkerStrategy.AddMarker(marker);

                                    //Bookmark mark = new Bookmark(
                                    //    TextEditor.Document
                                    //    , TextEditor.Document.OffsetToPosition(posStart)
                                    //    , false);
                                    //TextEditor.Document.BookmarkManager.AddMark(mark);

                                    //if (!isCentered)
                                    //{
                                    //    var lineNumber = TextEditor.Document.GetLineNumberForOffset(posStart);
                                    //    TextEditor.ActiveTextAreaControl.CenterViewOn(lineNumber, 0);
                                    //    isCentered = true;
                                    //}

                                    posStart = text.IndexOf(hlStr, posEnd + 1, comparation);
                                }
                                else
                                {
                                    posStart = -1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void FillTreeView(TreeView treeViewResults, string filter = null)
        {
            FilterNodesByName(filter);
            treeViewResults.Nodes.Clear();
            treeViewResults.BeginUpdate();
            CopySearchNodesToTreeNodes(Nodes, treeViewResults.Nodes);
            treeViewResults.EndUpdate();
        }

        void FilterNodesByName(string filter)
        {
            foreach (SearchNode node in Nodes)
            {
                FilterNodeByName(node, filter);
            }
        }

        void SetVisible(SearchNode node)
        {
            node.Visible = true;
            foreach (SearchNode childNode in node.Nodes)
            {
                SetVisible(childNode);
            }
        }

        bool FilterNodeByName(SearchNode node, string filter)
        {
            if (node == null)
                return false;

            bool isFound = false;
            //Пропуск пустых поисковых строк
            if (string.IsNullOrEmpty(filter))
            {
                SetVisible(node);
                isFound = true;
            }
            else
            {
                if (node.Name.ToUpper().Contains(filter.ToUpper()))
                {
                    SetVisible(node);
                    isFound = true;
                }
                else
                {
                    foreach (SearchNode subNode in node.Nodes)
                    {
                        if (FilterNodeByName(subNode, filter))
                        {
                            isFound = true;
                        }
                    }
                }
                node.Visible = isFound;
            }

            return isFound;
        }

        void CopySearchNodesToTreeNodes(List<SearchNode> searchNodes, TreeNodeCollection treeNodes)
        {
            foreach (SearchNode node in searchNodes)
            {
                if (node.Visible)
                {
                    TreeNode treeNode = treeNodes.Add(node.Name);
                    treeNode.Tag = node.IsbNode;

                    if (node.IsMatch)
                    {
                        treeNode.ForeColor = Color.Black;
                    }
                    else
                    {
                        treeNode.ForeColor = Color.Gray;
                    }

                    CopySearchNodesToTreeNodes(node.Nodes, treeNode.Nodes);
                }
            }
        }

        /// <summary>
        /// Возврат всех слов из текста реактора поискового запроса, 
        /// разделителями слов в тексте выступают пробельные символы.
        /// Выделение слов, удовлетворяющих криетериям поиска.
        /// </summary>
        /// <returns></returns>
        void SearchCriteriaChanged(object sender, EventArgs e)
        {
            if (regExp)
            {
                _searchStrs = CheckRegExpFormat();
            }
            else
            {
                var searchLineStrs = SearchCriteriaTextEditor.Text.Split(
                new string[] { "\n" }
                , StringSplitOptions.RemoveEmptyEntries
                );
                char[] delimeters = { ' ', '\t', '\n', '\r' };
                var searchWords = new List<string>();
                foreach (string searchLine in searchLineStrs)
                {
                    foreach (string searchWord in searchLine.Split(delimeters, StringSplitOptions.RemoveEmptyEntries))
                    {
                        searchWords.Add(searchWord.Trim());
                    }
                }
                _searchStrs = searchWords.ToArray();
            }

            MarkSearchStrings();
        }


        /// <summary>
        /// Проверка списка регулярных выражений на корректность и возврат списка строк корректных регулярных выражений.
        /// </summary>
        /// <returns></returns>
        string[] CheckRegExpFormat()
        {
            List<string> regExpResultList = new List<string>();
            bool errorExist = false;
            for (int indexRegExpCandidate = 0; indexRegExpCandidate < SearchCriteriaTextEditor.Document.LineCount; indexRegExpCandidate++)
            {
                var line = SearchCriteriaTextEditor.Document.GetLineByNumber(indexRegExpCandidate);
                ISegment segment = new TextSegment { StartOffset = line.Offset, EndOffset = line.EndOffset };
                var regExpCandidateString = SearchCriteriaTextEditor.Document.GetText(segment);
                try
                {
                    if (!string.IsNullOrEmpty(regExpCandidateString.Trim()))
                    {
                        new Regex(regExpCandidateString, RegexOptions.Compiled);
                        regExpResultList.Add(regExpCandidateString);
                    }
                }
                catch (ArgumentException)
                {
                    // TODO Highlight incorrect reg expressions
                    errorExist = true;
                }
            }
            if (errorExist)
            {
                SearchCriteriaTextEditor.TextArea.TextView.Redraw();
            }
            return regExpResultList.ToArray();
        }

            

        void BuildSearchNodes(bool copyAll = false)
        {
            Nodes.Clear();
            CopyMatchedIsblNodes(Nodes, IsbDev.Nodes, copyAll);
        }

        void CopyMatchedIsblNodes(List<SearchNode> targetNodes, List<IsbNode> sourceNodes, bool copyAll)
        {
            foreach (var isblNode in sourceNodes)
            {
                if (isblNode.IsMatch || isblNode.IsContainsMatchedNode || copyAll)
                {
                    var searchNode = new SearchNode(isblNode);
                    if (copyAll) searchNode.Visible = true;
                    if (isblNode.Nodes != null)
                    {
                        searchNode.Nodes = new List<SearchNode>();
                        CopyMatchedIsblNodes(searchNode.Nodes, isblNode.Nodes, copyAll);
                    }
                    targetNodes.Add(searchNode);
                }
            }
        }

        public void Dispose()
        {
            Nodes = null;
            IsbDev = null;
            _searchStrs = null;
            TextEditor = null;
            SearchCriteriaTextEditor = null;
        }
    }
}
