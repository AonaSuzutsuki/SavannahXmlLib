using System;
using System.Collections.Generic;

namespace SavannahXmlLib.XmlWrapper
{
    /// <summary>
    /// Extended methods for CommonXmlNode class.
    /// </summary>
    public static class CommonXmlNodeExtension
    {
        /// <summary>
        /// Convert enumerable AttributeInfo class to semi colon separated string.
        /// </summary>
        /// <param name="attributeInfos">Enumerable AttributeInfo class</param>
        /// <param name="separete">The separate string.</param>
        /// <returns>The separated attributes string.</returns>
        public static string ToAttributesText(this IEnumerable<AttributeInfo> attributeInfos, string separete)
        {
            return string.Join(separete, attributeInfos);
        }
    }
}
