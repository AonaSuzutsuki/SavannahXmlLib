using System;
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
        public const string Utf8Declaration = "version=\"1.0\"";

        private readonly XmlDocument xDocument = new XmlDocument();
        private readonly XmlProcessingInstruction xDeclaration;

        /// <summary>
        /// Initialize the class.
        /// </summary>
        public CommonXmlWriter() : this(Utf8Declaration)
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
            root.ResolvePrioritizeInnerXml();
            var xml = root.ToString();
            var declaration = xDeclaration.OuterXml;
            var data = Encoding.UTF8.GetBytes($"{declaration}\n{xml}\n");
            stream.Write(data, 0, data.Length);
        }

        public static Stream ConvertInnerXmlToXmlText(CommonXmlNode node)
        {
            var xml = node.PrioritizeInnerXml;
            var xDocument = new XmlDocument();
            var xDeclaration = xDocument.CreateProcessingInstruction("xml", Utf8Declaration);
            var elem = xDocument.CreateElement("root");
            xDocument.AppendChild(xDeclaration);
            elem.InnerXml = xml;
            xDocument.AppendChild(elem);

            var ms = new MemoryStream();
            xDocument.Save(ms);
            ms.Position = 0;

            return ms;
        }
    }
}
