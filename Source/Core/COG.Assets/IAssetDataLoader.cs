using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COG.Assets
{
    public delegate DATA AssetDataLoader<out DATA>(Stream stream);
 
    //public interface IAssetDataLoader<out DATA>
    //    where DATA : IAssetData
    //{
    //    DATA LoadData(Stream stream);

    //}
}
