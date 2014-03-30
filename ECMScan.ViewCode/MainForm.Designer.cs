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
			this.panelOptions = new System.Windows.Forms.Panel();
			this.groupBoxSearch = new System.Windows.Forms.GroupBox();
			this.textBoxSearch = new System.Windows.Forms.TextBox();
			this.groupBoxConnect = new System.Windows.Forms.GroupBox();
			this.buttonSearch = new System.Windows.Forms.Button();
			this.buttonRefresh = new System.Windows.Forms.Button();
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
			this.panelOptions.SuspendLayout();
			this.groupBoxSearch.SuspendLayout();
			this.groupBoxConnect.SuspendLayout();
			this.panelResults.SuspendLayout();
			this.groupBoxResults.SuspendLayout();
			this.splitContainerResults.Panel1.SuspendLayout();
			this.splitContainerResults.Panel2.SuspendLayout();
			this.splitContainerResults.SuspendLayout();
			this.panelTree.SuspendLayout();
			this.panelFilterTree.SuspendLayout();
			this.panelISBLResult.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelOptions
			// 
			this.panelOptions.Controls.Add(this.groupBoxSearch);
			this.panelOptions.Controls.Add(this.groupBoxConnect);
			this.panelOptions.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelOptions.Location = new System.Drawing.Point(0, 0);
			this.panelOptions.Name = "panelOptions";
			this.panelOptions.Size = new System.Drawing.Size(632, 202);
			this.panelOptions.TabIndex = 1;
			// 
			// groupBoxSearch
			// 
			this.groupBoxSearch.Controls.Add(this.textBoxSearch);
			this.groupBoxSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxSearch.Location = new System.Drawing.Point(146, 0);
			this.groupBoxSearch.Name = "groupBoxSearch";
			this.groupBoxSearch.Size = new System.Drawing.Size(486, 202);
			this.groupBoxSearch.TabIndex = 12;
			this.groupBoxSearch.TabStop = false;
			this.groupBoxSearch.Text = "Search Text";
			// 
			// textBoxSearch
			// 
			this.textBoxSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxSearch.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.textBoxSearch.Location = new System.Drawing.Point(3, 16);
			this.textBoxSearch.Multiline = true;
			this.textBoxSearch.Name = "textBoxSearch";
			this.textBoxSearch.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxSearch.Size = new System.Drawing.Size(480, 183);
			this.textBoxSearch.TabIndex = 121;
			// 
			// groupBoxConnect
			// 
			this.groupBoxConnect.Controls.Add(this.buttonSearch);
			this.groupBoxConnect.Controls.Add(this.buttonRefresh);
			this.groupBoxConnect.Controls.Add(this.buttonConnect);
			this.groupBoxConnect.Controls.Add(this.checkBoxWinAuth);
			this.groupBoxConnect.Controls.Add(this.textBoxPassword);
			this.groupBoxConnect.Controls.Add(this.textBoxLogin);
			this.groupBoxConnect.Controls.Add(this.textBoxDB);
			this.groupBoxConnect.Controls.Add(this.textBoxSQLServer);
			this.groupBoxConnect.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBoxConnect.Location = new System.Drawing.Point(0, 0);
			this.groupBoxConnect.Name = "groupBoxConnect";
			this.groupBoxConnect.Size = new System.Drawing.Size(146, 202);
			this.groupBoxConnect.TabIndex = 11;
			this.groupBoxConnect.TabStop = false;
			this.groupBoxConnect.Text = "Connect";
			// 
			// buttonSearch
			// 
			this.buttonSearch.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonSearch.Enabled = false;
			this.buttonSearch.Location = new System.Drawing.Point(3, 166);
			this.buttonSearch.Name = "buttonSearch";
			this.buttonSearch.Size = new System.Drawing.Size(140, 23);
			this.buttonSearch.TabIndex = 1222;
			this.buttonSearch.Text = "Search";
			this.buttonSearch.UseVisualStyleBackColor = true;
			this.buttonSearch.Click += new System.EventHandler(this.ButtonFilterClick);
			// 
			// buttonRefresh
			// 
			this.buttonRefresh.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonRefresh.Enabled = false;
			this.buttonRefresh.Location = new System.Drawing.Point(3, 143);
			this.buttonRefresh.Name = "buttonRefresh";
			this.buttonRefresh.Size = new System.Drawing.Size(140, 23);
			this.buttonRefresh.TabIndex = 117;
			this.buttonRefresh.Text = "Reload ISBL";
			this.buttonRefresh.UseVisualStyleBackColor = true;
			this.buttonRefresh.Click += new System.EventHandler(this.ButtonRefreshClick);
			// 
			// buttonConnect
			// 
			this.buttonConnect.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonConnect.Location = new System.Drawing.Point(3, 120);
			this.buttonConnect.Name = "buttonConnect";
			this.buttonConnect.Size = new System.Drawing.Size(140, 23);
			this.buttonConnect.TabIndex = 116;
			this.buttonConnect.Text = "Connect and Load ISBL";
			this.buttonConnect.UseVisualStyleBackColor = true;
			this.buttonConnect.Click += new System.EventHandler(this.ButtonConnectClick);
			// 
			// checkBoxWinAuth
			// 
			this.checkBoxWinAuth.Dock = System.Windows.Forms.DockStyle.Top;
			this.checkBoxWinAuth.Location = new System.Drawing.Point(3, 96);
			this.checkBoxWinAuth.Name = "checkBoxWinAuth";
			this.checkBoxWinAuth.Size = new System.Drawing.Size(140, 24);
			this.checkBoxWinAuth.TabIndex = 115;
			this.checkBoxWinAuth.Text = "Windows authentication";
			this.checkBoxWinAuth.UseVisualStyleBackColor = true;
			this.checkBoxWinAuth.CheckedChanged += new System.EventHandler(this.CheckBoxWinAuthCheckedChanged);
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Dock = System.Windows.Forms.DockStyle.Top;
			this.textBoxPassword.ForeColor = System.Drawing.SystemColors.GrayText;
			this.textBoxPassword.Location = new System.Drawing.Point(3, 76);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(140, 20);
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
			this.textBoxLogin.Location = new System.Drawing.Point(3, 56);
			this.textBoxLogin.Name = "textBoxLogin";
			this.textBoxLogin.Size = new System.Drawing.Size(140, 20);
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
			this.textBoxDB.Location = new System.Drawing.Point(3, 36);
			this.textBoxDB.Name = "textBoxDB";
			this.textBoxDB.Size = new System.Drawing.Size(140, 20);
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
			this.textBoxSQLServer.Location = new System.Drawing.Point(3, 16);
			this.textBoxSQLServer.Name = "textBoxSQLServer";
			this.textBoxSQLServer.Size = new System.Drawing.Size(140, 20);
			this.textBoxSQLServer.TabIndex = 111;
			this.textBoxSQLServer.Text = "Sql Server";
			this.textBoxSQLServer.TextChanged += new System.EventHandler(this.TextBoxLoginFormTextChanged);
			this.textBoxSQLServer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxLoginFormKeyDown);
			// 
			// panelResults
			// 
			this.panelResults.Controls.Add(this.groupBoxResults);
			this.panelResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelResults.Location = new System.Drawing.Point(0, 202);
			this.panelResults.Name = "panelResults";
			this.panelResults.Size = new System.Drawing.Size(632, 251);
			this.panelResults.TabIndex = 2;
			// 
			// groupBoxResults
			// 
			this.groupBoxResults.Controls.Add(this.splitContainerResults);
			this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxResults.Location = new System.Drawing.Point(0, 0);
			this.groupBoxResults.Name = "groupBoxResults";
			this.groupBoxResults.Size = new System.Drawing.Size(632, 251);
			this.groupBoxResults.TabIndex = 0;
			this.groupBoxResults.TabStop = false;
			this.groupBoxResults.Text = "Results";
			// 
			// splitContainerResults
			// 
			this.splitContainerResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerResults.Location = new System.Drawing.Point(3, 16);
			this.splitContainerResults.Name = "splitContainerResults";
			// 
			// splitContainerResults.Panel1
			// 
			this.splitContainerResults.Panel1.Controls.Add(this.panelTree);
			this.splitContainerResults.Panel1.Controls.Add(this.panelFilterTree);
			// 
			// splitContainerResults.Panel2
			// 
			this.splitContainerResults.Panel2.Controls.Add(this.panelISBLResult);
			this.splitContainerResults.Size = new System.Drawing.Size(626, 232);
			this.splitContainerResults.SplitterDistance = 207;
			this.splitContainerResults.TabIndex = 0;
			// 
			// panelTree
			// 
			this.panelTree.Controls.Add(this.treeViewResults);
			this.panelTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelTree.Location = new System.Drawing.Point(0, 20);
			this.panelTree.Name = "panelTree";
			this.panelTree.Size = new System.Drawing.Size(207, 212);
			this.panelTree.TabIndex = 212;
			// 
			// treeViewResults
			// 
			this.treeViewResults.CheckBoxes = true;
			this.treeViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewResults.HideSelection = false;
			this.treeViewResults.Location = new System.Drawing.Point(0, 0);
			this.treeViewResults.Name = "treeViewResults";
			this.treeViewResults.Size = new System.Drawing.Size(207, 212);
			this.treeViewResults.TabIndex = 2121;
			this.treeViewResults.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewResultsAfterSelect);
			// 
			// panelFilterTree
			// 
			this.panelFilterTree.Controls.Add(this.textBoxFilter);
			this.panelFilterTree.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelFilterTree.Location = new System.Drawing.Point(0, 0);
			this.panelFilterTree.Name = "panelFilterTree";
			this.panelFilterTree.Size = new System.Drawing.Size(207, 20);
			this.panelFilterTree.TabIndex = 211;
			// 
			// textBoxFilter
			// 
			this.textBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxFilter.Location = new System.Drawing.Point(0, 0);
			this.textBoxFilter.Name = "textBoxFilter";
			this.textBoxFilter.Size = new System.Drawing.Size(207, 20);
			this.textBoxFilter.TabIndex = 2111;
			this.textBoxFilter.TextChanged += new System.EventHandler(this.TextBoxFilter_TextChanged);
			// 
			// panelISBLResult
			// 
			this.panelISBLResult.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelISBLResult.Controls.Add(this.textEditorControlISBL);
			this.panelISBLResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelISBLResult.Location = new System.Drawing.Point(0, 0);
			this.panelISBLResult.Name = "panelISBLResult";
			this.panelISBLResult.Size = new System.Drawing.Size(415, 232);
			this.panelISBLResult.TabIndex = 220;
			// 
			// textEditorControlISBL
			// 
			this.textEditorControlISBL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textEditorControlISBL.IsReadOnly = false;
			this.textEditorControlISBL.LineViewerStyle = ICSharpCode.TextEditor.Document.LineViewerStyle.FullRow;
			this.textEditorControlISBL.Location = new System.Drawing.Point(0, 0);
			this.textEditorControlISBL.Name = "textEditorControlISBL";
			this.textEditorControlISBL.ShowEOLMarkers = true;
			this.textEditorControlISBL.ShowSpaces = true;
			this.textEditorControlISBL.ShowTabs = true;
			this.textEditorControlISBL.ShowVRuler = false;
			this.textEditorControlISBL.Size = new System.Drawing.Size(415, 232);
			this.textEditorControlISBL.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 453);
			this.Controls.Add(this.panelResults);
			this.Controls.Add(this.panelOptions);
			this.HelpButton = true;
			this.Name = "MainForm";
			this.Text = "ISBLScan ViewCode";
			this.panelOptions.ResumeLayout(false);
			this.groupBoxSearch.ResumeLayout(false);
			this.groupBoxSearch.PerformLayout();
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
			this.ResumeLayout(false);
		}
		private ICSharpCode.TextEditor.TextEditorControl textEditorControlISBL;
		private System.Windows.Forms.CheckBox checkBoxWinAuth;
		private System.Windows.Forms.Button buttonConnect;
		private System.Windows.Forms.Button buttonRefresh;

		private System.Windows.Forms.TextBox textBoxFilter;
		private System.Windows.Forms.Panel panelFilterTree;
		private System.Windows.Forms.Panel panelTree;
		private System.Windows.Forms.Panel panelResults;
		private System.Windows.Forms.Panel panelOptions;
		private System.Windows.Forms.Button buttonSearch;
		private System.Windows.Forms.TreeView treeViewResults;
		private System.Windows.Forms.SplitContainer splitContainerResults;
		private System.Windows.Forms.GroupBox groupBoxResults;
		private System.Windows.Forms.TextBox textBoxSQLServer;
		private System.Windows.Forms.TextBox textBoxDB;
		private System.Windows.Forms.TextBox textBoxLogin;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.GroupBox groupBoxConnect;
		private System.Windows.Forms.TextBox textBoxSearch;
		private System.Windows.Forms.GroupBox groupBoxSearch;
		private System.Windows.Forms.Panel panelISBLResult;
		//Main menu

		
	}
}
