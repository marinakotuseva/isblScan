using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;
using ICSharpCode.TextEditor.Document;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		Loader isblLoader;
		Font fontBold;
		Font fontBoldUnderline;

		List<Node> ISBLNodes { get; set; }

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			isblLoader = new Loader();
			
			string sqlServer;
			string dataBase;
			string login;
			bool isWinAuth;
			if(Configuration.Load(out sqlServer, out dataBase, out login, out isWinAuth))
			{
				textBoxSQLServer.Text = sqlServer;
				textBoxDB.Text = dataBase;
				textBoxLogin.Text = login;
				if(isWinAuth)
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
				"AddWhere     AddFrom",
				"AddOrderBy",
				"SQL(         CSQL",
				"Выполнить    Execute  Exec",
				"Login        Логин",
				"Pass         Пароль",
				"Secret       Секрет",
				"Admin        Админ",
				"drop         xp_",
				"AccessRights Grant    Control",
				"Encrypt      Decrypt  Шифр",
				"CreateObject ActiveX",
				""
			};
			foreach(string searchString in defaultSearchStrings)
			{
				this.textBoxSearch.Text += (searchString + System.Environment.NewLine);
			}
			//
			// TODO: Добавить хорошую стправку на начальной странице
			//
			ApplicationInfo applicationInfo = new ApplicationInfo();
			//this.richTextBoxResult.Text = string.Format("{0} {1}\n{2}", applicationInfo.ProductName, applicationInfo.Version, applicationInfo.Description);
			//HighLight("", null);

			string str =    "Ly8vIElTQkxTY2FuIENvZGVWaWV3Ci8vLyBUb29sIGZvciB2aWV3IHNvdXJjZSBjb2RlIG9mIEVDTSBzeXN0ZW1zIGJhc2VkIG9uIElTLUJ1aWxkZXIgNyAoRElSRUNUVU0sIE9yaWVuZ2UgQ29udGVycmEpIG" +
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
			byte[] data = System.Convert.FromBase64String(str);
			System.Text.Encoding win1251 = System.Text.Encoding.UTF8;
			string scriptText = win1251.GetString(data);
			textEditorControlISBL.Document.TextContent = scriptText;
			textEditorControlISBL.Document.HighlightingStrategy = 
				HighlightingStrategyFactory.CreateHighlightingStrategy("ISBL");
			MarkSearchStrings(textBoxSearch.Text);
		}
		
		void LoadSubNodes(TreeNodeCollection treeNodes, Node isblNode)
		{
			if(isblNode != null && isblNode.Visible)
			{
				TreeNode treeNode = treeNodes.Add(isblNode.Name);
				if(isblNode.Text != null)
				{
					treeNode.Tag = isblNode.Text;
				}
				if(isblNode.Nodes != null)
				{
					foreach(Node isblSubNode in isblNode.Nodes)
					{
						LoadSubNodes(treeNode.Nodes, isblSubNode);
					}
				}
			}
		}
		
		bool Conenct()
		{
			bool connect;
			if(checkBoxWinAuth.Checked)
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
			if(connect)
			{
				Configuration.Save(textBoxSQLServer.Text, textBoxDB.Text, textBoxLogin.Text, checkBoxWinAuth.Checked);
			}
			return connect;
		}
		
		void GetISBL()
		{
			ISBLNodes = isblLoader.Load();
			treeViewResults.Nodes.Clear();
			foreach(Node isblNode in ISBLNodes)
			{
				LoadSubNodes(this.treeViewResults.Nodes, isblNode);
			}
		}
		
		void ConnectAndGetISBL()
		{
				if((textBoxSQLServer.Text.Trim() != "")&&(textBoxDB.Text.Trim() != "")&&
			   		(checkBoxWinAuth.Checked || (!checkBoxWinAuth.Checked && textBoxLogin.Text.Trim() != ""))
			  	)
				{
					if(Conenct())
					{
						GetISBL();
						buttonRefresh.Enabled = true;
						buttonSearch.Enabled = true;
					}
					else
					{
						MessageBox.Show(isblLoader.errorText, "Ошибка открытия базы данных");
					}
				}
				else
				{
					if(textBoxLogin.Text.Trim() == "")
					{
						textBoxLogin.Text = "Login";
						textBoxLogin.Font = new Font(textBoxLogin.Font, FontStyle.Italic);
						textBoxLogin.BackColor = Color.LightCoral;
						textBoxLogin.SelectAll();
						textBoxLogin.Focus();
					}
					if(textBoxDB.Text.Trim() == "")
					{
						textBoxDB.Text = "Data Base";
						textBoxDB.Font = new Font(textBoxDB.Font, FontStyle.Italic);
						textBoxDB.BackColor = Color.LightCoral;
						textBoxDB.SelectAll();
						textBoxDB.Focus();
					}
					if(textBoxSQLServer.Text.Trim() == "")
					{
						textBoxSQLServer.Text = "Sql Server";
						textBoxSQLServer.Font = new Font(textBoxSQLServer.Font, FontStyle.Italic);
						textBoxSQLServer.BackColor = Color.LightCoral;
						textBoxSQLServer.SelectAll();
						textBoxSQLServer.Focus();
					}
				}			
		}
		void ButtonConnectClick(object sender, EventArgs e)
		{
			 ConnectAndGetISBL();
		}
		
		void TreeViewResultsAfterSelect(object sender, TreeViewEventArgs e)
		{
			if(e.Node.Tag != null)
			{				
				textEditorControlISBL.Enabled = false;
				textEditorControlISBL.IsReadOnly = true;
				textEditorControlISBL.Document.TextContent = e.Node.Tag.ToString();
				textEditorControlISBL.Enabled = true;

			}
			else
			{
				textEditorControlISBL.Document.TextContent = "";
				textEditorControlISBL.Enabled = false;
			}
		}
		
		//Рекурсивный поиск по дереву разработки
		bool FilterNode(TreeNode node, string searchStrs)
		{
			bool isFound = false;
			//Сначала выделим текущий элемент так, как будто в нём ничего не найдено
			node.ForeColor = Color.Gray;
			node.Checked = false;
			if(node.Nodes != null)
			{
				foreach(TreeNode subNode in node.Nodes)
				{
					if(FilterNode(subNode, searchStrs))
					{
						node.ForeColor = Color.Black;
						isFound = true;
					}
				}
			}
			if(node.Tag != null)
			{
				//RegexOptions ro = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase;
				//Разделение поисковых фраз по строкам
				char[] arrDelimeters = {'\n', '\t', ' ', '\r'};
				string[] searchPhrases = searchStrs.ToUpper().Split(arrDelimeters);
				string isblText = node.Tag.ToString().ToUpper();
				string nodeName = node.Name;
				foreach(string searchPhrase in searchPhrases)
				{
					//Пропуск пустых поисковых строк
					if(searchPhrase.Trim() != "")
					{
						if(
						   (isblText.Contains(searchPhrase.Trim()) ) ||
						   (nodeName.Contains(searchPhrase.Trim()) ) )
						{
							node.ForeColor = Color.Black;
							node.Checked = true;
							isFound = true;
						}				
					}
				}
			}
			return isFound;
		}
		
		void ButtonFilterClick(object sender, EventArgs e)
		{
			textBoxSearch.Enabled = false;
			buttonSearch.Enabled = false;
			foreach(TreeNode node in treeViewResults.Nodes)
			{
				FilterNode(node, textBoxSearch.Text);
			}
			buttonSearch.Enabled = true;
			textBoxSearch.Enabled = true;
		}
		
		
