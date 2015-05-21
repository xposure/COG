using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using COG.Assets;

namespace Atma
{
    [Serializable]
    [XmlRoot("font")]
    public class FontData : IAssetData
    {
        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public List<FontCharacter> Chars
        {
            get;
            set;
        }

        [XmlElement("common")]
        public FontCommon Common
        {
            get;
            set;
        }

        [XmlElement("info")]
        public FontInfo Info
        {
            get;
            set;
        }

        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public List<FontKerning> Kernings
        {
            get;
            set;
        }

        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public List<FontPage> Pages
        {
            get;
            set;
        }
    }


}
