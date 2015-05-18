using System;
using System.Collections.Generic;
using System.IO;

namespace COG.Assets
{
    public class AssetManager //: GameSystem
    {
        //public static readonly GameUri Uri = "engine:assets";

        //private static readonly Logger logger = Logger.getLogger(typeof(AssetManager));

        private Dictionary<string, IAssetSource> _sources = new Dictionary<string, IAssetSource>();
        //private Dictionary<AssetType, Func<Stream, IAssetLoader<IAssetData>>> _loaders = new Dictionary<int, Func<Stream, IAssetLoader<IAssetData>>>();
        //private Dictionary<int, AssetFactory<IAssetData, IAsset<IAssetData>>> _factories2 = new Dictionary<int, AssetFactory<IAssetData, IAsset<IAssetData>>>();
        private Dictionary<int, AssetFactory<IAssetData, IAsset>> _factories = new Dictionary<int, AssetFactory<IAssetData, IAsset>>();
        private Dictionary<int, List<AssetResolver<IAssetData>>> _resolvers = new Dictionary<int, List<AssetResolver<IAssetData>>>();

        private Dictionary<AssetUri, IAsset> _assetCache = new Dictionary<AssetUri, IAsset>();

        public void addAssetSource(IAssetSource source)
        {
            _sources.Add(source.ID, source);
            source.Init();
        }

        public IAssetEntry getAsset(AssetUri uri)
        {
            var source = _sources.Find(uri.normalisedModuleName);
            if (source != null)
            {
                return source.Find(uri);
            }

            return null;
        }

        public U resolve<U>(AssetUri uri)
            where U : IAssetData
        {
            if (uri.isValid())
            {
                List<AssetResolver<IAssetData>> resolvers = _resolvers.Find(uri.type.id);
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

        public void addResolver<DATA>(AssetType type, Func<AssetUri, DATA> resolver)
            where DATA : IAssetData
        {
            var resolvers = _resolvers.Find(type.id);
            if(resolvers == null)
            {
                resolvers = new List<AssetResolver<IAssetData>>();
                _resolvers.Add(type.id, resolvers);
            }

            resolvers.Add(new AssetResolver<IAssetData>((uri) =>
            {
                return resolver(uri);
            }));
        }

        public void setFactory<DATA, ASSET>(AssetType type, Func<AssetUri, DATA, ASSET> factory)
            where DATA : IAssetData
            where ASSET : IAsset<DATA>
        {
            _factories.Add(type.id, new AssetFactory<IAssetData, IAsset>((uri, data) =>
            {
                return factory(uri, (DATA)data);
                //return (IAsset<IAssetData>)r;
            }));
        }

        protected U loadAssetData<U>(AssetUri uri)
            where U : IAssetData
        {
            if (!uri.isValid())
                return default(U);

            var assetData = resolve<U>(uri);
            if (assetData != null)
                return assetData;

            var assetEntry = getAsset(uri);
            if (assetEntry == null)
            {
                //logger.warn("Unable to resolve asset: {0}", uri);
                return default(U);
            }

            return (U)uri.type.build(assetEntry);
        }

        public T loadAsset<T, U>(AssetUri uri)
            where T : IAsset<U>
            where U : IAssetData
        {
            if (!uri.isValid())
            {
                //logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            IAsset asset;
            if (_assetCache.TryGetValue(uri, out asset))
                return (T)asset;

            AssetFactory<IAssetData, IAsset> factory;
            if (!_factories.TryGetValue(uri.type.id, out factory))
            {
                //logger.warn("Unsupported asset type: {0}", uri.type);
                return default(T);
            }

            var data = loadAssetData<U>(uri);
            if (data == null)
                return default(T);

            asset = factory(uri, data);
            if (asset == null)
            {
                //logger.error("factory '{0}' returned null", typeof(T));
                return default(T);
            }

            if (!(asset is T))
            {
                //logger.error("factory returned a type '{0} 'that wasn't of '{1}'", asset.GetType(), typeof(T));
                return default(T);
            }

            _assetCache.Add(uri, asset);
            return (T)asset;
        }

        public T generateAsset<T, U>(AssetUri uri, U data)
            where T : IAsset<U>
            where U : IAssetData
        {
            if (!uri.isValid())
            {
                //logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            AssetFactory<IAssetData, IAsset> factory;
            if (!_factories.TryGetValue(uri.type.id, out factory))
            {
                //logger.warn("Unsupported asset type: {0}", uri.type);
                return default(T);
            }

            var t = factory(uri, data);

            if (t is T)
                return (T)t;

            if (t != null)
            {
                //logger.error("factory returned a type '{0} 'that wasn't of T", t.GetType());
            }

            return default(T);
        }

        public T cacheAsset<T>(T asset)
            where T : IAsset
        {
            var uri = asset.Uri;
            if (!uri.isValid())
            {
                //logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            _assetCache[uri] = asset;
            return asset;
        }

        //public void reload()
        //{
        //    //TODO: throw new NotImplementedException();
        //    //foreach(var asset in _assetCache.Values)

        //}

        public void clear()
        {
            _assetCache.Clear();
        }
    }

}
