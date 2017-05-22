using ISBLScan.ViewCode;
namespace ISBLScan.ViewCode
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

	
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelOptions = new System.Windows.Forms.Panel();
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.buttonAddNewTab = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonCloseCurrentTab = new System.Windows.Forms.Button();
            this.panelSearchText = new System.Windows.Forms.Panel();
            this.tabControlSarchText = new System.Windows.Forms.TabControl();
            this.tabPageSimpleSearchText = new System.Windows.Forms.TabPage();
            this.textEditorControlSearchText = new ICSharpCode.TextEditor.TextEditorControl();
            this.tabPageRegularExpression = new System.Windows.Forms.TabPage();
            this.textEditorControlRegExp = new ICSharpCode.TextEditor.TextEditorControl();
            this.panelSearchButtons = new System.Windows.Forms.Panel();
            this.checkBoxFindAll = new System.Windows.Forms.CheckBox();
            this.checkBoxFindRegExp = new System.Windows.Forms.CheckBox();
            this.checkBoxFindCaseSensitive = new System.Windows.Forms.CheckBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.groupBoxConnect = new System.Windows.Forms.GroupBox();
            this.buttonExpand = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.checkBoxWinAuth = new System.Windows.Forms.CheckBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.textBoxDB = new System.Windows.Forms.TextBox();
            this.textBoxSQLServer = new System.Windows.Forms.TextBox();
            this.panelResults = new System.Windows.Forms.Panel();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.splitContainerResults = new System.Windows.Forms.SplitContainer();
            this.panelTree = new System.Windows.Forms.Panel();
            this.treeViewResults = new System.Windows.Forms.TreeView();
            this.panelFilterTree = new System.Windows.Forms.Panel();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.panelISBLResult = new System.Windows.Forms.Panel();
            this.textEditorControlISBL = new ICSharpCode.TextEditor.TextEditorControl();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.timerRegExpFind = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelLastUpd = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelDash = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSelectedElement = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTest = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.backgroundWorkerFind = new System.ComponentModel.BackgroundWorker();
            this.panelOptions.SuspendLayout();
            this.groupBoxSearch.SuspendLayout();
            this.panelSearchText.SuspendLayout();
            this.tabControlSarchText.SuspendLayout();
            this.tabPageSimpleSearchText.SuspendLayout();
            this.tabPageRegularExpression.SuspendLayout();
            this.panelSearchButtons.SuspendLayout();
            this.groupBoxConnect.SuspendLayout();
            this.panelResults.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            this.splitContainerResults.Panel1.SuspendLayout();
            this.splitContainerResults.Panel2.SuspendLayout();
            this.splitContainerResults.SuspendLayout();
            this.panelTree.SuspendLayout();
            this.panelFilterTree.SuspendLayout();
            this.panelISBLResult.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelOptions
            // 
            this.panelOptions.Controls.Add(this.groupBoxSearch);
            this.panelOptions.Controls.Add(this.groupBoxConnect);
            this.panelOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOptions.Location = new System.Drawing.Point(0, 0);
            this.panelOptions.Margin = new System.Windows.Forms.Padding(4);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(1056, 201);
            this.panelOptions.TabIndex = 1;
            // 
            // groupBoxSearch
            // 
            this.groupBoxSearch.Controls.Add(this.buttonAddNewTab);
            this.groupBoxSearch.Controls.Add(this.buttonCloseCurrentTab);
            this.groupBoxSearch.Controls.Add(this.panelSearchText);
            this.groupBoxSearch.Controls.Add(this.panelSearchButtons);
            this.groupBoxSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSearch.Location = new System.Drawing.Point(195, 0);
            this.groupBoxSearch.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxSearch.Name = "groupBoxSearch";
            this.groupBoxSearch.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxSearch.Size = new System.Drawing.Size(861, 201);
            this.groupBoxSearch.TabIndex = 12;
            this.groupBoxSearch.TabStop = false;
            this.groupBoxSearch.Text = "Search Text";
            this.groupBoxSearch.DoubleClick += new System.EventHandler(this.buttonAddNewTab_Click);
            this.groupBoxSearch.Resize += new System.EventHandler(this.groupBoxSearch_Resize);
            // 
            // buttonAddNewTab
            // 
            this.buttonAddNewTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddNewTab.ImageIndex = 1;
            this.buttonAddNewTab.ImageList = this.imageList1;
            this.buttonAddNewTab.Location = new System.Drawing.Point(804, 0);
            this.buttonAddNewTab.Margin = new System.Windows.Forms.Padding(0);
            this.buttonAddNewTab.Name = "buttonAddNewTab";
            this.buttonAddNewTab.Size = new System.Drawing.Size(19, 17);
            this.buttonAddNewTab.TabIndex = 125;
            this.buttonAddNewTab.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonAddNewTab.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonAddNewTab.UseVisualStyleBackColor = true;
            this.buttonAddNewTab.Click += new System.EventHandler(this.buttonAddNewTab_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            this.imageList1.Images.SetKeyName(0, "close8.png");
            this.imageList1.Images.SetKeyName(1, "add8.png");
            // 
            // buttonCloseCurrentTab
            // 
            this.buttonCloseCurrentTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCloseCurrentTab.ImageIndex = 0;
            this.buttonCloseCurrentTab.ImageList = this.imageList1;
            this.buttonCloseCurrentTab.Location = new System.Drawing.Point(831, 0);
            this.buttonCloseCurrentTab.Margin = new System.Windows.Forms.Padding(0);
            this.buttonCloseCurrentTab.Name = "buttonCloseCurrentTab";
            this.buttonCloseCurrentTab.Size = new System.Drawing.Size(19, 17);
            this.buttonCloseCurrentTab.TabIndex = 1;
            this.buttonCloseCurrentTab.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonCloseCurrentTab.UseVisualStyleBackColor = true;
            this.buttonCloseCurrentTab.Click += new System.EventHandler(this.buttonCloseCurrentTab_Click);
            // 
            // panelSearchText
            // 
            this.panelSearchText.Controls.Add(this.tabControlSarchText);
            this.panelSearchText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSearchText.Location = new System.Drawing.Point(4, 19);
            this.panelSearchText.Margin = new System.Windows.Forms.Padding(4);
            this.panelSearchText.Name = "panelSearchText";
            this.panelSearchText.Size = new System.Drawing.Size(853, 150);
            this.panelSearchText.TabIndex = 124;
            // 
            // tabControlSarchText
            // 
            this.tabControlSarchText.AllowDrop = true;
            this.tabControlSarchText.Controls.Add(this.tabPageSimpleSearchText);
            this.tabControlSarchText.Controls.Add(this.tabPageRegularExpression);
            this.tabControlSarchText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSarchText.Location = new System.Drawing.Point(0, 0);
            this.tabControlSarchText.Margin = new System.Windows.Forms.Padding(4);
            this.tabControlSarchText.Name = "tabControlSarchText";
            this.tabControlSarchText.SelectedIndex = 0;
            this.tabControlSarchText.Size = new System.Drawing.Size(853, 150);
            this.tabControlSarchText.TabIndex = 123;
            this.tabControlSarchText.SelectedIndexChanged += new System.EventHandler(this.tabControlSarchText_SelectedIndexChanged);
            this.tabControlSarchText.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControlSarchText_Selecting);
            // 
            // tabPageSimpleSearchText
            // 
            this.tabPageSimpleSearchText.Controls.Add(this.textEditorControlSearchText);
            this.tabPageSimpleSearchText.Location = new System.Drawing.Point(4, 25);
            this.tabPageSimpleSearchText.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageSimpleSearchText.Name = "tabPageSimpleSearchText";
            this.tabPageSimpleSearchText.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageSimpleSearchText.Size = new System.Drawing.Size(845, 121);
            this.tabPageSimpleSearchText.TabIndex = 0;
            this.tabPageSimpleSearchText.Tag = this.textEditorControlSearchText;
            this.tabPageSimpleSearchText.Text = "Simple Search Text (sample)";
            this.tabPageSimpleSearchText.UseVisualStyleBackColor = true;
            // 
            // textEditorControlSearchText
            // 
            this.textEditorControlSearchText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorControlSearchText.IsReadOnly = false;
            this.textEditorControlSearchText.Location = new System.Drawing.Point(4, 4);
            this.textEditorControlSearchText.Margin = new System.Windows.Forms.Padding(4);
            this.textEditorControlSearchText.Name = "textEditorControlSearchText";
            this.textEditorControlSearchText.ShowEOLMarkers = true;
            this.textEditorControlSearchText.ShowSpaces = true;
            this.textEditorControlSearchText.ShowTabs = true;
            this.textEditorControlSearchText.Size = new System.Drawing.Size(837, 113);
            this.textEditorControlSearchText.TabIndex = 1;
            this.textEditorControlSearchText.TextChanged += new System.EventHandler(this.textEditorControlRegExp_TextChanged);
            // 
            // tabPageRegularExpression
            // 
            this.tabPageRegularExpression.Controls.Add(this.textEditorControlRegExp);
            this.tabPageRegularExpression.Location = new System.Drawing.Point(4, 25);
            this.tabPageRegularExpression.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageRegularExpression.Name = "tabPageRegularExpression";
            this.tabPageRegularExpression.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageRegularExpression.Size = new System.Drawing.Size(845, 121);
            this.tabPageRegularExpression.TabIndex = 1;
            this.tabPageRegularExpression.Tag = this.textEditorControlRegExp;
            this.tabPageRegularExpression.Text = "Regular Expression (sample)";
            this.tabPageRegularExpression.UseVisualStyleBackColor = true;
            // 
            // textEditorControlRegExp
            // 
            this.textEditorControlRegExp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorControlRegExp.IsReadOnly = false;
            this.textEditorControlRegExp.Location = new System.Drawing.Point(4, 4);
            this.textEditorControlRegExp.Margin = new System.Windows.Forms.Padding(4);
            this.textEditorControlRegExp.Name = "textEditorControlRegExp";
            this.textEditorControlRegExp.ShowEOLMarkers = true;
            this.textEditorControlRegExp.ShowSpaces = true;
            this.textEditorControlRegExp.ShowTabs = true;
            this.textEditorControlRegExp.Size = new System.Drawing.Size(837, 113);
            this.textEditorControlRegExp.TabIndex = 0;
            this.textEditorControlRegExp.TextChanged += new System.EventHandler(this.textEditorControlRegExp_TextChanged);
            // 
            // panelSearchButtons
            // 
            this.panelSearchButtons.Controls.Add(this.checkBoxFindAll);
            this.panelSearchButtons.Controls.Add(this.checkBoxFindRegExp);
            this.panelSearchButtons.Controls.Add(this.checkBoxFindCaseSensitive);
            this.panelSearchButtons.Controls.Add(this.buttonSearch);
            this.panelSearchButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSearchButtons.Location = new System.Drawing.Point(4, 169);
            this.panelSearchButtons.Margin = new System.Windows.Forms.Padding(0);
            this.panelSearchButtons.Name = "panelSearchButtons";
            this.panelSearchButtons.Size = new System.Drawing.Size(853, 28);
            this.panelSearchButtons.TabIndex = 123;
            // 
            // checkBoxFindAll
            // 
            this.checkBoxFindAll.AutoSize = true;
            this.checkBoxFindAll.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkBoxFindAll.Location = new System.Drawing.Point(320, 0);
            this.checkBoxFindAll.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxFindAll.Name = "checkBoxFindAll";
            this.checkBoxFindAll.Size = new System.Drawing.Size(146, 28);
            this.checkBoxFindAll.TabIndex = 1226;
            this.checkBoxFindAll.Text = "satisfies all criteria";
            this.checkBoxFindAll.UseVisualStyleBackColor = true;
            // 
            // checkBoxFindRegExp
            // 
            this.checkBoxFindRegExp.AutoSize = true;
            this.checkBoxFindRegExp.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkBoxFindRegExp.Location = new System.Drawing.Point(150, 0);
            this.checkBoxFindRegExp.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxFindRegExp.Name = "checkBoxFindRegExp";
            this.checkBoxFindRegExp.Size = new System.Drawing.Size(170, 28);
            this.checkBoxFindRegExp.TabIndex = 1225;
            this.checkBoxFindRegExp.Text = ".* (regular expression)";
            this.checkBoxFindRegExp.UseVisualStyleBackColor = true;
            this.checkBoxFindRegExp.CheckedChanged += new System.EventHandler(this.checkBoxFindRegExp_CheckedChanged);
            // 
            // checkBoxFindCaseSensitive
            // 
            this.checkBoxFindCaseSensitive.AutoSize = true;
            this.checkBoxFindCaseSensitive.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkBoxFindCaseSensitive.Location = new System.Drawing.Point(0, 0);
            this.checkBoxFindCaseSensitive.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxFindCaseSensitive.Name = "checkBoxFindCaseSensitive";
            this.checkBoxFindCaseSensitive.Size = new System.Drawing.Size(150, 28);
            this.checkBoxFindCaseSensitive.TabIndex = 1224;
            this.checkBoxFindCaseSensitive.Text = "Aa (case sensitive)";
            this.checkBoxFindCaseSensitive.UseVisualStyleBackColor = true;
            this.checkBoxFindCaseSensitive.CheckedChanged += new System.EventHandler(this.checkBoxFindCaseSensitive_CheckedChanged);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSearch.Enabled = false;
            this.buttonSearch.Location = new System.Drawing.Point(666, 0);
            this.buttonSearch.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(187, 28);
            this.buttonSearch.TabIndex = 1223;
            this.buttonSearch.Text = "Find";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.ButtonFilterClick);
            // 
            // groupBoxConnect
            // 
            this.groupBoxConnect.Controls.Add(this.buttonExpand);
            this.groupBoxConnect.Controls.Add(this.buttonConnect);
            this.groupBoxConnect.Controls.Add(this.checkBoxWinAuth);
            this.groupBoxConnect.Controls.Add(this.textBoxPassword);
            this.groupBoxConnect.Controls.Add(this.textBoxLogin);
            this.groupBoxConnect.Controls.Add(this.textBoxDB);
            this.groupBoxConnect.Controls.Add(this.textBoxSQLServer);
            this.groupBoxConnect.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBoxConnect.Location = new System.Drawing.Point(0, 0);
            this.groupBoxConnect.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxConnect.Name = "groupBoxConnect";
            this.groupBoxConnect.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxConnect.Size = new System.Drawing.Size(195, 201);
            this.groupBoxConnect.TabIndex = 11;
            this.groupBoxConnect.TabStop = false;
            this.groupBoxConnect.Text = "Connect";
            // 
            // buttonExpand
            // 
            this.buttonExpand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonExpand.Location = new System.Drawing.Point(4, 165);
            this.buttonExpand.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExpand.Name = "buttonExpand";
            this.buttonExpand.Size = new System.Drawing.Size(187, 32);
            this.buttonExpand.TabIndex = 1224;
            this.buttonExpand.Text = "Expand Tree";
            this.buttonExpand.UseVisualStyleBackColor = true;
            this.buttonExpand.Click += new System.EventHandler(this.buttonExpand_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonConnect.Location = new System.Drawing.Point(4, 137);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(4);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(187, 28);
            this.buttonConnect.TabIndex = 116;
            this.buttonConnect.Text = "Connect and Load ISBL";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.ButtonConnectClick);
            // 
            // checkBoxWinAuth
            // 
            this.checkBoxWinAuth.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxWinAuth.Location = new System.Drawing.Point(4, 107);
            this.checkBoxWinAuth.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxWinAuth.Name = "checkBoxWinAuth";
            this.checkBoxWinAuth.Size = new System.Drawing.Size(187, 30);
            this.checkBoxWinAuth.TabIndex = 115;
            this.checkBoxWinAuth.Text = "Windows authentication";
            this.checkBoxWinAuth.UseVisualStyleBackColor = true;
            this.checkBoxWinAuth.CheckedChanged += new System.EventHandler(this.CheckBoxWinAuthCheckedChanged);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxPassword.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxPassword.Location = new System.Drawing.Point(4, 85);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(187, 22);
            this.textBoxPassword.TabIndex = 114;
            this.textBoxPassword.Text = "**************";
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.TextBoxLoginFormTextChanged);
            this.textBoxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxLoginFormKeyDown);
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.textBoxLogin.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxLogin.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxLogin.HideSelection = false;
            this.textBoxLogin.Location = new System.Drawing.Point(4, 63);
            this.textBoxLogin.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(187, 22);
            this.textBoxLogin.TabIndex = 113;
            this.textBoxLogin.Text = "Login";
            this.textBoxLogin.TextChanged += new System.EventHandler(this.TextBoxLoginFormTextChanged);
            this.textBoxLogin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxLoginFormKeyDown);
            // 
            // textBoxDB
            // 
            this.textBoxDB.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.textBoxDB.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxDB.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDB.Location = new System.Drawing.Point(4, 41);
            this.textBoxDB.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxDB.Name = "textBoxDB";
            this.textBoxDB.Size = new System.Drawing.Size(187, 22);
            this.textBoxDB.TabIndex = 112;
            this.textBoxDB.Text = "Data Base";
            this.textBoxDB.TextChanged += new System.EventHandler(this.TextBoxLoginFormTextChanged);
            this.textBoxDB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxLoginFormKeyDown);
            // 
            // textBoxSQLServer
            // 
            this.textBoxSQLServer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.textBoxSQLServer.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxSQLServer.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxSQLServer.Location = new System.Drawing.Point(4, 19);
            this.textBoxSQLServer.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxSQLServer.Name = "textBoxSQLServer";
            this.textBoxSQLServer.Size = new System.Drawing.Size(187, 22);
            this.textBoxSQLServer.TabIndex = 111;
            this.textBoxSQLServer.Text = "Sql Server";
            this.textBoxSQLServer.TextChanged += new System.EventHandler(this.TextBoxLoginFormTextChanged);
            this.textBoxSQLServer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxLoginFormKeyDown);
            // 
            // panelResults
            // 
            this.panelResults.Controls.Add(this.groupBoxResults);
            this.panelResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelResults.Location = new System.Drawing.Point(0, 0);
            this.panelResults.Margin = new System.Windows.Forms.Padding(4);
            this.panelResults.Name = "panelResults";
            this.panelResults.Size = new System.Drawing.Size(1056, 552);
            this.panelResults.TabIndex = 2;
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.splitContainerResults);
            this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxResults.Location = new System.Drawing.Point(0, 0);
            this.groupBoxResults.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxResults.Size = new System.Drawing.Size(1056, 552);
            this.groupBoxResults.TabIndex = 0;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Results";
            // 
            // splitContainerResults
            // 
            this.splitContainerResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerResults.Location = new System.Drawing.Point(4, 19);
            this.splitContainerResults.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainerResults.Name = "splitContainerResults";
            // 
            // splitContainerResults.Panel1
            // 
            this.splitContainerResults.Panel1.Controls.Add(this.panelTree);
            this.splitContainerResults.Panel1.Controls.Add(this.panelFilterTree);
            this.splitContainerResults.Panel1MinSize = 0;
            // 
            // splitContainerResults.Panel2
            // 
            this.splitContainerResults.Panel2.Controls.Add(this.panelISBLResult);
            this.splitContainerResults.Size = new System.Drawing.Size(1048, 529);
            this.splitContainerResults.SplitterDistance = 280;
            this.splitContainerResults.SplitterWidth = 9;
            this.splitContainerResults.TabIndex = 0;
            // 
            // panelTree
            // 
            this.panelTree.Controls.Add(this.treeViewResults);
            this.panelTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTree.Location = new System.Drawing.Point(0, 25);
            this.panelTree.Margin = new System.Windows.Forms.Padding(4);
            this.panelTree.Name = "panelTree";
            this.panelTree.Size = new System.Drawing.Size(280, 504);
            this.panelTree.TabIndex = 212;
            // 
            // treeViewResults
            // 
            this.treeViewResults.CheckBoxes = true;
            this.treeViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewResults.HideSelection = false;
            this.treeViewResults.Location = new System.Drawing.Point(0, 0);
            this.treeViewResults.Margin = new System.Windows.Forms.Padding(4);
            this.treeViewResults.Name = "treeViewResults";
            this.treeViewResults.Size = new System.Drawing.Size(280, 504);
            this.treeViewResults.TabIndex = 2121;
            this.treeViewResults.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewResultsAfterSelect);
            // 
            // panelFilterTree
            // 
            this.panelFilterTree.Controls.Add(this.textBoxFilter);
            this.panelFilterTree.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilterTree.Location = new System.Drawing.Point(0, 0);
            this.panelFilterTree.Margin = new System.Windows.Forms.Padding(4);
            this.panelFilterTree.Name = "panelFilterTree";
            this.panelFilterTree.Size = new System.Drawing.Size(280, 25);
            this.panelFilterTree.TabIndex = 211;
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFilter.Location = new System.Drawing.Point(0, 0);
            this.textBoxFilter.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(280, 22);
            this.textBoxFilter.TabIndex = 2111;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.TextBoxFilter_TextChanged);
            // 
            // panelISBLResult
            // 
            this.panelISBLResult.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelISBLResult.Controls.Add(this.textEditorControlISBL);
            this.panelISBLResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelISBLResult.Location = new System.Drawing.Point(0, 0);
            this.panelISBLResult.Margin = new System.Windows.Forms.Padding(4);
            this.panelISBLResult.Name = "panelISBLResult";
            this.panelISBLResult.Size = new System.Drawing.Size(759, 529);
            this.panelISBLResult.TabIndex = 220;
            // 
            // textEditorControlISBL
            // 
            this.textEditorControlISBL.BracketMatchingStyle = ICSharpCode.TextEditor.Document.BracketMatchingStyle.Before;
            this.textEditorControlISBL.ConvertTabsToSpaces = true;
            this.textEditorControlISBL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorControlISBL.EnableFolding = false;
            this.textEditorControlISBL.IsIconBarVisible = true;
            this.textEditorControlISBL.IsReadOnly = false;
            this.textEditorControlISBL.Location = new System.Drawing.Point(0, 0);
            this.textEditorControlISBL.Margin = new System.Windows.Forms.Padding(4);
            this.textEditorControlISBL.Name = "textEditorControlISBL";
            this.textEditorControlISBL.ShowEOLMarkers = true;
            this.textEditorControlISBL.ShowSpaces = true;
            this.textEditorControlISBL.ShowTabs = true;
            this.textEditorControlISBL.Size = new System.Drawing.Size(759, 529);
            this.textEditorControlISBL.TabIndex = 0;
            this.textEditorControlISBL.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.textEditorControlISBL.VRulerRow = 100;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.panelOptions);
            this.splitContainerMain.Panel1MinSize = 146;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.panelResults);
            this.splitContainerMain.Size = new System.Drawing.Size(1056, 762);
            this.splitContainerMain.SplitterDistance = 201;
            this.splitContainerMain.SplitterWidth = 9;
            this.splitContainerMain.TabIndex = 3;
            // 
            // timerRegExpFind
            // 
            this.timerRegExpFind.Interval = 500;
            this.timerRegExpFind.Tick += new System.EventHandler(this.timerRegExpFind_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelLastUpd,
            this.toolStripStatusLabelDash,
            this.toolStripStatusLabelSelectedElement,
            this.toolStripStatusLabelTest});
            this.statusStrip1.Location = new System.Drawing.Point(0, 762);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1056, 25);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelLastUpd
            // 
            this.toolStripStatusLabelLastUpd.Name = "toolStripStatusLabelLastUpd";
            this.toolStripStatusLabelLastUpd.Size = new System.Drawing.Size(137, 20);
            this.toolStripStatusLabelLastUpd.Text = "00.00.0000 00:00:00";
            // 
            // toolStripStatusLabelDash
            // 
            this.toolStripStatusLabelDash.Name = "toolStripStatusLabelDash";
            this.toolStripStatusLabelDash.Size = new System.Drawing.Size(15, 20);
            this.toolStripStatusLabelDash.Text = "-";
            // 
            // toolStripStatusLabelSelectedElement
            // 
            this.toolStripStatusLabelSelectedElement.Name = "toolStripStatusLabelSelectedElement";
            this.toolStripStatusLabelSelectedElement.Size = new System.Drawing.Size(45, 20);
            this.toolStripStatusLabelSelectedElement.Text = "None";
            // 
            // toolStripStatusLabelTest
            // 
            this.toolStripStatusLabelTest.Name = "toolStripStatusLabelTest";
            this.toolStripStatusLabelTest.Size = new System.Drawing.Size(0, 20);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.splitContainerMain);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(4);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1056, 762);
            this.panelMain.TabIndex = 7;
            // 
            // backgroundWorkerFind
            // 
            this.backgroundWorkerFind.WorkerReportsProgress = true;
            this.backgroundWorkerFind.WorkerSupportsCancellation = true;
            this.backgroundWorkerFind.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerFind_DoWork);
            this.backgroundWorkerFind.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerFind_ProgressChanged);
            this.backgroundWorkerFind.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerFind_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 787);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.statusStrip1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "ISBLScan ViewCode";
            this.panelOptions.ResumeLayout(false);
            this.groupBoxSearch.ResumeLayout(false);
            this.panelSearchText.ResumeLayout(false);
            this.tabControlSarchText.ResumeLayout(false);
            this.tabPageSimpleSearchText.ResumeLayout(false);
            this.tabPageRegularExpression.ResumeLayout(false);
            this.panelSearchButtons.ResumeLayout(false);
            this.panelSearchButtons.PerformLayout();
            this.groupBoxConnect.ResumeLayout(false);
            this.groupBoxConnect.PerformLayout();
            this.panelResults.ResumeLayout(false);
            this.groupBoxResults.ResumeLayout(false);
            this.splitContainerResults.Panel1.ResumeLayout(false);
            this.splitContainerResults.Panel2.ResumeLayout(false);
            this.splitContainerResults.ResumeLayout(false);
            this.panelTree.ResumeLayout(false);
            this.panelFilterTree.ResumeLayout(false);
            this.panelFilterTree.PerformLayout();
            this.panelISBLResult.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private ICSharpCode.TextEditor.TextEditorControl textEditorControlISBL;
		private System.Windows.Forms.CheckBox checkBoxWinAuth;
        private System.Windows.Forms.Button buttonConnect;

		private System.Windows.Forms.TextBox textBoxFilter;
		private System.Windows.Forms.Panel panelFilterTree;
		private System.Windows.Forms.Panel panelTree;
		private System.Windows.Forms.Panel panelResults;
        private System.Windows.Forms.Panel panelOptions;
		private System.Windows.Forms.TreeView treeViewResults;
		private System.Windows.Forms.SplitContainer splitContainerResults;
		private System.Windows.Forms.GroupBox groupBoxResults;
		private System.Windows.Forms.TextBox textBoxSQLServer;
		private System.Windows.Forms.TextBox textBoxDB;
		private System.Windows.Forms.TextBox textBoxLogin;
		private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.GroupBox groupBoxConnect;
		private System.Windows.Forms.GroupBox groupBoxSearch;
        private System.Windows.Forms.Panel panelISBLResult;
        private System.Windows.Forms.Panel panelSearchButtons;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Panel panelSearchText;
        private System.Windows.Forms.TabControl tabControlSarchText;
        private System.Windows.Forms.TabPage tabPageSimpleSearchText;
        private System.Windows.Forms.TabPage tabPageRegularExpression;
        private ICSharpCode.TextEditor.TextEditorControl textEditorControlRegExp;
        private System.Windows.Forms.CheckBox checkBoxFindCaseSensitive;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Timer timerRegExpFind;
        private System.Windows.Forms.Button buttonCloseCurrentTab;
        public System.Windows.Forms.ImageList imageListIcons;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonAddNewTab;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLastUpd;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDash;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSelectedElement;
        private ICSharpCode.TextEditor.TextEditorControl textEditorControlSearchText;
        private System.Windows.Forms.CheckBox checkBoxFindRegExp;
        private System.ComponentModel.BackgroundWorker backgroundWorkerFind;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTest;
        private System.Windows.Forms.CheckBox checkBoxFindAll;
        private System.Windows.Forms.Button buttonExpand;
        //Main menu


    }
}
