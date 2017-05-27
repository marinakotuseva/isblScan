/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Вычисления из справочников.
	/// </summary>
	public class CustomCalculations : LoaderCommon
	{
		public CustomCalculations(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

        private class Calculation
        {
            public string ReferenceName { get; set; }
            public string RequisiteName { get; set; }
            public string CalculationName { get; set; }
            public string TableName { get; set; }
            public bool IsTextRequisite { get; set; }
        }

        private class Config 
        {
            public List<Calculation> Calculations{ get; set; }
        }

        private Config LoadConfig()
        {
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CustomCalculationConfig.json");
            var jsonSettings = File.ReadAllText(configFilePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(jsonSettings);
        }

        public Node Load()
		{
            Node listNode = null;
            listNode = new Node();
            listNode.Name = "Custom Calculations";
            listNode.Text = null;
            listNode.Nodes = new List<Node>();

            var config = LoadConfig();

            foreach (Calculation setting in config.Calculations)
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                if (setting.IsTextRequisite)
                {
                    command.CommandText = @"
select ref.NameAn + '(' + cast(ref.Analit as varchar) + ')'
    , MBText." + setting.RequisiteName + @"
from MBAnalitSpr ref
    join " + setting.TableName + @" tbl on tbl.Analit = ref.Analit
    join MBText on tbl.XRecID = MBText.SrcRecID and MBText.SrcObjID = (select XRecID from XObj where TblName like '" + setting.TableName + @"')
where ref.Vid = (select Vid from MBVidAn where Kod = '" + setting.ReferenceName + "')";
                }
                else
                {
                    command.CommandText = @"
select ref.NameAn + '(' + cast(ref.Analit as varchar) + ')'
    , tbl." + setting.RequisiteName + @"
from MBAnalitSpr ref
    join " + setting.TableName + @" tbl on tbl.Analit = ref.Analit
where ref.Vid = (select Vid from MBVidAn where Kod = '" + setting.ReferenceName + "')";
                }
                
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Node refNode = new Node();
                    refNode.Nodes = new List<Node>();
                    refNode.Name = setting.CalculationName;
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(1))
                        {
                            string calculation = "";
                            if (setting.IsTextRequisite)
                            {
                                var Bytes = (byte[])reader.GetValue(1);
                                calculation = System.Text.Encoding.UTF8.GetString(Bytes);
                            }
                            else
                            {
                                calculation = reader.GetString(1);
                            }

                            Node recordNode = new Node();
                            recordNode.Text = calculation;
                            recordNode.Name = reader.GetString(0);
                            refNode.Nodes.Add(recordNode);
                        }
                    }
                    listNode.Nodes.Add(refNode);
                }
                reader.Close();
            }
            return listNode;
        }
	}
}
