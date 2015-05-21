using System;
using System.Collections.Generic;
using System.Text;
using COG.Logging;

namespace COG.Assets
{
    public abstract class AbstractSource : IAssetSource
    {
        private readonly static Logger logger = Logger.GetLogger(typeof(AbstractSource));

        private AssetManager m_assets;
        private Dictionary<AssetUri, IAssetEntry> m_entries = new Dictionary<AssetUri, IAssetEntry>();
        private Dictionary<int, List<IAssetEntry>> m_entryByTypes = new Dictionary<int, List<IAssetEntry>>();

        public AbstractSource(string id)
        {
            this.ID = id;
        }

        public void Init(AssetManager assets)
        {
            m_assets = assets;
            m_entries.Clear();

            foreach (var list in m_entryByTypes)
                list.Value.Clear();

            m_entryByTypes.Clear();

            load();
        }


        protected abstract void load();

        protected void addEntry(IAssetEntry ae)
        {
            if (m_entries.ContainsKey(ae.Uri))
            {
                logger.Warn("{0} already existed", ae.Uri);
            }

            m_entries[ae.Uri] = ae;

            AssetType type;
            m_assets.GetTypeFor(ae.Extension, out type);
            
            List<IAssetEntry> byType;
            if (!m_entryByTypes.TryGetValue(type.id, out byType))
            {
                byType = new List<IAssetEntry>();
                m_entryByTypes.Add(type.id, byType);
            }

            byType.Add(ae);
        }

        public string ID { get; private set; }

        public IEnumerable<IAssetEntry> List()
        {
            foreach (var ae in m_entries.Values)
                yield return ae;
        }

        public IEnumerable<IAssetEntry> List(AssetType type)
        {
            List<IAssetEntry> byType;
            if (m_entryByTypes.TryGetValue(type.id, out byType))
            {
                foreach (var ae in byType)
                    yield return ae;
            }
        }

        public IAssetEntry Find(AssetUri uri)
        {
            IAssetEntry ae;
            if (m_entries.TryGetValue(uri, out ae))
                return ae;

            return null;
        }

        protected AssetUri getAssetUri(string relativePath, out string extension)
        {
            extension = string.Empty;
            relativePath = relativePath.ToLower();
            String[] parts = relativePath.Split(new char[] { '/' }, 2);
            if (parts.Length > 1)
            {
                //int lastSepIndex = parts[1].IndexOf("/");
                //if (lastSepIndex != -1)
                //{
                //    parts[1] = parts[1].Substring(lastSepIndex + 1);
                //}
                int extensionSeparator = parts[1].LastIndexOf(".");
                if (extensionSeparator != -1)
                {
                    var name = parts[1].Substring(0, extensionSeparator);
                    extension = parts[1].Substring(extensionSeparator + 1);
                    AssetType assetType;
                    if (m_assets.GetTypeFor(extension, out assetType))
                    {
                        return assetType.CreateUri(ID, name);
                    }
                }
            }

            return null;
        }
    }
}
