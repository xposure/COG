using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using COG.Assets;
using COG.Framework;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{

    #region TextureData
    public class TextureData2D : IAssetData
    {
        private byte[] m_data;
        private int m_width, m_height;
        private PixelInternalFormat m_pixelInternalFormat;
        private PixelFormat m_pixelFormat;
        private PixelType m_pixelType;


        public TextureData2D(byte[] data, int width, int height, PixelInternalFormat pixelInternalFormat, PixelFormat pixelFormat, PixelType pixelType)
        {
            m_data = data;
            m_width = width;
            m_height = height;
            m_pixelInternalFormat = pixelInternalFormat;
            m_pixelFormat = pixelFormat;
            m_pixelType = pixelType;
        }

        public byte[] PixelData { get { return m_data; } }
        public int Width { get { return m_width; } }
        public int Height { get { return m_height; } }
        public PixelInternalFormat PixelInternalFormat { get { return m_pixelInternalFormat; } }
        public PixelFormat PixelFormat { get { return m_pixelFormat; } }
        public PixelType PixelType { get { return m_pixelType; } }

        public static TextureData2D FromColorArray(Color[] data, int width, int height)
        {
            var ubytes = new byte[data.Length * 4];
            for (var i = 0; i < data.Length; i++)
            {
                var index = i * 4;
                ubytes[index] = (byte)(data[i].R * 255);
                ubytes[index + 1] = (byte)(data[i].G * 255);
                ubytes[index + 2] = (byte)(data[i].B * 255);
                ubytes[index + 3] = (byte)(data[i].A * 255);
            }

            return new TextureData2D(ubytes, width, height, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte);
        }

        public static TextureData2D LoadPng(Stream fs)
        {
            using (var bitmap = new System.Drawing.Bitmap(fs))
            {
                var bd = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                              System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

                var stride = Math.Abs(bd.Stride);
                var bgraData = new byte[bitmap.Height * stride];

                System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, bgraData, 0, bitmap.Height * stride);

                bitmap.UnlockBits(bd);

                return new TextureData2D(bgraData, bitmap.Width, bitmap.Height, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte);
            }
        }

        public static TextureData2D LoadBitmap(Stream fs)
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

            return new TextureData2D(data, width, height, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
        }

        public static Texture2D CreateMetaball(int radius, Func<float, int, int, float> falloff, Func<float, int, int, Color> colorPicker)
        {
            int length = radius * 2;
            var colors = new byte[length * length * 4];

            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    var vector = Vector2.One - (new Vector2(x, y) / radius);
                    //float distance = Vector2.Distance(Vector2.One,
                    //    new Vector2(x, y) / radius);
                    var distance = vector.Length;
                    var alpha = falloff(distance, x, y);

                    var color = colorPicker(distance, x, y);
                    color.A = alpha;
                    //color.A = Utility.Clamp(alpha * 2, 1f, 0f);
                    //color = Color.FromNonPremultiplied(color.R, color.G, color.B, Utility.Clamp(alpha + 0.5f, 1f, 0f));
                    var index = (y * length + x) * 4;

                    colors[index] = (byte)(color.R * 255);
                    colors[index + 1] = (byte)(color.B * 255);
                    colors[index + 2] = (byte)(color.G * 255);
                    colors[index + 3] = (byte)(color.A * 255);
                }
            }

            var tdata = new TextureData2D(colors, length, length, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte);
            return new Texture2D("dredger:texture:test", tdata);
        }

        public static Color ColorWhite(float distance, int x, int y)
        {
            return Color.FromNonPremultiplied(1, 1, 1, 1);
        }

        public static Color ColorBlack(float distance, int x, int y)
        {
            return Color.FromNonPremultiplied(0, 0, 0, 0);
        }

        public static float CircleFalloff(float distance, int x, int y)
        {
            if (0 < distance && distance < 1f / 3f)
            {
                return 1 - (3 * (distance * distance));
            }
            else if (1f / 3f < distance && distance < 1f)
            {
                return (3f / 2f) * ((1f - distance) * (1f - distance));
            }
            else if (distance == 0)
                return 1f;

            return 0f;
        }

    }
    #endregion

    public abstract class Texture : DisposableObject
    {
        private static int g_lastBoundTextureID = 0;

        private int m_textureId;
        private List<KeyValuePair<TextureParameterName, int>> m_stateChanges;

        public bool IsValid { get { return m_textureId != 0; } }

        public int TextureID { get { return m_textureId; } }

        public abstract TextureTarget TextureTarget { get; }

        public void Bind()
        {
            if (!IsValid)
                return;

            if (g_lastBoundTextureID != m_textureId)
            {
                GL.BindTexture(TextureTarget, m_textureId);
                g_lastBoundTextureID = m_textureId;
            }

            ProcessTextureParams();
        }

        protected void GLSetupTexture()
        {
            if (m_textureId == 0)
                m_textureId = GL.GenTexture();

            Bind();

        }

        protected void Destroy()
        {
            if (m_textureId != 0)
                GL.DeleteTexture(m_textureId);

            m_textureId = 0;
        }

        protected void ProcessTextureParams()
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

            Destroy();
        }
    }

    public class Texture2D : Texture, IAsset<TextureData2D>
    {
        public static readonly AssetType TEXTURE = AssetType.Create("TEXTURE");
        private TextureMagFilter m_magFilter = TextureMagFilter.Nearest;
        private TextureMinFilter m_minFilter = TextureMinFilter.Nearest;
        private AssetUri m_uri;
        private int m_width, m_height;

        public Texture2D(AssetUri uri, TextureData2D data)
        {
            m_uri = uri;

            Reload(data);
        }

        public AssetUri Uri { get { return m_uri; } }

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

        public override TextureTarget TextureTarget
        {
            get { return TextureTarget.Texture2D; }
        }

        public int Width { get { return m_width; } }

        public int Height { get { return m_height; } }

        public void Reload(TextureData2D data)
        {
            m_width = data.Width;
            m_height = data.Height;

            GLSetupTexture();

            GL.TexImage2D(TextureTarget.Texture2D, 0, data.PixelInternalFormat, m_width, m_height, 0, data.PixelFormat, data.PixelType, data.PixelData);

            MagFilter = m_magFilter;
            MinFilter = m_minFilter;

            ProcessTextureParams();

        }

        public Vector2 GetUV(int x, int y)
        {
            return new Vector2(x / (float)m_width, y / (float)m_height);
        }

    }
}
