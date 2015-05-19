using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using COG.Assets;
using COG.Framework;
using COG.Logging;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{
    public abstract class Shader : DisposableObject, IAsset<TextData>
    {
        private static readonly Logger g_logger = Logger.GetLogger(typeof(Shader));

        private int m_shaderID;
        private AssetUri m_uri;
        private ShaderType m_shaderType;

        protected Shader(AssetUri uri, TextData data, ShaderType type)
        {
            m_uri = uri;
            m_shaderType = type;
            Reload(data);
        }

        public int ShaderID { get { return m_shaderID; } }
        public AssetUri Uri { get { return m_uri; } }
        public bool IsValid { get { return m_shaderID != 0; } }

        public void Reload(TextData t)
        {
            Destroy();

            m_shaderID = GL.CreateShader(m_shaderType);

            //Console.WriteLine("Compiling shader {0}:{1}", shaderType, path);

            var code = t.Content;
            GL.ShaderSource(m_shaderID, code);
            GL.CompileShader(m_shaderID);

            var result = 0;
            GL.GetShader(m_shaderID, ShaderParameter.CompileStatus, out result);
            if (result != 1)
            {
                var info = GL.GetShaderInfoLog(m_shaderID);
                g_logger.error("Could not compile {0} shader {1} with following error: {2}", m_shaderType, Uri, info);

                Destroy();
            }
        }

        private void Destroy()
        {
            if (m_shaderID != 0)
                GL.DeleteShader(m_shaderID);

            m_shaderID = 0;
        }

        protected override void DisposedUnmanaged()
        {
            base.DisposedUnmanaged();

            Destroy();
        }
    }

    public class VertexShader : Shader
    {
        public static readonly AssetType VERTEX = AssetType.Create("VERTEX");

        public VertexShader(AssetUri uri, TextData data)
            : base(uri, data, ShaderType.VertexShader)
        {

        }
    }

    public class FragmentShader : Shader
    {
        public static readonly AssetType FRAGMENT = AssetType.Create("FRAGMENT");

        public FragmentShader(AssetUri uri, TextData data)
            : base(uri, data, ShaderType.FragmentShader)
        {

        }
    }

    public class ProgramData : DisposableObject, IAssetData
    {
        private Shader[] m_shaders;

        public ProgramData(params Shader[] shaders)
        {
            m_shaders = shaders;
        }

        public Shader[] Shaders { get { return m_shaders; } }

        protected override void DisposedUnmanaged()
        {
            base.DisposedUnmanaged();

            if (m_shaders != null)
            {
                foreach (var shader in m_shaders)
                    shader.Dispose();

                m_shaders = null;
            }
        }
    }

    public class Program : DisposableObject, IAsset<ProgramData>
    {
        public static readonly AssetType PROGRAM = AssetType.Create("PROGRAM");

        private static readonly Logger g_logger = Logger.GetLogger(typeof(Program));
        private static int g_lastProgramBound;

        private int m_programID;
        private AssetUri m_uri;

        public Program(AssetUri uri, ProgramData data)
        {
            m_uri = uri;
            Reload(data);
        }

        public int ProgramID { get { return m_programID; } }
        public AssetUri Uri { get { return m_uri; } }
        public bool IsValid { get { return m_programID != 0; } }
        public void Bind()
        {
            if (!IsValid)
                return;

            if (g_lastProgramBound != m_programID)
            {
                GL.UseProgram(m_programID);
                g_lastProgramBound = m_programID;
            }

            UpdateParams();
        }

        private void UpdateParams()
        {
            
        }

        public void Reload(ProgramData t)
        {
            Destroy();

            m_programID = GL.CreateProgram();
            for(var i =0; i < t.Shaders.Length;++i)
                GL.AttachShader(m_programID, t.Shaders[i].ShaderID);

            GL.LinkProgram(m_programID);

            var result = 0;
            GL.GetProgram(m_programID, GetProgramParameterName.LinkStatus, out result);
            if (result != 1)
            {
                var info = GL.GetProgramInfoLog(m_programID);
                g_logger.error("Failed to link program {0} with error: {1}", Uri, info);
            }
        }

        protected override void DisposedUnmanaged()
        {
            base.DisposedUnmanaged();

            Destroy();
        }

        private void Destroy()
        {
            if (m_programID != 0)
                GL.DeleteProgram(m_programID);

            m_programID = 0;
        }
    }

}
