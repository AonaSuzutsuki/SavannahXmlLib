namespace SavannahXmlLib.XmlWrapper.Nodes
{
    public class SavannahCdataNode : AbstractSavannahXmlNode
    {
        /// <summary>
        /// InnerXml of this node.
        /// </summary>
        public override string InnerXml => InnerText;

        public SavannahCdataNode()
        {
            TagName = SavannahXmlConstants.CdataTagName;
        }
    }
}
