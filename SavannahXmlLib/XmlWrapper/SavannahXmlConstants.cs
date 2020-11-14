using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavannahXmlLib.XmlWrapper
{
    public static class SavannahXmlConstants
    {
        public static readonly string Utf8Declaration = "version=\"1.0\"";
        public static string Declaration => $"<?xml {Utf8Declaration} ?>";

        public static string TextTagName = "#text";
        public static string CommentTagName = "#comment";
        public static string CdataTagName = "#cdata-section";
    }
}
