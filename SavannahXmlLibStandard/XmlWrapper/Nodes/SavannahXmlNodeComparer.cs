﻿using System.Collections.Generic;

namespace SavannahXmlLib.XmlWrapper.Nodes
{
    public class SavannahXmlNodeComparer : IEqualityComparer<AbstractSavannahXmlNode>
    {
        public bool Equals(AbstractSavannahXmlNode x, AbstractSavannahXmlNode y)
        {
            return x == y;
        }

        public int GetHashCode(AbstractSavannahXmlNode obj)
        {
            return obj.GetHashCode();
        }
    }
}
