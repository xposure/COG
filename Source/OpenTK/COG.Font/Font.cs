using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using COG.Assets;
using COG.Graphics;
using OpenTK;


namespace Atma
{
    public class Font : AbstractAsset<FontData>
    {
        #region Static
        public static readonly AssetType FONT = AssetType.Create("FONT");
        #endregion

        private Dictionary<char, FontCharacter> m_characterMap;
        private Dictionary<long, int> m_characterKerning;
        private FontData m_fontFile;
        private Texture2D[] m_texturePages;
        //private TextureRef _texture;
        //private SpriteBatch m_batch = new SpriteBatch();
        //private AssetManager _manager;
        private AssetManager m_assets;

        public Font(AssetUri uri, AssetManager assets, FontData fontFile)
            : base(uri)
        {

            m_fontFile = fontFile;
            m_assets = assets;

            Reload(fontFile);

            //_fontFile = fontFile;
            //_material = Root.instance.assets.loadAsset<Texture2D, TextureData>(fontTexture);
            //_material.SetBlendState( BlendState.NonPremultiplied);
            //_material.textureName = fontTexture;

            //_texture = Root.instance.resources.findTexture(fontTexture);

            //_characterMap = new Dictionary<char, FontCharacter>();

            //foreach (var fontCharacter in _fontFile.Chars)
            //{
            //    char c = (char)fontCharacter.ID;
            //    _characterMap.Add(c, fontCharacter);
            //    if (fontCharacter.Height + fontCharacter.YOffset > MaxLineHeight)
            //        MaxLineHeight = fontCharacter.Height + fontCharacter.YOffset;
            //}
        }

        public override void Reload(FontData t)
        {
            m_texturePages = new Texture2D[m_fontFile.Pages.Count];

            //var mdata = new MaterialData();
            //mdata.SetBlendOpaque();
            //mdata.SetSamplerPointClamp();

            for (var i = 0; i < m_texturePages.Length; i++)
            {
                var file = t.Pages[i].File;
                var index = file.LastIndexOf('.');
                if (index > -1)
                    file = file.Substring(0, index);

                m_texturePages[i] = m_assets.LoadTexture(new AssetUri(Uri.Module, "TEXTURE", file));
                //m_texturePages[i] = _manager.getTexture(new GameUri(Uri.Module, file));
            }

            //_material = CoreRegistry.require<ResourceManager>(ResourceManager.Uri).createMaterialFromTexture(fontTexture, fontTexture);
            //_material.SetBlendState( BlendState.NonPremultiplied);
            //_material.textureName = fontTexture;

            //_texture = Root.instance.resources.findTexture(fontTexture);

            MaxLineHeight = 0;
            m_characterMap = new Dictionary<char, FontCharacter>(m_fontFile.Chars.Count);
            foreach (var fontCharacter in m_fontFile.Chars)
            {
                var c = (char)fontCharacter.ID;
                m_characterMap.Add(c, fontCharacter);
                //fontCharacter.material = m_texturePages[fontCharacter.Page];
                if (fontCharacter.Height + fontCharacter.YOffset > MaxLineHeight)
                    MaxLineHeight = fontCharacter.Height + fontCharacter.YOffset;

            }

            m_characterKerning = new Dictionary<long, int>(m_fontFile.Kernings.Count);
            foreach (var fk in m_fontFile.Kernings)
            {
                m_characterKerning.Add(((long)fk.First << 32) + fk.Second, fk.Amount);
            }
            //_characterKerning.Add(((long)(char)'t' << 32) + (long)(char)'u', 1);
            //_characterKerning.Add(((long)(char)'u' << 32) + (long)(char)'v', -1);

        }



        public int MaxLineHeight { get; private set; }

        //internal void DrawText(int renderQueue, Vector2 pos, float scale, string text, Color color)
        //{
        //    DrawText(renderQueue, pos, scale, text, color, 0);
        //}

        public bool CanFit(string text, float width)
        {
            return false;
        }

        public Vector2 MeasureString(string text, Vector2 maxSize)
        {
            if(Real.IsPositiveInfinity(maxSize.X))
                return MeasureString(text);

            var size = Vector2.Zero;
            var currentSize = Vector2.Zero;
            var current = 0;

            while (current != -1 && current < text.Length)
            {
                current = FindWordIndexFromBounds(current, maxSize.X, text, out currentSize);

                if (size.X < currentSize.X)
                    size.X = currentSize.X;

                size.Y += currentSize.Y;
            }

            return size;
        }

        public Vector2 MeasureMinWrappedString(string text)
        {
            var size = Vector2.Zero;
            var width = 0f;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];

                FontCharacter fc;
                if (m_characterMap.TryGetValue(c, out fc))
                {
                    width += fc.XAdvance;

                    if (c == ' ')
                    {
                        size.Y = MaxLineHeight;
                        if (width > size.X)
                            size.X = width;
                    }
                }
            }

