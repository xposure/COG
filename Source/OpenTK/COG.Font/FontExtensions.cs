using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Atma;
using COG.Assets;
using COG.Graphics;
using OpenTK;

public static class FontExtensions
{
    public static Font LoadFont(this AssetManager m_assets, AssetUri uri)
    {
        return m_assets.LoadAsset<Font, FontData>(uri);
    }

    public static void InitializeFonts(this AssetManager assets)
    {
        assets.RegisterTypeExtension<FontData>(Font.FONT, "fnt", LoadFontData);
        assets.SetFactory<FontData, Font>(Font.FONT, new Func<AssetUri, FontData, Font>((uri, data) => { return FontFactory(uri, data, assets); }));
    }

    private static Font FontFactory(AssetUri uri, FontData data, AssetManager assets)
    {
        return new Font(uri, assets, data);
    }

    public static FontData LoadFontData(Stream stream)
    {
        var deserializer = new XmlSerializer(typeof(FontData));
        var file = (FontData)deserializer.Deserialize(stream);
        return file;
    }

    public static void DrawText(this SpriteRenderer rm, Font font, Vector2 pos, string text, Color? color = null, float depth = 0f, int renderQueue = 0, float scale = 1f)
    {
        rm.DrawText(font, scale, pos, text, color ?? Color.White, depth);
    }

    public static void DrawText(this SpriteRenderer rm, Font font, float scale, Vector2 pos, string text, Color color, float depth)
    {
        font.DrawText(rm, pos, scale, text, color, depth);
    }

    public static void DrawText(this SpriteRenderer rm, Font font, Vector2 pos, float scale, string text, Color color, float depth)
    {
        font.DrawText(rm, pos, scale, text, color, depth, null);
    }

    public static void DrawText(this SpriteRenderer rm, Font font, Vector2 pos, float scale, string text, Color color, float depth, float? width)
    {
        font.DrawText(rm, pos, scale, text, color, depth, width);
    }

    public static void DrawWrappedOnWordText(this SpriteRenderer rm, Font font, Vector2 pos, float scale, string text, Color color, float depth, Vector2 size)
    {
        font.DrawWrappedOnWordText(rm, pos, scale, text, color, depth, size);
    }
}
