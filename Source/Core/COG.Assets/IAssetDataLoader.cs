using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COG.Assets
{
    public interface IAssetDataLoader<out T>
        where T: IAssetData
    {
        T Load(Stream stream);
    }
}
