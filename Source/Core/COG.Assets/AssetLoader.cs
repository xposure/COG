using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COG.Assets
{
    public class AssetLoader
    {
        private Func<Stream, IAssetData> m_loader;
        private AssetType m_type;
        //private List<string> m_directories = new List<string>();

        public AssetLoader(AssetType type, Func<Stream, IAssetData> loader)
        {
            m_type = type;
            m_loader = loader;
        }

        public Func<Stream, IAssetData> Loader { get { return m_loader; } }
        public AssetType Type { get { return m_type; } }
        //public bool CheckDirectory(string dir)
        //{
        //    foreach (var d in m_directories)
        //        if (d == dir)
        //            return true;

        //    return false;
        //}

        //public void AddDirectory(string dir)
        //{
        //    m_directories.Add(dir.ToLower());
        //}
    }
}
