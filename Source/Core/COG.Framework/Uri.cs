#region GPLv3 License

/*
Atma
Copyright © 2014 Atma Project Team

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License V3
as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
General Public License V3 for more details.

You should have received a copy of the GNU General Public License V3
along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/

#endregion

#region Namespace Declarations

using System;


#endregion Namespace Declarations

namespace COG.Framework
{
    #region IUri
    /// <summary>
    /// Uris are used to identify resources, like assets and systems introduced by mods. Uris can then be serialized/deserialized to and from Strings.
    /// Uris are case-insensitive. They have a normalised form which is lower-case (using English casing).
    /// Uris are immutable.
    /// 
    /// All uris include a module name as part of their structure.
    /// </summary>
    public interface IUri : IComparable<IUri>, IEquatable<IUri>
    {
        /// <summary>
        /// The name of the resource.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The name of the module the resource in question resides in.
        /// </summary>
        string Module { get; }

        /// <returns>The normalised form of the uri. Generally this means lower case.</returns>
        string ToNormalizedString();

        /// <summary>
        /// </summary>
        /// <returns>Whether this uri represents a valid, well formed uri.</returns>
        bool IsValid();

    }
    #endregion

    #region SimpleUri
    public struct SimpleUri : IUri
    {
        public static readonly char MODULE_SEPARATOR = ':';
        public static readonly SimpleUri NULL = new SimpleUri("engine", "null");

        private int m_hashCode;
        private string m_name;
        private string m_module;


        #region Constructors

        public SimpleUri(string module, string name)
            : this()
        {
            Contract.RequiresNotEmpty(module, "module");
            Contract.RequiresNotEmpty(name, "name");

            m_hashCode = 0;
            m_module = module;
            m_name = name;
        }
        #endregion Constructors

        #region Properties

        public string Module { get { return m_module; } }

        public string Name { get { return m_name; } }

        #endregion Properties

        #region Methods

        public static SimpleUri ParseUri(string simpleUri)
        {
            string[] split = simpleUri.Split(MODULE_SEPARATOR);
            if (split.Length == 2)
                return new SimpleUri(split[0], split[1]);

            return NULL;
        }

        public bool IsValid()
        {
            return m_hashCode != 0 && m_hashCode != NULL.m_hashCode;
        }

        public string ToNormalizedString()
        {
            if (!IsValid())
                return NULL.ToNormalizedString();

            var normalizedModuleName = UriUtil.normalise(m_module);
            var normalizedName = UriUtil.normalise(m_name);

            return string.Format("{0}{1}{2}", normalizedModuleName, MODULE_SEPARATOR, normalizedName);
        }

        public override string ToString()
        {
            if (!IsValid())
                return NULL.ToString();

            return string.Format("{0}{1}{2}", m_module, MODULE_SEPARATOR, m_name);
        }

        public int CompareTo(IUri other)
        {
            return string.Compare(this.ToNormalizedString(), other.ToNormalizedString());
        }

        public bool Equals(IUri other)
        {
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return ToNormalizedString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is SimpleUri)
                return Equals((SimpleUri)obj);

            return false;
        }

        public bool Equals(SimpleUri uri)
        {
            if(this.IsValid() && uri.IsValid())
                return this.m_hashCode == uri.m_hashCode;

            return false;
        }

        #endregion Methods

        #region Operators

        public static implicit operator string(SimpleUri uri)
        {
            return uri.ToNormalizedString();
        }

        public static implicit operator SimpleUri(string val)
        {
            return ParseUri(val);
        }

        public static bool operator ==(SimpleUri a, SimpleUri b)
        {
            if (a.IsValid() && b.IsValid())
                return a.m_hashCode == b.m_hashCode;

            return false;
        }

        public static bool operator !=(SimpleUri a, SimpleUri b)
        {
            if (a.IsValid() && b.IsValid())
                return a.m_hashCode != b.m_hashCode;

            return false;
        }
        #endregion
    }

    #endregion

    #region UriUtil
    public static class UriUtil
    {
        /// <summary>
        /// Normalises a uri or uri part. The normal form is used for comparison/string matching.
        /// This process includes lower-casing the uri.
        /// </summary>
        /// <param name="value">value A uri or uri part</param>
        /// <returns>The normal form of the given value.</returns>
        public static string normalise(string value)
        {
            return value.ToLowerInvariant();
        }
    }
    #endregion
}

