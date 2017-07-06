using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace ISBLScan.ViewCode
{
    static class ReaderAll
    {
        private static string GetNodeString(XmlNode node)
        {
            string parsedString = "";
            parsedString = node.InnerText;
            parsedString = parsedString.Replace("{5314B05F-CF9F-4F66-99EC-24992A5FB114}", "");
            try
            {
                byte[] data = Convert.FromBase64String(parsedString);
                parsedString = System.Text.Encoding.GetEncoding(1251).GetString(data);
            }
            catch { }
            return parsedString;
        }
        public static void Read(IsbNodesList nodes, ConnectionParams connectionParams)
        {
            if (!String.IsNullOrWhiteSpace(connectionParams.ISXPath))
            {
                var UnitIDReqName = "ИДМодуля"; // "UnitID" 
                var UnitReqName = "Модуль"; // "Unit"  
                var IDReqName = "ИД"; // "ISBID"
                var NameReqName = "Наименование"; // "ISBName"
                var CommentReqName = "Примечание"; //"Description"
                var TextReqName = "Текст"; // "Text"
                var TemplateReqName = "Шаблон"; // "Template"
                var CalculationReqName = "Расчет"; // "Script"
                var TypeReqName = "Тип"; // "Type"
                var RowNumerReqName = "НомСтр"; // "RowNumber"
                var CodeReqName = "Код"; // "ISBCode"
                var DescriptionReqName = "Описание";


                var components = Models.ISX.Parser.Parse(connectionParams.ISXPath);
                if (components.Scripts != null)
                {
                    var itemsNode = new IsbNode("Сценарий (расчёт)");
                    var units = components.Scripts.Components.Select(c => new
                    {
                        Id = int.Parse(c.Requisites.ByCode(UnitIDReqName).Value),
                        Name = c.Requisites.ByCode(UnitReqName).Text
                    }).Distinct();
                    foreach (var unit in units)
                    {
                        var unitNode = new IsbNode(unit.Name);
                        unitNode.Id = unit.Id;
                        var items = components.Scripts.Components
                            .Where(c => int.Parse(c.Requisites.ByCode(UnitIDReqName).Value) == unit.Id)
                            .Select(c => new
                            {
                                Id = int.Parse(c.Requisites.ByCode(IDReqName).Value),
                                Name = c.Requisites.ByCode(NameReqName).Text,
                                Note = c.Requisites.ByCode(DescriptionReqName).Text,
                                Text = c.Requisites.ByCode(TextReqName).Text
                            });
                        foreach (var item in items)
                        {
                            var itemNode = new IsbNode(item.Name);
                            itemNode.Type = IsbNodeType.Script;
                            itemNode.Id = item.Id;
                            itemNode.Text = item.Note;

                            var scriptTextNode = new IsbNode();
                            scriptTextNode.Name = "-=[ Текст сценария ]=-";
                            scriptTextNode.Text = item.Text;
                            itemNode.Nodes.Add(scriptTextNode);

                            unitNode.Nodes.Add(itemNode);
                        }

                        itemsNode.Nodes.Add(unitNode);
                    }
                    nodes.Add(itemsNode);
                }
                if (components.Reports != null)
                {
                    var itemsNode = new IsbNode("Аналитический отчёт");
                    var units = components.Reports.Components
                        .Where(c => c.Requisites.ByCode(TypeReqName).Text != "MBAnalitV")
                        .Select(c => new
                        {
                            Id = int.Parse(c.Requisites.ByCode(UnitIDReqName).Value),
                            Name = c.Requisites.ByCode(UnitReqName).Text
                        }).Distinct();
                    foreach (var unit in units)
                    {
                        var unitNode = new IsbNode(unit.Name);
                        unitNode.Id = unit.Id;
                        var items = components.Reports.Components
                            .Where(c => c.Requisites.ByCode(TypeReqName).Text != "MBAnalitV")
                            .Where(c => int.Parse(c.Requisites.ByCode(UnitIDReqName).Value) == unit.Id)
                            .Select(c => new
                            {
                                Id = int.Parse(c.Requisites.ByCode(IDReqName).Value),
                                Name = c.Requisites.ByCode(NameReqName).Text,
                                Note = c.Requisites.ByCode(DescriptionReqName).Text,
                                Template = c.Requisites.ByCode(TemplateReqName).Text,
                                Text = c.Requisites.ByCode(CalculationReqName).Text
                            });
                        foreach (var item in items)
                        {
                            var itemNode = new IsbNode(item.Name);
                            itemNode.Type = IsbNodeType.Script;
                            itemNode.Id = item.Id;
                            if (!String.IsNullOrWhiteSpace(item.Note))
                            {
                                itemNode.Text = item.Note;
                            }

                            if (!String.IsNullOrWhiteSpace(item.Template))
                            {
                                var reportTextNode = new IsbNode();
                                reportTextNode.Name = "-=[ Шаблон ]=-";
                                reportTextNode.Text = item.Template;
                                itemNode.Nodes.Add(reportTextNode);
                            }

                            if (!String.IsNullOrWhiteSpace(item.Text))
                            {
                                var reportTemplateNode = new IsbNode();
                                reportTemplateNode.Name = "-=[ Расчёт ]=-";
                                reportTemplateNode.Text = item.Text;
                                itemNode.Nodes.Add(reportTemplateNode);
                            }

                            unitNode.Nodes.Add(itemNode);
                        }

                        itemsNode.Nodes.Add(unitNode);
                    }
                    nodes.Add(itemsNode);
                }
                if (components.Reports != null)
                {
                    var itemsNode = new IsbNode("Интегрированный отчёт");

                    var items = components.Reports.Components
                        .Where(c => c.Requisites.ByCode(TypeReqName).Text == "MBAnalitV")
                        .Select(c => new
                        {
                            Id = int.Parse(c.Requisites.ByCode(IDReqName).Value),
                            Name = c.Requisites.ByCode(NameReqName).Text,
                            Note = c.Requisites.ByCode(CommentReqName).Text,
                            Template = c.Requisites.ByCode(TemplateReqName).Text,
                            Text = c.Requisites.ByCode(CalculationReqName).Text
                        });
                    foreach (var item in items)
                    {
                        var itemNode = new IsbNode(item.Name);
                        itemNode.Type = IsbNodeType.Script;
                        itemNode.Id = item.Id;
                        if (!String.IsNullOrWhiteSpace(item.Note))
                        {
                            itemNode.Text = item.Note;
                        }


                        if (!String.IsNullOrWhiteSpace(item.Template))
                        {
                            var reportTextNode = new IsbNode();
                            reportTextNode.Name = "-=[ Шаблон ]=-";
                            reportTextNode.Text = item.Template;
                            itemNode.Nodes.Add(reportTextNode);
                        }

                        if (!String.IsNullOrWhiteSpace(item.Text))
                        {
                            var reportTemplateNode = new IsbNode();
                            reportTemplateNode.Name = "-=[ Расчёт ]=-";
                            reportTemplateNode.Text = item.Text;
                            itemNode.Nodes.Add(reportTemplateNode);
                        }


                        itemsNode.Nodes.Add(itemNode);
                    }
                    nodes.Add(itemsNode);
                }
                if (components.Functions != null)
                {
                    var itemsNode = new IsbNode("Функция");

                    foreach (var type in new string[] { "Пользовательская", "Системная" })
                    {
                        var functionsNode = new IsbNode(type);
                        itemsNode.Nodes.Add(functionsNode);
                        var units = components.Functions.Components
                            .Where(c => c.Requisites.ByCode("ISBFuncCategory").ValueLocalizeID == (type == "Пользовательская" ? "SYSRES_SYSCOMP.FUNCTION_CATEGORY_USER" : "SYSRES_SYSCOMP.FUNCTION_CATEGORY_SYSTEM"))
                            .Select(c => new
                            {
                                Name = c.Requisites.ByCode("ISBFuncGroup").Value
                            }).Distinct();
                        foreach (var unit in units)
                        {
                            var unitNode = new IsbNode(unit.Name);
                            var items = components.Functions.Components
                                .Where(c => c.Requisites.ByCode("ISBFuncCategory").ValueLocalizeID == (type == "Пользовательская" ? "SYSRES_SYSCOMP.FUNCTION_CATEGORY_USER" : "SYSRES_SYSCOMP.FUNCTION_CATEGORY_SYSTEM"))
                                .Where(c => c.Requisites.ByCode("ISBFuncGroup").Value == unit.Name)
                                .Select(c => new
                                {
                                    Id = int.Parse(c.Requisites.ByCode(IDReqName).Value),
                                    Name = c.Requisites.ByCode(NameReqName).Text,
                                    Note = c.Requisites.ByCode("ISBFuncComment").Text,
                                    Text = c.Requisites.ByCode("ISBFuncText").Text,
                                    Params = c.DetailDataSet?.DetailDataSet1.Requisites
                                    .Select(r => r.ByCode(RowNumerReqName).Value + ".\t" + r.ByCode("ISBFuncParamIdent").Text + ".\t" + CodeToNameConverter.FunctionParamTypeIDToName(r.ByCode("ISBFuncParamType").ValueLocalizeID) + ".\t" + "\r\n")
                                    .Aggregate((all, next) => all + next)
                                });
                            foreach (var item in items)
                            {
                                var itemNode = new IsbNode(item.Name);
                                itemNode.Type = IsbNodeType.Function;
                                itemNode.Id = item.Id;

                                if (!String.IsNullOrWhiteSpace(item.Note))
                                {
                                    var funcDescriptionNode = new IsbNode();
                                    funcDescriptionNode.Name = "-=[ Описание функции ]=-";
                                    funcDescriptionNode.Text = item.Note;
                                    itemNode.Nodes.Add(funcDescriptionNode);
                                }
                                if (!String.IsNullOrWhiteSpace(item.Params))
                                {
                                    var funcRecvNode = new IsbNode("-=[ Параметры функции ]=-");
                                    funcRecvNode.Text = item.Params;
                                    itemNode.Nodes.Add(funcRecvNode);
                                }
                                if (!String.IsNullOrWhiteSpace(item.Text))
                                {
                                    var funcTextNode = new IsbNode();
                                    funcTextNode.Name = "-=[ Текст функции ]=-";
                                    funcTextNode.Text = item.Text;
                                    itemNode.Nodes.Add(funcTextNode);
                                }

                                unitNode.Nodes.Add(itemNode);
                            }

                            functionsNode.Nodes.Add(unitNode);
                        }
                    }
                    nodes.Add(itemsNode);
                }
                if (components.WorkflowBlocks != null)
                {
                    var itemsNode = new IsbNode("Блок типового маршрута");
                    var units = components.WorkflowBlocks.Components.Select(c => new
                    {
                        Name = c.Requisites.ByCode("BlockGroup").Value
                    }).Distinct();
                    foreach (var unit in units)
                    {
                        var unitNode = new IsbNode(unit.Name);
                        var items = components.WorkflowBlocks.Components
                            .Where(c => c.Requisites.ByCode("BlockGroup").Value == unit.Name)
                            .Select(c => new
                            {
                                Id = int.Parse(c.Requisites.ByCode(IDReqName).Value),
                                Code = c.Requisites.ByCode(CodeReqName).Text,
                                Note = c.Requisites.ByCode(DescriptionReqName).Text,
                                Properties = c.Requisites.ByCode("Properties").Text
                            });
                        foreach (var item in items)
                        {
                            var itemNode = new IsbNode(item.Code);
                            itemNode.Type = IsbNodeType.RouteBlock;
                            itemNode.Id = item.Id;
                            if (!String.IsNullOrWhiteSpace(item.Note))
                            {
                                itemNode.Text = item.Note;
                            }

                            RouteParser.ParseBlockProperties(item.Properties, itemNode);

                            unitNode.Nodes.Add(itemNode);
                        }

                        itemsNode.Nodes.Add(unitNode);
                    }
                    nodes.Add(itemsNode);
                }
                if (components.RefTypes != null)
                {
                    var itemsNode = new IsbNode("Тип справочника");

                    var items = components.RefTypes.Components
                        .Select(c => new
                        {
                            Id = int.Parse(c.Requisites.ByCode(IDReqName).Value),
                            Code = c.KeyValue.Trim(),
                            Name = c.DisplayValue.Trim() + " (" + c.KeyValue.Trim() + ")",
                            Note = c.Requisites.ByCode("ISBRefTypeComment").Text.Trim(),
                            Events = c.Requisites.ByCode("ISBRefTypeEventText").Text.Trim(),
                            Datasets = c.DetailDataSet
                        });
                    foreach (var item in items)
                    {
                        var itemNode = new IsbNode(item.Name);
                        itemNode.Type = IsbNodeType.ReferenceType;
                        itemNode.Id = item.Id;
                        itemNode.Code = item.Code;
                        if (!String.IsNullOrWhiteSpace(item.Note))
                        {
                            itemNode.Text = item.Note;
                        }
                        if (!String.IsNullOrWhiteSpace(item.Events))
                        {
                            ReferenceEventsParser.ParseEvents(item.Events, itemNode);
                        }
                        var RequisitesSections = item.Datasets.DetailDataSet1.Requisites.Select(r => r.ByCode("ISBRefTypeReqSection").ValueLocalizeID).Distinct();
                        foreach(var section in RequisitesSections)
                        {

                        }

                        if(item.Datasets.DetailDataSet2 != null)
                        {
                            var actionsNode = new IsbNode(ReferenceEventsParser.SectionCodeToName['К']);
                            actionsNode.Code = "К";
                            foreach (var action in item.Datasets.DetailDataSet2.Requisites)
                            {
                                var actionNode = new IsbNode(action.ByCode("ISBRefTypeActDescription").Text);
                                actionNode.Code = action.ByCode("ISBRefTypeActCode").Text;

                                var text = action.ByCode("ISBRefTypeActOnExecute").Text;
                                if (!String.IsNullOrWhiteSpace(text))
                                {
                                    var exprnRefRecvNode = new IsbNode("-=[ Вычисление ]=-");
                                    exprnRefRecvNode.Text = text;
                                    actionNode.Nodes.Add(exprnRefRecvNode);
                                }
                                actionsNode.Nodes.Add(actionNode);
                            }
                            itemNode.Nodes.Add(actionsNode);
                        }

                        itemsNode.Nodes.Add(itemNode);
                    }
                    nodes.Add(itemsNode);
                }
            }
            //if (!String.IsNullOrWhiteSpace(connectionParams.SRPath))
            //{
            //    var routesNode = new IsbNode("Типовой маршрут");

            //    var package = new XmlDocument();
            //    package.Load(connectionParams.SRPath);

            //    var groups = package.SelectNodes("/ROOT/RecordRef/Requisite/RecordRef[@RefName = 'STANDARD_ROUTE_GROUPS']");
            //    var existingGroups = new List<string>();
            //    foreach (XmlNode group in groups)
            //    {
            //        var groupName = group.Attributes["Name"].Value;
            //        var groupCode = group.Attributes["Code"].Value;

            //        if (!existingGroups.Exists(n => n == groupCode))
            //        {
            //            existingGroups.Add(groupCode);
            //            var groupNode = new IsbNode(groupName);
            //            routesNode.Nodes.Add(groupNode);

            //            var routes = package.SelectNodes("/ROOT/RecordRef[@RefName = 'STANDARD_ROUTES' and Requisite/RecordRef[@RefName = 'STANDARD_ROUTE_GROUPS']/@Code = '" + groupCode + "']");
            //            foreach (XmlNode route in routes)
            //            {
            //                var name = route.Attributes["Name"].Value;
            //                var IMGFileName = route.SelectSingleNode("Requisite[@Name = 'ISBSearchCondition']").Attributes["Value"].Value;
            //                var fullIMGPath = Path.Combine(Path.GetDirectoryName(connectionParams.SRPath), IMGFileName);
            //                var routeNode = new IsbNode(name);
            //                routeNode.Name = route.Attributes["Name"].Value;
            //                routeNode.Code = route.Attributes["Code"].Value;
            //                routeNode.Type = IsbNodeType.StandardRoute;
            //                var IMGFileData = File.ReadAllText(fullIMGPath);
            //                var schemaString = System.Text.Encoding.GetEncoding(1251).GetString(Convert.FromBase64String(IMGFileData));
            //                RouteParser.ParseRoute(schemaString, routeNode);
            //                groupNode.Nodes.Add(routeNode);
            //            }
            //        }
            //    }

            //    nodes.Add(routesNode);
            //}
            //if (!String.IsNullOrWhiteSpace(connectionParams.WIZPath))
            //{
            //    var wizardsNode = new IsbNode("Мастер действий");

            //    var package = new XmlDocument();
            //    package.Load(connectionParams.WIZPath);
            //    var groups = package.SelectNodes("/ROOT/RecordRef[@RefName = 'WIZARD_GROUPS']");
            //    foreach (XmlNode group in groups)
            //    {
            //        var groupName = group.Attributes["Name"].Value;
            //        var groupCode = group.Attributes["Code"].Value;

            //        var groupNode = new IsbNode(groupName);
            //        wizardsNode.Nodes.Add(groupNode);

            //        var wizards = package.SelectNodes("/ROOT/RecordRef[@RefName = 'WIZARDS' and Requisite/RecordRef[@RefName = 'WIZARD_GROUPS']/@Code = '" + groupCode + "']");
            //        foreach (XmlNode wizard in wizards)
            //        {
            //            var name = wizard.Attributes["Name"].Value;
            //            var text = GetNodeString(wizard.SelectSingleNode("Requisite[@Name = 'ISBSearchCondition']/node()"));
            //            var wizardNode = new IsbNode(name);
            //            wizardNode.Type = IsbNodeType.Wizard;
            //            WizardParser.ParseWizardText(text, wizardNode);
            //            groupNode.Nodes.Add(wizardNode);
            //        }
            //    }

            //    nodes.Add(wizardsNode);
            //}
        }
    }
}
