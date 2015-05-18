using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using COG.Assets;
using COG.Framework;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{

    #region TextureData
    public class TextureData : IAssetData
    {
        private byte[] m_data;

        public byte[] PixelData { get { return m_data; } }

        public TextureData(byte[] data)
        {
            m_data = data;
        }
    }
    #endregion

    #region TextureBitmapLoader
    public class TexturBitmapLoader : IAssetDataLoader<TextureData>
    {
        public TextureData Load(Stream fs)
        {
            // Data read from the header of the BMP file
            var header = new byte[54]; // Each BMP file begins by a 54-bytes header
            var dataPos = 0;     // Position in the file where the actual data begins
            var width = 0;
            var height = 0;
            var imageSize = 0;   // = width*height*3

            if (fs.Read(header, 0, 54) != 54)
            {
                // If not 54 bytes read : problem
                //Console.WriteLine("Not a correct BMP file [{0}]", filepath);
                return null;
            }

            if (header[0] != 'B' || header[1] != 'M')
            {
                //Console.WriteLine("Not a correct BMP file [{0}]", filepath);
                return null;
            }

            // Read ints from the byte array
            dataPos = BitConverter.ToInt32(header, 0x0A);
            imageSize = BitConverter.ToInt32(header, 0x22);
            width = BitConverter.ToInt32(header, 0x12);
            height = BitConverter.ToInt32(header, 0x16);

            // Some BMP files are misformatted, guess missing information
            if (imageSize == 0) imageSize = width * height * 3; // 3 : one byte for each Red, Green and Blue component
            if (dataPos == 0) dataPos = 54; // The BMP header is done that way

            // Actual RGB data
            var data = new byte[imageSize];
            fs.Read(data, 0, imageSize);

            return new TextureData(data);
        }
    }
    #endregion

    public abstract class Texture : DisposableObject, IAsset<TextureData>
    {
        private static int g_lastBoundTextureID = 0;

        private AssetUri m_uri;
        private int m_textureId;
        private List<KeyValuePair<TextureParameterName, int>> m_stateChanges;

        protected Texture()
        {
        }

        public AssetUri Uri { get { return m_uri; } }

        public bool IsValid { get { return m_textureId != 0; } }

        public int TextureID { get { return m_textureId; } }

        public abstract TextureTarget TextureTarget { get; }

        public void Bind()
        {
            if (!IsValid)
                return;

            if (g_lastBoundTextureID != m_textureId)
                GL.BindTexture(TextureTarget, m_textureId);

            ProcessTextureParams();
        }

        public void Reload(TextureData data)
        {
            if (m_textureId == 0)
                m_textureId = GL.GenTexture();

            if (!OnReload(data))
                Unload();
        }

        protected void Unload()
        {
            if (m_textureId != 0)
                GL.DeleteTexture(m_textureId);

            m_textureId = 0;
        }

        protected abstract bool OnReload(TextureData data);

        private void ProcessTextureParams()
        {
            if (m_stateChanges != null && m_stateChanges.Count > 0)
            {
                for (var i = 0; i < m_stateChanges.Count; ++i)
                    GL.TexParameter(TextureTarget, m_stateChanges[i].Key, m_stateChanges[i].Value);

                m_stateChanges.Clear();
            }
        }

        protected void ChangeTextureParam(TextureParameterName param, int val)
        {
            if (m_stateChanges == null)
                m_stateChanges = new List<KeyValuePair<TextureParameterName, int>>();

            for (var i = 0; i < m_stateChanges.Count; ++i)
            {
                if (m_stateChanges[i].Key == param)
                {
                    m_stateChanges[i] = new KeyValuePair<TextureParameterName, int>(param, val);
                    return;
                }
            }

            m_stateChanges.Add(new KeyValuePair<TextureParameterName, int>(param, val));
        }

        protected override void DisposedUnmanaged()
        {
            base.DisposedUnmanaged();

            Unload();
        }
    }

    public abstract class Texture2D : Texture
    {
        private TextureMagFilter m_magFilter = TextureMagFilter.Nearest;
        private TextureMinFilter m_minFilter = TextureMinFilter.Nearest;

        public TextureMagFilter MagFilter
        {
            get { return m_magFilter; }
            set
            {
                ChangeTextureParam(TextureParameterName.TextureMagFilter, (int)value);
                m_magFilter = value;
            }
        }

        public TextureMinFilter MinFilter
        {
            get { return m_minFilter; }
            set
            {
                ChangeTextureParam(TextureParameterName.TextureMinFilter, (int)value);
                m_minFilter = value;
            }
        }

        public abstract int Width { get; }
        public abstract int Height { get; }

        protected override bool OnReload(TextureData data)
        {
            throw new NotImplementedException();
        }
    }
}
