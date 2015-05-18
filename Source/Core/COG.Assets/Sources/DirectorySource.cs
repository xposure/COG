using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using cphillips;

namespace COG.Assets
{
    public class DirectorySource : AbstractSource
    {
        private class FileEntry : IAssetEntry
        {
            private string _path;
            private AssetUri _uri;

            public AssetUri uri { get { return _uri; } }

            public FileEntry(AssetUri uri, string fullpath)
            {
                _uri = uri;
                _path = fullpath;
            }

            public Stream getReadStream()
            {
                //return TitleContainer.OpenStream("..\\content\\" + _path);
                return new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }

        //private static readonly Logger logger = Logger.getLogger(typeof(DirectorySource));

        private string _directory;

        public DirectorySource(string id, string directory)
            : base(id)
        {
            if (!System.IO.Path.IsPathRooted(directory))
                directory = System.IO.Path.Combine(System.Environment.CurrentDirectory, directory);

            _directory = directory;
        }

        protected override void load()
        {
            if (!System.IO.Directory.Exists(_directory))
            {
                //logger.error("Directory not found '{0}'", _directory);
                return;
            }

            var dirs = new Stack<string>();
            dirs.Push(_directory);

            while (dirs.Count > 0)
            {
                var currentDir = dirs.Pop();
                var files = System.IO.Directory.GetFiles(currentDir);
                foreach (var file in files)
                {
                    var relFile = file.Substring(_directory.Length + 1).Replace("\\", "/");
                    var uri = getAssetUri(relFile);
                    if (!uri.isValid())
                    {
                        //logger.error("Could not get a valid uri for '{0}'", relFile);
                        continue;
                    }

                    var ae = new FileEntry(uri, file);
                    addEntry(ae);
                }

                foreach (var dir in System.IO.Directory.GetDirectories(currentDir))
                    dirs.Push(dir);
            }
        }
    }
}
