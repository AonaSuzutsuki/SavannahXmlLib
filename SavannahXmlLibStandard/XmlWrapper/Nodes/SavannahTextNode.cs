namespace SavannahXmlLib.XmlWrapper.Nodes
{
    public class SavannahTextNode : AbstractSavannahXmlNode
    {
        /// <summary>
        /// InnerXml of this node.
        /// </summary>
        public override string InnerXml => InnerText;

        public SavannahTextNode()
        {
            TagName = SavannahXmlConstants.TextTagName;
        }

        /// <summary>
        /// Generate the text node.
        /// </summary>
        /// <param name="innerText">The inner text.</param>
        /// <returns>The text node.</returns>
        public static SavannahTextNode CreateTextNode(string innerText)
        {
            var node = new SavannahTextNode
            {
                InnerText = innerText
            };
            return node;
        }
    }
}
