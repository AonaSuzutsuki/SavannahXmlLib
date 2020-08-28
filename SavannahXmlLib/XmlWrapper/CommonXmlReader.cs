﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CommonCoreLib.CommonLinq;
using CommonExtensionLib.Extensions;

namespace SavannahXmlLib.XmlWrapper
{
    /// <summary>
    /// 
    /// </summary>
    public class CommonXmlReader
    {
        private readonly XmlDocument document = new XmlDocument();

        /// <summary>
        /// Get the xml declaration.
        /// </summary>
        public string Declaration { get; private set; }

        /// <summary>
        /// Initialize CommonXmlReader with the specified file.
        /// </summary>
        /// <param name="xmlPath">File path to be parsed</param>
        /// <param name="ignoreComments">Whether to ignore the comments.</param>
        public CommonXmlReader(string xmlPath, bool ignoreComments = true)
        {
            using var fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Initialize(fs, ignoreComments);
        }

        /// <summary>
        /// Initialize CommonXmlReader with the specified Stream.
        /// </summary>
        /// <param name="stream">Stream to be parsed</param>
        /// <param name="ignoreComments">Whether to ignore the comments.</param>
        public CommonXmlReader(Stream stream, bool ignoreComments = true)
        {
            Initialize(stream, ignoreComments);
        }

        private void Initialize(Stream stream, bool ignoreComments)
        {
            var readerSettings = new XmlReaderSettings
            {
                IgnoreComments = ignoreComments
            };
            using var reader = XmlReader.Create(stream, readerSettings);

            document.Load(reader);
            var declaration = document.ChildNodes
                                .OfType<XmlDeclaration>()
                                .FirstOrDefault();
            Declaration = declaration.InnerText;
        }

        /// <summary>
        /// Get values of an attribute from the specified XPath.
        /// </summary>
        /// <param name="name">Attributes name.</param>
        /// <param name="xpath">XPath indicating the location of the attribute to be retrieved.</param>
        /// <param name="isContaisNoValue">Whether to include an empty value.</param>
        /// <returns></returns>
        public IList<string> GetAttributes(string name, string xpath, bool isContaisNoValue = true)
        {
            var hierarchy = xpath.Count(c => c == '/');
            var nodeList = ConvertXmlNodes(ConvertXmlNode(document.SelectNodes(xpath)), hierarchy);
            var cond = Conditions.If<IList<string>>(() => isContaisNoValue)
                .Then(() => (from node in nodeList
                             let attr = node.GetAttribute(name).Value
                             select attr).ToList())
                .Else(() => (from node in nodeList
                             let attr = node.GetAttribute(name).Value
                             where !string.IsNullOrEmpty(attr)
                             select attr).ToList());
            return cond.Invoke();
        }

        /// <summary>
        /// Get values from the specified XPath.
        /// </summary>
        /// <param name="xpath">XPath indicating the location of the value to be retrieved.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>Values</returns>
        public IList<string> GetValues(string xpath, bool isRemoveSpace = true)
        {
            var hierarchy = xpath.Count(c => c == '/');
            var nodeList = ConvertXmlNodes(ConvertXmlNode(document.SelectNodes(xpath)), hierarchy, isRemoveSpace);
            return (from node in nodeList
                    let text = node.InnerText
                    where !string.IsNullOrEmpty(text) select text).ToList();
        }

        /// <summary>
        /// Get the node.
        /// </summary>
        /// <param name="xpath">XPath indicating the location of the node to be retrieved.</param>
        /// <returns>The node found. null is returned if not found.</returns>
        public CommonXmlNode GetNode(string xpath)
        {
            var node = document.SelectSingleNode(xpath);
            var hierarchy = xpath.Count(c => c == '/');
            return node == null ? null : ConvertXmlNode(node, hierarchy);
        }

