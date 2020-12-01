namespace SavannahXmlLib.XmlWrapper.Nodes
{
    public class SavannahCommentNode : AbstractSavannahXmlNode
    {
        /// <summary>
        /// InnerXml of this node.
        /// </summary>
        public override string InnerXml => InnerText;

        public SavannahCommentNode()
        {
            TagName = SavannahXmlConstants.CommentTagName;
        }
    }
}
