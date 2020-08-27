using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonCoreLib.Bool;
using CommonExtensionLib.Extensions;

namespace SavannahXmlLib.XmlWrapper
{
    /// <summary>
    /// Extended methods for CommonXmlNode class.
    /// </summary>
    public static class CommonXmlNodeExtension
    {
        /// <summary>
        /// Convert enumerable AttributeInfo class to semi colon separated string.
        /// </summary>
        /// <param name="attributeInfos">Enumerable AttributeInfo class</param>
        /// <returns></returns>
        public static string ToAttributesText(this IEnumerable<AttributeInfo> attributeInfos)
        {
            return string.Join(", ", attributeInfos);
        }
    }

    /// <summary>
    /// XmlNode Type
    /// </summary>
    public enum XmlNodeType
    {
        /// <summary>
        /// Tag
        /// </summary>
        Tag,

        /// <summary>
        /// Text
        /// </summary>
        Text
    }

    /// <summary>
    /// Represents an Xml node as a tree structure.
    /// </summary>
    public class CommonXmlNode
    {
        #region Properties

        public int IndentSize { get; set; } = 2;

        /// <summary>
        /// The type of this node.
        /// </summary>
        public XmlNodeType NodeType { get; set; }

        /// <summary>
        /// The name of this node.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Enumerable attributes of this node.
        /// </summary>
        public IEnumerable<AttributeInfo> Attributes
        {
            get => attributes;
            set => attributes = new HashSet<AttributeInfo>(value);
        }

        /// <summary>
        /// Enumerable children of this node.
        /// </summary>
        public IEnumerable<CommonXmlNode> ChildNodes
        {
            get => childNodes;
            set => childNodes = new List<CommonXmlNode>(value);
        }

        /// <summary>
        /// InnerText without tag of this node.
        /// </summary>
        public string InnerText { get; set; } = string.Empty;

        /// <summary>
        /// InnerXml of this node.
        /// </summary>
        public string InnerXml => ToString(ChildNodes);

        /// <summary>
        /// The High priority InnerXml.
        /// Used to force Xml to be rewritten.
        /// </summary>
        public string PrioritizeInnerXml { get; set; }
        #endregion

        #region Fields
        private HashSet<AttributeInfo> attributes = new HashSet<AttributeInfo>();
        private List<CommonXmlNode> childNodes = new List<CommonXmlNode>();
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
            if (!attributes.Contains(info))
                attributes.Add(info);
        }

        /// <summary>
        /// Get the attribute from this node.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve</param>
        /// <returns>The value of the attribute</returns>
        public AttributeInfo GetAttribute(string name)
        {
            foreach (var attributeInfo in attributes)
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
            if (attributes.Contains(attr))
                attributes.Remove(attr);
        }

        /// <summary>
        /// Remove the attribute from this node.
        /// </summary>
        /// <param name="info">The AttributeInfo to be removed</param>
        public void RemoveAttribute(AttributeInfo info)
        {
            if (attributes.Contains(info))
                attributes.Remove(info);
        }

