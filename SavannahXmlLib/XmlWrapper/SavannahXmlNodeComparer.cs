using System.Collections.Generic;

namespace SavannahXmlLib.XmlWrapper
{
    public class SavannahXmlNodeComparer : IEqualityComparer<SavannahXmlNode>
    {
        public bool Equals(SavannahXmlNode x, SavannahXmlNode y)
        {
            return x == y;
        }

        public int GetHashCode(SavannahXmlNode obj)
        {
            return obj.GetHashCode();
        }
    }
}
