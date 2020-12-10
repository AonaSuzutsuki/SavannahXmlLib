using System.Collections.Generic;
using System.IO;
using System.Xml;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace SavannahXmlLib.XmlWrapper
{
    public class CachedSavannahXmlReader : SavannahXmlReader
    {
        public CachedSavannahXmlReader(string xmlPath, bool ignoreComments = true) : base(xmlPath, ignoreComments)
        {
        }

        public CachedSavannahXmlReader(Stream stream, bool ignoreComments = true) : base(stream, ignoreComments)
        {
        }

        private Dictionary<string, XmlNodeList> _xmlNodeListCache = new Dictionary<string, XmlNodeList>();
        private Dictionary<XmlNode, AbstractSavannahXmlNode> _savannahCache = new Dictionary<XmlNode, AbstractSavannahXmlNode>();

        public void ClearCache()
        {
            _xmlNodeListCache = new Dictionary<string, XmlNodeList>();
            _savannahCache = new Dictionary<XmlNode, AbstractSavannahXmlNode>();
        }

        protected override XmlNodeList SelectNodes(string xpath)
        {
            if (_xmlNodeListCache.ContainsKey(xpath))
                return _xmlNodeListCache[xpath];

            var nodes = base.SelectNodes(xpath);
            _xmlNodeListCache.Add(xpath, nodes);

            return nodes;
        }

        protected override Dictionary<XmlNode, AbstractSavannahXmlNode> CreateTable(XmlNode node, int indentSize, bool isRemoveSpace)
        {
            if (_savannahCache.ContainsKey(node))
                return _savannahCache;

            return base.CreateTable(node, indentSize, isRemoveSpace);
        }
    }
}
