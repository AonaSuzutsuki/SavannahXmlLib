﻿using System;
using System.Collections.Generic;

namespace SavannahXmlLib.XmlWrapper
{
    public class CommonXmlText
    {
        public string Text { get; set; } = string.Empty;
        public string Xml { get; set; } = string.Empty;

        public override string ToString()
        {
            return Text;
        }

        public override int GetHashCode()
        {
            return 1249999374 + EqualityComparer<string>.Default.GetHashCode(Text);
        }

        public override bool Equals(object obj)
        {
            return obj is CommonXmlText text &&
                   Text == text.Text;
        }
    }
}
