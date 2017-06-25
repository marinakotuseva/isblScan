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
            if (k1 == obj.Length - 1)
            {
                k1 = s.IndexOf(inh) + inh.Length;
            }
            var k2 = s.IndexOf(":");
            k2 = k2 > -1 ? k2 : k1 + 1;
            node.PropertyName = s.Substring(k1 + 1, k2 - k1 - 1).Trim();
            node.PropertyClass = s.Substring(k2 + 1, s.Length - k2 - 1).Trim();

            var index = 0;
            var count = objectData.Count() - 2;
            while (index < count)
            {
                index = index + ReadObjectProperty(objectData.Skip(index + 1), node);
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
                ItemCounter.Reset();
            }
            else
            {
                node.PropertyName = "item" + ItemCounter.NextValue;
            }

            var index = 0;
            var count = objectData.Count() - 1;
            while (index < count)
            {
                index = index + ReadObjectProperty(objectData.Skip(index + 1), node);
            }

            return node;
        }
        private static int ReadObjectProperty(IEnumerable<string> objectData, DfmDataNode parent)
        {
            var endIndex = 0;
            var s = objectData.ElementAt(0);
            DfmDataNode node = null;
            if (s.EndsWith(" item"))
            {
                var ident = s.Substring(0, s.IndexOf("item"));
                endIndex = objectData.ToList().IndexOf(
                                    objectData.First(o => {
                                        return o.EndsWith(ident + "end") || o.EndsWith(ident + "end>");
                                    })
                                );
                
                node = ReadItemsProperties(
                            objectData.Take(endIndex),
                            parent
                        );
            }
            else if (s.IndexOf("inherited") > -1)
            {
                var ident = s.Substring(0, s.IndexOf("inherited"));
                endIndex = objectData.ToList().IndexOf(ident + "end");
                node = ReadObjectProperties(
                            objectData.Take(endIndex + 1)
                        );
            }
            else if (s.IndexOf("object") > -1)
            {
                var ident = s.Substring(0, s.IndexOf("object"));
                endIndex = objectData.ToList().IndexOf(ident + "end");
                node = ReadObjectProperties(
                            objectData.Take(endIndex + 1)
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
                    endIndex = objectData.ToList().IndexOf(
                                    objectData.First(o => {
                                        return o.EndsWith(endTerm) && !o.EndsWith("<>");
                                    })
                                );
                    if (b1 || b2)
                    {
                        node.PropertyValue = new List<string>();
                        if (b1)
                        {
                            ((List<string>)node.PropertyValue).Add("(");
                        }
                        else
                        {
                            ((List<string>)node.PropertyValue).Add("{");
                        }
                        ((List<string>)node.PropertyValue).AddRange(
                            objectData.Skip(1).Take(endIndex)
                        );
                    }
                    else
                    {
                        node = ReadItemsProperties(
                            objectData.Take(endIndex + 1),
                            parent
                        );
                    }
                }
                else
                {
                    if (s.Trim().EndsWith("="))
                    {
                        endIndex = objectData.Skip(1).ToList().IndexOf(
                                   objectData.Skip(1).First(o => {
                                       return !o.EndsWith("+");
                                   })
                               ) + 1;
                        node.PropertyValue = objectData.Skip(1).Take(endIndex).Select(l => ClearDfmString(l)).Aggregate((text, line) => text + line);
                    }
                    else
                    {
                        node.PropertyValue = s.Substring(k + 1, s.Length - k - 1).Trim();
                        if ((s.Contains("'")) || (s.Contains("#")))
                        {
                            node.PropertyValue = ClearDfmString((string)node.PropertyValue);
                        }
                    }
                }
            }
            parent.Nodes.Add(node);
            return endIndex + 1;
        }
    }
}
