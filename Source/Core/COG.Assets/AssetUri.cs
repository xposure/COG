
using System;
namespace COG.Assets
{
    public struct AssetUri : IComparable<AssetUri>, IEquatable<AssetUri>
    {
        /// <summary>
        /// The character(s) use to separate the module name from other parts of the Uri
        /// </summary>    
        public static readonly char MODULE_SEPARATOR = ':';
        public static readonly char TYPE_SEPARATOR = ':';

        private string normalisedName;
        private string name;

        public static readonly AssetUri NULL = new AssetUri(AssetType.NULL, "engine", "null");

        #region Constructors
        /// <summary>
        /// Creates a SimpleUri from a string in the format "module:object". If the string does not match this format, it will be marked invalid
        /// </summary>
        /// <param name="simpleUri">module:object string</param>
        public AssetUri(string simpleUri)
            : this()
        {
            string[] split = simpleUri.Split(MODULE_SEPARATOR);
            if (split.Length == 3)
            {
                moduleName = split[0];
                normalisedModuleName = split[0].ToLowerInvariant();
                type = AssetType.find(split[1]);
                objectName = split[2];
                normalisedObjectName = split[2].ToLowerInvariant();
                normalisedName = normalisedModuleName + MODULE_SEPARATOR + type.name.ToLowerInvariant() + TYPE_SEPARATOR + normalisedObjectName;
                name = moduleName + MODULE_SEPARATOR + type.name + TYPE_SEPARATOR + objectName;
            }
        }

        /// <summary>
        /// Creates a SimpleUri for the given module:object combo
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="objectName"></param>
        public AssetUri(AssetType _type, string _moduleName, string _objectName)
            : this()
        {
            //Contract.RequiresNotEmpty(_moduleName, "moduleName");
            //Contract.RequiresNotEmpty(_objectName, "objectName");

            type = _type;
            moduleName = _moduleName;
            objectName = _objectName;
            normalisedModuleName = _moduleName.ToLowerInvariant();
            normalisedObjectName = _objectName.ToLowerInvariant();
            normalisedName = normalisedModuleName + MODULE_SEPARATOR + type.name.ToLowerInvariant() + TYPE_SEPARATOR + normalisedObjectName;
            name = moduleName + MODULE_SEPARATOR + type.name + TYPE_SEPARATOR + objectName;
        }
        #endregion Constructors

        #region Properties

        public AssetType type { get; private set; }

        public string moduleName { get; private set; }

        public string normalisedModuleName { get; private set; }

        public string objectName { get; private set; }

        public string normalisedObjectName { get; private set; }

        #endregion Properties

        #region Methods

        public bool isValid()
        {
            return type != AssetType.NULL && !string.IsNullOrEmpty(normalisedModuleName) && !string.IsNullOrEmpty(normalisedObjectName);
        }

        public string toNormalisedString()
        {
            if (!isValid())
            {
                return string.Empty;
            }
            return normalisedName;
        }

        public override string ToString()
        {
            if (!isValid())
            {
                return string.Empty;
            }
            return name;
        }

        public int CompareTo(AssetUri other)
        {
            return string.Compare(normalisedName, other.toNormalisedString());
        }

        public bool Equals(AssetUri uri)
        {
            return this.normalisedName == uri.normalisedName;
        }

        public override int GetHashCode()
        {
            return toNormalisedString().GetHashCode();
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
            return uri.normalisedName;
        }

        public static implicit operator AssetUri(string val)
        {
            if (string.IsNullOrEmpty(val))
                return AssetUri.NULL;

            return new AssetUri(val);
        }

        public static bool operator ==(AssetUri a, AssetUri b)
        {
            if (a.isValid() && b.isValid())
                return a.normalisedName == b.normalisedName;

            return false;
        }

        public static bool operator !=(AssetUri a, AssetUri b)
        {
            if (a.isValid() && b.isValid())
                return a.normalisedName != b.normalisedName;

            return false;
        }
        #endregion
    }

}