        /// <summary>
        /// Convert the XmlNode object to the CommonXmlNode object.
        /// </summary>
        /// <param name="node">XmlNode object.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>The node.</returns>
        public CommonXmlNode ConvertXmlNode(XmlNode node, int hierarchy, bool isRemoveSpace = true)
        {
            return new CommonXmlNode
            {
                NodeType = XmlNodeType.Tag,
                TagName = node.Name,
                InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                Attributes = ConvertAttributeInfoArray(node.Attributes),
                ChildNodes = GetElements(node.ChildNodes, isRemoveSpace, hierarchy)
            };
        }

        /// <summary>
        /// Get the node array.
        /// </summary>
        /// <param name="xpath">XPath indicating the location of the nodes to be retrieved.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>Nodes found. null is returned if not found.</returns>
        public CommonXmlNode[] GetNodes(string xpath, bool isRemoveSpace = true)
        {
            var nodeList = ConvertXmlNode(document.SelectNodes(xpath));
            var hierarchy = xpath.Count(c => c == '/');
            return nodeList == null ? null : ConvertXmlNodes(nodeList, hierarchy, isRemoveSpace);
        }

        /// <summary>
        /// Convert the XmlNode array to the CommonXmlNode array.
        /// </summary>
        /// <param name="nodeList">The target XmlNode array.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>The converted CommonXmlNode object array</returns>
        public CommonXmlNode[] ConvertXmlNodes(XmlNode[] nodeList, int hierarchy, bool isRemoveSpace = true)
        {
            var list = from node in nodeList
                       select new CommonXmlNode
                       {
                           NodeType = XmlNodeType.Tag,
                           TagName = node.Name,
                           InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                           Attributes = ConvertAttributeInfoArray(node.Attributes),
                           ChildNodes = GetElements(node.ChildNodes, isRemoveSpace, hierarchy)
                       };
            return list.ToArray();
        }

        /// <summary>
        /// Get all nodes including the root.
        /// </summary>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>The root node.</returns>
        public CommonXmlNode GetAllNodes(bool isRemoveSpace = true)
        {
            var nodeList = document.SelectSingleNode("/*");
            var root = new CommonXmlNode
            {
                NodeType = XmlNodeType.Tag,
                TagName = nodeList.Name,
                InnerText = ResolveInnerText(nodeList, isRemoveSpace).Text,
                Attributes = ConvertAttributeInfoArray(nodeList.Attributes),
                ChildNodes = GetElements(nodeList.ChildNodes, isRemoveSpace).ToArray()
            };
            return root;
        }

        private CommonXmlNode GetAllNodesForPriority()
        {
            var nodeList = document.SelectSingleNode("/*");
            var root = new CommonXmlNode
            {
                NodeType = XmlNodeType.Tag,
                TagName = nodeList.Name,
                InnerText = ResolveInnerText(nodeList, true).Text,
                Attributes = ConvertAttributeInfoArray(nodeList.Attributes),
                ChildNodes = GetElements(nodeList.ChildNodes, true).ToArray()
            };
            return root;
        }

        /// <summary>
        /// Parse XML from the Stream and returns all nodes but the root.
        /// </summary>
        /// <param name="stream">Target stream.</param>
        /// <param name="ignoreComments">Whether to ignore the comments.</param>
        /// <returns>Enumerable xml nodes.</returns>
        public static IEnumerable<CommonXmlNode> GetChildNodesFromStream(Stream stream, bool ignoreComments)
        {
            var reader = new CommonXmlReader(stream, ignoreComments);
            var _node = reader.GetAllNodesForPriority();
            return _node.ChildNodes;
        }

        private static string RemoveSpace(string text, int space, bool isAddLine = false)
        {
            var sb = new StringBuilder();

            text = text.UnifiedBreakLine().TrimStart('\n');
            var spaceLength = space == 0 ? GetSpaceLength(text) : space;

            var expression = spaceLength > 0 ? $"^( {{0,{spaceLength}}})(?<text>.*)$" : "^ *(?<text>.*)$";
            var reg = new Regex(expression);
            using var sr = new StringReader(text);
            while (sr.Peek() > -1)
            {
                var line = sr.ReadLine() ?? string.Empty;

                var match = reg.Match(line);
                if (match.Success)
                    if (isAddLine)
                        sb.Append($"{match.Groups["text"].Value}\n");
                    else
                        sb.Append(match.Groups["text"].Value);
                else
                    sb.Append(sr.ReadLine());
            }

            return sb.ToString().TrimStart('\n').TrimEnd('\n');
        }

