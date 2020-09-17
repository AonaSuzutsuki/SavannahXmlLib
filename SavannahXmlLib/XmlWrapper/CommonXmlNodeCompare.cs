using System.Collections.Generic;

namespace SavannahXmlLib.XmlWrapper
{
    class CommonXmlNodeCompare : IEqualityComparer<CommonXmlNode>
    {
        public bool Equals(CommonXmlNode x, CommonXmlNode y)
        {
            return x == y;
        }

        public int GetHashCode(CommonXmlNode obj)
        {
            return obj.GetHashCode();
        }
    }
}
