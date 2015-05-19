using System.Collections.Generic;
using System;
using System.IO;
using COG.Framework;
using COG.Logging;

namespace COG.Assets
{
    public class AssetType
    {
        //private static readonly Logger g_logger = Logger.GetLogger(typeof(AssetType));

        private static int g_typeId;

        //private static Dictionary<string, List<AssetType>> m_subDirLookup = new Dictionary<string, List<AssetType>>();
        //private static Dictionary<string, AssetType> m_nameLookup = new Dictionary<string, AssetType>();

        public static readonly AssetType NULL = Create("NULL");

        //public static bool GetTypeFor(string dir, string extension, out AssetType type)
        //{
        //    type = NULL;
        //    var types = m_subDirLookup.Find(dir);
        //    if (types != null)
        //    {
        //        foreach (var t in types)
        //        {
        //            if (t.CheckExt(extension))
        //            {
        //                type = t;
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public static AssetType Find(string name)
        //{
        //    //Contract.RequiresNotNull(name, "name");

        //    var normalizedName = name.ToLower();
        //    AssetType at;
        //    if (m_nameLookup.TryGetValue(normalizedName, out at))
        //        return at;

        //    return NULL;
        //}

        public static AssetType Create(string name)
        {
            var normalizedName = name.ToLower();

            var at = new AssetType(g_typeId++, name);//, folders, exts);
           // m_nameLookup.Add(normalizedName, at);
            return at;
        }

        public readonly int id;
        public readonly string name;
        public readonly string nameNormalized;
        //private readonly Func<Stream, IAssetData> _loader;
        //private readonly string[] _dirs;
        //private readonly string[] _exts;

        private AssetType(int _id, string _name/*, Func<Stream, IAssetData> loader,  string[] dirs, string[] exts*/)
        {
            id = _id;
            name = _name;
            nameNormalized = name.ToLower();
            //_loader = loader;
            //_dirs = dirs;
            //_exts = exts;

            //if (dirs != null && exts != null)
            //{
            //    foreach (var d in dirs)
            //    {
            //        var sub = m_subDirLookup.FindOrCreate(d);
            //        sub.Add(this);
            //    }
            //}
        }

        public AssetUri CreateUri(string module, string item)
        {
            return new AssetUri(module, name, item);
        }

        //public IAssetData Build(IAssetEntry e)
        //{
        //    if (_loader == null)
        //        return null;

        //    using (var s = e.getReadStream())
        //        return _loader(s);
        //}

        //public bool CheckExt(string ext)
        //{
        //    if (_exts == null)
        //        return false;

        //    foreach (var e in _exts)
        //        if (e == ext)
        //            return true;

        //    return false;
        //}

        public override bool Equals(object obj)
        {
            if (obj != null && obj is AssetType)
                return Equals((AssetType)obj);

            return false;
        }

        public bool Equals(AssetType other)
        {
            return this.id == other.id;// && this.name == other.name;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public static bool operator ==(AssetType a, AssetType b)
        {
            return System.Object.ReferenceEquals(a, b);
        }

        public static bool operator !=(AssetType a, AssetType b)
        {

            return !(a == b);
        }

        public override string ToString()
        {
            return string.Format("AssetType: {0}", this.name);
        }
    }
}
