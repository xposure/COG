using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COG.Assets
{
    public class TextData : IAssetData
    {
        public string m_content;

        public TextData(string content)
        {
            m_content = content;
        }

        public string Content { get { return m_content; } }

        public static TextData LoadData(Stream stream)
        {
            var sr = new StreamReader(stream);
            return new TextData(sr.ReadToEnd());
        }
    }


}
