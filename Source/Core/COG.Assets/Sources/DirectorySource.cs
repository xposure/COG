using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using COG.Logging;
//using cphillips;

namespace COG.Assets
{
    public class DirectorySource : AbstractSource
    {
        private class FileEntry : IAssetEntry
        {
            private string m_path;
            private string m_extension;
            private AssetUri m_uri;

            public string Extension { get { return m_extension; } }
            public AssetUri Uri { get { return m_uri; } }

            public FileEntry(AssetUri uri, string fullpath, string extension)
            {
                m_uri = uri;
                m_path = fullpath;
                m_extension = extension;
            }

            public Stream GetReadStream()
            {
                return new FileStream(m_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }

        private static readonly Logger g_logger = Logger.GetLogger(typeof(DirectorySource));

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
                g_logger.Error("Directory not found '{0}'", _directory);
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
                    string extension;
                    var relFile = file.Substring(_directory.Length + 1).Replace("\\", "/");
                    var uri = getAssetUri(relFile, out extension);
                    if (!uri.IsValid())
                    {
                        g_logger.Error("Could not get a valid uri for '{0}'", relFile);
                        continue;
                    }

                    var ae = new FileEntry(uri, file, extension);
                    addEntry(ae);
                }

                foreach (var dir in System.IO.Directory.GetDirectories(currentDir))
                    dirs.Push(dir);
            }
        }
    }
}
