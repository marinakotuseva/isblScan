using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using System.Drawing;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Color = System.Drawing.Color;

namespace ISBLScan.ViewCode
{
    public class SearchController
    {
        private MainForm _form;
        public Search ActiveSearch;
        public List<Node> SourceIsblNodes { get; set; } = new List<Node>();
        public SearchController(MainForm form)
        {
            this._form = form;
        }

        public Search AddSearch(string name)
        {
            var search = new Search(this, name);
            _form.AddTab(search.Tab);
            search.Activate();
            return search;
        }

        public class Search
        {
            private SearchController _controller;
            private MainForm Form { get { return _controller._form; } }
            private List<Node> IsblNodes { get; set; }
            public TabPage Tab { get; set; }
            public ICSharpCode.AvalonEdit.TextEditor SearchCriteriaTextEditor { get; set; }
            public ICSharpCode.AvalonEdit.TextEditor TextEditor { get; set; }
            public TreeView TreeViewResults { get; set; }
            public System.Windows.Forms.TextBox TextBoxFilter { get; set; }
            public string FilterText { get; set; }
            public bool CaseSensitive { get; set; }
            public bool RegExp { get; set; }
            public bool FindAll { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindRegExp { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindAll { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindCaseSensitive { get; set; }
            public System.Windows.Forms.Integration.ElementHost TextEditorHost { get; set; }
            public System.Windows.Forms.Integration.ElementHost SearchCriteriaTextEditorHost { get; set; }
            /// <summary>
            /// Словарь поисковых фраз и соотвествующих регулярных выражений
            /// </summary>
            Dictionary<string, Regex> _dictRegEx = new Dictionary<string, Regex>();

            string[] _searchStrs;
            public Search(SearchController controller, string name)
            {
                _controller = controller;
                Tab = new TabPage(name);
                IsblNodes = new List<Node>();
                CopyIsblNodes(IsblNodes, _controller.SourceIsblNodes); 
                
                TreeViewResults = new TreeView();

                SearchCriteriaTextEditorHost = new System.Windows.Forms.Integration.ElementHost();
                SearchCriteriaTextEditorHost.Dock = DockStyle.Fill;
                SearchCriteriaTextEditorHost.Location = new Point(0, 0);
                SearchCriteriaTextEditorHost.TabIndex = 1;

                SearchCriteriaTextEditor = new ICSharpCode.AvalonEdit.TextEditor();
                SearchCriteriaTextEditor.Options.ShowEndOfLine = true;
                SearchCriteriaTextEditor.Options.ShowSpaces = true;
                SearchCriteriaTextEditor.Options.ShowTabs = true; 
                SearchCriteriaTextEditor.TextChanged += new System.EventHandler(Form.textEditorControlRegExp_TextChanged);

                SearchCriteriaTextEditorHost.Child = SearchCriteriaTextEditor;
                Tab.Tag = this;
                Tab.Controls.Add(SearchCriteriaTextEditorHost);
                Tab.UseVisualStyleBackColor = true;
               
                TreeViewResults.CheckBoxes = false;
                TreeViewResults.Dock = DockStyle.Fill;
                TreeViewResults.HideSelection = false;
                TreeViewResults.Location = new Point(0, 0);
                TreeViewResults.Margin = new Padding(4);
                TreeViewResults.Name = "treeViewResults";
                TreeViewResults.TabIndex = 2121;
                TreeViewResults.AfterSelect += new TreeViewEventHandler(Form.TreeViewResultsAfterSelect);
                BuildDisplayTree();
                Form.AddTreeViewResults(TreeViewResults);

                TextEditorHost = new System.Windows.Forms.Integration.ElementHost();
                TextEditorHost.Dock = DockStyle.Fill;
                TextEditorHost.Location = new Point(0, 0);
                TextEditorHost.Margin = new System.Windows.Forms.Padding(4);
                TextEditorHost.TabIndex = 150;

                TextEditor = new ICSharpCode.AvalonEdit.TextEditor();
                TextEditor.Options.ConvertTabsToSpaces = true;
                TextEditor.Options.ShowEndOfLine = true;
                TextEditor.Options.ShowSpaces = true;
                TextEditor.Options.ShowTabs = true;
                TextEditor.Options.AllowScrollBelowDocument = false;
                TextEditor.Options.EnableRectangularSelection = true;
                TextEditor.ShowLineNumbers = true;
                TextEditor.Text = "";
                TextEditor.SyntaxHighlighting =
                    HighlightingManager.Instance.GetDefinition("ISBL");

                TextEditor.IsReadOnly = true;
                TextEditorHost.Child = TextEditor;
                Form.AddTextEditor(TextEditorHost);

                TextBoxFilter = new System.Windows.Forms.TextBox();
                TextBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
                TextBoxFilter.Location = new System.Drawing.Point(0, 0);
                TextBoxFilter.Margin = new System.Windows.Forms.Padding(4);
                TextBoxFilter.Name = "textBoxFilter";
                TextBoxFilter.Size = new System.Drawing.Size(280, 22);
                TextBoxFilter.TabIndex = 2111;
                TextBoxFilter.TextChanged += new System.EventHandler(Form.TextBoxFilter_TextChanged);
                Form.AddTreeFilter(TextBoxFilter);

                CheckBoxFindAll = new System.Windows.Forms.CheckBox();
                CheckBoxFindAll.AutoSize = true;
                CheckBoxFindAll.Dock = System.Windows.Forms.DockStyle.None;
                CheckBoxFindAll.Location = new System.Drawing.Point(320, 4);
                CheckBoxFindAll.Margin = new System.Windows.Forms.Padding(4);
                CheckBoxFindAll.Name = "checkBoxFindAll";
                CheckBoxFindAll.Size = new System.Drawing.Size(146, 28);
                CheckBoxFindAll.TabIndex = 1226;
                CheckBoxFindAll.Text = "satisfies all criteria";
                CheckBoxFindAll.UseVisualStyleBackColor = true;
                CheckBoxFindAll.CheckedChanged += new System.EventHandler(Form.checkBoxFindAll_CheckedChanged);
                Form.AddCheckBox(CheckBoxFindAll);

                CheckBoxFindRegExp = new System.Windows.Forms.CheckBox();
                CheckBoxFindRegExp.AutoSize = true;
                CheckBoxFindRegExp.Dock = System.Windows.Forms.DockStyle.None;
                CheckBoxFindRegExp.Location = new System.Drawing.Point(150, 4);
                CheckBoxFindRegExp.Margin = new System.Windows.Forms.Padding(4);
                CheckBoxFindRegExp.Name = "CheckBoxFindRegExp";
                CheckBoxFindRegExp.Size = new System.Drawing.Size(170, 28);
                CheckBoxFindRegExp.TabIndex = 1225;
                CheckBoxFindRegExp.Text = ".* (regular expression)";
                CheckBoxFindRegExp.UseVisualStyleBackColor = true;
                CheckBoxFindRegExp.CheckedChanged += new System.EventHandler(Form.checkBoxFindRegExp_CheckedChanged);
                Form.AddCheckBox(CheckBoxFindRegExp);

                CheckBoxFindCaseSensitive = new System.Windows.Forms.CheckBox();
                CheckBoxFindCaseSensitive.AutoSize = true;
                CheckBoxFindCaseSensitive.Dock = System.Windows.Forms.DockStyle.None;
                CheckBoxFindCaseSensitive.Location = new System.Drawing.Point(2, 4);
                CheckBoxFindCaseSensitive.Margin = new System.Windows.Forms.Padding(4);
                CheckBoxFindCaseSensitive.Name = "checkBoxFindCaseSensitive";
                CheckBoxFindCaseSensitive.Size = new System.Drawing.Size(150, 28);
                CheckBoxFindCaseSensitive.TabIndex = 1224;
                CheckBoxFindCaseSensitive.Text = "Aa (case sensitive)";
                CheckBoxFindCaseSensitive.UseVisualStyleBackColor = true;
                CheckBoxFindCaseSensitive.CheckedChanged += new System.EventHandler(Form.checkBoxFindCaseSensitive_CheckedChanged);
                Form.AddCheckBox(CheckBoxFindCaseSensitive);
            }

            public void Clear()
            {
                IsblNodes = null;
                Form.RemoveCheckBox(CheckBoxFindCaseSensitive);
                CheckBoxFindCaseSensitive.Dispose();
                CheckBoxFindCaseSensitive = null;
                Form.RemoveCheckBox(CheckBoxFindAll);
                CheckBoxFindAll.Dispose();
                CheckBoxFindAll = null;
                Form.RemoveCheckBox(CheckBoxFindRegExp);
                CheckBoxFindRegExp.Dispose();
                CheckBoxFindRegExp = null;
                Form.RemoveTreeView(TreeViewResults);
                TreeViewResults.Dispose();
                TreeViewResults = null;
                Tab.Controls.Remove(SearchCriteriaTextEditorHost);
                SearchCriteriaTextEditor = null;
                Form.RemoveTextEditor(TextEditorHost);
                TextEditor = null;
                Form.RemoveTreeFilter(TextBoxFilter);
                TextBoxFilter.Dispose();
                TextBoxFilter = null;
                Tab.Dispose();
                Tab = null;
                _dictRegEx = null;
                _searchStrs = null;
                GC.Collect();
            }

            public void Refresh()
            {
                IsblNodes.Clear();
                CopyIsblNodes(IsblNodes, _controller.SourceIsblNodes);
                BuildDisplayTree();
                GC.Collect();
            }

            public void Process()
            {
                GetSearchStrArray();
                foreach (Node node in _controller.SourceIsblNodes)
                {
                    FilterNode(node, _searchStrs, CaseSensitive, RegExp, FindAll);
                }
                BuildTabIsblNodes();
                BuildDisplayTree();
                Activate();
                GC.Collect();
            }
            public void Activate()
            {
                _controller.ActiveSearch = this;
                TreeViewResults.BringToFront();
                TextEditorHost.BringToFront();
                TextBoxFilter.BringToFront();
                CheckBoxFindRegExp.BringToFront();
                CheckBoxFindAll.BringToFront();
                CheckBoxFindCaseSensitive.BringToFront();
            }
            /// <summary>
            /// Подсветка строк поиска в текущем тексте разработки
            /// </summary>
            public void MarkSearchStrings()
            {
                GetSearchStrArray();
                //TextEditor.Document.MarkerStrategy.RemoveAll((marker) => { return true; });
                //TextEditor.Document.BookmarkManager.RemoveMarks((bookmark) => { return true; });

                if (RegExp)
                {
                    MarkSearchStringsRegExp(_searchStrs, CaseSensitive);
                }
                else
                {
                    MarkSearchStrings(_searchStrs, CaseSensitive);
                }
                SearchCriteriaTextEditor.TextArea.TextView.Redraw();
            }

            public void MarkSearchStringsRegExp(string[] regExpArray, bool caseSensitive)
            {
                RegexOptions regExpOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

                //Подсветка искомого текста
                if (regExpArray.Length > 0)
                {
                    String text = TextEditor.Text;
                    bool isCentered = false;

                    for (int indexRegExpStrings = 0; indexRegExpStrings < regExpArray.Length; indexRegExpStrings++)
                    {
                        string hlStr = regExpArray[indexRegExpStrings];
                        if (hlStr != "")
                        {
                            Regex regExp = new Regex(hlStr, regExpOptions);
                            MatchCollection regExpFindResults = regExp.Matches(text);

                            foreach (Match match in regExpFindResults)
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
                                //    TextEditor.ActiveTextAreaControl.CenterViewOn(lineNumber, 0);
                                //    isCentered = true;
                                //}
                            }
                        }
                    }
                }

            }

            public void MarkSearchStrings(string[] findArray, bool caseSensitive)
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
                        bool isCentered = false;

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

            public void BuildDisplayTree()
            {
                FilterIsblNodesByName();
                TreeViewResults.Nodes.Clear();
                TreeViewResults.BeginUpdate();
                CopyIsblNodesToTreeNodes(IsblNodes, TreeViewResults.Nodes);
                TreeViewResults.EndUpdate();
            }

            void FilterIsblNodesByName()
            {
                if (IsblNodes != null)
                {
                    foreach (Node node in IsblNodes)
                    {
                        FilterNodeByName(node, FilterText);
                    }
                }
            }

            void SetVisible(Node node)
            {
                node.Visible = true;
                if (node.Nodes != null)
                {
                    foreach (Node childNode in node.Nodes)
                    {
                        SetVisible(childNode);
                    }
                }
            }

            bool FilterNodeByName(Node node, string nameFilter)
            {
                if (node == null)
                    return false;

                bool isFound = false;
                //Пропуск пустых поисковых строк
                if (string.IsNullOrEmpty(nameFilter))
                {
                    SetVisible(node);
                    isFound = true;
                }
                else
                {
                    if (node.Name.ToUpper().Contains(nameFilter.ToUpper()))
                    {
                        SetVisible(node);
                        isFound = true;
                    }
                    else
                    {
                        if (node.Nodes != null)
                        {
                            foreach (Node subNode in node.Nodes)
                            {
                                if (FilterNodeByName(subNode, nameFilter))
                                {
                                    isFound = true;
                                }
                            }
                        }
                    }
                    node.Visible = isFound;
                }

                return isFound;
            }

            void CopyIsblNodesToTreeNodes(List<Node> isblNodes, TreeNodeCollection treeNodes)
            {
                foreach (Node isblNode in isblNodes)
                {
                    if (isblNode.Visible)
                    {
                        TreeNode treeNode = treeNodes.Add(isblNode.Name);
                        treeNode.Tag = isblNode;

                        if (isblNode.IsMatch)
                        {
                            treeNode.ForeColor = Color.Black;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Gray;
                        }

                        if (isblNode.Nodes != null)
                        {
                            CopyIsblNodesToTreeNodes(isblNode.Nodes, treeNode.Nodes);
                        }
                    }
                }
            }

            /// <summary>
            /// Возврат всех слов из текста реактора поискового запроса, 
            /// разделителями слов в тексте выступают пробельные символы.
            /// </summary>
            /// <returns></returns>
            void GetSearchStrArray()
            {
                if (RegExp)
                {
                    _searchStrs = CheckRegExpFormat();
                }
                else
                {
                   // SearchCriteriaTextEditor.Document.MarkerStrategy.RemoveAll((marker) => { return true; });
                    //SearchCriteriaTextEditor.Document.BookmarkManager.RemoveMarks((bookmark) => { return true; });
                    SearchCriteriaTextEditor.TextArea.TextView.Redraw();
                    string[] searchLineStrs = SearchCriteriaTextEditor.Text.Split(
                    new string[] { "\n" }
                    , StringSplitOptions.RemoveEmptyEntries
                    );
                    char[] delimeters = { ' ', '\t', '\n', '\r' };
                    List<string> searchWords = new List<string>();
                    foreach (string searchLine in searchLineStrs)
                    {
                        foreach (string searchWord in searchLine.Split(delimeters, StringSplitOptions.RemoveEmptyEntries))
                        {
                            searchWords.Add(searchWord.Trim());
                        }
                    }
                    _searchStrs = searchWords.ToArray();
                }
            }


            /// <summary>
            /// Проверка списка регулярных выражений на корректность и возврат списка строк корректных регулярных выражений.
            /// </summary>
            /// <returns></returns>
            string[] CheckRegExpFormat()
            {
                List<string> regExpResultList = new List<string>();
                //SearchCriteriaTextEditor.Document.MarkerStrategy.RemoveAll((marker) => { return true; });
                //SearchCriteriaTextEditor.Document.BookmarkManager.RemoveMarks((bookmark) => { return true; });
                bool errorExist = false;
                for (int indexRegExpCandidate = 0; indexRegExpCandidate < SearchCriteriaTextEditor.Document.LineCount; indexRegExpCandidate++)
                {
                    var line = SearchCriteriaTextEditor.Document.GetLineByNumber(indexRegExpCandidate);
                    ISegment segment = new TextSegment { StartOffset = line.Offset, EndOffset = line.EndOffset };
                    string regExpCandidateString = SearchCriteriaTextEditor.Document.GetText(segment);
                    try
                    {
                        if (!String.IsNullOrEmpty(regExpCandidateString.Trim()))
                        {
                            Regex regEx = new Regex(regExpCandidateString, RegexOptions.Compiled);
                            regExpResultList.Add(regExpCandidateString);
                        }
                    }
                    catch (System.ArgumentException ex)
                    {
                        //TextMarker marker = new TextMarker(
                        //    segment.Offset
                        //    , segment.Length
                        //    , TextMarkerType.WaveLine
                        //    , Color.Red
                        //    , Color.DarkRed
                        //    );
                        //marker.ToolTip = ex.Message;
                        //SearchCriteriaTextEditor.Document.MarkerStrategy.AddMarker(marker);

                        //Bookmark mark = new Bookmark(
                        //    SearchCriteriaTextEditor.Document
                        //    , SearchCriteriaTextEditor.Document.OffsetToPosition(segment.Offset)
                        //    , true);
                        //SearchCriteriaTextEditor.Document.BookmarkManager.AddMark(mark);
                        errorExist = true;
                    }
                }
                if (errorExist)
                {
                    SearchCriteriaTextEditor.TextArea.TextView.Redraw();
                }
                return regExpResultList.ToArray();
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
            bool FilterNode(Node node, string[] searchStrArray, bool caseSensitive, bool regExp, bool findAll)
            {
                bool isFound = false;
                //Сначала выделим текущий элемент так, как будто в нём ничего не найдено
                node.IsMatch = false;
                node.IsContainsMatchedNode = false;
                if (node.Nodes != null)
                {
                    foreach (Node subNode in node.Nodes)
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
                        Regex regEx = null;
                        if (_dictRegEx.ContainsKey(searchPhrase))
                        {
                            regEx = _dictRegEx[searchPhrase];
                        }
                        else
                        {
                            regEx = new Regex(searchPhrase, regExpOptions);
                            _dictRegEx.Add(searchPhrase, regEx);
                        }
                        if ((!String.IsNullOrEmpty(isblText) && regEx.IsMatch(isblText)) || regEx.IsMatch(nodeName))
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
                    StringComparison comparation = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

                    foreach (string searchPhrase in searchStrArray)
                    {
                        if (
                            (!String.IsNullOrEmpty(isblText) && isblText.IndexOf(searchPhrase, 0, comparation) >= 0) ||
                            (!String.IsNullOrEmpty(nodeName) && nodeName.IndexOf(searchPhrase, 0, comparation) >= 0)
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

            void BuildTabIsblNodes()
            {
                IsblNodes.Clear();
                CopyMatchedIsblNodes(IsblNodes, _controller.SourceIsblNodes);
            }
            void CopyMatchedIsblNodes(List<Node> targetNodes, List<Node> sourceNodes)
            {
                if (sourceNodes != null)
                {
                    foreach (Node isblNode in sourceNodes)
                    {
                        if (isblNode.IsMatch || isblNode.IsContainsMatchedNode)
                        {
                            var nodeCopy = isblNode.Clone();
                            if (isblNode.Nodes != null)
                            {
                                nodeCopy.Nodes = new List<Node>();
                                CopyMatchedIsblNodes(nodeCopy.Nodes, isblNode.Nodes);
                            }
                            targetNodes.Add(nodeCopy);
                        }
                    }
                }
            }
            void CopyIsblNodes(List<Node> targetNodes, List<Node> sourceNodes)
            {
                if (sourceNodes != null)
                {
                    foreach (Node isblNode in sourceNodes)
                    {
                        var nodeCopy = isblNode.Clone();
                        if (isblNode.Nodes != null)
                        {
                            nodeCopy.Nodes = new List<Node>();
                            CopyIsblNodes(nodeCopy.Nodes, isblNode.Nodes);
                        }
                        targetNodes.Add(nodeCopy);
                    }
                }
            }
        }
    }
}
