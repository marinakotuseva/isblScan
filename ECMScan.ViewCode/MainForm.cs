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
        public IsbDev SourceDev = new IsbDev();
        class SearchControls : IDisposable
        {
            public System.Windows.Forms.CheckBox CheckBoxFindRegExp { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindAll { get; set; }
            public System.Windows.Forms.CheckBox CheckBoxFindCaseSensitive { get; set; }
            public System.Windows.Forms.Integration.ElementHost TextEditorHost { get; set; }
            public System.Windows.Forms.Integration.ElementHost SearchCriteriaTextEditorHost { get; set; }
            public System.Windows.Forms.TextBox TextBoxFilter { get; set; }
            public TreeView TreeViewResults  = new TreeView();
            public Search Search { get; set; }
            private MainForm _form { get; set; }
            private TabPage _tab { get; set; }

            public SearchControls(MainForm form, TabPage tab)
            {
                _form = form;
                _tab = tab;
                Search = new Search(form.SourceDev);

                SearchCriteriaTextEditorHost = new System.Windows.Forms.Integration.ElementHost();
                SearchCriteriaTextEditorHost.Dock = DockStyle.Fill;
                SearchCriteriaTextEditorHost.Location = new Point(0, 0);
                SearchCriteriaTextEditorHost.TabIndex = 1;
                SearchCriteriaTextEditorHost.Child = Search.SearchCriteriaTextEditor;

                tab.Tag = this;
                tab.Controls.Add(SearchCriteriaTextEditorHost);
                tab.UseVisualStyleBackColor = true;

                Search.FillTreeView(TreeViewResults);
                TreeViewResults.CheckBoxes = false;
                TreeViewResults.Dock = DockStyle.Fill;
                TreeViewResults.HideSelection = false;
                TreeViewResults.Location = new Point(0, 0);
                TreeViewResults.Margin = new Padding(4);
                TreeViewResults.Name = "treeViewResults";
                TreeViewResults.TabIndex = 2121;
                TreeViewResults.AfterSelect += new TreeViewEventHandler(form.TreeViewResultsAfterSelect);
                _form.panelTree.Controls.Add(TreeViewResults);

                TextEditorHost = new System.Windows.Forms.Integration.ElementHost();
                TextEditorHost.Dock = DockStyle.Fill;
                TextEditorHost.Location = new Point(0, 0);
                TextEditorHost.Margin = new System.Windows.Forms.Padding(4);
                TextEditorHost.TabIndex = 150;
                TextEditorHost.Child = Search.TextEditor;
                _form.panelISBLResult.Controls.Add(TextEditorHost);

                TextBoxFilter = new System.Windows.Forms.TextBox();
                TextBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
                TextBoxFilter.Location = new System.Drawing.Point(0, 0);
                TextBoxFilter.Margin = new System.Windows.Forms.Padding(4);
                TextBoxFilter.Name = "textBoxFilter";
                TextBoxFilter.Size = new System.Drawing.Size(280, 22);
                TextBoxFilter.TabIndex = 2111;
                TextBoxFilter.TextChanged += new System.EventHandler(form.TextBoxFilter_TextChanged);
                _form.panelFilterTree.Controls.Add(TextBoxFilter);

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
                CheckBoxFindAll.CheckedChanged += new System.EventHandler(form.checkBoxFindAll_CheckedChanged);
                _form.panelSearchButtons.Controls.Add(CheckBoxFindAll);

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
                CheckBoxFindRegExp.CheckedChanged += new System.EventHandler(form.checkBoxFindRegExp_CheckedChanged);
                _form.panelSearchButtons.Controls.Add(CheckBoxFindRegExp);

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
                CheckBoxFindCaseSensitive.CheckedChanged += new System.EventHandler(form.checkBoxFindCaseSensitive_CheckedChanged);
                _form.panelSearchButtons.Controls.Add(CheckBoxFindCaseSensitive);
            }

            public void Activate()
            {
                TreeViewResults.BringToFront();
                TextEditorHost.BringToFront();
                TextBoxFilter.BringToFront();
                CheckBoxFindRegExp.BringToFront();
                CheckBoxFindAll.BringToFront();
                CheckBoxFindCaseSensitive.BringToFront();
            }

            public void Dispose()
            {
                TextEditorHost.Child = null;
                SearchCriteriaTextEditorHost.Child = null;
                _tab.Controls.Remove(SearchCriteriaTextEditorHost);
                _form.panelTree.Controls.Remove(TreeViewResults);
                _form.panelISBLResult.Controls.Remove(TextEditorHost);
                _form.panelFilterTree.Controls.Remove(TextBoxFilter);
                _form.panelSearchButtons.Controls.Remove(CheckBoxFindCaseSensitive);
                _form.panelSearchButtons.Controls.Remove(CheckBoxFindAll);
                _form.panelSearchButtons.Controls.Remove(CheckBoxFindRegExp);
                Search.Nodes.Clear();
                Search.Dispose();
                TreeViewResults.Nodes.Clear();
                TreeViewResults.Dispose();
                TextEditorHost.Dispose();
                SearchCriteriaTextEditorHost.Dispose();
                TextBoxFilter.Dispose();
                CheckBoxFindCaseSensitive.Dispose();
                CheckBoxFindAll.Dispose();
                CheckBoxFindRegExp.Dispose();
            }
        }
        

        public void textEditorControlRegExp_TextChanged(object sender, EventArgs e)
        {
            timerRegExpFind.Enabled = false;
            timerRegExpFind.Enabled = true;
        }

        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            groupBoxSearch_Resize(null, null);
            AddNewTab();

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
            SourceDev.Nodes.Clear();
            _isblLoader.Load(SourceDev.Nodes);

            bool isblNodesIsEmpty = true;
            foreach (var node in SourceDev.Nodes)
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
                        var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
                        searchControls.Search.Process();
                        searchControls.Search.FillTreeView(searchControls.TreeViewResults);
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
		    var searchControls = (SearchControls) tabControlSearchText.SelectedTab.Tag;
		    searchControls.Search.Process();
		    searchControls.Search.FillTreeView(searchControls.TreeViewResults);

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

        /// <summary>
        /// Изменение текста в поле фильтрации дерева проекта
        /// </summary>
        public void TextBoxFilter_TextChanged (object sender, System.EventArgs e)
		{
		    var searchControls = (SearchControls)tabControlSearchText.SelectedTab.Tag;
		    searchControls.Search.FillTreeView(searchControls.TreeViewResults, ((TextBox)sender).Text);
        }
		
		void CheckBoxWinAuthCheckedChanged(object sender, EventArgs e)
		{
			textBoxLogin.Enabled = !checkBoxWinAuth.Checked;
			textBoxPassword.Enabled = textBoxLogin.Enabled;
		}

        private void buttonCloseCurrentTab_Click(object sender, EventArgs e)
        {
            if (tabControlSearchText.TabCount > 1)
            {
                var tabPageForClose = tabControlSearchText.SelectedTab;
                ((SearchControls) tabPageForClose.Tag).Dispose();
                tabPageForClose.Tag = null;
                
                tabControlSearchText.SelectedIndex = tabControlSearchText.SelectedIndex > 0 ?
                    tabControlSearchText.SelectedIndex - 1 : 0;
                tabControlSearchText.Controls.Remove(tabPageForClose);
                tabPageForClose.Dispose();
                GC.Collect();
            }
        }

        private void buttonAddNewTab_Click(object sender, EventArgs e)
        {
            AddNewTab();
        }

        private void AddNewTab()
        {
            var tab = new TabPage(string.Format("Search {0}", tabControlSearchText.TabCount + 1));
            new SearchControls(this, tab);
            tabControlSearchText.Controls.Add(tab);
            tabControlSearchText.SelectedTab = tab;
        }
        public void checkBoxFindRegExp_CheckedChanged(object sender, EventArgs e)
        {
            ((SearchControls)tabControlSearchText.SelectedTab.Tag).Search.RegExp = ((CheckBox)sender).Checked;
        }

        public void checkBoxFindAll_CheckedChanged(object sender, EventArgs e)
        {
            ((SearchControls)tabControlSearchText.SelectedTab.Tag).Search.FindAll = ((CheckBox)sender).Checked;
        }

        public void tabControlSearchText_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((SearchControls) tabControlSearchText.SelectedTab.Tag).Activate();
        }

        public void checkBoxFindCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            ((SearchControls) tabControlSearchText.SelectedTab.Tag).Search.CaseSensitive = ((CheckBox)sender).Checked;
        }
	
	    void groupBoxSearch_Resize (object sender, System.EventArgs e)
	    {
		    this.buttonAddNewTab.Left = groupBoxSearch.Width - 36;
		    this.buttonCloseCurrentTab.Left = groupBoxSearch.Width - 19;
	    }

        private void buttonExpand_Click(object sender, EventArgs e)
        {
            var treeViewResults = ((SearchControls)tabControlSearchText.SelectedTab.Tag).TreeViewResults;
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
                var node = (IsbNode)e.Node.Tag;
                ((SearchControls)tabControlSearchText.SelectedTab.Tag).Search.TextEditor.Text = node.Text;
                toolStripStatusLabelSelectedElement.Text = node.Name;
            }
            else
            {
                ((SearchControls)tabControlSearchText.SelectedTab.Tag).Search.TextEditor.Text = "";
            }
        }
    }
}
