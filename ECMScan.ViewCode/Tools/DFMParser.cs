using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ISBLScan.ViewCode
{
    public class DfmDataNode
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string PropertyClass { get; set; }
        public object PropertyValue { get; set; }
        public List<DfmDataNode> Nodes { get; set; } = new List<DfmDataNode>();
    }

    static public class DfmParser
    {
        static public string DecodeMatchedSharpChar(Match source)
        {
            return (Char.ConvertFromUtf32(int.Parse(source.ToString().Substring(1)))).ToString();
        }
        static public string ClearDfmString(string source)
        {
            var tmpString = "@@" + (new Guid()).ToString() + "@@";
            var cleared = source.Trim().TrimEnd('+').Trim().Replace("'", tmpString);
            cleared = Regex.Replace(cleared, "(#[0-9]+)", new MatchEvaluator(DecodeMatchedSharpChar), RegexOptions.None);
            cleared = cleared.Replace(tmpString, "");
            return cleared;
        }
        static public DfmDataNode Parse(string dfm)
        {
            var clearedDfm = Regex.Replace(dfm, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            var dfmLines = clearedDfm.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).Where(l => !String.IsNullOrWhiteSpace(l));
            return ReadObjectProperties(dfmLines);
        }
        private static DfmDataNode ReadObjectProperties(IEnumerable<string> objectData)
        {
            var obj = "object";
            var inh = "inherited";
            var node = new DfmDataNode();
            var s = objectData.ElementAt(0);
            var k1 = s.IndexOf(obj) + obj.Length;
            node.PropertyType = obj;
            if (k1 == obj.Length - 1)
            {
                k1 = s.IndexOf(inh) + inh.Length;
                node.PropertyType = inh;
            }
            var k2 = s.IndexOf(":");
            k2 = k2 > -1 ? k2 : k1 + 1;
            node.PropertyName = s.Substring(k1 + 1, k2 - k1 - 1).Trim();
            node.PropertyClass = s.Substring(k2 + 1, s.Length - k2 - 1).Trim();
            //node.PropertyValue = node;

            var index = 0;
            var count = objectData.Count() - 2;
            while (index < count)
            {
                index = ReadObjectProperty(index, objectData, node);
            }

            return node;
        }

        private static class ItemCounter
        {
            private static int current = 0;
            internal static int NextValue
            {
                get { return ++current; }
            }
            internal static void Reset()
            {
                current = 0;
            }
        }

        private static DfmDataNode ReadItemsProperties(IEnumerable<string> objectData, DfmDataNode parent)
        {
            var node = new DfmDataNode();
            var s = objectData.ElementAt(0);
            var k = s.IndexOf("=");
            if (k > -1)
            {
                node.PropertyName = s.Substring(0, k - 1).Trim();
                node.PropertyType = "items";
                ItemCounter.Reset();
            }
            else
            {
                node.PropertyName = "item" + ItemCounter.NextValue;
                node.PropertyType = "item";
            }
            //node.PropertyValue = node;

            var index = 0;
            var count = objectData.Count() - 1;
            while (index < count)
            {
                index = ReadObjectProperty(index, objectData, node);
            }

            return node;
        }
        private static int ReadObjectProperty(int startIndex, IEnumerable<string> objectData, DfmDataNode parent)
        {
            objectData = objectData.Skip(1).Take(objectData.Count());
            var s = objectData.ElementAt(startIndex);
            var endIndex = startIndex;
            DfmDataNode node = null;
            if (s.EndsWith(" item"))
            {
                var ident = s.Substring(0, s.IndexOf("item"));
                endIndex = objectData.Skip(startIndex).ToList().IndexOf(
                                    objectData.Skip(startIndex).First(o => {
                                        return o.EndsWith(ident + "end") || o.EndsWith(ident + "end>");
                                    })
                                );
                endIndex = endIndex + startIndex;
                
                node = ReadItemsProperties(
                            objectData.Skip(startIndex)
                                        .Take(endIndex - startIndex),
                            parent
                        );
            }
            else if (s.IndexOf("inherited") > -1)
            {
                var ident = s.Substring(0, s.IndexOf("inherited"));
                endIndex = objectData.Skip(startIndex).ToList().IndexOf(ident + "end") + startIndex;
                node = ReadObjectProperties(
                            objectData.Skip(startIndex)
                                        .Take(endIndex - startIndex + 1)
                        );
            }
            else if (s.IndexOf("object") > -1)
            {
                var ident = s.Substring(0, s.IndexOf("object"));
                endIndex = objectData.Skip(startIndex).ToList().IndexOf(ident + "end") + startIndex;
                node = ReadObjectProperties(
                            objectData.Skip(startIndex)
                                        .Take(endIndex - startIndex + 1)
                        );
            }
            else
            {
                var k = s.IndexOf("=");
                node = new DfmDataNode();
                node.PropertyName = s.Substring(0, k - 1).Trim();
                var b0 = (!s.StartsWith("'")) && (!s.StartsWith("#"));
                var b1 = s.EndsWith("= (");
                var b2 = s.EndsWith("= {");
                var b3 = s.EndsWith("= <");

                if ((b0) && (b1 || b2 || b3))
                {
                    var endTerm1 = ")";
                    var endTerm2 = "}";
                    var endTerm3 = ">";
                    string endTerm = endTerm1;
                    if (b2) endTerm = endTerm2;
                    if (b3) endTerm = endTerm3;
                    endIndex = objectData.Skip(startIndex).ToList().IndexOf(
                                    objectData.Skip(startIndex).First(o => {
                                        return o.EndsWith(endTerm) && !o.EndsWith("<>");
                                    })
                                );
                    if (b1 || b2)
                    {
                        node.PropertyValue = new List<string>();
                        if (b1)
                        {
                            node.PropertyType = "record";
                            ((List<string>)node.PropertyValue).Add("(");
                        }
                        else
                        {
                            node.PropertyType = "binary";
                            ((List<string>)node.PropertyValue).Add("{");
                        }
                        ((List<string>)node.PropertyValue).AddRange(
                            objectData.Skip(startIndex + 1).Take(endIndex - startIndex)
                        );
                    }
                    else
                    {
                        node = ReadItemsProperties(
                            objectData.Skip(startIndex).Take(endIndex + 1),
                            parent
                        );
                    }
                    endIndex = endIndex + startIndex;
                }
                else
                {
                    if (s.Trim().EndsWith("="))
                    {
                        node.PropertyType = "string";
                        endIndex = objectData.Skip(startIndex + 1).ToList().IndexOf(
                                   objectData.Skip(startIndex + 1).First(o => {
                                       return !o.EndsWith("+");
                                   })
                               ) + startIndex + 1;
                        node.PropertyValue = objectData.Skip(startIndex + 1).Take(endIndex - startIndex).Select(l => ClearDfmString(l)).Aggregate((text, line) => text + line);
                    }
                    else
                    {
                        node.PropertyValue = s.Substring(k + 1, s.Length - k - 1).Trim();
                        if ((s.Contains("'")) || (s.Contains("#")))
                        {
                            node.PropertyType = "string";
                            node.PropertyValue = ClearDfmString((string)node.PropertyValue);
                        }
                        else
                        {
                            
                            var numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                            var v = node.PropertyValue.ToString();
                            var c = v.Substring(v.Length - 1, 1);
                            if (numbers.Contains(c))
                            {
                                node.PropertyType = "number";
                            }
                            else if ((node.PropertyType == "True") || (node.PropertyType == "False"))
                            {
                                node.PropertyType = "boolean";
                            }
                            else
                            {
                                node.PropertyType = "unknown";
                            }
                        }
                    }
                }
            }
            parent.Nodes.Add(node);
            return endIndex + 1;
        }
    }
}
