using System;
using System.Collections.Generic;
using System.Text;

namespace COG.Assets
{
    public interface IAssetSource 
    {
        string ID { get; }
        void Init();
        IEnumerable<IAssetEntry> List();
        IEnumerable<IAssetEntry> List(AssetType type);
        IAssetEntry Find(AssetUri uri);
    }
}
