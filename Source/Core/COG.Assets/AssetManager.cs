using System;
using System.Collections.Generic;
using System.IO;
using COG.Framework;
using COG.Logging;

namespace COG.Assets
{
    public class AssetManager : Registry
    {
        private readonly Logger m_logger = Logger.GetLogger(typeof(AssetManager));

        private Dictionary<string, IAssetSource> m_sources = new Dictionary<string, IAssetSource>();
        private Dictionary<string, AssetFactory<IAssetData, IAsset>> m_factories = new Dictionary<string, AssetFactory<IAssetData, IAsset>>();
        private Dictionary<string, List<AssetResolver<IAssetData>>> m_resolvers = new Dictionary<string, List<AssetResolver<IAssetData>>>();
        private Dictionary<AssetUri, IAsset> m_assetCache = new Dictionary<AssetUri, IAsset>();

        private Dictionary<string, AssetLoader> m_assetLoaders = new Dictionary<string, AssetLoader>();
        private Dictionary<string, AssetType> m_nameLookup = new Dictionary<string, AssetType>();

        //private Dictionary<string, List<AssetType>> m_subDirLookup = new Dictionary<string, List<AssetType>>();

        public void AddAssetSource(IAssetSource source)
        {
            m_sources.Add(source.ID, source);
            source.Init(this);
        }

        public void RegisterTypeExtension<T>(AssetType type, string extension, AssetDataLoader<T> loader)
            where T : IAssetData
        {
            var loadWrapper =

            extension = extension.ToLower();

            var assetLoader = m_assetLoaders.Find(extension);
            if (assetLoader == null)
            {
                assetLoader = new AssetLoader(type, new Func<Stream, IAssetData>(
                    s =>
                    {
                        var data = loader(s);
                        return data;
                    })
                );

                m_assetLoaders.Add(extension, assetLoader);

                if(!m_nameLookup.ContainsKey(type.name.ToLower()))
                    m_nameLookup.Add(type.name.ToLower(), type);
            }
        }

        //public void RegisterTypeDirectory(AssetType type, string directory)
        //{

        //}

        internal bool GetTypeFor(string extension, out AssetType type)
        {
            type = AssetType.NULL;

            var assetLoader = m_assetLoaders.Find(extension);
            //var types = m_subDirLookup.Find(dir);
            //if (types != null)
            if(assetLoader != null)
            {
                type = assetLoader.Type;
                return true;
                //foreach (var t in types)
                //{
                //    if (t.CheckExt(extension))
                //    {
                //        type = t;
                //        return true;
                //    }
                //}
            }

            return false;
        }

        public IAssetEntry FindAsset(AssetUri uri)
        {
            var source = m_sources.Find(UriUtil.normalise(uri.Module));
            if (source != null)
            {
                return source.Find(uri);
            }

            return null;
        }

        public U ResolveAsset<U>(AssetUri uri)
            where U : IAssetData
        {
            if (uri.IsValid())
            {
                List<AssetResolver<IAssetData>> resolvers = m_resolvers.Find(uri.Type.ToLower());
                if (resolvers != null)
                {
                    foreach (var resolver in resolvers)
                    {
                        var data = (U)resolver(uri);
                        if (data != null)
                            return (U)data;
                    }
                }
            }

            return default(U);
        }

        public void AddResolver<DATA>(AssetType type, Func<AssetUri, DATA> resolver)
            where DATA : IAssetData
        {
            var resolvers = m_resolvers.Find(type.name.ToLower());
            if (resolvers == null)
            {
                resolvers = new List<AssetResolver<IAssetData>>();
                m_resolvers.Add(type.name.ToLower(), resolvers);
            }

            resolvers.Add(new AssetResolver<IAssetData>((uri) =>
            {
                return resolver(uri);
            }));
        }

        public void SetFactory<DATA, ASSET>(AssetType type, Func<AssetUri, DATA, ASSET> factory)
            where DATA : IAssetData
            where ASSET : IAsset<DATA>
        {
            m_factories.Add(type.nameNormalized, new AssetFactory<IAssetData, IAsset>((uri, data) =>
            {
                return factory(uri, (DATA)data);
                //return (IAsset<IAssetData>)r;
            }));
        }

        protected U LoadAssetData<U>(AssetUri uri)
            where U : IAssetData
        {
            if (!uri.IsValid())
                return default(U);

            var assetData = ResolveAsset<U>(uri);
            if (assetData != null)
                return assetData;

            var assetEntry = FindAsset(uri);
            if (assetEntry == null)
            {
                m_logger.warn("Unable to resolve asset: {0}", uri);
                return default(U);
            }

            var assetLoader = m_assetLoaders.Find(assetEntry.Extension);
            if (assetLoader == null)
                return default(U);

            using (var stream = assetEntry.GetReadStream())
                return (U)assetLoader.Loader(stream);
        }

        public T LoadAsset<T, U>(AssetUri uri)
            where T : IAsset<U>
            where U : IAssetData
        {
            if (!uri.IsValid())
            {
                m_logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            IAsset asset;
            if (m_assetCache.TryGetValue(uri, out asset))
                return (T)asset;

            AssetFactory<IAssetData, IAsset> factory;
            if (!m_factories.TryGetValue(uri.Type.ToLower(), out factory))
            {
                m_logger.warn("Unsupported asset type: {0}", uri.Type);
                return default(T);
            }

            var data = LoadAssetData<U>( uri);
            if (data == null)
                return default(T);

            asset = factory(uri, data);
            if (asset == null)
            {
                m_logger.error("factory '{0}' returned null", typeof(T));
                return default(T);
            }

            if (!(asset is T))
            {
                m_logger.error("factory returned a type '{0} 'that wasn't of '{1}'", asset.GetType(), typeof(T));
                return default(T);
            }

            m_assetCache.Add(uri, asset);
            return (T)asset;
        }

        public T GenerateAsset<T, U>(AssetUri uri, U data)
            where T : IAsset<U>
            where U : IAssetData
        {
            if (!uri.IsValid())
            {
                m_logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            var type = m_nameLookup.Find(uri.Type.ToLower());
            AssetFactory<IAssetData, IAsset> factory;
            if (!m_factories.TryGetValue(type.nameNormalized, out factory))
            {
                m_logger.warn("Unsupported asset type: {0}", uri.Type);
                return default(T);
            }

            var t = factory(uri, data);

            if (t is T)
                return (T)t;

            if (t != null)
            {
                m_logger.error("factory returned a type '{0} 'that wasn't of T", t.GetType());
            }

            return default(T);
        }

        public T CacheAsset<T>(T asset)
            where T : IAsset
        {
            var uri = asset.Uri;
            if (!uri.IsValid())
            {
                m_logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            m_assetCache[uri] = asset;
            return asset;
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            foreach (var asset in m_assetCache.Values)
                asset.Dispose();

            m_assetCache.Clear();
        }
        //public void reload()
        //{
        //    //TODO: throw new NotImplementedException();
        //    //foreach(var asset in _assetCache.Values)

        //}

    //    public void Clear()
    //    {
    //        m_assetCache.Clear();
    //    }
    }

}