        private static int GetSpaceLength(string text)
        {
            const string expression = "^(?<space>[\\s\\t]*)(?<text>.*)$";
            var reg = new Regex(expression);
            var textArray = text.Split('\n');
            if (textArray.Length > 0)
            {
                var match = reg.Match(textArray[0]);
                if (match.Success)
                    return match.Groups["space"].Value.Length;
            }

            return 0;
        }

        private CommonXmlText ResolveInnerText(XmlNode node, bool isRemoveSpace, int space = 0)
        {
            var xml = node.InnerXml;
            var xmlText = new CommonXmlText
            {
                Xml = xml.Replace("&#xD;&#xA;", "&#xD;").Replace("&#xD;", "&#xA;").Replace("&#xA;", "\n")
            };

            if (xml.Contains("<") || xml.Contains(">"))
                return xmlText;

            xmlText.Text = Conditions.IfElse(isRemoveSpace, () => RemoveSpace(node.InnerText, space, true),
                () => node.InnerText).UnifiedBreakLine();
            return xmlText;
        }

        private XmlNode[] ConvertXmlNode(XmlNodeList nodeList)
        {
            if (nodeList == null)
                return null;

            var list = new List<XmlNode>(nodeList.Count);
            foreach (var node in nodeList)
            {
                if (node is XmlNode)
                    list.Add((XmlNode)node);
            }

            return list.ToArray();
        }

        private AttributeInfo[] ConvertAttributeInfoArray(XmlAttributeCollection collection)
        {
            if (collection == null)
                return null;

            var list = new List<XmlAttribute>(collection.Count);
            foreach (var attr in collection)
            {
                if (attr is XmlAttribute)
                    list.Add((XmlAttribute)attr);
            }

            return (from attr in list
                    select new AttributeInfo
                    {
                        Name = attr.Name,
                        Value = attr.Value
                    }).ToArray();
        }

        private List<CommonXmlNode> GetElements(XmlNodeList nodeList, bool isRemoveSpace, int hierarchy = 1)
        {
            var list = new List<CommonXmlNode>();
            if (nodeList.Count <= 0)
                return list;

            var space = 2 * hierarchy;

            foreach (var n in nodeList)
            {
                if (n is XmlElement)
                {
                    var node = (XmlElement)n;
                    var commonXmlNode = new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
                        TagName = node.Name,
                        InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                        Attributes = ConvertAttributeInfoArray(node.Attributes)
                    };
                    if (node.ChildNodes.Count > 0)
                        commonXmlNode.ChildNodes = GetElements(node.ChildNodes, isRemoveSpace, hierarchy + 1).ToArray();
                    list.Add(commonXmlNode);
                }

                if (n is XmlCharacterData)
                {
                    var node = (XmlCharacterData)n;
                    if (node.NodeType == System.Xml.XmlNodeType.Comment)
                    {
                        var commonXmlNode = new CommonXmlNode
                        {
                            NodeType = XmlNodeType.Comment,
                            TagName = node.Name,
                            InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                        };
                        list.Add(commonXmlNode);
                    }
                    else
                    {
                        var commonXmlNode = new CommonXmlNode
                        {
                            NodeType = XmlNodeType.Text,
                            TagName = node.Name,
                            InnerText = ResolveInnerText(node, isRemoveSpace, space).Text,
                        };
                        list.Add(commonXmlNode);
                    }
                }
            }
            return list;
        }

        private static XmlNode GetFirstTextNode(XmlNodeList nodeList)
        {
            foreach (var node in nodeList)
            {
                if (node is XmlCharacterData n)
                {
                    if (n.NodeType == System.Xml.XmlNodeType.Text)
                        return n;
                }
            }
            return null;
        }
    }
}
