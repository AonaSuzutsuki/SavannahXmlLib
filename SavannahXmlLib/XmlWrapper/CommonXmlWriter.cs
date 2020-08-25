using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CommonExtensionLib.Extensions;

namespace SavannahXmlLib.XmlWrapper
{
    /// <summary>
    /// It provides a set of functions for writing XML.
    /// </summary>
    public class CommonXmlWriter
    {
        private readonly XmlDocument xDocument = new XmlDocument();
        private readonly XmlProcessingInstruction xDeclaration;

        /// <summary>
        /// Initialize the class.
        /// </summary>
        public CommonXmlWriter() : this("version=\"1.0\"")
        {
        }

        /// <summary>
        /// Initialize the class with the specified declaration.
        /// </summary>
        /// <param name="declaration">Declaration to be written in XML</param>
        public CommonXmlWriter(string declaration)
        {
            xDeclaration = xDocument.CreateProcessingInstruction("xml", declaration);
        }

        /// <summary>
        /// Writes the XML to the specified file.
        /// </summary>
        /// <param name="path">Path of the file to be written</param>
        /// <param name="root">The root of the XML to be written</param>
        public void Write(string path, CommonXmlNode root)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            Write(fs, root);
        }

        /// <summary>
        /// Writes the XML to the specified file.
        /// </summary>
        /// <param name="stream">Stream to be written</param>
        /// <param name="root">The root of the XML to be written</param>
        public void Write(Stream stream, CommonXmlNode root)
        {
            using var ms = new MemoryStream();

            var elem = CreateXmlElement(root);
            xDocument.AppendChild(xDeclaration);
            xDocument.AppendChild(elem);

            xDocument.Save(ms);

            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms, Encoding.UTF8);
            while (sr.Peek() > -1)
            {
                var text = $"{ConvertStringReference(sr.ReadLine())}\r\n".UnifiedBreakLine();
                var data = Encoding.UTF8.GetBytes(text);
                stream.Write(data, 0, data.Length);
            }
        }

        private XmlNode CreateXmlElement(CommonXmlNode root)
        {
            if (root.NodeType == XmlNodeType.Tag)
            {
                var elem = xDocument.CreateElement(root.TagName);

                if (root.Attributes.Any())
                    foreach (AttributeInfo attributeInfo in root.Attributes)
                        elem.SetAttribute(attributeInfo.Name, attributeInfo.Value);
                if (!string.IsNullOrEmpty(root.InnerText) && !root.ChildNodes.Any())
                    elem.InnerText = root.InnerText;

                if (root.PrioritizeInnerXml != null)
                {
                    elem.InnerXml = root.PrioritizeInnerXml;
                    root.PrioritizeInnerXml = null;
                }
                else if (root.ChildNodes.Any())
                    foreach (var child in root.ChildNodes)
                        elem.AppendChild(CreateXmlElement(child));

                return elem;
            }
            else
            {
                var elem = xDocument.CreateTextNode(root.InnerText);
                return elem;
            }
        }

        private string ConvertStringReference(string text)
        {
            return text.Replace("&#xD;", "\r").Replace("&#xA;", "\n");
        }
    }
}
