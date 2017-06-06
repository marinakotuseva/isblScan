﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using Color = System.Drawing.Color;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

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
        public MainForm.SearchControls searchControls { get; set; }

        public bool RegExp
        {
            get { return regExp; }
            set
            {
                regExp = value;
                TextEditor.TextArea.TextView.Redraw();
            }
        }

        public bool CaseSensitive
        {
            get { return regExp; }
            set
            {
                caseSensitive = value;
                TextEditor.TextArea.TextView.Redraw();
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
            SearchCriteriaTextEditor.TextArea.KeyUp += new System.Windows.Input.KeyEventHandler(SearchCriteriaTextArea_KeyUp);
            SearchCriteriaTextEditor.TextArea.TextView.LineTransformers.Add(new HighlightIncorrectRegExp(this));

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
            TextEditor.TextArea.TextView.LineTransformers.Add(new HighlightSearchedStrings(this));

            var syntaxHighlightingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ISBL.xshd");
            using (Stream s = File.OpenRead(syntaxHighlightingFilePath))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        public void SearchCriteriaTextArea_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                Process();
                FillTreeView(searchControls.TreeViewResults);
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
        public class HighlightSearchedStrings : DocumentColorizingTransformer
        {
            private Search Search { get; set; }
            public HighlightSearchedStrings(Search search)
            {
                Search = search;
            }
            protected override void ColorizeLine(DocumentLine line)
            {
                if (Search._searchStrs.Length > 0)
                {
                    int lineStartOffset = line.Offset;
                    string text = CurrentContext.Document.GetText(line);
                    Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 156, 255, 156));

                    if (!Search.regExp)
                    {
                        StringComparison comparation = Search.caseSensitive
                            ? StringComparison.CurrentCulture
                            : StringComparison.CurrentCultureIgnoreCase;
                        
                        int start;
                        int index;
                        foreach (var word in Search._searchStrs)
                        {
                            start = 0;
                            while ((index = text.IndexOf(word, start, comparation)) >= 0)
                            {
                                base.ChangeLinePart(
                                    lineStartOffset + index, // startOffset
                                    lineStartOffset + index + word.Length, // endOffset
                                    (VisualLineElement element) =>
                                    {
                                        element.BackgroundBrush = brush;
                                    });
                                start = index + 1; // search for next occurrence
                            }
                        }
                    }
                    else
                    {
                        RegexOptions regExpOptions = Search.caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                        for (int indexRegExpStrings = 0; indexRegExpStrings < Search._searchStrs.Length; indexRegExpStrings++)
                        {
                            string hlStr = Search._searchStrs[indexRegExpStrings];
                            if (hlStr != "")
                            {
                                var regEx = new Regex(hlStr, regExpOptions);
                                var regExpFindResults = regEx.Matches(text);

                                foreach (Match match in regExpFindResults)
                                {
                                    base.ChangeLinePart(
                                        lineStartOffset + match.Index, // startOffset
                                        lineStartOffset + match.Index + match.Length, // endOffset
                                        (VisualLineElement element) =>
                                        {
                                            element.BackgroundBrush = brush;
                                        });
                                }
                            }
                        }
                    }
                } 
            }
        }

        /// <summary>
        /// Подсветка строк поиска в текущем тексте разработки
        /// </summary>
        public class HighlightIncorrectRegExp : DocumentColorizingTransformer
        {
            private Search Search { get; set; }
            public HighlightIncorrectRegExp(Search search)
            {
                Search = search;
            }
            protected override void ColorizeLine(DocumentLine line)
            {
                int lineStartOffset = line.Offset;
                string text = CurrentContext.Document.GetText(line).Trim();
                Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));

                if (text != "")
                {
                    if (Search.regExp)
                    {
                        RegexOptions regExpOptions = Search.caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    
                        try
                        {
                            new Regex(text, regExpOptions);
                        }
                        catch (ArgumentException)
                        {
                            base.ChangeLinePart(
                                lineStartOffset, // startOffset
                                lineStartOffset + line.Length, // endOffset
                                (VisualLineElement element) =>
                                {
                                    element.BackgroundBrush = brush;
                                });
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

            TextEditor.TextArea.TextView.Redraw();
        }


        /// <summary>
        /// Проверка списка регулярных выражений на корректность и возврат списка строк корректных регулярных выражений.
        /// </summary>
        /// <returns></returns>
        string[] CheckRegExpFormat()
        {
            List<string> regExpResultList = new List<string>();
            bool errorExist = false;
            for (int indexRegExpCandidate = 1; indexRegExpCandidate <= SearchCriteriaTextEditor.Document.LineCount; indexRegExpCandidate++)
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
