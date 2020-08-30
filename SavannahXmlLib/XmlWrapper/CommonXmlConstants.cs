using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavannahXmlLib.XmlWrapper
{
    public static class CommonXmlConstants
    {
        public static readonly string Utf8Declaration = "version=\"1.0\"";
        public static string Declaration => $"<?xml {Utf8Declaration} ?>";
    }
}
