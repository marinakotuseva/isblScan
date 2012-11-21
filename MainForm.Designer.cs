namespace isblTest
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
			this.groupBoxOptions = new System.Windows.Forms.GroupBox();
			this.groupBoxFilter = new System.Windows.Forms.GroupBox();
			this.textBoxFilter = new System.Windows.Forms.TextBox();
			this.groupBoxActions = new System.Windows.Forms.GroupBox();
			this.buttonFilter = new System.Windows.Forms.Button();
			this.groupBoxConnect = new System.Windows.Forms.GroupBox();
			this.buttonConnect = new System.Windows.Forms.Button();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.textBoxLogin = new System.Windows.Forms.TextBox();
			this.textBoxDB = new System.Windows.Forms.TextBox();
			this.textBoxSQLServer = new System.Windows.Forms.TextBox();
			this.groupBoxResults = new System.Windows.Forms.GroupBox();
			this.splitContainerResults = new System.Windows.Forms.SplitContainer();
			this.treeViewResults = new System.Windows.Forms.TreeView();
			this.richTextBoxResult = new System.Windows.Forms.RichTextBox();
			this.groupBoxOptions.SuspendLayout();
			this.groupBoxFilter.SuspendLayout();
			this.groupBoxActions.SuspendLayout();
			this.groupBoxConnect.SuspendLayout();
			this.groupBoxResults.SuspendLayout();
			this.splitContainerResults.Panel1.SuspendLayout();
			this.splitContainerResults.Panel2.SuspendLayout();
			this.splitContainerResults.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxOptions
			// 
			this.groupBoxOptions.Controls.Add(this.groupBoxFilter);
			this.groupBoxOptions.Controls.Add(this.groupBoxConnect);
			this.groupBoxOptions.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBoxOptions.Location = new System.Drawing.Point(0, 0);
			this.groupBoxOptions.Name = "groupBoxOptions";
			this.groupBoxOptions.Size = new System.Drawing.Size(440, 145);
			this.groupBoxOptions.TabIndex = 3;
			this.groupBoxOptions.TabStop = false;
			this.groupBoxOptions.Text = "Options";
			// 
			// groupBoxFilter
			// 
			this.groupBoxFilter.Controls.Add(this.textBoxFilter);
			this.groupBoxFilter.Controls.Add(this.groupBoxActions);
			this.groupBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxFilter.Location = new System.Drawing.Point(147, 16);
			this.groupBoxFilter.Name = "groupBoxFilter";
			this.groupBoxFilter.Size = new System.Drawing.Size(290, 126);
			this.groupBoxFilter.TabIndex = 2;
			this.groupBoxFilter.TabStop = false;
			this.groupBoxFilter.Text = "Filter";
			// 
			// textBoxFilter
			// 
			this.textBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxFilter.Location = new System.Drawing.Point(3, 16);
			this.textBoxFilter.Multiline = true;
			this.textBoxFilter.Name = "textBoxFilter";
			this.textBoxFilter.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxFilter.Size = new System.Drawing.Size(225, 107);
			this.textBoxFilter.TabIndex = 4;
			this.textBoxFilter.Text = @"AddWhere
