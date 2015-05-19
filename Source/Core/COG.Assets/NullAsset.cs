using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace COG.Assets
{
    public class NullAssetData : IAssetData
    {
        public static readonly NullAssetData NULL = new NullAssetData();
    }

    public class NullAsset : AbstractAsset<NullAssetData>
    {
        public NullAsset()
            : base("asset:null:null")
        {

        }

        public override void Reload(NullAssetData t)
        {
        }

        public static NullAssetData LoadData(Stream stream)
        {
            return NullAssetData.NULL;
        }
    }

    //public class NullAssetDataLoader : IAssetDataLoader<NullAssetData>
    //{
    //    public static readonly NullAssetDataLoader NULL = new NullAssetDataLoader();

    //    public NullAssetData LoadData(System.IO.Stream stream)
    //    {
    //        return NullAssetData.NULL;
    //    }

    //}
}
