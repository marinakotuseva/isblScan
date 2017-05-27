using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ICSharpCode.TextEditor.Document;

namespace ISBLScan.ViewCode
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        Loader isblLoader;

        /// <summary>
        /// Список корневых узлов элементов разработки (узлы могут быть пустыми, null)
        /// </summary>
		List<Node> SourceISBLNodes { get; set; }

        /// <summary>
        /// Список поисковых фраз по которым выполняется поиск, служебная глобальная переменная
        /// </summary>
        string[] searchStrs;

        class MyTextEditorControl : ICSharpCode.TextEditor.TextEditorControl
        {
            public TreeView TreeViewResults { get; set; } = new TreeView();

            public MyTextEditorControl(MainForm form)
            {
                TreeViewResults.CheckBoxes = false;
                TreeViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
                TreeViewResults.HideSelection = false;
                TreeViewResults.Location = new System.Drawing.Point(0, 0);
                TreeViewResults.Margin = new System.Windows.Forms.Padding(4);
                TreeViewResults.Name = "treeViewResults";
                //TreeViewResults.Size = new System.Drawing.Size(280, 504);
                TreeViewResults.TabIndex = 2121;
                TreeViewResults.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(form.TreeViewResultsAfterSelect);

                form.panelTree.Controls.Add(TreeViewResults);
                form.panelTree.Dock = System.Windows.Forms.DockStyle.Fill;
                form.panelTree.Location = new System.Drawing.Point(0, 25);
                form.panelTree.Margin = new System.Windows.Forms.Padding(4);
                form.panelTree.Name = "panelTree";
                //form.panelTree.Size = new System.Drawing.Size(280, 504);
                form.panelTree.TabIndex = 212;
            }
            
        }
        /// <summary>
        /// Редактор текста с критериями поиска, активный в текущий момент
        /// </summary>
        MyTextEditorControl ActiveSearchStringControl  { get; set; }

        /// <summary>
        /// Список элементов разработки, отфильтрованный по критериям поиска вкладки, активной в текущий момент
        /// </summary>
        List<Node> ActiveISBLNodes
        {
            get { return (List<Node>)ActiveSearchStringControl.Tag; }
            set { ActiveSearchStringControl.Tag = value; }
        }

        /// <summary>
        /// Словарь поисковых фраз и соотвествующих регулярных выражений
        /// </summary>
        Dictionary<string, Regex> dictRegEx = new Dictionary<string, Regex>();

        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            groupBoxSearch_Resize(null, null);
            isblLoader = new Loader();

            string sqlServer;
            string dataBase;
            string login;
            bool isWinAuth;
            if (Configuration.Load(out sqlServer, out dataBase, out login, out isWinAuth))
            {
                textBoxSQLServer.Text = sqlServer;
                textBoxDB.Text = dataBase;
                textBoxLogin.Text = login;
                if (isWinAuth)
                {
                    checkBoxWinAuth.Checked = isWinAuth;
                }
                else
                {
                    textBoxPassword.Text = "";
                }
            }
            //
            // TODO: Добавить хороший список предопределённых строк.
            //
            string[] defaultSearchStrings =
            {
                "AddWhere      AddFrom  AddOrderBy  AddSelect     SQL(     CSQL   drop    xp_",
                "Выполнить     Execute  Exec        CreateObject",
                "Login         Логин    Pass        Пароль        Admin    Админ",
                "AccessRights  Grant    Control     Encrypt       Decrypt  Шифр   Secret  Секрет",
                ""
            };
            foreach (string searchString in defaultSearchStrings)
            {
                this.textEditorControlSearchText.Document.TextContent += (searchString + this.textEditorControlSearchText.Document.TextEditorProperties.LineTerminator);
            }

            //
            // TODO: Добавить хорошую стправку на начальной странице
            //
            ApplicationInfo applicationInfo = new ApplicationInfo();
            //this.richTextBoxResult.Text = string.Format("{0} {1}\n{2}", applicationInfo.ProductName, applicationInfo.Version, applicationInfo.Description);
            //HighLight("", null);

            string str = "Ly8vIElTQkxTY2FuIENvZGVWaWV3Ci8vLyBUb29sIGZvciB2aWV3IHNvdXJjZSBjb2RlIG9mIEVDTSBzeXN0ZW1zIGJhc2VkIG9uIElTLUJ1aWxkZXIgNyAoRElSRUNUVU0sIE9yaWVuZ2UgQ29udGVycmEpIG" +
                    "FuZCBDUE0gc3lzdGVtcyBiYXNlZCBvbiBJUy1CdWlsZGVyIDUgKFByZXN0aW1hKS4KLy8vIFNhbXBsZSBJU0JMIHNvdXJjZSBjb2RlOgoKICBRVUVSWV9GSUVMRFNfREVMSU1JVEVSID0gIiwiCiAgRUxFTUVO" +
                    "VFNfRE9FU19OT1RfRVhJU1QgPSAwCiAgU1RSSU5HX0xFTkdUSCA9IDgwIAogIFNUUklOR19TVEFSVF9JTkRFWCA9IDEKICBCQV9FVkVOVFNfVEFCTEVfTkFNRV9URU1QTEFURSA9ICJCQUV2ZW50cyVzIgogIE" +
                    "5VTExfVkFMVUUgPSAiTlVMTCIgICAgICAKICBUQUJMRV9OQU1FX1JFUExBQ0VfRlJPTV9URVhUID0gIl0iCiAgVEFCTEVfTkFNRV9SRVBMQUNFX1RPX1RFWFQgPSAiXV0iICAKICBCQUV2ZW50c1RhYmxlTmFt" +
                    "ZSA9IEZvcm1hdChCQV9FVkVOVFNfVEFCTEVfTkFNRV9URU1QTEFURTsgU3RhbmRhcmRSb3V0ZUNvZGUpCiAgQkFFdmVudHNUYWJsZU5hbWUgPSBSZXBsYWNlKEJBRXZlbnRzVGFibGVOYW1lOyBUQUJMRV9OQU" +
                    "1FX1JFUExBQ0VfRlJPTV9URVhUOyBUQUJMRV9OQU1FX1JFUExBQ0VfVE9fVEVYVCkKICBpZiBGaWVsZExpc3QuQ291bnQgPiBFTEVNRU5UU19ET0VTX05PVF9FWElTVAogICAgLy8g0J/QvtC70YPRh9C40YLR" +
                    "jCDRiNCw0LHQu9C+0L3RiyDQt9Cw0L/RgNC+0YHQvtCyINC00LvRjyDQstGB0YLQsNCy0LrQuCDQuCDQvtCx0L3QvtCy0LvQtdC90LjRjyDQt9Cw0L/QuNGB0LXQuSDQsiDRgtCw0LHQu9C40YbQtS4g0J7Qsd" +
                    "C90L7QstC70Y/RjtGC0YHRjyDQv9C+0LvRjywg0L/QtdGA0LXQtNCw0L3QvdGL0LUg0LIg0L/QsNGA0LDQvNC10YLRgNC1IEZpZWxkTGlzdAogICAgLy8g0KTQvtGA0LzQsNGCINGB0L/QuNGB0LrQsCBGaWVs" +
                    "ZExpc3Q6CiAgICAvLyBOYW1lIC0g0JjQvNGPINC/0L7Qu9GPCiAgICAvLyBWYWx1ZSAtINCX0L3QsNGH0LXQvdC40LUg0L/QvtC70Y8KICAgIEluZGV4ID0gMAogICAgVXBkYXRlU2V0Rm9yVXBkYXRlUXVlcn" +
                    "kgPSAiIgogICAgRmllbGROYW1lc0Zvckluc2VydFF1ZXJ5ID0gIiIKICAgIEZpZWxkVmFsdWVzRm9ySW5zZXJ0UXVlcnkgPSAiIgogICAgZm9yZWFjaCBGaWVsZFZhbHVlI" +
                    "GluIEZpZWxkTGlzdAogICAgICBGaWVsZE5hbWUgPSBGaWVsZExpc3QuTmFtZXMoSW5kZXgpCiAgICAgIGlmIEFzc2lnbmVkKEZpZWxkVmFsdWUpCiAgICAgICAgRmllbGRWYWx1ZSA9IEZvcm1hdCgiJyVzJyI" +
                    "7IENvcHkoRmllbGRWYWx1ZTsgU1RSSU5HX1NUQVJUX0lOREVYOyBTVFJJTkdfTEVOR1RIKSkKICAgICAgZWxzZQogICAgICAgIEZpZWxkVmFsdWUgPSBOVUxMX1ZBTFVFICAKICAgICAgZW5kaWYKICAgICAgV" +
                    "XBkYXRlU2V0Rm9yVXBkYXRlUXVlcnkgPSBBZGRTdWJTdHJpbmcoRm9ybWF0KCIlcyA9ICVzIjsgQXJyYXlPZihGaWVsZE5hbWU7IEZpZWxkVmFsdWUpKTsgVXBkYXRlU2V0Rm9yVXBkYXRlUXVlcnk7IFFVRVJ" +
                    "ZX0ZJRUxEU19ERUxJTUlURVIpCiAgICAgIEZpZWxkTmFtZXNGb3JJbnNlcnRRdWVyeSA9IEFkZFN1YlN0cmluZyhGaWVsZE5hbWU7IEZpZWxkTmFtZXNGb3JJbnNlcnRRdWVyeTsgUVVFUllfRklFTERTX0RFT" +
                    "ElNSVRFUikKICAgICAgRmllbGRWYWx1ZXNGb3JJbnNlcnRRdWVyeSA9IEFkZFN1YlN0cmluZyhGaWVsZFZhbHVlOyBGaWVsZFZhbHVlc0Zvckluc2VydFF1ZXJ5OyBRVUVSWV9GSUVMRFNfREVMSU1JVEVSKSA" +
                    "KICAgICAgSW5kZXggPSBJbmRleCArIDEKICAgIGVuZGZvcmVhY2gKICAgIFNlbGVjdFF1ZXJ5VGVtcGxhdGUgPSAiCiAgICAgIHNlbGVjdCAKICAgICAgICAxCiAgICAgIGZyb20KICAgICAgICBbJTA6c10KI" +
                    "CAgICAgd2hlcmUKICAgICAgICBUYXNrSUQgPSAlMTpzCiAgICAgICAgYW5kIEJsb2NrSUQgPSAlMjpzCiAgICAgICAgYW5kIEl0ZXJhdGlvbiA9ICUzOnMiCiAgICBRdWVyeVJlc3VsdCA9IFNRTChGb3JtYXQ" +
                    "oU2VsZWN0UXVlcnlUZW1wbGF0ZTsgQXJyYXlPZihCQUV2ZW50c1RhYmxlTmFtZTsgVGFza0lEOyBCbG9ja0lEOyBJdGVyYXRpb24pKSkKICAgIGlmIEFzc2lnbmVkKFF1ZXJ5UmVzdWx0KQogICAgICAvLyDQl" +
                    "dGB0LvQuCDQt9Cw0L/QuNGB0Ywg0LXRgdGC0YwsINGC0L4g0L7QsdC90L7QstC40YLRjAogICAgICBVcGRhdGVRdWVyeVRlbXBsYXRlID0gIgogICAgICAgIHVwZGF0ZSBbJTA6c10gc2V0CiAgICAgICAgICA" +
                    "lNDpzCiAgICAgICAgd2hlcmUKICAgICAgICAgIFRhc2tJRCA9ICUxOnMKICAgICAgICAgIGFuZCBCbG9ja0lEID0gJTI6cwogICAgICAgICAgYW5kIEl0ZXJhdGlvbiA9ICUzOnMiCiAgICAgIFNRTChGb3JtY" +
                    "XQoVXBkYXRlUXVlcnlUZW1wbGF0ZTsgQXJyYXlPZihCQUV2ZW50c1RhYmxlTmFtZTsgVGFza0lEOyBCbG9ja0lEOyBJdGVyYXRpb247IFVwZGF0ZVNldEZvclVwZGF0ZVF1ZXJ5KSkpCiAgICBlbmRpZiAgIAo" +
                    "gIGVuZGlmCgoKICAhVEFTS19TVEFURV9ET05FID0gItCS0YvQv9C+0LvQvdC10L3QsCIKICAhUFJPSkVDVF9UQVNLX0NPTVBMRVRFX1BFUkNFTlQgPSAxMDAKCiAvLyAhUHJvamVjdD3QodGG0LXQvdCw0YDQu" +
                    "NC50J/QsNGA0LDQvCgiUHJvamVjdCIpCiAgIU1TVGFza3MgPSDQodGG0LXQvdCw0YDQuNC50J/QsNGA0LDQvCgiVGFza3Mi" +
                    "KSAKICAhSUREb2MgPSDQodGG0LXQvdCw0YDQuNC50J/QsNGA0LDQvCgiSUQiKQogLy8g0LXRgdC70Lgg0L3QtdGCINC60LDQutC+0LPQvi3Qu9C40LHQviDQv9Cw0YDQsNC80LXRgtGA0LAsINGC0L4g0LfQvd" +
                    "Cw0YfQuNGCINGB0YbQtdC90LDRgNC40Lkg0LLRi9C30YvQstCw0LXRgtGB0Y8g0L3QtSDQuNC3IE1TIE9mZmljZSAgIAogIGlmICghTVNUYXNrcyA9PSAiIikgb3IgKCFJRERvYyA9PSAiIikKICAgICFNZXNz" +
                    "YWdlID0gTG9hZFN0cmluZygiRElSU1RSXzQwIjsgIkVETSIpCiAgICBSYWlzZShDcmVhdGVFeGNlcHRpb24oIiI7ICFNZXNzYWdlOyBlY1dhcm5pbmcpKSAvLyBb0JrQsNGC0LXQs9C+0YDQuNGPXSA9IGVjRX" +
                    "hjZXB0aW9uIE9SIGVjV2FybmluZyBPUiBlY0luZm9ybWF0aW9uIAogIGVuZGlmCiAgCiAvLyDQv9C+0LvRg9GH0LjRgtGMINC/0YDQuNC70L7QttC10L3QuNC1CiAgIUFwcCA9ICFTZW5kZXIuQXBwbGljYXRp" +
                    "b24KIC8vINC40L3RhNC+0YDQvNCw0YbQuNGPINC+INC00L7QutGD0LzQtdC90YLQtSAo0L7QsdGK0LXQutGC0LUpIAogICFFRG9jSW5mbyA9ICFBcHAuRURvY3VtZW50RmFjdG9yeS5PYmplY3RJbmZvKCFJRE" +
                    "RvYykKCiAgIVByb2dyZXNzID0gQ3JlYXRlUHJvZ3Jlc3MoIDsgIU1TVGFza3MuQ291bnQ7KQogICFQcm9ncmVzcy5TaG93CiAgIUZpbGxlZCA9IDAKIC8vINC/0YDQvtC50LTRkdC8INC/0L4g0LLRgdC10Lwg" +
                    "0LfQsNC00LDRh9Cw0LwgCiAgIWkgPSAxCiAgd2hpbGUgIWkgPD0gIU1TVGFza3MuQ291bnQKICAgLy8g0LXRgdC70Lgg0LfQsNC00LDRh9CwINGB0L7Qt9C00LDQvdCwCiAgICAhTVNUYXNrID0gIU1TVGFza3" +
                    "MuSXRlbSghaSkKICAgIGlmIG5vdCBWYXJJc051bGwoIU1TVGFzaykgIAogICAgICBpZiAhTVNUYXNrLkZsYWcyMAogICAgICAgICFIeXBlckxpbmsgPSAhTVNUYXNrLlRleHQyMAogICAgICAgICFJRCA9IFN1" +
                    "YlN0cmluZyghSHlwZXJMaW5rOyAiPSI7IDMpCiAgICAgICAgRXhjZXB0aW9uc09mZigpCiAgICAgICAgRnJlZUV4Y2VwdGlvbigpCiAgICAgICAgIVRhc2sgPSAhQXBwLlRhc2tGYWN0b3J5LkNvbXBvbmVudC" +
                    "ghSUQpCiAgICAgICAgaWYgbm90IEV4Y2VwdGlvbkV4aXN0cygpCiAgICAgICAgIC8vINC30LDQv9C+0LvQvdC40Lwg0YTQsNC60YIg0L/QviDQt9Cw0LTQsNGH0LUg0LX" +
                    "RgdC70Lgg0LfQsNC00LDRh9CwINCy0YvQv9C+0LvQvdC10L3QsAogICAgICAgICAvLyDQvdC+INGN0YLQsNC/INC10YnQtSDQvdC1INC30LDQstC10YDRiNC10L0KICAgICAgICAgIGlmICghVGFzay5EYXRhU" +
                    "2V0LlJlcXVpc2l0ZXMoIlRhc2tTdGF0ZSIpLkFzU3RyaW5nID09ICFUQVNLX1NUQVRFX0RPTkUpIGFuZAogICAgICAgICAgICAgKCFNU1Rhc2suUGVyY2VudENvbXBsZXRlIDwgIVBST0pFQ1RfVEFTS19DT01" +
                    "QTEVURV9QRVJDRU5UKSAKICAgICAgICAgICAgIU1TVGFzay5BY3R1YWxGaW5pc2ggPSAhVGFzay5EYXRhU2V0LlJlcXVpc2l0ZXMoIkVuZERhdGUiKS5Bc1N0cmluZwogICAgICAgICAgICAhTVNUYXNrLlBlc" +
                    "mNlbnRDb21wbGV0ZSA9ICFQUk9KRUNUX1RBU0tfQ09NUExFVEVfUEVSQ0VOVAogICAgICAgICAgICAhRmlsbGVkID0gIUZpbGxlZCArIDEgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgZW5kaWYKICA" +
                    "gICAgICBlbmRpZgogICAgICAgIEV4Y2VwdGlvbnNPbigpICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgZW5kaWYKICAgIGVuZGlmICAgCiAgICAhaSA9ICFpI" +
                    "CsgMQogICAgIVByb2dyZXNzLk5leHQKICBlbmR3aGlsZSAgICAgICAKCiAgaWYgIUZpbGxlZCA+IDEKICAgLy8gItCX0LDQv9C+0LvQvdC10L0g0YTQsNC60YIg0L/QviAlcyDRjdGC0LDQv9Cw0LwuIiAgICA" +
                    "gICAgCiAgICAhUmVzdWx0ID0gTG9hZFN0cmluZ0ZtdCgiRElSOEZFOEQxRDRfNTlGN180Mzc2Xzg5QkZfNkJGQkVBMDJGNTJGIjsgIkNPTU1PTiI7IEFycmF5T2YoIUZpbGxlZCkpICAgIAogIGVuZGlmICAKI" +
                    "CBpZiAoQ29weSghRmlsbGVkOyBMZW5ndGgoIUZpbGxlZCk7IDEpID09ICIxIikgYW5kIChDb3B5KCFGaWxsZWQ7IExlbmd0aCghRmlsbGVkKS0xOyAxKSA8PD4+ICIxIikKICAgLy8gItCX0LDQv9C+0LvQvdC" +
                    "10L0g0YTQsNC60YIg0L/QviAlcyDRjdGC0LDQv9GDLiIKICAgICFSZXN1bHQgPSBMb2FkU3RyaW5nRm10KCJESVIyQThBNjE4NF9CQkZBXzQyMEVfQUFBRl84OEU0MUM5MTQ4NDQiOyAiQ09NTU9OIjsgQXJyY" +
                    "XlPZighRmlsbGVkKSkgICAgICAKICBlbmRpZgogIGlmICFGaWxsZWQgPSAxCiAgIC8vICLQl9Cw0L/QvtC70L3QtdC9INGE0LDQutGCINC/0L4g0L7QtNC90L7QvNGDIN" +
                    "GN0YLQsNC/0YMuIgogICAgIVJlc3VsdCA9IExvYWRTdHJpbmcoIkRJUjQxQkIxRUU5Xzk3NTNfNDExQl85NDVFXzNDMEIwQTFDMjE4NyI7ICJDT01NT04iKQogIGVuZGlmCiAgaWYgIUZpbGxlZCA9IDAKICAg" +
                    "Ly8gItCd0Lgg0L/QviDQvtC00L3QvtC80YMg0Y3RgtCw0L/RgyDRhNCw0LrRgiDQvdC1INCx0YvQuyDQt9Cw0L/QvtC70L3QtdC9LiIKICAgICFSZXN1bHQgPSBMb2FkU3RyaW5nKCJESVJGOUREQjlERF80QU" +
                    "M5XzRGNzRfQTgzQl8yODM0Q0M1NzU4NzkiOyAiQ09NTU9OIikgICAgCiAgZW5kaWYg";
            ActiveSearchStringControl = this.textEditorControlSearchText;

            byte[] data = System.Convert.FromBase64String(str);
            System.Text.Encoding win1251 = System.Text.Encoding.UTF8;
            string scriptText = win1251.GetString(data);
            textEditorControlISBL.Document.TextContent = scriptText;
            textEditorControlISBL.Document.HighlightingStrategy =
                HighlightingStrategyFactory.CreateHighlightingStrategy("ISBL");
            textEditorControlISBL.Document.FoldingManager.FoldingStrategy =
                new IndentFoldingStrategy();

            ITextEditorProperties prop = textEditorControlISBL.Document.TextEditorProperties;
            prop.AllowCaretBeyondEOL = prop.AllowCaretBeyondEOL;
            prop.IsIconBarVisible = true;
            textEditorControlISBL.Document.TextEditorProperties = prop;
            textEditorControlISBL.Document.ReadOnly = true;
            MarkSearchStrings();
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
                connect = isblLoader.Connect(textBoxSQLServer.Text,
                                   textBoxDB.Text,
                                   "",
                                   "",
                                   true);
            }
            else
            {
                connect = isblLoader.Connect(textBoxSQLServer.Text,
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
        bool GetISBL()
        {
            SourceISBLNodes = isblLoader.Load();
            bool ISBLNodesIsEmpty = true;
            foreach (Node node in SourceISBLNodes)
            {
                if (node != null)
                {
                    ISBLNodesIsEmpty = false;
                    break;
                }
            }
            if (ISBLNodesIsEmpty)
            {
                string loginSQLUser = this.textBoxLogin.Text;
                if (this.checkBoxWinAuth.Checked)
                {
                    loginSQLUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
                else
                {
                    loginSQLUser = this.textBoxLogin.Text;
                }
                string text = string.Format(
@"В выбранной базе данных ""{0}"" сервера ""{1}"" отсутствуют таблицы разработки.
Или у пользователя с логином ""{2}"" нет прав на просмотр содержимого таблиц разработки в указанной базе данных."
                , this.textBoxDB.Text
                , this.textBoxSQLServer.Text
                , loginSQLUser
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
        void ConnectAndGetISBL()
        {
            if ((textBoxSQLServer.Text.Trim() != "") && (textBoxDB.Text.Trim() != "") &&
                   (checkBoxWinAuth.Checked || (!checkBoxWinAuth.Checked && textBoxLogin.Text.Trim() != ""))
              )
            {
                if (Connect())
                {
                    if (GetISBL())
                    {
                        ActiveISBLNodes = new List<Node>(SourceISBLNodes);
                        buildDisplayTree();
                        showTree();
                        buttonSearch.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show(isblLoader.errorText, "Ошибка открытия базы данных");
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
            ConnectAndGetISBL();
        }

        /// <summary>
        /// Выбор узла в дереве разработки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TreeViewResultsAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                textEditorControlISBL.IsReadOnly = true;
                Node node = e.Node.Tag as Node;
                textEditorControlISBL.Document.TextContent = node.Text;
                MarkSearchStrings();
                toolStripStatusLabelSelectedElement.Text = node.Name;
            }
            else
            {
                textEditorControlISBL.Document.TextContent = "";
            }
        }

        /// <summary>
        /// Проверка списка регулярных выражений на корректность и возврат списка строк корректных регулярных выражений.
        /// </summary>
        /// <returns></returns>
        string[] checkRegExpFormat()
        {
            string[] searchStrs = ActiveSearchStringControl.Text.Split(
                new string[] { ActiveSearchStringControl.TextEditorProperties.LineTerminator }
                , StringSplitOptions.RemoveEmptyEntries
                );
            List<string> regExpResultList = new List<string>();
            ActiveSearchStringControl.Document.MarkerStrategy.RemoveAll((marker) => { return true; });
            ActiveSearchStringControl.Document.BookmarkManager.RemoveMarks((bookmark) => { return true; });
            bool errorExist = false;
            for (int indexRegExpCandidate = 0; indexRegExpCandidate < ActiveSearchStringControl.Document.TotalNumberOfLines; indexRegExpCandidate++)
            {
                LineSegment segment = ActiveSearchStringControl.Document.GetLineSegment(indexRegExpCandidate);
                string regExpCandidateString = ActiveSearchStringControl.Document.GetText(segment);
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
                    TextMarker marker = new TextMarker(
                        segment.Offset
                        , segment.Length
                        , TextMarkerType.WaveLine
                        , Color.Red
                        , Color.DarkRed
                        );
                    marker.ToolTip = ex.Message;
                    ActiveSearchStringControl.Document.MarkerStrategy.AddMarker(marker);

                    Bookmark mark = new Bookmark(
                        ActiveSearchStringControl.Document
                        , ActiveSearchStringControl.Document.OffsetToPosition(segment.Offset)
                        , true);
                    ActiveSearchStringControl.Document.BookmarkManager.AddMark(mark);
                    errorExist = true;
                }
            }
            if (errorExist)
            {
                ActiveSearchStringControl.Refresh();
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
                    if (dictRegEx.ContainsKey(searchPhrase))
                    {
                        regEx = dictRegEx[searchPhrase];
                    }
                    else
                    {
                        regEx = new Regex(searchPhrase, regExpOptions);
                        dictRegEx.Add(searchPhrase, regEx);
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

        /// <summary>
        /// Возврат всех слов из текста реактора поискового запроса, 
        /// разделителями слов в тексте выступают пробельные символы.
        /// </summary>
        /// <returns></returns>
        void GetSearchStrArray()
        {
            if (checkBoxFindRegExp.Checked)
            {
                searchStrs = checkRegExpFormat();
            }
            else
            {
                ActiveSearchStringControl.Document.MarkerStrategy.RemoveAll((marker) => { return true; });
                ActiveSearchStringControl.Document.BookmarkManager.RemoveMarks((bookmark) => { return true; });
                ActiveSearchStringControl.Refresh();
                string[] searchLineStrs = ActiveSearchStringControl.Text.Split(
                new string[] { ActiveSearchStringControl.TextEditorProperties.LineTerminator }
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
                searchStrs = searchWords.ToArray();
            }
        }

        /// <summary>
        /// Нажатие кнопки "Find"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void ButtonFilterClick(object sender, EventArgs e)
		{
            buttonSearch.Enabled = false;
            GetSearchStrArray();
            foreach (Node node in SourceISBLNodes)
            {
                FilterNode(node, searchStrs, checkBoxFindCaseSensitive.Checked, checkBoxFindRegExp.Checked, checkBoxFindAll.Checked);
            }
            buildTabISBLNodes();
            buildDisplayTree();
            showTree();
            buttonSearch.Enabled = true;
        }
		

        /// <summary>
        /// Подсветка строк поиска в текущем тексте разработки
        /// </summary>
        public void MarkSearchStrings()
        {
             GetSearchStrArray();
            textEditorControlISBL.Document.MarkerStrategy.RemoveAll((marker) => { return true; });
            textEditorControlISBL.Document.BookmarkManager.RemoveMarks((bookmark) => { return true; });

            if (checkBoxFindRegExp.Checked)
            {
                MarkSearchStringsRegExp(searchStrs, checkBoxFindCaseSensitive.Checked);
            }
            else
            {
                MarkSearchStrings(searchStrs, checkBoxFindCaseSensitive.Checked);
            }
            textEditorControlISBL.Refresh();
        }

        public void MarkSearchStringsRegExp(string[] regExpArray, bool caseSensitive)
        {
            RegexOptions regExpOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

			//Подсветка искомого текста
            if (regExpArray.Length > 0)
            {
                String text = textEditorControlISBL.Document.TextContent;
                bool isCentered = false;

                for (int indexRegExpStrings = 0; indexRegExpStrings < regExpArray.Length; indexRegExpStrings++)
                {
                    string hlStr = regExpArray[indexRegExpStrings];
                    if(hlStr != "")
                    {
                        Regex regExp = new Regex(hlStr, regExpOptions);
                        MatchCollection regExpFindResults = regExp.Matches(text);
                     
                        foreach(Match match in regExpFindResults)
                        {
                            TextMarker marker = new TextMarker(
                                match.Index
                                , match.Length
                                , TextMarkerType.SolidBlock
                                , Color.FromArgb(255, 156, 255, 156) // светло-зелёный
                                , Color.FromArgb(255, 18, 10, 143) // ультрамарин
                                );
                            marker.ToolTip = hlStr;
							textEditorControlISBL.Document.MarkerStrategy.AddMarker(marker);

                            Bookmark mark = new Bookmark(
                                textEditorControlISBL.Document
                                , textEditorControlISBL.Document.OffsetToPosition(match.Index)
                                , false);
                            textEditorControlISBL.Document.BookmarkManager.AddMark(mark);

                            if (!isCentered)
                            {
                                var lineNumber = textEditorControlISBL.Document.GetLineNumberForOffset(match.Index);
                                textEditorControlISBL.ActiveTextAreaControl.CenterViewOn(lineNumber, 0);
                                isCentered = true;
                            }
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
                String text = textEditorControlISBL.Document.TextContent;

                foreach (string findStr in findArray)
                {
                    //Подстветка строк
                    int posEnd = 0;
                    int posStart = 0;

                    char[] charsDelimeters = { '\n', '\t', ' ', '\r' };
                    string[] strs = findStr.Split(charsDelimeters, StringSplitOptions.RemoveEmptyEntries);
                    bool isCentered = false;

                    foreach (string str in strs)
                    {
                        string hlStr = str.Trim();
                        if (hlStr != "")
                        {
                            posEnd = 0;
                            posStart = text.IndexOf(hlStr, 0, comparation);
                            while (posStart >= 0)
                            {
                                posEnd = posStart + hlStr.Length - 1;
                                if (posEnd >= 0)
                                {
                                    TextMarker marker = new TextMarker(
                                        posStart
                                        , posEnd - posStart + 1
                                        , TextMarkerType.SolidBlock
                                        , Color.FromArgb(255, 156, 255, 156) // светло-зелёный
                                        , Color.FromArgb(255, 18, 10, 143) // ультрамарин
                                        );
                                    marker.ToolTip = hlStr;
                                    textEditorControlISBL.Document.MarkerStrategy.AddMarker(marker);

                                    Bookmark mark = new Bookmark(
                                        textEditorControlISBL.Document
                                        , textEditorControlISBL.Document.OffsetToPosition(posStart)
                                        , false);
                                    textEditorControlISBL.Document.BookmarkManager.AddMark(mark);

                                    if (!isCentered)
                                    {
                                        var lineNumber = textEditorControlISBL.Document.GetLineNumberForOffset(posStart);
                                        textEditorControlISBL.ActiveTextAreaControl.CenterViewOn(lineNumber, 0);
                                        isCentered = true;
                                    }

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
		

		/// <summary>
		/// Обработка нажатия клавиши в форме аутентификации на SQL Server
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>		
		void TextBoxLoginFormKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				ConnectAndGetISBL();
			}
		}
		
		
		
		void TextBoxLoginFormTextChanged(object sender, EventArgs e)
		{
			(sender as TextBox).Font = new Font((sender as TextBox).Font, FontStyle.Regular);
			(sender as TextBox).BackColor = this.textBoxFilter.BackColor;
            (sender as TextBox).ForeColor = this.textBoxFilter.ForeColor;
		}

        void buildDisplayTree()
        {
            filterISBLNodesByName();
            ActiveSearchStringControl.TreeViewResults.Nodes.Clear();
            copyISBLNodesToTreeNodes(ActiveISBLNodes, ActiveSearchStringControl.TreeViewResults.Nodes);
        }
        void showTree()
        {
            ActiveSearchStringControl.TreeViewResults.BringToFront();
            var SelectedNode = ActiveSearchStringControl.TreeViewResults.SelectedNode;
            if(SelectedNode != null)
            {
                Node node = SelectedNode.Tag as Node;
                textEditorControlISBL.Document.TextContent = node.Text;
                MarkSearchStrings();
                toolStripStatusLabelSelectedElement.Text = node.Name;
            }
        }
        void filterISBLNodesByName()
        {
            if(ActiveISBLNodes != null)
            {
                foreach (Node node in ActiveISBLNodes)
                {
                    FilterNodeByName(node, textBoxFilter.Text);
                }
            }
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
        void copyISBLNodesToTreeNodes(List<Node> ISBLNodes, TreeNodeCollection TreeNodes)
        {
            foreach (Node isblNode in ISBLNodes)
            {
                if (isblNode.Visible)
                {
                    TreeNode treeNode = TreeNodes.Add(isblNode.Name);
                    treeNode.Tag = isblNode;
                    isblNode.Tag = treeNode;

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
                        copyISBLNodesToTreeNodes(isblNode.Nodes, treeNode.Nodes);
                    }
                }
            }
        }

        void buildTabISBLNodes()
        {
            ActiveISBLNodes.Clear();
            copyMatchedISBLNodes(ActiveISBLNodes, SourceISBLNodes);
        }

        void copyMatchedISBLNodes(List<Node> TargetNodes, List<Node> SourceNodes)
        {
            if (SourceNodes != null)
            {
                foreach (Node isblNode in SourceNodes)
                {
                    if (isblNode.IsMatch || isblNode.IsContainsMatchedNode)
                    {
                        var nodeCopy = isblNode.Clone();
                        if (isblNode.Nodes != null)
                        {
                            nodeCopy.Nodes = new List<Node>();
                            copyMatchedISBLNodes(nodeCopy.Nodes, isblNode.Nodes);
                        }
                        TargetNodes.Add(nodeCopy);
                    }
                }
            }
        }

        /// <summary>
        /// Изменение текста в поле фильтрации дерева проекта
        /// </summary>
        void TextBoxFilter_TextChanged (object sender, System.EventArgs e)
		{
            buildDisplayTree();
            showTree();
        }
		
		void CheckBoxWinAuthCheckedChanged(object sender, EventArgs e)
		{
			textBoxLogin.Enabled = !checkBoxWinAuth.Checked;
			textBoxPassword.Enabled = textBoxLogin.Enabled;
		}

        private void textEditorControlRegExp_TextChanged(object sender, EventArgs e)
        {
            timerRegExpFind.Enabled = false;
            timerRegExpFind.Enabled = true;
        }

        private void timerRegExpFind_Tick(object sender, EventArgs e)
        {
            timerRegExpFind.Enabled = false;
            MarkSearchStrings();            
        }

        private void buttonCloseCurrentTab_Click(object sender, EventArgs e)
        {
            if (tabControlSarchText.TabCount > 1)
            {
                TabPage tabPageForClose = tabControlSarchText.SelectedTab;
                tabControlSarchText.SelectedIndex = tabControlSarchText.SelectedIndex > 0 ?
                    tabControlSarchText.SelectedIndex - 1 : 0;
                tabControlSarchText.Controls.Remove(tabPageForClose);
            }
        }

        private void buttonAddNewTab_Click(object sender, EventArgs e)
        {
            TabPage tabPageSearchNew = new TabPage(string.Format("Search {0}", tabControlSarchText.TabCount+1));

            MyTextEditorControl textEditor = new MyTextEditorControl(this);
            textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            textEditor.IsReadOnly = false;
            textEditor.Location = new System.Drawing.Point(0, 0);
            textEditor.ShowEOLMarkers = true;
            textEditor.ShowSpaces = true;
            textEditor.ShowTabs = true;
            //textEditor.Size = new System.Drawing.Size(100, 50);
            textEditor.TabIndex = 1;
            textEditor.TextChanged += new System.EventHandler(textEditorControlRegExp_TextChanged);
            textEditor.Tag = new List<Node>(SourceISBLNodes);
            tabPageSearchNew.Tag = textEditor;
            tabPageSearchNew.Controls.Add(textEditor);

            tabControlSarchText.Controls.Add(tabPageSearchNew);
            tabControlSarchText.SelectedTab = tabPageSearchNew;
        }

        private void tabControlSarchText_Selecting(object sender, TabControlCancelEventArgs e)
        {
            ActiveSearchStringControl = (MyTextEditorControl)tabControlSarchText.SelectedTab.Tag;            
        }

        private void checkBoxFindRegExp_CheckedChanged(object sender, EventArgs e)
        {
            MarkSearchStrings();
        }

        private void tabControlSarchText_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveSearchStringControl = (MyTextEditorControl)tabControlSarchText.SelectedTab.Tag;
            showTree();
        }

        private void checkBoxFindCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            MarkSearchStrings();
        }
	
	    void groupBoxSearch_Resize (object sender, System.EventArgs e)
	    {
		    this.buttonAddNewTab.Left = groupBoxSearch.Width - 36;
		    this.buttonCloseCurrentTab.Left = groupBoxSearch.Width - 19;
	    }

        private void buttonExpand_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in ActiveSearchStringControl.TreeViewResults.Nodes)
            {
                node.ExpandAll();
            }
            showTree();
        }
    }
}
