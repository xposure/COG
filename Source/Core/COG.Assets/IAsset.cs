
using System;
namespace COG.Assets
{
    public interface IReloadableAsset<T> : IAsset
        where T : IAssetData
    {
        void reload(T data);

    }

    public interface IAsset : IDisposable
    //where T: IAssetData
    {
        AssetUri Uri { get; }
        bool IsDisposed { get; }

    }

    public interface IAsset<in T> : IAsset
        where T : IAssetData
    {

        void Reload(T t);

    }
}