AddFrom
AddOrderBy
AddSelect
SQL(
CSQL(
Выполнить(
Execute
Вызвать(
exec(
drop
grant
ExecutePrecess(
CommandText
CallProcedure(
Call
Delete
CreateObject
Shell
ADODB
Password
Encrypt
Crypt
Decrypt
Sign
Access
.AccessRights
РеестрЗапись
GetObject
Secret
CAPICOM
ENCODE
Пароль
Логин
Login
Authentication";
			// 
			// groupBoxActions
			// 
			this.groupBoxActions.Controls.Add(this.buttonFilter);
			this.groupBoxActions.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBoxActions.Location = new System.Drawing.Point(228, 16);
			this.groupBoxActions.Name = "groupBoxActions";
			this.groupBoxActions.Size = new System.Drawing.Size(59, 107);
			this.groupBoxActions.TabIndex = 3;
			this.groupBoxActions.TabStop = false;
			this.groupBoxActions.Text = "Actions";
			// 
			// buttonFilter
			// 
			this.buttonFilter.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonFilter.Location = new System.Drawing.Point(3, 16);
			this.buttonFilter.Name = "buttonFilter";
			this.buttonFilter.Size = new System.Drawing.Size(53, 23);
			this.buttonFilter.TabIndex = 1;
			this.buttonFilter.Text = "Filter";
			this.buttonFilter.UseVisualStyleBackColor = true;
			this.buttonFilter.Click += new System.EventHandler(this.ButtonFilterClick);
			// 
			// groupBoxConnect
			// 
			this.groupBoxConnect.Controls.Add(this.buttonConnect);
			this.groupBoxConnect.Controls.Add(this.textBoxPassword);
			this.groupBoxConnect.Controls.Add(this.textBoxLogin);
			this.groupBoxConnect.Controls.Add(this.textBoxDB);
			this.groupBoxConnect.Controls.Add(this.textBoxSQLServer);
			this.groupBoxConnect.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBoxConnect.Location = new System.Drawing.Point(3, 16);
			this.groupBoxConnect.Name = "groupBoxConnect";
			this.groupBoxConnect.Size = new System.Drawing.Size(144, 126);
			this.groupBoxConnect.TabIndex = 1;
			this.groupBoxConnect.TabStop = false;
			this.groupBoxConnect.Text = "Connect";
			// 
			// buttonConnect
			// 
			this.buttonConnect.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonConnect.Location = new System.Drawing.Point(3, 96);
			this.buttonConnect.Margin = new System.Windows.Forms.Padding(8);
			this.buttonConnect.Name = "buttonConnect";
			this.buttonConnect.Size = new System.Drawing.Size(138, 26);
			this.buttonConnect.TabIndex = 4;
			this.buttonConnect.Text = "Get ISBL";
			this.buttonConnect.UseVisualStyleBackColor = true;
			this.buttonConnect.Click += new System.EventHandler(this.ButtonConnectClick);
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Dock = System.Windows.Forms.DockStyle.Top;
			this.textBoxPassword.Location = new System.Drawing.Point(3, 76);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(138, 20);
			this.textBoxPassword.TabIndex = 3;
			this.textBoxPassword.UseSystemPasswordChar = true;
			this.textBoxPassword.Text = "**************";
			// 
			// textBoxLogin
			// 
			this.textBoxLogin.Dock = System.Windows.Forms.DockStyle.Top;
			this.textBoxLogin.Location = new System.Drawing.Point(3, 56);
			this.textBoxLogin.Name = "textBoxLogin";
			this.textBoxLogin.Size = new System.Drawing.Size(138, 20);
			this.textBoxLogin.TabIndex = 2;
			this.textBoxLogin.Text = "login";
			// 
			// textBoxDB
			// 
			this.textBoxDB.Dock = System.Windows.Forms.DockStyle.Top;
			this.textBoxDB.Location = new System.Drawing.Point(3, 36);
			this.textBoxDB.Name = "textBoxDB";
			this.textBoxDB.Size = new System.Drawing.Size(138, 20);
			this.textBoxDB.TabIndex = 1;
			this.textBoxDB.Text = "DataBase";
			// 
			// textBoxSQLServer
			// 
			this.textBoxSQLServer.Dock = System.Windows.Forms.DockStyle.Top;
			this.textBoxSQLServer.Location = new System.Drawing.Point(3, 16);
			this.textBoxSQLServer.Name = "textBoxSQLServer";
			this.textBoxSQLServer.Size = new System.Drawing.Size(138, 20);
			this.textBoxSQLServer.TabIndex = 0;
			this.textBoxSQLServer.Text = "sql Server";
			// 
			// groupBoxResults
			// 
			this.groupBoxResults.Controls.Add(this.splitContainerResults);
			this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxResults.Location = new System.Drawing.Point(0, 145);
			this.groupBoxResults.Name = "groupBoxResults";
			this.groupBoxResults.Size = new System.Drawing.Size(440, 253);
			this.groupBoxResults.TabIndex = 4;
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
			this.splitContainerResults.Panel1.Controls.Add(this.treeViewResults);
			// 
			// splitContainerResults.Panel2
			// 
			this.splitContainerResults.Panel2.Controls.Add(this.richTextBoxResult);
			this.splitContainerResults.Size = new System.Drawing.Size(434, 234);
			this.splitContainerResults.SplitterDistance = 144;
			this.splitContainerResults.TabIndex = 2;
			// 
			// treeViewResults
			// 
			this.treeViewResults.CheckBoxes = true;
			this.treeViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewResults.HideSelection = false;
			this.treeViewResults.Location = new System.Drawing.Point(0, 0);
			this.treeViewResults.Name = "treeViewResults";
			this.treeViewResults.Size = new System.Drawing.Size(144, 234);
			this.treeViewResults.TabIndex = 0;
			this.treeViewResults.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewResultsAfterSelect);
			// 
			// richTextBoxResult
			// 
			this.richTextBoxResult.AcceptsTab = true;
			this.richTextBoxResult.AutoWordSelection = true;
			this.richTextBoxResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBoxResult.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.richTextBoxResult.Location = new System.Drawing.Point(0, 0);
			this.richTextBoxResult.Name = "richTextBoxResult";
			this.richTextBoxResult.Size = new System.Drawing.Size(286, 234);
			this.richTextBoxResult.TabIndex = 2;
			this.richTextBoxResult.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(440, 398);
			this.Controls.Add(this.groupBoxResults);
			this.Controls.Add(this.groupBoxOptions);
			this.Name = "MainForm";
			this.Text = "isblTest";
			this.groupBoxOptions.ResumeLayout(false);
			this.groupBoxFilter.ResumeLayout(false);
			this.groupBoxFilter.PerformLayout();
			this.groupBoxActions.ResumeLayout(false);
			this.groupBoxConnect.ResumeLayout(false);
			this.groupBoxConnect.PerformLayout();
			this.groupBoxResults.ResumeLayout(false);
			this.splitContainerResults.Panel1.ResumeLayout(false);
			this.splitContainerResults.Panel2.ResumeLayout(false);
			this.splitContainerResults.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button buttonFilter;
		private System.Windows.Forms.GroupBox groupBoxActions;
		private System.Windows.Forms.RichTextBox richTextBoxResult;
		private System.Windows.Forms.TreeView treeViewResults;
		private System.Windows.Forms.SplitContainer splitContainerResults;
		private System.Windows.Forms.GroupBox groupBoxResults;
		private System.Windows.Forms.TextBox textBoxSQLServer;
		private System.Windows.Forms.TextBox textBoxDB;
		private System.Windows.Forms.TextBox textBoxLogin;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Button buttonConnect;
		private System.Windows.Forms.GroupBox groupBoxConnect;
		private System.Windows.Forms.TextBox textBoxFilter;
		private System.Windows.Forms.GroupBox groupBoxFilter;
		private System.Windows.Forms.GroupBox groupBoxOptions;
	}
}