            return size;
        }

        public Vector2 MeasureString(string text)
        {
            float dx = 0;
            float dy = 0;
            foreach (char c in text)
            {
                FontCharacter fc;
                if (m_characterMap.TryGetValue(c, out fc))
                {
                    dx += fc.XAdvance;
                    if (fc.Height + fc.YOffset > dy)
                        dy = fc.Height + fc.YOffset;
                }
            }
            return new Vector2(dx, dy);
        }

        public int FindWordIndexFromBounds(int start, float width, string text, out Vector2 size)
        {
            var index = -1;
            size = Vector2.Zero;

            for (int i = start; i < text.Length; i++)
            {
                var c = text[i];

                FontCharacter fc;
                if (m_characterMap.TryGetValue(c, out fc))
                {
                    if (size.X + fc.XAdvance > width)
                        return index;

                    size.X += fc.XAdvance;

                    if (c == ' ')
                        index = i + 1;

                    if (fc.Height + fc.YOffset > size.Y)
                        size.Y = fc.Height + fc.YOffset;
                }
            }

            index = text.Length;

            return index;
        }

        internal void DrawText(SpriteRenderer rm, int renderQueue, Vector2 pos, float scale, string text, Color color, float depth)
        {
            DrawText(rm, renderQueue, pos, scale, text, color, depth, null);
        }

        internal void DrawText(SpriteRenderer rm, int renderQueue, Vector2 pos, float scale, string text, Color color, float depth, float? width)
        {
            float dx = (float)Math.Floor(pos.X);
            float dy = (float)Math.Floor(pos.Y);
            //m_batch.Begin();
            int prevChar = 0;
            foreach (char c in text)
            {
                FontCharacter fc;
                if (m_characterMap.TryGetValue(c, out fc))
                {
                    if (width.HasValue && dx + fc.XAdvance * scale > width.Value + pos.X)
                        break;

                    var kernKey = ((long)prevChar << 32) + fc.ID;
                    var kernAmount = 0;
                    m_characterKerning.TryGetValue(kernKey, out kernAmount);
                    dx += kernAmount * scale;


                    var uv0 = m_texturePages[fc.Page].GetUV(fc.X, fc.Y);
                    var uv1 = m_texturePages[fc.Page].GetUV(fc.Width, fc.Height);
                    //var sourceRectangle = AxisAlignedBox2.FromRect(fc.X, fc.Y, fc.Width, fc.Height);// AxisAlignedBox2.FromRect(fc.X, fc.Y, fc.Width, fc.Height);
                    var destRectangle = AxisAlignedBox2.FromRect((int)(dx + fc.XOffset * scale), (int)(dy + fc.YOffset * scale), (int)(fc.Width * scale), (int)(fc.Height * scale));
                    //var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    var sprite = Sprite.Create(m_texturePages[fc.Page], destRectangle.X0, destRectangle.Y1, destRectangle.X1, destRectangle.Y0);
                    sprite.SetDepth(depth);
                    sprite.SetColor(color);
                    sprite.SetTexture(uv0, uv1);
                    rm.AddQuad(sprite);

                    //rm.Draw(renderQueue, _material[fc.Page], destRectangle, sourceRectangle, color, 0f, new Vector2(0f, 0f), SpriteEffects.None, depth);
                    //spriteBatch.Draw(_texture, position, sourceRectangle, Color.White);

                    dx += fc.XAdvance * scale;
                    prevChar = fc.ID;
                }
            }
            //m_batch.End();
        }

        internal void DrawWrappedOnWordText(SpriteRenderer rm, int renderQueue, Vector2 pos, float scale, string text, Color color, float depth, Vector2 size)
        {
            float dx = (float)Math.Floor(pos.X);
            float dy = (float)Math.Floor(pos.Y);

            var currentSize = Vector2.Zero;
            var current = 0;

            //m_batch.Begin();
            while (current != -1 && current < text.Length)
            {
                var start = current;
                current = FindWordIndexFromBounds(current, size.X, text, out currentSize);

                if (current > 0)
                {
                    var prevChar = 0;
                    for (int i = start; i < current; i++)
                    {
                        var c = text[i];
                        FontCharacter fc;
                        if (m_characterMap.TryGetValue(c, out fc))
                        {                           
                            var kernKey = ((long)prevChar << 32) + fc.ID;
                            var kernAmount = 0;
                            m_characterKerning.TryGetValue(kernKey, out kernAmount);
                            dx += kernAmount * scale;

                            var uv0 = m_texturePages[fc.Page].GetUV(fc.X, fc.Y);
                            var uv1 = m_texturePages[fc.Page].GetUV(fc.Width, fc.Height);
                            //var sourceRectangle = AxisAlignedBox2.FromRect(fc.X, fc.Y, fc.Width, fc.Height);// AxisAlignedBox2.FromRect(fc.X, fc.Y, fc.Width, fc.Height);
                            var destRectangle = AxisAlignedBox2.FromRect((int)(dx + fc.XOffset * scale), (int)(dy + fc.YOffset * scale), (int)(fc.Width * scale), (int)(fc.Height * scale));

                            //var sourceRectangle = AxisAlignedBox2.FromRect(fc.X, fc.Y, fc.Width, fc.Height);
                            //var destRectangle = AxisAlignedBox2.FromRect(dx + fc.XOffset * scale, dy + fc.YOffset * scale, fc.Width * scale, fc.Height * scale);
                            ////var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                            var sprite = Sprite.Create(m_texturePages[fc.Page], destRectangle.X0, destRectangle.Y1, destRectangle.X1, destRectangle.Y0);
                            sprite.SetDepth(depth);
                            sprite.SetColor(color);
                            sprite.SetTexture(uv0, uv1);
                            rm.AddQuad(sprite);
                            //rm.Draw(renderQueue, _material[fc.Page], destRectangle, sourceRectangle, color, 0f, new Vector2(0f, 0f), SpriteEffects.None, depth);

                            //spriteBatch.Draw(_texture, position, sourceRectangle, Color.White);

                            dx += fc.XAdvance * scale;
                            prevChar = fc.ID;
                        }
                    }

                    dx = (float)Math.Floor(pos.X);
                    dy += MaxLineHeight;
                }

                size.Y += currentSize.Y;
            }
            //m_batch.End();
        }
    }




}