using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ISBLScan.ViewCode.Models.ISX
{
    public class Component
    {
        [XmlAttribute(AttributeName = "KeyValue")]
        public string KeyValue { get; set; }
        [XmlAttribute(AttributeName = "DisplayValue")]
        public string DisplayValue { get; set; }
        [XmlAttribute(AttributeName = "CompHash")]
        public string CompHash { get; set; }
        [XmlAttribute(AttributeName = "AutoAdded")]
        public string AutoAdded { get; set; }

        [XmlElement(ElementName = "Requisites")]
        public Requisites Requisites { get; set; }

        [XmlElement(ElementName = "DetailDataSet")]
        public DetailDataSets DetailDataSet { get; set; }

        public Components Components { get; set; }
    }

    [XmlRoot(ElementName = "Requisite")]
    public class Requisite
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "Text")]
        public string TextAttribute { get; set; }
        [XmlText]
        public string XmlText { get; set; }
        [XmlAttribute(AttributeName = "ValueLocalizeID")]
        public string ValueLocalizeID { get; set; }

        public bool IsText
        {
            get { return TextAttribute == "Text"; }
            set { TextAttribute = IsText ? "Text" : null; }
        }
        public string Text
        {
            get { return System.Text.Encoding.GetEncoding(1251).GetString(Convert.FromBase64String(XmlText)); }
            set { XmlText = Convert.ToBase64String(System.Text.Encoding.GetEncoding(1251).GetBytes(Text)); }
        }
    }

    [XmlRoot(ElementName = "Requisites")]
    public class Requisites
    {
        [XmlElement(ElementName = "Requisite")]
        public List<Requisite> Requisite { get; set; }

        public Requisite ByCode(string code)
        {
            return Requisite.Where(r => r.Code == code).SingleOrDefault();
        }
    }

    public class DetailDataSet
    {
        [XmlElement(ElementName = "Requisites")]
        public List<Requisites> Requisites { get; set; }
    }

    public class DetailDataSets
    {
        [XmlElement(ElementName = "DetailDataSet1")]
        public DetailDataSet DetailDataSet1 { get; set; }
        [XmlElement(ElementName = "DetailDataSet2")]
        public DetailDataSet DetailDataSet2 { get; set; }
        [XmlElement(ElementName = "DetailDataSet3")]
        public DetailDataSet DetailDataSet3 { get; set; }
        [XmlElement(ElementName = "DetailDataSet4")]
        public DetailDataSet DetailDataSet4 { get; set; }
        [XmlElement(ElementName = "DetailDataSet5")]
        public DetailDataSet DetailDataSet5 { get; set; }
        [XmlElement(ElementName = "DetailDataSet6")]
        public DetailDataSet DetailDataSet6 { get; set; }
    }

    public class Constants
    {
        [XmlElement(ElementName = "Constants")]
        public List<Component> Components { get; set; }
    }
    public class EDocRequisites
    {
        [XmlElement(ElementName = "EDocRequisites")]
        public List<Component> Components { get; set; }
    }
    public class EDCardTypes
    {
        [XmlElement(ElementName = "EDCardTypes")]
        public List<Component> Components { get; set; }
    }
    public class GrFunctions
    {
        [XmlElement(ElementName = "GrFunctions")]
        public List<Component> Components { get; set; }
    }
    public class LocalizedStrings
    {
        [XmlElement(ElementName = "LocalizedStrings")]
        public List<Component> Components { get; set; }
    }
    public class Modules
    {
        [XmlElement(ElementName = "Modules")]
        public List<Component> Components { get; set; }
    }
    public class RefTypes
    {
        [XmlElement(ElementName = "RefTypes")]
        public List<Component> Components { get; set; }
    }
    public class RefRequisites
    {
        [XmlElement(ElementName = "RefRequisites")]
        public List<Component> Components { get; set; }
    }
    public class WorkflowBlockGroups
    {
        [XmlElement(ElementName = "WorkflowBlockGroups")]
        public List<Component> Components { get; set; }
    }
    public class WorkflowBlocks
    {
        [XmlElement(ElementName = "WorkflowBlocks")]
        public List<Component> Components { get; set; }
    }
    public class Scripts
    {
        [XmlElement(ElementName = "Scripts")]
        public List<Component> Components { get; set; }
    }
    public class Viewers
    {
        [XmlElement(ElementName = "Viewers")]
        public List<Component> Components { get; set; }
    }
    public class Functions
    {
        [XmlElement(ElementName = "Functions")]
        public List<Component> Components { get; set; }
    }
    public class Reports
    {
        [XmlElement(ElementName = "Reports")]
        public List<Component> Components { get; set; }
    }

    [XmlRoot(ElementName = "Components")]
    public class Components
    {
        [XmlElement(ElementName = "Constants")]
        public Constants Constants { get; set; }
        [XmlElement(ElementName = "EDocRequisites")]
        public EDocRequisites EDocRequisites { get; set; }
        [XmlElement(ElementName = "EDCardTypes")]
        public EDCardTypes EDCardTypes { get; set; }
        [XmlElement(ElementName = "GrFunctions")]
        public GrFunctions GrFunctions { get; set; }
        [XmlElement(ElementName = "Functions")]
        public Functions Functions { get; set; }
        [XmlElement(ElementName = "LocalizedStrings")]
        public LocalizedStrings LocalizedStrings { get; set; }
        [XmlElement(ElementName = "Modules")]
        public Modules Modules { get; set; }
        [XmlElement(ElementName = "RefTypes")]
        public RefTypes RefTypes { get; set; }
        [XmlElement(ElementName = "RefRequisites")]
        public RefRequisites RefRequisites { get; set; }
        [XmlElement(ElementName = "Reports")]
        public Reports Reports { get; set; }
        [XmlElement(ElementName = "WorkflowBlockGroups")]
        public WorkflowBlockGroups WorkflowBlockGroups { get; set; }
        [XmlElement(ElementName = "WorkflowBlocks")]
        public WorkflowBlocks WorkflowBlocks { get; set; }
        [XmlElement(ElementName = "Scripts")]
        public Scripts Scripts { get; set; }
        [XmlElement(ElementName = "Viewers")]
        public Viewers Viewers { get; set; }
        [XmlAttribute(AttributeName = "PlatformVesion")]
        public string PlatformVesion { get; set; }
        [XmlAttribute(AttributeName = "SystemMask")]
        public string SystemMask { get; set; }
        [XmlAttribute(AttributeName = "ForMainServer")]
        public string ForMainServer { get; set; }
        [XmlAttribute(AttributeName = "ImitationMode")]
        public string ImitationMode { get; set; }
    }

    public static class Parser
    {
        public static Components Parse(string fileName)
        {
            StreamReader xmlStream = new StreamReader(fileName, System.Text.Encoding.GetEncoding("windows-1251"));
            XmlSerializer serializer = new XmlSerializer(typeof(Components));
            return (Components)serializer.Deserialize(xmlStream);
        }
    }
}

