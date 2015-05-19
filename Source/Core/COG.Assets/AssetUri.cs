using System;
using COG.Framework;

namespace COG.Assets
{
    public struct AssetUri : IUri
    {
        public static readonly char MODULE_SEPARATOR = ':';
        public static readonly char TYPE_SEPARATOR = ':';

        private int m_hashCode;
        private string m_moduleName, m_assetName;
        private string m_type;
        //private AssetType m_type;

        public static readonly AssetUri NULL = new AssetUri("engine", "null", "<invalid>");

        #region Constructors
        public AssetUri(string moduleName, string typeName, string assetName)
            : this()
        {
            Contract.RequiresNotEmpty(moduleName, "moduleName");
            Contract.RequiresNotEmpty(typeName, "typeName");
            Contract.RequiresNotEmpty(assetName, "objectName");

            m_hashCode = 0;
            m_moduleName = moduleName;
            m_type = typeName;
            m_assetName = assetName;
            
            ComputeHashCode();
        }
        #endregion Constructors

        #region Properties

        public string  Type { get { return m_type; } }

        public string Module { get { return m_moduleName; } }

        public string Name { get { return m_assetName; } }

        #endregion Properties

        #region Methods

        private void ComputeHashCode()
        {
            if (string.IsNullOrEmpty(m_moduleName) || string.IsNullOrEmpty(m_assetName) || string.IsNullOrEmpty(m_type))
            {
                m_hashCode = 0;
            }
            else
            {
                var normalizedModuleName = UriUtil.normalise(m_moduleName);
                var normalizedAssetName = UriUtil.normalise(m_assetName);
                var normalizedTypeName = UriUtil.normalise(m_type);

                m_hashCode = normalizedModuleName.GetHashCode() ^ normalizedTypeName.GetHashCode() ^ normalizedAssetName.GetHashCode();
            }
        }

        public bool IsValid()
        {
            return m_hashCode != 0 && m_hashCode != NULL.m_hashCode;
        }

        public static AssetUri ParseUri(string uri)
        {
            string[] split = uri.Split(MODULE_SEPARATOR);
            if (split.Length == 3)
            {
                //var type = AssetType.Find(split[1]);
                //if (type != null)
                    return new AssetUri(split[0], split[1], split[2]);
            }

            return NULL;
        }

        public string ToNormalizedString()
        {
            if (!IsValid())
            {
                return NULL.ToNormalizedString();
            }

            var normalizedModuleName = UriUtil.normalise(m_moduleName);
            var normalizedTypeName = UriUtil.normalise(m_type);
            var normalizedAssetName = UriUtil.normalise(m_assetName);

            return string.Format("{0}{1}{2}{3}{4}", normalizedModuleName, MODULE_SEPARATOR,
                normalizedTypeName, TYPE_SEPARATOR, normalizedAssetName);
        }

        public override string ToString()
        {
            if (!IsValid())
            {
                return NULL.ToString();
            }

            return string.Format("{0}{1}{2}{3}{4}", m_moduleName, MODULE_SEPARATOR,
                           m_type, TYPE_SEPARATOR, m_assetName);
        }

        public int CompareTo(IUri other)
        {
            return string.Compare(this.ToNormalizedString(), other.ToNormalizedString());
        }

        public bool Equals(IUri obj)
        {
            if (obj != null && obj is AssetUri)
                return this.Equals((AssetUri)obj);

            return false;
        }

        public bool Equals(AssetUri uri)
        {
            if (this.IsValid() && uri.IsValid())
                return this.m_hashCode == uri.m_hashCode;

            return false;
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is AssetUri)
                return this.Equals((AssetUri)obj);

            return false;
        }

        #endregion Methods

        #region Operators

        public static implicit operator string(AssetUri uri)
        {
            return uri.ToString();
        }

        public static implicit operator AssetUri(string val)
        {
            if (string.IsNullOrEmpty(val))
                return AssetUri.NULL;

            return AssetUri.ParseUri(val);
        }

        public static bool operator ==(AssetUri a, AssetUri b)
        {
            if (a.IsValid() && b.IsValid())
                return a.m_hashCode == b.m_hashCode;

            return false;
        }

        public static bool operator !=(AssetUri a, AssetUri b)
        {
            if (a.IsValid() && b.IsValid())
                return a.m_hashCode != b.m_hashCode;

            return false;
        }
        #endregion
    }

}
