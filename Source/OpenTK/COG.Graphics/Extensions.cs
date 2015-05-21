using System;
using COG.Assets;
using COG.Graphics;

public static class GraphicExtensions
{
    public static void InitializeGraphics(this AssetManager m_assets, string module)
    {
        m_assets.RegisterTypeExtension(Texture2D.TEXTURE, "bmp", TextureData2D.LoadBitmap);
        m_assets.RegisterTypeExtension(Texture2D.TEXTURE, "png", TextureData2D.LoadPng);
        m_assets.SetFactory<TextureData2D, Texture2D>(Texture2D.TEXTURE, Texture2DFactory);
    }

    public static ProgramManager InitializePrograms(this AssetManager m_assets, string module)
    {
        var programs = new ProgramManager(m_assets);

        m_assets.RegisterTypeExtension(VertexShader.VERTEX, "vert", TextData.LoadData);
        m_assets.RegisterTypeExtension(FragmentShader.FRAGMENT, "frag", TextData.LoadData);

        m_assets.AddResolver<ProgramData>(Program.PROGRAM, new Func<AssetUri, ProgramData>(uri => { return m_assets.ResolveProgramData(uri, module); }));
        m_assets.SetFactory<ProgramData, Program>(Program.PROGRAM, new Func<AssetUri, ProgramData, Program>((uri, data) => { return ProgramFactory(uri, programs, data); }));

        m_assets.SetFactory<TextData, VertexShader>(VertexShader.VERTEX, VertexFactory);
        m_assets.SetFactory<TextData, FragmentShader>(FragmentShader.FRAGMENT, FragmentFactory);

        return programs;
    }

    #region Resolvers
    private static ProgramData ResolveProgramData(this AssetManager m_assets, AssetUri uri, string module)
    {
        var vertexShader = m_assets.LoadAsset<VertexShader, TextData>(VertexShader.VERTEX.CreateUri(module, uri.Name));
        var fragmentShader = m_assets.LoadAsset<FragmentShader, TextData>(FragmentShader.FRAGMENT.CreateUri(module, uri.Name));

        return new ProgramData(vertexShader, fragmentShader);
    }
    #endregion

    #region Factories
    private static Texture2D Texture2DFactory(AssetUri uri, TextureData2D data)
    {
        return new Texture2D(uri, data);
    }

    private static VertexShader VertexFactory(AssetUri uri, TextData data)
    {
        return new VertexShader(uri, data);
    }

    private static FragmentShader FragmentFactory(AssetUri uri, TextData data)
    {
        return new FragmentShader(uri, data);
    }

    private static Program ProgramFactory(AssetUri uri, ProgramManager programs, ProgramData data)
    {
        using (data)
        {
            return new Program(uri, programs, data);
        }
    }
    #endregion

    public static Texture2D LoadTexture(this AssetManager m_assets, AssetUri uri)
    {
        return m_assets.LoadAsset<Texture2D, TextureData2D>(uri);
    }

    public static Program LoadProgram(this AssetManager m_assets, AssetUri uri)
    {
        return m_assets.LoadAsset<Program, ProgramData>(uri);
    }
}
