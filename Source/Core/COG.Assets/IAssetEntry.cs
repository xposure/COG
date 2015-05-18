using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace COG.Assets
{
    public interface IAssetEntry
    {
        AssetUri uri { get; }
        Stream getReadStream();
    }
}
