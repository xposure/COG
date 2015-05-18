
namespace COG.Assets
{
    
    public delegate DATA AssetResolver<out DATA>(AssetUri uri)
        where DATA : IAssetData;

    //public delegate IAsset<IAssetData> AssetFactory(AssetUri uri, IAssetData data);
        
}
