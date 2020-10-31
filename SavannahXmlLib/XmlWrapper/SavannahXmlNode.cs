using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CommonCoreLib.Bool;
using CommonExtensionLib.Extensions;
using SavannahXmlLib.Extensions;
using SavannahXmlLib.XmlWrapper;

namespace SavannahXmlLib.XmlWrapper
{
    /// <summary>
    /// Represents an Xml node as a tree structure.
    /// </summary>
    public class SavannahXmlNode
    {
        #region Constants

        public const string TextTagName = "#text";
        public const string CommentTagName = "#comment";
        public const string CdataTagName = "#cdata-section";
        public const int DefaultIndentSize = 2;

        #endregion

        #region Properties

        /// <summary>
        /// The type of this node.
        /// </summary>
        public XmlNodeType NodeType { get; set; }

        /// <summary>
        /// The name of this node.
        /// </summary>
        public string TagName { get; set; }

        public SavannahXmlNode Parent { get; internal set; }

        /// <summary>
        /// Enumerable attributes of this node.
        /// </summary>
        public IEnumerable<AttributeInfo> Attributes
        {
            get => _attributes;
            set => _attributes = new HashSet<AttributeInfo>(value);
        }

        /// <summary>
        /// Enumerable children of this node.
        /// </summary>
        public IEnumerable<SavannahXmlNode> ChildNodes
        {
            get => _childNodes;
            set => _childNodes = ResolveChildrenParent(new LinkedList<SavannahXmlNode>(value), this);
        }

        /// <summary>
        /// InnerText without tag of this node.
        /// </summary>
        public string InnerText { get; set; } = string.Empty;

        /// <summary>
        /// InnerXml of this node.
        /// </summary>
        public string InnerXml => ToString(ChildNodes, DefaultIndentSize);

        public string OutterXml => GenerateOutterXml(this, DefaultIndentSize);

        /// <summary>
        /// The High priority InnerXml.
        /// Used to force Xml to be rewritten.
        /// </summary>
        public string PrioritizeInnerXml { get; set; }
        #endregion

        #region Fields
        private HashSet<AttributeInfo> _attributes = new HashSet<AttributeInfo>();
        private LinkedList<SavannahXmlNode> _childNodes = new LinkedList<SavannahXmlNode>();
        #endregion

        #region Constructor
        #endregion

        #region Member Methods
        /// <summary>
        /// Append the attribute to this node.
        /// </summary>
        /// <param name="name">Name to be added</param>
        /// <param name="value">Value to be added</param>
        public void AppendAttribute(string name, string value)
        {
            var attr = new AttributeInfo { Name = name, Value = value};
            AppendAttribute(attr);
        }

        /// <summary>
        /// Append the attribute to this node.
        /// </summary>
        /// <param name="info">AttributeInfo to be added</param>
        public void AppendAttribute(AttributeInfo info)
        {
            if (!_attributes.Contains(info))
                _attributes.Add(info);
        }

        /// <summary>
        /// Get the attribute from this node.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve</param>
        /// <returns>The value of the attribute</returns>
        public AttributeInfo GetAttribute(string name)
        {
            foreach (var attributeInfo in _attributes)
            {
                if (attributeInfo.Name == name)
                    return attributeInfo;
            }
            return new AttributeInfo();
        }

        /// <summary>
        /// Remove the attribute from this node.
        /// </summary>
        /// <param name="name">The name of the attribute to be removed</param>
        public void RemoveAttribute(string name)
        {
            var attr = GetAttribute(name);
            if (_attributes.Contains(attr))
                _attributes.Remove(attr);
        }

        /// <summary>
        /// Remove the attribute from this node.
        /// </summary>
        /// <param name="info">The AttributeInfo to be removed</param>
        public void RemoveAttribute(AttributeInfo info)
        {
            if (_attributes.Contains(info))
                _attributes.Remove(info);
        }

        /// <summary>
        /// Create and add the child element to this node.
        /// </summary>
        /// <param name="tagName">The name of the tag to add</param>
        /// <param name="attributeInfos">Enumerable attribute info to add</param>
        /// <param name="commonXmlNodes">Enumerable xml node to add</param>
        /// <returns>The node of the created tag.</returns>
        public SavannahXmlNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<SavannahXmlNode> commonXmlNodes = null)
        {
            var node = CreateElement(tagName, attributeInfos, commonXmlNodes);
            AddChildElement(node);
            return node;
        }

