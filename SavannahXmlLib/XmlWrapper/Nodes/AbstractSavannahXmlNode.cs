using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonCoreLib.Bool;
using CommonExtensionLib.Extensions;
using SavannahXmlLib.Extensions;

namespace SavannahXmlLib.XmlWrapper.Nodes
{
    public abstract class AbstractSavannahXmlNode
    {
        public static int DefaultIndentSize = 2;

        /// <summary>
        /// The name of this node.
        /// </summary>
        public string TagName { get; set; }

        public AbstractSavannahXmlNode Parent { get; internal set; }

        /// <summary>
        /// InnerText without tag of this node.
        /// </summary>
        public string InnerText { get; set; } = string.Empty;

        /// <summary>
        /// InnerXml of this node.
        /// </summary>
        public abstract string InnerXml { get; }

        public string OutterXml => GenerateOutterXml(this, DefaultIndentSize);

        #region Methods

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
        protected string ToString(IEnumerable<AbstractSavannahXmlNode> commonXmlNodes, int indentSize)
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
        protected static string ToString(AbstractSavannahXmlNode node, int indentSize, int space = 0)
        {
            var spaceText = MakeSpace(space);

            var sb = new StringBuilder();
            if (node is SavannahTagNode tagNode)
            {
                var attr = tagNode.Attributes.ToAttributesText(" ");
                attr = string.IsNullOrEmpty(attr) ? attr : $" {attr}";

                if (tagNode.ChildNodes.Any())
                {
                    sb.Append($"{spaceText}<{tagNode.TagName}{attr}>\n");
                    space += indentSize;
                    foreach (var childNode in tagNode.ChildNodes)
                    {
                        sb.Append($"{ToString(childNode, indentSize, space)}\n");
                    }

                    sb.Append($"{spaceText}</{tagNode.TagName}>");
                }
                else
                {
                    sb.Append($"{spaceText}<{tagNode.TagName}{attr} />");
                }
            }
            else if (node is SavannahTextNode)
            {
                sb.Append(ResolveInnerText(node, spaceText));
            }
            else if (node is SavannahCdataNode)
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

        protected static string ResolveInnerText(AbstractSavannahXmlNode node, string spaceText)
        {
            var text = node.InnerText.UnifiedBreakLine();
            var lines = text.Split('\n');
            var converted = string.Join("\n", lines.Select(x => $"{spaceText}{x}"));
            return converted;
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

        protected static string GenerateOutterXml(AbstractSavannahXmlNode node, int indentSize)
        {
            if (node.Parent == null)
                return string.Empty;

            if (node.Parent is SavannahTagNode parentTagNode)
            {
                var indent = MakeSpace(indentSize);
                var str = node.ToString();
                var innerXml = string.Join("", node.ToString().Split('\n').Select(item => $"{indent}{item}\n"));
                var attr = parentTagNode.Attributes.ToAttributesText(" ");
                return $"<{node.Parent.TagName}{attr}>\n{innerXml}</{node.Parent.TagName}>";
            }

            return string.Empty;
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

            var element = (AbstractSavannahXmlNode)obj;
            var collector = new BoolCollector();

            collector.ChangeBool(TagName, TagName == element.TagName);
            collector.ChangeBool(InnerText, InnerText.Equals(element.InnerText));

            return collector.Value;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hash value.</returns>
        public override int GetHashCode()
        {
            var hashCode = 2014152738;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TagName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InnerText);
            return hashCode;
        }
        #endregion
    }
}
