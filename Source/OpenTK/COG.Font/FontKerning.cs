using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Atma
{
    [Serializable]
    public class FontKerning
    {
        [XmlAttribute("amount")]
        public Int32 Amount
        {
            get;
            set;
        }

        [XmlAttribute("first")]
        public Int32 First
        {
            get;
            set;
        }

        [XmlAttribute("second")]
        public Int32 Second
        {
            get;
            set;
        }
    }


}
