using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        Loader _isblLoader = new Loader();

        SearchController _controller;

        public void textEditorControlRegExp_TextChanged(object sender, EventArgs e)
        {
            timerRegExpFind.Enabled = false;
            timerRegExpFind.Enabled = true;
        }

        public void AddTreeViewResults(TreeView treeViewResults)
        {
            panelTree.Controls.Add(treeViewResults);
            panelTree.Dock = DockStyle.Fill;
            panelTree.Location = new Point(0, 25);
            panelTree.Margin = new Padding(4);
            panelTree.Name = "panelTree";
            panelTree.TabIndex = 212;
        }

        public void RemoveTreeView(TreeView treeViewResults)
        {
            panelTree.Controls.Remove(treeViewResults);
        }

        public void AddTextEditor(Control textEditorControlIsbl)
        {
            panelISBLResult.Controls.Add(textEditorControlIsbl);
        }
        public void RemoveTextEditor(Control textEditorControlIsbl)
        {
            panelISBLResult.Controls.Remove(textEditorControlIsbl);
        }

        public void AddTreeFilter(TextBox textBoxFilter)
        {
            panelFilterTree.Controls.Add(textBoxFilter);
        }
        public void RemoveTreeFilter(TextBox textBoxFilter)
        {
            panelFilterTree.Controls.Remove(textBoxFilter);
        }

        public void AddCheckBox(CheckBox checkBox)
        {
            panelSearchButtons.Controls.Add(checkBox);
        }
        public void RemoveCheckBox(CheckBox checkBox)
        {
            panelSearchButtons.Controls.Remove(checkBox);
        }

        /// <summary>
        /// Выбор узла в дереве разработки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TreeViewResultsAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                Node node = e.Node.Tag as Node;
                _controller.ActiveSearch.TextEditor.Text = node.SourceNode.Text;
                toolStripStatusLabelSelectedElement.Text = node.Name;
                _controller.ActiveSearch.MarkSearchStrings();
            }
            else
            {
                _controller.ActiveSearch.TextEditor.Text = "";
            }
        }

        public void AddTab(TabPage tab)
        {
            tabControlSearchText.Controls.Add(tab);
            tabControlSearchText.SelectedTab = tab;
        }

        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            groupBoxSearch_Resize(null, null);
            _controller = new SearchController(this);
            _controller.AddSearch("Search Criteria");

            string sqlServer;
            string dataBase;
            string login;
            string password;
            bool isWinAuth = false;


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
                string isWinAuthString = "false";
                namedArguments.TryGetValue("-S", out sqlServer);
                namedArguments.TryGetValue("-D", out dataBase);
                namedArguments.TryGetValue("-N", out login);
                namedArguments.TryGetValue("-W", out password);
                namedArguments.TryGetValue("-IsOSAuth", out isWinAuthString);
                isWinAuth = isWinAuthString?.ToLower() == "true";

                textBoxSQLServer.Text = sqlServer;
                textBoxDB.Text = dataBase;
                textBoxLogin.Text = login;
                checkBoxWinAuth.Checked = isWinAuth;
                textBoxPassword.Text = password;

                ConnectAndGetIsbl();
            }
            else
            {
                if (Configuration.Load(out sqlServer, out dataBase, out login, out isWinAuth))
                {
                    textBoxSQLServer.Text = sqlServer;
                    textBoxDB.Text = dataBase;
                    textBoxLogin.Text = login;
                    if (isWinAuth)
                    {
                        textBoxPassword.Text = "";
                        checkBoxWinAuth.Checked = isWinAuth;
                    }
                    else
                    {
                        textBoxPassword.Text = "";
                    }
                }
            }
            
        }

        /// <summary>
        /// Установка соединения с базой данных SQL Server
        /// </summary>
        /// <returns></returns>
		bool Connect()
        {
            bool connect;
            if (checkBoxWinAuth.Checked)
            {
                connect = _isblLoader.Connect(textBoxSQLServer.Text,
                                   textBoxDB.Text,
                                   "",
                                   "",
                                   true);
            }
            else
            {
                connect = _isblLoader.Connect(textBoxSQLServer.Text,
                                   textBoxDB.Text,
                                   textBoxLogin.Text,
                                   textBoxPassword.Text,
                                   false
                                  );
            }
            if (connect)
            {
                Configuration.Save(textBoxSQLServer.Text, textBoxDB.Text, textBoxLogin.Text, checkBoxWinAuth.Checked);
            }
            return connect;
        }

        /// <summary>
        /// Загрузка элементов разработки из базы данных
        /// </summary>
        bool GetIsbl()
        {
            _controller.SourceIsblNodes.Clear();
            _isblLoader.Load(_controller.SourceIsblNodes);

            bool isblNodesIsEmpty = true;
            foreach (Node node in _controller.SourceIsblNodes)
            {
                if (node != null)
                {
                    isblNodesIsEmpty = false;
                    break;
                }
            }
            if (isblNodesIsEmpty)
            {
                string loginSqlUser = this.textBoxLogin.Text;
                if (this.checkBoxWinAuth.Checked)
                {
                    loginSqlUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
                else
                {
                    loginSqlUser = this.textBoxLogin.Text;
                }
                string text = string.Format(
@"В выбранной базе данных ""{0}"" сервера ""{1}"" отсутствуют таблицы разработки.
Или у пользователя с логином ""{2}"" нет прав на просмотр содержимого таблиц разработки в указанной базе данных."
                , this.textBoxDB.Text
                , this.textBoxSQLServer.Text
                , loginSqlUser
);
                string caption = "Разработка не загружена";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Information;
                //MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1;
                MessageBox.Show(text, caption, buttons, icon);
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Соединение с SQL Server и загрузка элементов разработки из базы данных
        /// </summary>
        void ConnectAndGetIsbl()
        {
            if ((textBoxSQLServer.Text.Trim() != "") && (textBoxDB.Text.Trim() != "") &&
                   (checkBoxWinAuth.Checked || (!checkBoxWinAuth.Checked && textBoxLogin.Text.Trim() != ""))
              )
            {
                if (Connect())
                {
                    if (GetIsbl())
                    {
                        _controller.ActiveSearch.Refresh();
                        buttonSearch.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show(_isblLoader.ErrorText, "Ошибка открытия базы данных");
                }
            }
            else
            {
                if (textBoxLogin.Text.Trim() == "")
                {
                    textBoxLogin.Text = "Login";
                    textBoxLogin.Font = new Font(textBoxLogin.Font, FontStyle.Italic);
                    textBoxLogin.BackColor = Color.LightCoral;
                    textBoxLogin.SelectAll();
                    textBoxLogin.Focus();
                }
                if (textBoxDB.Text.Trim() == "")
                {
                    textBoxDB.Text = "Data Base";
                    textBoxDB.Font = new Font(textBoxDB.Font, FontStyle.Italic);
                    textBoxDB.BackColor = Color.LightCoral;
                    textBoxDB.SelectAll();
                    textBoxDB.Focus();
                }
                if (textBoxSQLServer.Text.Trim() == "")
                {
                    textBoxSQLServer.Text = "Sql Server";
                    textBoxSQLServer.Font = new Font(textBoxSQLServer.Font, FontStyle.Italic);
                    textBoxSQLServer.BackColor = Color.LightCoral;
                    textBoxSQLServer.SelectAll();
                    textBoxSQLServer.Focus();
                }
            }
        }

        /// <summary>
        /// Нажатие кнопки "Conact and Load ISBL"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void ButtonConnectClick(object sender, EventArgs e)
        {
            ConnectAndGetIsbl();
        }

        

       

        /// <summary>
        /// Нажатие кнопки "Find"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void ButtonFilterClick(object sender, EventArgs e)
		{
            buttonSearch.Enabled = false;
            _controller.ActiveSearch.Process();
            buttonSearch.Enabled = true;
        }

		/// <summary>
		/// Обработка нажатия клавиши в форме аутентификации на SQL Server
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>		
		void TextBoxLoginFormKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				ConnectAndGetIsbl();
			}
		}

		void TextBoxLoginFormTextChanged(object sender, EventArgs e)
		{
			(sender as TextBox).Font = new Font((sender as TextBox).Font, FontStyle.Regular);
			(sender as TextBox).BackColor = this.textBoxPassword.BackColor;
            (sender as TextBox).ForeColor = this.textBoxPassword.ForeColor;
		}

        bool FilterNodeByName(Node node, string nameFilter)
        {
            if (node == null)
                return false;

            bool isFound = false;
            //Пропуск пустых поисковых строк
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
            return isFound;
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
                        treeNode.Checked = true;
                        treeNode.ForeColor = Color.Black;
                    }
                    else
                    {
                        treeNode.Checked = false;
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
        /// Изменение текста в поле фильтрации дерева проекта
        /// </summary>
        public void TextBoxFilter_TextChanged (object sender, System.EventArgs e)
		{
		    _controller.ActiveSearch.FilterText = ((TextBox)sender).Text;
            _controller.ActiveSearch.BuildDisplayTree();
        }
		
		void CheckBoxWinAuthCheckedChanged(object sender, EventArgs e)
		{
			textBoxLogin.Enabled = !checkBoxWinAuth.Checked;
			textBoxPassword.Enabled = textBoxLogin.Enabled;
		}

        private void timerRegExpFind_Tick(object sender, EventArgs e)
        {
            timerRegExpFind.Enabled = false;
            _controller.ActiveSearch.MarkSearchStrings();
        }

        private void buttonCloseCurrentTab_Click(object sender, EventArgs e)
        {
            if (tabControlSearchText.TabCount > 1)
            {
                TabPage tabPageForClose = tabControlSearchText.SelectedTab;
                tabControlSearchText.SelectedIndex = tabControlSearchText.SelectedIndex > 0 ?
                    tabControlSearchText.SelectedIndex - 1 : 0;
                ((SearchController.Search) tabPageForClose.Tag).Clear();
                tabControlSearchText.Controls.Remove(tabPageForClose);
                GC.Collect();
            }
        }

        private void buttonAddNewTab_Click(object sender, EventArgs e)
        {
            _controller.AddSearch(string.Format("Search {0}", tabControlSearchText.TabCount + 1));
        }
        public void checkBoxFindRegExp_CheckedChanged(object sender, EventArgs e)
        {
            _controller.ActiveSearch.RegExp = ((CheckBox)sender).Checked;
            _controller.ActiveSearch.MarkSearchStrings();
        }

        public void checkBoxFindAll_CheckedChanged(object sender, EventArgs e)
        {
            _controller.ActiveSearch.FindAll = ((CheckBox)sender).Checked;
        }

        public void tabControlSearchText_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((SearchController.Search) tabControlSearchText.SelectedTab.Tag).Activate();
        }

        public void checkBoxFindCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            _controller.ActiveSearch.CaseSensitive = ((CheckBox)sender).Checked;
            _controller.ActiveSearch.MarkSearchStrings();
        }
	
	    void groupBoxSearch_Resize (object sender, System.EventArgs e)
	    {
		    this.buttonAddNewTab.Left = groupBoxSearch.Width - 36;
		    this.buttonCloseCurrentTab.Left = groupBoxSearch.Width - 19;
	    }

        private void buttonExpand_Click(object sender, EventArgs e)
        {
            var treeViewResults = _controller.ActiveSearch.TreeViewResults;
            treeViewResults.BeginUpdate();
            foreach (TreeNode node in treeViewResults.Nodes)
            {
                node.ExpandAll();
            }
            treeViewResults.EndUpdate();
        }
    }
}
