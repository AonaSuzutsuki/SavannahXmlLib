using System;
using System.Collections.Generic;

namespace SavannahXmlLib.XmlWrapper
{
    /// <summary>
    /// This holds the Value of XML.
    /// </summary>
    public class CommonXmlText
    {
        /// <summary>
        /// String of XML values without tags
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// String containing tags in XML value
        /// </summary>
        public string Xml { get; set; } = string.Empty;

        /// <summary>
        /// Return a string of XML values without tags.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Return a hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 1249999374 + EqualityComparer<string>.Default.GetHashCode(Text);
        }

        /// <summary>
        /// Returns object-to-object equivalence.
        /// </summary>
        /// <param name="obj">Target object</param>
        /// <returns>Equivalence as bool</returns>
        public override bool Equals(object obj)
        {
            return obj is CommonXmlText text &&
                   Text == text.Text;
        }
    }
}