//		/// <summary>
//		///Подстветка специальных конструкций в тексте 
//		/// </summary>
//		private void HighLightSpecialConstruction(ref int posStart, ref int posEnd)
//		{
//			//Если текущая позиция в начале строки
//			if((posStart == 0)||((posStart > 0)&&(richTextBoxResult.Text.Substring(posStart-1, 1)==System.Environment.NewLine)))
//			{
//				/********************************************************************
//				 * Подсветка специальных конструкций (заголовков, вставленных программой)
//				 ********************************************************************/
//				if((posStart+4<richTextBoxResult.Text.Length)&&(richTextBoxResult.Text.Substring(posStart, 4)=="-=[ "))
//				{
//					posEnd = richTextBoxResult.Text.IndexOf(" ]=-", posStart+1);
//					if(posEnd > posStart)
//					{
//						posEnd = posEnd+4;
//						richTextBoxResult.Select(posStart, posEnd-posStart);
//						richTextBoxResult.SelectionFont = this.fontBoldUnderline;
//						richTextBoxResult.SelectionBackColor = Color.LightGray;
//						posStart = posEnd;
//					}
//				}
//			}
//		}

		public void MarkSearchStrings (string searchStr)
		{
			//Подсветка искомого текста
			if(searchStr != "")
			{
				String text = textEditorControlISBL.Document.TextContent.Replace("\r", "\n");
				string strDelimeters = "%^&*()-=+\\/;:<>.,?[]{}\n\t ";

				this.fontBold = new Font(textEditorControlISBL.Document.TextEditorProperties.Font, FontStyle.Bold);
				this.fontBoldUnderline = new Font(textEditorControlISBL.Document.TextEditorProperties.Font, FontStyle.Bold | FontStyle.Underline);
				
				//Подстветка строк
				int posEnd = 0;
				int posStart = 0;

				char[] charsDelimeters = {'\n', '\t', ' ', '\r'};
				string[] strs = searchStr.Split(charsDelimeters);
				int minumumPosStart = 0;
				int indexStrs = 0;
				for(indexStrs = 0; indexStrs < strs.Length; indexStrs++)
				{
					string hlStr = strs[indexStrs].Trim();
					if(hlStr != "")
					{
						posEnd = 0;
						posStart = text.IndexOf(hlStr, 0, StringComparison.OrdinalIgnoreCase);
						while(posStart >= 0)
						{
							if((minumumPosStart == 0) || (minumumPosStart > posStart))
							{
								minumumPosStart = posStart;
							}
							posEnd = posStart + hlStr.Length-1;
							if(posEnd >= 0)
							{
								TextMarker marker = new TextMarker(
									posStart
									, posEnd-posStart+1
									, TextMarkerType.SolidBlock
									, Color.DarkGoldenrod
									, Color.Yellow);
								textEditorControlISBL.Document.MarkerStrategy.AddMarker(marker);
								posStart = text.IndexOf(hlStr, posEnd+1,  StringComparison.OrdinalIgnoreCase);
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
		
//		/// <summary>
//		///Подсветка синтаксиса 
//		/// </summary>
//		/// <param name="searchStr">
//		/// A <see cref="System.String"/>
//		/// </param>
//		/// <param name="treeNode">
//		/// A <see cref="TreeNode"/>
//		/// </param>
//		public void HighLight(string searchStr,  TreeNode treeNode)
//		{
//			String text = richTextBoxResult.Text.Replace("\r", "\n");
//			string strDelimeters = "%^&*()-=+\\/;:<>.,?[]{}\n\t ";
//
//			this.fontBold = new Font(richTextBoxResult.SelectionFont, FontStyle.Bold);
//			this.fontBoldUnderline = new Font(richTextBoxResult.SelectionFont, FontStyle.Bold | FontStyle.Underline);
//			
//			//Подстветка строк
//			int posEnd = 0;
//			int posStart = 0;
//			
//			string[] keywords = {"if", "endif", "если", "конецесли", "else", "иначе",
//				"while", "endwhile", "пока", "конецпока",
//				"try", "catch", "endtry",
//				"foreach", "endforeach", "in", "все", "конецвсе", "в"
//			};
//			string[] operators = {"or", "или", 
//				"and", "и",
//				"not", "не"
//			};
//			string[] constants = {
//				//Основные константы
//				"true", "false",
//				"cr", "tab",
//				"YES_VALUE", "NO_VALUE",
//				"null", "nil",
//				//Запуск внешних приложений
//				"smHidden", "smMaximized", "smMinimized", "smNormal", "wmNo", "wmYes",
//				//Работа с диалоговыми окнами
//				"cbsCommandLinks", "cbsDefault", "mrCancel", "mrOk", "ATTENTION_CAPTION", "CONFIRMATION_CAPTION", "ERROR_CAPTION", "INFORMATION_CAPTION",
//				//Работа с ISBL-редактором
//				"ISBL_SYNTAX", "NO_SYNTAX",
//				//Работа с электронными документами
//				"EDOC_VERSION_ACTIVE_STAGE_CODE", "EDOC_VERSION_DESIGN_STAGE_CODE", "EDOC_VERSION_OBSOLETE_STAGE_CODE",
//				//Работа с ЭЦП и шифрованием
//				"cpDataEnciphermentEnabled", "cpDigitalSignatureEnabled", "cpID", "cpIssuer", "cpSerial", "cpSubjectName", "cpSubjSimpleName", "cpValidFromDate", "cpValidToDate",
//				//События объектов
//				"dseBeforeOpen", "dseAfterOpen", "dseBeforeClose", "dseAfterClose", "dseOnValidDelete", "dseBeforeDelete", "dseAfterDelete", "dseAfterDeleteOutOfTransaction", "dseOnDeleteError", "dseBeforeInsert", "dseAfterInsert", "dseOnValidUpdate", "dseBeforeUpdate", "dseOnUpdateRatifiedRecord", "dseAfterUpdate", "dseAfterUpdateOutOfTransaction", "dseOnUpdateError", "dseAfterScroll", "dseOnOpenRecord", "dseOnCloseRecord", "dseBeforeCancel", "dseAfterCancel", "reOnChange",
//				//Идентификаторы правил
//				"AUTO_NUMERATION_RULE_ID", "CANT_CHANGE_ID_REQUISITE_RULE_ID", "CANT_CHANGE_OURFIRM_REQUISITE_RULE_ID", "CHECK_CHANGING_REFERENCE_RECORD_USE_RULE_ID", "CHECK_CODE_REQUISITE_RULE_ID", "CHECK_DELETING_REFERENCE_RECORD_USE_RULE_ID", "CHECK_FILTRATER_CHANGES_RULE_ID", "CHECK_REFERENCE_INTERVAL_RULE_ID", "CHECK_REQUIRED_REQUISITES_FULLNESS_RULE_ID", "MAKE_RECORD_UNRATIFIED_RULE_ID", "RESTORE_AUTO_NUMERATION_RULE_ID", "SET_DEFAULT_FIRM_CONTEXT_RULE_ID", "SET_DEPARTMENT_SECTION_BOUNDS_RULE_ID", "SET_FIRM_CONTEXT_FROM_RECORD_RULE_ID", "SET_FIRST_RECORD_IN_LIST_FORM_RULE_ID", "SET_IDSPS_VALUE_RULE_ID", "SET_NEXT_CODE_VALUE_RULE_ID", "SET_OURFIRM_BOUNDS_RULE_ID", "SET_OURFIRM_REQUISITE_RULE_ID",
//				//Параметры объектов
//				"SHOW_RECORD_PROPERTIES_FORM", "PREVIOUS_CARD_TYPE_NAME",
//				//Типы объектов системы в таблице связей
//				"EDOCUMENT_LINK_KIND", "FOLDER_LINK_KIND", "TASK_LINK_KIND", "JOB_LINK_KIND", "DOCUMENT_LINK_KIND", "REFERENCE_LINK_KIND",
//				//Дополнительные типы блокируемых объектов
//				"EDOCUMENT_VERSION_LOCK_TYPE", "COMPONENT_TOKEN_LOCK_TYPE",
//				//Прочие константы
//				"ISBSYSDEV", "USER_NAME_FORMAT",  "MEMORY_DATASET_DESRIPTIONS_FILENAME",  "FILTER_OPERANDS_DELIMITER",  "FILTER_OPERATIONS_DELIMITER", 
//				//Реквизиты справочников
//				"SYSREQ_ID", "SYSREQ_STATE", "SYSREQ_NAME", "SYSREQ_NAME_LOCALIZE_ID", "SYSREQ_DESCRIPTION", "SYSREQ_DESCRIPTION_LOCALIZE_ID", "SYSREQ_NOTE", "SYSREQ_CONTENTS", "SYSREQ_CODE", "SYSREQ_TYPE", "SYSREQ_LAST_UPDATE", "SYSREQ_OUR_FIRM", "SYSREQ_LEADER_REFERENCE", "SYSREQ_ORIGINAL_RECORD", "SYSREQ_DOUBLE", "SYSREQ_RECORD_STATUS", "SYSREQ_UNIT", "SYSREQ_UNIT_ID", "SYSREQ_MAIN_RECORD_ID", "SYSREQ_LINE_NUMBER",
//				//Предопределенные реквизиты электронных документов
//				"SYSREQ_EDOC_AUTHOR", "SYSREQ_EDOC_CREATED", "SYSREQ_EDOC_EDITOR", "SYSREQ_EDOC_ENCODE_TYPE", "SYSREQ_EDOC_ENCRYPTION_PLUGIN_NAME", "SYSREQ_EDOC_EXPORTER", "SYSREQ_EDOC_EXPORT_DATE", "SYSREQ_EDOC_KIND", "SYSREQ_EDOC_LIFE_STAGE_NAME", "SYSREQ_EDOC_LOCKED_FOR_SERVER_CODE", "SYSREQ_EDOC_MODIFIED", "SYSREQ_EDOC_NAME", "SYSREQ_EDOC_NOTE", "SYSREQ_EDOC_SIGNATURE_TYPE", "SYSREQ_EDOC_SIGNED", "SYSREQ_EDOC_STORAGE", "SYSREQ_EDOC_TEXT_MODIFIED", "SYSREQ_EDOC_ACCESS_TYPE", "SYSREQ_EDOC_QUALIFIED_ID", "SYSREQ_EDOC_SESSION_KEY", "SYSREQ_EDOC_SESSION_KEY_ENCRYPTION_PLUGIN_NAME", "SYSREQ_EDOC_VERSION_AUTHOR", "SYSREQ_EDOC_VERSION_CRC", "SYSREQ_EDOC_VERSION_DATA", "SYSREQ_EDOC_VERSION_EDITOR", "SYSREQ_EDOC_VERSION_EXPORT_DATE", "SYSREQ_EDOC_VERSION_HIDDEN", "SYSREQ_EDOC_VERSION_LIFE_STAGE", "SYSREQ_EDOC_VERSION_LOCKED_FOR_SERVER_CODE", "SYSREQ_EDOC_VERSION_MODIFIED", "SYSREQ_EDOC_VERSION_NOTE", "SYSREQ_EDOC_VERSION_SIGNATURE_TYPE", "SYSREQ_EDOC_VERSION_SIGNED", "SYSREQ_EDOC_VERSION_SIZE", "SYSREQ_EDOC_VERSION_SOURCE", "SYSREQ_EDOC_VERSION_TEXT_MODIFIED",
//
//				"SYSREQ_CODE", "SYSREQ_CONTENTS", "SYSREQ_EDOC_NAME", "SYSREQ_ID", "SYSREQ_EDOC_KIND", "SYSRES_SBDATA",
//				"References", "EDocuments", "Application",
//				"Object", "Sender", 
//				};
//
//			while(posStart < text.Length)
//			{
//				HighLightSpecialConstruction(ref posStart, ref posEnd);
//				/*
//				//Если текущая позиция в начале строки
//				if((posStart == 0)||((posStart > 0)&&(text.Substring(posStart-1, 1)==System.Environment.NewLine)))
//				{
//					// ----------------------------------------------------------------------
//					// Подсветка специальных конструкций (заголовков, вставленных программой)
//					// ----------------------------------------------------------------------
//					if((posStart+4<text.Length)&&(text.Substring(posStart, 4)=="-=[ "))
//					{
//						posEnd = text.IndexOf(" ]=-", posStart+1);
//						if(posEnd > posStart)
//						{
//							posEnd = posEnd+4;
//							richTextBoxResult.Select(posStart, posEnd-posStart);
//							richTextBoxResult.SelectionFont = fontBoldUnderline;
//							richTextBoxResult.SelectionBackColor = Color.LightGray;
//							posStart = posEnd;
//						}
//					}
//				}
//				*/
//				/********************************************************************
//				 * Подсветка ключевых слов
//				 ********************************************************************/
//				foreach(string keyword in keywords)
//				{
//					if(posStart+keyword.Length <= text.Length)
//					{
//						string testStr = text.Substring(posStart, keyword.Length).ToLower();
//						if(testStr == keyword.ToLower())
//						{
//							bool isKeyword = true;
//							
//							if(posStart > 0)
//							{
//								string prevChar = text.Substring(posStart-1, 1);
//								if(!(strDelimeters.Contains(prevChar)))
//								{
//									isKeyword = false;
//								}
//							}
//							
//							if(posStart+keyword.Length < text.Length-1)
//							{
//								string postChar = text.Substring(posStart+keyword.Length, 1);
//								if(!(strDelimeters.Contains(postChar)))
//								{
//									isKeyword = false;
//								}
//							}
//							if(isKeyword)
//							{
//								posEnd = posStart+keyword.Length;
//								richTextBoxResult.Select(posStart, posEnd-posStart);
//								richTextBoxResult.SelectionFont = fontBold;
//								posStart = posEnd;
//							}
//						}
//					}
//				}
//				/********************************************************************
//				 * Подсветка операций
//				 ********************************************************************/
//				foreach(string keyword in operators)
//				{
//					if(posStart+keyword.Length <= text.Length)
//					{
//						string testStr = text.Substring(posStart, keyword.Length).ToLower();
//						if(testStr == keyword.ToLower())
//						{
//							bool isKeyword = true;
//							
//							if(posStart > 0)
//							{
//								string prevChar = text.Substring(posStart-1, 1);
//								if(!(strDelimeters.Contains(prevChar)))
//								{
//									isKeyword = false;
//								}
//							}
//							
//							if(posStart+keyword.Length < text.Length-1)
//							{
//								string postChar = text.Substring(posStart+keyword.Length, 1);
//								if(!(strDelimeters.Contains(postChar)))
//								{
//									isKeyword = false;
//								}
//							}
//							if(isKeyword)
//							{
//								posEnd = posStart+keyword.Length;
//								richTextBoxResult.Select(posStart, posEnd-posStart);
//								richTextBoxResult.SelectionFont = fontBold;
//								richTextBoxResult.SelectionColor = Color.LightSeaGreen;
//								posStart = posEnd;
//							}
//						}
//					}
//				}
//				/********************************************************************
//				 * Подсветка констант
//				 ********************************************************************/
//				foreach(string keyword in constants)
//				{
//					if(posStart+keyword.Length <= text.Length)
//					{
//						string testStr = text.Substring(posStart, keyword.Length).ToLower();
//						if(testStr == keyword.ToLower())
//						{
//							bool isKeyword = true;
//							
//							if(posStart > 0)
//							{
//								string prevChar = text.Substring(posStart-1, 1);
//								if(!(strDelimeters.Contains(prevChar)))
//								{
//									isKeyword = false;
//								}
//							}
//							
//							if(posStart+keyword.Length < text.Length-1)
//							{
//								string postChar = text.Substring(posStart+keyword.Length, 1);
//								if(!(strDelimeters.Contains(postChar)))
//								{
//									isKeyword = false;
//								}
//							}
//							if(isKeyword)
//							{
//								posEnd = posStart+keyword.Length;
//								richTextBoxResult.Select(posStart, posEnd-posStart);
//								richTextBoxResult.SelectionFont = fontBold;
//								richTextBoxResult.SelectionColor = Color.DarkBlue;
//								posStart = posEnd;
//							}
//						}
//					}
//				}
//				
//				if(posStart == text.Length)
//				{
//					break;
//				}
//				switch (text.Substring(posStart, 1))
//				{
//					case ";":
//							posEnd = posStart;
//							richTextBoxResult.Select(posStart, 1);
//							richTextBoxResult.SelectionColor = Color.SeaGreen;
//						break;
//					case "&":
//					case "+":
//					case "-":
//					case "*":
//							posEnd = posStart;
//							richTextBoxResult.Select(posStart, 1);
//							richTextBoxResult.SelectionFont = fontBold;
//						break;
//					case "=":
//					case "<":
//					case ">":
//							posEnd = posStart;
//							richTextBoxResult.Select(posStart, 1);
//							richTextBoxResult.SelectionColor = Color.SteelBlue;
//						break;
//					case "(":
//					case ")":
//					case "[":
//					case "]":
//							posEnd = posStart;
//							richTextBoxResult.Select(posStart, 1);
//							richTextBoxResult.SelectionFont = fontBold;
//						break;
//					case "0":
//					case "1":
//					case "2":
//					case "3":
//					case "4":
//					case "5":
//					case "6":
//					case "7":
//					case "8":
//					case "9":
//						bool isNum = false;
//						if(posStart == 0)
//						{
//							isNum = true;
//						}
//						else
//						{
//							string prevChar = text.Substring(posStart-1, 1);
//							if(strDelimeters.Contains(prevChar))
//							{
//								isNum = true;
//							}
//						}
//						if(isNum)
//						{
//							string strDigits = "0123456789";
//							posEnd = posStart;
//							while((posEnd < text.Length) && strDigits.Contains(text.Substring(posEnd, 1)))
//							{
//								posEnd = posEnd + 1;
//							}
//							if(!strDigits.Contains(text.Substring(posEnd, 1)))
//							{
//								posEnd = posEnd - 1;
//							}
//							richTextBoxResult.Select(posStart, posEnd-posStart+1);
//							richTextBoxResult.SelectionColor = Color.DarkRed;
//							posStart = posEnd;
//						}
//						break;
//					case "'":
//						posEnd = text.IndexOf("'", posStart+1);
//						if(posEnd > posStart)
//						{
//							richTextBoxResult.Select(posStart, posEnd-posStart+1);
//							richTextBoxResult.SelectionColor = Color.Blue;
//							posStart = posEnd;
//						}
//						break;
//					case "\"":
//						posEnd = text.IndexOf("\"", posStart+1);
//						if(posEnd > posStart)
//						{
//							richTextBoxResult.Select(posStart, posEnd-posStart+1);
//							richTextBoxResult.SelectionColor = Color.Blue;
//							posStart = posEnd;
//						}
//						break;
//					case "/":
//						switch(text.Substring(posStart, 2))
//						{
//							case "/*":
//								posEnd = text.IndexOf("*/", posStart+2);
//								if(posEnd > posStart)
//								{
//									richTextBoxResult.Select(posStart, posEnd-posStart+2);
//									richTextBoxResult.SelectionColor = Color.Green;
//									posStart = posEnd+1;
//								}
//								break;
//							case "//":
//								posEnd = text.IndexOf("\n", posStart+1);
//								if(posEnd >= posStart)
//								{
//								}
//								else
//								{
//									posEnd = text.Length;
//								}
//								richTextBoxResult.Select(posStart, posEnd-posStart+1);
//								richTextBoxResult.SelectionColor = Color.Green;
//								posStart = posEnd;
//								break;
//						}
//						break;
//				}
//				
//				posStart = posStart + 1;
//			}
//			
//			//Подсветка искомого текста
//			if(searchStr != "")
//			{
//				char[] charsDelimeters = {'\n', '\t', ' ', '\r'};
//				string[] strs = searchStr.Split(charsDelimeters);
//				int minumumPosStart = 0;
//				int indexStrs = 0;
//				for(indexStrs = 0; indexStrs < strs.Length; indexStrs++)
//				{
//					string hlStr = strs[indexStrs].Trim();
//					if(hlStr != "")
//					{
//						posEnd = 0;
//						posStart = text.IndexOf(hlStr, 0, StringComparison.OrdinalIgnoreCase);
//						while(posStart >= 0)
//						{
//							if((minumumPosStart == 0) || (minumumPosStart > posStart))
//							{
//								minumumPosStart = posStart;
//							}
//							posEnd = posStart + hlStr.Length-1;
//							if(posEnd >= 0)
//							{
//								richTextBoxResult.SelectionStart = posStart;
//								richTextBoxResult.Select(posStart, posEnd-posStart+1);
//								richTextBoxResult.SelectionBackColor = Color.Yellow;
//								richTextBoxResult.SelectionColor = Color.DarkGoldenrod;
//								richTextBoxResult.SelectionFont = fontBoldUnderline;
//								posStart = text.IndexOf(hlStr, posEnd+1,  StringComparison.OrdinalIgnoreCase);
//							}
//							else
//							{
//								posStart = -1;
//							}
//						}
//					}
//				}
//				richTextBoxResult.Location = new Point(0,0);
//				richTextBoxResult.ScrollToCaret();
//				richTextBoxResult.Location = richTextBoxResult.GetPositionFromCharIndex(minumumPosStart);
//				richTextBoxResult.ScrollToCaret();
//			}
//		}
//		



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
			(sender as TextBox).BackColor = this.textBoxSearch.BackColor;
			(sender as TextBox).ForeColor = this.textBoxSearch.ForeColor;
		}
		
		void RichTextBoxResultHScroll(object sender, EventArgs e)
		{
			
		}
		
		void RichTextBoxResultVScroll(object sender, EventArgs e)
		{
			
		}
		
//		void RichTextBoxResult_TextChanged(object sender, System.EventArgs e)
//		{
//			if(this.richTextBoxResult.Lines.Length > this.richTextBoxLineNumbers.Lines.Length)
//			{
//				for(int index = this.richTextBoxLineNumbers.Lines.Length + 1; index <= this.richTextBoxResult.Lines.Length; index++)
//				{
//					this.richTextBoxLineNumbers.AppendText(index.ToString("0000") + System.Environment.NewLine);
//				}
//			}
//			if(this.richTextBoxResult.Lines.Length < this.richTextBoxLineNumbers.Lines.Length)
//			{
//				int indexForRemove = this.richTextBoxResult.Lines.Length*(4+System.Environment.NewLine.Length)-1;
//				if (indexForRemove > 0)
//				{
//					//this.richTextBoxLineNumbers.Text.Remove(indexForRemove);
//				}
//				else
//				{
//					this.richTextBoxLineNumbers.Clear();
//				}
//			}
//			
//			//Получить координаты первого символа
//			int indexFirstChar = this.richTextBoxResult.GetCharIndexFromPosition(new System.Drawing.Point(0, 0));
//			int indexFirstLine = this.richTextBoxResult.GetLineFromCharIndex(indexFirstChar);
//			//Получить координаты последнего символа
//			
//		}

		//Рекурсивный поиск по дереву разработки
		bool FilterNodeByName (Node node, string nameFilter)
		{
			if(node == null)
				return false;

			bool isFound = false;
			//Пропуск пустых поисковых строк
			if (node.Name.ToUpper ().Contains (nameFilter.ToUpper ()))
			{
				SetVisible (node, true);
				isFound = true;
			}
			else
			{
				if(node.Nodes != null)
				{
					foreach(Node subNode in node.Nodes)
					{
						if(FilterNodeByName(subNode, nameFilter))
						{
							isFound = true;
						}
					}
				}
			}
			node.Visible = isFound;
			return isFound;
		}

		void SetVisible (Node node, bool isVisible)
		{
			if(node == null)
				return;
			node.Visible = isVisible;
			if (node.Nodes != null)
			{
				foreach(Node subNode in node.Nodes)
				{
					SetVisible(subNode, isVisible);
				}
			}
		}


		/// <summary>
		/// Изменение текста в поле фильтрации дерева проекта
		/// </summary>
		void TextBoxFilter_TextChanged (object sender, System.EventArgs e)
		{
			if(ISBLNodes == null)
				return;
			if (textBoxFilter.Text == "")
			{
				foreach (Node node in ISBLNodes)
				{
					SetVisible (node, true);
				}
			}
			else
			{
				foreach (Node node in ISBLNodes)
				{
					FilterNodeByName (node, textBoxFilter.Text);
				}
			}
			treeViewResults.Nodes.Clear();
			foreach(Node isblNode in ISBLNodes)
			{
				LoadSubNodes(this.treeViewResults.Nodes, isblNode);
			}
		}
		
		void ButtonRefreshClick(object sender, EventArgs e)
		{
			GetISBL();
		}
		
		void CheckBoxWinAuthCheckedChanged(object sender, EventArgs e)
		{
			textBoxLogin.Enabled = !checkBoxWinAuth.Checked;
			textBoxPassword.Enabled = textBoxLogin.Enabled;
		}
		
	}
}