        /// <summary>
        /// Create and add the child element to this node.
        /// </summary>
        /// <param name="tagName">The name of the tag to add</param>
        /// <param name="attributeInfos">Enumerable attribute info to add</param>
        /// <param name="commonXmlNodes">Enumerable xml node to add</param>
        /// <returns></returns>
        public CommonXmlNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<CommonXmlNode> commonXmlNodes = null)
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
        /// <returns></returns>
        public CommonXmlNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos
            , string innerXml)
        {
            var node = CreateElement(tagName, attributeInfos, innerXml);
            AddChildElement(node);
            return node;
        }

        /// <summary>
        /// Create and add the child element to this node.
        /// </summary>
        /// <param name="node">The node to add</param>
        /// <returns></returns>
        public CommonXmlNode CreateChildElement(CommonXmlNode node)
        public void AddChildElement(CommonXmlNode node)
        {
            childNodes.Add(node);
        }

        /// <summary>
        /// Return xml string with tags.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commonXmlNodes"></param>
        /// <returns></returns>
        public string ToString(IEnumerable<CommonXmlNode> commonXmlNodes)
        {
            var sb = new StringBuilder();
            foreach (var node in commonXmlNodes)
            {
                sb.Append($"{ToString(node)}\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public string ToString(CommonXmlNode node, int space = 0)
        {
            var spaceText = MakeSpace(space);

            var sb = new StringBuilder();
            if (node.NodeType == XmlNodeType.Tag)
            {
                var attr = string.Join(" ", from x in node.Attributes select x.ToString());
                attr = string.IsNullOrEmpty(attr) ? attr : $" {attr}";

                if (node.ChildNodes.Any())
                {
                    sb.Append($"{spaceText}<{node.TagName}{attr}>\n");
                    space += IndentSize;
                    foreach (var childNode in node.ChildNodes)
                    {
                        sb.Append($"{ToString(childNode, space)}\n");
                    }

                    sb.Append($"{spaceText}</{node.TagName}>");
                }
                else
                {
                    sb.Append($"{spaceText}<{node.TagName}{attr} />");
                }
            }
            else
            {
                sb.Append(ResolveInnerText(node, spaceText));
            }
            

            return sb.ToString();
        }

        private string ResolveInnerText(CommonXmlNode node, string spaceText)
        {
            var text = node.InnerText.UnifiedBreakLine();
            var lines = text.Split('\n');
            var converted = string.Join("\n", lines.Select(x => $"{spaceText}{x}"));
            return converted;
        }

        public void ResolvePrioritizeInnerXml(CommonXmlNode node = null)
        {
            node ??= this;

            if (!string.IsNullOrEmpty(node.PrioritizeInnerXml))
            {
                using var ms = CommonXmlWriter.ConvertInnerXmlToXmlText(node);
                var cNode = CommonXmlReader.GetChildNodesFromStream(ms);
                node.ChildNodes = cNode;
                node.PrioritizeInnerXml = null;
            }
            else
            {
                if (!node.ChildNodes.Any())
                    return;
                foreach (var nodeChildNode in node.ChildNodes)
                {
                    ResolvePrioritizeInnerXml(nodeChildNode);
                }
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static CommonXmlNode CreateRoot(string tagName)
        {
            var root = new CommonXmlNode
            {
                TagName = tagName
            };
            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="attributeInfos"></param>
        /// <param name="commonXmlNodes"></param>
        /// <returns></returns>
        public static CommonXmlNode CreateElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<CommonXmlNode> commonXmlNodes = null)
        {
            if (attributeInfos == null)
                attributeInfos = new AttributeInfo[0];

            if (commonXmlNodes == null)
                commonXmlNodes = new CommonXmlNode[0];

            var node = new CommonXmlNode
            {
                TagName = tagName,
                Attributes = attributeInfos,
                ChildNodes = commonXmlNodes
            };
            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="attributeInfos"></param>
        /// <param name="innerXml"></param>
        /// <returns></returns>
        public static CommonXmlNode CreateElement(string tagName, IEnumerable<AttributeInfo> attributeInfos,
            string innerXml)
        {
            if (attributeInfos == null)
                attributeInfos = new AttributeInfo[0];

            var node = new CommonXmlNode
            {
                TagName = tagName,
                Attributes = attributeInfos,
                PrioritizeInnerXml = innerXml
            };
            return node;
        }
        #endregion

        #region Equals
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var element = (CommonXmlNode)obj;
            var collector = new BoolCollector();

            collector.ChangeBool(TagName, TagName == element.TagName);
            collector.ChangeBool(Attributes, Attributes.SequenceEqual(element.Attributes));
            collector.ChangeBool(ChildNodes, ChildNodes.SequenceEqual(element.ChildNodes));
            collector.ChangeBool(InnerText, InnerText.Equals(element.InnerText));

            return collector.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 2061855513;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TagName);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<AttributeInfo>>.Default.GetHashCode(Attributes);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<CommonXmlNode>>.Default.GetHashCode(ChildNodes);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InnerText);
            return hashCode;
        }
        #endregion
    }
}
