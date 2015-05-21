using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Atma
{
    [Serializable]
    public class FontPage
    {
        [XmlAttribute("file")]
        public String File
        {
            get;
            set;
        }

        [XmlAttribute("id")]
        public Int32 ID
        {
            get;
            set;
        }
    }
}