        /// <summary>
        /// Create and add the child element to this node.
        /// </summary>
        /// <param name="tagName">The name of the tag to add</param>
        /// <param name="attributeInfos">Enumerable attribute info to add</param>
        /// <param name="innerXml">The inner xml to add</param>
        /// <returns>The node of the created tag.</returns>
        public SavannahXmlNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos
            , string innerXml)
        {
            var node = CreateElement(tagName, attributeInfos, innerXml);
            AddChildElement(node);
            return node;
        }

        /// <summary>
        /// Add the child element to this node.
        /// </summary>
        /// <param name="node">The node to add</param>
        public void AddChildElement(SavannahXmlNode node)
        {
            _childNodes.AddLast(node);
            node.Parent = this;
        }

        /// <summary>
        /// Remove the element from children.
        /// </summary>
        /// <param name="node">The node to remove</param>
        public void RemoveChildElement(SavannahXmlNode node)
        {
            var listNode = _childNodes.Find(node, new SavannahXmlNodeComparer());
            if (listNode == null)
                return;
            _childNodes.Remove(listNode);
        }

        /// <summary>
        /// Add the child element to before 1st argument node.
        /// </summary>
        /// <param name="node">Nodes to search.</param>
        /// <param name="newNode">Nodes to be added</param>
        public void AddBeforeChildElement(SavannahXmlNode node, SavannahXmlNode newNode)
        {
            var listNode = _childNodes.Find(node, new SavannahXmlNodeComparer());
            if (listNode == null)
                return;
            _childNodes.AddBefore(listNode, newNode);
            newNode.Parent = this;
        }

        /// <summary>
        /// Add the child element to after 1st argument node.
        /// </summary>
        /// <param name="node">Nodes to search.</param>
        /// <param name="newNode">Nodes to be added</param>
        public void AddAfterChildElement(SavannahXmlNode node, SavannahXmlNode newNode)
        {
            var listNode = _childNodes.Find(node, new SavannahXmlNodeComparer());
            if (listNode == null)
                return;
            _childNodes.AddAfter(listNode, newNode);
            newNode.Parent = this;
        }

        /// <summary>
        /// Create a SavannahXmlReader object from the current node.
        /// </summary>
        /// <returns>The SavannahXmlReader object. Returns null for text and comment nodes.</returns>
        public SavannahXmlReader GetReader()
        {
            var type = NodeType;
            if (type == XmlNodeType.Text || type == XmlNodeType.Comment)
                return null;

            var innerXml = ToString();
            var outterXml = $"{SavannahXmlConstants.Declaration}\n{innerXml}";

            var data = Encoding.UTF8.GetBytes(outterXml);
            using var ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Position = 0;

            var reader = new SavannahXmlReader(ms);
            return reader;
        }

        /// <summary>
        /// Return xml string with tags.
        /// </summary>
        /// <returns>String in XML format.</returns>
        public override string ToString()
        {
            return ToString(DefaultIndentSize);
        }

        public string ToString(int indentSize)
        {
            return ToString(this, indentSize);
        }

        /// <summary>
        /// Returns a string in XML format from a collection of nodes.
        /// </summary>
        /// <param name="commonXmlNodes">Enumerable xml nodes.</param>
        /// <returns>String in XML format.</returns>
        public string ToString(IEnumerable<SavannahXmlNode> commonXmlNodes, int indentSize)
        {
            var sb = new StringBuilder();
            foreach (var node in commonXmlNodes)
            {
                sb.Append($"{ToString(node, indentSize)}\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate a string in XML format with child elements from a node.
        /// </summary>
        /// <param name="node">Target node.</param>
        /// <param name="space">Indent space size</param>
        /// <returns>String in XML format.</returns>
        public static string ToString(SavannahXmlNode node, int indentSize, int space = 0)
        {
            var spaceText = MakeSpace(space);

            var sb = new StringBuilder();
            if (node.NodeType == XmlNodeType.Tag)
            {
                //var attr = string.Join(" ", from x in node.Attributes select x.ToString());
                var attr = node.Attributes.ToAttributesText(" ");
                attr = string.IsNullOrEmpty(attr) ? attr : $" {attr}";

                if (node.ChildNodes.Any())
                {
                    sb.Append($"{spaceText}<{node.TagName}{attr}>\n");
                    space += indentSize;
                    foreach (var childNode in node.ChildNodes)
                    {
                        sb.Append($"{ToString(childNode, indentSize, space)}\n");
                    }

                    sb.Append($"{spaceText}</{node.TagName}>");
                }
                else
                {
                    sb.Append($"{spaceText}<{node.TagName}{attr} />");
                }
            }
            else if (node.NodeType == XmlNodeType.Text)
            {
                sb.Append(ResolveInnerText(node, spaceText));
            }
            else if (node.NodeType == XmlNodeType.CDATA)
            {
                sb.Append($"{spaceText}<![CDATA[\n");
                sb.Append(ResolveInnerText(node, MakeSpace(space)));
                sb.Append($"\n{spaceText}]]>");
            }
            else
            {
                sb.Append($"{spaceText}<!--\n");
                sb.Append(ResolveInnerText(node, MakeSpace(space + indentSize)));
                sb.Append($"\n{spaceText}-->");
            }
            

            return sb.ToString();
        }

        private static string ResolveInnerText(SavannahXmlNode node, string spaceText)
        {
            var text = node.InnerText.UnifiedBreakLine();
            var lines = text.Split('\n');
            var converted = string.Join("\n", lines.Select(x => $"{spaceText}{x}"));
            return converted;
        }

        /// <summary>
        /// Resolve all PrioritizeInnerXml elements including child elements.
        /// </summary>
        /// <param name="ignoreComments">Whether to ignore the comments.</param>
        /// <param name="node">Target node. The current node is specified if it is null.</param>
        public void ResolvePrioritizeInnerXml(bool ignoreComments = true, SavannahXmlNode node = null)
        {
            node ??= this;

            if (!string.IsNullOrEmpty(node.PrioritizeInnerXml))
            {
                if (node.NodeType == XmlNodeType.Tag)
                {
                    using var ms = SavannahXmlWriter.ConvertInnerXmlToXmlText(node);
                    var cNode = SavannahXmlReader.GetChildNodesFromStream(ms, ignoreComments);
                    node.ChildNodes = cNode;
                    node.PrioritizeInnerXml = null;
                }
                else
                {
                    node.InnerText = node.PrioritizeInnerXml;
                    node.PrioritizeInnerXml = null;
                }
            }
            else
            {
                if (!node.ChildNodes.Any())
                    return;
                foreach (var nodeChildNode in node.ChildNodes)
                {
                    ResolvePrioritizeInnerXml(ignoreComments, nodeChildNode);
                }
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Generate space text.
        /// </summary>
        /// <param name="count">The number of spaces to generate</param>
        /// <returns>Space text.</returns>
        public static string MakeSpace(int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(" ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate the root node.
        /// </summary>
        /// <param name="tagName">Tag name of root.</param>
        /// <returns>Root node.</returns>
        public static SavannahXmlNode CreateRoot(string tagName)
        {
            var root = new SavannahXmlNode
            {
                TagName = tagName
            };
            return root;
        }

        /// <summary>
        /// Generate the xml node.
        /// </summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="attributeInfos">Enumerable attribute infos</param>
        /// <param name="commonXmlNodes">Enumerable children nodes.</param>
        /// <returns>Xml node.</returns>
        public static SavannahXmlNode CreateElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<SavannahXmlNode> commonXmlNodes = null)
        {
            if (attributeInfos == null)
                attributeInfos = new AttributeInfo[0];

            if (commonXmlNodes == null)
                commonXmlNodes = new SavannahXmlNode[0];

            var node = new SavannahXmlNode
            {
                TagName = tagName,
                Attributes = attributeInfos,
                ChildNodes = commonXmlNodes
            };
            return node;
        }

        /// <summary>
        /// Generate the xml node.
        /// </summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="attributeInfos">Enumerable attribute infos</param>
        /// <param name="innerXml">Internal XML represented in text format.</param>
        /// <returns>Xml node.</returns>
        public static SavannahXmlNode CreateElement(string tagName, IEnumerable<AttributeInfo> attributeInfos,
            string innerXml)
        {
            attributeInfos ??= new AttributeInfo[0];

            var node = new SavannahXmlNode
            {
                TagName = tagName,
                Attributes = attributeInfos,
                PrioritizeInnerXml = innerXml
            };
            return node;
        }

        /// <summary>
        /// Generate the text node.
        /// </summary>
        /// <param name="innerText">The inner text.</param>
        /// <returns>The text node.</returns>
        public static SavannahXmlNode CreateTextNode(string innerText)
        {
            var node = new SavannahXmlNode
            {
                NodeType = XmlNodeType.Text,
                TagName = TextTagName,
                InnerText = innerText
            };
            return node;
        }

        private static LinkedList<SavannahXmlNode> ResolveChildrenParent(LinkedList<SavannahXmlNode> childNodes, SavannahXmlNode parent)
        {
            foreach (var child in childNodes)
            {
                child.Parent = parent;
            }

            return childNodes;
        }

        private static string GenerateOutterXml(SavannahXmlNode node, int indentSize)
        {
            if (node.Parent == null)
                return string.Empty;

            var indent = MakeSpace(indentSize);
            var str = node.ToString();
            var innerXml = string.Join("", node.ToString().Split('\n').Select(item => $"{indent}{item}\n"));
            var attr = node.Parent.Attributes.ToAttributesText(" ");
            return $"<{node.Parent.TagName}{attr}>\n{innerXml}</{node.Parent.TagName}>";
        }
        #endregion

        #region Equals
        /// <summary>
        /// Evaluate the equivalence of objects.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <returns>Object equivalence.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            if (this == obj)
                return true;

            var element = (SavannahXmlNode)obj;
            var collector = new BoolCollector();

            collector.ChangeBool(TagName, TagName == element.TagName);
            collector.ChangeBool(Attributes, Attributes.SequenceEqual(element.Attributes));
            collector.ChangeBool(ChildNodes, ChildNodes.SequenceEqual(element.ChildNodes));
            collector.ChangeBool(InnerText, InnerText.Equals(element.InnerText));

            return collector.Value;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hash value.</returns>
        public override int GetHashCode()
        {
            var hashCode = 2061855513;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TagName);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<AttributeInfo>>.Default.GetHashCode(Attributes);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<SavannahXmlNode>>.Default.GetHashCode(ChildNodes);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InnerText);
            return hashCode;
        }
        #endregion
    }
}
