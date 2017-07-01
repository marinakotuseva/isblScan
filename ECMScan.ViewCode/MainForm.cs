using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CheckBox = System.Windows.Forms.CheckBox;
using FontStyle = System.Drawing.FontStyle;
using MessageBox = System.Windows.Forms.MessageBox;
using Point = System.Drawing.Point;
using TextBox = System.Windows.Forms.TextBox;
using TreeView = System.Windows.Forms.TreeView;
using ExtensionMethods;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        public IsbDev SourceDev = new IsbDev();
        public IsbDev TargetDev = new IsbDev();
        public SearchControls searchControl { get; set; }
        public class SearchControls : IDisposable
        {
            public System.Windows.Forms.CheckBox CheckBoxFindRegExp { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindAll { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindCaseSensitive { get; set; }
            public System.Windows.Forms.Integration.ElementHost TextEditorHost { get; set; }
            public System.Windows.Forms.Integration.ElementHost TextEditorHostTarget { get; set; }
            public System.Windows.Forms.Integration.ElementHost SearchCriteriaTextEditorHost { get; set; }
            public System.Windows.Forms.TextBox TextBoxFilterName { get; set; }
            public System.Windows.Forms.DateTimePicker TextBoxFilterStartDate { get; set; }
            public System.Windows.Forms.DateTimePicker TextBoxFilterEndDate { get; set; }
            public TreeView TreeViewResults = new TreeView();
            public Search Search { get; set; }
            public MainForm _form { get; set; }
            private TabPage _tab { get; set; }
            private List<TreeNode> AlreadyAutoSelectedTreeNodes { get; set; } = new List<TreeNode>();
            public System.Windows.Controls.ScrollViewer ScrollViewerSource { get; set; }
            public System.Windows.Controls.ScrollViewer ScrollViewerTarget { get; set; }

            public SearchControls(MainForm form)
            {
                _form = form;
                Search = new Search(form.SourceDev);
                Search.SearchControls = this;

                //SearchCriteriaTextEditorHost = new System.Windows.Forms.Integration.ElementHost();
                //SearchCriteriaTextEditorHost.Dock = DockStyle.Fill;
                //SearchCriteriaTextEditorHost.Location = new Point(0, 0);
                //SearchCriteriaTextEditorHost.TabIndex = 1;
                //SearchCriteriaTextEditorHost.Child = Search.SearchCriteriaTextEditor;

                //_tab.Tag = this;
                //_tab.Controls.Add(SearchCriteriaTextEditorHost);
                //_tab.UseVisualStyleBackColor = true;
                //Search.SearchCriteriaTextEditor.Focus();

                Search.FillTreeView(TreeViewResults);
                TreeViewResults.CheckBoxes = false;
                TreeViewResults.Dock = DockStyle.Fill;
                TreeViewResults.HideSelection = false;
                TreeViewResults.Location = new Point(0, 0);
                TreeViewResults.Margin = new Padding(4);
                TreeViewResults.Name = "treeViewResults";
                TreeViewResults.TabIndex = 2121;
                TreeViewResults.AfterSelect += new TreeViewEventHandler(form.TreeViewResultsAfterSelect);
                _form.splitContainer3.Panel2.Controls.Add(TreeViewResults);

                TextEditorHost = new System.Windows.Forms.Integration.ElementHost();
                TextEditorHost.Dock = DockStyle.Fill;
                TextEditorHost.Location = new Point(0, 0);
                TextEditorHost.Margin = new System.Windows.Forms.Padding(4);
                TextEditorHost.TabIndex = 150;
                TextEditorHost.Child = Search.TextEditor;
                _form.splitContainer2.Panel2.Controls.Add(TextEditorHost);

                TextEditorHostTarget = new System.Windows.Forms.Integration.ElementHost();
                TextEditorHostTarget.Dock = DockStyle.Fill;
                TextEditorHostTarget.Location = new Point(0, 0);
                TextEditorHostTarget.Margin = new System.Windows.Forms.Padding(4);
                TextEditorHostTarget.TabIndex = 151;
                TextEditorHostTarget.Child = Search.TextEditorTarget;
                _form.splitContainer2.Panel1.Controls.Add(TextEditorHostTarget);

                TextBoxFilterName = new System.Windows.Forms.TextBox();
                //TextBoxFilterName.Dock = System.Windows.Forms.DockStyle.Fill;
                TextBoxFilterName.Location = new System.Drawing.Point(10, 10);
                TextBoxFilterName.Margin = new System.Windows.Forms.Padding(4);
                TextBoxFilterName.Name = "textBoxFilter";
                TextBoxFilterName.Size = new System.Drawing.Size(280, 22);
                TextBoxFilterName.TabIndex = 2111;
                TextBoxFilterName.TextChanged += new System.EventHandler(form.TextBoxFilter_TextChanged);
                _form.splitContainer3.Panel1.Controls.Add(TextBoxFilterName);

                TextBoxFilterStartDate = new System.Windows.Forms.DateTimePicker();
                TextBoxFilterStartDate.Format = DateTimePickerFormat.Custom;
                TextBoxFilterStartDate.CustomFormat = "yyyy-MM-dd HH:mm";
                TextBoxFilterStartDate.Location = new System.Drawing.Point(10, 37);
                TextBoxFilterStartDate.Margin = new System.Windows.Forms.Padding(4);
                TextBoxFilterStartDate.Name = "textBoxFilterStartDate";
                TextBoxFilterStartDate.Size = new System.Drawing.Size(130, 22);
                TextBoxFilterStartDate.TabIndex = 2112;
                TextBoxFilterStartDate.TextChanged += new System.EventHandler(form.TextBoxFilterStartDate_TextChanged);
                TextBoxFilterStartDate.Value = DateTimePicker.MinimumDateTime.Date;
                _form.splitContainer3.Panel1.Controls.Add(TextBoxFilterStartDate);

                TextBoxFilterEndDate = new System.Windows.Forms.DateTimePicker();
                TextBoxFilterEndDate.Format = DateTimePickerFormat.Custom;
                TextBoxFilterEndDate.CustomFormat = "yyyy-MM-dd HH:mm";
                TextBoxFilterEndDate.Location = new System.Drawing.Point(160, 37);
                TextBoxFilterEndDate.Margin = new System.Windows.Forms.Padding(4);
                TextBoxFilterEndDate.Name = "textBoxFilterEndDate";
                TextBoxFilterEndDate.Size = new System.Drawing.Size(130, 22);
                TextBoxFilterEndDate.TabIndex = 2113;
                TextBoxFilterEndDate.TextChanged += new System.EventHandler(form.TextBoxFilterEndDate_TextChanged);
                TextBoxFilterEndDate.Value = DateTimePicker.MaximumDateTime.Date.AddDays(-1).AddHours(23).AddMinutes(59);
                _form.splitContainer3.Panel1.Controls.Add(TextBoxFilterEndDate);

                //CheckBoxFindAll = new System.Windows.Forms.CheckBox();
                //CheckBoxFindAll.AutoSize = true;
                //CheckBoxFindAll.Dock = System.Windows.Forms.DockStyle.None;
                //CheckBoxFindAll.Location = new System.Drawing.Point(320, 4);
                //CheckBoxFindAll.Margin = new System.Windows.Forms.Padding(4);
                //CheckBoxFindAll.Name = "checkBoxFindAll";
                //CheckBoxFindAll.Size = new System.Drawing.Size(146, 28);
                //CheckBoxFindAll.TabIndex = 1226;
                //CheckBoxFindAll.Text = "satisfies all criteria";
                //CheckBoxFindAll.UseVisualStyleBackColor = true;
                //CheckBoxFindAll.CheckedChanged += new System.EventHandler(form.checkBoxFindAll_CheckedChanged);
                //_form.splitContainer3.Panel1.Controls.Add(CheckBoxFindAll);

                //CheckBoxFindRegExp = new System.Windows.Forms.CheckBox();
                //CheckBoxFindRegExp.AutoSize = true;
                //CheckBoxFindRegExp.Dock = System.Windows.Forms.DockStyle.None;
                //CheckBoxFindRegExp.Location = new System.Drawing.Point(150, 4);
                //CheckBoxFindRegExp.Margin = new System.Windows.Forms.Padding(4);
                //CheckBoxFindRegExp.Name = "CheckBoxFindRegExp";
                //CheckBoxFindRegExp.Size = new System.Drawing.Size(170, 28);
                //CheckBoxFindRegExp.TabIndex = 1225;
                //CheckBoxFindRegExp.Text = ".* (regular expression)";
                //CheckBoxFindRegExp.UseVisualStyleBackColor = true;
                //CheckBoxFindRegExp.CheckedChanged += new System.EventHandler(form.checkBoxFindRegExp_CheckedChanged);
                //_form.splitContainer3.Panel1.Controls.Add(CheckBoxFindRegExp);

                //CheckBoxFindCaseSensitive = new System.Windows.Forms.CheckBox();
                //CheckBoxFindCaseSensitive.AutoSize = true;
                //CheckBoxFindCaseSensitive.Dock = System.Windows.Forms.DockStyle.None;
                //CheckBoxFindCaseSensitive.Location = new System.Drawing.Point(2, 4);
                //CheckBoxFindCaseSensitive.Margin = new System.Windows.Forms.Padding(4);
                //CheckBoxFindCaseSensitive.Name = "checkBoxFindCaseSensitive";
                //CheckBoxFindCaseSensitive.Size = new System.Drawing.Size(150, 28);
                //CheckBoxFindCaseSensitive.TabIndex = 1224;
                //CheckBoxFindCaseSensitive.Text = "Aa (case sensitive)";
                //CheckBoxFindCaseSensitive.UseVisualStyleBackColor = true;
                //CheckBoxFindCaseSensitive.CheckedChanged += new System.EventHandler(form.checkBoxFindCaseSensitive_CheckedChanged);
                //_form.splitContainer3.Panel1.Controls.Add(CheckBoxFindCaseSensitive);

                _form.searchControl = this;
                this.Activate();
            }

            public void onScroll(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
            {
                if (sender == ScrollViewerSource)
                {
                    ScrollViewerTarget.ScrollToVerticalOffset(e.VerticalOffset);
                }
                else
                {
                    ScrollViewerSource.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }

            public void Activate()
            {
                TreeViewResults.BringToFront();
                TextEditorHost.BringToFront();
                TextEditorHostTarget.BringToFront();
                TextBoxFilterName.BringToFront();
                TextBoxFilterStartDate.BringToFront();
                TextBoxFilterEndDate.BringToFront();
                //CheckBoxFindRegExp.BringToFront();
                //CheckBoxFindAll.BringToFront();
                //CheckBoxFindCaseSensitive.BringToFront();
            }

            public void Dispose()
            {
                //TextEditorHost.Child = null;
                //SearchCriteriaTextEditorHost.Child = null;
                //_tab.Controls.Remove(SearchCriteriaTextEditorHost);
                //_form.panelTree.Controls.Remove(TreeViewResults);
                //_form.panelISBLResult.Controls.Remove(TextEditorHost);
                //_form.panelFilterTreeName.Controls.Remove(TextBoxFilterName);
                //_form.panelFilterTreeDate.Controls.Remove(TextBoxFilterStartDate);
                //_form.panelFilterTreeDate.Controls.Remove(TextBoxFilterEndDate);
                //_form.panelSearchButtons.Controls.Remove(CheckBoxFindCaseSensitive);
                //_form.panelSearchButtons.Controls.Remove(CheckBoxFindAll);
                //_form.panelSearchButtons.Controls.Remove(CheckBoxFindRegExp);
                //Search.Nodes.Clear();
                //Search.Dispose();
                //TreeViewResults.Nodes.Clear();
                //TreeViewResults.Dispose();
                //TextEditorHost.Dispose();
                //SearchCriteriaTextEditorHost.Dispose();
                //TextBoxFilterName.Dispose();
                //CheckBoxFindCaseSensitive.Dispose();
                //CheckBoxFindAll.Dispose();
                //CheckBoxFindRegExp.Dispose();
            }

            public void Process()
            {
                //_form.buttonSearch.Enabled = false;
                Search.Process();
                Search.FillTreeView(TreeViewResults);
                AlreadyAutoSelectedTreeNodes.Clear();
                TreeSelectNextMatched(TreeViewResults.Nodes);
                //_form.buttonSearch.Enabled = true;
            }

            public bool TreeSelectNextMatched(TreeNodeCollection treeNodes)
            {
                bool isFound = false;
                foreach (TreeNode node in treeNodes)
                {
                    var searchNode = (SearchNode)node.Tag;
                    if (searchNode.IsMatch && !AlreadyAutoSelectedTreeNodes.Contains(node))
                    {
                        TreeViewResults.SelectedNode = node;
                        AlreadyAutoSelectedTreeNodes.Add(node);
                        isFound = true;
                        break;
                    }
                    else
                    {
                        isFound = TreeSelectNextMatched(node.Nodes);
                        if (isFound)
                        {
                            break;
                        }
                    }
                }
                return isFound;
            }

            public void TextEditorShowNextMatchedString(bool gotoNextTreeNodeIfNoMore = true)
            {
                var matchedPos = Search.GetMatchedWordPositions(Search.TextEditor.Text, Search.TextEditor.TextArea.Caret.Offset).FirstOrDefault();
                if (matchedPos != null)
                {
                    Search.TextEditor.TextArea.Caret.Offset = matchedPos.Start + matchedPos.Length;
                    Search.TextEditor.TextArea.Caret.BringCaretToView();
                    Search.TextEditor.SelectionStart = matchedPos.Start;
                    Search.TextEditor.SelectionLength = matchedPos.Length;
                }
                else
                {
                    if(gotoNextTreeNodeIfNoMore) TreeSelectNextMatched(TreeViewResults.Nodes);
                }
            }
        }

        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            this.Load += Window_Loaded;
            this.KeyDown += MainForm_KeyDown;
            new SearchControls(this);

            string sqlServer;
            string dataBase;
            string login;
            string password;

            string sqlServerTarget;
            string dataBaseTarget;
            string loginTarget;
            string passwordTarget;


            var namedArguments = new Dictionary<string, string>();
            String[] arguments = Environment.GetCommandLineArgs();

            foreach (string argument in arguments)
            {
                string[] splitted = argument.Split('=');

                if (splitted.Length == 2)
                {
                    namedArguments[splitted[0]] = splitted[1];
                }
            }
            if (namedArguments.Count > 0)
            {
                namedArguments.TryGetValue("-S", out sqlServer);
                namedArguments.TryGetValue("-D", out dataBase);
                namedArguments.TryGetValue("-N", out login);
                namedArguments.TryGetValue("-W", out password);

                SourceDev.ConnectionParams.Server = sqlServer;
                SourceDev.ConnectionParams.Database = dataBase;
                SourceDev.ConnectionParams.Login = login;
                SourceDev.ConnectionParams.Password = password;
                SourceDev.Load();


                namedArguments.TryGetValue("-S2", out sqlServerTarget);
                namedArguments.TryGetValue("-D2", out dataBaseTarget);
                namedArguments.TryGetValue("-N2", out loginTarget);
                namedArguments.TryGetValue("-W2", out passwordTarget);

                TargetDev.ConnectionParams.Server = sqlServerTarget;
                TargetDev.ConnectionParams.Database = dataBaseTarget;
                TargetDev.ConnectionParams.Login = loginTarget;
                TargetDev.ConnectionParams.Password = passwordTarget;
                TargetDev.Load();

                SourceDev.CompareWith(TargetDev);
                searchControl = new SearchControls(this);
            }
        }

        private void Window_Loaded(object sender, EventArgs eventArgs)
        {
            //searchControl.Search.SearchCriteriaTextEditor.Focus();
            searchControl.ScrollViewerTarget = searchControl.Search.TextEditorTarget.GetChildOfType<System.Windows.Controls.ScrollViewer>();
            searchControl.ScrollViewerTarget.ScrollChanged += new System.Windows.Controls.ScrollChangedEventHandler(searchControl.onScroll);
            searchControl.ScrollViewerSource = searchControl.Search.TextEditor.GetChildOfType<System.Windows.Controls.ScrollViewer>();
            searchControl.ScrollViewerSource.ScrollChanged += new System.Windows.Controls.ScrollChangedEventHandler(searchControl.onScroll);
        }

        /// <summary>
        /// Нажатие кнопки "Find"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void ButtonFilterClick(object sender, EventArgs e)
        {
            searchControl.Process();
        }

        /// <summary>
        /// Изменение текста в поле фильтрации дерева проекта
        /// </summary>
        public void TextBoxFilter_TextChanged(object sender, System.EventArgs e)
        {
            searchControl.Search.FilterName = ((TextBox)sender).Text;
            searchControl.Search.FillTreeView(searchControl.TreeViewResults);
        }
        /// <summary>
        /// Изменение начальной даты в поле фильтрации дерева проекта
        /// </summary>
        public void TextBoxFilterStartDate_TextChanged(object sender, System.EventArgs e)
        {
            searchControl.Search.FilterStartDate = ((DateTimePicker)sender).Value;
            searchControl.Search.FillTreeView(searchControl.TreeViewResults);
        }
        /// <summary>
        /// Изменение конечной даты в поле фильтрации дерева проекта
        /// </summary>
        public void TextBoxFilterEndDate_TextChanged(object sender, System.EventArgs e)
        {
            searchControl.Search.FilterEndDate = ((DateTimePicker)sender).Value;
            searchControl.Search.FillTreeView(searchControl.TreeViewResults);
        }

        //private void buttonCloseCurrentTab_Click(object sender, EventArgs e)
        //{
        //    if (tabControlSearchText.TabCount > 1)
        //    {
        //        var tabPageForClose = tabControlSearchText.SelectedTab;
        //        ((SearchControls)tabPageForClose.Tag).Dispose();
        //        tabPageForClose.Tag = null;

        //        tabControlSearchText.SelectedIndex = tabControlSearchText.SelectedIndex > 0 ?
        //            tabControlSearchText.SelectedIndex - 1 : 0;
        //        tabControlSearchText.Controls.Remove(tabPageForClose);
        //        tabPageForClose.Dispose();
        //        GC.Collect();
        //    }
        //}

        private void buttonAddNewTab_Click(object sender, EventArgs e)
        {
            new SearchControls(this);
        }

        public void checkBoxFindRegExp_CheckedChanged(object sender, EventArgs e)
        {
            searchControl.Search.RegExp = ((CheckBox)sender).Checked;
        }

        public void checkBoxFindAll_CheckedChanged(object sender, EventArgs e)
        {
            searchControl.Search.FindAll = ((CheckBox)sender).Checked;
        }

        public void tabControlSearchText_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchControl?.Activate();
        }

        public void checkBoxFindCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            searchControl.Search.CaseSensitive = ((CheckBox)sender).Checked;
        }

        private void buttonExpand_Click(object sender, EventArgs e)
        {
            var treeViewResults = searchControl.TreeViewResults;
            treeViewResults.BeginUpdate();
            foreach (TreeNode node in treeViewResults.Nodes)
            {
                node.ExpandAll();
            }
            treeViewResults.EndUpdate();
        }
        /// <summary>
        /// Выбор узла в дереве разработки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewResultsAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                var node = (SearchNode)e.Node.Tag;
                searchControl.Search.TextEditor.Text = node.IsbNode.SourceNode?.Text;
                searchControl.Search.TextEditor.TextArea.Caret.Offset = 0;
                searchControl.Search.TextEditorTarget.Text = node.IsbNode.TargetNode?.Text;
                searchControl.Search.TextEditorTarget.TextArea.Caret.Offset = 0;
                //searchControl.TextEditorShowNextMatchedString(false);
                toolStripStatusLabelSelectedElement.Text = node.Name;
                toolStripStatusLabelLastUpd.Text = node.IsbNode.SourceNode?.LastUpdate?.ToString("yyyy-MM-dd HH:mm");

                if (node.IsbNode.CompareResult == CompareResult.Changed)
                {
                    var dmp = new DiffMatchPatch.diff_match_patch();
                    searchControl.Search.Diffs = dmp.diff_main(searchControl.Search.TextEditor.Text, searchControl.Search.TextEditorTarget.Text);
                    dmp.diff_cleanupSemantic(searchControl.Search.Diffs);
                }
                else
                {
                    searchControl.Search.Diffs = null;
                }
            }
            else
            {
                searchControl.Search.TextEditor.Text = "";
                toolStripStatusLabelSelectedElement.Text = "";
                toolStripStatusLabelLastUpd.Text = "";
            }
        }

        public void MainForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                searchControl.TreeSelectNextMatched(searchControl.TreeViewResults.Nodes);
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.F3)
            {
                searchControl.TextEditorShowNextMatchedString();
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.T && e.Control)
            {
                new SearchControls(this);
            }
            if (e.KeyCode == Keys.O && e.Control)
            {
                var selectedNode = searchControl.TreeViewResults.SelectedNode;
                if(selectedNode != null)
                {
                    ((SearchNode)selectedNode.Tag).IsbNode.SourceNode?.OpenInSbrte(SourceDev.ConnectionParams);
                }
            }
        }
    }  
}

namespace ExtensionMethods
{
    public  static class MyExtension
    {
        public static T GetChildOfType<T>(this DependencyObject depObj)
        where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}