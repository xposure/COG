
namespace COG.Assets
{
    public delegate ASSET AssetFactory<in DATA, out ASSET>(AssetUri uri, DATA data)
        where DATA : IAssetData
        where ASSET : IAsset;

    //public delegate IAsset<IAssetData> AssetFactory(AssetUri uri, IAssetData data);
        
}
