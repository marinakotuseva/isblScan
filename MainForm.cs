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

namespace isblTest
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		isblTest.Loader isblLoader;
		Font fontBold;
		Font fontBoldUnderline;

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			isblLoader = new isblTest.Loader();
			
			string sqlServer;
			string dataBase;
			string login;
			
			if(isblTest.Configuration.Load(out sqlServer, out dataBase, out login))
			{
				this.textBoxSQLServer.Text = sqlServer;
				this.textBoxDB.Text = dataBase;
				this.textBoxLogin.Text = login;
				this.textBoxPassword.Text = "";
			}
			//
			// TODO: Добавить хороший список предопределённых строк.
			//
			string[] defaultSearchStrings = 
			{
				"AddWhere",
				"AddFrom",
				"AddOrderBy",
				"SQL(",
				"Выполнить",
				"Execute",
				""
			};
			foreach(string searchString in defaultSearchStrings)
			{
				this.textBoxSearch.Text += (searchString + System.Environment.NewLine);
			}
			//
			// TODO: Добавить хорошую стправку на начальной странице
			//
			this.richTextBoxResult.Text = @"ISBLTest
Программа поиска в текстах прикладной разработки навигации по прикладной разработке.
-=[ Элементы интерфейса ]=-

-=[ Возможности программы ]=-
";
			HighLight("", null);
		}
		
		void LoadSubNodes(TreeNodeCollection treeNodes, isblTest.Node isblNode)
		{
			if(isblNode != null)
			{
				TreeNode treeNode = treeNodes.Add(isblNode.Name);
				if(isblNode.Text != null)
				{
					treeNode.Tag = isblNode.Text;
				}
				if(isblNode.Nodes != null)
				{
					foreach(isblTest.Node isblSubNode in isblNode.Nodes)
					{
						LoadSubNodes(treeNode.Nodes, isblSubNode);
					}
				}
			}
		}
		void ButtonConnectClick(object sender, EventArgs e)
		{

		}
		
		void TreeViewResultsAfterSelect(object sender, TreeViewEventArgs e)
		{
			if(e.Node.Tag != null)
			{
				
				richTextBoxResult.Enabled = false;
				richTextBoxResult.ClearUndo();
				richTextBoxResult.Clear();
				richTextBoxResult.Text = e.Node.Tag.ToString();
				HighLight(textBoxSearch.Text.Trim(), e.Node);
				richTextBoxResult.Enabled = true;
				richTextBoxResult.ScrollBars =  RichTextBoxScrollBars.ForcedBoth;
			}
			else
			{
				richTextBoxResult.Text = "";
				richTextBoxResult.Enabled = false;
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
				char[] arrDelimeters = {'\n'};
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
		
		
		/// <summary>
		///Подстветка специальных конструкций в тексте 
		/// </summary>
		private void HighLightSpecialConstruction(ref int posStart, ref int posEnd)
		{
			//Если текущая позиция в начале строки
			if((posStart == 0)||((posStart > 0)&&(richTextBoxResult.Text.Substring(posStart-1, 1)==System.Environment.NewLine)))
			{
				/********************************************************************
				 * Подсветка специальных конструкций (заголовков, вставленных программой)
				 ********************************************************************/
				if((posStart+4<richTextBoxResult.Text.Length)&&(richTextBoxResult.Text.Substring(posStart, 4)=="-=[ "))
				{
					posEnd = richTextBoxResult.Text.IndexOf(" ]=-", posStart+1);
					if(posEnd > posStart)
					{
						posEnd = posEnd+4;
						richTextBoxResult.Select(posStart, posEnd-posStart);
						richTextBoxResult.SelectionFont = this.fontBoldUnderline;
						richTextBoxResult.SelectionBackColor = Color.LightGray;
						posStart = posEnd;
					}
				}
			}
		}
		/// <summary>
		///Подсветка синтаксиса 
		/// </summary>
		/// <param name="searchStr">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="treeNode">
		/// A <see cref="TreeNode"/>
		/// </param>
		public void HighLight(string searchStr,  TreeNode treeNode)
		{
			String text = richTextBoxResult.Text.Replace("\r", "\n");
			string strDelimeters = "%^&*()-=+\\/;:<>.,?[]{}\n\t ";

			this.fontBold = new Font(richTextBoxResult.SelectionFont, FontStyle.Bold);
			this.fontBoldUnderline = new Font(richTextBoxResult.SelectionFont, FontStyle.Bold | FontStyle.Underline);
			
			//Подстветка строк
			int posEnd = 0;
			int posStart = 0;
			
			string[] keywords = {"if", "endif", "если", "конецесли", "else", "иначе",
				"while", "endwhile", "пока", "конецпока",
				"try", "catch", "endtry",
				"foreach", "endforeach", "in", "все", "конецвсе", "в"
			};
			string[] operators = {"or", "или", 
				"and", "и",
				"not", "не"
			};
			string[] constants = {
				//Основные константы
				"true", "false",
				"cr", "tab",
				"YES_VALUE", "NO_VALUE",
				"null", "nil",
				//Запуск внешних приложений
				"smHidden", "smMaximized", "smMinimized", "smNormal", "wmNo", "wmYes",
				//Работа с диалоговыми окнами
				"cbsCommandLinks", "cbsDefault", "mrCancel", "mrOk", "ATTENTION_CAPTION", "CONFIRMATION_CAPTION", "ERROR_CAPTION", "INFORMATION_CAPTION",
				//Работа с ISBL-редактором
				"ISBL_SYNTAX", "NO_SYNTAX",
				//Работа с электронными документами
				"EDOC_VERSION_ACTIVE_STAGE_CODE", "EDOC_VERSION_DESIGN_STAGE_CODE", "EDOC_VERSION_OBSOLETE_STAGE_CODE",
				//Работа с ЭЦП и шифрованием
				"cpDataEnciphermentEnabled", "cpDigitalSignatureEnabled", "cpID", "cpIssuer", "cpSerial", "cpSubjectName", "cpSubjSimpleName", "cpValidFromDate", "cpValidToDate",
				//События объектов
				"dseBeforeOpen", "dseAfterOpen", "dseBeforeClose", "dseAfterClose", "dseOnValidDelete", "dseBeforeDelete", "dseAfterDelete", "dseAfterDeleteOutOfTransaction", "dseOnDeleteError", "dseBeforeInsert", "dseAfterInsert", "dseOnValidUpdate", "dseBeforeUpdate", "dseOnUpdateRatifiedRecord", "dseAfterUpdate", "dseAfterUpdateOutOfTransaction", "dseOnUpdateError", "dseAfterScroll", "dseOnOpenRecord", "dseOnCloseRecord", "dseBeforeCancel", "dseAfterCancel", "reOnChange",
				//Идентификаторы правил
				"AUTO_NUMERATION_RULE_ID", "CANT_CHANGE_ID_REQUISITE_RULE_ID", "CANT_CHANGE_OURFIRM_REQUISITE_RULE_ID", "CHECK_CHANGING_REFERENCE_RECORD_USE_RULE_ID", "CHECK_CODE_REQUISITE_RULE_ID", "CHECK_DELETING_REFERENCE_RECORD_USE_RULE_ID", "CHECK_FILTRATER_CHANGES_RULE_ID", "CHECK_REFERENCE_INTERVAL_RULE_ID", "CHECK_REQUIRED_REQUISITES_FULLNESS_RULE_ID", "MAKE_RECORD_UNRATIFIED_RULE_ID", "RESTORE_AUTO_NUMERATION_RULE_ID", "SET_DEFAULT_FIRM_CONTEXT_RULE_ID", "SET_DEPARTMENT_SECTION_BOUNDS_RULE_ID", "SET_FIRM_CONTEXT_FROM_RECORD_RULE_ID", "SET_FIRST_RECORD_IN_LIST_FORM_RULE_ID", "SET_IDSPS_VALUE_RULE_ID", "SET_NEXT_CODE_VALUE_RULE_ID", "SET_OURFIRM_BOUNDS_RULE_ID", "SET_OURFIRM_REQUISITE_RULE_ID",
				//Параметры объектов
				"SHOW_RECORD_PROPERTIES_FORM", "PREVIOUS_CARD_TYPE_NAME",
				//Типы объектов системы в таблице связей
				"EDOCUMENT_LINK_KIND", "FOLDER_LINK_KIND", "TASK_LINK_KIND", "JOB_LINK_KIND", "DOCUMENT_LINK_KIND", "REFERENCE_LINK_KIND",
				//Дополнительные типы блокируемых объектов
				"EDOCUMENT_VERSION_LOCK_TYPE", "COMPONENT_TOKEN_LOCK_TYPE",
				//Прочие константы
				"ISBSYSDEV", "USER_NAME_FORMAT",  "MEMORY_DATASET_DESRIPTIONS_FILENAME",  "FILTER_OPERANDS_DELIMITER",  "FILTER_OPERATIONS_DELIMITER", 
				//Реквизиты справочников
				"SYSREQ_ID", "SYSREQ_STATE", "SYSREQ_NAME", "SYSREQ_NAME_LOCALIZE_ID", "SYSREQ_DESCRIPTION", "SYSREQ_DESCRIPTION_LOCALIZE_ID", "SYSREQ_NOTE", "SYSREQ_CONTENTS", "SYSREQ_CODE", "SYSREQ_TYPE", "SYSREQ_LAST_UPDATE", "SYSREQ_OUR_FIRM", "SYSREQ_LEADER_REFERENCE", "SYSREQ_ORIGINAL_RECORD", "SYSREQ_DOUBLE", "SYSREQ_RECORD_STATUS", "SYSREQ_UNIT", "SYSREQ_UNIT_ID", "SYSREQ_MAIN_RECORD_ID", "SYSREQ_LINE_NUMBER",
				//Предопределенные реквизиты электронных документов
				"SYSREQ_EDOC_AUTHOR", "SYSREQ_EDOC_CREATED", "SYSREQ_EDOC_EDITOR", "SYSREQ_EDOC_ENCODE_TYPE", "SYSREQ_EDOC_ENCRYPTION_PLUGIN_NAME", "SYSREQ_EDOC_EXPORTER", "SYSREQ_EDOC_EXPORT_DATE", "SYSREQ_EDOC_KIND", "SYSREQ_EDOC_LIFE_STAGE_NAME", "SYSREQ_EDOC_LOCKED_FOR_SERVER_CODE", "SYSREQ_EDOC_MODIFIED", "SYSREQ_EDOC_NAME", "SYSREQ_EDOC_NOTE", "SYSREQ_EDOC_SIGNATURE_TYPE", "SYSREQ_EDOC_SIGNED", "SYSREQ_EDOC_STORAGE", "SYSREQ_EDOC_TEXT_MODIFIED", "SYSREQ_EDOC_ACCESS_TYPE", "SYSREQ_EDOC_QUALIFIED_ID", "SYSREQ_EDOC_SESSION_KEY", "SYSREQ_EDOC_SESSION_KEY_ENCRYPTION_PLUGIN_NAME", "SYSREQ_EDOC_VERSION_AUTHOR", "SYSREQ_EDOC_VERSION_CRC", "SYSREQ_EDOC_VERSION_DATA", "SYSREQ_EDOC_VERSION_EDITOR", "SYSREQ_EDOC_VERSION_EXPORT_DATE", "SYSREQ_EDOC_VERSION_HIDDEN", "SYSREQ_EDOC_VERSION_LIFE_STAGE", "SYSREQ_EDOC_VERSION_LOCKED_FOR_SERVER_CODE", "SYSREQ_EDOC_VERSION_MODIFIED", "SYSREQ_EDOC_VERSION_NOTE", "SYSREQ_EDOC_VERSION_SIGNATURE_TYPE", "SYSREQ_EDOC_VERSION_SIGNED", "SYSREQ_EDOC_VERSION_SIZE", "SYSREQ_EDOC_VERSION_SOURCE", "SYSREQ_EDOC_VERSION_TEXT_MODIFIED",

				"SYSREQ_CODE", "SYSREQ_CONTENTS", "SYSREQ_EDOC_NAME", "SYSREQ_ID", "SYSREQ_EDOC_KIND", "SYSRES_SBDATA",
				"References", "EDocuments", "Application",
				"Object", "Sender", 
				};

			while(posStart < text.Length)
			{
				HighLightSpecialConstruction(ref posStart, ref posEnd);
				/*
				//Если текущая позиция в начале строки
				if((posStart == 0)||((posStart > 0)&&(text.Substring(posStart-1, 1)==System.Environment.NewLine)))
				{
					// ----------------------------------------------------------------------
					// Подсветка специальных конструкций (заголовков, вставленных программой)
					// ----------------------------------------------------------------------
					if((posStart+4<text.Length)&&(text.Substring(posStart, 4)=="-=[ "))
					{
						posEnd = text.IndexOf(" ]=-", posStart+1);
						if(posEnd > posStart)
						{
							posEnd = posEnd+4;
							richTextBoxResult.Select(posStart, posEnd-posStart);
							richTextBoxResult.SelectionFont = fontBoldUnderline;
							richTextBoxResult.SelectionBackColor = Color.LightGray;
							posStart = posEnd;
						}
					}
				}
				*/
				/********************************************************************
				 * Подсветка ключевых слов
				 ********************************************************************/
				foreach(string keyword in keywords)
				{
					if(posStart+keyword.Length <= text.Length)
					{
						string testStr = text.Substring(posStart, keyword.Length).ToLower();
						if(testStr == keyword.ToLower())
						{
							bool isKeyword = true;
							
							if(posStart > 0)
							{
								string prevChar = text.Substring(posStart-1, 1);
								if(!(strDelimeters.Contains(prevChar)))
								{
									isKeyword = false;
								}
							}
							
							if(posStart+keyword.Length < text.Length-1)
							{
								string postChar = text.Substring(posStart+keyword.Length, 1);
								if(!(strDelimeters.Contains(postChar)))
								{
									isKeyword = false;
								}
							}
							if(isKeyword)
							{
								posEnd = posStart+keyword.Length;
								richTextBoxResult.Select(posStart, posEnd-posStart);
								richTextBoxResult.SelectionFont = fontBold;
								posStart = posEnd;
							}
						}
					}
				}
				/********************************************************************
				 * Подсветка операций
				 ********************************************************************/
				foreach(string keyword in operators)
				{
					if(posStart+keyword.Length <= text.Length)
					{
						string testStr = text.Substring(posStart, keyword.Length).ToLower();
						if(testStr == keyword.ToLower())
						{
							bool isKeyword = true;
							
							if(posStart > 0)
							{
								string prevChar = text.Substring(posStart-1, 1);
								if(!(strDelimeters.Contains(prevChar)))
								{
									isKeyword = false;
								}
							}
							
							if(posStart+keyword.Length < text.Length-1)
							{
								string postChar = text.Substring(posStart+keyword.Length, 1);
								if(!(strDelimeters.Contains(postChar)))
								{
									isKeyword = false;
								}
							}
							if(isKeyword)
							{
								posEnd = posStart+keyword.Length;
								richTextBoxResult.Select(posStart, posEnd-posStart);
								richTextBoxResult.SelectionFont = fontBold;
								richTextBoxResult.SelectionColor = Color.LightSeaGreen;
								posStart = posEnd;
							}
						}
					}
				}
				/********************************************************************
				 * Подсветка констант
				 ********************************************************************/
				foreach(string keyword in constants)
				{
					if(posStart+keyword.Length <= text.Length)
					{
						string testStr = text.Substring(posStart, keyword.Length).ToLower();
						if(testStr == keyword.ToLower())
						{
							bool isKeyword = true;
							
							if(posStart > 0)
							{
								string prevChar = text.Substring(posStart-1, 1);
								if(!(strDelimeters.Contains(prevChar)))
								{
									isKeyword = false;
								}
							}
							
							if(posStart+keyword.Length < text.Length-1)
							{
								string postChar = text.Substring(posStart+keyword.Length, 1);
								if(!(strDelimeters.Contains(postChar)))
								{
									isKeyword = false;
								}
							}
							if(isKeyword)
							{
								posEnd = posStart+keyword.Length;
								richTextBoxResult.Select(posStart, posEnd-posStart);
								richTextBoxResult.SelectionFont = fontBold;
								richTextBoxResult.SelectionColor = Color.DarkBlue;
								posStart = posEnd;
							}
						}
					}
				}
				
				if(posStart == text.Length)
				{
					break;
				}
				switch (text.Substring(posStart, 1))
				{
					case ";":
							posEnd = posStart;
							richTextBoxResult.Select(posStart, 1);
							richTextBoxResult.SelectionColor = Color.SeaGreen;
						break;
					case "&":
					case "+":
					case "-":
					case "*":
							posEnd = posStart;
							richTextBoxResult.Select(posStart, 1);
							richTextBoxResult.SelectionFont = fontBold;
						break;
					case "=":
					case "<":
					case ">":
							posEnd = posStart;
							richTextBoxResult.Select(posStart, 1);
							richTextBoxResult.SelectionColor = Color.SteelBlue;
						break;
					case "(":
					case ")":
					case "[":
					case "]":
							posEnd = posStart;
							richTextBoxResult.Select(posStart, 1);
							richTextBoxResult.SelectionFont = fontBold;
						break;
					case "0":
					case "1":
					case "2":
					case "3":
					case "4":
					case "5":
					case "6":
					case "7":
					case "8":
					case "9":
						bool isNum = false;
						if(posStart == 0)
						{
							isNum = true;
						}
						else
						{
							string prevChar = text.Substring(posStart-1, 1);
							if(strDelimeters.Contains(prevChar))
							{
								isNum = true;
							}
						}
						if(isNum)
						{
							string strDigits = "0123456789";
							posEnd = posStart;
							while((posEnd < text.Length) && strDigits.Contains(text.Substring(posEnd, 1)))
							{
								posEnd = posEnd + 1;
							}
							if(!strDigits.Contains(text.Substring(posEnd, 1)))
							{
								posEnd = posEnd - 1;
							}
							richTextBoxResult.Select(posStart, posEnd-posStart+1);
							richTextBoxResult.SelectionColor = Color.DarkRed;
							posStart = posEnd;
						}
						break;
					case "'":
						posEnd = text.IndexOf("'", posStart+1);
						if(posEnd > posStart)
						{
							richTextBoxResult.Select(posStart, posEnd-posStart+1);
							richTextBoxResult.SelectionColor = Color.Blue;
							posStart = posEnd;
						}
						break;
					case "\"":
						posEnd = text.IndexOf("\"", posStart+1);
						if(posEnd > posStart)
						{
							richTextBoxResult.Select(posStart, posEnd-posStart+1);
							richTextBoxResult.SelectionColor = Color.Blue;
							posStart = posEnd;
						}
						break;
					case "/":
						switch(text.Substring(posStart, 2))
						{
							case "/*":
								posEnd = text.IndexOf("*/", posStart+2);
								if(posEnd > posStart)
								{
									richTextBoxResult.Select(posStart, posEnd-posStart+2);
									richTextBoxResult.SelectionColor = Color.Green;
									posStart = posEnd+1;
								}
								break;
							case "//":
								posEnd = text.IndexOf("\n", posStart+1);
								if(posEnd >= posStart)
								{
								}
								else
								{
									posEnd = text.Length;
								}
								richTextBoxResult.Select(posStart, posEnd-posStart+1);
								richTextBoxResult.SelectionColor = Color.Green;
								posStart = posEnd;
								break;
						}
						break;
				}
				
				posStart = posStart + 1;
			}
			
			//Подсветка искомого текста
			if(searchStr != "")
			{
				char[] charsDelimeters = {'\n'};
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
								richTextBoxResult.SelectionStart = posStart;
								richTextBoxResult.Select(posStart, posEnd-posStart+1);
								richTextBoxResult.SelectionBackColor = Color.Yellow;
								richTextBoxResult.SelectionColor = Color.DarkGoldenrod;
								richTextBoxResult.SelectionFont = fontBoldUnderline;
								posStart = text.IndexOf(hlStr, posEnd+1,  StringComparison.OrdinalIgnoreCase);
							}
							else
							{
								posStart = -1;
							}
						}
					}
				}
				richTextBoxResult.Location = new Point(0,0);
				richTextBoxResult.ScrollToCaret();
				richTextBoxResult.Location = richTextBoxResult.GetPositionFromCharIndex(minumumPosStart);
				richTextBoxResult.ScrollToCaret();
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
				if((textBoxSQLServer.Text.Trim() != "")&&(textBoxDB.Text.Trim() != "")&&(textBoxLogin.Text.Trim() != ""))
				{
					if(isblLoader.Connect(textBoxSQLServer.Text, textBoxDB.Text, textBoxLogin.Text, textBoxPassword.Text))
					{
						isblTest.Configuration.Save(textBoxSQLServer.Text, textBoxDB.Text, textBoxLogin.Text);
						List<isblTest.Node> isblNodes = isblLoader.Load();
						treeViewResults.Nodes.Clear();
						foreach(isblTest.Node isblNode in isblNodes)
						{
							LoadSubNodes(this.treeViewResults.Nodes, isblNode);
						}
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
		void RichTextBoxResult_TextChanged(object sender, System.EventArgs e)
		{
			if(this.richTextBoxResult.Lines.Length > this.richTextBoxLineNumbers.Lines.Length)
			{
				for(int index = this.richTextBoxLineNumbers.Lines.Length + 1; index <= this.richTextBoxResult.Lines.Length; index++)
				{
					this.richTextBoxLineNumbers.AppendText(index.ToString("0000") + System.Environment.NewLine);
				}
			}
			if(this.richTextBoxResult.Lines.Length < this.richTextBoxLineNumbers.Lines.Length)
			{
				int indexForRemove = this.richTextBoxResult.Lines.Length*(4+System.Environment.NewLine.Length)-1;
				if (indexForRemove > 0)
				{
					this.richTextBoxLineNumbers.Text.Remove(indexForRemove);
				}
				else
				{
					this.richTextBoxLineNumbers.Clear();
				}
			}
			
			//Получить координаты первого символа
			int indexFirstChar = this.richTextBoxResult.GetCharIndexFromPosition(new System.Drawing.Point(0, 0));
			int indexFirstLine = this.richTextBoxResult.GetLineFromCharIndex(indexFirstChar);
			//Получить координаты последнего символа
			
		}
	}
}
