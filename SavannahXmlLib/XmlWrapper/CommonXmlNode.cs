﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonCoreLib.Bool;

namespace SavannahXmlLib.XmlWrapper
{
    public static class CommonXmlNodeExtension
    {
        public static string ToAttributesText(this IEnumerable<AttributeInfo> attributeInfos)
        {
            return string.Join(", ", attributeInfos);
        }
    }

    public enum XmlNodeType
    {
        Tag,
        Text
    }

    public class CommonXmlNode
    {
        #region Properties
        public XmlNodeType NodeType { get; set; }
        public string TagName { get; set; }
        public IEnumerable<AttributeInfo> Attributes
        {
            get => attributes;
            set => attributes = new HashSet<AttributeInfo>(value);
        }
        public IEnumerable<CommonXmlNode> ChildNodes
        {
            get => childNodes;
            set => childNodes = new List<CommonXmlNode>(value);
        }

        public string InnerText { get; set; } = string.Empty;
        public string InnerXml => ToString(ChildNodes);

        public string PrioritizeInneXml { get; set; }
        #endregion

        #region Fields
        private HashSet<AttributeInfo> attributes = new HashSet<AttributeInfo>();
        private List<CommonXmlNode> childNodes = new List<CommonXmlNode>();
        #endregion

        #region Member Methods
        public void AppendAttribute(string name, string value)
        {
            var attr = new AttributeInfo { Name = name, Value = value};
            AppendAttribute(attr);
        }
        public void AppendAttribute(AttributeInfo info)
        {
            if (!attributes.Contains(info))
                attributes.Add(info);
        }
        public AttributeInfo GetAttribute(string name)
        {
            foreach (var attributeInfo in attributes)
            {
                if (attributeInfo.Name == name)
                    return attributeInfo;
            }
            return new AttributeInfo();
        }

        public void RemoveAttribute(string name)
        {
            var attr = GetAttribute(name);
            if (attributes.Contains(attr))
                attributes.Remove(attr);
        }
        public void RemoveAttribute(AttributeInfo info)
        {
            if (attributes.Contains(info))
                attributes.Remove(info);
        }

        public CommonXmlNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<CommonXmlNode> commonXmlNodes = null)
        {
            var node = CreateElement(tagName, attributeInfos, commonXmlNodes);
            return CreateChildElement(node);
        }

        public CommonXmlNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos
            , string innerXml)
        {
            var node = CreateElement(tagName, attributeInfos, innerXml);
            return CreateChildElement(node);
        }

        public CommonXmlNode CreateChildElement(CommonXmlNode node)
        {
            childNodes.Add(node);
            return node;
        }

        public override string ToString()
        {
            return ToString(this);
        }

        public string ToString(IEnumerable<CommonXmlNode> commonXmlNodes)
        {
            var sb = new StringBuilder();
            foreach (var node in commonXmlNodes)
            {
                sb.Append($"{ToString(node)}\n");
            }
            return sb.ToString();
        }

        public static string ToString(CommonXmlNode node, int space = 0)
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
                    space += 4;
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
                sb.Append($"{spaceText}{node.InnerText}");
            }
            

            return sb.ToString();
        }
        #endregion

        #region Static Methods
        public static string MakeSpace(int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(" ");
            }

            return sb.ToString();
        }
        public static CommonXmlNode CreateRoot(string tagName)
        {
            var root = new CommonXmlNode
            {
                TagName = tagName
            };
            return root;
        }

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

        public static CommonXmlNode CreateElement(string tagName, IEnumerable<AttributeInfo> attributeInfos,
            string innerXml)
        {
            if (attributeInfos == null)
                attributeInfos = new AttributeInfo[0];

            var node = new CommonXmlNode
            {
                TagName = tagName,
                Attributes = attributeInfos,
                PrioritizeInneXml = innerXml
            };
            return node;
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var element = (CommonXmlNode)obj;
            var boolcollector = new BoolCollector();

            boolcollector.ChangeBool(TagName, TagName == element.TagName);
            boolcollector.ChangeBool(Attributes, Attributes.SequenceEqual(element.Attributes));
            boolcollector.ChangeBool(ChildNodes, ChildNodes.SequenceEqual(element.ChildNodes));
            boolcollector.ChangeBool(InnerText, InnerText.Equals(element.InnerText));

            return boolcollector.Value;
        }

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
