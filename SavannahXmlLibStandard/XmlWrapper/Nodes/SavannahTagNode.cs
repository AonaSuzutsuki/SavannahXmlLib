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

namespace SavannahXmlLib.XmlWrapper.Nodes
{

    public class SavannahTagNode : AbstractSavannahXmlNode
    {
        /// <summary>
        /// Enumerable attributes of this node.
        /// </summary>
        public IEnumerable<AttributeInfo> Attributes
        {
            get => _attributes.Values;
            set => _attributes = value.ToDictionary(info => info.Name);
        }

        /// <summary>
        /// Enumerable children of this node.
        /// </summary>
        public IEnumerable<AbstractSavannahXmlNode> ChildNodes
        {
            get => _childNodes;
            set => _childNodes = ResolveChildrenParent(new LinkedList<AbstractSavannahXmlNode>(value), this);
        }

        /// <summary>
        /// InnerXml of this node.
        /// </summary>
        public override string InnerXml => ToString(ChildNodes, DefaultIndentSize);

        /// <summary>
        /// The High priority InnerXml.
        /// Used to force Xml to be rewritten.
        /// </summary>
        public string PrioritizeInnerXml { get; set; }

        #region Fields
        private Dictionary<string, AttributeInfo> _attributes = new Dictionary<string, AttributeInfo>();
        private LinkedList<AbstractSavannahXmlNode> _childNodes = new LinkedList<AbstractSavannahXmlNode>();
        #endregion

        public SavannahTagNode()
        {

        }

        #region Methods
        /// <summary>
        /// Append the attribute to this node.
        /// </summary>
        /// <param name="name">Name to be added</param>
        /// <param name="value">Value to be added</param>
        public void AppendAttribute(string name, string value)
        {
            var attr = new AttributeInfo { Name = name, Value = value };
            AppendAttribute(attr);
        }

        /// <summary>
        /// Append the attribute to this node.
        /// </summary>
        /// <param name="info">AttributeInfo to be added</param>
        public void AppendAttribute(AttributeInfo info)
        {
            if (!_attributes.ContainsKey(info.Name))
                _attributes.Add(info.Name, info);
        }

        public void ChangeAttribute(string name, string value)
        {
            var attr = new AttributeInfo { Name = name, Value = value };
            if (!_attributes.ContainsKey(name))
                return;
            _attributes[name] = attr;
        }

        /// <summary>
        /// Get the attribute from this node.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve</param>
        /// <returns>The value of the attribute</returns>
        public AttributeInfo GetAttribute(string name)
        {
            if (_attributes.ContainsKey(name))
                return _attributes[name];
            return null;
        }

        /// <summary>
        /// Remove the attribute from this node.
        /// </summary>
        /// <param name="name">The name of the attribute to be removed</param>
        public void RemoveAttribute(string name)
        {
            if (_attributes.ContainsKey(name))
                _attributes.Remove(name);
        }

        /// <summary>
        /// Create and add the child element to this node.
        /// </summary>
        /// <param name="tagName">The name of the tag to add</param>
        /// <param name="attributeInfos">Enumerable attribute info to add</param>
        /// <param name="commonXmlNodes">Enumerable xml node to add</param>
        /// <returns>The node of the created tag.</returns>
        public SavannahTagNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<SavannahTagNode> commonXmlNodes = null)
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
        public SavannahTagNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos
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
        public void AddChildElement(AbstractSavannahXmlNode node)
        {
            _childNodes.AddLast(node);
            node.Parent = this;
        }

        /// <summary>
        /// Remove the element from children.
        /// </summary>
        /// <param name="node">The node to remove</param>
        public void RemoveChildElement(AbstractSavannahXmlNode node)
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
        public void AddBeforeChildElement(AbstractSavannahXmlNode node, AbstractSavannahXmlNode newNode)
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
        public void AddAfterChildElement(AbstractSavannahXmlNode node, AbstractSavannahXmlNode newNode)
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
        /// Resolve all PrioritizeInnerXml elements including child elements.
        /// </summary>
        /// <param name="ignoreComments">Whether to ignore the comments.</param>
        /// <param name="node">Target node. The current node is specified if it is null.</param>
        public void ResolvePrioritizeInnerXml(bool ignoreComments = true, AbstractSavannahXmlNode node = null)
        {
            node ??= this;

            if (node is SavannahTagNode tagNode)
            {
                if (!string.IsNullOrEmpty(tagNode.PrioritizeInnerXml))
                {
                    using var ms = SavannahXmlWriter.ConvertInnerXmlToXmlText(tagNode);
                    var cNode = SavannahXmlReader.GetChildNodesFromStream(ms, ignoreComments);
                    tagNode.ChildNodes = cNode;
                    tagNode.PrioritizeInnerXml = null;
                }
                else
                {
                    if (!tagNode.ChildNodes.Any())
                        return;
                    foreach (var nodeChildNode in tagNode.ChildNodes)
                    {
                        ResolvePrioritizeInnerXml(ignoreComments, nodeChildNode);
                    }
                }
            }
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// Generate the root node.
        /// </summary>
        /// <param name="tagName">Tag name of root.</param>
        /// <returns>Root node.</returns>
        public static SavannahTagNode CreateRoot(string tagName)
        {
            var root = new SavannahTagNode
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
        public static SavannahTagNode CreateElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<SavannahTagNode> commonXmlNodes = null)
        {
            if (attributeInfos == null)
                attributeInfos = new AttributeInfo[0];

            if (commonXmlNodes == null)
                commonXmlNodes = new SavannahTagNode[0];

            var node = new SavannahTagNode
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
        public static SavannahTagNode CreateElement(string tagName, IEnumerable<AttributeInfo> attributeInfos,
            string innerXml)
        {
            attributeInfos ??= new AttributeInfo[0];

            var node = new SavannahTagNode
            {
                TagName = tagName,
                Attributes = attributeInfos,
                PrioritizeInnerXml = innerXml
            };
            return node;
        }

        private static LinkedList<AbstractSavannahXmlNode> ResolveChildrenParent(LinkedList<AbstractSavannahXmlNode> childNodes, AbstractSavannahXmlNode parent)
        {
            foreach (var child in childNodes)
            {
                child.Parent = parent;
            }

            return childNodes;
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

            var element = (SavannahTagNode)obj;
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
            var hashCode = 1321504521;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TagName);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<AttributeInfo>>.Default.GetHashCode(Attributes);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<AbstractSavannahXmlNode>>.Default.GetHashCode(ChildNodes);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InnerText);
            return hashCode;
        }
        #endregion
    }
}
