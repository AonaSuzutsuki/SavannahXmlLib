using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CommonCoreLib.CommonLinq;
using CommonExtensionLib.Extensions;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace SavannahXmlLib.XmlWrapper
{
    /// <summary>
    /// 
    /// </summary>
    public class SavannahXmlReader
    {
        #region Fields

        private readonly XmlDocument _document;

        private readonly XmlNamespaceManager _xmlNamespaceManager;

        #endregion

        #region Properties

        /// <summary>
        /// Get the xml declaration.
        /// </summary>
        public string Declaration { get; private set; }

        public int IndentSize { get; set; } = 2;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize SavannahXmlReader with the specified file.
        /// </summary>
        /// <param name="xmlPath">File path to be parsed</param>
        /// <param name="ignoreComments">Whether to ignore the comments.</param>
        public SavannahXmlReader(string xmlPath, bool ignoreComments = true)
        {
            using var stream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var (xmlDocument, declaration) = Initialize(stream, ignoreComments);
            _xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            _document = xmlDocument;
            Declaration = declaration;
        }

        /// <summary>
        /// Initialize SavannahXmlReader with the specified Stream.
        /// </summary>
        /// <param name="stream">Stream to be parsed</param>
        /// <param name="ignoreComments">Whether to ignore the comments.</param>
        public SavannahXmlReader(Stream stream, bool ignoreComments = true)
        {
            var (xmlDocument, declaration) = Initialize(stream, ignoreComments);
            _xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            _document = xmlDocument;
            Declaration = declaration;
        }

        #endregion

        #region Member Methods

        /// <summary>
        /// Add the namespace.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        /// <param name="uri">URI.</param>
        public void AddNamespace(string prefix, string uri)
        {
            _xmlNamespaceManager.AddNamespace(prefix, uri);
        }

        /// <summary>
        /// Remove the namespace.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        /// <param name="uri">URI.</param>
        public void RemoveNamespace(string prefix, string uri)
        {
            _xmlNamespaceManager.RemoveNamespace(prefix, uri);
        }

        /// <summary>
        /// Get values of an attribute from the specified XPath.
        /// </summary>
        /// <param name="name">Attributes name.</param>
        /// <param name="xpath">XPath indicating the location of the attribute to be retrieved.</param>
        /// <param name="isContaisNoValue">Whether to include an empty value.</param>
        /// <returns>Values of an attribute</returns>
        public IEnumerable<string> GetAttributes(string name, string xpath, bool isContaisNoValue = true)
        {
            var nodeList = ConvertXmlNodeList(_document.SelectNodes(xpath, _xmlNamespaceManager));
            var table = CreateTable(nodeList.FirstOrDefault(), IndentSize, true);
            var cond = Conditions.If<IEnumerable<string>>(() => isContaisNoValue)
                .Then(() => (from node in nodeList
                             let val = table.Get(node)
                             where val is SavannahTagNode
                             let attr = ((SavannahTagNode)val)?.GetAttribute(name).Value
                             select attr).ToList())
                .Else(() => (from node in nodeList
                             let val = table.Get(node)
                             where val is SavannahTagNode
                             let attr = ((SavannahTagNode)val)?.GetAttribute(name).Value
                             where !string.IsNullOrEmpty(attr)
                             select attr).ToList());
            return cond.Invoke();
        }

        /// <summary>
        /// Get the value of an attribute from the specified XPath.
        /// </summary>
        /// <param name="name">Attributes name.</param>
        /// <param name="xpath">XPath indicating the location of the attribute to be retrieved.</param>
        /// <param name="isContaisNoValue">Whether to include an empty value.</param>
        /// <returns>The attribute.</returns>
        public string GetAttribute(string name, string xpath, bool isContaisNoValue = true)
        {
            var attrs = GetAttributes(name, xpath, isContaisNoValue);
            return attrs.FirstOrDefault();
        }

        /// <summary>
        /// Get values from the specified XPath.
        /// </summary>
        /// <param name="xpath">XPath indicating the location of the value to be retrieved.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>Values</returns>
        public IEnumerable<string> GetValues(string xpath, bool isRemoveSpace = true)
        {
            var xmlNode = _document.SelectNodes(xpath, _xmlNamespaceManager);
            var xmlNodes = ConvertXmlNodeList(xmlNode);
            var table = CreateTable(xmlNodes.FirstOrDefault(), IndentSize, isRemoveSpace);
            return (from node in xmlNodes
                    let val = table.Get(node)
                    let text = val?.InnerText
                    where !string.IsNullOrEmpty(text)
                    select text.Trim()).ToList();
        }

        /// <summary>
        /// Get the value from the specified XPath.
        /// </summary>
        /// <param name="xpath">XPath indicating the location of the value to be retrieved.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>The value</returns>
        public string GetValue(string xpath, bool isRemoveSpace = true)
        {
            var values = GetValues(xpath, isRemoveSpace);
            return values.Any() ? values.First() : string.Empty;
        }

        /// <summary>
        /// Get the node.
        /// </summary>
        /// <param name="xpath">XPath indicating the location of the node to be retrieved.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>The node found. null is returned if not found.</returns>
        public AbstractSavannahXmlNode GetNode(string xpath, bool isRemoveSpace = true)
        {
            var node = GetNodes(xpath, isRemoveSpace)?.First();
            return node;
        }

        /// <summary>
        /// Get the node array.
        /// </summary>
        /// <param name="xpath">XPath indicating the location of the nodes to be retrieved.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>Nodes found. null is returned if not found.</returns>
        public IEnumerable<AbstractSavannahXmlNode> GetNodes(string xpath, bool isRemoveSpace = true)
        {
            var nodes = _document.SelectNodes(xpath, _xmlNamespaceManager);
            var nodeList = ConvertXmlNodeList(nodes);

            var table = CreateTable(nodeList.First(), IndentSize, isRemoveSpace);

            return (from node in nodeList where table.ContainsKey(node) select table[node]).ToList();
        }

        /// <summary>
        /// Get all nodes including the root.
        /// </summary>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>The root node.</returns>
        public SavannahTagNode GetAllNodes(bool isRemoveSpace = true)
        {
            var node = _document.SelectSingleNode("/*", _xmlNamespaceManager);
            var table = CreateTable(node, IndentSize, isRemoveSpace);
            var root = table.Get(node) as SavannahTagNode;
            return root;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Parse XML from the Stream and returns all nodes but the root.
        /// </summary>
        /// <param name="stream">Target stream.</param>
        /// <param name="ignoreComments">Whether to ignore the comments.</param>
        /// <returns>Enumerable xml nodes.</returns>
        public static IEnumerable<AbstractSavannahXmlNode> GetChildNodesFromStream(Stream stream, bool ignoreComments)
        {
            var reader = new SavannahXmlReader(stream, ignoreComments);
            var _node = reader.GetAllNodes();
            return _node.ChildNodes;
        }

        /// <summary>
        /// Convert the XmlNode object to the SavannahXmlNode object.
        /// </summary>
        /// <param name="node">XmlNode object.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <param name="table">
        ///   This is a table of XmlNode and SavannahXmlNode.
        ///   It associates the generated SavannahXmlNode with the XmlNode.
        /// </param>
        /// <returns>The node.</returns>
        public static AbstractSavannahXmlNode ConvertXmlNode(XmlNode node, int indentSize, bool isRemoveSpace = true, Dictionary<XmlNode, AbstractSavannahXmlNode> table = null)
        {
            var hierarchy = GetHierarchyFromParent(node);
            var commonXmlNode = new SavannahTagNode
            {
                TagName = node.Name,
                InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                Attributes = ConvertAttributeInfoArray(node.Attributes),
                ChildNodes = GetElements(node.ChildNodes, isRemoveSpace, indentSize, hierarchy, table)
            };
            table?.Add(node, commonXmlNode);
            ApplyInnerText(commonXmlNode);
            return commonXmlNode;
        }

        /// <summary>
        /// Convert the XmlNode array to the SavannahXmlNode array.
        /// </summary>
        /// <param name="nodeList">The target XmlNode array.</param>
        /// <param name="isRemoveSpace">Whether to clear indentation blanks.</param>
        /// <returns>The converted SavannahXmlNode object array</returns>
        public static AbstractSavannahXmlNode[] ConvertXmlNodes(IEnumerable<XmlNode> nodeList, int indentSize, bool isRemoveSpace = true)
        {
            var list = from node in nodeList select ConvertXmlNode(node, indentSize, isRemoveSpace);
            return list.ToArray();
        }

        #endregion

        #region Private Static Methods

        private static (XmlDocument xmlDocument, string declaration) Initialize(Stream stream, bool ignoreComments)
        {
            var readerSettings = new XmlReaderSettings
            {
                IgnoreComments = ignoreComments
            };
            using var reader = XmlReader.Create(stream, readerSettings);

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            var declaration = xmlDocument.ChildNodes
                                .OfType<XmlDeclaration>()
                                .FirstOrDefault();
            var declarationText = declaration == null ? SavannahXmlConstants.Utf8Declaration : declaration.InnerText;

            return (xmlDocument, declarationText);
        }

        private static void ApplyInnerText(AbstractSavannahXmlNode node)
        {
            if (node is SavannahTagNode tagNode)
            {
                var sb = new List<string>();
                foreach (var child in tagNode.ChildNodes)
                {
                    if (child is SavannahTextNode textNode)
                        sb.Add(textNode.InnerText);

                    ApplyInnerText(child);
                }

                tagNode.InnerText = string.Join("\n", sb);
            }
        }

        private static Dictionary<XmlNode, AbstractSavannahXmlNode> CreateTable(XmlNode node, int indentSize, bool isRemoveSpace)
        {
            var root = GetRootNode(node);

            var table = new Dictionary<XmlNode, AbstractSavannahXmlNode>();
            _ = ConvertXmlNode(root, indentSize, isRemoveSpace, table);

            return table;
        }

        private static XmlNode GetRootNode(XmlNode node, XmlNode prev = null)
        {
            while (true)
            {
                if (node.ParentNode == null)
                    return prev;
                prev = node;
                node = node.ParentNode;
            }
        }

        private static int GetHierarchyFromParent(XmlNode xmlNode, int hierarchy = 0)
        {
            if (xmlNode.ParentNode == null)
                return hierarchy;

            return GetHierarchyFromParent(xmlNode.ParentNode, hierarchy + 1);
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

        private static SavannahXmlText ResolveInnerText(XmlNode node, bool isRemoveSpace, int space = 0)
        {
            var xml = node.InnerXml;
            var xmlText = new SavannahXmlText
            {
                Xml = xml.Replace("&#xD;&#xA;", "&#xD;").Replace("&#xD;", "&#xA;").Replace("&#xA;", "\n")
            };

            if (xml.Contains("<") || xml.Contains(">"))
                return xmlText;

            xmlText.Text = Conditions.IfElse(isRemoveSpace, () => RemoveSpace(node.InnerText, space, true),
                () => node.InnerText).UnifiedBreakLine();
            return xmlText;
        }

        private static IEnumerable<XmlNode> ConvertXmlNodeList(XmlNodeList nodeList)
        {
            if (nodeList == null)
                return null;

            var list = new List<XmlNode>(nodeList.Count);
            foreach (var node in nodeList)
            {
                if (node is XmlNode xmlNode)
                    list.Add(xmlNode);
            }

            return list;
        }

        private static IEnumerable<AttributeInfo> ConvertAttributeInfoArray(XmlAttributeCollection collection)
        {
            if (collection == null)
                return null;

            var list = new List<AttributeInfo>(collection.Count);
            foreach (var attr in collection)
            {
                if (attr is XmlAttribute attribute)
                    list.Add(new AttributeInfo
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    });
            }

            return list;
        }

        private static List<AbstractSavannahXmlNode> GetElements(XmlNodeList nodeList, bool isRemoveSpace, int indentSize, int hierarchy = 1, Dictionary<XmlNode, AbstractSavannahXmlNode> table = null)
        {
            var list = new List<AbstractSavannahXmlNode>();
            if (nodeList.Count <= 0)
                return list;

            var space = indentSize * hierarchy;

            foreach (var n in nodeList)
            {
                if (n is XmlElement)
                {
                    var node = (XmlElement)n;
                    var commonXmlNode = new SavannahTagNode
                    {
                        TagName = node.Name,
                        InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                        Attributes = ConvertAttributeInfoArray(node.Attributes)
                    };
                    if (node.ChildNodes.Count > 0)
                        commonXmlNode.ChildNodes = GetElements(node.ChildNodes, isRemoveSpace, indentSize, hierarchy + 1, table).ToArray();
                    list.Add(commonXmlNode);
                    table?.Add(node, commonXmlNode);
                }

                if (n is XmlCharacterData)
                {
                    var node = (XmlCharacterData)n;
                    if (node.NodeType == System.Xml.XmlNodeType.Comment)
                    {
                        var commonXmlNode = new SavannahCommentNode
                        {
                            TagName = node.Name,
                            InnerText = ResolveInnerText(node, isRemoveSpace).Text
                        };
                        list.Add(commonXmlNode);
                        table?.Add(node, commonXmlNode);
                    }
                    else if (node.NodeType == System.Xml.XmlNodeType.CDATA)
                    {
                        var commonXmlNode = new SavannahCdataNode
                        {
                            TagName = node.Name,
                            InnerText = ResolveInnerText(node, isRemoveSpace, space).Text
                        };
                        list.Add(commonXmlNode);
                        table?.Add(node, commonXmlNode);
                    }
                    else
                    {
                        var commonXmlNode = new SavannahTextNode
                        {
                            TagName = node.Name,
                            InnerText = ResolveInnerText(node, isRemoveSpace, space).Text
                        };
                        list.Add(commonXmlNode);
                        table?.Add(node, commonXmlNode);
                    }
                }
            }
            return list;
        }

        #endregion
    }
}
