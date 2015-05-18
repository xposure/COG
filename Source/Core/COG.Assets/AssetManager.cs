using System;
using System.Collections.Generic;
using System.IO;
using COG.Framework;
using COG.Logging;

namespace COG.Assets
{
    public class AssetManager : Registry
    {
        private static readonly Logger g_logger = Logger.getLogger(typeof(AssetManager));

        private Dictionary<string, IAssetSource> m_sources = new Dictionary<string, IAssetSource>();
        private Dictionary<int, AssetFactory<IAssetData, IAsset>> m_factories = new Dictionary<int, AssetFactory<IAssetData, IAsset>>();
        private Dictionary<int, List<AssetResolver<IAssetData>>> m_resolvers = new Dictionary<int, List<AssetResolver<IAssetData>>>();
        private Dictionary<AssetUri, IAsset> m_assetCache = new Dictionary<AssetUri, IAsset>();

        public void AddAssetSource(IAssetSource source)
        {
            m_sources.Add(source.ID, source);
            source.Init();
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
                List<AssetResolver<IAssetData>> resolvers = m_resolvers.Find(uri.Type.id);
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
            var resolvers = m_resolvers.Find(type.id);
            if(resolvers == null)
            {
                resolvers = new List<AssetResolver<IAssetData>>();
                m_resolvers.Add(type.id, resolvers);
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
            m_factories.Add(type.id, new AssetFactory<IAssetData, IAsset>((uri, data) =>
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
                g_logger.warn("Unable to resolve asset: {0}", uri);
                return default(U);
            }

            return (U)uri.Type.Build(assetEntry);
        }

        public T LoadAsset<T, U>(AssetUri uri)
            where T : IAsset<U>
            where U : IAssetData
        {
            if (!uri.IsValid())
            {
                g_logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            IAsset asset;
            if (m_assetCache.TryGetValue(uri, out asset))
                return (T)asset;

            AssetFactory<IAssetData, IAsset> factory;
            if (!m_factories.TryGetValue(uri.Type.id, out factory))
            {
                g_logger.warn("Unsupported asset type: {0}", uri.Type);
                return default(T);
            }

            var data = LoadAssetData<U>(uri);
            if (data == null)
                return default(T);

            asset = factory(uri, data);
            if (asset == null)
            {
                g_logger.error("factory '{0}' returned null", typeof(T));
                return default(T);
            }

            if (!(asset is T))
            {
                g_logger.error("factory returned a type '{0} 'that wasn't of '{1}'", asset.GetType(), typeof(T));
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
                g_logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            AssetFactory<IAssetData, IAsset> factory;
            if (!m_factories.TryGetValue(uri.Type.id, out factory))
            {
                g_logger.warn("Unsupported asset type: {0}", uri.Type);
                return default(T);
            }

            var t = factory(uri, data);

            if (t is T)
                return (T)t;

            if (t != null)
            {
                g_logger.error("factory returned a type '{0} 'that wasn't of T", t.GetType());
            }

            return default(T);
        }

        public T CacheAsset<T>(T asset)
            where T : IAsset
        {
            var uri = asset.Uri;
            if (!uri.IsValid())
            {
                g_logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            m_assetCache[uri] = asset;
            return asset;
        }

        //public void reload()
        //{
        //    //TODO: throw new NotImplementedException();
        //    //foreach(var asset in _assetCache.Values)

        //}

        public void Clear()
        {
            m_assetCache.Clear();
        }
    }

}
