using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using COG.Assets;
using COG.Framework;
using COG.Logging;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace COG.Graphics
{
    #region Shader
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
    #endregion Shader

    #region VertexShader
    public class VertexShader : Shader
    {
        public static readonly AssetType VERTEX = AssetType.Create("VERTEX");

        public VertexShader(AssetUri uri, TextData data)
            : base(uri, data, ShaderType.VertexShader)
        {

        }
    }
    #endregion VertexShader

    #region FragmentShader
    public class FragmentShader : Shader
    {
        public static readonly AssetType FRAGMENT = AssetType.Create("FRAGMENT");

        public FragmentShader(AssetUri uri, TextData data)
            : base(uri, data, ShaderType.FragmentShader)
        {

        }
    }
    #endregion FragmentShader

    #region ProgramData
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
    #endregion ProgramData

    #region Program
    public class Program : DisposableObject, IAsset<ProgramData>
    {
        #region Variables
        public static readonly AssetType PROGRAM = AssetType.Create("PROGRAM");

        private static readonly Logger g_logger = Logger.GetLogger(typeof(Program));

        internal int i_lastAutoUniform;

        private int m_programID;
        private AssetUri m_uri;
        private List<VertexAttribute> m_attributes = new List<VertexAttribute>();
        private List<VertexUniform> m_uniforms = new List<VertexUniform>();
        private ProgramManager m_programs;
        #endregion

        #region Ctor
        public Program(AssetUri uri, ProgramManager programs, ProgramData data)
        {
            m_uri = uri;
            m_programs = programs;
            m_programs.AddProgram(this);

            Reload(data);
        }
        #endregion

        #region Properties
        public int ProgramID { get { return m_programID; } }

        public AssetUri Uri { get { return m_uri; } }

        public bool IsValid { get { return m_programID != 0; } }

        public IEnumerable<VertexAttribute> Attributes
        {
            get
            {
                foreach (var attr in m_attributes)
                    yield return attr;
            }
        }

        public IEnumerable<VertexUniform> Uniforms
        {
            get
            {
                foreach (var uni in m_uniforms)
                    yield return uni;
            }
        }
        #endregion Properties

        #region Methods
        public void Bind()
        {
            if (!IsValid)
                return;

            m_programs.SetCurrentProgram(this);
        }

        private bool InitializeUniforms()
        {
            int max_uniform_length;
            GL.GetProgram(m_programID, GetProgramParameterName.ActiveUniformMaxLength, out max_uniform_length);
            StringBuilder name = new StringBuilder(max_uniform_length);

            int num_uniforms;
            GL.GetProgram(m_programID, GetProgramParameterName.ActiveUniforms, out num_uniforms);

            for (int index = 0; index < num_uniforms; index++)
            {
                int length, size;
                ActiveUniformType type;
                name.Remove(0, name.Length);
                GL.GetActiveUniform(m_programID, index, name.Capacity, out length, out size, out type, name);

                if (length > 0)
                {
                    var uniformName = name.ToString();
                    int location = GL.GetUniformLocation(m_programID, uniformName);
                    g_logger.info("Uniform: {0}, {1}, {2}", uniformName, location, type);
                    var element = VertexUniform.CreateFromProgram(uniformName, (ushort)location, type);

                    if (element == null)
                    {
                        g_logger.error("{0} failed to process uniforms.", m_uri);
                        return false;
                    }

                    m_uniforms.Add(element);
                }
            }
            return true;
        }

        private bool InitializeAttributes()
        {
            //referernce: http://www.opentk.com/node/3568
            m_attributes.Clear();

            int max_attribute_length;
            GL.GetProgram(m_programID, GetProgramParameterName.ActiveAttributeMaxLength, out max_attribute_length);
            StringBuilder name = new StringBuilder(max_attribute_length);

            int num_attributes;
            GL.GetProgram(m_programID, GetProgramParameterName.ActiveAttributes, out num_attributes);

            for (int index = 0; index < num_attributes; index++)
            {
                int length, size;
                ActiveAttribType type;
                name.Remove(0, name.Length);
                GL.GetActiveAttrib(m_programID, index, name.Capacity, out length, out size, out type, name);

                if (length > 0)
                {
                    var attribName = name.ToString();
                    int location = GL.GetAttribLocation(m_programID, attribName);
                    var attribute = VertexAttribute.CreateFromProgram(attribName, (ushort)location, type);

                    if (attribute == null)
                    {
                        g_logger.error("{0} failed to process attributes.", m_uri);
                        return false;
                    }

                    m_attributes.Add(attribute);
                }
            }

            return true;
        }

        public void Reload(ProgramData t)
        {
            Destroy();

            m_programID = GL.CreateProgram();
            for (var i = 0; i < t.Shaders.Length; ++i)
                GL.AttachShader(m_programID, t.Shaders[i].ShaderID);

            GL.LinkProgram(m_programID);

            var result = 0;
            GL.GetProgram(m_programID, GetProgramParameterName.LinkStatus, out result);
            if (result != 1)
            {
                var info = GL.GetProgramInfoLog(m_programID);
                g_logger.error("Failed to link program {0} with error: {1}", Uri, info);
                Destroy();
            }
            else
            {
                if (!InitializeAttributes())
                {
                    Destroy();
                    return;
                }

                if (!InitializeUniforms())
                {
                    Destroy();
                    return;
                }
            }
        }

        public VertexAttribute FindAttributeBySemantic(VertexElementSemantic semantic)
        {
            foreach (var attr in m_attributes)
                if (attr.Semantic == semantic)
                    return attr;

            return null;
        }

        private VertexUniform GetUniform(string name)
        {
            for (var i = 0; i < m_uniforms.Count; i++)
            {
                if (m_uniforms[i].Name == name)
                    return m_uniforms[i];
            }

            return null;
        }

        public bool SetUniformMatrix4(string name, Matrix4 matrix)
        {
            var uniform = GetUniform(name);
            if (uniform == null)
            {
                g_logger.warn("{0} uniform {1} doesn't exist.", m_uri, name);
                return false;
            }

            if (uniform.Semantic != VertexUniformSemantic.Matrix4)
            {
                g_logger.warn("{0} uniform {1} is not a Matrix4.", m_uri, name);
                return false;
            }

            this.Bind();

            GL.UniformMatrix4(uniform.Location, false, ref matrix);
            return true;
        }
        #endregion Methods

        #region Disposing
        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            m_programs.RemoveProgram(this);
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

            m_attributes.Clear();
            m_uniforms.Clear();
            m_programID = 0;
        }
        #endregion Disposing
    }
    #endregion Program

    public class ProgramManager : DisposableObject
    {
        internal int i_lastUpdate;
        private AssetManager m_assets;
        private Program m_currentProgram;
        private List<Program> m_programs = new List<Program>();
        private Dictionary<string, AutoVertexUniform> m_uniforms = new Dictionary<string, AutoVertexUniform>();

        public Program CurrentProgram { get { return m_currentProgram; } }

        public ProgramManager(AssetManager assets)
        {
            m_assets = assets;
        }

        internal void AddProgram(Program program)
        {
            m_programs.Add(program);
        }

        internal void RemoveProgram(Program program)
        {
            for (int i = m_programs.Count - 1; i >= 0; i--)
            {
                if (m_programs[i].ProgramID == program.ProgramID)
                {
                    m_programs.RemoveAt(i);
                    return;
                }
            }
        }

        internal void SetCurrentProgram(Program program)
        {
            if (program == null)
                GL.UseProgram(0);

            if (program)
            {
                if (m_currentProgram == null || m_currentProgram.ProgramID != program.ProgramID)
                {
                    m_currentProgram = program;
                    GL.UseProgram(program.ProgramID);
                }

                if (i_lastUpdate != program.i_lastAutoUniform)
                {
                    //auto sync params
                    foreach (var uni in program.Uniforms)
                    {
                        AutoVertexUniform autoUni;
                        if (m_uniforms.TryGetValue(uni.Name, out autoUni))
                            autoUni.Update(uni);
                    }

                    program.i_lastAutoUniform = i_lastUpdate;
                }
            }
            else
                m_currentProgram = null;

        }

        public AutoVertexUniformMatrix4 CreateAutoUniformMatrix4(string name)
        {
            var uniform = new AutoVertexUniformMatrix4(this, name);
            m_uniforms.Add(name, uniform);

            i_lastUpdate++;

            return uniform;
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            for (int i = m_programs.Count - 1; i >= 0; i--)
                m_programs[i].Dispose();
        }

    }

    public abstract class AutoVertexUniform
    {
        protected ProgramManager m_programs;
        protected string m_name;

        protected AutoVertexUniform(ProgramManager programs, string name)
        {
            m_programs = programs;
            m_name = name;
        }

        public abstract void Update(VertexUniform uniform);
    }

    public abstract class AutoVertexUniform<T> : AutoVertexUniform
        where T : IEquatable<T>
    {
        protected T m_value;

        protected AutoVertexUniform(ProgramManager programs, string name)
            : base(programs, name)
        {

        }

        public void SetValue(T t)
        {
            if (!t.Equals(m_value))
            {
                m_programs.i_lastUpdate++;
                m_value = t;
            }
        }
    }

    public class AutoVertexUniformMatrix4 : AutoVertexUniform<Matrix4>
    {
        private bool m_transpose = false;

        internal AutoVertexUniformMatrix4(ProgramManager programs, string name)
            : base(programs, name)
        {

        }

        public override void Update(VertexUniform uniform)
        {
            GL.UniformMatrix4(uniform.Location, m_transpose, ref  m_value);
        }
    }

}
