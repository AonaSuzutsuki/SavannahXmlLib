using System;
using System.Collections.Generic;
using SavannahXmlLib.XmlWrapper;

namespace SavannahXmlLib.Extensions
{
    /// <summary>
    /// Extended methods for SavannahXmlNode class.
    /// </summary>
    public static class SavannahXmlNodeExtensions
    {
        /// <summary>
        /// Convert enumerable AttributeInfo class to semi colon separated string.
        /// </summary>
        /// <param name="attributeInfos">Enumerable AttributeInfo class</param>
        /// <param name="separater">The separate string.</param>
        /// <returns>The separated attributes string.</returns>
        public static string ToAttributesText(this IEnumerable<AttributeInfo> attributeInfos, string separater)
        {
            return string.Join(separater, attributeInfos);
        }
    }
}
