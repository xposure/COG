using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace COG.Assets
{
    public interface IAssetEntry
    {
        string Extension { get; }
        AssetUri Uri { get; }
        Stream GetReadStream();
    }
}
