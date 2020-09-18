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
    public class SavannahXmlWriter
    {
        #region Fields

        private readonly XmlDocument xDocument = new XmlDocument();
        private readonly XmlProcessingInstruction xDeclaration;

        #endregion

        #region Properties

        /// <summary>
        /// Whether to ignore the comments for PrioritizeInnerXml.
        /// </summary>
        public bool IgnoreComments { get; set; } = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize the class.
        /// </summary>
        public SavannahXmlWriter() : this(SavannahXmlConstants.Utf8Declaration)
        {
        }

        /// <summary>
        /// Initialize the class with the specified declaration.
        /// </summary>
        /// <param name="declaration">Declaration to be written in XML</param>
        public SavannahXmlWriter(string declaration)
        {
            xDeclaration = xDocument.CreateProcessingInstruction("xml", declaration);
        }

        #endregion

        #region Member Methods

        /// <summary>
        /// Writes the XML to the specified file.
        /// </summary>
        /// <param name="path">Path of the file to be written</param>
        /// <param name="root">The root of the XML to be written</param>
        public void Write(string path, SavannahXmlNode root)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            Write(fs, root);
        }

        /// <summary>
        /// Writes the XML to the specified file.
        /// </summary>
        /// <param name="stream">Stream to be written</param>
        /// <param name="root">The root of the XML to be written</param>
        public void Write(Stream stream, SavannahXmlNode root)
        {
            root.ResolvePrioritizeInnerXml(IgnoreComments);
            var xml = root.ToString();
            var declaration = xDeclaration.OuterXml;
            var data = Encoding.UTF8.GetBytes($"{declaration}\n{xml}\n");
            stream.Write(data, 0, data.Length);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Convert PrioritizeInnerXml To regular XML text.
        /// </summary>
        /// <param name="node">Target node</param>
        /// <returns>Stream written regular XML text.</returns>
        public static Stream ConvertInnerXmlToXmlText(SavannahXmlNode node)
        {
            var lines = node.PrioritizeInnerXml.UnifiedBreakLine().Split('\n');
            var spaceText = SavannahXmlNode.MakeSpace(node.IndentSize);
            var converted = string.Join("\n", lines.Select(x => $"{spaceText}{x}"));

            var xml = $"\n{converted}\n";
            var xDocument = new XmlDocument();
            var xDeclaration = xDocument.CreateProcessingInstruction("xml", SavannahXmlConstants.Utf8Declaration);
            var elem = xDocument.CreateElement("root");
            xDocument.AppendChild(xDeclaration);
            elem.InnerXml = xml;
            xDocument.AppendChild(elem);

            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.Unicode)
            {
                Formatting = Formatting.Indented,
                Indentation = 2,
                IndentChar = ' '
            };
            xDocument.WriteContentTo(writer);
            writer.Flush();
            ms.Flush();
            ms.Position = 0;

            return ms;
        }

        #endregion
    }
}
